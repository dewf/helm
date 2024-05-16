using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CSharpFunctionalExtensions;
using Org.Whatever.QtTesting.Support;
using ModuleHandle = Org.Whatever.QtTesting.Support.ModuleHandle;

using static Org.Whatever.QtTesting.Signal;
using static Org.Whatever.QtTesting.Widget;

namespace Org.Whatever.QtTesting
{
    public static class TabWidget
    {
        private static ModuleHandle _module;
        internal static ModuleMethodHandle _create;
        internal static ModuleMethodHandle _handle_addTab;
        internal static ModuleMethodHandle _handle_insertTab;
        internal static ModuleMethodHandle _handle_onCurrentChanged;
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
            public void AddTab(Widget.Handle page, string label)
            {
                NativeImplClient.PushString(label);
                Widget.Handle__Push(page);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_addTab);
            }
            public void InsertTab(int index, Widget.Handle page, string label)
            {
                NativeImplClient.PushString(label);
                Widget.Handle__Push(page);
                NativeImplClient.PushInt32(index);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_insertTab);
            }
            public void OnCurrentChanged(IntDelegate handler)
            {
                IntDelegate__Push(handler);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_onCurrentChanged);
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
            _module = NativeImplClient.GetModule("TabWidget");
            // assign module handles
            _create = NativeImplClient.GetModuleMethod(_module, "create");
            _handle_addTab = NativeImplClient.GetModuleMethod(_module, "Handle_addTab");
            _handle_insertTab = NativeImplClient.GetModuleMethod(_module, "Handle_insertTab");
            _handle_onCurrentChanged = NativeImplClient.GetModuleMethod(_module, "Handle_onCurrentChanged");
            _handle_dispose = NativeImplClient.GetModuleMethod(_module, "Handle_dispose");

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
