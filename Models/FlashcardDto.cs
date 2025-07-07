namespace JapaneseFlashcardApi.Models
{
    public class CreateFlashcardDto
    {
        public string Kanji { get; set; } = string.Empty;
        public string Hiragana { get; set; } = string.Empty;
        public string Katakana { get; set; } = string.Empty;
        public string Meaning { get; set; } = string.Empty;
        public string? Example { get; set; }
        public WordType WordType { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public Category Category { get; set; }
    }

    public class UpdateFlashcardDto
    {
        public string? Kanji { get; set; }
        public string? Hiragana { get; set; }
        public string? Katakana { get; set; }
        public string? Meaning { get; set; }
        public string? Example { get; set; }
        public WordType? WordType { get; set; }
        public DifficultyLevel? Difficulty { get; set; }
        public Category? Category { get; set; }
        public bool? IsFavorite { get; set; }
    }

    public class FlashcardQueryDto
    {
        public Category? Category { get; set; }
        public DifficultyLevel? Difficulty { get; set; }
        public WordType? WordType { get; set; }
        public bool? IsFavorite { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
