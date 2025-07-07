using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JapaneseFlashcardApi.Data
{
    /// <summary>
    /// 設計時 DbContext 工廠
    /// 用於 Entity Framework Core 工具（如遷移）在設計時創建 DbContext
    /// </summary>
    public class FlashcardDbContextFactory : IDesignTimeDbContextFactory<FlashcardDbContext>
    {
        public FlashcardDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FlashcardDbContext>();
            
            // 使用本地 PostgreSQL 連接字串用於開發
            // 在生產環境中，會使用 Railway 提供的環境變數
            optionsBuilder.UseNpgsql("Host=localhost;Database=flashcards_dev;Username=postgres;Password=password");

            return new FlashcardDbContext(optionsBuilder.Options);
        }
    }
}
