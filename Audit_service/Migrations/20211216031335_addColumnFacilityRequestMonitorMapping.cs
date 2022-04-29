using Microsoft.EntityFrameworkCore.Migrations;

namespace Audit_service.Migrations
{
    public partial class addColumnFacilityRequestMonitorMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "FACILITY_REQUEST_MONITOR_MAPPING",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "process_status",
                table: "FACILITY_REQUEST_MONITOR_MAPPING",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comment",
                table: "FACILITY_REQUEST_MONITOR_MAPPING");

            migrationBuilder.DropColumn(
                name: "process_status",
                table: "FACILITY_REQUEST_MONITOR_MAPPING");
        }
    }
}
