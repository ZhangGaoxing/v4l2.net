FROM microsoft/dotnet:2.1-stretch-arm32v7 AS build
WORKDIR /app

COPY src .
WORKDIR /app/V4l2.Samples
RUN dotnet restore
RUN dotnet publish -c release -r linux-arm -o out

FROM microsoft/dotnet:2.1-runtime-stretch-slim-arm32v7 AS runtime
WORKDIR /app
COPY --from=build /app/V4l2.Samples/out ./
ENTRYPOINT ["dotnet", "V4l2.Samples.dll"]