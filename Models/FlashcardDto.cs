namespace JapaneseFlashcardApi.Models
{
    /// <summary>
    /// 創建單字卡的資料傳輸物件
    /// 用於新增單字卡時的 API 請求
    /// </summary>
    public class CreateFlashcardDto
    {
        /// <summary>
        /// 漢字表記
        /// </summary>
        /// <example>桜</example>
        public string Kanji { get; set; } = string.Empty;

        /// <summary>
        /// 平假名表記
        /// </summary>
        /// <example>さくら</example>
        public string Hiragana { get; set; } = string.Empty;

        /// <summary>
        /// 片假名表記
        /// </summary>
        /// <example>サクラ</example>
        public string Katakana { get; set; } = string.Empty;

        /// <summary>
        /// 中文意義
        /// </summary>
        /// <example>櫻花</example>
        public string Meaning { get; set; } = string.Empty;

        /// <summary>
        /// 例句（可選）
        /// </summary>
        /// <example>春に桜が咲きます。</example>
        public string? Example { get; set; }

        /// <summary>
        /// 單字類型
        /// </summary>
        public WordType WordType { get; set; }

        /// <summary>
        /// 難易度等級
        /// </summary>
        public DifficultyLevel Difficulty { get; set; }

        /// <summary>
        /// 單字分類
        /// </summary>
        public Category Category { get; set; }
    }

    /// <summary>
    /// 更新單字卡的資料傳輸物件
    /// 用於修改現有單字卡時的 API 請求，所有欄位皆為可選
    /// </summary>
    public class UpdateFlashcardDto
    {
        /// <summary>
        /// 漢字表記（可選更新）
        /// </summary>
        public string? Kanji { get; set; }

        /// <summary>
        /// 平假名表記（可選更新）
        /// </summary>
        public string? Hiragana { get; set; }

        /// <summary>
        /// 片假名表記（可選更新）
        /// </summary>
        public string? Katakana { get; set; }

        /// <summary>
        /// 中文意義（可選更新）
        /// </summary>
        public string? Meaning { get; set; }

        /// <summary>
        /// 例句（可選更新）
        /// </summary>
        public string? Example { get; set; }

        /// <summary>
        /// 單字類型（可選更新）
        /// </summary>
        public WordType? WordType { get; set; }

        /// <summary>
        /// 難易度等級（可選更新）
        /// </summary>
        public DifficultyLevel? Difficulty { get; set; }

        /// <summary>
        /// 單字分類（可選更新）
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// 是否收藏（可選更新）
        /// </summary>
        public bool? IsFavorite { get; set; }
    }

    /// <summary>
    /// 單字卡查詢條件的資料傳輸物件
    /// 用於搜尋和篩選單字卡的 API 請求
    /// </summary>
    public class FlashcardQueryDto
    {
        /// <summary>
        /// 按分類篩選（可選）
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// 按難易度篩選（可選）
        /// </summary>
        public DifficultyLevel? Difficulty { get; set; }

        /// <summary>
        /// 按單字類型篩選（可選）
        /// </summary>
        public WordType? WordType { get; set; }

        /// <summary>
        /// 只顯示收藏的單字卡（可選）
        /// </summary>
        public bool? IsFavorite { get; set; }

        /// <summary>
        /// 關鍵字搜尋
        /// 會在漢字、平假名、片假名、意義中搜尋
        /// </summary>
        /// <example>桜</example>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// 頁碼
        /// 用於分頁顯示，從 1 開始
        /// </summary>
        /// <example>1</example>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 每頁顯示數量
        /// 每頁最多顯示的單字卡數量
        /// </summary>
        /// <example>10</example>
        public int PageSize { get; set; } = 10;
    }
}
