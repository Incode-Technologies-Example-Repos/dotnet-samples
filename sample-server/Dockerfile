FROM mcr.microsoft.com/dotnet/aspnet:7.0

COPY bin/Release/net7.0/publish/ /App
WORKDIR /APP
ENTRYPOINT [ "dotnet", "TokenServer.dll" ]

# Use SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["TokenServer.csproj", "./"]
RUN dotnet restore "./TokenServer.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "TokenServer.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TokenServer.csproj" -c Release -o /app/publish

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TokenServer.dll"]
