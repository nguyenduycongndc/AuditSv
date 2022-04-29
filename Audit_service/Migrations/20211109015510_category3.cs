using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class category3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "createdate",
                table: "CAT_CONTROL",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "CAT_CONTROL",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isdeleted",
                table: "CAT_CONTROL",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "relatestep",
                table: "CAT_CONTROL",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdate",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "description",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "isdeleted",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "relatestep",
                table: "CAT_CONTROL");
        }
    }
}
