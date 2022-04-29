using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class updateAuditWorkScopeFacilityNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_STRATEGY_RISK_AUDIT_WORK_SCOPE_auditwork_scope_id",
                table: "AUDIT_STRATEGY_RISK");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_STRATEGY_RISK_AUDIT_WORK_SCOPE_FACILITY_auditwork_sco~",
                table: "AUDIT_STRATEGY_RISK",
                column: "auditwork_scope_id",
                principalTable: "AUDIT_WORK_SCOPE_FACILITY",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_STRATEGY_RISK_AUDIT_WORK_SCOPE_FACILITY_auditwork_sco~",
                table: "AUDIT_STRATEGY_RISK");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_STRATEGY_RISK_AUDIT_WORK_SCOPE_auditwork_scope_id",
                table: "AUDIT_STRATEGY_RISK",
                column: "auditwork_scope_id",
                principalTable: "AUDIT_WORK_SCOPE",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
