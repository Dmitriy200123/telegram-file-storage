FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TelegramAuth/TelegramAuth.csproj", "TelegramAuth/"]
RUN dotnet restore "TelegramAuth/TelegramAuth.csproj"
COPY . .
WORKDIR "/src/TelegramAuth"
RUN dotnet build "TelegramAuth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramAuth.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=publish /app/publish .
COPY ../Config  ../Config
ENTRYPOINT ["dotnet", "TelegramAuth.dll"]
