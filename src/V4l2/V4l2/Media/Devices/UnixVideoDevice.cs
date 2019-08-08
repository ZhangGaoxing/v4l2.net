using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static v4l2_format;

namespace System.Device.Media
{
    internal class UnixVideoDevice : VideoDevice
    {
        private readonly VideoConnectionSettings _settings;
        private const string DefaultDevicePath = "/dev/video";
        private const int BufferCount = 4;
        private v4l2_capability capability;
        private int _deviceFileDescriptor = -1;
        private static readonly object s_initializationLock = new object();

        public string DevicePath { get; set; }

        public (uint Width, uint Height) MaxCropSize
        {
            get
            {
                v4l2_cropcap cropcap = new v4l2_cropcap()
                {
                    type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE
                };
                V4l2Struct(VideoSettings.VIDIOC_CROPCAP, ref cropcap);

                return (cropcap.bounds.width, cropcap.bounds.height);
            }
        }

        public (uint Width, uint Height) DefaulCroptSize
        {
            get
            {
                v4l2_cropcap cropcap = new v4l2_cropcap()
                {
                    type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE
                };
                V4l2Struct(VideoSettings.VIDIOC_CROPCAP, ref cropcap);

                return (cropcap.defrect.width, cropcap.defrect.height);
            }
        }

        public (int Left, int Top) DefaultCropPosition
        {
            get
            {
                v4l2_cropcap cropcap = new v4l2_cropcap()
                {
                    type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE
                };
                V4l2Struct(VideoSettings.VIDIOC_CROPCAP, ref cropcap);

                return (cropcap.defrect.left, cropcap.defrect.top);
            }
        }

        public UnixVideoDevice(VideoConnectionSettings settings)
        {
            _settings = settings;
            DevicePath = DefaultDevicePath;

            Initialize();
        }

        public override void Capture(string path)
        {
            SetCaptureSettings();
            byte[] dataBuffer = ProcessCaptureData();

            using FileStream fs = new FileStream(path, FileMode.Create);
            fs.Write(dataBuffer);
            fs.Flush();
        }

        public override unsafe MemoryStream Capture()
        {
            SetCaptureSettings();
            byte[] dataBuffer = ProcessCaptureData();

            return new MemoryStream(dataBuffer);
        }

        public override List<PixelFormat> GetSupportedPixelFormats()
        {
            v4l2_fmtdesc fmtdesc = new v4l2_fmtdesc
            {
                index = 0,
                type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE
            };

            List<PixelFormat> result = new List<PixelFormat>();
            while (V4l2Struct(VideoSettings.VIDIOC_ENUM_FMT, ref fmtdesc) != -1)
            {
                result.Add((PixelFormat)fmtdesc.pixelformat);
                fmtdesc.index++;
            }

            return result;
        }

        public override List<(uint Width, uint Height)> GetPixelFormatResolutions(PixelFormat format)
        {
            v4l2_frmsizeenum size = new v4l2_frmsizeenum()
            {
                index = 0,
                pixel_format = (uint)format
            };

            List<(uint Width, uint Height)> result = new List<(uint Width, uint Height)>();
            while (V4l2Struct(VideoSettings.VIDIOC_ENUM_FRAMESIZES, ref size) != -1)
            {
                result.Add((size.discrete.width, size.discrete.height));
                size.index++;
            }

            return result;
        }

