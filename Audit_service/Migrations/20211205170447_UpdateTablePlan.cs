using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class UpdateTablePlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_WORK_AUDIT_PLAN_auditplan_id",
                table: "AUDIT_WORK");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_WORK_auditplan_id",
                table: "AUDIT_WORK");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_SCOPE_PLAN_auditwork_id",
                table: "AUDIT_WORK_SCOPE_PLAN",
                column: "auditwork_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_PLAN_person_in_charge",
                table: "AUDIT_WORK_PLAN",
                column: "person_in_charge");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_ASSIGNMENT_PLAN_auditwork_id",
                table: "AUDIT_ASSIGNMENT_PLAN",
                column: "auditwork_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_ASSIGNMENT_PLAN_user_id",
                table: "AUDIT_ASSIGNMENT_PLAN",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_ASSIGNMENT_PLAN_AUDIT_WORK_PLAN_auditwork_id",
                table: "AUDIT_ASSIGNMENT_PLAN",
                column: "auditwork_id",
                principalTable: "AUDIT_WORK_PLAN",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_ASSIGNMENT_PLAN_USERS_user_id",
                table: "AUDIT_ASSIGNMENT_PLAN",
                column: "user_id",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_WORK_PLAN_USERS_person_in_charge",
                table: "AUDIT_WORK_PLAN",
                column: "person_in_charge",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_WORK_SCOPE_PLAN_AUDIT_WORK_PLAN_auditwork_id",
                table: "AUDIT_WORK_SCOPE_PLAN",
                column: "auditwork_id",
                principalTable: "AUDIT_WORK_PLAN",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_ASSIGNMENT_PLAN_AUDIT_WORK_PLAN_auditwork_id",
                table: "AUDIT_ASSIGNMENT_PLAN");

            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_ASSIGNMENT_PLAN_USERS_user_id",
                table: "AUDIT_ASSIGNMENT_PLAN");

            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_WORK_PLAN_USERS_person_in_charge",
                table: "AUDIT_WORK_PLAN");

            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_WORK_SCOPE_PLAN_AUDIT_WORK_PLAN_auditwork_id",
                table: "AUDIT_WORK_SCOPE_PLAN");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_WORK_SCOPE_PLAN_auditwork_id",
                table: "AUDIT_WORK_SCOPE_PLAN");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_WORK_PLAN_person_in_charge",
                table: "AUDIT_WORK_PLAN");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_ASSIGNMENT_PLAN_auditwork_id",
                table: "AUDIT_ASSIGNMENT_PLAN");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_ASSIGNMENT_PLAN_user_id",
                table: "AUDIT_ASSIGNMENT_PLAN");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_auditplan_id",
                table: "AUDIT_WORK",
                column: "auditplan_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_WORK_AUDIT_PLAN_auditplan_id",
                table: "AUDIT_WORK",
                column: "auditplan_id",
                principalTable: "AUDIT_PLAN",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
