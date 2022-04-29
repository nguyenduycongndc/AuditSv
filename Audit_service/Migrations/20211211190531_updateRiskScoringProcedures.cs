using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class updateRiskScoringProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lst_auditor",
                table: "RISK_SCORING_PROCEDURES");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "RISK_SCORING_PROCEDURES",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RISK_SCORING_PROCEDURES_user_id",
                table: "RISK_SCORING_PROCEDURES",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_RISK_SCORING_PROCEDURES_USERS_user_id",
                table: "RISK_SCORING_PROCEDURES",
                column: "user_id",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RISK_SCORING_PROCEDURES_USERS_user_id",
                table: "RISK_SCORING_PROCEDURES");

            migrationBuilder.DropIndex(
                name: "IX_RISK_SCORING_PROCEDURES_user_id",
                table: "RISK_SCORING_PROCEDURES");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "RISK_SCORING_PROCEDURES");

            migrationBuilder.AddColumn<string>(
                name: "lst_auditor",
                table: "RISK_SCORING_PROCEDURES",
                type: "text",
                nullable: true);
        }
    }
}
