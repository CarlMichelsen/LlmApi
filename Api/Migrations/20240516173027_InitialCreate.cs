using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "LlmApi");

            migrationBuilder.CreateTable(
                name: "PriceEntity",
                schema: "LlmApi",
                columns: table => new
                {
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: true),
                    MillionInputTokenPrice = table.Column<int>(type: "integer", nullable: false),
                    MillionOutputTokenPrice = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceEntity", x => x.ModelId);
                });

            migrationBuilder.CreateTable(
                name: "ModelEntity",
                schema: "LlmApi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Available = table.Column<bool>(type: "boolean", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    ModelIdentifierName = table.Column<string>(type: "text", nullable: false),
                    MaxTokenCount = table.Column<long>(type: "bigint", nullable: false),
                    ContextTokenCount = table.Column<long>(type: "bigint", nullable: false),
                    ImageSupport = table.Column<bool>(type: "boolean", nullable: false),
                    VideoSupport = table.Column<bool>(type: "boolean", nullable: false),
                    JsonResponseOptimized = table.Column<bool>(type: "boolean", nullable: false),
                    ModelDisplayName = table.Column<string>(type: "text", nullable: false),
                    ModelDescription = table.Column<string>(type: "text", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelEntity_PriceEntity_Id",
                        column: x => x.Id,
                        principalSchema: "LlmApi",
                        principalTable: "PriceEntity",
                        principalColumn: "ModelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromptContentEntity",
                schema: "LlmApi",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    PromptMessageEntityId = table.Column<long>(type: "bigint", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptContentEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptEntity",
                schema: "LlmApi",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccessTokenIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderPromptIdentifier = table.Column<string>(type: "text", nullable: true),
                    InternalModelIdentifier = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Streamed = table.Column<bool>(type: "boolean", nullable: false),
                    PromptCompletionTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CurrentMillionInputTokenPrice = table.Column<int>(type: "integer", nullable: false),
                    CurrentMillionOutputTokenPrice = table.Column<int>(type: "integer", nullable: false),
                    InputTokens = table.Column<long>(type: "bigint", nullable: false),
                    OutputTokens = table.Column<long>(type: "bigint", nullable: false),
                    SystemMessage = table.Column<string>(type: "text", nullable: true),
                    ResponseMessageId = table.Column<long>(type: "bigint", nullable: false),
                    PromptFinnishedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptMessageEntity",
                schema: "LlmApi",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsUserMessage = table.Column<bool>(type: "boolean", nullable: false),
                    PromptEntityId = table.Column<long>(type: "bigint", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptMessageEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptMessageEntity_PromptEntity_PromptEntityId",
                        column: x => x.PromptEntityId,
                        principalSchema: "LlmApi",
                        principalTable: "PromptEntity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromptContentEntity_PromptMessageEntityId",
                schema: "LlmApi",
                table: "PromptContentEntity",
                column: "PromptMessageEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptEntity_ResponseMessageId",
                schema: "LlmApi",
                table: "PromptEntity",
                column: "ResponseMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptMessageEntity_PromptEntityId",
                schema: "LlmApi",
                table: "PromptMessageEntity",
                column: "PromptEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_PromptContentEntity_PromptMessageEntity_PromptMessageEntity~",
                schema: "LlmApi",
                table: "PromptContentEntity",
                column: "PromptMessageEntityId",
                principalSchema: "LlmApi",
                principalTable: "PromptMessageEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PromptEntity_PromptMessageEntity_ResponseMessageId",
                schema: "LlmApi",
                table: "PromptEntity",
                column: "ResponseMessageId",
                principalSchema: "LlmApi",
                principalTable: "PromptMessageEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromptEntity_PromptMessageEntity_ResponseMessageId",
                schema: "LlmApi",
                table: "PromptEntity");

            migrationBuilder.DropTable(
                name: "ModelEntity",
                schema: "LlmApi");

            migrationBuilder.DropTable(
                name: "PromptContentEntity",
                schema: "LlmApi");

            migrationBuilder.DropTable(
                name: "PriceEntity",
                schema: "LlmApi");

            migrationBuilder.DropTable(
                name: "PromptMessageEntity",
                schema: "LlmApi");

            migrationBuilder.DropTable(
                name: "PromptEntity",
                schema: "LlmApi");
        }
    }
}
