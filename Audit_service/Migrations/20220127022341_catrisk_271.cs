using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class catrisk_271 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BUSSINESS_ACTIVITY");

            migrationBuilder.AddColumn<int>(
                name: "risktype",
                table: "CAT_RISK",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "risktype",
                table: "CAT_RISK");

            migrationBuilder.CreateTable(
                name: "BUSSINESS_ACTIVITY",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    UserCreate = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BUSSINESS_ACTIVITY", x => x.ID);
                });
        }
    }
}
