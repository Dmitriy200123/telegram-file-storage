# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app 
#
# copy csproj and restore as distinct layers
COPY *.sln .
COPY FileStorageAPI/*.csproj ./FileStorageAPI/
COPY FilesStorage/*.csproj ./FilesStorage/
COPY InfoStorage/*.csproj ./InfoStorage/

COPY FileStorageAPI.Tests/*.csproj ./FileStorageAPI.Tests/
COPY FilesStorageTests/*.csproj ./FilesStorageTests/
COPY InfoStorage.Tests/*.csproj ./InfoStorage.Tests/
#
RUN dotnet restore 
#
# copy everything else and build app
COPY FileStorageAPI/. ./FileStorageAPI/
COPY FilesStorage/. ./FilesStorage/
COPY InfoStorage/. ./InfoStorage/
#
WORKDIR /app/FileStorageAPI
RUN dotnet publish -c Debug -o out -p:UseAppHost=false
#
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app 
#
COPY --from=build /app/FileStorageAPI/out ./

ENTRYPOINT ["dotnet", "FileStorageAPI.dll"]

