using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class AuditWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_ASSIGNMENT",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    users_id = table.Column<int>(type: "integer", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_AUDIT_ASSIGNMENT", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_ASSIGNMENT_USERS_users_id",
                        column: x => x.users_id,
                        principalTable: "USERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_WORK",
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
                    table.PrimaryKey("PK_AUDIT_WORK", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_WORK_USERS_person_in_charge",
                        column: x => x.person_in_charge,
                        principalTable: "USERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_ASSIGNMENT_users_id",
                table: "AUDIT_ASSIGNMENT",
                column: "users_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_person_in_charge",
                table: "AUDIT_WORK",
                column: "person_in_charge");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_ASSIGNMENT");

            migrationBuilder.DropTable(
                name: "AUDIT_WORK");
        }
    }
}
