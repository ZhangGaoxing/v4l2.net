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
                CaptureSize = (2560, 1920),
                PixelFormat = PixelFormat.JPEG
            };
            using VideoDevice device = VideoDevice.Create(settings);

            foreach (var item in device.GetSupportedPixelFormats())
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine();

            foreach (var item in device.GetPixelFormatResolutions(PixelFormat.YUYV))
            {
                Console.Write($"{item.Width}x{item.Height} ");
            }
            Console.WriteLine();

            device.Capture("/home/pi/test.jpg");
        }
    }
}
