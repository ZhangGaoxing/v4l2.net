using System;
using System.Device.Media;

namespace V4l2.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            VideoConnectionSettings settings = new VideoConnectionSettings(0)
            {
                CaptureSize = (1280, 768),
                PixelFormat = PixelFormat.JPEG
            };
            using VideoDevice device = VideoDevice.Create(settings);

            foreach (var item in device.GetSupportedPixelFormat())
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine();

            device.Capture("/home/pi/test.jpg");
        }
    }
}
