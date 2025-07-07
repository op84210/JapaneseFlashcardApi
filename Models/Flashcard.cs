namespace JapaneseFlashcardApi.Models
{
    public class Flashcard
    {
        public int Id { get; set; } 
        public string Kanji { get; set; } = string.Empty; //漢字
        public string Hiragana { get; set; } = string.Empty; //平假名
        public string Katakana { get; set; } = string.Empty; //片假名
        public string Meaning { get; set; } = string.Empty; //意義
        public string? Example { get; set; }//例句
        public WordType WordType { get; set; } // 單字類型（日文原生/外來語等）
        public DifficultyLevel Difficulty { get; set; }// 難易度
        public Category Category { get; set; }// 分類
        public DateTime CreatedDate { get; set; }// 創建日期
        public DateTime LastReviewedDate { get; set; }// 最後審核日期
        public int ReviewCount { get; set; }// 審核次數
        public bool IsFavorite { get; set; }// 是否為收藏
    }

    public enum WordType
    {
        Native = 0,        // 日文原生詞（平假名為主）
        SinoJapanese = 1,  // 漢語詞（漢字+平假名）
        Foreign = 2,       // 外來語（片假名為主）
        Mixed = 3          // 混合型（平假名+片假名）
    }

    public enum DifficultyLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3,
        Expert = 4
    }

    public enum Category
    {
        General = 0,
        Animals = 1,
        Colors = 2,
        Food = 3,
        Nature = 4,
        Family = 5,
        Body = 6,
        Transportation = 7,
        Time = 8,
        Numbers = 9,
        Verbs = 10,
        Adjectives = 11
    }
}
