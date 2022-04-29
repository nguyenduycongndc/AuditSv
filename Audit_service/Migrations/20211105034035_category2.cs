using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class category2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "CAT_RISK",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "relatestep",
                table: "CAT_RISK",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "CAT_RISK");

            migrationBuilder.DropColumn(
                name: "relatestep",
                table: "CAT_RISK");
        }
    }
}
