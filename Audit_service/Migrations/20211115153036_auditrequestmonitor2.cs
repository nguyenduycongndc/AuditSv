using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class auditrequestmonitor2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detectid",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.AlterColumn<int>(
                name: "detectid",
                table: "AUDIT_REQUEST_MONITOR",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "auditcomment",
                table: "AUDIT_REQUEST_MONITOR",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "captaincomment",
                table: "AUDIT_REQUEST_MONITOR",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "AUDIT_REQUEST_MONITOR",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "evidence",
                table: "AUDIT_REQUEST_MONITOR",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "leadercomment",
                table: "AUDIT_REQUEST_MONITOR",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reason",
                table: "AUDIT_REQUEST_MONITOR",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "unitcomment",
                table: "AUDIT_REQUEST_MONITOR",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detectid",
                table: "AUDIT_REQUEST_MONITOR",
                column: "detectid",
                principalTable: "AUDIT_DETECT",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detectid",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "auditcomment",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "captaincomment",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "comment",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "evidence",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "leadercomment",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "reason",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.DropColumn(
                name: "unitcomment",
                table: "AUDIT_REQUEST_MONITOR");

            migrationBuilder.AlterColumn<int>(
                name: "detectid",
                table: "AUDIT_REQUEST_MONITOR",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_REQUEST_MONITOR_AUDIT_DETECT_detectid",
                table: "AUDIT_REQUEST_MONITOR",
                column: "detectid",
                principalTable: "AUDIT_DETECT",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
