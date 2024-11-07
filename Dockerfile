FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY AirQualityIndex.sln .
COPY AirQualityIndex/AirQualityIndex.csproj ./AirQualityIndex/
RUN dotnet restore ./AirQualityIndex/AirQualityIndex.csproj

COPY ./AirQualityIndex/ ./AirQualityIndex/
WORKDIR /src/AirQualityIndex
RUN dotnet build -c $BUILD_CONFIGURATION -o app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AirQualityIndex.dll"]
