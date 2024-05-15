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
using static Org.Whatever.QtTesting.Layout;

namespace Org.Whatever.QtTesting
{
    public static class MainWindow
    {
        private static ModuleHandle _module;
        internal static ModuleMethodHandle _create;
        internal static ModuleMethodHandle _handle_setCentralWidget;
        internal static ModuleMethodHandle _handle_dispose;

        public static Handle Create(Widget.Handle parent, Kind kind)
        {
            Kind__Push(kind);
            Widget.Handle__Push(parent);
            NativeImplClient.InvokeModuleMethod(_create);
            return Handle__Pop();
        }
        public enum Kind
        {
            Widget,
            Window,
            Dialog,
            Sheet,
            Drawer,
            Popup,
            Tool,
            ToolTip,
            SplashScreen,
            SubWindow,
            ForeignWindow,
            CoverWindow
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Kind__Push(Kind value)
        {
            NativeImplClient.PushInt32((int)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Kind Kind__Pop()
        {
            var ret = NativeImplClient.PopInt32();
            return (Kind)ret;
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
            public void SetCentralWidget(Widget.Handle widget)
            {
                Widget.Handle__Push(widget);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setCentralWidget);
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
            _module = NativeImplClient.GetModule("MainWindow");
            // assign module handles
            _create = NativeImplClient.GetModuleMethod(_module, "create");
            _handle_setCentralWidget = NativeImplClient.GetModuleMethod(_module, "Handle_setCentralWidget");
            _handle_dispose = NativeImplClient.GetModuleMethod(_module, "Handle_dispose");

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
