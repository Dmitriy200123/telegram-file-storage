# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
#
# copy csproj and restore as distinct layers
COPY *.sln .

COPY Config/. ./Config

COPY FileStorageAPI/*.csproj ./FileStorageAPI/
COPY SearchDocumentsAPI/*.csproj ./SearchDocumentsAPI/
COPY DocumentClassificationsAPI/*.csproj ./DocumentClassificationsAPI/

COPY FilesStorage/*.csproj ./FilesStorage/
COPY InfoStorage/*.csproj ./InfoStorage/
COPY JwtAuth/*.csproj ./JwtAuth/
COPY TelegramAuth/*.csproj ./TelegramAuth/
COPY DocumentsIndex/*.csproj ./DocumentsIndex/
COPY RightServices/*.csproj ./RightServices/
COPY TokenGenerator/*.csproj ./TokenGenerator/

COPY API/*.csproj ./API/
COPY Data/*.csproj ./Data/
COPY DocumentsIndex.Contracts/*.csproj ./DocumentsIndex.Contracts/
COPY DataBaseFiller/*.csproj ./DataBaseFiller/

COPY FileStorageAPI.Tests/*.csproj ./FileStorageAPI.Tests/
COPY FilesStorageTests/*.csproj ./FilesStorageTests/
COPY InfoStorage.Tests/*.csproj ./InfoStorage.Tests/
COPY DocumentsIndex.Tests/*.csproj ./DocumentsIndex.Tests/
COPY DocumentsIndex.Benchmarks/*.csproj ./DocumentsIndex.Benchmarks/
COPY SearchDocumentsAPI.Tests/*.csproj ./SearchDocumentsAPI.Tests/

RUN dotnet restore
#
# copy everything else and build app
COPY FileStorageAPI/. ./FileStorageAPI/
COPY FilesStorage/. ./FilesStorage/
COPY InfoStorage/. ./InfoStorage/
COPY DataBaseFiller/. ./DataBaseFiller
COPY JwtAuth/. ./JwtAuth
COPY API/. ./API
COPY Data/. ./Data
COPY DocumentsIndex.Contracts/. ./DocumentsIndex.Contracts
COPY DocumentsIndex/. ./DocumentsIndex/
COPY SearchDocumentsAPI/. ./SearchDocumentsAPI
COPY DocumentClassificationsAPI/. ./DocumentClassificationsAPI
COPY RightServices/. ./RightServices/
COPY TokenGenerator/. ./TokenGenerator/

# Собираем часть с основным апи
FROM build AS build_FileStorageAPI

WORKDIR /app/FileStorageAPI
RUN dotnet publish -c Debug -o out -p:UseAppHost=false

# Собираем апи для индексации
FROM build AS build_SearchDocumentsAPI

WORKDIR /app/SearchDocumentsAPI
RUN dotnet publish -c Debug -o out -p:UseAppHost=false

# Собираем апи для классификации документов
FROM build AS build_DocumentClassificationsAPI

WORKDIR /app/DocumentClassificationsAPI
RUN dotnet publish -c Debug -o out -p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base_runtime

WORKDIR /app
COPY --from=build /app/Config ../Config

# Запускаем основное апи
FROM base_runtime AS runtime_FileStorageAPI

COPY --from=build_FileStorageAPI /app/FileStorageAPI/out ./

ENTRYPOINT ["dotnet", "FileStorageAPI.dll"]

# Запускаем апи для индекасции
FROM base_runtime AS runtime_SearchDocumentsAPI

COPY --from=build_SearchDocumentsAPI /app/SearchDocumentsAPI/out ./

ENTRYPOINT ["dotnet", "SearchDocumentsAPI.dll"]

# Запускаем апи для классификаций
FROM base_runtime AS runtime_DocumentClassificationsAPI

COPY --from=build_DocumentClassificationsAPI /app/DocumentClassificationsAPI/out ./

ENTRYPOINT ["dotnet", "DocumentClassificationsAPI.dll"]

