namespace JapaneseFlashcardApi.Models
{
    /// <summary>
    /// 日文單字卡實體模型
    /// 包含漢字、平假名、片假名、意義等完整日文學習資訊
    /// </summary>
    public class Flashcard
    {
        /// <summary>
        /// 單字卡唯一識別碼
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 漢字表記
        /// 日文中的漢字寫法，可為空（純平假名或片假名詞彙）
        /// </summary>
        /// <example>桜</example>
        public string Kanji { get; set; } = string.Empty;

        /// <summary>
        /// 平假名表記
        /// 日文的平假名讀音，用於表示發音
        /// </summary>
        /// <example>さくら</example>
        public string Hiragana { get; set; } = string.Empty;

        /// <summary>
        /// 片假名表記
        /// 日文的片假名寫法，主要用於外來語
        /// </summary>
        /// <example>サクラ</example>
        public string Katakana { get; set; } = string.Empty;

        /// <summary>
        /// 中文意義
        /// 該日文單字的中文解釋
        /// </summary>
        /// <example>櫻花</example>
        public string Meaning { get; set; } = string.Empty;

        /// <summary>
        /// 例句
        /// 包含該單字的日文例句，可選欄位
        /// </summary>
        /// <example>春に桜が咲きます。</example>
        public string? Example { get; set; }

        /// <summary>
        /// 單字類型
        /// 標示該詞彙是日文原生詞、漢語詞、外來語或混合型
        /// </summary>
        public WordType WordType { get; set; }

        /// <summary>
        /// 難易度等級
        /// 從初學者到專家的四個等級
        /// </summary>
        public DifficultyLevel Difficulty { get; set; }

        /// <summary>
        /// 單字分類
        /// 按主題分類，如動物、顏色、食物等
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// 創建日期
        /// 該單字卡加入系統的時間
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 最後複習日期
        /// 用戶最後一次複習此單字卡的時間
        /// </summary>
        public DateTime LastReviewedDate { get; set; }

        /// <summary>
        /// 複習次數
        /// 該單字卡被複習的總次數
        /// </summary>
        public int ReviewCount { get; set; }

        /// <summary>
        /// 是否收藏
        /// 標示用戶是否將此單字卡加入收藏清單
        /// </summary>
        public bool IsFavorite { get; set; }
    }

    /// <summary>
    /// 日文單字類型列舉
    /// 定義單字的來源和特性分類
    /// </summary>
    public enum WordType
    {
        /// <summary>
        /// 日文原生詞
        /// 純日語詞彙，通常以平假名為主
        /// </summary>
        Native = 0,

        /// <summary>
        /// 漢語詞（音讀詞）
        /// 來自中文的詞彙，通常包含漢字和平假名
        /// </summary>
        SinoJapanese = 1,

        /// <summary>
        /// 外來語
        /// 來自其他語言（主要是英語）的詞彙，以片假名表記
        /// </summary>
        Foreign = 2,

        /// <summary>
        /// 混合型
        /// 結合平假名和片假名的複合詞
        /// </summary>
        Mixed = 3
    }

    /// <summary>
    /// 難易度等級列舉
    /// 根據日語學習程度分級
    /// </summary>
    public enum DifficultyLevel
    {
        /// <summary>
        /// 初學者等級
        /// 適合日語初學者的基礎詞彙
        /// </summary>
        Beginner = 1,

        /// <summary>
        /// 中級等級
        /// 適合有一定日語基礎的學習者
        /// </summary>
        Intermediate = 2,

        /// <summary>
        /// 高級等級
        /// 適合日語程度較好的學習者
        /// </summary>
        Advanced = 3,

        /// <summary>
        /// 專家等級
        /// 適合日語專業人士或接近母語水平者
        /// </summary>
        Expert = 4
    }

    /// <summary>
    /// 單字分類列舉
    /// 按主題對單字進行分類，便於學習管理
    /// </summary>
    public enum Category
    {
        /// <summary>
        /// 一般詞彙
        /// 無特定分類的通用詞彙
        /// </summary>
        General = 0,

        /// <summary>
        /// 動物類
        /// 與動物相關的詞彙
        /// </summary>
        Animals = 1,

        /// <summary>
        /// 顏色類
        /// 表示顏色的詞彙
        /// </summary>
        Colors = 2,

        /// <summary>
        /// 食物類
        /// 與食物、飲料相關的詞彙
        /// </summary>
        Food = 3,

        /// <summary>
        /// 自然類
        /// 與自然、天氣、環境相關的詞彙
        /// </summary>
        Nature = 4,

        /// <summary>
        /// 家庭類
        /// 與家庭成員、親屬關係相關的詞彙
        /// </summary>
        Family = 5,

        /// <summary>
        /// 身體部位類
        /// 與人體部位相關的詞彙
        /// </summary>
        Body = 6,

        /// <summary>
        /// 交通工具類
        /// 與交通、運輸相關的詞彙
        /// </summary>
        Transportation = 7,

        /// <summary>
        /// 時間類
        /// 與時間、日期相關的詞彙
        /// </summary>
        Time = 8,

        /// <summary>
        /// 數字類
        /// 數字、數量相關的詞彙
        /// </summary>
        Numbers = 9,

        /// <summary>
        /// 動詞類
        /// 表示動作的詞彙
        /// </summary>
        Verbs = 10,

        /// <summary>
        /// 形容詞類
        /// 表示性質、狀態的詞彙
        /// </summary>
        Adjectives = 11
    }
}
