using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class updateAWS_addASR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "brief_review",
                table: "AUDIT_WORK_SCOPE",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "path",
                table: "AUDIT_WORK_SCOPE",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AUDIT_STRATEGY_RISK",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    auditwork_scope_id = table.Column<int>(type: "integer", nullable: false),
                    name_risk = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    risk_level = table.Column<int>(type: "integer", nullable: false),
                    audit_strategy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_STRATEGY_RISK", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_STRATEGY_RISK_AUDIT_WORK_SCOPE_auditwork_scope_id",
                        column: x => x.auditwork_scope_id,
                        principalTable: "AUDIT_WORK_SCOPE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_STRATEGY_RISK_auditwork_scope_id",
                table: "AUDIT_STRATEGY_RISK",
                column: "auditwork_scope_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_STRATEGY_RISK");

            migrationBuilder.DropColumn(
                name: "brief_review",
                table: "AUDIT_WORK_SCOPE");

            migrationBuilder.DropColumn(
                name: "path",
                table: "AUDIT_WORK_SCOPE");
        }
    }
}
