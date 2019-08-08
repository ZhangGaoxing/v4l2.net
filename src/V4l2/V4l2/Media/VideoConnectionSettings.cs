using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Device.Media
{
    public class VideoConnectionSettings
    {
        public VideoConnectionSettings(int busId)
        {
            BusId = busId;
        }

        public int BusId { get; }
        public (uint Width, uint Height) CaptureSize { get; set; } = (0, 0);
        public PixelFormat PixelFormat { get; set; } = PixelFormat.YUYV;
    }
}
