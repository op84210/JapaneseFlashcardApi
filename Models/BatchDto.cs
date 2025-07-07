using JapaneseFlashcardApi.Models;

namespace JapaneseFlashcardApi.Models
{
    /// <summary>
    /// 批量操作結果的資料傳輸物件
    /// 包含操作統計資訊和詳細結果
    /// </summary>
    public class BatchOperationResult
    {
        /// <summary>
        /// 總處理數量
        /// 批量操作中總共處理的項目數量
        /// </summary>
        public int TotalProcessed { get; set; }

        /// <summary>
        /// 成功處理數量
        /// 成功完成操作的項目數量
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 錯誤數量
        /// 處理失敗的項目數量
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// 錯誤訊息清單
        /// 詳細的錯誤訊息，用於除錯和使用者提示
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new();

        /// <summary>
        /// 成功創建的單字卡清單
        /// 批量創建操作中成功新增的單字卡
        /// </summary>
        public List<Flashcard> CreatedFlashcards { get; set; } = new();
    }

    /// <summary>
    /// CSV 匯入的單字卡資料結構
    /// 對應 CSV 檔案中的欄位結構
    /// </summary>
    public class FlashcardCsvRecord
    {
        /// <summary>
        /// 漢字表記
        /// CSV 檔案中的漢字欄位
        /// </summary>
        public string Kanji { get; set; } = string.Empty;

        /// <summary>
        /// 平假名表記
        /// CSV 檔案中的平假名欄位
        /// </summary>
        public string Hiragana { get; set; } = string.Empty;

        /// <summary>
        /// 片假名表記
        /// CSV 檔案中的片假名欄位
        /// </summary>
        public string Katakana { get; set; } = string.Empty;

        /// <summary>
        /// 中文意義
        /// CSV 檔案中的意義欄位
        /// </summary>
        public string Meaning { get; set; } = string.Empty;

        /// <summary>
        /// 例句
        /// CSV 檔案中的例句欄位，可為空
        /// </summary>
        public string? Example { get; set; }

        /// <summary>
        /// 單字類型（數值）
        /// CSV 檔案中的單字類型，以數字表示
        /// </summary>
        public int WordType { get; set; }

        /// <summary>
        /// 難易度等級（數值）
        /// CSV 檔案中的難易度，以數字表示
        /// </summary>
        public int Difficulty { get; set; }

        /// <summary>
        /// 單字分類（數值）
        /// CSV 檔案中的分類，以數字表示
        /// </summary>
        public int Category { get; set; }
    }

    /// <summary>
    /// 批量創建單字卡的請求資料傳輸物件
    /// 用於一次性創建多個單字卡的 API 請求
    /// </summary>
    public class BatchCreateFlashcardsDto
    {
        /// <summary>
        /// 要批量創建的單字卡清單
        /// 包含所有要新增的單字卡資料
        /// </summary>
        public List<CreateFlashcardDto> Flashcards { get; set; } = new();

        /// <summary>
        /// 是否跳過重複項目
        /// 設為 true 時，遇到重複的單字卡會跳過而不是報錯
        /// </summary>
        public bool SkipDuplicates { get; set; } = true;

        /// <summary>
        /// 是否僅進行驗證
        /// 設為 true 時，只檢查資料格式而不實際新增到資料庫
        /// </summary>
        public bool ValidateOnly { get; set; } = false;
    }

    /// <summary>
    /// 匯出選項的資料傳輸物件
    /// 定義匯出單字卡時的篩選條件和格式
    /// </summary>
    public class ExportOptionsDto
    {
        /// <summary>
        /// 按分類篩選匯出
        /// 只匯出指定分類的單字卡，null 表示不篩選
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// 按難易度篩選匯出
        /// 只匯出指定難易度的單字卡，null 表示不篩選
        /// </summary>
        public DifficultyLevel? Difficulty { get; set; }

        /// <summary>
        /// 按單字類型篩選匯出
        /// 只匯出指定類型的單字卡，null 表示不篩選
        /// </summary>
        public WordType? WordType { get; set; }

        /// <summary>
        /// 是否只匯出收藏的單字卡
        /// true 只匯出收藏項目，false 只匯出非收藏項目，null 表示不篩選
        /// </summary>
        public bool? IsFavorite { get; set; }

        /// <summary>
        /// 匯出檔案格式
        /// 支援 CSV、JSON、Excel 等格式
        /// </summary>
        public ExportFormat Format { get; set; } = ExportFormat.Csv;
    }

    /// <summary>
    /// 匯出檔案格式列舉
    /// 定義支援的匯出檔案格式
    /// </summary>
    public enum ExportFormat
    {
        /// <summary>
        /// CSV 格式
        /// 逗號分隔值檔案，適合在 Excel 中開啟
        /// </summary>
        Csv = 0,

        /// <summary>
        /// JSON 格式
        /// JavaScript 物件標記法，適合程式處理
        /// </summary>
        Json = 1,

        /// <summary>
        /// Excel 格式
        /// Microsoft Excel 檔案格式（.xlsx）
        /// </summary>
        Excel = 2
    }
}
