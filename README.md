# 日文單字卡 API

一個基於 .NET 8.0 的 Web API，用於管理日文單字卡學習系統。已成功部署到 Railway 雲端平台，使用 PostgreSQL 資料庫儲存。

## 🚀 線上版本

- **API 地址**: https://japaneseflashcardapi-production.up.railway.app
- **Swagger 文檔**: https://japaneseflashcardapi-production.up.railway.app/swagger
- **資料庫**: Railway PostgreSQL（資料持久化）

## 功能特色

- ✅ 創建、讀取、更新、刪除單字卡
- ✅ 平假名和片假名分別支援
- ✅ 單字類型分類（日文原生詞/漢語詞/外來語/混合型）
- ✅ 批量操作（批量創建、CSV匯入匯出、JSON匯出）
- ✅ 按分類、難度和單字類型篩選
- ✅ 搜索功能（漢字、平假名、片假名、意思）
- ✅ 隨機獲取單字卡進行練習
- ✅ 標記最愛和複習追蹤
- ✅ 分頁功能
- ✅ Swagger UI 文檔
- ✅ PostgreSQL 資料庫儲存
- ✅ Railway 雲端部署

## 🧪 API 測試

使用 `test-railway-postgresql.http` 檔案測試線上 API 功能。

## 快速開始

### 本地開發

```bash
cd JapaneseFlashcardApi
dotnet run
```

API 將在以下地址運行：
- HTTPS: https://localhost:7777
- HTTP: http://localhost:5555
- Swagger UI: https://localhost:7777 (開發環境中自動跳轉)

### 部署

參閱 `RAILWAY_DEPLOY.md` 了解如何部署到 Railway 平台。

### 項目結構

```
JapaneseFlashcardApi/
├── Controllers/
│   └── FlashcardsController.cs     # API 控制器
├── Models/
│   ├── Flashcard.cs               # 單字卡模型
│   └── FlashcardDto.cs            # 數據傳輸對象
├── Services/
│   └── FlashcardService.cs        # 業務邏輯服務
├── Program.cs                     # 應用程式入口點
└── api-tests.http                 # API 測試文件
```

## API 端點

### 單字卡管理

| 方法 | 端點 | 描述 |
|------|------|------|
| GET | `/api/flashcards` | 獲取所有單字卡（支援分頁和篩選） |
| GET | `/api/flashcards/{id}` | 根據 ID 獲取單字卡 |
| POST | `/api/flashcards` | 創建新單字卡 |
| PUT | `/api/flashcards/{id}` | 更新單字卡 |
| DELETE | `/api/flashcards/{id}` | 刪除單字卡 |

### 批量操作

| 方法 | 端點 | 描述 |
|------|------|------|
| POST | `/api/flashcards/batch` | 批量創建單字卡 |
| POST | `/api/flashcards/import/csv` | 從 CSV 檔案匯入單字卡 |
| GET | `/api/flashcards/export/csv` | 匯出單字卡為 CSV 檔案 |
| GET | `/api/flashcards/export/json` | 匯出單字卡為 JSON 檔案 |
| GET | `/api/flashcards/template/csv` | 下載 CSV 範本檔案 |

### 學習功能

| 方法 | 端點 | 描述 |
|------|------|------|
| POST | `/api/flashcards/{id}/review` | 標記單字卡為已複習 |
| GET | `/api/flashcards/random` | 獲取隨機單字卡進行練習 |

### 元數據

| 方法 | 端點 | 描述 |
|------|------|------|
| GET | `/api/flashcards/categories` | 獲取所有分類 |
| GET | `/api/flashcards/difficulties` | 獲取所有難度級別 |
| GET | `/api/flashcards/wordtypes` | 獲取所有單字類型 |

## 查詢參數

### 獲取單字卡 (`GET /api/flashcards`)

- `category`: 分類篩選 (0-11)
- `difficulty`: 難度篩選 (1-4)
- `wordType`: 單字類型篩選 (0-3)
- `isFavorite`: 是否最愛 (true/false)
- `searchTerm`: 搜索關鍵字（支援漢字、平假名、片假名、中文意思）
- `pageNumber`: 頁碼 (預設: 1)
- `pageSize`: 每頁數量 (預設: 10)

