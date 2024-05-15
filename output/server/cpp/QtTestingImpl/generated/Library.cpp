#include "Application_wrappers.h"
#include "Common_wrappers.h"
#include "Signal_wrappers.h"
#include "Layout_wrappers.h"
#include "Widget_wrappers.h"
#include "BoxLayout_wrappers.h"
#include "ComboBox_wrappers.h"
#include "LineEdit_wrappers.h"
#include "MainWindow_wrappers.h"
#include "PlainTextEdit_wrappers.h"
#include "PushButton_wrappers.h"

extern "C" int nativeLibraryInit() {
    ::Application::__register();
    ::Common::__register();
    ::Signal::__register();
    ::Layout::__register();
    ::Widget::__register();
    ::BoxLayout::__register();
    ::ComboBox::__register();
    ::LineEdit::__register();
    ::MainWindow::__register();
    ::PlainTextEdit::__register();
    ::PushButton::__register();
    // should we do module inits here as well?
    // currently they are manually done on the C# side inside the <module>.Init methods (which perform registration first) - and those are individually called by Library.Init, which first calls nativeImplInit
    return 0;
}

extern "C" void nativeLibraryShutdown() {
    // module shutdowns? see note above
}
