// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um\profileapi.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.InteropServices;
using System.Security;

namespace TerraFX.Interop
{
    public static unsafe partial class Kernel32
    {
        #region External Methods
        [DllImport(DllName, BestFitMapping = false, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, EntryPoint = "QueryPerformanceCounter", ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        [SuppressUnmanagedCodeSecurity]
        [return: ComAliasName("BOOL")]
        public static extern int QueryPerformanceCounter(
            [Out] out LARGE_INTEGER lpPerformanceCount
        );

        [DllImport(DllName, BestFitMapping = false, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, EntryPoint = "QueryPerformanceFrequency", ExactSpelling = true, PreserveSig = true, SetLastError = true, ThrowOnUnmappableChar = false)]
        [SuppressUnmanagedCodeSecurity]
        [return: ComAliasName("BOOL")]
        public static extern int QueryPerformanceFrequency(
            [Out] out LARGE_INTEGER lpFrequency
        );
        #endregion
    }
}
