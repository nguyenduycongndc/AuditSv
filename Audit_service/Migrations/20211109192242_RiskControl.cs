using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class RiskControl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RISK_CONTROL_controlid",
                table: "RISK_CONTROL",
                column: "controlid");

            migrationBuilder.CreateIndex(
                name: "IX_RISK_CONTROL_riskid",
                table: "RISK_CONTROL",
                column: "riskid");

            migrationBuilder.AddForeignKey(
                name: "FK_RISK_CONTROL_CAT_CONTROL_controlid",
                table: "RISK_CONTROL",
                column: "controlid",
                principalTable: "CAT_CONTROL",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RISK_CONTROL_CAT_RISK_riskid",
                table: "RISK_CONTROL",
                column: "riskid",
                principalTable: "CAT_RISK",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RISK_CONTROL_CAT_CONTROL_controlid",
                table: "RISK_CONTROL");

            migrationBuilder.DropForeignKey(
                name: "FK_RISK_CONTROL_CAT_RISK_riskid",
                table: "RISK_CONTROL");

            migrationBuilder.DropIndex(
                name: "IX_RISK_CONTROL_controlid",
                table: "RISK_CONTROL");

            migrationBuilder.DropIndex(
                name: "IX_RISK_CONTROL_riskid",
                table: "RISK_CONTROL");
        }
    }
}
