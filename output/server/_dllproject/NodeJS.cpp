#include <napi.h>
#include <thread>

#include "../../../../core/NativeImplCore.h"

#if _WIN32 || _WIN64
#if _WIN64
#define ENV64
#else
#define ENV32
#endif
#endif

#if __GNUC__
#if __x86_64__ || __ppc64__
#define ENV64
#else
#define ENV32
#endif
#endif

// misc =====================================================
static bool bool_dontcare;

inline void *decodePtr(Napi::Value value) {
#ifdef ENV64
	return (void *)value.As<Napi::BigInt>().Uint64Value(&bool_dontcare);
#else
	return (void *)value.As<Napi::Number>().Uint32();
#endif
}

inline Napi::Value encodePtr(Napi::Env env, void *ptr) {
#ifdef ENV64
	return Napi::BigInt::New(env, (uint64_t)ptr);
#else
	return Napi::Number::New(env, (uint32_t)ptr);
#endif
}

// C callbacks ==============================================

static Napi::FunctionReference func_clientFuncExec;
static Napi::ThreadSafeFunction func_clientFuncExec_threadSafe;

static Napi::FunctionReference func_clientFuncRelease;
static Napi::ThreadSafeFunction func_clientFuncRelease_threadSafe;

static Napi::FunctionReference func_clientMethodExec;
static Napi::ThreadSafeFunction func_clientMethodExec_threadSafe;

static Napi::FunctionReference func_clientObjectRelease;
static Napi::ThreadSafeFunction func_clientObjectRelease_threadSafe;

static Napi::FunctionReference func_clientClearSafetyArea;
static Napi::ThreadSafeFunction func_clientClearSafetyArea_threadSafe;

static std::thread::id mainThreadId;

static void node_clientFuncExec(int id) {
	if (std::this_thread::get_id() == mainThreadId) {
		auto env = func_clientFuncExec.Env();
		func_clientFuncExec.Call({ Napi::Number::New(env, id) });
	}
	else {
		// use thread-safe version
		auto callback = [id](Napi::Env env, Napi::Function jsCallback) {
			jsCallback.Call({ Napi::Number::New(env, id) });
		};
		auto status = func_clientFuncExec_threadSafe.BlockingCall(callback);
		if (status != napi_ok) {
			printf("node_clientFuncExec: blocking call failed\n");
		}
	}
}

static void node_clientFuncRelease(int id) {
	if (std::this_thread::get_id() == mainThreadId) {
		auto env = func_clientFuncRelease.Env();
		func_clientFuncRelease.Call({ Napi::Number::New(env, id) });
	}
	else {
		// use thread-safe version
		auto callback = [id](Napi::Env env, Napi::Function jsCallback) {
			jsCallback.Call({ Napi::Number::New(env, id) });
		};
		auto status = func_clientFuncRelease_threadSafe.BlockingCall(callback);
		if (status != napi_ok) {
			printf("node_clientFuncRelease: blocking call failed\n");
		}
	}
}

static void node_clientMethodExec(ni_InterfaceMethodRef method, int objId) {
	if (std::this_thread::get_id() == mainThreadId) {
		auto env = func_clientMethodExec.Env();
		auto jsMethod = encodePtr(env, method);
		auto jsObjId = Napi::Number::New(env, objId);
		func_clientMethodExec.Call({ jsMethod, jsObjId });
	}
	else {
		// use thread-safe version
		auto callback = [method, objId](Napi::Env env, Napi::Function jsCallback) {
			auto jsMethod = encodePtr(env, method);
			auto jsObjId = Napi::Number::New(env, objId);
			jsCallback.Call({ jsMethod, jsObjId });
		};
		auto status = func_clientMethodExec_threadSafe.BlockingCall(callback);
		if (status != napi_ok) {
			printf("node_clientMethodExec: blocking call failed\n");
		}
	}
}

static void node_clientObjectRelease(int id) {
	if (std::this_thread::get_id() == mainThreadId) {
		auto env = func_clientObjectRelease.Env();
		func_clientObjectRelease.Call({ Napi::Number::New(env, id) });
	}
	else {
		// use thread-safe version
		auto callback = [id](Napi::Env env, Napi::Function jsCallback) {
			jsCallback.Call({ Napi::Number::New(env, id) });
		};
		auto status = func_clientObjectRelease_threadSafe.BlockingCall(callback);
		if (status != napi_ok) {
			printf("node_clientObjectRelease: blocking call failed\n");
		}
	}
}

static void node_clientClearSafetyArea() {
	if (std::this_thread::get_id() == mainThreadId) {
		func_clientClearSafetyArea.Call({});
	}
	else {
		// use thread-safe version
		auto callback = [](Napi::Env env, Napi::Function jsCallback) {
			jsCallback.Call({});
		};
		auto status = func_clientClearSafetyArea_threadSafe.BlockingCall(callback);
		if (status != napi_ok) {
			printf("node_clientClearSafetyArea: blocking call failed\n");
		}
	}
}