### 隨機單字卡 (`GET /api/flashcards/random`)

- `count`: 數量 (1-50, 預設: 5)
- `category`: 分類篩選
- `difficulty`: 難度篩選

## 數據模型

### 單字卡 (Flashcard)

```json
{
  "id": 1,
  "kanji": "犬",
  "hiragana": "いぬ",
  "katakana": "",
  "meaning": "狗",
  "example": "私の犬はとても可愛いです。",
  "wordType": 1,
  "difficulty": 1,
  "category": 1,
  "createdDate": "2024-01-01T00:00:00",
  "lastReviewedDate": "2024-01-15T00:00:00",
  "reviewCount": 3,
  "isFavorite": true
}
```

### 單字類型 (WordType)

- 0: Native (日文原生詞) - 主要使用平假名
- 1: SinoJapanese (漢語詞) - 漢字+平假名讀音
- 2: Foreign (外來語) - 主要使用片假名
- 3: Mixed (混合型) - 平假名+片假名

### 分類 (Category)

- 0: General (一般)
- 1: Animals (動物)
- 2: Colors (顏色)
- 3: Food (食物)
- 4: Nature (自然)
- 5: Family (家庭)
- 6: Body (身體)
- 7: Transportation (交通)
- 8: Time (時間)
- 9: Numbers (數字)
- 10: Verbs (動詞)
- 11: Adjectives (形容詞)

### 難度級別 (Difficulty)

- 1: Beginner (初學者)
- 2: Intermediate (中級)
- 3: Advanced (高級)
- 4: Expert (專家)

## 示例請求

### 批量創建單字卡

```bash
curl -X POST "https://localhost:7777/api/flashcards/batch" \
  -H "Content-Type: application/json" \
  -d '{
    "flashcards": [
      {
        "kanji": "本",
        "hiragana": "ほん",
        "katakana": "",
        "meaning": "書",
        "wordType": 1,
        "difficulty": 1,
        "category": 0
      }
    ],
    "skipDuplicates": true
  }'
```

### CSV 匯入匯出

```bash
# 下載 CSV 範本
curl "https://localhost:7777/api/flashcards/template/csv" -o template.csv

# 匯入 CSV 檔案
curl -X POST "https://localhost:7777/api/flashcards/import/csv" \
  -F "file=@sample_flashcards.csv"

# 匯出為 CSV
curl "https://localhost:7777/api/flashcards/export/csv" -o export.csv

# 匯出特定分類為 JSON
curl "https://localhost:7777/api/flashcards/export/json?category=1" -o animals.json
```

### 搜索單字卡（片假名）

```bash
curl "https://localhost:7777/api/flashcards?searchTerm=コーヒー&pageSize=5"
```

### 按單字類型篩選

```bash
# 只顯示外來語
curl "https://localhost:7777/api/flashcards?wordType=2"

# 只顯示日文原生詞
curl "https://localhost:7777/api/flashcards?wordType=0"
```

### 獲取隨機練習卡

```bash
curl "https://localhost:7777/api/flashcards/random?count=3&category=1&difficulty=1"
```

## 開發說明

- 目前使用記憶體存儲（重啟後數據會重置）
- 包含四個示例單字卡數據：
  - 犬 (いぬ) - 漢語詞
  - コーヒー - 外來語
  - おはよう - 日文原生詞
  - コンピューター - 外來語
- 支援 CORS，可用於前端開發
- 使用 .NET 8.0 框架
- 完整支援平假名和片假名分別處理

## 下一步擴展

- [ ] 添加資料庫持久化 (Entity Framework Core)
- [ ] 用戶認證和授權
- [ ] 學習進度統計
- [ ] 匯入/匯出功能
- [ ] 語音功能整合
- [ ] 間隔重複算法 (SRS)

## 技術棧

- .NET 8.0
- ASP.NET Core Web API
- Swagger/OpenAPI
- 未來可擴展: Entity Framework Core、SQL Server
