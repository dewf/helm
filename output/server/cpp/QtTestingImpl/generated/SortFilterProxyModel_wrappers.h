#pragma once
#include "SortFilterProxyModel.h"

namespace SortFilterProxyModel
{

    void SignalMask__push(SignalMask value);
    SignalMask SignalMask__pop();

    void SignalHandler__push(std::shared_ptr<SignalHandler> inst, bool isReturn);
    std::shared_ptr<SignalHandler> SignalHandler__pop();

    void SignalHandler_destroyed__wrapper(int serverID);

    void SignalHandler_objectNameChanged__wrapper(int serverID);

    void SignalHandler_columnsAboutToBeInserted__wrapper(int serverID);

    void SignalHandler_columnsAboutToBeMoved__wrapper(int serverID);

    void SignalHandler_columnsAboutToBeRemoved__wrapper(int serverID);

    void SignalHandler_columnsInserted__wrapper(int serverID);

    void SignalHandler_columnsMoved__wrapper(int serverID);

    void SignalHandler_columnsRemoved__wrapper(int serverID);

    void SignalHandler_dataChanged__wrapper(int serverID);

    void SignalHandler_headerDataChanged__wrapper(int serverID);

    void SignalHandler_layoutAboutToBeChanged__wrapper(int serverID);

    void SignalHandler_layoutChanged__wrapper(int serverID);

    void SignalHandler_modelAboutToBeReset__wrapper(int serverID);

    void SignalHandler_modelReset__wrapper(int serverID);

    void SignalHandler_rowsAboutToBeInserted__wrapper(int serverID);

    void SignalHandler_rowsAboutToBeMoved__wrapper(int serverID);

    void SignalHandler_rowsAboutToBeRemoved__wrapper(int serverID);

    void SignalHandler_rowsInserted__wrapper(int serverID);

    void SignalHandler_rowsMoved__wrapper(int serverID);

    void SignalHandler_rowsRemoved__wrapper(int serverID);

    void SignalHandler_sourceModelChanged__wrapper(int serverID);

    void SignalHandler_autoAcceptChildRowsChanged__wrapper(int serverID);

    void SignalHandler_filterCaseSensitivityChanged__wrapper(int serverID);

    void SignalHandler_filterRoleChanged__wrapper(int serverID);

    void SignalHandler_recursiveFilteringEnabledChanged__wrapper(int serverID);

    void SignalHandler_sortCaseSensitivityChanged__wrapper(int serverID);

    void SignalHandler_sortLocaleAwareChanged__wrapper(int serverID);

    void SignalHandler_sortRoleChanged__wrapper(int serverID);

    void Handle__push(HandleRef value);
    HandleRef Handle__pop();

    void Handle_setAutoAcceptChildRows__wrapper();

    void Handle_setDynamicSortFilter__wrapper();

    void Handle_setFilterCaseSensitivity__wrapper();

    void Handle_setFilterKeyColumn__wrapper();

    void Handle_setFilterRegularExpression__wrapper();

    void Handle_setFilterRole__wrapper();

    void Handle_setSortLocaleAware__wrapper();

    void Handle_setRecursiveFilteringEnabled__wrapper();

    void Handle_setSortCaseSensitivity__wrapper();

    void Handle_setSortRole__wrapper();

    void Handle_setSignalMask__wrapper();

    void Handle_dispose__wrapper();

    void create__wrapper();

    int __register();
}
