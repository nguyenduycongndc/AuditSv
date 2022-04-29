using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class AuditObserveAuditDetectUpWorkingPaper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "working_paper_code",
                table: "AUDIT_OBSERVE",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WORKING_PAPER",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WORKING_PAPER", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_OBSERVE_working_paper_id",
                table: "AUDIT_OBSERVE",
                column: "working_paper_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_OBSERVE_WORKING_PAPER_working_paper_id",
                table: "AUDIT_OBSERVE",
                column: "working_paper_id",
                principalTable: "WORKING_PAPER",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_OBSERVE_WORKING_PAPER_working_paper_id",
                table: "AUDIT_OBSERVE");

            migrationBuilder.DropTable(
                name: "WORKING_PAPER");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_OBSERVE_working_paper_id",
                table: "AUDIT_OBSERVE");

            migrationBuilder.DropColumn(
                name: "working_paper_code",
                table: "AUDIT_OBSERVE");
        }
    }
}
