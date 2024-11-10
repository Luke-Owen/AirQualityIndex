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

# Create a non-root user and group for better security
#RUN groupadd -g 1001 appuser && \
#    useradd -r -u 1001 -g appuser appuser

# Set the working directory for the runtime container
WORKDIR /app

# Copy the published output from the build stage to the runtime stage
COPY --from=build /app/publish ./

# Change ownership of the files to the non-root user
#RUN chown -R appuser:appuser /app

# Switch to the non-root user
#USER appuser

# Set the environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose the ports the app will run on
EXPOSE 8080
EXPOSE 8081

# Install curl command for health check
RUN apt-get update 
RUN apt-get --yes install curl

# Start the app
ENTRYPOINT ["dotnet", "AirQualityIndex.dll"]