static Napi::Number node_init(const Napi::CallbackInfo& info) {
	// save main thread to compare against later
	mainThreadId = std::this_thread::get_id();

	auto env = info.Env();

	auto f1 = info[0].As<Napi::Function>();
	func_clientFuncExec = Napi::Persistent(f1);
	func_clientFuncExec_threadSafe = Napi::ThreadSafeFunction::New(env, f1, "clientFuncExec", 0, 1);

	auto f2 = info[1].As<Napi::Function>();
	func_clientFuncRelease = Persistent(f2);
	func_clientFuncRelease_threadSafe = Napi::ThreadSafeFunction::New(env, f2, "clientFuncRelease", 0, 1);

	auto f3 = info[2].As<Napi::Function>();
	func_clientMethodExec = Persistent(f3);
	func_clientMethodExec_threadSafe = Napi::ThreadSafeFunction::New(env, f3, "clientMethodExec", 0, 1);

	auto f4 = info[3].As<Napi::Function>();
	func_clientObjectRelease = Persistent(f4);
	func_clientObjectRelease_threadSafe = Napi::ThreadSafeFunction::New(env, f4, "clientObjectRelease", 0, 1);

	auto f5 = info[4].As<Napi::Function>();
	func_clientClearSafetyArea = Persistent(f5);
	func_clientClearSafetyArea_threadSafe = Napi::ThreadSafeFunction::New(env, f5, "clientClearSafetyArea", 0, 1);

	auto result =
		ni_nativeImplInit(
			&node_clientFuncExec,
			&node_clientFuncRelease,
			&node_clientMethodExec,
			&node_clientObjectRelease,
			&node_clientClearSafetyArea
		);
	return Napi::Number::New(env, result);
}

static void node_shutdown(const Napi::CallbackInfo& info) {
	printf("in node_shutdown\n");

	ni_nativeImplShutdown();

	func_clientFuncExec_threadSafe.Release();
	func_clientFuncRelease_threadSafe.Release();
	func_clientMethodExec_threadSafe.Release();
	func_clientObjectRelease_threadSafe.Release();
	func_clientClearSafetyArea_threadSafe.Release();

	func_clientFuncExec.Unref();
	func_clientFuncRelease.Unref();
	func_clientMethodExec.Unref();
	func_clientObjectRelease.Unref();
	func_clientClearSafetyArea.Unref();

	printf("node shutdown complete\n");
}

static Napi::Value node_getModule(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	auto name = info[0].As<Napi::String>().Utf8Value();
	auto res = ni_getModule(name.c_str());
	return encodePtr(env, res);
}

static Napi::Value node_getModuleMethod(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	auto m = (ni_ModuleRef)decodePtr(info[0]);
	auto name = info[1].As<Napi::String>().Utf8Value();
	auto res = ni_getModuleMethod(m, name.c_str());
	return encodePtr(env, res);
}

static void node_invokeModuleMethod(const Napi::CallbackInfo& info) {
	auto method = (ni_ModuleMethodRef)decodePtr(info[0]);
	ni_invokeModuleMethod(method);
}

static Napi::Value node_invokeModuleMethodWithExceptions(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	auto method = (ni_ModuleMethodRef)decodePtr(info[0]);
	auto e = ni_invokeModuleMethodWithExceptions(method);
	return encodePtr(env, e);
}

static void node_pushInt(const Napi::CallbackInfo& info) {
	auto x = info[0].As<Napi::Number>().Int32Value();
	ni_pushInt(x);
}

static Napi::Number node_popInt(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	return Napi::Number::New(env, ni_popInt());
}

static void node_pushClientFunc(const Napi::CallbackInfo& info) {
	auto id = info[0].As<Napi::Number>().Int32Value();
	ni_pushClientFunc(id);
}

static Napi::Number node_popServerFunc(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	return Napi::Number::New(env, ni_popServerFunc());
}

static void node_execServerFunc(const Napi::CallbackInfo& info) {
	auto id = info[0].As<Napi::Number>().Int32Value();
	ni_execServerFunc(id);
}

static void node_pushModuleConstants(const Napi::CallbackInfo& info) {
	auto module = (ni_ModuleRef)decodePtr(info[0]);
	ni_pushModuleConstants(module);
}

static Napi::Value node_getInterface(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	auto module = (ni_ModuleRef)decodePtr(info[0]);
	auto name = info[1].As<Napi::String>().Utf8Value();
	auto res = ni_getInterface(module, name.c_str());
	return encodePtr(env, res);
}

static Napi::Value node_getInterfaceMethod(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	auto iface = (ni_InterfaceRef)decodePtr(info[0]);
	auto name = info[1].As<Napi::String>().Utf8Value();
	auto res = ni_getInterfaceMethod(iface, name.c_str());
	return encodePtr(env, res);
}

static void node_invokeInterfaceMethod(const Napi::CallbackInfo& info) {
	auto method = (ni_InterfaceMethodRef)decodePtr(info[0]);
	auto serverId = info[1].As<Napi::Number>().Int32Value();
	ni_invokeInterfaceMethod(method, serverId);
}

