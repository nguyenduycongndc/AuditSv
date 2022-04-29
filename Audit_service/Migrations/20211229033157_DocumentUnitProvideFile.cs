using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class DocumentUnitProvideFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOCUMENT_UNIT_PROVIDE_FILE",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    documentid = table.Column<int>(type: "integer", nullable: false),
                    path = table.Column<string>(type: "text", nullable: true),
                    file_type = table.Column<string>(type: "text", nullable: true),
                    isdelete = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCUMENT_UNIT_PROVIDE_FILE", x => x.id);
                    table.ForeignKey(
                        name: "FK_DOCUMENT_UNIT_PROVIDE_FILE_DOCUMENT_UNIT_PROVIDE_documentid",
                        column: x => x.documentid,
                        principalTable: "DOCUMENT_UNIT_PROVIDE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENT_UNIT_PROVIDE_FILE_documentid",
                table: "DOCUMENT_UNIT_PROVIDE_FILE",
                column: "documentid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOCUMENT_UNIT_PROVIDE_FILE");
        }
    }
}
