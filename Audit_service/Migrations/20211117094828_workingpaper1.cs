using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class workingpaper1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "approvedate",
                table: "WORKING_PAPER",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "asigneeid",
                table: "WORKING_PAPER",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "auditworkid",
                table: "WORKING_PAPER",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "conclusion",
                table: "WORKING_PAPER",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "processid",
                table: "WORKING_PAPER",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "prototype",
                table: "WORKING_PAPER",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "reviewerid",
                table: "WORKING_PAPER",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "riskid",
                table: "WORKING_PAPER",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "WORKING_PAPER",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "unitid",
                table: "WORKING_PAPER",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "year",
                table: "WORKING_PAPER",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "approvedate",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "asigneeid",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "auditworkid",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "conclusion",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "processid",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "prototype",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "reviewerid",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "riskid",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "status",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "unitid",
                table: "WORKING_PAPER");

            migrationBuilder.DropColumn(
                name: "year",
                table: "WORKING_PAPER");
        }
    }
}
