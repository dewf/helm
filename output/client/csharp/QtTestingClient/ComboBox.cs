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
    public static class ComboBox
    {
        private static ModuleHandle _module;

        // built-in array type: string[]
        internal static ModuleMethodHandle _create;
        internal static ModuleMethodHandle _handle_clear;
        internal static ModuleMethodHandle _handle_setItems;
        internal static ModuleMethodHandle _handle_setCurrentIndex;
        internal static ModuleMethodHandle _handle_onCurrentIndexChanged;
        internal static ModuleMethodHandle _handle_onCurrentTextChanged;
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
            public void Clear()
            {
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_clear);
            }
            public void SetItems(string[] items)
            {
                NativeImplClient.PushStringArray(items);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setItems);
            }
            public void SetCurrentIndex(int index)
            {
                NativeImplClient.PushInt32(index);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setCurrentIndex);
            }
            public void OnCurrentIndexChanged(IntDelegate handler)
            {
                IntDelegate__Push(handler);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_onCurrentIndexChanged);
            }
            public void OnCurrentTextChanged(StringDelegate handler)
            {
                StringDelegate__Push(handler);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_onCurrentTextChanged);
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
            _handle_clear = NativeImplClient.GetModuleMethod(_module, "Handle_clear");
            _handle_setItems = NativeImplClient.GetModuleMethod(_module, "Handle_setItems");
            _handle_setCurrentIndex = NativeImplClient.GetModuleMethod(_module, "Handle_setCurrentIndex");
            _handle_onCurrentIndexChanged = NativeImplClient.GetModuleMethod(_module, "Handle_onCurrentIndexChanged");
            _handle_onCurrentTextChanged = NativeImplClient.GetModuleMethod(_module, "Handle_onCurrentTextChanged");
            _handle_dispose = NativeImplClient.GetModuleMethod(_module, "Handle_dispose");

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
