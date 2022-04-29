using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "CAT_RISK",
                type: "integer",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "createdate",
                table: "CAT_RISK",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isdeleted",
                table: "CAT_RISK",
                type: "boolean",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "CAT_CONTROL",
                type: "integer",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "CAT_AUDIT_PROCEDURES",
                type: "integer",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdate",
                table: "CAT_RISK");

            migrationBuilder.DropColumn(
                name: "isdeleted",
                table: "CAT_RISK");

            migrationBuilder.AlterColumn<bool>(
                name: "status",
                table: "CAT_RISK",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "status",
                table: "CAT_CONTROL",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "status",
                table: "CAT_AUDIT_PROCEDURES",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
