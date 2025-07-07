using JapaneseFlashcardApi.Services;
using JapaneseFlashcardApi.Data;
using Microsoft.EntityFrameworkCore;

// 日文單字卡 Web API 應用程式進入點
// 提供完整的日文學習單字卡管理功能

var builder = WebApplication.CreateBuilder(args);

// Railway PORT 支援 - 雲端部署時使用動態 PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// 註冊控制器服務
builder.Services.AddControllers();

// 註冊 API 探索服務（用於 Swagger）
builder.Services.AddEndpointsApiExplorer();

// 配置 Swagger API 文件
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Japanese Flashcard API", 
        Version = "v1",
        Description = "日文單字卡學習系統 API，支援 CRUD 操作、批量匯入匯出、複習功能等"
    });
    
    // 啟用 XML 註解支援
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 資料庫配置
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

Console.WriteLine($"🔍 環境變數檢查:");
Console.WriteLine($"   DATABASE_URL: {(Environment.GetEnvironmentVariable("DATABASE_URL") != null ? "已設定" : "未設定")}");
Console.WriteLine($"   連接字串長度: {connectionString?.Length ?? 0}");

if (!string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("🗄️  使用 PostgreSQL 資料庫儲存");
    Console.WriteLine($"   連接目標: {connectionString.Substring(0, Math.Min(50, connectionString.Length))}...");
    
    // Railway PostgreSQL 格式轉換 (postgres://user:pass@host:port/database)
    if (connectionString.StartsWith("postgres://"))
    {
        Console.WriteLine("🔄 轉換 Railway PostgreSQL 連接格式...");
        var uri = new Uri(connectionString);
        connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
        Console.WriteLine($"   轉換後格式: Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')}...");
    }
    
    builder.Services.AddDbContext<FlashcardDbContext>(options =>
        options.UseNpgsql(connectionString));
    
    // 使用資料庫服務
    builder.Services.AddScoped<IFlashcardService, DatabaseFlashcardService>();
}
else
{
    Console.WriteLine("🧠 使用記憶體儲存（開發模式）");
    Console.WriteLine("   注意：資料在應用程式重啟後會消失");
    
    // 如果沒有資料庫連接字串，使用記憶體服務（開發/測試用）
    builder.Services.AddScoped<IFlashcardService, FlashcardService>();
}

// 配置 CORS（跨域請求）
// 允許前端應用程式呼叫此 API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// 建立應用程式實例
var app = builder.Build();

// 資料庫自動遷移（僅在有資料庫時）
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<FlashcardDbContext>();
    if (context != null)
    {
        try
        {
            Console.WriteLine("🔄 開始資料庫遷移...");
            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("✅ 資料庫遷移完成");
        }
        catch (Exception ex)
        {
            // 記錄詳細錯誤但不停止應用程式啟動
            Console.WriteLine($"❌ 資料庫遷移失敗: {ex.Message}");
            Console.WriteLine($"詳細錯誤: {ex}");
            Console.WriteLine("⚠️  將使用記憶體儲存模式");
        }
    }
    else
    {
        Console.WriteLine("ℹ️  未配置資料庫，使用記憶體儲存");
    }
}

// 配置 HTTP 請求處理管線
if (app.Environment.IsDevelopment())
{
    // 開發環境啟用 Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Japanese Flashcard API v1");
        c.RoutePrefix = string.Empty; // 讓 Swagger UI 成為根頁面
        c.DocumentTitle = "日文單字卡 API 文件";
    });
}
else
{
    // 生產環境也啟用 Swagger（方便測試）
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Japanese Flashcard API v1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "日文單字卡 API 文件";
    });
}

// 健康檢查端點（Railway 部署需要）
app.MapGet("/health", () => new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName 
});

// 根路徑重導向到 Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// 取消 HTTPS 重導向（Railway 會在 Proxy 層處理 HTTPS）
// app.UseHttpsRedirection();

// 啟用 CORS
app.UseCors("AllowAll");

// 啟用授權（目前未實作身分驗證）
app.UseAuthorization();

// 對應控制器路由
app.MapControllers();

// 啟動應用程式
app.Run();
