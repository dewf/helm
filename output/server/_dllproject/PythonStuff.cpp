#define PY_SSIZE_T_CLEAN
#include <Python.h>

#include "../../../../core/NativeImplCore.h"

// exception to use later
static PyObject* TestLibraryError;

static PyObject* pyClientFuncExec;
static PyObject* pyClientFuncRelease;
static PyObject* pyClientMethodExec;
static PyObject* pyClientObjectRelease;
static PyObject* pyClientClearSafetyArea;

static unsigned long pyMainThreadId = 0;
static bool gilLocked = false;
static PyGILState_STATE gstate;

static inline void maybeLockGIL() {
    if (PyThread_get_thread_native_id() != pyMainThreadId) { // also try with std::thread stuff if this doesn't work ...
        gstate = PyGILState_Ensure();
        gilLocked = true;
    }
}

static inline void maybeReleaseGIL() {
    if (gilLocked) {
        PyGILState_Release(gstate);
        gilLocked = false;
    }
}

static void pyCallback(PyObject* pyFunc, PyObject* arglist) {
    auto result = PyObject_CallObject(pyFunc, arglist);
    Py_DECREF(arglist);
    if (result == NULL) {
        // uhh proper way to handle this?
        printf("**** pyCallback: python callback result was NULL!");
        return;
    }
    Py_DECREF(result);
}

// C callbacks ==============================================

static void clientFuncExecWrapper(int id) {
    maybeLockGIL();
    pyCallback(pyClientFuncExec, Py_BuildValue("(i)", id));
    maybeReleaseGIL();
}

static void clientFuncReleaseWrapper(int id) {
    maybeLockGIL();
    pyCallback(pyClientFuncRelease, Py_BuildValue("(i)", id));
    maybeReleaseGIL();
}

static void clientMethodExecWrapper(ni_InterfaceMethodRef method, int objID) {
    maybeLockGIL();
    pyCallback(pyClientMethodExec, Py_BuildValue("(ni)", method, objID));
    maybeReleaseGIL();
}

static void clientObjectReleaseWrapper(int id) {
    maybeLockGIL();
    pyCallback(pyClientObjectRelease, Py_BuildValue("(i)", id)); // not a tuple?
    maybeReleaseGIL();
}

static void clientClearSafetyAreaWrapper() {
    maybeLockGIL();
    pyCallback(pyClientClearSafetyArea, Py_BuildValue("()"));
    maybeReleaseGIL();
}

// actual python invokable methods =================================================================

static PyObject*
testlibrary_init(PyObject* self, PyObject* args)
{
    Py_Initialize();
    pyMainThreadId = PyThread_get_thread_native_id();

    if (PyArg_ParseTuple(args, "OOOOO:init", &pyClientFuncExec, &pyClientFuncRelease, &pyClientMethodExec, &pyClientObjectRelease, &pyClientClearSafetyArea)) {
        if (!PyCallable_Check(pyClientFuncExec) ||
            !PyCallable_Check(pyClientFuncRelease) ||
            !PyCallable_Check(pyClientMethodExec) ||
            !PyCallable_Check(pyClientObjectRelease) ||
            !PyCallable_Check(pyClientClearSafetyArea))
        {
            PyErr_SetString(PyExc_TypeError, "All parameters must be callable");
            return NULL;
        }
        Py_XINCREF(pyClientFuncExec);
        Py_XINCREF(pyClientFuncRelease);
        Py_XINCREF(pyClientMethodExec);
        Py_XINCREF(pyClientObjectRelease);
        Py_XINCREF(pyClientClearSafetyArea);

        // actual init w/ wrappers
        int ret = ni_nativeImplInit(
            &clientFuncExecWrapper,
            &clientFuncReleaseWrapper,
            &clientMethodExecWrapper,
            &clientObjectReleaseWrapper,
            &clientClearSafetyAreaWrapper
        );

        return PyLong_FromLong(ret);
    }
    // basic failure to parse args
    return NULL;
}

