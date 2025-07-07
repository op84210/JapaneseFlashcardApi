using JapaneseFlashcardApi.Models;
using JapaneseFlashcardApi.Data;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace JapaneseFlashcardApi.Services
{
    /// <summary>
    /// 基於 Entity Framework Core 的單字卡服務實作
    /// 使用 PostgreSQL 資料庫進行資料持久化
    /// </summary>
    public class DatabaseFlashcardService : IFlashcardService
    {
        private readonly FlashcardDbContext _context;

        public DatabaseFlashcardService(FlashcardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 根據查詢條件取得所有單字卡
        /// </summary>
        public async Task<IEnumerable<Flashcard>> GetAllFlashcardsAsync(FlashcardQueryDto query)
        {
            var flashcards = _context.Flashcards.AsQueryable();

            // 分類篩選
            if (query.Category.HasValue)
                flashcards = flashcards.Where(f => f.Category == query.Category.Value);

            // 難度篩選
            if (query.Difficulty.HasValue)
                flashcards = flashcards.Where(f => f.Difficulty == query.Difficulty.Value);

            // 單字類型篩選
            if (query.WordType.HasValue)
                flashcards = flashcards.Where(f => f.WordType == query.WordType.Value);

            // 我的最愛篩選
            if (query.IsFavorite.HasValue)
                flashcards = flashcards.Where(f => f.IsFavorite == query.IsFavorite.Value);

            // 搜尋條件
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                flashcards = flashcards.Where(f =>
                    f.Kanji.ToLower().Contains(searchTerm) ||
                    f.Hiragana.ToLower().Contains(searchTerm) ||
                    f.Katakana.ToLower().Contains(searchTerm) ||
                    f.Meaning.ToLower().Contains(searchTerm));
            }

            // 分頁
            return await flashcards
                .OrderBy(f => f.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        /// <summary>
        /// 根據 ID 取得特定單字卡
        /// </summary>
        public async Task<Flashcard?> GetFlashcardByIdAsync(int id)
        {
            return await _context.Flashcards.FindAsync(id);
        }

        /// <summary>
        /// 創建新的單字卡
        /// </summary>
        public async Task<Flashcard> CreateFlashcardAsync(CreateFlashcardDto createDto)
        {
            var flashcard = new Flashcard
            {
                Kanji = createDto.Kanji,
                Hiragana = createDto.Hiragana,
                Katakana = createDto.Katakana,
                Meaning = createDto.Meaning,
                Example = createDto.Example,
                WordType = createDto.WordType,
                Difficulty = createDto.Difficulty,
                Category = createDto.Category,
                CreatedDate = DateTime.UtcNow,
                LastReviewedDate = null,
                ReviewCount = 0,
                IsFavorite = false
            };

            _context.Flashcards.Add(flashcard);
            await _context.SaveChangesAsync();
            return flashcard;
        }

        /// <summary>
        /// 更新現有單字卡
        /// </summary>
        public async Task<Flashcard?> UpdateFlashcardAsync(int id, UpdateFlashcardDto updateDto)
        {
            var flashcard = await _context.Flashcards.FindAsync(id);
            if (flashcard == null) return null;

            // 更新字段
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

            await _context.SaveChangesAsync();
            return flashcard;
        }

        /// <summary>
        /// 刪除單字卡
        /// </summary>
        public async Task<bool> DeleteFlashcardAsync(int id)
        {
            var flashcard = await _context.Flashcards.FindAsync(id);
            if (flashcard == null) return false;

            _context.Flashcards.Remove(flashcard);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 標記單字卡為已複習
        /// </summary>
        public async Task<Flashcard?> MarkAsReviewedAsync(int id)
        {
            var flashcard = await _context.Flashcards.FindAsync(id);
            if (flashcard == null) return null;

            flashcard.LastReviewedDate = DateTime.UtcNow;
            flashcard.ReviewCount++;
            await _context.SaveChangesAsync();
            return flashcard;
        }

        /// <summary>
        /// 取得隨機單字卡用於複習
        /// </summary>
        public async Task<IEnumerable<Flashcard>> GetRandomFlashcardsAsync(int count, Category? category = null, DifficultyLevel? difficulty = null)
        {
            var flashcards = _context.Flashcards.AsQueryable();

            if (category.HasValue)
                flashcards = flashcards.Where(f => f.Category == category.Value);

            if (difficulty.HasValue)
                flashcards = flashcards.Where(f => f.Difficulty == difficulty.Value);

            // 使用 SQL 的隨機排序功能
            return await flashcards
                .OrderBy(x => EF.Functions.Random())
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// 批量創建單字卡
        /// </summary>
        public async Task<BatchOperationResult> CreateFlashcardsBatchAsync(BatchCreateFlashcardsDto batchDto)
        {
            var result = new BatchOperationResult
            {
                TotalProcessed = batchDto.Flashcards.Count
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                foreach (var createDto in batchDto.Flashcards)
                {
                    try
                    {
                        // 檢查重複（如果設定要跳過重複）
                        if (batchDto.SkipDuplicates)
                        {
                            var isDuplicate = await _context.Flashcards.AnyAsync(f =>
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

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.ErrorCount = result.TotalProcessed;
                result.ErrorMessages.Clear();
                result.ErrorMessages.Add($"批量操作失敗: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// 從 CSV 檔案匯入單字卡
        /// </summary>
        public async Task<BatchOperationResult> ImportFromCsvAsync(Stream csvStream)
        {
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

        /// <summary>
        /// 匯出單字卡為 CSV 格式
        /// </summary>
        public async Task<byte[]> ExportToCsvAsync(ExportOptionsDto options)
        {
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

        /// <summary>
        /// 匯出單字卡為 JSON 格式
        /// </summary>
        public async Task<string> ExportToJsonAsync(ExportOptionsDto options)
        {
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
