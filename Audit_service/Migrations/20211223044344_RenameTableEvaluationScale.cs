using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class RenameTableEvaluationScale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EvaluationScale",
                table: "EvaluationScale");

            migrationBuilder.RenameTable(
                name: "EvaluationScale",
                newName: "EVALUATION_SCALE");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EVALUATION_SCALE",
                table: "EVALUATION_SCALE",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EVALUATION_SCALE",
                table: "EVALUATION_SCALE");

            migrationBuilder.RenameTable(
                name: "EVALUATION_SCALE",
                newName: "EvaluationScale");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EvaluationScale",
                table: "EvaluationScale",
                column: "id");
        }
    }
}
