using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lama.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRankingSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RankingSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ScopeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScopeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: true, comment: "Posición en el ranking (1 = mejor)"),
                    TotalPoints = table.Column<int>(type: "int", nullable: false),
                    TotalMiles = table.Column<double>(type: "float", nullable: false),
                    EventsCount = table.Column<int>(type: "int", nullable: false),
                    VisitorClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastCalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankingSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RankingSnapshots_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Index 1: Principal para consultas por ámbito
            migrationBuilder.CreateIndex(
                name: "IX_RankingSnapshots_TenantYear_Scope",
                table: "RankingSnapshots",
                columns: new[] { "TenantId", "Year", "ScopeType", "ScopeId" });

            // Index 2: Único para búsqueda de miembro en ranking
            migrationBuilder.CreateIndex(
                name: "UX_RankingSnapshots_TenantYear_Scope_Member",
                table: "RankingSnapshots",
                columns: new[] { "TenantId", "Year", "ScopeType", "ScopeId", "MemberId" },
                unique: true);

            // Index 3: Para ordenamiento por puntos
            migrationBuilder.CreateIndex(
                name: "IX_RankingSnapshots_Scope_Points",
                table: "RankingSnapshots",
                columns: new[] { "TenantId", "Year", "ScopeType", "ScopeId", "TotalPoints" });

            // Index 4: Para LastCalculatedAt
            migrationBuilder.CreateIndex(
                name: "IX_RankingSnapshots_LastCalculatedAt",
                table: "RankingSnapshots",
                column: "LastCalculatedAt");

            // Index 5: Para búsquedas por MemberId
            migrationBuilder.CreateIndex(
                name: "IX_RankingSnapshots_MemberId",
                table: "RankingSnapshots",
                column: "MemberId");

            // Index 6: Para limpieza de datos históricos
            migrationBuilder.CreateIndex(
                name: "IX_RankingSnapshots_Year_CreatedAt",
                table: "RankingSnapshots",
                columns: new[] { "Year", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RankingSnapshots");
        }
    }
}
