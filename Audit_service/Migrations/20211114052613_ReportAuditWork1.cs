using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class ReportAuditWork1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "base_rating_total",
                table: "REPORT_AUDIT_WORK",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date_field",
                table: "REPORT_AUDIT_WORK",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "general_conclusions",
                table: "REPORT_AUDIT_WORK",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "num_of_workdays",
                table: "REPORT_AUDIT_WORK",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rating_level_total",
                table: "REPORT_AUDIT_WORK",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "report_date",
                table: "REPORT_AUDIT_WORK",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date_field",
                table: "REPORT_AUDIT_WORK",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "audit_rating_report",
                table: "AUDIT_WORK_SCOPE",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "base_rating_report",
                table: "AUDIT_WORK_SCOPE",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "base_rating_total",
                table: "REPORT_AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "end_date_field",
                table: "REPORT_AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "general_conclusions",
                table: "REPORT_AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "num_of_workdays",
                table: "REPORT_AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "rating_level_total",
                table: "REPORT_AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "report_date",
                table: "REPORT_AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "start_date_field",
                table: "REPORT_AUDIT_WORK");

            migrationBuilder.DropColumn(
                name: "audit_rating_report",
                table: "AUDIT_WORK_SCOPE");

            migrationBuilder.DropColumn(
                name: "base_rating_report",
                table: "AUDIT_WORK_SCOPE");
        }
    }
}
