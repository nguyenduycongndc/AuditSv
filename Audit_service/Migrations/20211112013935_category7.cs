using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class category7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "actualcontrol",
                table: "CAT_CONTROL",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "controlformat",
                table: "CAT_CONTROL",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "controlfrequency",
                table: "CAT_CONTROL",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "controltype",
                table: "CAT_CONTROL",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actualcontrol",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "controlformat",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "controlfrequency",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "controltype",
                table: "CAT_CONTROL");
        }
    }
}
