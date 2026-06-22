using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureCSharpRAGAssistant.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddChatHistoryChunksJson2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Chunks",
                table: "ChatHistories",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Context",
                table: "ChatHistories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Chunks",
                table: "ChatHistories");

            migrationBuilder.DropColumn(
                name: "Context",
                table: "ChatHistories");
        }
    }
}