static Napi::Value node_invokeInterfaceMethodWithExceptions(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	auto method = (ni_InterfaceMethodRef)decodePtr(info[0]);
	auto serverId = info[1].As<Napi::Number>().Int32Value();
	auto e = ni_invokeInterfaceMethodWithExceptions(method, serverId);
	return encodePtr(env, e);
}

static void node_releaseServerFunc(const Napi::CallbackInfo& info) {
	auto id = info[0].As<Napi::Number>().Int32Value();
	ni_releaseServerFunc(id);
}

static Napi::Object node_popInstance(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	bool isClientId;
	auto id = ni_popInstance(&isClientId);
	auto result = Napi::Object::New(env);
	result.Set("id", Napi::Number::New(env, id));
	result.Set("isClientId", Napi::Boolean::New(env, isClientId));
	return result;
}

static void node_releaseServerInst(const Napi::CallbackInfo& info) {
	auto id = info[0].As<Napi::Number>().Int32Value();
	ni_releaseServerInst(id);
}

static void node_clearServerSafetyArea(const Napi::CallbackInfo& info) {
	ni_clearServerSafetyArea();
}

static void node_pushServerInst(const Napi::CallbackInfo& info) {
	auto id = info[0].As<Napi::Number>().Int32Value();
	ni_pushServerInst(id);
}

static void node_dumpTables(const Napi::CallbackInfo& info) {
	ni_dumpTables();
}

static void node_pushClientInst(const Napi::CallbackInfo& info) {
	auto id = info[0].As<Napi::Number>().Int32Value();
	ni_pushClientInst(id);
}

static void node_pushBool(const Napi::CallbackInfo& info) {
	auto value = info[0].As<Napi::Boolean>();
	ni_pushBool(value);
}

static Napi::Boolean node_popBool(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	return Napi::Boolean::New(env, ni_popBool());
}

static Napi::Value node_getException(const Napi::CallbackInfo& info) {
	auto env = info.Env();
	auto module = (ni_ModuleRef)decodePtr(info[0]);
	auto name = info[1].As<Napi::String>().Utf8Value();
	auto e = ni_getException(module, name.c_str());
	return encodePtr(env, e);
}

static void node_setException(const Napi::CallbackInfo& info) {
	auto exception = (ni_ExceptionRef)decodePtr(info[0]);
	ni_setException(exception);
}

Napi::Object InitAll(Napi::Env env, Napi::Object exports) {
	exports.Set("init", Napi::Function::New(env, node_init));
	exports.Set("shutdown", Napi::Function::New(env, node_shutdown));
	exports.Set("getModule", Napi::Function::New(env, node_getModule));
	exports.Set("getModuleMethod", Napi::Function::New(env, node_getModuleMethod));
	exports.Set("invokeModuleMethod", Napi::Function::New(env, node_invokeModuleMethod));
	exports.Set("invokeModuleMethodWithExceptions", Napi::Function::New(env, node_invokeModuleMethodWithExceptions));
	exports.Set("pushInt", Napi::Function::New(env, node_pushInt));
	exports.Set("popInt", Napi::Function::New(env, node_popInt));
	exports.Set("pushClientFunc", Napi::Function::New(env, node_pushClientFunc));
	exports.Set("popServerFunc", Napi::Function::New(env, node_popServerFunc));
	exports.Set("execServerFunc", Napi::Function::New(env, node_execServerFunc));
	exports.Set("pushModuleConstants", Napi::Function::New(env, node_pushModuleConstants));
	exports.Set("getInterface", Napi::Function::New(env, node_getInterface));
	exports.Set("getInterfaceMethod", Napi::Function::New(env, node_getInterfaceMethod));
	exports.Set("invokeInterfaceMethod", Napi::Function::New(env, node_invokeInterfaceMethod));
	exports.Set("invokeInterfaceMethodWithExceptions", Napi::Function::New(env, node_invokeInterfaceMethodWithExceptions));
	exports.Set("releaseServerFunc", Napi::Function::New(env, node_releaseServerFunc));
	exports.Set("popInstance", Napi::Function::New(env, node_popInstance));
	exports.Set("releaseServerInst", Napi::Function::New(env, node_releaseServerInst));
	exports.Set("clearServerSafetyArea", Napi::Function::New(env, node_clearServerSafetyArea));
	exports.Set("pushServerInst", Napi::Function::New(env, node_pushServerInst));
	exports.Set("dumpTables", Napi::Function::New(env, node_dumpTables));
	exports.Set("pushClientInst", Napi::Function::New(env, node_pushClientInst));
	exports.Set("pushBool", Napi::Function::New(env, node_pushBool));
	exports.Set("popBool", Napi::Function::New(env, node_popBool));
	exports.Set("getException", Napi::Function::New(env, node_getException));
	exports.Set("setException", Napi::Function::New(env, node_setException));
	return exports;
}

NODE_API_MODULE(testlibrary_native, InitAll)
