using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class addTableAuditRequestMonitorFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_REQUEST_MONITOR_FiLE",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    audit_request_monitor_id = table.Column<int>(type: "integer", nullable: false),
                    path = table.Column<string>(type: "text", nullable: true),
                    file_type = table.Column<string>(type: "text", nullable: true),
                    isdelete = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_REQUEST_MONITOR_FiLE", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_REQUEST_MONITOR_FiLE_AUDIT_REQUEST_MONITOR_audit_requ~",
                        column: x => x.audit_request_monitor_id,
                        principalTable: "AUDIT_REQUEST_MONITOR",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_FiLE_audit_request_monitor_id",
                table: "AUDIT_REQUEST_MONITOR_FiLE",
                column: "audit_request_monitor_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_REQUEST_MONITOR_FiLE");
        }
    }
}
