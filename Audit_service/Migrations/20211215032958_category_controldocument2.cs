using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class category_controldocument2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CONTROL_DOCUMENT",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    controlid = table.Column<int>(type: "integer", nullable: false),
                    documentid = table.Column<Guid>(type: "uuid", nullable: false),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTROL_DOCUMENT", x => x.id);
                    table.ForeignKey(
                        name: "FK_CONTROL_DOCUMENT_CAT_CONTROL_controlid",
                        column: x => x.controlid,
                        principalTable: "CAT_CONTROL",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CONTROL_DOCUMENT_controlid",
                table: "CONTROL_DOCUMENT",
                column: "controlid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONTROL_DOCUMENT");
        }
    }
}
