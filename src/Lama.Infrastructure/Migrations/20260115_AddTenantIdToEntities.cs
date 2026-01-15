using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lama.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Variable para el GUID por defecto de LAMA
            var defaultTenantId = new Guid("00000000-0000-0000-0000-000000000001");

            // Agregar columna TenantId a Members
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Members",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: defaultTenantId);

            // Agregar columna TenantId a Vehicles
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: defaultTenantId);

            // Agregar columna TenantId a Events
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Events",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: defaultTenantId);

            // Agregar columna TenantId a Attendance
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Attendance",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: defaultTenantId);

            // Crear índice compuesto (TenantId, Id) para optimizar queries filtradas por tenant
            migrationBuilder.CreateIndex(
                name: "IX_Members_TenantId",
                table: "Members",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TenantId",
                table: "Vehicles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_TenantId",
                table: "Events",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_TenantId",
                table: "Attendance",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar índices
            migrationBuilder.DropIndex(
                name: "IX_Members_TenantId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TenantId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Events_TenantId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_TenantId",
                table: "Attendance");

            // Eliminar columnas TenantId
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Attendance");
        }
    }
}
