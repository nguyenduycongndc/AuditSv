using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class AuditRequestMinitorUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detect_id1",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_detect_id1",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "detect_id1",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_detect_id",
                table: "AUDIT_REQUEST_MONITOR",
                column: "detect_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detect_id",
                table: "AUDIT_REQUEST_MONITOR",
                column: "detect_id",
                principalTable: "AUDIT_DETECT",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detect_id",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_detect_id",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.AddColumn<int>(
                name: "detect_id1",
                table: "AUDIT_REQUEST_MONITOR",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_detect_id1",
                table: "AUDIT_REQUEST_MONITOR",
                column: "detect_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detect_id1",
                table: "AUDIT_REQUEST_MONITOR",
                column: "detect_id1",
                principalTable: "AUDIT_DETECT",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
