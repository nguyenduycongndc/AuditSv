using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class AddTableMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "flag",
                table: "AUDIT_REQUEST_MONITOR",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FACILITY_REQUEST_MONITOR_MAPPING",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    audit_request_monitor_id = table.Column<int>(type: "integer", nullable: false),
                    audit_facility_id = table.Column<int>(type: "integer", nullable: false),
                    audit_facility_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FACILITY_REQUEST_MONITOR_MAPPING", x => x.id);
                    table.ForeignKey(
                        name: "FK_FACILITY_REQUEST_MONITOR_MAPPING_AUDIT_FACILITY_audit_facil~",
                        column: x => x.audit_facility_id,
                        principalTable: "AUDIT_FACILITY",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FACILITY_REQUEST_MONITOR_MAPPING_AUDIT_REQUEST_MONITOR_audi~",
                        column: x => x.audit_request_monitor_id,
                        principalTable: "AUDIT_REQUEST_MONITOR",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FACILITY_REQUEST_MONITOR_MAPPING_audit_facility_id",
                table: "FACILITY_REQUEST_MONITOR_MAPPING",
                column: "audit_facility_id");

            migrationBuilder.CreateIndex(
                name: "IX_FACILITY_REQUEST_MONITOR_MAPPING_audit_request_monitor_id",
                table: "FACILITY_REQUEST_MONITOR_MAPPING",
                column: "audit_request_monitor_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FACILITY_REQUEST_MONITOR_MAPPING");

            migrationBuilder.DropColumn(
                name: "flag",
                table: "AUDIT_REQUEST_MONITOR");
        }
    }
}
