﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                name: "Models",
                schema: "LlmApi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    ModelIdentifierName = table.Column<string>(type: "text", nullable: false),
                    MaxTokenCount = table.Column<long>(type: "bigint", nullable: false),
                    ImageSupport = table.Column<bool>(type: "boolean", nullable: false),
                    VideoSupport = table.Column<bool>(type: "boolean", nullable: false),
                    JsonResponseOptimized = table.Column<bool>(type: "boolean", nullable: false),
                    ModelDisplayName = table.Column<string>(type: "text", nullable: false),
                    ModelDescription = table.Column<string>(type: "text", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Models",
                schema: "LlmApi");
        }
    }
}
