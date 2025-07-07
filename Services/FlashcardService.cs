using JapaneseFlashcardApi.Models;

namespace JapaneseFlashcardApi.Services
{
    public interface IFlashcardService
    {
        Task<IEnumerable<Flashcard>> GetAllFlashcardsAsync(FlashcardQueryDto query);
        Task<Flashcard?> GetFlashcardByIdAsync(int id);
        Task<Flashcard> CreateFlashcardAsync(CreateFlashcardDto createDto);
        Task<Flashcard?> UpdateFlashcardAsync(int id, UpdateFlashcardDto updateDto);
        Task<bool> DeleteFlashcardAsync(int id);
        Task<Flashcard?> MarkAsReviewedAsync(int id);
        Task<IEnumerable<Flashcard>> GetRandomFlashcardsAsync(int count, Category? category = null, DifficultyLevel? difficulty = null);
    }

    public class FlashcardService : IFlashcardService
    {
        private static readonly List<Flashcard> _flashcards = new()
        {
            new Flashcard
            {
                Id = 1,
                Kanji = "犬",
                Hiragana = "いぬ",
                Katakana = "",
                Meaning = "狗",
                Example = "私の犬はとても可愛いです。",
                WordType = WordType.SinoJapanese,
                Difficulty = DifficultyLevel.Beginner,
                Category = Category.Animals,
                CreatedDate = DateTime.Now.AddDays(-30),
                LastReviewedDate = DateTime.Now.AddDays(-5),
                ReviewCount = 3,
                IsFavorite = true
            },
            new Flashcard
            {
                Id = 2,
                Kanji = "",
                Hiragana = "",
                Katakana = "コーヒー",
                Meaning = "咖啡",
                Example = "朝のコーヒーは美味しいです。",
                WordType = WordType.Foreign,
                Difficulty = DifficultyLevel.Beginner,
                Category = Category.Food,
                CreatedDate = DateTime.Now.AddDays(-25),
                LastReviewedDate = DateTime.Now.AddDays(-3),
                ReviewCount = 5,
                IsFavorite = false
            },
            new Flashcard
            {
                Id = 3,
                Kanji = "",
                Hiragana = "おはよう",
                Katakana = "",
                Meaning = "早安",
                Example = "おはようございます。",
                WordType = WordType.Native,
                Difficulty = DifficultyLevel.Beginner,
                Category = Category.General,
                CreatedDate = DateTime.Now.AddDays(-20),
                LastReviewedDate = DateTime.Now.AddDays(-2),
                ReviewCount = 2,
                IsFavorite = true
            },
            new Flashcard
            {
                Id = 4,
                Kanji = "",
                Hiragana = "",
                Katakana = "コンピューター",
                Meaning = "電腦",
                Example = "新しいコンピューターを買いました。",
                WordType = WordType.Foreign,
                Difficulty = DifficultyLevel.Intermediate,
                Category = Category.General,
                CreatedDate = DateTime.Now.AddDays(-15),
                LastReviewedDate = DateTime.Now.AddDays(-1),
                ReviewCount = 1,
                IsFavorite = false
            }
        };

        private static int _nextId = 5;

        public async Task<IEnumerable<Flashcard>> GetAllFlashcardsAsync(FlashcardQueryDto query)
        {
            await Task.Delay(1); // 模擬異步操作
            
            var flashcards = _flashcards.AsQueryable();

            if (query.Category.HasValue)
                flashcards = flashcards.Where(f => f.Category == query.Category.Value);

            if (query.Difficulty.HasValue)
                flashcards = flashcards.Where(f => f.Difficulty == query.Difficulty.Value);

            if (query.WordType.HasValue)
                flashcards = flashcards.Where(f => f.WordType == query.WordType.Value);

            if (query.IsFavorite.HasValue)
                flashcards = flashcards.Where(f => f.IsFavorite == query.IsFavorite.Value);

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                flashcards = flashcards.Where(f => 
                    f.Kanji.ToLower().Contains(searchTerm) ||
                    f.Hiragana.ToLower().Contains(searchTerm) ||
                    f.Katakana.ToLower().Contains(searchTerm) ||
                    f.Meaning.ToLower().Contains(searchTerm));
            }

