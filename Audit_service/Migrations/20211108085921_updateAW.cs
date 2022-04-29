using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class updateAW : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "classify",
                table: "AUDIT_WORK",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "year",
                table: "AUDIT_WORK",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "classify",
                table: "AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "year",
                table: "AUDIT_WORK");
        }
    }
}
