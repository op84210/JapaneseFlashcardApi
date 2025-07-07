# 🚂 Railway 快速部署教學

## 🎯 為什麼選擇 Railway？

✅ **只需要 3 分鐘就能上線**  
✅ **$5/月 包含資料庫**  
✅ **自動 CI/CD 整合**  
✅ **零配置部署**  
✅ **現代化介面**  

相比之下：
- Heroku 需要 $12/月 (應用 $7 + 資料庫 $5)
- Azure 需要 $18/月 (應用 $13 + 資料庫 $5)

## 🚀 部署步驟

### 1. 準備 GitHub 存儲庫
確保您的日文單字卡專案已推送到 GitHub

### 2. 註冊 Railway 帳戶
1. 前往 [railway.app](https://railway.app)
2. 點擊 "Login" 
3. 選擇 "Login with GitHub"
4. 授權 Railway 存取您的 GitHub

### 3. 建立新專案
1. 點擊 "New Project"
2. 選擇 "Deploy from GitHub repo"
3. 選擇您的 `JapaneseFlashcardApi` 存儲庫
4. Railway 會自動偵測這是 .NET 專案

### 4. 專案會自動部署！
- Railway 使用 Nixpacks 自動偵測 .NET 8.0
- 自動執行 `dotnet restore` 和 `dotnet publish`
- 大約 2-3 分鐘完成首次部署

### 5. 獲取應用程式 URL
部署完成後，您會看到：
```
✅ Deployed successfully
🌐 https://your-app-name.up.railway.app
```

## 🔧 可選配置

### 添加 PostgreSQL 資料庫
1. 在專案頁面點擊 "+ New"
2. 選擇 "Database" → "PostgreSQL"
3. Railway 會自動建立 `DATABASE_URL` 環境變數

### 設定自定義網域
1. 前往 "Settings" → "Domains"
2. 點擊 "Custom Domain"
3. 輸入您的網域名稱

### 環境變數設定
Railway 會自動設定以下變數：
```bash
PORT=8080                           # Railway 自動分配
ASPNETCORE_ENVIRONMENT=Production   # 自動設定
RAILWAY_STATIC_URL=https://...      # 您的應用 URL
```

## 📱 測試您的部署

部署完成後，訪問您的 URL：

1. **Swagger UI**: `https://your-app.up.railway.app/`
2. **健康檢查**: `https://your-app.up.railway.app/health`
3. **API 端點**: `https://your-app.up.railway.app/api/flashcards`

## 🔄 自動 CI/CD

每次您推送代碼到 GitHub：
1. Railway 自動偵測變更
2. 觸發新的部署
3. 2-3 分鐘內更新線上版本
4. 無需手動操作！

## 💡 生產環境優化

### 1. 配置自定義啟動指令
編輯 `railway.toml`：
```toml
[deploy]
startCommand = "dotnet JapaneseFlashcardApi.dll --urls http://0.0.0.0:$PORT"
```

### 2. 設定健康檢查
```toml
[deploy]
healthcheckPath = "/health"
healthcheckTimeout = 300
```

### 3. 配置重啟政策
```toml
[deploy]
restartPolicyType = "ON_FAILURE"
restartPolicyMaxRetries = 10
```

## 🆚 與其他平台比較

| 功能 | Railway | Heroku | Azure |
|------|---------|--------|-------|
| 部署速度 | ⚡ 2-3分鐘 | 🐌 5-8分鐘 | 🐌 5-10分鐘 |
| 月費用 | 💰 $5 | 💰 $12 | 💰 $18 |
| 包含資料庫 | ✅ | ❌ | ❌ |
| 自動睡眠 | ❌ | ✅ Eco dyno | ❌ |
| 介面友善度 | 🎨 極佳 | 🎨 普通 | 🎨 複雜 |

## 🎉 完成！

恭喜！您的日文單字卡 API 現在已經在雲端運行了！

您可以：
- 📱 分享 URL 給朋友測試
- 🔗 將 API 整合到前端應用
- 📊 在 Railway 儀表板監控使用情況
- 🚀 持續開發新功能（自動部署）

## 📞 需要幫助？

如果遇到問題：
1. 檢查 Railway 的 "Deployments" 頁面查看部署日誌
2. 確認 `railway.toml` 配置正確
3. 檢查 GitHub 存儲庫是否包含所有必要檔案

**下一步：考慮添加前端應用程式，搭配這個 API 建立完整的日文學習平台！** 🇯🇵
