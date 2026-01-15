using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lama.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleAndUserScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Crear tabla UserRoles
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalSubjectId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            // Crear tabla UserScopes
            migrationBuilder.CreateTable(
                name: "UserScopes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalSubjectId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ScopeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScopeId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserScopes", x => x.Id);
                });

            // Crear índices para UserRoles
            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TenantId_ExternalSubjectId",
                table: "UserRoles",
                columns: new[] { "TenantId", "ExternalSubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TenantId_Role",
                table: "UserRoles",
                columns: new[] { "TenantId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TenantId_ExternalSubjectId_IsActive",
                table: "UserRoles",
                columns: new[] { "TenantId", "ExternalSubjectId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AssignedAt",
                table: "UserRoles",
                column: "AssignedAt");

            // Crear índices para UserScopes
            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_TenantId_ExternalSubjectId",
                table: "UserScopes",
                columns: new[] { "TenantId", "ExternalSubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_TenantId_ExternalSubjectId_ScopeType",
                table: "UserScopes",
                columns: new[] { "TenantId", "ExternalSubjectId", "ScopeType" });

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_TenantId_ExternalSubjectId_IsActive",
                table: "UserScopes",
                columns: new[] { "TenantId", "ExternalSubjectId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_TenantId_ScopeType_ScopeId",
                table: "UserScopes",
                columns: new[] { "TenantId", "ScopeType", "ScopeId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_AssignedAt",
                table: "UserScopes",
                column: "AssignedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserScopes");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
