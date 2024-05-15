#include <jni.h>
// javah org.prefixed.nativeimpl.NativeMethods
#include "../../testproject/src/_headers/org_prefixed_nativeimpl_NativeMethods.h"
#include "../../../../core/NativeImplCore.h"

static JavaVM* jvm = nullptr;
thread_local JNIEnv* _env = nullptr; // thread local!

static inline JNIEnv* safeGetEnv() {
	if (_env == nullptr) {
		// new thread, need to attach!
		//printf("===== JVM ATTACHING NEW THREAD ======================\n");
		//fflush(stdout);
		jvm->AttachCurrentThreadAsDaemon((void**)&_env, nullptr);
	}
	return _env;
}

static jclass class_InstanceID;
static jmethodID method_InstanceID_ctor;

static jobject callbacksDelegate;
static jmethodID method_clientFuncExec;
static jmethodID method_clientFuncRelease;
static jmethodID method_clientMethodExec;
static jmethodID method_clientObjectRelease;
static jmethodID method_clientClearSafetyArea;

// callback wrappers - send to designated callback handler

static void javaClientFuncExec(int id) {
	safeGetEnv()->CallVoidMethod(callbacksDelegate, method_clientFuncExec, id);
}

static void javaClientFuncRelease(int id) {
	safeGetEnv()->CallVoidMethod(callbacksDelegate, method_clientFuncRelease, id);
}

static void javaClientMethodExec(ni_InterfaceMethodRef method, int objID) {
	// if we're calling back in from a new thread,
	// requires special handling ...
	safeGetEnv()->CallVoidMethod(callbacksDelegate, method_clientMethodExec, method, objID);
}

static void javaClientObjectRelease(int id) {
	safeGetEnv()->CallVoidMethod(callbacksDelegate, method_clientObjectRelease, id);
}

static void javaClientClearSafetyArea() {
	// hmmm not even remotely sure about the thread situation here
	safeGetEnv()->CallVoidMethod(callbacksDelegate, method_clientClearSafetyArea);
}

