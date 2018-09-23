#if DESKTOP

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Inferno.Runtime.Native
{
    internal class FunctionLoader
    {
        private class Windows
        {
            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibraryW(string lpszLib);
        }

        //TODO: Other OSes

        public static IntPtr LoadLibrary(string libname)
        {
            return Windows.LoadLibraryW(libname);
        }

        public static T LoadFunction<T>(IntPtr library, string function, bool throwIfNotFound = false)
        {
            var ret = IntPtr.Zero;

            ret = Windows.GetProcAddress(library, function);

            if (ret == IntPtr.Zero)
            {
                if (throwIfNotFound)
                    throw new EntryPointNotFoundException(function);

                return default(T);
            }

#if NETSTANDARD
            return Marshal.GetDelegateForFunctionPointer<T>(ret);
#else
            return (T)(object)Marshal.GetDelegateForFunctionPointer(ret, typeof(T));
#endif
        }
    }
}

#endif