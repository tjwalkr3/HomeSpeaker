#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-focal-slim-arm32v7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-focal-arm32v7 AS build
WORKDIR /src
COPY ["HomeSpeaker.Web/HomeSpeaker.Web.csproj", "HomeSpeaker.Web/"]
COPY ["HomeSpeaker.Lib/HomeSpeaker.Lib.csproj", "HomeSpeaker.Lib/"]
RUN dotnet restore "HomeSpeaker.Web/HomeSpeaker.Web.csproj"
COPY . .
WORKDIR "/src/HomeSpeaker.Web"
RUN dotnet build "HomeSpeaker.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeSpeaker.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeSpeaker.Web.dll"]
