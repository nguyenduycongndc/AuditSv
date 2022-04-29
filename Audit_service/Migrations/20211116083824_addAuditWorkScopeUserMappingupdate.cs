using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class addAuditWorkScopeUserMappingupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_WORK_SCOPE_USER_MAPPING _USERS_users_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_WORK_SCOPE_USER_MAPPING _users_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ");

            migrationBuilder.DropColumn(
                name: "users_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_SCOPE_USER_MAPPING _user_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_WORK_SCOPE_USER_MAPPING _USERS_user_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ",
                column: "user_id",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_WORK_SCOPE_USER_MAPPING _USERS_user_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_WORK_SCOPE_USER_MAPPING _user_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ");

            migrationBuilder.AddColumn<int>(
                name: "users_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_SCOPE_USER_MAPPING _users_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ",
                column: "users_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_WORK_SCOPE_USER_MAPPING _USERS_users_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ",
                column: "users_id",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
