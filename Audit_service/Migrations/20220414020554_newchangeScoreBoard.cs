using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Audit_service.Migrations
{
    public partial class newchangeScoreBoard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RATING_SCALE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserCreate = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    DomainId = table.Column<int>(type: "integer", nullable: false),
                    RiskLevel = table.Column<int>(type: "integer", nullable: false),
                    RiskLevelName = table.Column<string>(type: "text", nullable: true),
                    Min = table.Column<double>(type: "double precision", nullable: true),
                    Max = table.Column<double>(type: "double precision", nullable: true),
                    MinFunction = table.Column<string>(type: "text", nullable: true),
                    MaxFunction = table.Column<string>(type: "text", nullable: true),
                    ApplyFor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RATING_SCALE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SCORE_BOARD",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserCreate = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    DomainId = table.Column<int>(type: "integer", nullable: false),
                    AssessmentStageId = table.Column<int>(type: "integer", nullable: false),
                    ObjectName = table.Column<string>(type: "text", nullable: true),
                    ObjectId = table.Column<int>(type: "integer", nullable: false),
                    ApplyFor = table.Column<string>(type: "text", nullable: true),
                    ObjectCode = table.Column<string>(type: "text", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    StageValue = table.Column<int>(type: "integer", nullable: true),
                    CurrentStatus = table.Column<int>(type: "integer", nullable: false),
                    StateInfo = table.Column<string>(type: "text", nullable: true),
                    Point = table.Column<double>(type: "double precision", nullable: true),
                    RiskLevel = table.Column<string>(type: "text", nullable: true),
                    RatingScaleId = table.Column<int>(type: "integer", nullable: true),
                    AuditCycleName = table.Column<string>(type: "text", nullable: true),
                    Target = table.Column<string>(type: "text", nullable: true),
                    MainProcess = table.Column<string>(type: "text", nullable: true),
                    ITSystem = table.Column<string>(type: "text", nullable: true),
                    Project = table.Column<string>(type: "text", nullable: true),
                    Outsourcing = table.Column<string>(type: "text", nullable: true),
                    Customer = table.Column<string>(type: "text", nullable: true),
                    Supplier = table.Column<string>(type: "text", nullable: true),
                    InternalRegulations = table.Column<string>(type: "text", nullable: true),
                    LawRegulations = table.Column<string>(type: "text", nullable: true),
                    AttachFile = table.Column<string>(type: "text", nullable: true),
                    AttachName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCORE_BOARD", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ASSESSMENT_RESULT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserCreate = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    DomainId = table.Column<int>(type: "integer", nullable: false),
                    ScoreBoardId = table.Column<int>(type: "integer", nullable: false),
                    StageStatus = table.Column<int>(type: "integer", nullable: false),
                    RiskLevel = table.Column<int>(type: "integer", nullable: false),
                    RiskLevelChange = table.Column<int>(type: "integer", nullable: false),
                    RiskLevelChangeName = table.Column<string>(type: "text", nullable: true),
                    Audit = table.Column<bool>(type: "boolean", nullable: false),
                    AuditReason = table.Column<int>(type: "integer", nullable: true),
                    PassAuditReason = table.Column<string>(type: "text", nullable: true),
                    AuditDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastAudit = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastRiskLevel = table.Column<string>(type: "text", nullable: true),
                    AssessmentStatus = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASSESSMENT_RESULT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ASSESSMENT_RESULT_SCORE_BOARD_ScoreBoardId",
                        column: x => x.ScoreBoardId,
                        principalTable: "SCORE_BOARD",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ASSESSMENT_RESULT_ScoreBoardId",
                table: "ASSESSMENT_RESULT",
                column: "ScoreBoardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ASSESSMENT_RESULT");

            migrationBuilder.DropTable(
                name: "RATING_SCALE");

            migrationBuilder.DropTable(
                name: "SCORE_BOARD");
        }
    }
}
