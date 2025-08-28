#import <Foundation/Foundation.h>
#include <iostream>
#include "bitboard.h"
#include "position.h"
#include "search.h"
#include "thread.h"
#include "tt.h"
#include "uci.h"
#include <sstream>
#include <cstdlib>
#include <cstring>

namespace StockfishBridge {
struct Queues {
    std::mutex m;
    std::condition_variable cv_in;
    std::queue<std::string> in_lines;
    std::queue<std::string> out_lines;
    std::atomic<bool> running{false};
};

class InBuf : public std::streambuf {
public:
    explicit InBuf(Queues& q) : q_(q) {}
protected:
    int_type underflow() override {
        if (!q_.running.load()) return traits_type::eof();
        if (gptr() && gptr() < egptr()) return traits_type::to_int_type(*gptr());

        std::unique_lock<std::mutex> lk(q_.m);
        q_.cv_in.wait(lk, [&]{ return !q_.in_lines.empty() || !q_.running.load(); });
        if (!q_.running.load() && q_.in_lines.empty()) return traits_type::eof();

        curr_ = std::move(q_.in_lines.front()); q_.in_lines.pop();
        lk.unlock();

        curr_.push_back('\n');
        buf_.assign(curr_.begin(), curr_.end());

        setg(buf_.data(), buf_.data(), buf_.data() + buf_.size());
        return traits_type::to_int_type(*gptr());
    }
private:
    Queues& q_;
    std::vector<char> buf_;
    std::string curr_;
};

class OutBuf : public std::streambuf {
public:
    explicit OutBuf(Queues& q) : q_(q) {}

protected:
    // Write a single character
    int_type overflow(int_type ch) override {
        if (ch == traits_type::eof()) {
            flush_pending(/*force=*/true);
            return traits_type::not_eof(ch);
        }
        char c = traits_type::to_char_type(ch);
        if (c == '\n') {
            // emit a full line
            enqueue_line(pending_);
            pending_.clear();
        } else {
            pending_.push_back(c);
            // optional safety: avoid unbounded growth if engine outputs no '\n'
            if (pending_.size() >= kMaxPending) {
                enqueue_line(pending_);
                pending_.clear();
            }
        }
        return ch;
    }

    // Write a block
    std::streamsize xsputn(const char* s, std::streamsize n) override {
        if (!s || n <= 0) return 0;
        std::streamsize written = 0;
        const char* cur = s;
        const char* end = s + n;
        while (cur < end) {
            const char* nl = static_cast<const char*>(memchr(cur, '\n', static_cast<size_t>(end - cur)));
            if (!nl) {
                // no newline in the remainder
                pending_.append(cur, static_cast<size_t>(end - cur));
                written += (end - cur);
                break;
            }
            // append up to newline, emit line, continue
            pending_.append(cur, static_cast<size_t>(nl - cur));
            enqueue_line(pending_);
            pending_.clear();
            written += (nl - cur) + 1;
            cur = nl + 1;
        }
        // Safety cap (same as overflow)
        if (pending_.size() >= kMaxPending) {
            enqueue_line(pending_);
            pending_.clear();
        }
        return written;
    }

    int sync() override {
        flush_pending(/*force=*/true);
        return 0;
    }

private:
    static constexpr size_t kMaxPending = 4096;

    void enqueue_line(const std::string& line) {
        std::lock_guard<std::mutex> lk(q_.m);
        q_.out_lines.emplace(line);
    }

    void flush_pending(bool force) {
        if (force && !pending_.empty()) {
            enqueue_line(pending_);
            pending_.clear();
        }
    }

    Queues& q_;
    std::string pending_;
};

struct RdbufGuard {
    std::streambuf* cin_old{nullptr};
    std::streambuf* cout_old{nullptr};
    ~RdbufGuard() {
        if (cin_old) std::cin.rdbuf(cin_old);
        if (cout_old) std::cout.rdbuf(cout_old);
    }
};

static std::string FindNNUEPathInBundle() {
    @autoreleasepool {
        NSBundle *bundle = [NSBundle mainBundle];
        NSString *resPath = [bundle resourcePath];
        NSFileManager *fm = [NSFileManager defaultManager];

        // Unity copies StreamingAssets -> Data/Raw
        NSString *raw = [resPath stringByAppendingPathComponent:@"Data/Raw"];
        NSArray<NSString*> *paths = @[resPath, raw];
        for (NSString *root in paths) {
            NSArray<NSString*> *contents = [fm contentsOfDirectoryAtPath:root error:nil];
            for (NSString *name in contents) {
                if ([[name pathExtension].lowercaseString isEqualToString:@"nnue"]) {
                    NSString *full = [root stringByAppendingPathComponent:name];
                    return std::string([full UTF8String]);
                }
            }
        }
    }
    return {};
}

static void Trace(Queues& q, const char* msg) {
    std::lock_guard<std::mutex> lk(q.m);
    q.out_lines.emplace(std::string("info string [bridge] ") + msg);
}

struct Engine {
    Queues q;
    std::thread t;
    
