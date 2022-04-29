using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class UpdateColumnReposible : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EVALUATION_COMPLIANCE_USERS_reponsible",
                table: "EVALUATION_COMPLIANCE");

            migrationBuilder.AlterColumn<int>(
                name: "reponsible",
                table: "EVALUATION_COMPLIANCE",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_EVALUATION_COMPLIANCE_USERS_reponsible",
                table: "EVALUATION_COMPLIANCE",
                column: "reponsible",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EVALUATION_COMPLIANCE_USERS_reponsible",
                table: "EVALUATION_COMPLIANCE");

            migrationBuilder.AlterColumn<int>(
                name: "reponsible",
                table: "EVALUATION_COMPLIANCE",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EVALUATION_COMPLIANCE_USERS_reponsible",
                table: "EVALUATION_COMPLIANCE",
                column: "reponsible",
                principalTable: "USERS",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
