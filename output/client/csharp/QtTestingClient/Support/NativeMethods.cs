using System.Runtime.InteropServices;

namespace Org.Whatever.QtTesting.Support
{
    internal static partial class NativeMethods
    {
        internal delegate void ExecFunctionDelegate(int id);
        internal delegate void ClientMethodDelegate(IntPtr methodHandle, int objectId);
        internal delegate void ClientResourceReleaseDelegate(int id);
        internal delegate void ClientClearSafetyAreaDelegate();

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial int nativeImplInit(
            ExecFunctionDelegate execFunctionDelegate,
            ClientMethodDelegate methodDelegate,
            ClientResourceReleaseDelegate resourceReleaseDelegate,
            ClientClearSafetyAreaDelegate clientClearSafetyAreaDelegate
            );

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void nativeImplShutdown();

        [LibraryImport("QtTestingServer.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial IntPtr getModule(string name);

        [LibraryImport("QtTestingServer.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial IntPtr getModuleMethod(IntPtr module, string name);

        [LibraryImport("QtTestingServer.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial IntPtr getInterface(IntPtr module, string name);

        [LibraryImport("QtTestingServer.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial IntPtr getInterfaceMethod(IntPtr iface, string name);

        [LibraryImport("QtTestingServer.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial IntPtr getException(IntPtr module, string name);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void invokeModuleMethod(IntPtr method);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial IntPtr invokeModuleMethodWithExceptions(IntPtr method);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushPtr(IntPtr value);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial IntPtr popPtr();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushPtrArray(IntPtr[] values, IntPtr count);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popPtrArray(out IntPtr valuesPtr, out IntPtr count); // void***, size_t*

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushSizeT(IntPtr value);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial IntPtr popSizeT();

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushSizeTArray(IntPtr[] values, IntPtr count);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popSizeTArray(out IntPtr valuesPtr, out IntPtr count); // size_t**, size_t*

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushBool(
            [MarshalAs(UnmanagedType.U1)] bool value);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static partial bool popBool();

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushBoolArray(byte[] values, IntPtr count); // bool*, size_t

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popBoolArray(out IntPtr valuesPtr, out IntPtr count); // bool**, size_t*
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushInt8(sbyte x);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial sbyte popInt8();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushInt8Array(sbyte[] values, IntPtr count); // size_t count
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popInt8Array(out IntPtr values, out IntPtr count); // both args are pointers
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushUInt8(byte x);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial byte popUInt8();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushUInt8Array(byte[] values, IntPtr count); // size_t count
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popUInt8Array(out IntPtr values, out IntPtr count); // both args are pointers
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushInt16(short x);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial short popInt16();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushInt16Array(short[] values, IntPtr count); // size_t count
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popInt16Array(out IntPtr values, out IntPtr count); // both args are pointers
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushUInt16(ushort x);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial ushort popUInt16();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushUInt16Array(ushort[] values, IntPtr count); // size_t count

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popUInt16Array(out IntPtr values, out IntPtr count); // both args are pointers
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushInt32(int x);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial int popInt32();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushInt32Array(int[] values, IntPtr count); // size_t count
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popInt32Array(out IntPtr values, out IntPtr count); // both args are pointers
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushUInt32(uint x);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial uint popUInt32();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushUInt32Array(uint[] values, IntPtr count); // size_t count

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popUInt32Array(out IntPtr values, out IntPtr count); // both args are pointers
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushInt64(long x);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial long popInt64();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushInt64Array(long[] values, IntPtr count); // size_t count

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popInt64Array(out IntPtr values, out IntPtr count); // both args are pointers
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushUInt64(ulong x);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial ulong popUInt64();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushUInt64Array(ulong[] values, IntPtr count); // size_t count

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popUInt64Array(out IntPtr values, out IntPtr count); // both args are pointers

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushFloat(float x);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial float popFloat();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushFloatArray(float[] values, IntPtr count);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popFloatArray(out IntPtr values, out IntPtr count);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushDouble(double x);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial double popDouble();
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushDoubleArray(double[] values, IntPtr count);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popDoubleArray(out IntPtr values, out IntPtr count);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushString(IntPtr str, IntPtr length); // size_t length

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popString(out IntPtr strPtr, out IntPtr length); // size_t* length

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushStringArray(IntPtr[] strs, IntPtr[] lengths, IntPtr count); // const char **, size_t*, size_t

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popStringArray(out IntPtr strsPtr, out IntPtr lengthsPtr, out IntPtr count); // const char ***, size_t**, size_t*

        public struct BufferDescriptor
        {
            public IntPtr Start;
            public int ElementSize;
            public IntPtr TotalCount;
            public IntPtr TotalSize;
        }

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushBuffer(int id, [MarshalAs(UnmanagedType.U1)] bool isClientId, ref BufferDescriptor descriptor);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void popBuffer(out int id, [MarshalAs(UnmanagedType.U1)] out bool isClientId, out BufferDescriptor descriptor);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushClientFunc(int id);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial int popServerFunc();

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void execServerFunc(int id);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial IntPtr execServerFuncWithExceptions(int id);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void invokeInterfaceMethod(IntPtr method, int serverId);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial IntPtr invokeInterfaceMethodWithExceptions(IntPtr method, int serverId);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushInstance(int id, [MarshalAs(UnmanagedType.U1)] bool isClientId);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial int popInstance([MarshalAs(UnmanagedType.U1)] out bool isClientId);
        
        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushNull();

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial int releaseServerResource(int id);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void clearServerSafetyArea();

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void dumpTables();

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void pushModuleConstants(IntPtr module);

        [LibraryImport("QtTestingServer.dll")]
        [UnmanagedCallConv(CallConvs = new [] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void setException(IntPtr exceptionHandle);
    }
}