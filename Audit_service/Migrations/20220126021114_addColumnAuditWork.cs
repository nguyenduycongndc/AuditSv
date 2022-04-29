using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class addColumnAuditWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "from_date",
                table: "AUDIT_WORK",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "to_date",
                table: "AUDIT_WORK",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "from_date",
                table: "AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "to_date",
                table: "AUDIT_WORK");
        }
    }
}
