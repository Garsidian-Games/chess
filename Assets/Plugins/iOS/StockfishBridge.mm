#import <Foundation/Foundation.h>
#include <iostream>
#include "bitboard.h"
#include "position.h"
#include "search.h"
#include "thread.h"
#include "tt.h"
#include "uci.h"
#include <sstream>

namespace StockfishBridge {
struct Engine {
    void start() {
    }
    
    void stop() {
    }

    void send(const char* line) {
    }

    const char* poll() {
        return nullptr;
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

const char* _stockfish_read() {
    using namespace StockfishBridge;
    if (!engine) return nullptr;
    return engine->poll();
}
}
