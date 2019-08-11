using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Iot.Device.Media
{
    public abstract partial class VideoDevice
    {
        private static Color YuvToRgb(int y, int u, int v)
        {
            byte r = (byte)(y + 1.4075 * (v - 128));
            byte g = (byte)(y - 0.3455 * (u - 128) - (0.7169 * (v - 128)));
            byte b = (byte)(y + 1.7790 * (u - 128));

            return Color.FromArgb(r, g, b);
        }

        public static Color[] YuvToRgb(MemoryStream stream)
        {
            int y, u, v;

            List<Color> colors = new List<Color>();
            while (stream.Position != stream.Length)
            {
                y = stream.ReadByte();
                u = stream.ReadByte();
                v = stream.ReadByte();

                colors.Add(YuvToRgb(y, u, v));
            }

            return colors.ToArray();
        }

        public static Color[] YuyvToRgb(MemoryStream stream)
        {
            int y0, u, y1, v;

            List<Color> colors = new List<Color>();
            while (stream.Position != stream.Length)
            {
                y0 = stream.ReadByte();
                u = stream.ReadByte();
                y1 = stream.ReadByte();
                v = stream.ReadByte();

                colors.Add(YuvToRgb(y0, u, v));
                colors.Add(YuvToRgb(y1, u, v));
            }

            return colors.ToArray();
        }

        public static Color[] Yv12ToRgb(MemoryStream stream, (uint Width, uint Height) size)
        {
            int y0, u, v;
            int width = (int)size.Width, height = (int)size.Height;
            int total = width * height;
            int shift, vShift = total / 4;

            byte[] yuv = stream.ToArray();
            
            List<Color> colors = new List<Color>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    shift = (y / 2) * (width / 2) + (x / 2);

                    y0 = yuv[y * width + x];
                    u = yuv[total + shift];
                    v = yuv[total + shift + vShift];

                    colors.Add(YuvToRgb(y0, u, v));
                }
            }

            return colors.ToArray();
        }

        public static Color[] Nv12ToRgb(MemoryStream stream, (uint Width, uint Height) size)
        {
            int y0, u, v;
            int width = (int)size.Width, height = (int)size.Height;
            int total = width * height;
            int shift;

            byte[] yuv = stream.ToArray();

            List<Color> colors = new List<Color>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    shift = y / 2 * width + x - x % 2;

                    y0 = yuv[y * width + x];
                    u = yuv[total + shift];
                    v = yuv[total + shift + 1];

                    colors.Add(YuvToRgb(y0, u, v));
                }
            }

            return colors.ToArray();
        }

        public static Bitmap RgbToBitmap((uint Width, uint Height) size, Color[] colors, System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        {
            int width = (int)size.Width, height = (int)size.Height;

            Bitmap pic = new Bitmap(width, height, format);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pic.SetPixel(x, y, colors[y * width + x]);
                }
            }

            return pic;
        }
    }
}
