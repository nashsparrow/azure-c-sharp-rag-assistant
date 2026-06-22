using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureCSharpRAGAssistant.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddChatHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestName",
                table: "ChatHistories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TestRunName",
                table: "ChatHistories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Testing",
                table: "ChatHistories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestName",
                table: "ChatHistories");

            migrationBuilder.DropColumn(
                name: "TestRunName",
                table: "ChatHistories");

            migrationBuilder.DropColumn(
                name: "Testing",
                table: "ChatHistories");
        }
    }
}
