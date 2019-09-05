// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    [DllImport(LibavdeviceLibrary, SetLastError = true)]
    internal static extern void avdevice_register_all();

    [DllImport(LibavdeviceLibrary, SetLastError = true)]
    internal static extern void avcodec_register_all();

    [DllImport(LibavdeviceLibrary, SetLastError = true)]
    internal static extern void av_register_all();
}
