using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class AddTableEvaluation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EVALUATION",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    year = table.Column<int>(type: "integer", nullable: true),
                    audit_id = table.Column<int>(type: "integer", nullable: true),
                    audit_code = table.Column<string>(type: "text", nullable: true),
                    point = table.Column<double>(type: "double precision", nullable: true),
                    plan = table.Column<string>(type: "text", nullable: true),
                    actual = table.Column<string>(type: "text", nullable: true),
                    explain = table.Column<string>(type: "text", nullable: true),
                    isdelete = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    evaluation_criteria_id = table.Column<int>(type: "integer", nullable: true),
                    evaluation_criteria_parent = table.Column<int>(type: "integer", nullable: true),
                    evaluation_criteria_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVALUATION", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EVALUATION");
        }
    }
}
