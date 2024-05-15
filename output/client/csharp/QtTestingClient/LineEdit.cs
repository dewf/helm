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
    public static class LineEdit
    {
        private static ModuleHandle _module;
        internal static ModuleMethodHandle _create;
        internal static ModuleMethodHandle _handle_setText;
        internal static ModuleMethodHandle _handle_onTextEdited;
        internal static ModuleMethodHandle _handle_onReturnPressed;
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
            public void SetText(string str)
            {
                NativeImplClient.PushString(str);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setText);
            }
            public void OnTextEdited(StringDelegate handler)
            {
                StringDelegate__Push(handler);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_onTextEdited);
            }
            public void OnReturnPressed(VoidDelegate handler)
            {
                VoidDelegate__Push(handler);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_onReturnPressed);
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
            _module = NativeImplClient.GetModule("LineEdit");
            // assign module handles
            _create = NativeImplClient.GetModuleMethod(_module, "create");
            _handle_setText = NativeImplClient.GetModuleMethod(_module, "Handle_setText");
            _handle_onTextEdited = NativeImplClient.GetModuleMethod(_module, "Handle_onTextEdited");
            _handle_onReturnPressed = NativeImplClient.GetModuleMethod(_module, "Handle_onReturnPressed");
            _handle_dispose = NativeImplClient.GetModuleMethod(_module, "Handle_dispose");

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
