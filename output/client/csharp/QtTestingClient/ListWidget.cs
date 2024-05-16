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
using static Org.Whatever.QtTesting.Signal;

namespace Org.Whatever.QtTesting
{
    public static class ListWidget
    {
        private static ModuleHandle _module;

        // built-in array type: string[]
        internal static ModuleMethodHandle _create;
        internal static ModuleMethodHandle _handle_setItems;
        internal static ModuleMethodHandle _handle_setSelectionMode;
        internal static ModuleMethodHandle _handle_onCurrentRowChanged;
        internal static ModuleMethodHandle _handle_dispose;

        public static Handle Create()
        {
            NativeImplClient.InvokeModuleMethod(_create);
            return Handle__Pop();
        }
        public enum SelectionMode
        {
            None,
            Single,
            Multi,
            Extended,
            Contiguous
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SelectionMode__Push(SelectionMode value)
        {
            NativeImplClient.PushInt32((int)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SelectionMode SelectionMode__Pop()
        {
            var ret = NativeImplClient.PopInt32();
            return (SelectionMode)ret;
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
            public void SetItems(string[] items)
            {
                NativeImplClient.PushStringArray(items);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setItems);
            }
            public void SetSelectionMode(SelectionMode mode)
            {
                SelectionMode__Push(mode);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setSelectionMode);
            }
            public void OnCurrentRowChanged(IntDelegate handler)
            {
                IntDelegate__Push(handler);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_onCurrentRowChanged);
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
            _module = NativeImplClient.GetModule("ListWidget");
            // assign module handles
            _create = NativeImplClient.GetModuleMethod(_module, "create");
            _handle_setItems = NativeImplClient.GetModuleMethod(_module, "Handle_setItems");
            _handle_setSelectionMode = NativeImplClient.GetModuleMethod(_module, "Handle_setSelectionMode");
            _handle_onCurrentRowChanged = NativeImplClient.GetModuleMethod(_module, "Handle_onCurrentRowChanged");
            _handle_dispose = NativeImplClient.GetModuleMethod(_module, "Handle_dispose");

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
