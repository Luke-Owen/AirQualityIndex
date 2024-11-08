FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY AirQualityIndex/AirQualityIndex.csproj ./
RUN dotnet restore 

COPY . .

RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "AirQualityIndex.dll"]