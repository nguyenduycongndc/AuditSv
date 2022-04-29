using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class UpdateNameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reson",
                table: "EVALUATION_COMPLIANCE",
                newName: "reason");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reason",
                table: "EVALUATION_COMPLIANCE",
                newName: "reson");
        }
    }
}
