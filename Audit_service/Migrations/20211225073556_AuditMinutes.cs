using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class AuditMinutes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_MINUTES",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    year = table.Column<int>(type: "integer", nullable: true),
                    auditwork_id = table.Column<int>(type: "integer", nullable: true),
                    auditwork_name = table.Column<string>(type: "text", nullable: true),
                    auditwork_code = table.Column<string>(type: "text", nullable: true),
                    audit_work_taget = table.Column<string>(type: "text", nullable: true),
                    audit_work_person = table.Column<int>(type: "integer", nullable: false),
                    audit_work_classify = table.Column<int>(type: "integer", nullable: false),
                    auditfacilities_id = table.Column<int>(type: "integer", nullable: true),
                    auditfacilities_name = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true),
                    path = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_AUDIT_MINUTES", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_MINUTES_AUDIT_FACILITY_auditfacilities_id",
                        column: x => x.auditfacilities_id,
                        principalTable: "AUDIT_FACILITY",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AUDIT_MINUTES_AUDIT_WORK_auditwork_id",
                        column: x => x.auditwork_id,
                        principalTable: "AUDIT_WORK",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AUDIT_MINUTES_USERS_audit_work_person",
                        column: x => x.audit_work_person,
                        principalTable: "USERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_MINUTES_audit_work_person",
                table: "AUDIT_MINUTES",
                column: "audit_work_person");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_MINUTES_auditfacilities_id",
                table: "AUDIT_MINUTES",
                column: "auditfacilities_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_MINUTES_auditwork_id",
                table: "AUDIT_MINUTES",
                column: "auditwork_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_MINUTES");
        }
    }
}
