using JapaneseFlashcardApi.Models;
using CsvHelper;
using System.Globalization;
using System.Text;

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
        
        // 批量操作
        Task<BatchOperationResult> CreateFlashcardsBatchAsync(BatchCreateFlashcardsDto batchDto);
        Task<BatchOperationResult> ImportFromCsvAsync(Stream csvStream);
        Task<byte[]> ExportToCsvAsync(ExportOptionsDto options);
        Task<string> ExportToJsonAsync(ExportOptionsDto options);
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

        public async Task<BatchOperationResult> CreateFlashcardsBatchAsync(BatchCreateFlashcardsDto batchDto)
        {
            await Task.Delay(1);
            
            var result = new BatchOperationResult
            {
                TotalProcessed = batchDto.Flashcards.Count
            };

            foreach (var createDto in batchDto.Flashcards)
            {
                try
                {
                    // 檢查重複（如果設定要跳過重複）
                    if (batchDto.SkipDuplicates)
                    {
                        var isDuplicate = _flashcards.Any(f => 
                            f.Kanji == createDto.Kanji && 
                            f.Hiragana == createDto.Hiragana && 
                            f.Katakana == createDto.Katakana);
                        
                        if (isDuplicate)
                        {
                            result.ErrorCount++;
                            result.ErrorMessages.Add($"重複的單字卡: {createDto.Kanji}{createDto.Hiragana}{createDto.Katakana}");
                            continue;
                        }
                    }

                    if (!batchDto.ValidateOnly)
                    {
                        var flashcard = await CreateFlashcardAsync(createDto);
                        result.CreatedFlashcards.Add(flashcard);
                    }
                    
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.ErrorMessages.Add($"創建失敗: {ex.Message}");
                }
            }

            return result;
        }

        public async Task<BatchOperationResult> ImportFromCsvAsync(Stream csvStream)
        {
            await Task.Delay(1);
            
            var result = new BatchOperationResult();
            var createDtos = new List<CreateFlashcardDto>();

            try
            {
                using var reader = new StreamReader(csvStream, Encoding.UTF8);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                
                var records = csv.GetRecords<FlashcardCsvRecord>().ToList();
                result.TotalProcessed = records.Count;

                foreach (var record in records)
                {
                    try
                    {
                        // 驗證必填欄位
                        if (string.IsNullOrWhiteSpace(record.Meaning))
                        {
                            result.ErrorCount++;
                            result.ErrorMessages.Add($"第 {result.TotalProcessed - records.Count + result.SuccessCount + result.ErrorCount} 行: 意思為必填欄位");
                            continue;
                        }

                        // 驗證枚舉值
                        if (!Enum.IsDefined(typeof(WordType), record.WordType))
                        {
                            result.ErrorCount++;
                            result.ErrorMessages.Add($"第 {result.TotalProcessed - records.Count + result.SuccessCount + result.ErrorCount} 行: 無效的單字類型 {record.WordType}");
                            continue;
                        }

                        if (!Enum.IsDefined(typeof(DifficultyLevel), record.Difficulty))
                        {
                            result.ErrorCount++;
                            result.ErrorMessages.Add($"第 {result.TotalProcessed - records.Count + result.SuccessCount + result.ErrorCount} 行: 無效的難度 {record.Difficulty}");
                            continue;
                        }

                        if (!Enum.IsDefined(typeof(Category), record.Category))
                        {
                            result.ErrorCount++;
                            result.ErrorMessages.Add($"第 {result.TotalProcessed - records.Count + result.SuccessCount + result.ErrorCount} 行: 無效的分類 {record.Category}");
                            continue;
                        }

                        var createDto = new CreateFlashcardDto
                        {
                            Kanji = record.Kanji ?? string.Empty,
                            Hiragana = record.Hiragana ?? string.Empty,
                            Katakana = record.Katakana ?? string.Empty,
                            Meaning = record.Meaning,
                            Example = record.Example,
                            WordType = (WordType)record.WordType,
                            Difficulty = (DifficultyLevel)record.Difficulty,
                            Category = (Category)record.Category
                        };

                        createDtos.Add(createDto);
                    }
                    catch (Exception ex)
                    {
                        result.ErrorCount++;
                        result.ErrorMessages.Add($"處理 CSV 記錄時發生錯誤: {ex.Message}");
                    }
                }

                // 批量創建有效的記錄
                if (createDtos.Any())
                {
                    var batchResult = await CreateFlashcardsBatchAsync(new BatchCreateFlashcardsDto 
                    { 
                        Flashcards = createDtos,
                        SkipDuplicates = true 
                    });
                    
                    result.SuccessCount = batchResult.SuccessCount;
                    result.ErrorCount += batchResult.ErrorCount;
                    result.ErrorMessages.AddRange(batchResult.ErrorMessages);
                    result.CreatedFlashcards = batchResult.CreatedFlashcards;
                }
            }
            catch (Exception ex)
            {
                result.ErrorCount = result.TotalProcessed;
                result.ErrorMessages.Add($"CSV 匯入失敗: {ex.Message}");
            }

            return result;
        }

        public async Task<byte[]> ExportToCsvAsync(ExportOptionsDto options)
        {
            await Task.Delay(1);
            
            // 根據選項篩選數據
            var query = new FlashcardQueryDto
            {
                Category = options.Category,
                Difficulty = options.Difficulty,
                WordType = options.WordType,
                IsFavorite = options.IsFavorite,
                PageSize = int.MaxValue
            };

            var flashcards = await GetAllFlashcardsAsync(query);
            
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // 寫入 CSV 標頭
            csv.WriteField("Kanji");
            csv.WriteField("Hiragana");
            csv.WriteField("Katakana");
            csv.WriteField("Meaning");
            csv.WriteField("Example");
            csv.WriteField("WordType");
            csv.WriteField("Difficulty");
            csv.WriteField("Category");
            csv.WriteField("CreatedDate");
            csv.WriteField("ReviewCount");
            csv.WriteField("IsFavorite");
            csv.NextRecord();

            // 寫入數據
            foreach (var flashcard in flashcards)
            {
                csv.WriteField(flashcard.Kanji);
                csv.WriteField(flashcard.Hiragana);
                csv.WriteField(flashcard.Katakana);
                csv.WriteField(flashcard.Meaning);
                csv.WriteField(flashcard.Example);
                csv.WriteField((int)flashcard.WordType);
                csv.WriteField((int)flashcard.Difficulty);
                csv.WriteField((int)flashcard.Category);
                csv.WriteField(flashcard.CreatedDate.ToString("yyyy-MM-dd"));
                csv.WriteField(flashcard.ReviewCount);
                csv.WriteField(flashcard.IsFavorite);
                csv.NextRecord();
            }

            writer.Flush();
            return memoryStream.ToArray();
        }

        public async Task<string> ExportToJsonAsync(ExportOptionsDto options)
        {
            await Task.Delay(1);
            
            // 根據選項篩選數據
            var query = new FlashcardQueryDto
            {
                Category = options.Category,
                Difficulty = options.Difficulty,
                WordType = options.WordType,
                IsFavorite = options.IsFavorite,
                PageSize = int.MaxValue
            };

            var flashcards = await GetAllFlashcardsAsync(query);
            
            return System.Text.Json.JsonSerializer.Serialize(flashcards, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }
    }
}
