using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class auditrequest5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_detectid",
                table: "AUDIT_REQUEST_MONITOR",
                column: "detectid");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detectid",
                table: "AUDIT_REQUEST_MONITOR",
                column: "detectid",
                principalTable: "AUDIT_DETECT",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detectid",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_detectid",
                table: "AUDIT_REQUEST_MONITOR");
        }
    }
}
