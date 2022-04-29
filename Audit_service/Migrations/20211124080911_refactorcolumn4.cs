using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class refactorcolumn4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIT_REQUEST_MONITOR",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    auditrequesttypeid = table.Column<int>(type: "integer", nullable: true),
                    unitid = table.Column<int>(type: "integer", nullable: true),
                    complete_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    actual_complete_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    userid = table.Column<int>(type: "integer", nullable: true),
                    co_operate_unit_id = table.Column<int>(type: "integer", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    time_status = table.Column<int>(type: "integer", nullable: true),
                    process_status = table.Column<int>(type: "integer", nullable: true),
                    conclusion = table.Column<int>(type: "integer", nullable: true),
                    detectid = table.Column<int>(type: "integer", nullable: true),
                    evidence = table.Column<string>(type: "text", nullable: true),
                    unit_comment = table.Column<string>(type: "text", nullable: true),
                    audit_comment = table.Column<string>(type: "text", nullable: true),
                    captain_comment = table.Column<string>(type: "text", nullable: true),
                    leader_comment = table.Column<string>(type: "text", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_REQUEST_MONITOR", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detectid",
                        column: x => x.detectid,
                        principalTable: "AUDIT_DETECT",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_FACILITY_unitid",
                        column: x => x.unitid,
                        principalTable: "AUDIT_FACILITY",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AUDIT_REQUEST_MONITOR_CAT_AUDIT_REQUEST_auditrequesttypeid",
                        column: x => x.auditrequesttypeid,
                        principalTable: "CAT_AUDIT_REQUEST",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AUDIT_REQUEST_MONITOR_USERS_userid",
                        column: x => x.userid,
                        principalTable: "USERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_auditrequesttypeid",
                table: "AUDIT_REQUEST_MONITOR",
                column: "auditrequesttypeid");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_detectid",
                table: "AUDIT_REQUEST_MONITOR",
                column: "detectid");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_unitid",
                table: "AUDIT_REQUEST_MONITOR",
                column: "unitid");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_REQUEST_MONITOR_userid",
                table: "AUDIT_REQUEST_MONITOR",
                column: "userid");

            migrationBuilder.AddForeignKey(
                name: "FK_UNIT_COMMENT_AUDIT_FACILITY_counitid",
                table: "UNIT_COMMENT",
                column: "counitid",
                principalTable: "AUDIT_FACILITY",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UNIT_COMMENT_AUDIT_REQUEST_MONITOR_auditequestid",
                table: "UNIT_COMMENT",
                column: "auditequestid",
                principalTable: "AUDIT_REQUEST_MONITOR",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UNIT_COMMENT_AUDIT_FACILITY_counitid",
                table: "UNIT_COMMENT");

            migrationBuilder.DropForeignKey(
                name: "FK_UNIT_COMMENT_AUDIT_REQUEST_MONITOR_auditequestid",
                table: "UNIT_COMMENT");

            migrationBuilder.DropTable(
                name: "AUDIT_REQUEST_MONITOR");
        }
    }
}
