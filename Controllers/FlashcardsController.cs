using Microsoft.AspNetCore.Mvc;
using JapaneseFlashcardApi.Models;
using JapaneseFlashcardApi.Services;

namespace JapaneseFlashcardApi.Controllers
{
    /// <summary>
    /// 日文單字卡 API 控制器
    /// 提供單字卡的 CRUD 操作、查詢、複習、批量處理等功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FlashcardsController : ControllerBase
    {
        /// <summary>
        /// 單字卡服務實例
        /// </summary>
        private readonly IFlashcardService _flashcardService;

        /// <summary>
        /// 建構函式，注入單字卡服務
        /// </summary>
        /// <param name="flashcardService">單字卡服務介面</param>
        public FlashcardsController(IFlashcardService flashcardService)
        {
            _flashcardService = flashcardService;
        }

        /// <summary>
        /// 取得所有單字卡
        /// 支援分類、難易度、單字類型篩選，以及關鍵字搜尋和分頁
        /// </summary>
        /// <param name="query">查詢條件，包含篩選、搜尋、分頁參數</param>
        /// <returns>符合條件的單字卡清單</returns>
        /// <response code="200">成功取得單字卡清單</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Flashcard>), 200)]
        public async Task<ActionResult<IEnumerable<Flashcard>>> GetFlashcards([FromQuery] FlashcardQueryDto query)
        {
            var flashcards = await _flashcardService.GetAllFlashcardsAsync(query);
            return Ok(flashcards);
        }

        /// <summary>
        /// 根據 ID 取得特定單字卡
        /// </summary>
        /// <param name="id">單字卡的唯一識別碼</param>
        /// <returns>指定的單字卡詳細資訊</returns>
        /// <response code="200">成功取得單字卡</response>
        /// <response code="404">找不到指定的單字卡</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Flashcard), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Flashcard>> GetFlashcard(int id)
        {
            var flashcard = await _flashcardService.GetFlashcardByIdAsync(id);
            if (flashcard == null)
            {
                return NotFound($"找不到 ID 為 {id} 的單字卡");
            }
            return Ok(flashcard);
        }

        /// <summary>
        /// 創建新的單字卡
        /// </summary>
        /// <param name="createDto">包含新單字卡資料的請求物件</param>
        /// <returns>成功創建的單字卡</returns>
        /// <response code="201">成功創建單字卡</response>
        /// <response code="400">請求資料格式錯誤</response>
        [HttpPost]
        [ProducesResponseType(typeof(Flashcard), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Flashcard>> CreateFlashcard([FromBody] CreateFlashcardDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flashcard = await _flashcardService.CreateFlashcardAsync(createDto);
            return CreatedAtAction(nameof(GetFlashcard), new { id = flashcard.Id }, flashcard);
        }

        /// <summary>
        /// 更新現有單字卡
        /// 支援部分更新，只需提供要修改的欄位
        /// </summary>
        /// <param name="id">要更新的單字卡 ID</param>
        /// <param name="updateDto">包含更新資料的請求物件</param>
        /// <returns>更新後的單字卡</returns>
        /// <response code="200">成功更新單字卡</response>
        /// <response code="404">找不到指定的單字卡</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Flashcard), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Flashcard>> UpdateFlashcard(int id, [FromBody] UpdateFlashcardDto updateDto)
        {
            var flashcard = await _flashcardService.UpdateFlashcardAsync(id, updateDto);
            if (flashcard == null)
            {
                return NotFound($"找不到 ID 為 {id} 的單字卡");
            }
            return Ok(flashcard);
        }

        /// <summary>
        /// 刪除指定的單字卡
        /// </summary>
        /// <param name="id">要刪除的單字卡 ID</param>
        /// <returns>刪除操作結果</returns>
        /// <response code="204">成功刪除單字卡</response>
        /// <response code="404">找不到指定的單字卡</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteFlashcard(int id)
        {
            var deleted = await _flashcardService.DeleteFlashcardAsync(id);
            if (!deleted)
            {
                return NotFound($"找不到 ID 為 {id} 的單字卡");
            }
            return NoContent();
        }

        /// <summary>
        /// 標記單字卡為已複習
        /// 會自動更新最後複習時間和複習次數
        /// </summary>
        /// <param name="id">要標記的單字卡 ID</param>
        /// <returns>更新後的單字卡</returns>
        /// <response code="200">成功標記為已複習</response>
        /// <response code="404">找不到指定的單字卡</response>
        [HttpPost("{id}/review")]
        [ProducesResponseType(typeof(Flashcard), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Flashcard>> MarkAsReviewed(int id)
        {
            var flashcard = await _flashcardService.MarkAsReviewedAsync(id);
            if (flashcard == null)
            {
                return NotFound($"找不到 ID 為 {id} 的單字卡");
            }
            return Ok(flashcard);
        }

        /// <summary>
        /// 取得隨機單字卡用於練習
        /// 可依分類和難易度篩選
        /// </summary>
        /// <param name="count">要取得的單字卡數量（1-50）</param>
        /// <param name="category">可選的分類篩選</param>
        /// <param name="difficulty">可選的難易度篩選</param>
        /// <returns>隨機選取的單字卡清單</returns>
        /// <response code="200">成功取得隨機單字卡</response>
        /// <response code="400">參數驗證錯誤</response>
        [HttpGet("random")]
        [ProducesResponseType(typeof(IEnumerable<Flashcard>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<Flashcard>>> GetRandomFlashcards(
            [FromQuery] int count = 5,
            [FromQuery] Category? category = null,
            [FromQuery] DifficultyLevel? difficulty = null)
        {
            if (count <= 0 || count > 50)
            {
                return BadRequest("數量必須在 1 到 50 之間");
            }

            var flashcards = await _flashcardService.GetRandomFlashcardsAsync(count, category, difficulty);
            return Ok(flashcards);
        }

        /// <summary>
        /// 取得所有可用的單字分類
        /// </summary>
        /// <returns>分類清單及其對應的數值和名稱</returns>
        /// <response code="200">成功取得分類清單</response>
        [HttpGet("categories")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public ActionResult<IEnumerable<object>> GetCategories()
        {
            var categories = Enum.GetValues<Category>()
                .Select(c => new { Value = (int)c, Name = c.ToString() })
                .ToList();
            return Ok(categories);
        }

        /// <summary>
        /// 取得所有可用的難易度等級
        /// </summary>
        /// <returns>難易度清單及其對應的數值和描述</returns>
        /// <response code="200">成功取得難易度清單</response>
        [HttpGet("difficulties")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public ActionResult<IEnumerable<object>> GetDifficulties()
        {
            var difficulties = Enum.GetValues<DifficultyLevel>()
                .Select(d => new { 
                    Value = (int)d, 
                    Name = d.ToString(),
                    Description = d switch
                    {
                        DifficultyLevel.Beginner => "初學者等級",
                        DifficultyLevel.Intermediate => "中級等級", 
                        DifficultyLevel.Advanced => "高級等級",
                        DifficultyLevel.Expert => "專家等級",
                        _ => ""
                    }
                })
                .ToList();
            return Ok(difficulties);
        }

        /// <summary>
        /// 取得所有可用的單字類型
        /// </summary>
        /// <returns>單字類型清單及其對應的數值和描述</returns>
        /// <response code="200">成功取得單字類型清單</response>
        [HttpGet("wordtypes")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        public ActionResult<IEnumerable<object>> GetWordTypes()
        {
            var wordTypes = Enum.GetValues<WordType>()
                .Select(w => new { 
                    Value = (int)w, 
                    Name = w.ToString(),
                    Description = w switch
                    {
                        WordType.Native => "日文原生詞（主要使用平假名）",
                        WordType.SinoJapanese => "漢語詞（漢字+平假名讀音）",
                        WordType.Foreign => "外來語（主要使用片假名）",
                        WordType.Mixed => "混合型（平假名+片假名）",
                        _ => ""
                    }
                })
                .ToList();
            return Ok(wordTypes);
        }

        /// <summary>
        /// 批量創建多個單字卡
        /// 支援重複檢查和僅驗證模式
        /// </summary>
        /// <param name="batchDto">批量創建請求資料</param>
        /// <returns>批量操作結果統計</returns>
        /// <response code="200">批量操作完成</response>
        /// <response code="400">請求資料格式錯誤</response>
        [HttpPost("batch")]
        [ProducesResponseType(typeof(BatchOperationResult), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BatchOperationResult>> CreateFlashcardsBatch([FromBody] BatchCreateFlashcardsDto batchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!batchDto.Flashcards.Any())
            {
                return BadRequest("至少需要提供一個單字卡資料");
            }

            var result = await _flashcardService.CreateFlashcardsBatchAsync(batchDto);
            return Ok(result);
        }

        /// <summary>
        /// 從 CSV 檔案匯入單字卡
        /// CSV 格式：Kanji,Hiragana,Katakana,Meaning,Example,WordType,Difficulty,Category
        /// </summary>
        /// <param name="file">CSV 檔案</param>
        /// <returns>匯入操作結果統計</returns>
        /// <response code="200">匯入操作完成</response>
        /// <response code="400">檔案格式錯誤或檔案為空</response>
        [HttpPost("import/csv")]
        [ProducesResponseType(typeof(BatchOperationResult), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BatchOperationResult>> ImportFromCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("請選擇一個 CSV 檔案");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("檔案必須是 CSV 格式");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _flashcardService.ImportFromCsvAsync(stream);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"匯入失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 匯出單字卡為 CSV 檔案
        /// 支援依條件篩選要匯出的單字卡
        /// </summary>
        /// <param name="options">匯出選項和篩選條件</param>
        /// <returns>CSV 檔案下載</returns>
        /// <response code="200">成功匯出 CSV 檔案</response>
        /// <response code="400">匯出過程發生錯誤</response>
        [HttpGet("export/csv")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> ExportToCsv([FromQuery] ExportOptionsDto options)
        {
            try
            {
                var csvData = await _flashcardService.ExportToCsvAsync(options);
                var fileName = $"flashcards_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"匯出失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 匯出單字卡為 JSON 檔案
        /// 支援依條件篩選要匯出的單字卡
        /// </summary>
        /// <param name="options">匯出選項和篩選條件</param>
        /// <returns>JSON 檔案下載</returns>
        /// <response code="200">成功匯出 JSON 檔案</response>
        /// <response code="400">匯出過程發生錯誤</response>
        [HttpGet("export/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> ExportToJson([FromQuery] ExportOptionsDto options)
        {
            try
            {
                var jsonData = await _flashcardService.ExportToJsonAsync(options);
                var fileName = $"flashcards_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                
                return File(System.Text.Encoding.UTF8.GetBytes(jsonData), "application/json", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"匯出失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 下載 CSV 匯入範本檔案
        /// 包含範例資料和正確的欄位格式
        /// </summary>
        /// <returns>CSV 範本檔案</returns>
        /// <response code="200">成功下載範本檔案</response>
        [HttpGet("template/csv")]
        [ProducesResponseType(200)]
        public ActionResult GetCsvTemplate()
        {
            var csv = "Kanji,Hiragana,Katakana,Meaning,Example,WordType,Difficulty,Category\n" +
                     "犬,いぬ,,狗,私の犬はとても可愛いです。,1,1,1\n" +
                     ",,コーヒー,咖啡,朝のコーヒーは美味しいです。,2,1,3\n" +
                     ",おはよう,,早安,おはようございます。,0,1,0";
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "flashcards_template.csv");
        }
    }
}
