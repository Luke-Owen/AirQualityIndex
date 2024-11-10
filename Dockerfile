# Use .NET Core SDK as build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore any dependencies
COPY AirQualityIndex/AirQualityIndex.csproj ./
RUN dotnet restore 

# Copy the rest of the application code
COPY . .

# Publish the application
RUN dotnet publish -c Debug -o publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Set the environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose the ports the app will run on
EXPOSE 8080
EXPOSE 8081

# Start the app
ENTRYPOINT ["dotnet", "AirQualityIndex.dll"]
