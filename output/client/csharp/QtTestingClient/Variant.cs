using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CSharpFunctionalExtensions;
using Org.Whatever.QtTesting.Support;
using ModuleHandle = Org.Whatever.QtTesting.Support.ModuleHandle;

using static Org.Whatever.QtTesting.Icon;
using static Org.Whatever.QtTesting.PaintResources;

namespace Org.Whatever.QtTesting
{
    public static class Variant
    {
        private static ModuleHandle _module;
        internal static ModuleMethodHandle _handle_isValid;
        internal static ModuleMethodHandle _handle_toString2;
        internal static ModuleMethodHandle _handle_toInt;
        internal static ModuleMethodHandle _handle_dispose;
        internal static ModuleMethodHandle _ownedHandle_dispose;
        public class Handle : IDisposable, IComparable
        {
            internal readonly IntPtr NativeHandle;
            protected bool _disposed;
            internal Handle(IntPtr nativeHandle)
            {
                NativeHandle = nativeHandle;
            }
            public int CompareTo(object obj)
            {
                if (obj is Handle other)
                {
                    return NativeHandle.CompareTo(other.NativeHandle);
                }
                throw new Exception("CompareTo: wrong type");
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
            public bool IsValid()
            {
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_isValid);
                return NativeImplClient.PopBool();
            }
            public string ToString2()
            {
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_toString2);
                return NativeImplClient.PopString();
            }
            public int ToInt()
            {
                Handle__Push(this);
                NativeImplClient.InvokeModuleMethod(_handle_toInt);
                return NativeImplClient.PopInt32();
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
        public class OwnedHandle : Handle
        {
            internal OwnedHandle(IntPtr nativeHandle) : base(nativeHandle)
            {
            }
            public override void Dispose()
            {
                if (!_disposed)
                {
                    OwnedHandle__Push(this);
                    NativeImplClient.InvokeModuleMethod(_ownedHandle_dispose);
                    _disposed = true;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void OwnedHandle__Push(OwnedHandle thing)
        {
            NativeImplClient.PushPtr(thing?.NativeHandle ?? IntPtr.Zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static OwnedHandle OwnedHandle__Pop()
        {
            var ptr = NativeImplClient.PopPtr();
            return ptr != IntPtr.Zero ? new OwnedHandle(ptr) : null;
        }
        public abstract record Deferred
        {
            internal abstract void Push(bool isReturn);
            internal static Deferred Pop()
            {
                return NativeImplClient.PopInt32() switch
                {
                    0 => Empty.PopDerived(),
                    1 => FromString.PopDerived(),
                    2 => FromInt.PopDerived(),
                    3 => FromIcon.PopDerived(),
                    4 => FromColor.PopDerived(),
                    _ => throw new Exception("Deferred.Pop() - unknown tag!")
                };
            }
            public sealed record Empty : Deferred
            {
                internal override void Push(bool isReturn)
                {
                    // kind
                    NativeImplClient.PushInt32(0);
                }
                internal static Empty PopDerived()
                {
                    return new Empty();
                }
            }
            public sealed record FromString(string Value) : Deferred
            {
                public string Value { get; } = Value;
                internal override void Push(bool isReturn)
                {
                    NativeImplClient.PushString(Value);
                    // kind
                    NativeImplClient.PushInt32(1);
                }
                internal static FromString PopDerived()
                {
                    var value = NativeImplClient.PopString();
                    return new FromString(value);
                }
            }
            public sealed record FromInt(int Value) : Deferred
            {
                public int Value { get; } = Value;
                internal override void Push(bool isReturn)
                {
                    NativeImplClient.PushInt32(Value);
                    // kind
                    NativeImplClient.PushInt32(2);
                }
                internal static FromInt PopDerived()
                {
                    var value = NativeImplClient.PopInt32();
                    return new FromInt(value);
                }
            }
            public sealed record FromIcon(Icon.Deferred Value) : Deferred
            {
                public Icon.Deferred Value { get; } = Value;
                internal override void Push(bool isReturn)
                {
                    Icon.Deferred__Push(Value, isReturn);
                    // kind
                    NativeImplClient.PushInt32(3);
                }
                internal static FromIcon PopDerived()
                {
                    var value = Icon.Deferred__Pop();
                    return new FromIcon(value);
                }
            }
            public sealed record FromColor(Color.Deferred Value) : Deferred
            {
                public Color.Deferred Value { get; } = Value;
                internal override void Push(bool isReturn)
                {
                    Color.Deferred__Push(Value, isReturn);
                    // kind
                    NativeImplClient.PushInt32(4);
                }
                internal static FromColor PopDerived()
                {
                    var value = Color.Deferred__Pop();
                    return new FromColor(value);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Deferred__Push(Deferred thing, bool isReturn)
        {
            thing.Push(isReturn);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Deferred Deferred__Pop()
        {
            return Deferred.Pop();
        }

        internal static void __Init()
        {
            _module = NativeImplClient.GetModule("Variant");
            // assign module handles
            _handle_isValid = NativeImplClient.GetModuleMethod(_module, "Handle_isValid");
            _handle_toString2 = NativeImplClient.GetModuleMethod(_module, "Handle_toString2");
            _handle_toInt = NativeImplClient.GetModuleMethod(_module, "Handle_toInt");
            _handle_dispose = NativeImplClient.GetModuleMethod(_module, "Handle_dispose");
            _ownedHandle_dispose = NativeImplClient.GetModuleMethod(_module, "OwnedHandle_dispose");

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
