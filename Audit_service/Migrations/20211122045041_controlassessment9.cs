using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class controlassessment9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "effectiveconclusion",
                table: "CONTROL_ASSESSMENT",
                newName: "effective_conclusion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "effective_conclusion",
                table: "CONTROL_ASSESSMENT",
                newName: "effectiveconclusion");
        }
    }
}
