using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class UpdateProcessLevelRiskScoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PROCESS_LEVEL_RISK_SCORING_AUDIT_WORK_SCOPE_auditscope_id",
                table: "PROCESS_LEVEL_RISK_SCORING");

            migrationBuilder.RenameColumn(
                name: "auditscope_id",
                table: "PROCESS_LEVEL_RISK_SCORING",
                newName: "auditprogram_id");

            migrationBuilder.RenameIndex(
                name: "IX_PROCESS_LEVEL_RISK_SCORING_auditscope_id",
                table: "PROCESS_LEVEL_RISK_SCORING",
                newName: "IX_PROCESS_LEVEL_RISK_SCORING_auditprogram_id");

            migrationBuilder.AddForeignKey(
                name: "FK_PROCESS_LEVEL_RISK_SCORING_AUDIT_PROGRAM_auditprogram_id",
                table: "PROCESS_LEVEL_RISK_SCORING",
                column: "auditprogram_id",
                principalTable: "AUDIT_PROGRAM",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PROCESS_LEVEL_RISK_SCORING_AUDIT_PROGRAM_auditprogram_id",
                table: "PROCESS_LEVEL_RISK_SCORING");

            migrationBuilder.RenameColumn(
                name: "auditprogram_id",
                table: "PROCESS_LEVEL_RISK_SCORING",
                newName: "auditscope_id");

            migrationBuilder.RenameIndex(
                name: "IX_PROCESS_LEVEL_RISK_SCORING_auditprogram_id",
                table: "PROCESS_LEVEL_RISK_SCORING",
                newName: "IX_PROCESS_LEVEL_RISK_SCORING_auditscope_id");

            migrationBuilder.AddForeignKey(
                name: "FK_PROCESS_LEVEL_RISK_SCORING_AUDIT_WORK_SCOPE_auditscope_id",
                table: "PROCESS_LEVEL_RISK_SCORING",
                column: "auditscope_id",
                principalTable: "AUDIT_WORK_SCOPE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
