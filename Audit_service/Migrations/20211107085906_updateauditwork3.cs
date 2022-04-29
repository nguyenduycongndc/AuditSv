using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class updateauditwork3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "AUDIT_WORK",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "AUDIT_WORK");
        }
    }
}