static PyObject*
testlibrary_shutdown(PyObject* self, PyObject* args)
{
    ni_nativeImplShutdown();
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_get_module(PyObject* self, PyObject* args)
{
    const char* name;
    if (!PyArg_ParseTuple(args, "s", &name)) {
        return NULL;
    }
    auto res = ni_getModule(name);
    return PyLong_FromSize_t((size_t)res);
}

static PyObject*
testlibrary_get_module_method(PyObject* self, PyObject* args)
{
    ni_ModuleRef m;
    const char* name;
    if (!PyArg_ParseTuple(args, "ns", &m, &name)) {
        return NULL;
    }
    auto res = ni_getModuleMethod(m, name);
    return PyLong_FromSize_t((size_t)res);
}

static PyObject*
testlibrary_get_interface(PyObject* self, PyObject* args)
{
    ni_ModuleRef m;
    const char* name;
    if (!PyArg_ParseTuple(args, "ns", &m, &name)) {
        return NULL;
    }
    auto res = ni_getInterface(m, name);
    return PyLong_FromSize_t((size_t)res);
}

static PyObject*
testlibrary_get_interface_method(PyObject* self, PyObject* args)
{
    ni_InterfaceRef iface;
    const char* name;
    if (!PyArg_ParseTuple(args, "ns", &iface, &name)) {
        return NULL;
    }
    auto res = ni_getInterfaceMethod(iface, name);
    return PyLong_FromSize_t((size_t)res);
}

static PyObject*
testlibrary_get_exception(PyObject* self, PyObject* args)
{
    ni_ModuleRef m;
    const char* name;
    if (!PyArg_ParseTuple(args, "ns", &m, &name)) {
        return NULL;
    }
    auto res = ni_getException(m, name);
    return PyLong_FromSize_t((size_t)res);
}

static PyObject*
testlibrary_invoke_module_method(PyObject* self, PyObject* args)
{
    ni_ModuleMethodRef method;
    if (!PyArg_ParseTuple(args, "n", &method)) {
        return NULL;
    }
    ni_invokeModuleMethod(method);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_invoke_module_method_with_exceptions(PyObject* self, PyObject* args)
{
    ni_ModuleMethodRef method;
    if (!PyArg_ParseTuple(args, "n", &method)) {
        return NULL;
    }
    auto res = ni_invokeModuleMethodWithExceptions(method);
    return PyLong_FromSize_t((size_t)res);
}

static PyObject*
testlibrary_push_bool(PyObject* self, PyObject* args)
{
    int value; // python bool, 0 or 1
    if (!PyArg_ParseTuple(args, "p", &value)) {
        return NULL;
    }
    ni_pushBool(value);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_pop_bool(PyObject* self, PyObject* args)
{
    return PyBool_FromLong(ni_popBool());
}

static PyObject*
testlibrary_push_int(PyObject* self, PyObject* args)
{
    int value;
    if (!PyArg_ParseTuple(args, "i", &value)) {
        return NULL;
    }
    ni_pushInt(value);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_pop_int(PyObject* self, PyObject* args)
{
    return PyLong_FromLong(ni_popInt());
}


static PyObject*
testlibrary_push_client_func(PyObject* self, PyObject* args)
{
    int id;
    if (!PyArg_ParseTuple(args, "i", &id)) {
        return NULL;
    }
    ni_pushClientFunc(id);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_pop_server_func(PyObject* self, PyObject* args)
{
    return PyLong_FromLong(ni_popServerFunc());
}

static PyObject*
testlibrary_exec_server_func(PyObject* self, PyObject* args)
{
    int id;
    if (!PyArg_ParseTuple(args, "i", &id)) {
        return NULL;
    }
    ni_execServerFunc(id);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_release_server_func(PyObject* self, PyObject* args)
{
    int id;
    if (!PyArg_ParseTuple(args, "i", &id)) {
        return NULL;
    }
    ni_releaseServerFunc(id);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_push_module_constants(PyObject* self, PyObject* args)
{
    ni_ModuleRef m;
    if (!PyArg_ParseTuple(args, "n", &m)) {
        return NULL;
    }
    ni_pushModuleConstants(m);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_release_server_inst(PyObject* self, PyObject* args)
{
    int id;
    if (!PyArg_ParseTuple(args, "i", &id)) {
        return NULL;
    }
    ni_releaseServerInst(id);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_invoke_interface_method(PyObject* self, PyObject* args)
{
    ni_InterfaceMethodRef method;
    int id;
    if (!PyArg_ParseTuple(args, "ni", &method, &id)) {
        return NULL;
    }
    ni_invokeInterfaceMethod(method, id);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_invoke_interface_method_with_exceptions(PyObject* self, PyObject* args)
{
    ni_InterfaceMethodRef method;
    int id;
    if (!PyArg_ParseTuple(args, "ni", &method, &id)) {
        return NULL;
    }
    auto res = ni_invokeInterfaceMethodWithExceptions(method, id);
    return PyLong_FromSize_t((size_t)res);
}

static PyObject*
testlibrary_push_server_inst(PyObject* self, PyObject* args)
{
    int id;
    if (!PyArg_ParseTuple(args, "i", &id)) {
        return NULL;
    }
    ni_pushServerInst(id);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_pop_instance(PyObject* self, PyObject* args)
{
    bool isClientID;
    auto id = ni_popInstance(&isClientID);
    // make and return tuple
    return Py_BuildValue("(iO)", id, isClientID ? Py_True : Py_False); // should inc ref count of chosen bool value ... ?
}

static PyObject*
testlibrary_push_client_inst(PyObject* self, PyObject* args)
{
    int id;
    if (!PyArg_ParseTuple(args, "i", &id)) {
        return NULL;
    }
    ni_pushClientInst(id);
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_clear_server_safety_area(PyObject* self, PyObject* args)
{
    ni_clearServerSafetyArea();
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_dump_tables(PyObject* self, PyObject* args)
{
    ni_dumpTables();
    Py_RETURN_NONE;
}

static PyObject*
testlibrary_set_exception(PyObject* self, PyObject* args)
{
    ni_ExceptionRef e;
    if (!PyArg_ParseTuple(args, "n", &e)) {
        return NULL;
    }
    ni_setException(e);
    Py_RETURN_NONE;
}


// =============================================================================================================================
// PYTHON INIT STUFF 
// =============================================================================================================================

// method table
static PyMethodDef TestLibraryMethods[] = {
    {"init", testlibrary_init, METH_VARARGS, "TestLibrary Init"},
    {"shutdown", testlibrary_shutdown, METH_VARARGS, "TestLibrary Shutdown"},

    {"get_module", testlibrary_get_module, METH_VARARGS, "TestLibrary GetModule"},
    {"get_module_method", testlibrary_get_module_method, METH_VARARGS, "TestLibrary GetModuleMethod"},
    {"get_interface", testlibrary_get_interface, METH_VARARGS, "TestLibrary GetInterface"},
    {"get_interface_method", testlibrary_get_interface_method, METH_VARARGS, "TestLibrary GetInterfaceMethod"},
    {"get_exception", testlibrary_get_exception, METH_VARARGS, "TestLibrary GetException"},
    {"push_module_constants", testlibrary_push_module_constants, METH_VARARGS, "TestLibrary PushModuleConstants"},

    {"invoke_module_method", testlibrary_invoke_module_method, METH_VARARGS, "TestLibrary InvokeModuleMethod"},
    {"invoke_module_method_with_exceptions", testlibrary_invoke_module_method_with_exceptions, METH_VARARGS, "TestLibrary InvokeModuleMethodWithExceptions"},

    {"push_bool", testlibrary_push_bool, METH_VARARGS, "TestLibrary PushBool"},
    {"pop_bool", testlibrary_pop_bool, METH_VARARGS, "TestLibrary PopBool"},

    {"push_int", testlibrary_push_int, METH_VARARGS, "TestLibrary PushInt"},
    {"pop_int", testlibrary_pop_int, METH_VARARGS, "TestLibrary PopInt"},

    {"push_client_func", testlibrary_push_client_func, METH_VARARGS, "TestLibrary PushClientFuncId"},
    {"pop_server_func", testlibrary_pop_server_func, METH_VARARGS, "TestLibrary PopServerFuncId"},

    {"exec_server_func", testlibrary_exec_server_func, METH_VARARGS, "TestLibrary ExecServerFunc"},
    {"release_server_func", testlibrary_release_server_func, METH_VARARGS, "TestLibrary ReleaseServerFunc"},

    {"invoke_interface_method", testlibrary_invoke_interface_method, METH_VARARGS, "TestLibrary InvokeInterfaceMethod"},
    {"invoke_interface_method_with_exceptions", testlibrary_invoke_interface_method_with_exceptions, METH_VARARGS, "TestLibrary InvokeInterfaceMethodWithExceptions"},

    {"push_client_inst", testlibrary_push_client_inst, METH_VARARGS, "TestLibrary PushClientInstID"},
    {"push_server_inst", testlibrary_push_server_inst, METH_VARARGS, "TestLibrary PushServerInstID"},

    {"pop_instance", testlibrary_pop_instance, METH_VARARGS, "TestLibrary PopInstanceID"},
    {"release_server_inst", testlibrary_release_server_inst, METH_VARARGS, "TestLibrary ReleaseServerInstID"},

    {"clear_server_safety_area", testlibrary_clear_server_safety_area, METH_VARARGS, "TestLibrary ClearServerSafetyArea"},

    {"dump_tables", testlibrary_dump_tables, METH_VARARGS, "TestLibrary DumpTables"},
    {"set_exception", testlibrary_set_exception, METH_VARARGS, "TestLibrary SetException"},

    {NULL, NULL, 0, NULL}        /* Sentinel */
};

// module info
static struct PyModuleDef TestLibraryModule = {
    PyModuleDef_HEAD_INIT,
    "testlibrary_native",     /* name of module */
    NULL,                     /* module documentation, may be NULL */ // spam_doc
    -1,                       /* size of per-interpreter state of the module,
                                 or -1 if the module keeps state in global variables. */
    TestLibraryMethods
};

// DLL init
PyMODINIT_FUNC PyInit_testlibrary_native(void)
{
    PyObject* m;

    m = PyState_FindModule(&TestLibraryModule);
    if (m != NULL) {
        Py_INCREF(m);
        return m;
    }
    // else create
    m = PyModule_Create(&TestLibraryModule);
    if (m == NULL)
        return NULL;

    // how to create an exception ...
    TestLibraryError = PyErr_NewException("testlibrary_native.error", NULL, NULL);
    Py_XINCREF(TestLibraryError);
    if (PyModule_AddObject(m, "error", TestLibraryError) < 0) {
        Py_XDECREF(TestLibraryError);
        Py_CLEAR(TestLibraryError);
        Py_DECREF(m);
        return NULL;
    }

    return m;
}
