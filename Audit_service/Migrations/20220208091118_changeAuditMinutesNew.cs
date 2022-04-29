using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class changeAuditMinutesNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "idea",
                table: "AUDIT_MINUTES",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "problem",
                table: "AUDIT_MINUTES",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rating",
                table: "AUDIT_MINUTES",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idea",
                table: "AUDIT_MINUTES");

            migrationBuilder.DropColumn(
                name: "problem",
                table: "AUDIT_MINUTES");

            migrationBuilder.DropColumn(
                name: "rating",
                table: "AUDIT_MINUTES");
        }
    }
}
