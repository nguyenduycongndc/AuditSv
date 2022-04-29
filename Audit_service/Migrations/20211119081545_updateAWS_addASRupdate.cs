using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class updateAWS_addASRupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Modified_at",
                table: "AUDIT_STRATEGY_RISK",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "AUDIT_STRATEGY_RISK",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "AUDIT_STRATEGY_RISK",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "AUDIT_STRATEGY_RISK",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "deleted_by",
                table: "AUDIT_STRATEGY_RISK",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "AUDIT_STRATEGY_RISK",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "AUDIT_STRATEGY_RISK",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "modified_by",
                table: "AUDIT_STRATEGY_RISK",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Modified_at",
                table: "AUDIT_STRATEGY_RISK");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "AUDIT_STRATEGY_RISK");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "AUDIT_STRATEGY_RISK");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "AUDIT_STRATEGY_RISK");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "AUDIT_STRATEGY_RISK");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "AUDIT_STRATEGY_RISK");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "AUDIT_STRATEGY_RISK");

            migrationBuilder.DropColumn(
                name: "modified_by",
                table: "AUDIT_STRATEGY_RISK");
        }
    }
}
