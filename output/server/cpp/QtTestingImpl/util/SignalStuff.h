#pragma once

#include <QObject>
#include <map>

enum SignalState {
    Added,
    Removed,
    Unchanged
};

template <typename T>
std::map<T, SignalState> getSignalChanges(uint32_t oldMask, uint32_t newMask, int signalCount) {
    std::map<T, SignalState> result;
    uint32_t checkSignal = 1;
    for (int i = 0; i< signalCount; i++, checkSignal <<= 1) {
        auto lastState = ((oldMask & checkSignal) != 0);
        auto thisState = ((newMask & checkSignal) != 0);
        SignalState value;
        if (thisState && !lastState) {
            value = Added;
        } else if (lastState && !thisState) {
            value = Removed;
        } else {
            value = Unchanged;
        }
        result[(T)checkSignal] = value;
    }
    return result;
}

template <typename T>
struct SignalMapItem {
    T sig;
    const char *signal;
    const char *slot;
};

template <typename T>
void processChanges(uint32_t lastMask, uint32_t newMask, std::vector<SignalMapItem<T>> mapItems, QObject* thisPtr) {
    auto changes = getSignalChanges<T>(lastMask, newMask, (int)mapItems.size());

    for (auto item : mapItems) {
        auto change = changes[item.sig];
        if (change == Added) {
            QObject::connect(thisPtr, item.signal, thisPtr, item.slot);
        } else if (change == Removed) {
            QObject::disconnect(thisPtr, item.signal, thisPtr, item.slot);
        }
    }
}
