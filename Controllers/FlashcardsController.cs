using Microsoft.AspNetCore.Mvc;
using JapaneseFlashcardApi.Models;
using JapaneseFlashcardApi.Services;

namespace JapaneseFlashcardApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlashcardsController : ControllerBase
    {
        private readonly IFlashcardService _flashcardService;

        public FlashcardsController(IFlashcardService flashcardService)
        {
            _flashcardService = flashcardService;
        }

        /// <summary>
        /// 獲取所有單字卡
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flashcard>>> GetFlashcards([FromQuery] FlashcardQueryDto query)
        {
            var flashcards = await _flashcardService.GetAllFlashcardsAsync(query);
            return Ok(flashcards);
        }

        /// <summary>
        /// 根據 ID 獲取單字卡
        /// </summary>
        [HttpGet("{id}")]
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
        [HttpPost]
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
        /// 更新單字卡
        /// </summary>
        [HttpPut("{id}")]
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
        /// 刪除單字卡
        /// </summary>
        [HttpDelete("{id}")]
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
        /// </summary>
        [HttpPost("{id}/review")]
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
        /// 獲取隨機單字卡用於練習
        /// </summary>
        [HttpGet("random")]
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
        /// 獲取所有分類
        /// </summary>
        [HttpGet("categories")]
        public ActionResult<IEnumerable<object>> GetCategories()
        {
            var categories = Enum.GetValues<Category>()
                .Select(c => new { Value = (int)c, Name = c.ToString() })
                .ToList();
            return Ok(categories);
        }

        /// <summary>
        /// 獲取所有難度級別
        /// </summary>
        [HttpGet("difficulties")]
        public ActionResult<IEnumerable<object>> GetDifficulties()
        {
            var difficulties = Enum.GetValues<DifficultyLevel>()
                .Select(d => new { Value = (int)d, Name = d.ToString() })
                .ToList();
            return Ok(difficulties);
        }

        /// <summary>
        /// 獲取所有單字類型
        /// </summary>
        [HttpGet("wordtypes")]
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
    }
}
