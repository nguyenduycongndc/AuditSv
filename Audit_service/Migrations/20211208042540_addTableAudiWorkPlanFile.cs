using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class addTableAudiWorkPlanFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "file_type",
                table: "AUDIT_PLAN",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AUDIT_WORK_PLAN_FILE",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    auditwork_id = table.Column<int>(type: "integer", nullable: false),
                    path = table.Column<string>(type: "text", nullable: true),
                    file_type = table.Column<string>(type: "text", nullable: true),
                    isdelete = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_WORK_PLAN_FILE", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_WORK_PLAN_FILE_AUDIT_WORK_PLAN_auditwork_id",
                        column: x => x.auditwork_id,
                        principalTable: "AUDIT_WORK_PLAN",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_PLAN_FILE_auditwork_id",
                table: "AUDIT_WORK_PLAN_FILE",
                column: "auditwork_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_WORK_PLAN_FILE");

            migrationBuilder.DropColumn(
                name: "file_type",
                table: "AUDIT_PLAN");
        }
    }
}
