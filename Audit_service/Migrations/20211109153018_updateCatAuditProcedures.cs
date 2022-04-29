using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class updateCatAuditProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cat_control_id",
                table: "CAT_AUDIT_PROCEDURES",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CAT_AUDIT_PROCEDURES_cat_control_id",
                table: "CAT_AUDIT_PROCEDURES",
                column: "cat_control_id");

            migrationBuilder.AddForeignKey(
                name: "FK_CAT_AUDIT_PROCEDURES_CAT_CONTROL_cat_control_id",
                table: "CAT_AUDIT_PROCEDURES",
                column: "cat_control_id",
                principalTable: "CAT_CONTROL",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CAT_AUDIT_PROCEDURES_CAT_CONTROL_cat_control_id",
                table: "CAT_AUDIT_PROCEDURES");

            migrationBuilder.DropIndex(
                name: "IX_CAT_AUDIT_PROCEDURES_cat_control_id",
                table: "CAT_AUDIT_PROCEDURES");

            migrationBuilder.DropColumn(
                name: "cat_control_id",
                table: "CAT_AUDIT_PROCEDURES");
        }
    }
}
