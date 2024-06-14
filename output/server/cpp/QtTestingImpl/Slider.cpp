#include "generated/Slider.h"

#include <QSlider>
#include <utility>

#include "util/SignalStuff.h"

#define THIS ((SliderWithHandler*)_this)

namespace Slider
{
    class SliderWithHandler : public QSlider {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
            { SignalMask::ActionTriggered, SIGNAL(actionTriggered(int)), SLOT(onActionTriggered(int)) },
            { SignalMask::RangeChanged, SIGNAL(rangeChanged(int, int)), SLOT(onRangeChanged(int,int)) },
            { SignalMask::SliderMoved, SIGNAL(sliderMoved(int)), SLOT(onSliderMoved(int)) },
            { SignalMask::SliderPressed, SIGNAL(sliderPressed()), SLOT(onSliderPressed()) },
            { SignalMask::SliderReleased, SIGNAL(sliderReleased()), SLOT(onSliderReleased()) },
            { SignalMask::ValueChanged, SIGNAL(valueChanged(int)), SLOT(onValueChanged(int)) },
        };
    public:
        explicit SliderWithHandler(std::shared_ptr<SignalHandler> handler) : handler(std::move(handler)) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onActionTriggered(int action) {
            handler->actionTriggered((Slider::SliderAction)action);
        };
        void onRangeChanged(int min, int max) {
            handler->rangeChanged(min, max);
        }
        void onSliderMoved(int value) {
            handler->sliderMoved(value);
        }
        void onSliderPressed() {
            handler->sliderPressed();
        }
        void onSliderReleased() {
            handler->sliderReleased();
        }
        void onValueChanged(int value) {
            handler->valueChanged(value);
        }
    };

    void Handle_setTickPosition(HandleRef _this, TickPosition tpos) {
        THIS->setTickPosition((QSlider::TickPosition)tpos);
    }

    void Handle_setTickInterval(HandleRef _this, int32_t interval) {
        THIS->setTickInterval(interval);
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new SliderWithHandler(std::move(handler));
    }
}

#include "Slider.moc"
