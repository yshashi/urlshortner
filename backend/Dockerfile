# Use the official ASP.NET runtime image as a base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET SDK image to build the .NET project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the .NET project file and restore any dependencies
COPY ["urlshortner/urlshortner.csproj", "urlshortner/"]
RUN dotnet restore "urlshortner/urlshortner.csproj"

# Copy the rest of the .NET project files
COPY . .
WORKDIR "/src/urlshortner"
RUN dotnet build "urlshortner.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Use a Node.js image to build the Angular project
FROM node:20 AS node-build
WORKDIR /src/ui
COPY ["urlshortner/ui/package*.json", "."]
RUN npm install
COPY ["urlshortner/ui", "."]
RUN npm run build

# Publish the .NET project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "urlshortner.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Create the final image to run the application
FROM base AS final
WORKDIR /app

# Copy the .NET publish result
COPY --from=publish /app/publish .

# Copy the Angular build result to the wwwroot folder
COPY --from=node-build /src/ui/dist ./wwwroot

ENTRYPOINT ["dotnet", "urlshortner.dll"]
