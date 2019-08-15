FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch-arm32v7 AS build
# FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /app

# publish app
COPY src .
WORKDIR /app/V4l2.Samples
RUN dotnet restore
RUN dotnet publish -c release -r linux-arm -o out

## run app
FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim-arm32v7 AS runtime
# FROM mcr.microsoft.com/dotnet/core/runtime:2.1 AS runtime
WORKDIR /app
COPY --from=build /app/V4l2.Samples/out ./

# configure apt sources
########################
#        Debian        #
########################
# RUN mv /etc/apt/sources.list /etc/apt/sources.list.bak && \
#     echo "deb https://mirrors.tuna.tsinghua.edu.cn/debian/ stretch main contrib non-free" >/etc/apt/sources.list && \
#     echo "deb-src https://mirrors.tuna.tsinghua.edu.cn/debian/ stretch main contrib non-free" >>/etc/apt/sources.list && \
#     echo "deb https://mirrors.tuna.tsinghua.edu.cn/debian/ stretch-updates main contrib non-free" >>/etc/apt/sources.list && \
#     echo "deb-src https://mirrors.tuna.tsinghua.edu.cn/debian/ stretch-updates main contrib non-free" >>/etc/apt/sources.list && \
#     echo "deb https://mirrors.tuna.tsinghua.edu.cn/debian/ stretch-backports main contrib non-free" >>/etc/apt/sources.list && \
#     echo "deb-src https://mirrors.tuna.tsinghua.edu.cn/debian/ stretch-backports main contrib non-free" >>/etc/apt/sources.list && \
#     echo "deb https://mirrors.tuna.tsinghua.edu.cn/debian-security stretch/updates main contrib non-free" >>/etc/apt/sources.list && \
#     echo "deb-src https://mirrors.tuna.tsinghua.edu.cn/debian-security stretch/updates main contrib non-free" >>/etc/apt/sources.list

########################
#       Raspbian       #
########################
# RUN mv /etc/apt/sources.list /etc/apt/sources.list.bak && \
#     echo "deb http://mirrors.tuna.tsinghua.edu.cn/raspbian/raspbian/ stretch main non-free contrib" >/etc/apt/sources.list && \
#     echo "deb-src http://mirrors.tuna.tsinghua.edu.cn/raspbian/raspbian/ stretch main non-free contrib" >>/etc/apt/sources.list

# install System.Drawing native dependencies
RUN apt-get update && \
    apt-get install -y --allow-unauthenticated libc6-dev libgdiplus libx11-dev && \
    rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "V4l2.Samples.dll"]