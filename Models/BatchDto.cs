using JapaneseFlashcardApi.Models;

namespace JapaneseFlashcardApi.Models
{
    /// <summary>
    /// 批量操作結果
    /// </summary>
    public class BatchOperationResult
    {
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public List<Flashcard> CreatedFlashcards { get; set; } = new();
    }

    /// <summary>
    /// CSV 匯入的單字卡資料
    /// </summary>
    public class FlashcardCsvRecord
    {
        public string Kanji { get; set; } = string.Empty;
        public string Hiragana { get; set; } = string.Empty;
        public string Katakana { get; set; } = string.Empty;
        public string Meaning { get; set; } = string.Empty;
        public string? Example { get; set; }
        public int WordType { get; set; }
        public int Difficulty { get; set; }
        public int Category { get; set; }
    }

    /// <summary>
    /// 批量創建請求
    /// </summary>
    public class BatchCreateFlashcardsDto
    {
        public List<CreateFlashcardDto> Flashcards { get; set; } = new();
        public bool SkipDuplicates { get; set; } = true;
        public bool ValidateOnly { get; set; } = false;
    }

    /// <summary>
    /// 匯出選項
    /// </summary>
    public class ExportOptionsDto
    {
        public Category? Category { get; set; }
        public DifficultyLevel? Difficulty { get; set; }
        public WordType? WordType { get; set; }
        public bool? IsFavorite { get; set; }
        public ExportFormat Format { get; set; } = ExportFormat.Csv;
    }

    public enum ExportFormat
    {
        Csv = 0,
        Json = 1,
        Excel = 2
    }
}
