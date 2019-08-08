using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Device.Media
{
    public abstract class VideoDevice : IDisposable
    {
        public static VideoDevice Create(VideoConnectionSettings settings) => new UnixVideoDevice(settings);

        public abstract void Capture(string path);

        public abstract MemoryStream Capture();

        public abstract List<PixelFormat> GetSupportedPixelFormats();

        public abstract List<(uint Width, uint Height)> GetPixelFormatResolutions(PixelFormat format);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Nothing to do in base class.
        }
    }
}
