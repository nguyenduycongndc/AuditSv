using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class category4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "createdate",
                table: "CAT_AUDIT_PROCEDURES",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "CAT_AUDIT_PROCEDURES",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isdeleted",
                table: "CAT_AUDIT_PROCEDURES",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "relatestep",
                table: "CAT_AUDIT_PROCEDURES",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdate",
                table: "CAT_AUDIT_PROCEDURES");

            migrationBuilder.DropColumn(
                name: "description",
                table: "CAT_AUDIT_PROCEDURES");

            migrationBuilder.DropColumn(
                name: "isdeleted",
                table: "CAT_AUDIT_PROCEDURES");

            migrationBuilder.DropColumn(
                name: "relatestep",
                table: "CAT_AUDIT_PROCEDURES");
        }
    }
}
