using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class createTableAuditWorkScopeFacilityFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_WORK_SCOPE_FACILITY_FILE",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    auditworkscopefacility_id = table.Column<int>(type: "integer", nullable: false),
                    path = table.Column<string>(type: "text", nullable: true),
                    file_type = table.Column<string>(type: "text", nullable: true),
                    isdelete = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_WORK_SCOPE_FACILITY_FILE", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_WORK_SCOPE_FACILITY_FILE_AUDIT_WORK_SCOPE_FACILITY_au~",
                        column: x => x.auditworkscopefacility_id,
                        principalTable: "AUDIT_WORK_SCOPE_FACILITY",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_SCOPE_FACILITY_FILE_auditworkscopefacility_id",
                table: "AUDIT_WORK_SCOPE_FACILITY_FILE",
                column: "auditworkscopefacility_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_WORK_SCOPE_FACILITY_FILE");
        }
    }
}