            return flashcards
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();
        }

        public async Task<Flashcard?> GetFlashcardByIdAsync(int id)
        {
            await Task.Delay(1);
            return _flashcards.FirstOrDefault(f => f.Id == id);
        }

        public async Task<Flashcard> CreateFlashcardAsync(CreateFlashcardDto createDto)
        {
            await Task.Delay(1);
            
            var flashcard = new Flashcard
            {
                Id = _nextId++,
                Kanji = createDto.Kanji,
                Hiragana = createDto.Hiragana,
                Katakana = createDto.Katakana,
                Meaning = createDto.Meaning,
                Example = createDto.Example,
                WordType = createDto.WordType,
                Difficulty = createDto.Difficulty,
                Category = createDto.Category,
                CreatedDate = DateTime.Now,
                LastReviewedDate = DateTime.MinValue,
                ReviewCount = 0,
                IsFavorite = false
            };

            _flashcards.Add(flashcard);
            return flashcard;
        }

        public async Task<Flashcard?> UpdateFlashcardAsync(int id, UpdateFlashcardDto updateDto)
        {
            await Task.Delay(1);
            
            var flashcard = _flashcards.FirstOrDefault(f => f.Id == id);
            if (flashcard == null) return null;

            if (!string.IsNullOrEmpty(updateDto.Kanji))
                flashcard.Kanji = updateDto.Kanji;
            
            if (!string.IsNullOrEmpty(updateDto.Hiragana))
                flashcard.Hiragana = updateDto.Hiragana;
            
            if (!string.IsNullOrEmpty(updateDto.Katakana))
                flashcard.Katakana = updateDto.Katakana;
            
            if (!string.IsNullOrEmpty(updateDto.Meaning))
                flashcard.Meaning = updateDto.Meaning;
            
            if (updateDto.Example != null)
                flashcard.Example = updateDto.Example;
            
            if (updateDto.WordType.HasValue)
                flashcard.WordType = updateDto.WordType.Value;
            
            if (updateDto.Difficulty.HasValue)
                flashcard.Difficulty = updateDto.Difficulty.Value;
            
            if (updateDto.Category.HasValue)
                flashcard.Category = updateDto.Category.Value;
            
            if (updateDto.IsFavorite.HasValue)
                flashcard.IsFavorite = updateDto.IsFavorite.Value;

            return flashcard;
        }

        public async Task<bool> DeleteFlashcardAsync(int id)
        {
            await Task.Delay(1);
            
            var flashcard = _flashcards.FirstOrDefault(f => f.Id == id);
            if (flashcard == null) return false;

            _flashcards.Remove(flashcard);
            return true;
        }

        public async Task<Flashcard?> MarkAsReviewedAsync(int id)
        {
            await Task.Delay(1);
            
            var flashcard = _flashcards.FirstOrDefault(f => f.Id == id);
            if (flashcard == null) return null;

            flashcard.LastReviewedDate = DateTime.Now;
            flashcard.ReviewCount++;
            return flashcard;
        }

        public async Task<IEnumerable<Flashcard>> GetRandomFlashcardsAsync(int count, Category? category = null, DifficultyLevel? difficulty = null)
        {
            await Task.Delay(1);
            
            var flashcards = _flashcards.AsQueryable();

            if (category.HasValue)
                flashcards = flashcards.Where(f => f.Category == category.Value);

            if (difficulty.HasValue)
                flashcards = flashcards.Where(f => f.Difficulty == difficulty.Value);

            var random = new Random();
            return flashcards.OrderBy(x => random.Next()).Take(count).ToList();
        }
    }
}
