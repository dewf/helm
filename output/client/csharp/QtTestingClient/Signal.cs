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
    public static class Signal
    {
        private static ModuleHandle _module;
        public delegate void VoidDelegate();

        internal static void VoidDelegate__Push(VoidDelegate callback)
        {
            void CallbackWrapper()
            {
                callback();
            }
            NativeImplClient.PushClientFuncVal(CallbackWrapper, Marshal.GetFunctionPointerForDelegate(callback));
        }

        internal static VoidDelegate VoidDelegate__Pop()
        {
            var id = NativeImplClient.PopServerFuncValId();
            var remoteFunc = new ServerFuncVal(id);
            void Wrapper()
            {
                remoteFunc.Exec();
            }
            return Wrapper;
        }
        public delegate void StringDelegate(string s);

        internal static void StringDelegate__Push(StringDelegate callback)
        {
            void CallbackWrapper()
            {
                var s = NativeImplClient.PopString();
                callback(s);
            }
            NativeImplClient.PushClientFuncVal(CallbackWrapper, Marshal.GetFunctionPointerForDelegate(callback));
        }

        internal static StringDelegate StringDelegate__Pop()
        {
            var id = NativeImplClient.PopServerFuncValId();
            var remoteFunc = new ServerFuncVal(id);
            void Wrapper(string s)
            {
                NativeImplClient.PushString(s);
                remoteFunc.Exec();
            }
            return Wrapper;
        }

        internal static void __Init()
        {
            _module = NativeImplClient.GetModule("Signal");
            // assign module handles

            // no static init
        }

        internal static void __Shutdown()
        {
            // no static shutdown
        }
    }
}
