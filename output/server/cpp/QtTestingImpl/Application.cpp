#include "generated/Application.h"

#include <QApplication>

#define THIS ((QApplication*)_this)

namespace Application
{
    int32_t Handle_exec(HandleRef _this) {
        return THIS->exec();
    }

    void Handle_setStyle(HandleRef _this, std::string name) {
        THIS->setStyle(name.c_str());
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
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
