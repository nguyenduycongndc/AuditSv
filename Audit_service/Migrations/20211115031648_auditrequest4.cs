using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class auditrequest4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_REQUEST");

            migrationBuilder.CreateTable(
                name: "AUDIT_REQUEST_MONITOR",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    auditrequesttypeid = table.Column<int>(type: "integer", nullable: true),
                    unitid = table.Column<int>(type: "integer", nullable: false),
                    completeat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    actualcompleteat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    userid = table.Column<int>(type: "integer", nullable: true),
                    cooperateunitid = table.Column<int>(type: "integer", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    timestatus = table.Column<int>(type: "integer", nullable: false),
                    processstatus = table.Column<int>(type: "integer", nullable: true),
                    conclusion = table.Column<int>(type: "integer", nullable: true),
                    detectid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_REQUEST_MONITOR", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.CreateTable(
                name: "AUDIT_REQUEST",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    actualcompleteat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    auditrequesttypeid = table.Column<int>(type: "integer", nullable: true),
                    code = table.Column<string>(type: "text", nullable: true),
                    completeat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    conclusion = table.Column<int>(type: "integer", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    cooperateunitid = table.Column<int>(type: "integer", nullable: true),
                    processstatus = table.Column<int>(type: "integer", nullable: true),
                    timestatus = table.Column<int>(type: "integer", nullable: false),
                    unitid = table.Column<int>(type: "integer", nullable: false),
                    detectid = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_REQUEST", x => x.id);
                });
        }
    }
}
