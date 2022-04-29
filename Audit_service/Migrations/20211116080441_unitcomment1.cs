using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class unitcomment1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UNIT_COMMENT",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    counitid = table.Column<int>(type: "integer", nullable: true),
                    auditequestid = table.Column<int>(type: "integer", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: true),
                    processstatus = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UNIT_COMMENT", x => x.id);
                    table.ForeignKey(
                        name: "FK_UNIT_COMMENT_AUDIT_FACILITY_counitid",
                        column: x => x.counitid,
                        principalTable: "AUDIT_FACILITY",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UNIT_COMMENT_AUDIT_REQUEST_MONITOR_auditequestid",
                        column: x => x.auditequestid,
                        principalTable: "AUDIT_REQUEST_MONITOR",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UNIT_COMMENT_auditequestid",
                table: "UNIT_COMMENT",
                column: "auditequestid");

            migrationBuilder.CreateIndex(
                name: "IX_UNIT_COMMENT_counitid",
                table: "UNIT_COMMENT",
                column: "counitid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UNIT_COMMENT");
        }
    }
}
