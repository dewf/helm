#include "generated/Application.h"

#include <QApplication>

namespace Application
{
    int32_t Handle_exec(HandleRef _this) {
        return ((QApplication*)_this)->exec();
    }

    void Handle_dispose(HandleRef _this) {
        delete (QApplication*)_this;
    }

    HandleRef create(std::vector<std::string> args) {
        auto argc = (int)args.size();
        auto argv = new char*[argc];
        for (int i = 0; i< argc; i++) {
            argv[i] = const_cast<char*>(args[i].c_str());
        }
        auto ret = new QApplication(argc, argv);
        delete[] argv;
        return (HandleRef)ret;
    }
}
