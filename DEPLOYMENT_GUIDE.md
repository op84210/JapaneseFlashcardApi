# 雲端部署指南

## 🚀 部署方案選擇

### 方案 1: Azure App Service + Azure SQL (推薦新手)
**優點**: 簡單快速、自動化管理、與 .NET 完美整合
**缺點**: 成本較高
**適合**: 想要快速上線，不想管理基礎設施

### 方案 2: Docker 容器化部署 (推薦進階)
**優點**: 靈活性高、可移植性好、成本可控
**缺點**: 需要一些 Docker 知識
**適合**: 想要學習容器技術，有一定技術基礎

### 方案 3: 虛擬機器部署 (最經濟)
**優點**: 成本最低、完全控制
**缺點**: 需要管理作業系統和安全更新
**適合**: 預算有限，有系統管理經驗

### 方案 4: Railway 部署 (推薦快速上線)
**優點**: 
- 極簡部署流程 (3 分鐘上線)
- 包含免費 PostgreSQL 資料庫
- 現代化介面和開發體驗
- 無應用程式睡眠問題
- 優秀的 GitHub 整合

**缺點**: 
- 相對較新的平台
- 企業級功能較少

**適合**: 想要快速驗證想法、學習專案、小型應用
**成本**: $5/月 (包含資料庫)

### 方案 5: Heroku 部署 (傳統穩定選擇)
**優點**: 
- 久經考驗的平台
- 豐富的附加元件生態系統
- 企業級支援
- 詳細的文檔和社群

**缺點**: 
- 成本較高 ($12+/月)
- 部署速度較慢
- Eco dyno 會自動睡眠
- 介面較老舊

**適合**: 需要企業級穩定性、豐富附加元件
**成本**: $7/月 (應用) + $5/月 (資料庫) = $12/月

## 🏆 平台選擇建議

### 對於日文單字卡專案：
1. **🥇 Railway** - 最佳選擇 (快速、便宜、現代)
2. **🥈 Azure App Service** - 企業級 (穩定、功能完整)
3. **🥉 Heroku** - 傳統選擇 (穩定但昂貴)

## 🚂 Railway 快速部署指南

### 1. 準備專案
確保專案包含以下檔案：

```bash
# railway.toml (可選配置)
[build]
builder = "NIXPACKS"

[deploy]
startCommand = "dotnet JapaneseFlashcardApi.dll"
restartPolicyType = "ON_FAILURE"
restartPolicyMaxRetries = 10
```

### 2. Railway 部署步驟

