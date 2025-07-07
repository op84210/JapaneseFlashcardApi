# 使用官方的 .NET 8.0 運行時映像作為基礎映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 使用 .NET 8.0 SDK 映像進行建置
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 複製專案檔案
COPY ["JapaneseFlashcardApi.csproj", "."]
RUN dotnet restore "./JapaneseFlashcardApi.csproj"

# 複製所有原始碼
COPY . .
WORKDIR "/src/."
RUN dotnet build "./JapaneseFlashcardApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 發布應用程式
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JapaneseFlashcardApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 最終映像
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 設定環境變數
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# 建立非特權使用者
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "JapaneseFlashcardApi.dll"]
