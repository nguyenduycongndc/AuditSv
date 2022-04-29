using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class AuditObserveAuditDetect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_DETECT",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    short_title = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    evidence = table.Column<string>(type: "text", nullable: true),
                    path_audit_detect = table.Column<string>(type: "text", nullable: true),
                    affect = table.Column<string>(type: "text", nullable: true),
                    rating_risk = table.Column<int>(type: "integer", nullable: true),
                    cause = table.Column<string>(type: "text", nullable: true),
                    audit_report = table.Column<bool>(type: "boolean", nullable: false),
                    classify_audit_detect = table.Column<int>(type: "integer", nullable: true),
                    summary_audit_detect = table.Column<string>(type: "text", nullable: true),
                    followers = table.Column<int>(type: "integer", nullable: true),
                    year = table.Column<int>(type: "integer", nullable: true),
                    opinion_audit = table.Column<bool>(type: "boolean", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true),
                    auditwork_id = table.Column<int>(type: "integer", nullable: true),
                    auditwork_name = table.Column<int>(type: "integer", nullable: true),
                    auditprocess_id = table.Column<int>(type: "integer", nullable: true),
                    auditfacilities_id = table.Column<int>(type: "integer", nullable: true),
                    auditfacilities_name = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_DETECT", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_DETECT_AUDIT_FACILITY_auditfacilities_id",
                        column: x => x.auditfacilities_id,
                        principalTable: "AUDIT_FACILITY",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AUDIT_DETECT_AUDIT_PROCESS_auditprocess_id",
                        column: x => x.auditprocess_id,
                        principalTable: "AUDIT_PROCESS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AUDIT_DETECT_AUDIT_WORK_auditwork_id",
                        column: x => x.auditwork_id,
                        principalTable: "AUDIT_WORK",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AUDIT_DETECT_USERS_followers",
                        column: x => x.followers,
                        principalTable: "USERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_OBSERVE",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    discoverer = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: true),
                    auditwork_id = table.Column<int>(type: "integer", nullable: true),
                    auditwork_name = table.Column<int>(type: "integer", nullable: true),
                    audit_detect_id = table.Column<int>(type: "integer", nullable: true),
                    working_paper_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_OBSERVE", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_OBSERVE_AUDIT_DETECT_audit_detect_id",
                        column: x => x.audit_detect_id,
                        principalTable: "AUDIT_DETECT",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AUDIT_OBSERVE_AUDIT_WORK_auditwork_id",
                        column: x => x.auditwork_id,
                        principalTable: "AUDIT_WORK",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AUDIT_OBSERVE_USERS_discoverer",
                        column: x => x.discoverer,
                        principalTable: "USERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_DETECT_auditfacilities_id",
                table: "AUDIT_DETECT",
                column: "auditfacilities_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_DETECT_auditprocess_id",
                table: "AUDIT_DETECT",
                column: "auditprocess_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_DETECT_auditwork_id",
                table: "AUDIT_DETECT",
                column: "auditwork_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_DETECT_followers",
                table: "AUDIT_DETECT",
                column: "followers");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_OBSERVE_audit_detect_id",
                table: "AUDIT_OBSERVE",
                column: "audit_detect_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_OBSERVE_auditwork_id",
                table: "AUDIT_OBSERVE",
                column: "auditwork_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_OBSERVE_discoverer",
                table: "AUDIT_OBSERVE",
                column: "discoverer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_DETECT");

            migrationBuilder.DropTable(
                name: "AUDIT_PROCESS");
        }
    }
}
