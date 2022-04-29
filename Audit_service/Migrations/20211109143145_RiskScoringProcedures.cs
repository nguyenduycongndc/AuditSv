using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class RiskScoringProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RISK_SCORING_PROCEDURES",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    risk_scoring_id = table.Column<int>(type: "integer", nullable: true),
                    catprocedures_id = table.Column<int>(type: "integer", nullable: true),
                    lst_auditor = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RISK_SCORING_PROCEDURES", x => x.id);
                    table.ForeignKey(
                        name: "FK_RISK_SCORING_PROCEDURES_CAT_AUDIT_PROCEDURES_catprocedures_~",
                        column: x => x.catprocedures_id,
                        principalTable: "CAT_AUDIT_PROCEDURES",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RISK_SCORING_PROCEDURES_PROCESS_LEVEL_RISK_SCORING_risk_sco~",
                        column: x => x.risk_scoring_id,
                        principalTable: "PROCESS_LEVEL_RISK_SCORING",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RISK_SCORING_PROCEDURES_catprocedures_id",
                table: "RISK_SCORING_PROCEDURES",
                column: "catprocedures_id");

            migrationBuilder.CreateIndex(
                name: "IX_RISK_SCORING_PROCEDURES_risk_scoring_id",
                table: "RISK_SCORING_PROCEDURES",
                column: "risk_scoring_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RISK_SCORING_PROCEDURES");
        }
    }
}
