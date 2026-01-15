using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lama.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEvidence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Crear tabla Evidences
            migrationBuilder.CreateTable(
                name: "Evidences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    EvidenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PilotPhotoBlobPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OdometerPhotoBlobPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OdometerReading = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OdometerUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Kilometers"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AttendanceId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evidences_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evidences_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evidences_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Evidences_Attendance_AttendanceId",
                        column: x => x.AttendanceId,
                        principalTable: "Attendance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            // Crear índices
            migrationBuilder.CreateIndex(
                name: "IX_Evidences_TenantId_MemberId",
                table: "Evidences",
                columns: new[] { "TenantId", "MemberId" });

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_TenantId_Status",
                table: "Evidences",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_TenantId_EventId",
                table: "Evidences",
                columns: new[] { "TenantId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "UX_Evidences_CorrelationId",
                table: "Evidences",
                column: "CorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_CreatedAt",
                table: "Evidences",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_TenantId_VehicleId",
                table: "Evidences",
                columns: new[] { "TenantId", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_MemberId",
                table: "Evidences",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_VehicleId",
                table: "Evidences",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_EventId",
                table: "Evidences",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_AttendanceId",
                table: "Evidences",
                column: "AttendanceId",
                unique: true,
                filter: "[AttendanceId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evidences");
        }
    }
}
