using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class auditrequestmonitor_123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "incomplete_reason",
                table: "AUDIT_REQUEST_MONITOR",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "incomplete_reason",
                table: "AUDIT_REQUEST_MONITOR");
        }
    }
}
