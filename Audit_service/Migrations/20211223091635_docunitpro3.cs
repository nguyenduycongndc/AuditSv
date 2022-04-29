using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class docunitpro3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentUnitProvide",
                table: "DocumentUnitProvide");

            migrationBuilder.RenameTable(
                name: "DocumentUnitProvide",
                newName: "DOCUMENT_UNIT_PROVIDE");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DOCUMENT_UNIT_PROVIDE",
                table: "DOCUMENT_UNIT_PROVIDE",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DOCUMENT_UNIT_PROVIDE",
                table: "DOCUMENT_UNIT_PROVIDE");

            migrationBuilder.RenameTable(
                name: "DOCUMENT_UNIT_PROVIDE",
                newName: "DocumentUnitProvide");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentUnitProvide",
                table: "DocumentUnitProvide",
                column: "id");
        }
    }
}
