using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class Scheduleupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SCHEDULE_auditwork_id",
                table: "SCHEDULE",
                column: "auditwork_id");

            migrationBuilder.CreateIndex(
                name: "IX_SCHEDULE_user_id",
                table: "SCHEDULE",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_SCHEDULE_AUDIT_WORK_auditwork_id",
                table: "SCHEDULE",
                column: "auditwork_id",
                principalTable: "AUDIT_WORK",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SCHEDULE_USERS_user_id",
                table: "SCHEDULE",
                column: "user_id",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SCHEDULE_AUDIT_WORK_auditwork_id",
                table: "SCHEDULE");

            migrationBuilder.DropForeignKey(
                name: "FK_SCHEDULE_USERS_user_id",
                table: "SCHEDULE");

            migrationBuilder.DropIndex(
                name: "IX_SCHEDULE_auditwork_id",
                table: "SCHEDULE");

            migrationBuilder.DropIndex(
                name: "IX_SCHEDULE_user_id",
                table: "SCHEDULE");
        }
    }
}
