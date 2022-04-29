using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class AuditMinutesUpdatenew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_MINUTES_AUDIT_WORK_auditwork_id",
                table: "AUDIT_MINUTES");

            migrationBuilder.AlterColumn<int>(
                name: "auditwork_id",
                table: "AUDIT_MINUTES",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_MINUTES_AUDIT_WORK_auditwork_id",
                table: "AUDIT_MINUTES",
                column: "auditwork_id",
                principalTable: "AUDIT_WORK",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_MINUTES_AUDIT_WORK_auditwork_id",
                table: "AUDIT_MINUTES");

            migrationBuilder.AlterColumn<int>(
                name: "auditwork_id",
                table: "AUDIT_MINUTES",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_MINUTES_AUDIT_WORK_auditwork_id",
                table: "AUDIT_MINUTES",
                column: "auditwork_id",
                principalTable: "AUDIT_WORK",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
