using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class updateAuditWork6_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "end_date_planning",
                table: "AUDIT_WORK",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "release_date",
                table: "AUDIT_WORK",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date_real",
                table: "AUDIT_WORK",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_date_planning",
                table: "AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "release_date",
                table: "AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "start_date_real",
                table: "AUDIT_WORK");
        }
    }
}
