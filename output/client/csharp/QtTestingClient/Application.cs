using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CSharpFunctionalExtensions;
using Org.Whatever.QtTesting.Support;
using ModuleHandle = Org.Whatever.QtTesting.Support.ModuleHandle;

namespace Org.Whatever.QtTesting
{
    public static class Application
    {
        private static ModuleHandle _module;

        // built-in array type: string[]
        internal static ModuleMethodHandle _setStyle;
        internal static ModuleMethodHandle _exec;
        internal static ModuleMethodHandle _create;
        internal static ModuleMethodHandle _handle_dispose;

        public static void SetStyle(string name)
        {
            NativeImplClient.PushString(name);
            NativeImplClient.InvokeModuleMethod(_setStyle);
        }

        public static int Exec()
        {
            NativeImplClient.InvokeModuleMethod(_exec);
            return NativeImplClient.PopInt32();
        }

        public static Handle Create(string[] args)
        {
            NativeImplClient.PushStringArray(args);
            NativeImplClient.InvokeModuleMethod(_create);
            return Handle__Pop();
        }
        public class Handle : IDisposable
        {
            internal readonly IntPtr NativeHandle;
            protected bool _disposed;
            internal Handle(IntPtr nativeHandle)
            {
                NativeHandle = nativeHandle;
            }
            public virtual void Dispose()
            {
                if (!_disposed)
                {
                    Handle__Push(this);
                    NativeImplClient.InvokeModuleMethod(_handle_dispose);
                    _disposed = true;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Handle__Push(Handle thing)
        {
            NativeImplClient.PushPtr(thing?.NativeHandle ?? IntPtr.Zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Handle Handle__Pop()
        {
            var ptr = NativeImplClient.PopPtr();
            return ptr != IntPtr.Zero ? new Handle(ptr) : null;
        }

        internal static void __Init()
        {
            _module = NativeImplClient.GetModule("Application");
            // assign module handles
            _setStyle = NativeImplClient.GetModuleMethod(_module, "setStyle");
            _exec = NativeImplClient.GetModuleMethod(_module, "exec");
            _create = NativeImplClient.GetModuleMethod(_module, "create");
            _handle_dispose = NativeImplClient.GetModuleMethod(_module, "Handle_dispose");

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