    void start() {
        if (q.running.exchange(true)) return;
        t = std::thread([this] {
            @autoreleasepool {
                Trace(q, "thread start");
                
                Stockfish::StateListPtr states(new std::deque<Stockfish::StateInfo>(1));
                Stockfish::Bitboards::init();
                Stockfish::Position::init();
                
                InBuf inbuf(q);
                OutBuf outbuf(q);
                std::istream in(&inbuf);
                std::ostream out(&outbuf);
                
                static char argv0_storage[] = "stockfish";
                static char* argv_vec[] = { argv0_storage, nullptr };
                int argc = 1;
                Stockfish::UCIEngine* enginePtr = nullptr;
                try {
                    enginePtr = new Stockfish::UCIEngine(argc, argv_vec);
                } catch (...) {
                    Trace(q, "UCIEngine ctor threw");
                    q.running.store(false);
                    return;
                }
                
                Trace(q, "engine constructed");
                q.running.store(true);
                
                RdbufGuard guard;
                guard.cin_old  = std::cin.rdbuf(in.rdbuf());
                guard.cout_old = std::cout.rdbuf(out.rdbuf());
                
//                {
//                    std::string nn = FindNNUEPathInBundle();
//                    if (!nn.empty()) {
//                        Trace(q, "nn found");
//                        Trace(q, nn.c_str());
//                        std::lock_guard<std::mutex> lk(q.m);
//                        q.in_lines.emplace(std::string("setoption name EvalFile value ") + nn);
//                    }
//                }
                q.cv_in.notify_all();
                Trace(q, "pre-loop commands queued");
                
                try { enginePtr->loop(); }
                catch (...) { Trace(q, "engine loop threw"); }
                
                delete enginePtr;
                Trace(q, "engine destroyed");
                q.running.store(false);
            }
        });
    }
    
    void stop() {
        if (!q.running.exchange(false)) return;
        { std::lock_guard<std::mutex> lk(q.m); q.in_lines.emplace("quit"); }
        q.cv_in.notify_all();
        if (t.joinable()) t.join();
        std::queue<std::string> e1, e2;
        std::swap(q.in_lines, e1);
        std::swap(q.out_lines, e2);
    }

    void send(const char* line) {
        if (!line || !*line) return;
       { std::lock_guard<std::mutex> lk(q.m); q.in_lines.emplace(line); }
       q.cv_in.notify_all();
    }

    const char* poll() {
        std::lock_guard<std::mutex> lk(q.m);
        if (q.out_lines.empty()) return nullptr;
        static std::string last;
        last = std::move(q.out_lines.front()); q.out_lines.pop();
        return last.c_str();
    }
};

static Engine* engine = nullptr;
}

extern "C" {
void _stockfish_start() {
    using namespace StockfishBridge;
    if (engine) return;
    engine = new Engine();
    engine->start();
}

void _stockfish_stop() {
    using namespace StockfishBridge;
    if (!engine) return;
    engine->stop();
    delete engine;
    engine = nullptr;
}

void _stockfish_send(const char* cmd) {
    using namespace StockfishBridge;
    if (!engine) return;
    engine->send(cmd);
}

int _stockfish_read_copy(char* dst, int maxLen) {
    using namespace StockfishBridge;
    if (!engine || !dst || maxLen <= 1) return 0;
    std::string line;
    { std::lock_guard<std::mutex> lk(engine->q.m);
      if (engine->q.out_lines.empty()) return 0;
      line = std::move(engine->q.out_lines.front());
      engine->q.out_lines.pop();
    }
    int n = (int)std::min<size_t>((size_t)maxLen - 1, line.size());
    if (n > 0) std::memcpy(dst, line.data(), n);
    dst[n] = '\0';
    return n;
}
}
