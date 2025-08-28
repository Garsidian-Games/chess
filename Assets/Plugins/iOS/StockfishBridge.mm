#import <Foundation/Foundation.h>
#import <objc/message.h>
#include <atomic>
#include <mutex>
#include <string>

// ---- Minimal protocol & class forward-decls to avoid importing library headers
@protocol TalksToFish
- (void)info:(NSString*)line;
- (void)bestmove:(NSString*)line;
@end

@interface Stockfish : NSObject
- (instancetype)initWithTalksTo:(id)delegate;
- (void)send:(NSString*)uci;
@end
// ------------------------------------------------------------------------------

static id g_engine = nil;
static std::atomic_bool g_isReady{false};
static std::string g_lastBest;
static std::atomic_bool g_hasBest{false};
static std::mutex g_mtx;

// Delegate that receives callbacks from the library
@interface SFUnityDelegate : NSObject <TalksToFish>
@end

@implementation SFUnityDelegate
- (void)info:(NSString*)line {
    NSLog(@"SF info: %@", line);
    if ([line rangeOfString:@"readyok"].location != NSNotFound) {
        g_isReady.store(true, std::memory_order_release);
    }
}
- (void)bestmove:(NSString*)line {
    std::lock_guard<std::mutex> lk(g_mtx);
    g_lastBest = [line UTF8String];   // e.g. "bestmove e2e4 ponder ..."
    g_hasBest.store(true, std::memory_order_release);
}
@end

static SFUnityDelegate* g_delegate = nil;

static void sf_internal_init() {
    if (g_engine) return;
    g_delegate = [SFUnityDelegate new];
    Class cls = NSClassFromString(@"Stockfish");
    if (cls) {
        // Stockfish *engine = [[Stockfish alloc] initWithTalksTo:g_delegate];
        SEL sel = NSSelectorFromString(@"initWithTalksTo:");
        id obj = [cls alloc];
        if ([obj respondsToSelector:sel]) {
            g_engine = ((id (*)(id, SEL, id))objc_msgSend)(obj, sel, g_delegate);
        }
    }
}

static void sf_send_objc(NSString* cmd) {
    if (!g_engine) sf_internal_init();
    if ([g_engine respondsToSelector:@selector(send:)]) {
        [g_engine performSelector:@selector(send:) withObject:cmd];
    }
}

extern "C" {

// Initialize the engine (safe to call multiple times)
void sf_init() { sf_internal_init(); }

// Send any UCI command string
void sf_send(const char* uci) {
    sf_send_objc([NSString stringWithUTF8String:uci]);
}

void sf_clear_ready() {
    g_isReady.store(false, std::memory_order_release);
}

int sf_is_ready() {
    return g_isReady.load(std::memory_order_acquire) ? 1 : 0;
}

// Polling bestmove result
int  sf_has_bestmove() {
    return g_hasBest.load(std::memory_order_acquire) ? 1 : 0;
}

const char* sf_take_bestmove() {
    if (!g_hasBest.load(std::memory_order_acquire)) return "";
    std::lock_guard<std::mutex> lk(g_mtx);
    g_hasBest.store(false, std::memory_order_release);
    static thread_local char out[128];
    // Expect formats like "bestmove e2e4 ..." — return just the move token
    // Find the first space and copy the second token
    std::string s = g_lastBest;
    const char* p = s.c_str();
    const char* sp = strchr(p, ' ');
    const char* mv = sp ? sp + 1 : p; // fallback
    // Trim trailing after space
    size_t n = strcspn(mv, " \t\r\n");
    n = n < sizeof(out)-1 ? n : sizeof(out)-1;
    memcpy(out, mv, n); out[n] = 0;
    return out;
}

} // extern "C"