        private unsafe byte[] ProcessCaptureData()
        {
            // Apply for buffers, use memory mapping
            v4l2_requestbuffers req = new v4l2_requestbuffers
            {
                count = BufferCount,
                type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE,
                memory = v4l2_memory.V4L2_MEMORY_MMAP
            };
            V4l2Struct(VideoSettings.VIDIOC_REQBUFS, ref req);

            // Mapping the applied buffer to user space
            V4l2FrameBuffer* buffers = stackalloc V4l2FrameBuffer[4];
            for (uint i = 0; i < BufferCount; i++)
            {
                v4l2_buffer buffer = new v4l2_buffer
                {
                    index = i,
                    type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE,
                    memory = v4l2_memory.V4L2_MEMORY_MMAP
                };
                V4l2Struct(VideoSettings.VIDIOC_QUERYBUF, ref buffer);

                buffers[i].Length = buffer.length;
                buffers[i].Start = Interop.mmap(IntPtr.Zero, (int)buffer.length, MemoryMappedProtections.PROT_READ | MemoryMappedProtections.PROT_WRITE, MemoryMappedFlags.MAP_SHARED, _deviceFileDescriptor, (int)buffer.m.offset);
            }

            // Put the buffer in the processing queue
            for (uint i = 0; i < BufferCount; i++)
            {
                v4l2_buffer buffer = new v4l2_buffer
                {
                    index = i,
                    type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE,
                    memory = v4l2_memory.V4L2_MEMORY_MMAP
                };
                V4l2Struct(VideoSettings.VIDIOC_QBUF, ref buffer);
            }

            // Start data stream
            v4l2_buf_type type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE;
            Interop.ioctl(_deviceFileDescriptor, (int)VideoSettings.VIDIOC_STREAMON, new IntPtr(&type));

            // Get one frame from the buffer
            v4l2_buffer frame = new v4l2_buffer
            {
                type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE,
                memory = v4l2_memory.V4L2_MEMORY_MMAP,
            };
            V4l2Struct(VideoSettings.VIDIOC_DQBUF, ref frame);

            // Get data from pointer
            IntPtr intptr = buffers[frame.index].Start;
            byte[] dataBuffer = new byte[buffers[frame.index].Length];
            Marshal.Copy(source: intptr, destination: dataBuffer, startIndex: 0, length: (int)buffers[frame.index].Length);

            // Requeue the buffer
            V4l2Struct(VideoSettings.VIDIOC_QBUF, ref frame);

            // Close data stream
            Interop.ioctl(_deviceFileDescriptor, (int)VideoSettings.VIDIOC_STREAMOFF, new IntPtr(&type));

            // Unmapping the applied buffer to user space
            for (uint i = 0; i < BufferCount; i++)
            {
                Interop.munmap(buffers[i].Start, (int)buffers[i].Length);
            }

            return dataBuffer;
        }

        private void Initialize()
        {
            if (_deviceFileDescriptor >= 0)
            {
                return;
            }

            string deviceFileName = $"{DevicePath}{_settings.BusId}";
            lock (s_initializationLock)
            {
                if (_deviceFileDescriptor >= 0)
                {
                    return;
                }
                _deviceFileDescriptor = Interop.open(deviceFileName, FileOpenFlags.O_RDWR);

                if (_deviceFileDescriptor < 0)
                {
                    throw new IOException($"Error {Marshal.GetLastWin32Error()}. Can not open video device file '{deviceFileName}'.");
                }

                v4l2_capability capability = new v4l2_capability();
                V4l2Struct(VideoSettings.VIDIOC_QUERYCAP, ref capability);
            }
        }

        private unsafe void SetCaptureSettings()
        {
            // Set capture format
            v4l2_format format;
            if (_settings.CaptureSize == (0, 0))
            {
                // Some cameras can't get v4l2_fmtdesc
                // If v4l2_fmtdesc is not available, it will not be set
                format = new v4l2_format
                {
                    type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE,
                    fmt = new fmt
                    {
                        pix = new v4l2_pix_format
                        {
                            pixelformat = (uint)_settings.PixelFormat
                        }
                    }
                };
            }
            else
            {
                format = new v4l2_format
                {
                    type = v4l2_buf_type.V4L2_BUF_TYPE_VIDEO_CAPTURE,
                    fmt = new fmt
                    {
                        pix = new v4l2_pix_format
                        {
                            width = _settings.CaptureSize.Width,
                            height = _settings.CaptureSize.Height,
                            pixelformat = (uint)_settings.PixelFormat
                        }
                    }
                };
            }
            V4l2Struct(VideoSettings.VIDIOC_S_FMT, ref format);
        }

        private int V4l2Struct<T>(VideoSettings request, ref T @struct)
            where T : struct
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(@struct));
            Marshal.StructureToPtr(@struct, ptr, true);

            int result = Interop.ioctl(_deviceFileDescriptor, (int)request, ptr);
            @struct = Marshal.PtrToStructure<T>(ptr);

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (_deviceFileDescriptor >= 0)
            {
                Interop.close(_deviceFileDescriptor);
                _deviceFileDescriptor = -1;
            }

            base.Dispose(disposing);
        }
    }
}
