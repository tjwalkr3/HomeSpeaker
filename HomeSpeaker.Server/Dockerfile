#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt update && apt install --yes sox libsox-fmt-mp3 alsa-utils

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-arm64v8 AS build
# add debian backports to get a fresh grpc plugin
RUN echo "deb http://deb.debian.org/debian buster-backports main" | sudo tee /etc/apt/sources.list.d/backports.list

# add debian keys to use backports
RUN sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 7638D0442B90D010
RUN sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 04EE7237B7D453EC

# install protoc+plugin
RUN sudo apt update
RUN sudo apt install -t buster-backports protobuf-compiler-grpc

# tell dotnet to use the local protoc
RUN export PROTOBUF_PROTOC=/usr/bin/protoc

# trick dotnet to use the local grpc plugin as there are no environment variable to help
RUN ln -s /usr/bin ~/.nuget/packages/grpc.tools/2.29.0/tools/linux_

WORKDIR /src
COPY ["HomeSpeaker.Server/HomeSpeaker.Server.csproj", "HomeSpeaker.Server/"]
COPY ["HomeSpeaker.Shared/HomeSpeaker.Shared.csproj", "HomeSpeaker.Shared/"]
RUN dotnet restore "HomeSpeaker.Server/HomeSpeaker.Server.csproj"
COPY . .
WORKDIR "/src/HomeSpeaker.Server"
RUN dotnet build "HomeSpeaker.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeSpeaker.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeSpeaker.Server.dll"]