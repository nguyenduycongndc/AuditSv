using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class AuditWork1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_ASSIGNMENT_USERS_users_id",
                table: "AUDIT_ASSIGNMENT");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_ASSIGNMENT_users_id",
                table: "AUDIT_ASSIGNMENT");

            migrationBuilder.DropColumn(
                name: "users_id",
                table: "AUDIT_ASSIGNMENT");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_ASSIGNMENT_user_id",
                table: "AUDIT_ASSIGNMENT",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_ASSIGNMENT_USERS_user_id",
                table: "AUDIT_ASSIGNMENT",
                column: "user_id",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_ASSIGNMENT_USERS_user_id",
                table: "AUDIT_ASSIGNMENT");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_ASSIGNMENT_user_id",
                table: "AUDIT_ASSIGNMENT");

            migrationBuilder.AddColumn<int>(
                name: "users_id",
                table: "AUDIT_ASSIGNMENT",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_ASSIGNMENT_users_id",
                table: "AUDIT_ASSIGNMENT",
                column: "users_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_ASSIGNMENT_USERS_users_id",
                table: "AUDIT_ASSIGNMENT",
                column: "users_id",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
