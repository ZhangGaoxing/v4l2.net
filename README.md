# v4l2.net
V4L2 tool implemented by .NET

## Run the sample with Docker
```
docker build -t v4l2-sample -f Dockerfile .
docker run --rm -it --device /dev/video0 -v /home/pi v4l2-sample
```