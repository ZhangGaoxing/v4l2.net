using System;
using System.Drawing;
using Iot.Device.Media;

namespace V4l2.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            VideoConnectionSettings settings = new VideoConnectionSettings(0)
            {
                CaptureSize = (1024, 768),
                PixelFormat = PixelFormat.JPEG,
                ExposureType = ExposureType.Auto
            };
            using VideoDevice device = VideoDevice.Create(settings);

            // Get the supported formats of the device
            foreach (var item in device.GetSupportedPixelFormats())
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine();

            // Get the resolutions of the format
            foreach (var item in device.GetPixelFormatResolutions(PixelFormat.YUYV))
            {
                Console.Write($"{item.Width}x{item.Height} ");
            }
            Console.WriteLine();

            // Query v4l2 controls default and current value
            var value = device.GetVideoDeviceValue(VideoDeviceValueType.Rotate);
            Console.WriteLine($"{value.Name} Min: {value.Minimum} Max: {value.Maximum} Step: {value.Step} Default: {value.DefaultValue} Current: {value.CurrentValue}");

            // Capture static image
            device.Capture("/home/pi/jpg_direct_output.jpg");

            // Change capture setting
            device.Settings.PixelFormat = PixelFormat.YUV420;

            // Convert pixel format
            Color[] colors = VideoDevice.Yv12ToRgb(device.Capture(), settings.CaptureSize);
            Bitmap bitmap = VideoDevice.RgbToBitmap(settings.CaptureSize, colors);
            bitmap.Save("/home/pi/yuyv_to_jpg.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}