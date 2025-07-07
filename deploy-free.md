# 免費部署方案指南

## 選項 1：Railway.app 免費額度

### 步驟：
1. 註冊 Railway.app 帳號
2. 連接 GitHub repository
3. 自動部署 .NET 8.0 應用
4. 使用 In-Memory 資料庫

### 優點：
- 每月 $5 USD 免費額度
- 自動縮放
- 簡單部署

### 限制：
- 超過額度需付費
- 資料不持久（使用 In-Memory）

---

## 選項 2：Azure 學生免費方案

### 如果你是學生：
1. 申請 Azure for Students
2. 獲得 $100 USD 免費額度
3. 使用 Azure App Service F1 免費層

### 配置：
```bash
# 部署到 Azure App Service
az webapp up --name your-flashcard-api --resource-group myResourceGroup
```

---

## 選項 3：Vercel + JSON 檔案儲存

### 步驟：
1. 將 API 改為 Next.js API Routes
2. 使用 JSON 檔案作為資料庫
3. 部署到 Vercel 免費層

### 程式碼範例：
```javascript
// pages/api/flashcards.js
export default function handler(req, res) {
  // 讀取 JSON 檔案
  // 處理 CRUD 操作
  // 回傳結果
}
```

---

## 選項 4：GitHub Pages + GitHub Actions

### 概念：
1. 將 API 轉為靜態 JSON 檔案
2. 使用 GitHub Actions 自動更新
3. 前端透過 fetch 讀取資料

### 適合場景：
- 資料不常變動
- 主要是查詢操作
- 完全免費展示

---

## 推薦方案

### 開發階段：
**Railway.app 免費額度**
- 快速部署
- 支援完整 .NET API
- 有一定免費使用量

### 長期使用：
**Azure 免費層 + 學生方案**
- 如果是學生可獲得更多額度
- 可升級到付費方案
- 企業級服務品質

### 完全免費展示：
**GitHub Pages + 靜態 JSON**
- 適合作品集展示
- 永久免費
- 但功能較受限