1. **註冊 Railway 帳戶**
   - 訪問 [railway.app](https://railway.app)
   - 使用 GitHub 帳戶登入

2. **連接 GitHub 存儲庫**
   ```bash
   # 創建新專案
   railway login
   railway init
   railway link
   ```

3. **配置環境變數**
   ```bash
   # 在 Railway 儀表板設定
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://0.0.0.0:$PORT
   ```

4. **添加 PostgreSQL 資料庫**
   - 在 Railway 儀表板點擊 "Add Service"
   - 選擇 "PostgreSQL"
   - 自動獲得 DATABASE_URL 環境變數

5. **自動部署**
   - 推送代碼到 GitHub
   - Railway 自動觸發部署
   - 2-3 分鐘內完成部署

### 3. Railway 配置檔案

創建 `railway.toml` 檔案：
```toml
[build]
builder = "nixpacks"
buildCommand = "dotnet publish -c Release -o out"

[deploy]
startCommand = "dotnet out/JapaneseFlashcardApi.dll"
healthcheckPath = "/health"
healthcheckTimeout = 300
restartPolicyType = "ON_FAILURE"
restartPolicyMaxRetries = 10

[experimental]
configAsCode = true
```

### 4. 修改程式碼以支援 Railway

需要修改 `Program.cs` 以支援 Railway 的 PORT 環境變數：

```csharp
var builder = WebApplication.CreateBuilder(args);

// Railway PORT 支援
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ...existing code...

var app = builder.Build();

// 健康檢查端點
app.MapGet("/health", () => "OK");

app.Run();
```

## 📋 Azure App Service 部署步驟

### 1. 準備工作
- Azure 帳戶 (免費額度 $200)
- Visual Studio 或 Azure CLI
- .NET 8.0 SDK

### 2. 建立 Azure 資源

```bash
# 登入 Azure
az login

# 建立資源群組
az group create --name rg-flashcard-api --location "East Asia"

# 建立 App Service Plan
az appservice plan create \
  --name plan-flashcard-api \
  --resource-group rg-flashcard-api \
  --sku B1 \
  --is-linux

# 建立 Web App
az webapp create \
  --name flashcard-api-unique-name \
  --resource-group rg-flashcard-api \
  --plan plan-flashcard-api \
  --runtime "DOTNETCORE:8.0"

# 建立 SQL Database
az sql server create \
  --name flashcard-sql-server \
  --resource-group rg-flashcard-api \
  --admin-user flashcardadmin \
  --admin-password YourStrong@Password123

az sql db create \
  --name FlashcardDB \
  --server flashcard-sql-server \
  --resource-group rg-flashcard-api \
  --service-objective Basic
```

### 3. 設定環境變數

```bash
# 設定連線字串
az webapp config connection-string set \
  --name flashcard-api-unique-name \
  --resource-group rg-flashcard-api \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:flashcard-sql-server.database.windows.net,1433;Database=FlashcardDB;User ID=flashcardadmin;Password=YourStrong@Password123;Encrypt=True;"
```

### 4. 部署應用程式

```bash
# 發布到資料夾
dotnet publish -c Release -o ./publish

# 壓縮發布檔案
cd publish
zip -r ../app.zip .

# 部署到 Azure
az webapp deployment source config-zip \
  --name flashcard-api-unique-name \
  --resource-group rg-flashcard-api \
  --src ../app.zip
```

## 🐳 Docker 容器化部署步驟

### 1. 本地測試

```bash
# 建置 Docker 映像
docker build -t japanese-flashcard-api .

# 執行容器
docker run -p 8080:8080 japanese-flashcard-api

# 測試 API
curl http://localhost:8080/api/flashcards
```

### 2. 使用 Docker Compose

```bash
# 啟動所有服務
docker-compose up -d

# 查看日誌
docker-compose logs -f flashcard-api

# 停止服務
docker-compose down
```

### 3. 部署到雲端容器服務

#### Azure Container Instances
```bash
# 建立容器群組
az container create \
  --resource-group rg-flashcard-api \
  --name flashcard-api-container \
  --image your-registry/japanese-flashcard-api:latest \
  --cpu 1 \
  --memory 1 \
  --ports 8080 \
  --environment-variables ASPNETCORE_ENVIRONMENT=Production
```

#### Azure Container Registry
```bash
# 建立容器登錄
az acr create \
  --name flashcardregistry \
  --resource-group rg-flashcard-api \
  --sku Basic \
  --admin-enabled true

# 推送映像
docker tag japanese-flashcard-api flashcardregistry.azurecr.io/japanese-flashcard-api:latest
docker push flashcardregistry.azurecr.io/japanese-flashcard-api:latest
```

## 💰 成本估算 (每月)

### Railway
- **Hobby Plan**: $5/月
  - 包含 PostgreSQL 資料庫
  - 無執行時間限制
  - 500GB 網路流量
  - 8GB RAM + 8 vCPU

### Heroku
- **Eco Dyno**: $7/月
- **Heroku PostgreSQL**: $5/月
- **總計**: $12/月
  - 會有應用程式睡眠
  - 限制功能較多

### Azure App Service (參考)
- **Basic B1**: $13/月
- **SQL Database Basic**: $5/月  
- **總計**: $18/月
  - 企業級穩定性
  - 無睡眠限制

## 🔧 生產環境建議

### 1. 資料庫優化
- 使用 Entity Framework Core 替換記憶體儲存
- 實作資料庫遷移 (Migration)
- 設定連線池和快取

### 2. 安全性設定
- 實作 JWT 身份驗證
- 設定 CORS 政策
- 使用 HTTPS
- API 速率限制

### 3. 監控和日誌
- Azure Application Insights
- 健康檢查端點
- 錯誤追蹤和警報

### 4. 效能優化
- 實作快取機制
- 資料庫索引優化
- CDN 靜態資源

## 🎯 建議的部署流程

1. **本地開發** → 完成功能開發和測試
2. **Docker 化** → 建立 Dockerfile 和 docker-compose
3. **本地容器測試** → 確保容器環境正常運作
4. **雲端試部署** → 使用免費額度測試
5. **生產部署** → 正式環境上線
6. **監控優化** → 持續監控和改善

需要我協助您實作其中任何一個步驟嗎？
