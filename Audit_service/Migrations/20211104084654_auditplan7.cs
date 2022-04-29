using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class auditplan7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CAT_RISK",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "CAT_RISK",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CAT_RISK",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CAT_CONTROL",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "CAT_CONTROL",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CAT_CONTROL",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CAT_AUDIT_PROCEDURES",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "CAT_AUDIT_PROCEDURES",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CAT_AUDIT_PROCEDURES",
                newName: "id");

            migrationBuilder.AddColumn<int>(
                name: "activationid",
                table: "CAT_RISK",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "processid",
                table: "CAT_RISK",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "status",
                table: "CAT_RISK",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "unitid",
                table: "CAT_RISK",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "activationid",
                table: "CAT_CONTROL",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "processid",
                table: "CAT_CONTROL",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "status",
                table: "CAT_CONTROL",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "unitid",
                table: "CAT_CONTROL",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "activationid",
                table: "CAT_AUDIT_PROCEDURES",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "processid",
                table: "CAT_AUDIT_PROCEDURES",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "status",
                table: "CAT_AUDIT_PROCEDURES",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "unitid",
                table: "CAT_AUDIT_PROCEDURES",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activationid",
                table: "CAT_RISK");

            migrationBuilder.DropColumn(
                name: "processid",
                table: "CAT_RISK");

            migrationBuilder.DropColumn(
                name: "status",
                table: "CAT_RISK");

            migrationBuilder.DropColumn(
                name: "unitid",
                table: "CAT_RISK");

            migrationBuilder.DropColumn(
                name: "activationid",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "processid",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "status",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "unitid",
                table: "CAT_CONTROL");

            migrationBuilder.DropColumn(
                name: "activationid",
                table: "CAT_AUDIT_PROCEDURES");

            migrationBuilder.DropColumn(
                name: "processid",
                table: "CAT_AUDIT_PROCEDURES");

            migrationBuilder.DropColumn(
                name: "status",
                table: "CAT_AUDIT_PROCEDURES");

            migrationBuilder.DropColumn(
                name: "unitid",
                table: "CAT_AUDIT_PROCEDURES");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "CAT_RISK",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "CAT_RISK",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CAT_RISK",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "CAT_CONTROL",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "CAT_CONTROL",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CAT_CONTROL",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "CAT_AUDIT_PROCEDURES",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "CAT_AUDIT_PROCEDURES",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CAT_AUDIT_PROCEDURES",
                newName: "ID");
        }
    }
}
