using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class docunitpro2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentUnitProvide",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    year = table.Column<int>(type: "integer", nullable: false),
                    auditworkid = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    unitid = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    expridate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    providedate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    path = table.Column<string>(type: "text", nullable: true),
                    filetype = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: true),
                    createdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modifiedat = table.Column<string>(type: "text", nullable: true),
                    deletedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    createdby = table.Column<int>(type: "integer", nullable: true),
                    modifiedby = table.Column<int>(type: "integer", nullable: true),
                    deletedby = table.Column<int>(type: "integer", nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentUnitProvide", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentUnitProvide");
        }
    }
}
