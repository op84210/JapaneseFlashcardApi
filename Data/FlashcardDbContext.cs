using Microsoft.EntityFrameworkCore;
using JapaneseFlashcardApi.Models;

namespace JapaneseFlashcardApi.Data
{
    /// <summary>
    /// 單字卡資料庫上下文
    /// 負責管理與 PostgreSQL 資料庫的連接和操作
    /// </summary>
    public class FlashcardDbContext : DbContext
    {
        public FlashcardDbContext(DbContextOptions<FlashcardDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// 單字卡資料表
        /// </summary>
        public DbSet<Flashcard> Flashcards { get; set; }

        /// <summary>
        /// 模型配置
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 設定 Flashcard 實體
            modelBuilder.Entity<Flashcard>(entity =>
            {
                // 主鍵
                entity.HasKey(e => e.Id);

                // 字段配置
                entity.Property(e => e.Kanji)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.Hiragana)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.Katakana)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.Meaning)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.Example)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                // 枚舉轉換為整數儲存
                entity.Property(e => e.WordType)
                    .HasConversion<int>();

                entity.Property(e => e.Difficulty)
                    .HasConversion<int>();

                entity.Property(e => e.Category)
                    .HasConversion<int>();

                // 日期時間配置
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastReviewedDate)
                    .IsRequired(false);

                // 索引設定（提升查詢效能）
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Difficulty);
                entity.HasIndex(e => e.WordType);
                entity.HasIndex(e => e.IsFavorite);
                entity.HasIndex(e => e.CreatedDate);
            });

            // 種子資料（預設資料）
            SeedData(modelBuilder);
        }

        /// <summary>
        /// 種子資料配置
        /// 在資料庫建立時自動插入範例資料
        /// </summary>
        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flashcard>().HasData(
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
                    CreatedDate = DateTime.UtcNow.AddDays(-30),
                    LastReviewedDate = DateTime.UtcNow.AddDays(-5),
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
                    CreatedDate = DateTime.UtcNow.AddDays(-25),
                    LastReviewedDate = DateTime.UtcNow.AddDays(-3),
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
                    CreatedDate = DateTime.UtcNow.AddDays(-20),
                    LastReviewedDate = DateTime.UtcNow.AddDays(-2),
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
                    CreatedDate = DateTime.UtcNow.AddDays(-15),
                    LastReviewedDate = null,
                    ReviewCount = 1,
                    IsFavorite = false
                }
            );
        }
    }
}
