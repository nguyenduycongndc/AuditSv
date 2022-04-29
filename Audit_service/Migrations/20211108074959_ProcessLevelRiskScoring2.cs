using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class ProcessLevelRiskScoring2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "potential_risk_rating_name",
                table: "PROCESS_LEVEL_RISK_SCORING",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "remaining_risk_rating_name",
                table: "PROCESS_LEVEL_RISK_SCORING",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "potential_risk_rating_name",
                table: "PROCESS_LEVEL_RISK_SCORING");

            migrationBuilder.DropColumn(
                name: "remaining_risk_rating_name",
                table: "PROCESS_LEVEL_RISK_SCORING");
        }
    }
}
