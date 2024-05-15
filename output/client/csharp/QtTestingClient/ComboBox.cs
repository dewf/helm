using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CSharpFunctionalExtensions;
using Org.Whatever.QtTesting.Support;
using ModuleHandle = Org.Whatever.QtTesting.Support.ModuleHandle;

using static Org.Whatever.QtTesting.Widget;

namespace Org.Whatever.QtTesting
{
    public static class ComboBox
    {
        private static ModuleHandle _module;
        internal static ModuleMethodHandle _create;
        internal static ModuleMethodHandle _handle_todo;
        internal static ModuleMethodHandle _handle_dispose;

        public static Handle Create()
        {
            NativeImplClient.InvokeModuleMethod(_create);
            return Handle__Pop();
        }
        public class Handle : Widget.Handle
        {
            internal Handle(IntPtr nativeHandle) : base(nativeHandle)
            {
            }
            public override void Dispose()
            {
                if (!_disposed)
                {
                    Handle__Push(this);
                    NativeImplClient.InvokeModuleMethod(_handle_dispose);
                    _disposed = true;
                }
            }
            public void Todo()
            {
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_todo);
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
            _module = NativeImplClient.GetModule("ComboBox");
            // assign module handles
            _create = NativeImplClient.GetModuleMethod(_module, "create");
            _handle_todo = NativeImplClient.GetModuleMethod(_module, "Handle_todo");
            _handle_dispose = NativeImplClient.GetModuleMethod(_module, "Handle_dispose");

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
