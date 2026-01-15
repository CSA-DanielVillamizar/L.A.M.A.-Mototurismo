using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lama.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompleteNamesNormalized : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompleteNamesNormalized",
                table: "Members",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_CompleteNamesNormalized",
                table: "Members",
                column: "CompleteNamesNormalized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_CompleteNamesNormalized",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CompleteNamesNormalized",
                table: "Members");
        }
    }
}
