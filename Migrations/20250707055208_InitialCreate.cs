using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JapaneseFlashcardApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flashcards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kanji = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Hiragana = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Katakana = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Meaning = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Example = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WordType = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastReviewedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flashcards", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Flashcards",
                columns: new[] { "Id", "Category", "CreatedDate", "Difficulty", "Example", "Hiragana", "IsFavorite", "Kanji", "Katakana", "LastReviewedDate", "Meaning", "ReviewCount", "WordType" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 6, 7, 5, 52, 7, 336, DateTimeKind.Utc).AddTicks(9165), 1, "私の犬はとても可愛いです。", "いぬ", true, "犬", "", new DateTime(2025, 7, 2, 5, 52, 7, 336, DateTimeKind.Utc).AddTicks(9638), "狗", 3, 1 },
                    { 2, 3, new DateTime(2025, 6, 12, 5, 52, 7, 337, DateTimeKind.Utc).AddTicks(3735), 1, "朝のコーヒーは美味しいです。", "", false, "", "コーヒー", new DateTime(2025, 7, 4, 5, 52, 7, 337, DateTimeKind.Utc).AddTicks(3738), "咖啡", 5, 2 },
                    { 3, 0, new DateTime(2025, 6, 17, 5, 52, 7, 337, DateTimeKind.Utc).AddTicks(3741), 1, "おはようございます。", "おはよう", true, "", "", new DateTime(2025, 7, 5, 5, 52, 7, 337, DateTimeKind.Utc).AddTicks(3741), "早安", 2, 0 },
                    { 4, 0, new DateTime(2025, 6, 22, 5, 52, 7, 337, DateTimeKind.Utc).AddTicks(3744), 2, "新しいコンピューターを買いました。", "", false, "", "コンピューター", new DateTime(2025, 7, 6, 5, 52, 7, 337, DateTimeKind.Utc).AddTicks(3744), "電腦", 1, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_Category",
                table: "Flashcards",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_CreatedDate",
                table: "Flashcards",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_Difficulty",
                table: "Flashcards",
                column: "Difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_IsFavorite",
                table: "Flashcards",
                column: "IsFavorite");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_WordType",
                table: "Flashcards",
                column: "WordType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flashcards");
        }
    }
}
