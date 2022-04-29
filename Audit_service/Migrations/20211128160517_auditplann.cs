using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class auditplann : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "approval_user",
                table: "AUDIT_PLAN",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reason_reject",
                table: "AUDIT_PLAN",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_PLAN_approval_user",
                table: "AUDIT_PLAN",
                column: "approval_user");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_PLAN_USERS_approval_user",
                table: "AUDIT_PLAN",
                column: "approval_user",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_PLAN_USERS_approval_user",
                table: "AUDIT_PLAN");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_PLAN_approval_user",
                table: "AUDIT_PLAN");

            migrationBuilder.DropColumn(
                name: "approval_user",
                table: "AUDIT_PLAN");

            migrationBuilder.DropColumn(
                name: "reason_reject",
                table: "AUDIT_PLAN");
        }
    }
}
