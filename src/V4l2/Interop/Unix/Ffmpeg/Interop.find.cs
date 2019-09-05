// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal unsafe partial class Interop
{
    [DllImport(LibavdeviceLibrary, SetLastError = true)]
    public static extern AVInputFormat* av_find_input_format([MarshalAs(UnmanagedType.LPStr)] string short_name);

    [DllImport(LibavdeviceLibrary, SetLastError = true)]
    public static extern int av_dict_set(AVDictionary** pm, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPStr)] string value, int flags);
}
