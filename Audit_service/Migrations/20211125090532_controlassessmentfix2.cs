using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class controlassessmentfix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CONTROL_ASSESSMENT",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    working_paper_id = table.Column<int>(type: "integer", nullable: true),
                    risk_id = table.Column<int>(type: "integer", nullable: true),
                    control_id = table.Column<int>(type: "integer", nullable: true),
                    design_assessment = table.Column<string>(type: "text", nullable: true),
                    design_conclusion = table.Column<string>(type: "text", nullable: true),
                    effective_assessment = table.Column<string>(type: "text", nullable: true),
                    effective_conclusion = table.Column<string>(type: "text", nullable: true),
                    risk_id1 = table.Column<int>(type: "integer", nullable: true),
                    control_id1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTROL_ASSESSMENT", x => x.id);
                    table.ForeignKey(
                        name: "FK_CONTROL_ASSESSMENT_CAT_CONTROL_control_id1",
                        column: x => x.control_id1,
                        principalTable: "CAT_CONTROL",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONTROL_ASSESSMENT_CAT_RISK_risk_id1",
                        column: x => x.risk_id1,
                        principalTable: "CAT_RISK",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CONTROL_ASSESSMENT_control_id1",
                table: "CONTROL_ASSESSMENT",
                column: "control_id1");

            migrationBuilder.CreateIndex(
                name: "IX_CONTROL_ASSESSMENT_risk_id1",
                table: "CONTROL_ASSESSMENT",
                column: "risk_id1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONTROL_ASSESSMENT");
        }
    }
}
