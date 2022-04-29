using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class AddFKFacilityandUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_FACILITY_ObjectClassId",
                table: "AUDIT_FACILITY",
                column: "ObjectClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_FACILITY_UNIT_TYPE_ObjectClassId",
                table: "AUDIT_FACILITY",
                column: "ObjectClassId",
                principalTable: "UNIT_TYPE",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_FACILITY_UNIT_TYPE_ObjectClassId",
                table: "AUDIT_FACILITY");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_FACILITY_ObjectClassId",
                table: "AUDIT_FACILITY");
        }
    }
}
