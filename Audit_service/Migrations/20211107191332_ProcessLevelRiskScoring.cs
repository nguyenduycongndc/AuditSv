using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class ProcessLevelRiskScoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PROCESS_LEVEL_RISK_SCORING",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    catrisk_id = table.Column<int>(type: "integer", nullable: true),
                    potential_possibility = table.Column<int>(type: "integer", nullable: true),
                    potential_infulence_level = table.Column<int>(type: "integer", nullable: true),
                    potential_risk_rating = table.Column<int>(type: "integer", nullable: true),
                    potential_reason_rating = table.Column<string>(type: "text", nullable: true),
                    audit_proposal = table.Column<bool>(type: "boolean", nullable: true),
                    remaining_possibility = table.Column<int>(type: "integer", nullable: true),
                    remaining_infulence_level = table.Column<int>(type: "integer", nullable: true),
                    remaining_risk_rating = table.Column<int>(type: "integer", nullable: true),
                    remaining_reason_rating = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROCESS_LEVEL_RISK_SCORING", x => x.id);
                    table.ForeignKey(
                        name: "FK_PROCESS_LEVEL_RISK_SCORING_CAT_RISK_catrisk_id",
                        column: x => x.catrisk_id,
                        principalTable: "CAT_RISK",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PROCESS_LEVEL_RISK_SCORING_catrisk_id",
                table: "PROCESS_LEVEL_RISK_SCORING",
                column: "catrisk_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PROCESS_LEVEL_RISK_SCORING");
        }
    }
}
