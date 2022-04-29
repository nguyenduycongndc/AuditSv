using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class createConfigDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CONFIG_DOCUMENT",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<int>(type: "integer", nullable: true),
                    item_name = table.Column<string>(type: "text", nullable: true),
                    item_code = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: true),
                    is_show = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONFIG_DOCUMENT", x => x.id);
                    table.ForeignKey(
                        name: "FK_CONFIG_DOCUMENT_MENU_item_id",
                        column: x => x.item_id,
                        principalTable: "MENU",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CONFIG_DOCUMENT_item_id",
                table: "CONFIG_DOCUMENT",
                column: "item_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONFIG_DOCUMENT");
        }
    }
}
