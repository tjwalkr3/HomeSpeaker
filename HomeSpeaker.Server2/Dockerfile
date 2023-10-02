#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt update && apt install --yes sox libsox-fmt-mp3 alsa-utils vlc ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HomeSpeaker.Server2/HomeSpeaker.Server2.csproj", "HomeSpeaker.Server2/"]
RUN dotnet restore "HomeSpeaker.Server2/HomeSpeaker.Server2.csproj"
COPY . .
WORKDIR "/src/HomeSpeaker.Server2"
RUN dotnet build "HomeSpeaker.Server2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeSpeaker.Server2.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN useradd -ms /bin/bash homespeakeruser
RUN usermod -aG audio homespeakeruser
RUN usermod -aG video homespeakeruser
USER homespeakeruser
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeSpeaker.Server2.dll"]