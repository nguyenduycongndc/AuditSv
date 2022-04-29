using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class controlassessment6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "effective_conclusion",
                table: "CONTROL_ASSESSMENT",
                newName: "effectiveconclusion");

            migrationBuilder.RenameColumn(
                name: "effective_assessment",
                table: "CONTROL_ASSESSMENT",
                newName: "effectiveassessment");

            migrationBuilder.RenameColumn(
                name: "design_conclusion",
                table: "CONTROL_ASSESSMENT",
                newName: "designconclusion");

            migrationBuilder.RenameColumn(
                name: "design_assessmnet",
                table: "CONTROL_ASSESSMENT",
                newName: "designassessmnet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "effectiveconclusion",
                table: "CONTROL_ASSESSMENT",
                newName: "effective_conclusion");

            migrationBuilder.RenameColumn(
                name: "effectiveassessment",
                table: "CONTROL_ASSESSMENT",
                newName: "effective_assessment");

            migrationBuilder.RenameColumn(
                name: "designconclusion",
                table: "CONTROL_ASSESSMENT",
                newName: "design_conclusion");

            migrationBuilder.RenameColumn(
                name: "designassessmnet",
                table: "CONTROL_ASSESSMENT",
                newName: "design_assessmnet");
        }
    }
}
