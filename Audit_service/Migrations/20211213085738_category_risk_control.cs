using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class category_risk_control : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "createdby",
                table: "CAT_RISK",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "createdby",
                table: "CAT_CONTROL",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "createby",
                table: "CAT_AUDIT_PROCEDURES",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdby",
                table: "CAT_RISK");

            migrationBuilder.DropColumn(
                name: "createdby",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "createby",
                table: "CAT_AUDIT_PROCEDURES");
        }
    }
}
