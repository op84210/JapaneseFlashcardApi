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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
}

Console.WriteLine($"🔍 環境變數檢查:");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
Console.WriteLine($"   DATABASE_URL 存在: {databaseUrl != null}");
Console.WriteLine($"   DATABASE_URL 長度: {databaseUrl?.Length ?? 0}");
Console.WriteLine($"   DATABASE_URL 值: '{databaseUrl ?? "null"}'");
Console.WriteLine($"   最終連接字串長度: {connectionString?.Length ?? 0}");
if (!string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine($"   連接字串前50字元: '{connectionString.Substring(0, Math.Min(50, connectionString.Length))}'");
}

if (!string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("🗄️  使用 PostgreSQL 資料庫儲存");
    Console.WriteLine($"   連接目標: {connectionString.Substring(0, Math.Min(50, connectionString.Length))}...");
    
    // Railway PostgreSQL 格式轉換 (postgresql://user:pass@host:port/database 或 postgres://user:pass@host:port/database)
    if (connectionString.StartsWith("postgresql://") || connectionString.StartsWith("postgres://"))
    {
        Console.WriteLine("🔄 轉換 Railway PostgreSQL 連接格式...");
        try
        {
            var uri = new Uri(connectionString);
            var userInfo = uri.UserInfo.Split(':');
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";
            var database = uri.AbsolutePath.Trim('/');
            
            connectionString = $"Host={uri.Host};Port={uri.Port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
            Console.WriteLine($"   轉換後格式: Host={uri.Host};Port={uri.Port};Database={database};Username={username};...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ URI 解析失敗: {ex.Message}");
            Console.WriteLine($"   原始連接字串: {connectionString}");
            throw;
        }
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

// 環境變數診斷端點（僅限開發/除錯用）
app.MapGet("/debug/env", () => 
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    var port = Environment.GetEnvironmentVariable("PORT") ?? "not set";
    var railwayEnvironment = Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") ?? "not set";
    
    return new 
    { 
        timestamp = DateTime.UtcNow,
        environment = app.Environment.EnvironmentName,
        databaseUrl = new 
        {
            exists = databaseUrl != null,
            length = databaseUrl?.Length ?? 0,
            preview = databaseUrl?.Length > 0 ? databaseUrl.Substring(0, Math.Min(30, databaseUrl.Length)) + "..." : "empty"
        },
        port,
        railwayEnvironment,
        message = "檢查 Railway 控制台中的 DATABASE_URL 環境變數設定"
    };
});

// 根路徑重導向到 Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// 取消 HTTPS 重導向（Railway 會在 Proxy 層處處理 HTTPS）
// app.UseHttpsRedirection();

// 啟用 CORS
app.UseCors("AllowAll");

// 啟用授權（目前未實作身分驗證）
app.UseAuthorization();

// 對應控制器路由
app.MapControllers();

// 啟動應用程式
app.Run();
