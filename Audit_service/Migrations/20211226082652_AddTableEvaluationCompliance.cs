using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class AddTableEvaluationCompliance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EVALUATION_COMPLIANCE",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    year = table.Column<int>(type: "integer", nullable: true),
                    audit_id = table.Column<int>(type: "integer", nullable: true),
                    evaluation_standard_id = table.Column<int>(type: "integer", nullable: true),
                    evaluation_standard_code = table.Column<string>(type: "text", nullable: true),
                    evaluation_standard_title = table.Column<string>(type: "text", nullable: true),
                    evaluation_standard_request = table.Column<string>(type: "text", nullable: true),
                    compliance = table.Column<bool>(type: "boolean", nullable: false),
                    reson = table.Column<string>(type: "text", nullable: true),
                    plan = table.Column<string>(type: "text", nullable: true),
                    time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    reponsible = table.Column<int>(type: "integer", nullable: false),
                    isdelete = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVALUATION_COMPLIANCE", x => x.id);
                    table.ForeignKey(
                        name: "FK_EVALUATION_COMPLIANCE_EVALUATION_STANDARD_evaluation_standa~",
                        column: x => x.evaluation_standard_id,
                        principalTable: "EVALUATION_STANDARD",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EVALUATION_COMPLIANCE_USERS_reponsible",
                        column: x => x.reponsible,
                        principalTable: "USERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EVALUATION_COMPLIANCE_evaluation_standard_id",
                table: "EVALUATION_COMPLIANCE",
                column: "evaluation_standard_id");

            migrationBuilder.CreateIndex(
                name: "IX_EVALUATION_COMPLIANCE_reponsible",
                table: "EVALUATION_COMPLIANCE",
                column: "reponsible");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EVALUATION_COMPLIANCE");
        }
    }
}
