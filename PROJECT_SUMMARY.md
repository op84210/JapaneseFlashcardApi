# 🎌 日文單字卡 Web API 

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-black?style=flat&logo=github)](https://github.com/op84210/JapaneseFlashcardApi)

一個功能完整的日文學習單字卡管理 API，支援 CRUD 操作、批量處理、CSV 匯入匯出等功能。

## 🚀 專案網址
**GitHub 倉庫**: https://github.com/op84210/JapaneseFlashcardApi

## ✨ 主要功能

### 📚 單字卡管理
- ✅ 完整的 CRUD 操作（創建、讀取、更新、刪除）
- ✅ 支援漢字、平假名、片假名三種表記
- ✅ 單字類型分類（日文原生詞、漢語詞、外來語、混合型）
- ✅ 難易度分級（初學者、中級、高級、專家）
- ✅ 主題分類（動物、顏色、食物、自然等 12 種分類）

### 🔍 查詢與篩選
- ✅ 關鍵字搜尋（支援漢字、平假名、片假名、中文意義）
- ✅ 多條件篩選（分類、難易度、單字類型、收藏狀態）
- ✅ 分頁顯示
- ✅ 隨機抽卡功能

### 📥 批量操作
- ✅ 批量創建單字卡
- ✅ CSV 檔案匯入
- ✅ CSV/JSON 格式匯出
- ✅ 重複檢查
- ✅ 錯誤處理和報告

### 📖 學習功能
- ✅ 複習追蹤（複習次數、最後複習時間）
- ✅ 收藏功能
- ✅ 隨機練習

## 🛠️ 技術規格

- **框架**: .NET 8.0 Web API
- **文件**: Swagger UI 支援，完整的 XML 註解
- **資料儲存**: 記憶體儲存（可擴展至資料庫）
- **CSV 處理**: CsvHelper 套件
- **跨域支援**: CORS 配置

## 📖 API 文件

啟動應用程式後，訪問根目錄即可查看完整的 Swagger API 文件：
```
http://localhost:5000/
```

### 主要端點

| 端點 | 方法 | 說明 |
|------|------|------|
| `/api/flashcards` | GET | 取得所有單字卡（支援篩選和分頁） |
| `/api/flashcards/{id}` | GET | 取得特定單字卡 |
| `/api/flashcards` | POST | 創建新單字卡 |
| `/api/flashcards/{id}` | PUT | 更新單字卡 |
| `/api/flashcards/{id}` | DELETE | 刪除單字卡 |
| `/api/flashcards/{id}/review` | POST | 標記為已複習 |
| `/api/flashcards/random` | GET | 取得隨機單字卡 |
| `/api/flashcards/batch` | POST | 批量創建單字卡 |
| `/api/flashcards/import/csv` | POST | CSV 匯入 |
| `/api/flashcards/export/csv` | GET | CSV 匯出 |
| `/api/flashcards/export/json` | GET | JSON 匯出 |
| `/api/flashcards/template/csv` | GET | 下載 CSV 範本 |

## 🚀 快速開始

### 必要需求
- .NET 8.0 SDK

### 本地運行
```bash
git clone https://github.com/op84210/JapaneseFlashcardApi.git
cd JapaneseFlashcardApi
dotnet run
```

應用程式將在 `http://localhost:5000` 啟動。

## 📂 專案結構

```
JapaneseFlashcardApi/
├── Controllers/          # API 控制器
├── Models/              # 資料模型和 DTO
├── Services/            # 業務邏輯服務
├── sample_flashcards.csv # 範例 CSV 檔案
├── api-tests.http       # API 測試檔案
├── deploy-free.md       # 免費部署指南
└── README.md           # 專案說明
```

## 🎯 使用範例

### 創建單字卡
```json
POST /api/flashcards
{
  "kanji": "桜",
  "hiragana": "さくら",
  "katakana": "サクラ",
  "meaning": "櫻花",
  "example": "春に桜が咲きます。",
  "wordType": 1,
  "difficulty": 1,
  "category": 4
}
```

### 搜尋單字卡
```
GET /api/flashcards?searchTerm=桜&category=4&pageSize=10
```

### CSV 匯入格式
```csv
Kanji,Hiragana,Katakana,Meaning,Example,WordType,Difficulty,Category
犬,いぬ,,狗,私の犬はとても可愛いです。,1,1,1
,おはよう,,早安,おはようございます。,0,1,0
```

## 🌐 部署選項

詳細的免費部署指南請參考：[deploy-free.md](deploy-free.md)

推薦的部署平台：
- **Railway.app** - 免費額度，適合開發測試
- **Azure App Service** - 免費層，適合學習專案
- **DigitalOcean** - 低成本，適合正式使用

## 📝 開發紀錄

- **v1.0** - 基本 CRUD 功能
- **v1.1** - 新增批量操作和 CSV 匯入匯出
- **v1.2** - 完善 XML 註解和 API 文件

## 🤝 貢獻

歡迎提交 Issue 和 Pull Request！

## 📄 授權

本專案採用 MIT 授權條款。

---

⭐ 如果這個專案對你有幫助，請給它一個星星！
