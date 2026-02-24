# Use official .NET 8 ASP.NET image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy project files and restore dependencies
COPY EasyLog/EasyLog.csproj EasyLog/
COPY EasyLogAPI/EasyLogAPI.csproj EasyLogAPI/
RUN dotnet restore EasyLogAPI/EasyLogAPI.csproj
# Copy all source files
COPY EasyLog/ EasyLog/
COPY EasyLogAPI/ EasyLogAPI/
# Flatten nested EasyLogAPI/EasyLogAPI/ to avoid apphost naming conflict on Linux
RUN if [ -d "EasyLogAPI/EasyLogAPI" ]; then \
      cp -rn EasyLogAPI/EasyLogAPI/* EasyLogAPI/ 2>/dev/null || true; \
      rm -rf EasyLogAPI/EasyLogAPI; \
    fi
# Publish the application in Release mode to /app/publish
RUN dotnet publish EasyLogAPI/EasyLogAPI.csproj -c Release -o /app/publish

# Prepare the final image
FROM base AS final
WORKDIR /app
# Copy published files from build stage
COPY --from=build /app/publish .
# Expose port 8080 for API access
EXPOSE 8080
# Set ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
# Start the application
ENTRYPOINT ["dotnet", "EasyLogAPI.dll"]
