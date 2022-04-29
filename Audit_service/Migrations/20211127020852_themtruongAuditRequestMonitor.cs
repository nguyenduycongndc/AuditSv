using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class themtruongAuditRequestMonitor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "AUDIT_REQUEST_MONITOR",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "AUDIT_REQUEST_MONITOR",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "AUDIT_REQUEST_MONITOR",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "deleted_by",
                table: "AUDIT_REQUEST_MONITOR",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "AUDIT_REQUEST_MONITOR",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "AUDIT_REQUEST_MONITOR",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "AUDIT_REQUEST_MONITOR",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "modified_by",
                table: "AUDIT_REQUEST_MONITOR",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "modified_at",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "modified_by",
                table: "AUDIT_REQUEST_MONITOR");
        }
    }
}
