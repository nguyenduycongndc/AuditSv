using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class CreateAuditWorkScopeFaciliity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_WORK_SCOPE_FACILITY",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    auditwork_id = table.Column<int>(type: "integer", nullable: true),
                    year = table.Column<int>(type: "integer", nullable: true),
                    auditfacilities_id = table.Column<int>(type: "integer", nullable: true),
                    auditfacilities_name = table.Column<string>(type: "text", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    risk_rating = table.Column<int>(type: "integer", nullable: true),
                    risk_rating_name = table.Column<string>(type: "text", nullable: true),
                    auditing_time_nearest = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    audit_rating_level_report = table.Column<int>(type: "integer", nullable: true),
                    base_rating_report = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    score_board_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_WORK_SCOPE_FACILITY", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_WORK_SCOPE_FACILITY_AUDIT_WORK_auditwork_id",
                        column: x => x.auditwork_id,
                        principalTable: "AUDIT_WORK",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_SCOPE_FACILITY_auditwork_id",
                table: "AUDIT_WORK_SCOPE_FACILITY",
                column: "auditwork_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_WORK_SCOPE_FACILITY");
        }
    }
}
