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
    public static class BoxLayout
    {
        private static ModuleHandle _module;
        internal static ModuleMethodHandle _create;
        internal static ModuleMethodHandle _handle_addSpacing;
        internal static ModuleMethodHandle _handle_addStretch;
        internal static ModuleMethodHandle _handle_addWidget;
        internal static ModuleMethodHandle _handle_addWidget_overload1;
        internal static ModuleMethodHandle _handle_dispose;

        public static Handle Create(Direction dir)
        {
            Direction__Push(dir);
            NativeImplClient.InvokeModuleMethod(_create);
            return Handle__Pop();
        }
        [Flags]
        public enum Alignment
        {
            AlignLeft = 0x1,
            AlignRight = 0x2,
            AlignHCenter = 0x4,
            AlignJustify = 0x8,
            AlignTop = 0x20,
            AlignBottom = 0x40,
            AlignVCenter = 0x80,
            AlignBaseline = 0x100,
            AlignCenter = 0x84
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Alignment__Push(Alignment value)
        {
            NativeImplClient.PushUInt32((uint)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Alignment Alignment__Pop()
        {
            var ret = NativeImplClient.PopUInt32();
            return (Alignment)ret;
        }
        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            TopToBottom,
            BottomToTop
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Direction__Push(Direction value)
        {
            NativeImplClient.PushInt32((int)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Direction Direction__Pop()
        {
            var ret = NativeImplClient.PopInt32();
            return (Direction)ret;
        }
        public class Handle : Layout.Handle
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
            public void AddSpacing(int size)
            {
                NativeImplClient.PushInt32(size);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_addSpacing);
            }
            public void AddStretch(int stretch)
            {
                NativeImplClient.PushInt32(stretch);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_addStretch);
            }
            public void AddWidget(Widget.Handle widget)
            {
                Widget.Handle__Push(widget);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_addWidget);
            }
            public void AddWidget(Widget.Handle widget, int stretch)
            {
                NativeImplClient.PushInt32(stretch);
                Widget.Handle__Push(widget);
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_addWidget_overload1);
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
            _module = NativeImplClient.GetModule("BoxLayout");
            // assign module handles
            _create = NativeImplClient.GetModuleMethod(_module, "create");
            _handle_addSpacing = NativeImplClient.GetModuleMethod(_module, "Handle_addSpacing");
            _handle_addStretch = NativeImplClient.GetModuleMethod(_module, "Handle_addStretch");
            _handle_addWidget = NativeImplClient.GetModuleMethod(_module, "Handle_addWidget");
            _handle_addWidget_overload1 = NativeImplClient.GetModuleMethod(_module, "Handle_addWidget_overload1");
            _handle_dispose = NativeImplClient.GetModuleMethod(_module, "Handle_dispose");

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
