using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class AuditRequestMinitorUpdatedrop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_CAT_AUDIT_REQUEST_audit_request_type_~",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_audit_request_type_id1",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "audit_request_type_id1",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_audit_request_type_id",
                table: "AUDIT_REQUEST_MONITOR",
                column: "audit_request_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_CAT_AUDIT_REQUEST_audit_request_type_~",
                table: "AUDIT_REQUEST_MONITOR",
                column: "audit_request_type_id",
                principalTable: "CAT_AUDIT_REQUEST",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_CAT_AUDIT_REQUEST_audit_request_type_~",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_audit_request_type_id",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.AddColumn<int>(
                name: "audit_request_type_id1",
                table: "AUDIT_REQUEST_MONITOR",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_audit_request_type_id1",
                table: "AUDIT_REQUEST_MONITOR",
                column: "audit_request_type_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_CAT_AUDIT_REQUEST_audit_request_type_~",
                table: "AUDIT_REQUEST_MONITOR",
                column: "audit_request_type_id1",
                principalTable: "CAT_AUDIT_REQUEST",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
