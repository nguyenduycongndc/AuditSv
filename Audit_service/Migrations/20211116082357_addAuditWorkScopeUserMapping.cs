using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class addAuditWorkScopeUserMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "audit_scope_outside",
                table: "AUDIT_WORK",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AUDIT_WORK_SCOPE_USER_MAPPING ",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(type: "integer", nullable: false),
                    auditwork_scope_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    users_id = table.Column<int>(type: "integer", nullable: true),
                    full_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_WORK_SCOPE_USER_MAPPING ", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUDIT_WORK_SCOPE_USER_MAPPING _AUDIT_WORK_SCOPE_auditwork_s~",
                        column: x => x.auditwork_scope_id,
                        principalTable: "AUDIT_WORK_SCOPE",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AUDIT_WORK_SCOPE_USER_MAPPING _USERS_users_id",
                        column: x => x.users_id,
                        principalTable: "USERS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_SCOPE_USER_MAPPING _auditwork_scope_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ",
                column: "auditwork_scope_id");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_WORK_SCOPE_USER_MAPPING _users_id",
                table: "AUDIT_WORK_SCOPE_USER_MAPPING ",
                column: "users_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_WORK_SCOPE_USER_MAPPING ");

            migrationBuilder.DropColumn(
                name: "audit_scope_outside",
                table: "AUDIT_WORK");

        }
    }
}
