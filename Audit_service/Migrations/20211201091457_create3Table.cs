using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class create3Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_ASSIGNMENT_PLAN",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    auditwork_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    fullName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_ASSIGNMENT_PLAN", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_WORK_PLAN",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    target = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    num_of_workdays = table.Column<int>(type: "integer", nullable: true),
                    person_in_charge = table.Column<int>(type: "integer", nullable: true),
                    num_of_auditor = table.Column<int>(type: "integer", nullable: true),
                    req_skill_audit = table.Column<string>(type: "text", nullable: true),
                    req_outsourcing = table.Column<string>(type: "text", nullable: true),
                    req_other = table.Column<string>(type: "text", nullable: true),
                    scale_of_audit = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true),
                    execution_status = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    path = table.Column<string>(type: "text", nullable: true),
                    classify = table.Column<int>(type: "integer", nullable: true),
                    year = table.Column<string>(type: "text", nullable: true),
                    auditplan_id = table.Column<int>(type: "integer", nullable: true),
                    extension_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    audit_scope_outside = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_WORK_PLAN", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_WORK_PLAN_AUDIT_PLAN_auditplan_id",
                        column: x => x.auditplan_id,
                        principalTable: "AUDIT_PLAN",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_WORK_SCOPE_PLAN",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    auditwork_id = table.Column<int>(type: "integer", nullable: true),
                    auditprocess_id = table.Column<int>(type: "integer", nullable: true),
                    year = table.Column<int>(type: "integer", nullable: true),
                    auditprocess_name = table.Column<string>(type: "text", nullable: true),
                    bussinessactivities_id = table.Column<int>(type: "integer", nullable: true),
                    bussinessactivities_name = table.Column<string>(type: "text", nullable: true),
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
                    score_board_id = table.Column<int>(type: "integer", nullable: true),
                    path = table.Column<string>(type: "text", nullable: true),
                    brief_review = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_WORK_SCOPE_PLAN", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_PLAN_auditplan_id",
                table: "AUDIT_WORK_PLAN",
                column: "auditplan_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_ASSIGNMENT_PLAN");

            migrationBuilder.DropTable(
                name: "AUDIT_WORK_PLAN");

            migrationBuilder.DropTable(
                name: "AUDIT_WORK_SCOPE_PLAN");
        }
    }
}
