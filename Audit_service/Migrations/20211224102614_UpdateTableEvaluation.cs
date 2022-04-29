using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class UpdateTableEvaluation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "point",
                table: "EVALUATION");

            migrationBuilder.AddColumn<int>(
                name: "evaluation_scale_id",
                table: "EVALUATION",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EVALUATION_evaluation_criteria_id",
                table: "EVALUATION",
                column: "evaluation_criteria_id");

            migrationBuilder.CreateIndex(
                name: "IX_EVALUATION_evaluation_scale_id",
                table: "EVALUATION",
                column: "evaluation_scale_id");

            migrationBuilder.AddForeignKey(
                name: "FK_EVALUATION_EVALUATION_CRITERIA_evaluation_criteria_id",
                table: "EVALUATION",
                column: "evaluation_criteria_id",
                principalTable: "EVALUATION_CRITERIA",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EVALUATION_EVALUATION_SCALE_evaluation_scale_id",
                table: "EVALUATION",
                column: "evaluation_scale_id",
                principalTable: "EVALUATION_SCALE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EVALUATION_EVALUATION_CRITERIA_evaluation_criteria_id",
                table: "EVALUATION");

            migrationBuilder.DropForeignKey(
                name: "FK_EVALUATION_EVALUATION_SCALE_evaluation_scale_id",
                table: "EVALUATION");

            migrationBuilder.DropIndex(
                name: "IX_EVALUATION_evaluation_criteria_id",
                table: "EVALUATION");

            migrationBuilder.DropIndex(
                name: "IX_EVALUATION_evaluation_scale_id",
                table: "EVALUATION");

            migrationBuilder.DropColumn(
                name: "evaluation_scale_id",
                table: "EVALUATION");

            migrationBuilder.AddColumn<double>(
                name: "point",
                table: "EVALUATION",
                type: "double precision",
                nullable: true);
        }
    }
}
