FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /src
COPY ["HomeSpeaker.WebAssembly/HomeSpeaker.WebAssembly.csproj", "HomeSpeaker.WebAssembly/"]
RUN dotnet restore "HomeSpeaker.WebAssembly/HomeSpeaker.WebAssembly.csproj"
COPY . .
WORKDIR /src/HomeSpeaker.WebAssembly
RUN dotnet publish -c Release -o /app/output

FROM nginx:alpine
WORKDIR /var/www/web
COPY --from=build-env /app/output/wwwroot .
COPY HomeSpeaker.WebAssembly/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80