using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class addTalbeFileWP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "AUDIT_OBSERVE",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AUDIT_ASSESSMENT_FILE",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    control_assessment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    path = table.Column<string>(type: "text", nullable: true),
                    file_type = table.Column<string>(type: "text", nullable: true),
                    isdelete = table.Column<bool>(type: "boolean", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_ASSESSMENT_FILE", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_ASSESSMENT_FILE_CONTROL_ASSESSMENT_control_assessment~",
                        column: x => x.control_assessment_id,
                        principalTable: "CONTROL_ASSESSMENT",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_OBSERVE_FILE",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    observe_id = table.Column<int>(type: "integer", nullable: false),
                    path = table.Column<string>(type: "text", nullable: true),
                    file_type = table.Column<string>(type: "text", nullable: true),
                    isdelete = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_OBSERVE_FILE", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_OBSERVE_FILE_AUDIT_OBSERVE_observe_id",
                        column: x => x.observe_id,
                        principalTable: "AUDIT_OBSERVE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_ASSESSMENT_FILE_control_assessment_id",
                table: "AUDIT_ASSESSMENT_FILE",
                column: "control_assessment_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_OBSERVE_FILE_observe_id",
                table: "AUDIT_OBSERVE_FILE",
                column: "observe_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_ASSESSMENT_FILE");

            migrationBuilder.DropTable(
                name: "AUDIT_OBSERVE_FILE");

            migrationBuilder.DropColumn(
                name: "type",
                table: "AUDIT_OBSERVE");
        }
    }
}
