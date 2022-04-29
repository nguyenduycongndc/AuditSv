using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class workingpaper2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isdelete",
                table: "WORKING_PAPER",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "target",
                table: "WORKING_PAPER",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isdelete",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "target",
                table: "WORKING_PAPER");
        }
    }
}