// methods

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    staticInit
 * Signature: ()V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_staticInit(JNIEnv* env, jclass clazz)
{
	env->GetJavaVM(&jvm); // need this for later
	_env = env; // make sure env is known for main thread before any callbacks - no need to attach
	
	auto cbIface = env->FindClass("org/prefixed/nativeimpl/NativeMethods$Callbacks");

	// get callback method ids
	method_clientFuncExec = env->GetMethodID(cbIface, "clientFuncExec", "(I)V");
	method_clientFuncRelease = env->GetMethodID(cbIface, "clientFuncRelease", "(I)V");
	method_clientMethodExec = env->GetMethodID(cbIface, "clientMethodExec", "(JI)V");
	method_clientObjectRelease = env->GetMethodID(cbIface, "clientObjectRelease", "(I)V");
	method_clientClearSafetyArea = env->GetMethodID(cbIface, "clientClearSafetyArea", "()V");

	jclass temp = env->FindClass("org/prefixed/nativeimpl/InstanceID");
	class_InstanceID = (jclass) env->NewGlobalRef(temp); // must be made global, else won't work later :D

	// but method IDs are forever
	method_InstanceID_ctor = env->GetMethodID(class_InstanceID, "<init>", "(IZ)V");

	//printf("staticInit: class_InstanceID %p\n", class_InstanceID);
	//printf("staticInit:    - ctor: %p\n", method_InstanceID_ctor);
	//fflush(stdout);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    init
 * Signature: (Lorg/prefixed/nativeimpl/NativeMethods/Callbacks;)I
 */
JNIEXPORT jint JNICALL Java_org_prefixed_nativeimpl_NativeMethods_init(JNIEnv* env, jclass clazz, jobject callbacks)
{
	// save object as global handler for callbacks
	::callbacksDelegate = env->NewGlobalRef(callbacks);
	return ni_nativeImplInit(
		&javaClientFuncExec,
		&javaClientFuncRelease,
		&javaClientMethodExec,
		&javaClientObjectRelease,
		&javaClientClearSafetyArea
	);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    shutdown
 * Signature: ()V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_shutdown(JNIEnv*, jclass)
{
	// free the callbacks delegate? probably doesn't matter -
	//   (or in fact it might be bad, if the other side releases our objects after nativeImplShutdown)
	ni_nativeImplShutdown();
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    getModule
 * Signature: (Ljava/lang/String;)J
 */
JNIEXPORT jlong JNICALL Java_org_prefixed_nativeimpl_NativeMethods_getModule(JNIEnv* env, jclass clazz, jstring name)
{
	auto name_utf8 = env->GetStringUTFChars(name, nullptr);
	auto res = ni_getModule(name_utf8);
	env->ReleaseStringUTFChars(name, name_utf8);
	return (jlong)res;
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    getModuleMethod
 * Signature: (JLjava/lang/String;)J
 */
JNIEXPORT jlong JNICALL Java_org_prefixed_nativeimpl_NativeMethods_getModuleMethod(JNIEnv* env, jclass clazz, jlong moduleHandle, jstring name)
{
	auto name_utf8 = env->GetStringUTFChars(name, nullptr);
	auto res = ni_getModuleMethod((ni_ModuleRef)moduleHandle, name_utf8);
	env->ReleaseStringUTFChars(name, name_utf8);
	return (jlong)res;
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    getInterface
 * Signature: (JLjava/lang/String;)J
 */
JNIEXPORT jlong JNICALL Java_org_prefixed_nativeimpl_NativeMethods_getInterface(JNIEnv* env, jclass clazz, jlong moduleHandle, jstring name)
{
	auto name_utf8 = env->GetStringUTFChars(name, nullptr);
	auto res = ni_getInterface((ni_ModuleRef)moduleHandle, name_utf8);
	env->ReleaseStringUTFChars(name, name_utf8);
	return (jlong)res;
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    getInterfaceMethod
 * Signature: (JLjava/lang/String;)J
 */
JNIEXPORT jlong JNICALL Java_org_prefixed_nativeimpl_NativeMethods_getInterfaceMethod(JNIEnv* env, jclass clazz, jlong iface, jstring name)
{
	auto name_utf8 = env->GetStringUTFChars(name, nullptr);
	auto res = ni_getInterfaceMethod((ni_InterfaceRef)iface, name_utf8);
	env->ReleaseStringUTFChars(name, name_utf8);
	return (jlong)res;
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    getException
 * Signature: (JLjava/lang/String;)J
 */
JNIEXPORT jlong JNICALL Java_org_prefixed_nativeimpl_NativeMethods_getException(JNIEnv* env, jclass clazz, jlong moduleHandle, jstring name)
{
	auto name_utf8 = env->GetStringUTFChars(name, nullptr);
	auto res = ni_getException((ni_ModuleRef)moduleHandle, name_utf8);
	env->ReleaseStringUTFChars(name, name_utf8);
	return (jlong)res;
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    pushModuleConstants
 * Signature: (J)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_pushModuleConstants(JNIEnv* env, jclass clazz, jlong moduleHandle)
{
	ni_pushModuleConstants((ni_ModuleRef)moduleHandle);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    invokeModuleMethod
 * Signature: (J)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_invokeModuleMethod(JNIEnv* env, jclass clazz, jlong moduleMethodHandle)
{
	ni_invokeModuleMethod((ni_ModuleMethodRef)moduleMethodHandle);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    invokeModuleMethodWithExceptions
 * Signature: (J)J
 */
JNIEXPORT jlong JNICALL Java_org_prefixed_nativeimpl_NativeMethods_invokeModuleMethodWithExceptions(JNIEnv* env, jclass clazz, jlong moduleMethodHandle)
{
	return (jlong) ni_invokeModuleMethodWithExceptions((ni_ModuleMethodRef)moduleMethodHandle);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    pushBool
 * Signature: (Z)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_pushBool(JNIEnv* env, jclass clazz, jboolean value)
{
	ni_pushBool(value);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    popBool
 * Signature: ()Z
 */
JNIEXPORT jboolean JNICALL Java_org_prefixed_nativeimpl_NativeMethods_popBool(JNIEnv* env, jclass clazz)
{
	return ni_popBool();
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    pushInt
 * Signature: (I)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_pushInt(JNIEnv* env, jclass clazz, jint value)
{
	ni_pushInt(value);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    popInt
 * Signature: ()I
 */
JNIEXPORT jint JNICALL Java_org_prefixed_nativeimpl_NativeMethods_popInt(JNIEnv* env, jclass clazz)
{
	return ni_popInt();
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    pushClientFuncId
 * Signature: (I)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_pushClientFunc(JNIEnv* env, jclass clazz, jint id)
{
	ni_pushClientFunc(id);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    popServerFuncId
 * Signature: ()I
 */
JNIEXPORT jint JNICALL Java_org_prefixed_nativeimpl_NativeMethods_popServerFunc(JNIEnv* env, jclass clazz)
{
	return ni_popServerFunc();
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    execServerFunc
 * Signature: (I)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_execServerFunc(JNIEnv* env, jclass clazz, jint id)
{
	ni_execServerFunc(id);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    releaseServerFunc
 * Signature: (I)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_releaseServerFunc(JNIEnv* env, jclass clazz, jint id)
{
	ni_releaseServerFunc(id);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    invokeInterfaceMethod
 * Signature: (JI)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_invokeInterfaceMethod(JNIEnv* env, jclass clazz, jlong ifaceMethod, jint serverID)
{
	ni_invokeInterfaceMethod((ni_InterfaceMethodRef)ifaceMethod, serverID);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    invokeInterfaceMethodWithExceptions
 * Signature: (JI)J
 */
JNIEXPORT jlong JNICALL Java_org_prefixed_nativeimpl_NativeMethods_invokeInterfaceMethodWithExceptions(JNIEnv* env, jclass clazz, jlong ifaceMethod, jint serverID)
{
	return (jlong) ni_invokeInterfaceMethodWithExceptions((ni_InterfaceMethodRef)ifaceMethod, serverID);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    pushClientInstID
 * Signature: (I)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_pushClientInst(JNIEnv* env, jclass clazz, jint id)
{
	ni_pushClientInst(id);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    pushServerInstID
 * Signature: (I)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_pushServerInst(JNIEnv* env, jclass clazz, jint id)
{
	ni_pushServerInst(id);
}


/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    popInstanceID
 * Signature: ()Lorg/prefixed/nativeimpl/InstanceID;
 */
JNIEXPORT jobject JNICALL Java_org_prefixed_nativeimpl_NativeMethods_popInstance(JNIEnv* env, jclass clazz)
{
	bool isClientID;
	auto id = ni_popInstance(&isClientID);

	//class_InstanceID = env->FindClass("org/prefixed/nativeimpl/InstanceID");
	//method_InstanceID_ctor = env->GetMethodID(class_InstanceID, "<init>", "(IZ)V");
	jobject ret = env->NewObject(class_InstanceID, method_InstanceID_ctor, id, isClientID); //  ? JNI_TRUE : JNI_FALSE

	//printf("popInstanceID: class_InstanceID %p\n", class_InstanceID);
	//printf("popInstanceID:    - ctor: %p\n", method_InstanceID_ctor);
	//fflush(stdout);

	return ret;
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    releaseServerInstID
 * Signature: (I)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_releaseServerInst(JNIEnv* env, jclass clazz, jint id)
{
	ni_releaseServerInst(id);
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    clearServerSafetyArea
 * Signature: ()V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_clearServerSafetyArea(JNIEnv* env, jclass clazz)
{
	ni_clearServerSafetyArea();
}

/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    dumpTables
 * Signature: ()V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_dumpTables(JNIEnv* env, jclass clazz)
{
	ni_dumpTables();
}


/*
 * Class:     org_prefixed_nativeimpl_NativeMethods
 * Method:    setException
 * Signature: (J)V
 */
JNIEXPORT void JNICALL Java_org_prefixed_nativeimpl_NativeMethods_setException(JNIEnv* env, jclass clazz, jlong e)
{
	ni_setException((ni_ExceptionRef) e);
}
