using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CSharpFunctionalExtensions;
using Org.Whatever.QtTesting.Support;
using ModuleHandle = Org.Whatever.QtTesting.Support.ModuleHandle;

using static Org.Whatever.QtTesting.Common;
using static Org.Whatever.QtTesting.Signal;
using static Org.Whatever.QtTesting.Layout;

namespace Org.Whatever.QtTesting
{
    public static class Widget
    {
        private static ModuleHandle _module;
        internal static ModuleMethodHandle _create;
        internal static ModuleMethodHandle _handle_setEnabled;
        internal static ModuleMethodHandle _handle_setMaximumWidth;
        internal static ModuleMethodHandle _handle_setMaximumHeight;
        internal static ModuleMethodHandle _handle_getRect;
        internal static ModuleMethodHandle _handle_resize;
        internal static ModuleMethodHandle _handle_show;
        internal static ModuleMethodHandle _handle_hide;
        internal static ModuleMethodHandle _handle_setVisible;
        internal static ModuleMethodHandle _handle_setWindowTitle;
        internal static ModuleMethodHandle _handle_setLayout;
        internal static ModuleMethodHandle _handle_getLayout;
        internal static ModuleMethodHandle _handle_onWindowTitleChanged;
        internal static ModuleMethodHandle _handle_dispose;
        public static int WIDGET_SIZE_MAX { get; internal set; }

        public static Handle Create()
        {
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
            public void SetEnabled(bool state)
            {
                NativeImplClient.PushBool(state);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setEnabled);
            }
            public void SetMaximumWidth(int maxWidth)
            {
                NativeImplClient.PushInt32(maxWidth);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setMaximumWidth);
            }
            public void SetMaximumHeight(int maxHeight)
            {
                NativeImplClient.PushInt32(maxHeight);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setMaximumHeight);
            }
            public Rect GetRect()
            {
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_getRect);
                return Rect__Pop();
            }
            public void Resize(int width, int height)
            {
                NativeImplClient.PushInt32(height);
                NativeImplClient.PushInt32(width);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_resize);
            }
            public void Show()
            {
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_show);
            }
            public void Hide()
            {
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_hide);
            }
            public void SetVisible(bool state)
            {
                NativeImplClient.PushBool(state);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setVisible);
            }
            public void SetWindowTitle(string title)
            {
                NativeImplClient.PushString(title);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setWindowTitle);
            }
            public void SetLayout(Layout.Handle layout)
            {
                Layout.Handle__Push(layout);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_setLayout);
            }
            public Layout.Handle GetLayout()
            {
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_getLayout);
                return Layout.Handle__Pop();
            }
            public void OnWindowTitleChanged(StringDelegate func)
            {
                StringDelegate__Push(func);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_onWindowTitleChanged);
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
            _module = NativeImplClient.GetModule("Widget");
            // assign module constants
            NativeImplClient.PushModuleConstants(_module);
            WIDGET_SIZE_MAX = NativeImplClient.PopInt32();
            // assign module handles
            _create = NativeImplClient.GetModuleMethod(_module, "create");
            _handle_setEnabled = NativeImplClient.GetModuleMethod(_module, "Handle_setEnabled");
            _handle_setMaximumWidth = NativeImplClient.GetModuleMethod(_module, "Handle_setMaximumWidth");
            _handle_setMaximumHeight = NativeImplClient.GetModuleMethod(_module, "Handle_setMaximumHeight");
            _handle_getRect = NativeImplClient.GetModuleMethod(_module, "Handle_getRect");
            _handle_resize = NativeImplClient.GetModuleMethod(_module, "Handle_resize");
            _handle_show = NativeImplClient.GetModuleMethod(_module, "Handle_show");
            _handle_hide = NativeImplClient.GetModuleMethod(_module, "Handle_hide");
            _handle_setVisible = NativeImplClient.GetModuleMethod(_module, "Handle_setVisible");
            _handle_setWindowTitle = NativeImplClient.GetModuleMethod(_module, "Handle_setWindowTitle");
            _handle_setLayout = NativeImplClient.GetModuleMethod(_module, "Handle_setLayout");
            _handle_getLayout = NativeImplClient.GetModuleMethod(_module, "Handle_getLayout");
            _handle_onWindowTitleChanged = NativeImplClient.GetModuleMethod(_module, "Handle_onWindowTitleChanged");
            _handle_dispose = NativeImplClient.GetModuleMethod(_module, "Handle_dispose");

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
