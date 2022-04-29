using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class category1111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "editby",
                table: "CAT_RISK",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "editdate",
                table: "CAT_RISK",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "editby",
                table: "CAT_CONTROL",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "editdate",
                table: "CAT_CONTROL",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "editby",
                table: "CAT_AUDIT_PROCEDURES",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "editdate",
                table: "CAT_AUDIT_PROCEDURES",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "editby",
                table: "CAT_RISK");

            migrationBuilder.DropColumn(
                name: "editdate",
                table: "CAT_RISK");

            migrationBuilder.DropColumn(
                name: "editby",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "editdate",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "editby",
                table: "CAT_AUDIT_PROCEDURES");

            migrationBuilder.DropColumn(
                name: "editdate",
                table: "CAT_AUDIT_PROCEDURES");
        }
    }
}
