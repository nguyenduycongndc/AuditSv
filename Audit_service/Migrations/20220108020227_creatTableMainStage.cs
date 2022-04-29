using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class creatTableMainStage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MAINSTAGE",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    auditwork_id = table.Column<int>(type: "integer", nullable: false),
                    expected_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    actual_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAINSTAGE", x => x.id);
                    table.ForeignKey(
                        name: "FK_MAINSTAGE_AUDIT_WORK_auditwork_id",
                        column: x => x.auditwork_id,
                        principalTable: "AUDIT_WORK",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MAINSTAGE_auditwork_id",
                table: "MAINSTAGE",
                column: "auditwork_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MAINSTAGE");
        }
    }
}
