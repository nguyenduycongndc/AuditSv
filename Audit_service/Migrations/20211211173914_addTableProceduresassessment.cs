using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class addTableProceduresassessment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PROCEDURES_ASSESSMENT",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    control_assessment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    procedures_id = table.Column<int>(type: "integer", nullable: true),
                    result = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROCEDURES_ASSESSMENT", x => x.id);
                    table.ForeignKey(
                        name: "FK_PROCEDURES_ASSESSMENT_CONTROL_ASSESSMENT_control_assessment~",
                        column: x => x.control_assessment_id,
                        principalTable: "CONTROL_ASSESSMENT",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PROCEDURES_ASSESSMENT_control_assessment_id",
                table: "PROCEDURES_ASSESSMENT",
                column: "control_assessment_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PROCEDURES_ASSESSMENT");
        }
    }
}
