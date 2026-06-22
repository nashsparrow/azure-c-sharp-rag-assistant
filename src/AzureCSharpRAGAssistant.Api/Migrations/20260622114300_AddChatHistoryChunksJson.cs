using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureCSharpRAGAssistant.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddChatHistoryChunksJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ContentHash",
                table: "Documents",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ContentHash",
                table: "Documents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
