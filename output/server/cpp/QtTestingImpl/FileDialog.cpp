#include "generated/FileDialog.h"

#include <QFileDialog>

#include "util/SignalStuff.h"

#define THIS ((FileDialogWithHandler*)_this)

namespace FileDialog
{
    class FileDialogWithHandler : public QFileDialog {
        Q_OBJECT
    private:
        std::shared_ptr<SignalHandler> handler;
        uint32_t lastMask = 0;
        std::vector<SignalMapItem<SignalMask>> signalMap = {
            // from Dialog:
            { SignalMask::Accepted, SIGNAL(accepted()), SLOT(onAccepted()) },
            { SignalMask::Finished, SIGNAL(finished(int)), SLOT(onFinished(int)) },
            { SignalMask::Rejected, SIGNAL(rejected()), SLOT(onRejected()) },
            // FileDialog:
            { SignalMask::CurrentChanged, SIGNAL(currentChanged(QString)), SLOT(onCurrentChanged(QString)) },
            { SignalMask::CurrentUrlChanged, SIGNAL(currentUrlChanged(QUrl)), SLOT(onCurrentUrlChanged(QUrl)) },
            { SignalMask::DirectoryEntered, SIGNAL(directoryEntered(QString)), SLOT(onDirectoryEntered(QString)) },
            { SignalMask::DirectoryUrlEntered, SIGNAL(directoryUrlEntered(QUrl)), SLOT(onDirectoryUrlEntered(QUrl)) },
            { SignalMask::FileSelected, SIGNAL(fileSelected(QString)), SLOT(onFileSelected(QString)) },
            { SignalMask::FilesSelected, SIGNAL(filesSelected(QStringList)), SLOT(onFilesSelected(QStringList)) },
            { SignalMask::FilterSelected, SIGNAL(filterSelected(QString)), SLOT(onFilterSelected(QString)) },
            { SignalMask::UrlSelected, SIGNAL(urlSelected(QUrl)), SLOT(onUrlSelected(QUrl)) },
            { SignalMask::UrlsSelected, SIGNAL(urlsSelected(QList<QUrl>)), SLOT(onUrlsSelected(QList<QUrl>)) },
        };
    public:
        explicit FileDialogWithHandler(const std::shared_ptr<SignalHandler> &handler)
            : handler(handler) {}
        void setSignalMask(uint32_t newMask) {
            if (newMask != lastMask) {
                processChanges(lastMask, newMask, signalMap, this);
                lastMask = newMask;
            }
        }
    public slots:
        void onAccepted() {
            handler->accepted();
        };
        void onFinished(int result) {
            handler->finished(result);
        }
        void onRejected() {
            handler->rejected();
        }
        void onCurrentChanged(const QString &path) {
            handler->currentChanged(path.toStdString());
        }
        void onCurrentUrlChanged(const QUrl &url) {
            handler->currentUrlChanged(url.toString().toStdString());
        }
        void onDirectoryEntered(const QString &directory) {
            handler->directoryEntered(directory.toStdString());
        }
        void onDirectoryUrlEntered(const QUrl &directory) {
            handler->directoryUrlEntered(directory.toString().toStdString());
        }
        void onFileSelected(const QString &file) {
            handler->fileSelected(file.toStdString());
        }
        void onFilesSelected(const QStringList &selected) {
            std::vector<std::string> selected2;
            for (auto &str : selected) {
                selected2.push_back(str.toStdString());
            }
            handler->filesSelected(selected2);
        }
        void onFilterSelected(const QString &filter) {
            handler->filterSelected(filter.toStdString());
        }
        void onUrlSelected(const QUrl &url) {
            handler->urlSelected(url.toString().toStdString());
        }
        void onUrlsSelected(const QList<QUrl> &urls) {
            std::vector<std::string> urls2;
            for (auto &url: urls) {
                urls2.push_back(url.toString().toStdString());
            }
            handler->urlsSelected(urls2);
        }
    };

    void Handle_setAcceptMode(HandleRef _this, AcceptMode mode) {
        THIS->setAcceptMode((QFileDialog::AcceptMode)mode);
    }

    void Handle_setFileMode(HandleRef _this, FileMode mode) {
        THIS->setFileMode((QFileDialog::FileMode)mode);
    }

    void Handle_setNameFilter(HandleRef _this, std::string filter) {
        THIS->setNameFilter(QString::fromStdString(filter));
    }

    void Handle_setNameFilters(HandleRef _this, std::vector<std::string> filters) {
        QStringList qFilters;
        for (auto &filter: filters) {
            qFilters.append(QString::fromStdString(filter));
        }
        THIS->setNameFilters(qFilters);
    }

    void Handle_setMimeTypeFilters(HandleRef _this, std::vector<std::string> filters) {
        QStringList qFilters;
        for (auto &filter: filters) {
            qFilters.append(QString::fromStdString(filter));
        }
        THIS->setMimeTypeFilters(qFilters);
    }

    void Handle_setViewMode(HandleRef _this, ViewMode mode) {
        THIS->setViewMode((QFileDialog::ViewMode)mode);
    }

    void Handle_setDefaultSuffix(HandleRef _this, std::string suffix) {
        THIS->setDefaultSuffix(QString::fromStdString(suffix));
    }

    void Handle_setDirectory(HandleRef _this, std::string dir) {
        THIS->setDirectory(QString::fromStdString(dir));
    }

    void Handle_selectFile(HandleRef _this, std::string file) {
        THIS->selectFile(QString::fromStdString(file));
    }

    std::vector<std::string> Handle_selectedFiles(HandleRef _this) {
        std::vector<std::string> ret;
        for (auto &file : THIS->selectedFiles()) {
            ret.push_back(file.toStdString());
        }
        return ret;
    }

    void Handle_setSignalMask(HandleRef _this, uint32_t mask) {
        THIS->setSignalMask(mask);
    }

    void Handle_dispose(HandleRef _this) {
        delete THIS;
    }

    HandleRef create(std::shared_ptr<SignalHandler> handler) {
        return (HandleRef) new FileDialogWithHandler(handler);
    }
}

#include "FileDialog.moc"
