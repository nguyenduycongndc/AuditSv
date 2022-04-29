using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class AddForeignKeyAuditminutes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_MINUTES_rating_level_total",
                table: "AUDIT_MINUTES",
                column: "rating_level_total");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_MINUTES_SYSTEM_CATEGORY_rating_level_total",
                table: "AUDIT_MINUTES",
                column: "rating_level_total",
                principalTable: "SYSTEM_CATEGORY",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_MINUTES_SYSTEM_CATEGORY_rating_level_total",
                table: "AUDIT_MINUTES");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_MINUTES_rating_level_total",
                table: "AUDIT_MINUTES");
        }
    }
}
