using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class ProcessLevelRiskScoring1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "auditscope_id",
                table: "PROCESS_LEVEL_RISK_SCORING",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PROCESS_LEVEL_RISK_SCORING_auditscope_id",
                table: "PROCESS_LEVEL_RISK_SCORING",
                column: "auditscope_id");

            migrationBuilder.AddForeignKey(
                name: "FK_PROCESS_LEVEL_RISK_SCORING_AUDIT_WORK_SCOPE_auditscope_id",
                table: "PROCESS_LEVEL_RISK_SCORING",
                column: "auditscope_id",
                principalTable: "AUDIT_WORK_SCOPE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PROCESS_LEVEL_RISK_SCORING_AUDIT_WORK_SCOPE_auditscope_id",
                table: "PROCESS_LEVEL_RISK_SCORING");

            migrationBuilder.DropIndex(
                name: "IX_PROCESS_LEVEL_RISK_SCORING_auditscope_id",
                table: "PROCESS_LEVEL_RISK_SCORING");

            migrationBuilder.DropColumn(
                name: "auditscope_id",
                table: "PROCESS_LEVEL_RISK_SCORING");
        }
    }
}
