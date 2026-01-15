using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lama.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorExternalSubjectId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ActorMemberId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BeforeJson = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    AfterJson = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberStatusTypes",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberStatusTypes", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalSubjectId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserScopes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalSubjectId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ScopeType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScopeId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChapterId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    EventStartDateAAAAMMDD = table.Column<DateOnly>(name: "Event Start Date (AAAA/MM/DD)", type: "date", nullable: false),
                    Nameoftheevent = table.Column<string>(name: "Name of the event", type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Class = table.Column<int>(type: "int", nullable: false),
                    Mileage = table.Column<double>(type: "float", nullable: false),
                    Pointsperevent = table.Column<int>(name: "Points per event", type: "int", nullable: false),
                    PointsperDistance = table.Column<int>(name: "Points per Distance", type: "int", nullable: false),
                    Pointsawardedpermember = table.Column<int>(name: "Points awarded per member", type: "int", nullable: false),
                    StartLocationCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartLocationContinent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EndLocationCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EndLocationContinent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChapterId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CompleteNames = table.Column<string>(name: " Complete Names", type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Dama = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CountryBirth = table.Column<string>(name: "Country Birth", type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InLamaSince = table.Column<int>(name: "In Lama Since", type: "int", nullable: true),
                    STATUS = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    is_eligible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Continent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    MemberStatusTypeStatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Members_MemberStatusTypes_MemberStatusTypeStatusId",
                        column: x => x.MemberStatusTypeStatusId,
                        principalTable: "MemberStatusTypes",
                        principalColumn: "StatusId");
                });

            migrationBuilder.CreateTable(
                name: "IdentityUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalSubjectId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUsers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    MotorcycleData = table.Column<string>(name: " Motorcycle Data", type: "nvarchar(max)", nullable: false),
                    LicPlate = table.Column<string>(name: "Lic Plate", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Trike = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Photography = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "PENDING"),
                    StartYearEvidenceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CutOffEvidenceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartYearEvidenceValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CutOffEvidenceValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvidenceValidatedBy = table.Column<int>(type: "int", nullable: true),
                    OdometerUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Miles"),
                    StartingOdometer = table.Column<double>(name: "Starting Odometer", type: "float", nullable: true),
                    FinalOdometer = table.Column<double>(name: "Final Odometer", type: "float", nullable: true),
                    StartingOdometerDate = table.Column<DateOnly>(name: "Starting Odometer Date", type: "date", nullable: true),
                    FinalOdometerDate = table.Column<DateOnly>(name: "Final Odometer Date", type: "date", nullable: true),
                    IsActiveForChampionship = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Members_EvidenceValidatedBy",
                        column: x => x.EvidenceValidatedBy,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vehicles_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "PENDING"),
                    Pointsperevent = table.Column<int>(name: "Points per event", type: "int", nullable: true),
                    PointsperDistance = table.Column<int>(name: "Points per Distance", type: "int", nullable: true),
                    Pointsawardedpermember = table.Column<int>(name: "Points awarded per member", type: "int", nullable: true),
                    VisitorClass = table.Column<string>(name: "Visitor Class", type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendance_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendance_Members_ConfirmedBy",
                        column: x => x.ConfirmedBy,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Attendance_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendance_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                        name: "FK_Evidences_Attendance_AttendanceId",
                        column: x => x.AttendanceId,
                        principalTable: "Attendance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Evidences_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_ConfirmedBy",
                table: "Attendance",
                column: "ConfirmedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_MemberId",
                table: "Attendance",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_VehicleId",
                table: "Attendance",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "UQ_Attendance_MemberEvent",
                table: "Attendance",
                columns: new[] { "EventId", "MemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CorrelationId",
                table: "AuditLogs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantActionDate",
                table: "AuditLogs",
                columns: new[] { "TenantId", "Action", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantDate",
                table: "AuditLogs",
                columns: new[] { "TenantId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantEntityDate",
                table: "AuditLogs",
                columns: new[] { "TenantId", "EntityType", "EntityId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantIpDate",
                table: "AuditLogs",
                columns: new[] { "TenantId", "IpAddress", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantMemberDate",
                table: "AuditLogs",
                columns: new[] { "TenantId", "ActorMemberId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_Key",
                table: "Configuration",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_ChapterId",
                table: "Events",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_AttendanceId",
                table: "Evidences",
                column: "AttendanceId",
                unique: true,
                filter: "[AttendanceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_CreatedAt",
                table: "Evidences",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_EventId",
                table: "Evidences",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_MemberId",
                table: "Evidences",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_TenantId_EventId",
                table: "Evidences",
                columns: new[] { "TenantId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_TenantId_MemberId",
                table: "Evidences",
                columns: new[] { "TenantId", "MemberId" });

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_TenantId_Status",
                table: "Evidences",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_TenantId_VehicleId",
                table: "Evidences",
                columns: new[] { "TenantId", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Evidences_VehicleId",
                table: "Evidences",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "UX_Evidences_CorrelationId",
                table: "Evidences",
                column: "CorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUsers_CreatedAt",
                table: "IdentityUsers",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUsers_MemberId",
                table: "IdentityUsers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUsers_TenantId",
                table: "IdentityUsers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUsers_TenantId_Email",
                table: "IdentityUsers",
                columns: new[] { "TenantId", "Email" });

            migrationBuilder.CreateIndex(
                name: "UX_IdentityUsers_TenantId_ExternalSubjectId",
                table: "IdentityUsers",
                columns: new[] { "TenantId", "ExternalSubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_ChapterId",
                table: "Members",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberStatusTypeStatusId",
                table: "Members",
                column: "MemberStatusTypeStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberStatusTypes_DisplayOrder",
                table: "MemberStatusTypes",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "UQ_MemberStatusTypes_StatusName",
                table: "MemberStatusTypes",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RankingSnapshots_LastCalculatedAt",
                table: "RankingSnapshots",
                column: "LastCalculatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RankingSnapshots_MemberId",
                table: "RankingSnapshots",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RankingSnapshots_Scope_Points",
                table: "RankingSnapshots",
                columns: new[] { "TenantId", "Year", "ScopeType", "ScopeId", "TotalPoints" });

            migrationBuilder.CreateIndex(
                name: "IX_RankingSnapshots_TenantYear_Scope",
                table: "RankingSnapshots",
                columns: new[] { "TenantId", "Year", "ScopeType", "ScopeId" });

            migrationBuilder.CreateIndex(
                name: "IX_RankingSnapshots_Year_CreatedAt",
                table: "RankingSnapshots",
                columns: new[] { "Year", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "UX_RankingSnapshots_TenantYear_Scope_Member",
                table: "RankingSnapshots",
                columns: new[] { "TenantId", "Year", "ScopeType", "ScopeId", "MemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AssignedAt",
                table: "UserRoles",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TenantId_ExternalSubjectId",
                table: "UserRoles",
                columns: new[] { "TenantId", "ExternalSubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TenantId_ExternalSubjectId_IsActive",
                table: "UserRoles",
                columns: new[] { "TenantId", "ExternalSubjectId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TenantId_Role",
                table: "UserRoles",
                columns: new[] { "TenantId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_AssignedAt",
                table: "UserScopes",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_TenantId_ExternalSubjectId",
                table: "UserScopes",
                columns: new[] { "TenantId", "ExternalSubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_TenantId_ExternalSubjectId_IsActive",
                table: "UserScopes",
                columns: new[] { "TenantId", "ExternalSubjectId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_TenantId_ExternalSubjectId_ScopeType",
                table: "UserScopes",
                columns: new[] { "TenantId", "ExternalSubjectId", "ScopeType" });

            migrationBuilder.CreateIndex(
                name: "IX_UserScopes_TenantId_ScopeType_ScopeId",
                table: "UserScopes",
                columns: new[] { "TenantId", "ScopeType", "ScopeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_EvidenceValidatedBy",
                table: "Vehicles",
                column: "EvidenceValidatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Lic Plate",
                table: "Vehicles",
                column: "Lic Plate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_MemberId",
                table: "Vehicles",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "Evidences");

            migrationBuilder.DropTable(
                name: "IdentityUsers");

            migrationBuilder.DropTable(
                name: "RankingSnapshots");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserScopes");

            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "MemberStatusTypes");
        }
    }
}
