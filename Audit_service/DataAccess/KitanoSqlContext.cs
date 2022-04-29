using Audit_service.Models.MigrationsModels;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Models.MigrationsModels.Category;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Audit_service.DataAccess
{
    public class KitanoSqlContext : DbContext
    {
        public KitanoSqlContext()
        {
        }
        public KitanoSqlContext(DbContextOptions<KitanoSqlContext> options) : base(options)
        {
        }

        // TABLE
        //===================================================================
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersGroup> UsersGroup { get; set; }
        public virtual DbSet<UsersGroupMapping> UsersGroupMapping { get; set; }
        public virtual DbSet<AuditFacility> Department { get; set; }
        public virtual DbSet<UnitType> UnitType { get; set; }
        public virtual DbSet<UsersWorkHistory> UsersWorkHistory { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<UsersRoles> UsersRoles { get; set; }
        public virtual DbSet<UsersGroupRoles> UsersGroupRoles { get; set; }
        public virtual DbSet<SystemParameter> SystemParameter { get; set; }
        public virtual DbSet<AuditPlan> AuditPlan { get; set; }
        public virtual DbSet<AuditWork> AuditWork { get; set; }
        public virtual DbSet<AuditAssignment> AuditAssignment { get; set; }
        public virtual DbSet<CatAuditProcedures> CatAuditProcedures { get; set; }
        public virtual DbSet<CatControl> CatControl { get; set; }
        public virtual DbSet<CatRisk> CatRisk { get; set; }

        public virtual DbSet<AuditWorkScope> AuditWorkScope { get; set; }
        public virtual DbSet<ProcessLevelRiskScoring> ProcessLevelRiskScoring { get; set; }
        public virtual DbSet<RiskScoringProcedures> RiskScoringProcedures { get; set; }
        public virtual DbSet<RiskControl> RiskControl { get; set; }
        public virtual DbSet<AuditRequestMonitor> AuditRequest { get; set; }

        public virtual DbSet<AuditDetect> AuditDetect { get; set; }
        public virtual DbSet<AuditObserve> AuditObserve { get; set; }
        public virtual DbSet<ReportAuditWork> ReportAuditWork { get; set; }
        public virtual DbSet<CatAuditRequest> CatAuditRequest { get; set; }
        public virtual DbSet<CatDetectType> CatDetectType { get; set; }
        public virtual DbSet<UnitComment> UnitComment { get; set; }
        public virtual DbSet<ControlAssessment> ControlAssessment { get; set; }

        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<AuditWorkPlan> AuditWorkPlan { get; set; }
        public virtual DbSet<AuditWorkScopePlan> AuditWorkScopePlan { get; set; }
        public virtual DbSet<AuditAssignmentPlan> AuditAssignmentPlan { get; set; }
        public virtual DbSet<WorkingPaper> WorkingPaper { get; set; }
        

        public virtual DbSet<AuditWorkPlanFile> AuditWorkPlanFile { get; set; }

        public virtual DbSet<AuditObserveFile> AuditObserveFile { get; set; }
        public virtual DbSet<ControlAssessmentFile> ControllerAssessmentFile { get; set; }
        public virtual DbSet<ProceduresAssessment> ProceduresAssessment { get; set; }
        public virtual DbSet<AuditRequestMonitorFile> AuditRequestMonitorFile { get; set; }

        public virtual DbSet<ControlDocument> ControlDocument { get; set; }
        public virtual DbSet<DiscussionHistory> DiscussionHistory { get; set; }
        public virtual DbSet<DocumentUnitProvide> DocumentUnitProvide { get; set; }
        public virtual DbSet<DocumentUnitProvideFile> DocumentUnitProvideFile { get; set; }

        public virtual DbSet<EvaluationCriteria> EvaluationCriteria { get; set; }
        public virtual DbSet<EvaluationScale> EvaluationScale { get; set; }
        public virtual DbSet<EvaluationStandard> EvaluationStandard { get; set; }
        public virtual DbSet<Evaluation> Evaluation { get; set; }
        public virtual DbSet<EvaluationCompliance> EvaluationCompliance { get; set; }


        public virtual DbSet<AuditWorkScopePlanFacility> AuditWorkScopePlanFacility { get; set; }
        public virtual DbSet<AuditMinutes> AuditMinutes { get; set; }
        public virtual DbSet<AuditWorkScopeFacility> AuditWorkScopeFacility { get; set; }
        public virtual DbSet<AuditDetectFile> AuditDetectFile { get; set; }
        public virtual DbSet<AuditMinutesFile> AuditMinutesFile { get; set; }
        public virtual DbSet<AuditWorkScopeFacilityFile> AuditWorkScopeFacilityFile { get; set; }
        public virtual DbSet<ApprovalFunction> ApprovalFunction { get; set; }
        public virtual DbSet<ApprovalFunctionFile> ApprovalFunctionFile { get; set; }
        public virtual DbSet<AuditProgram> AuditProgram { get; set; }
        public virtual DbSet<AuditProcess> AuditProcess { get; set; }
        public virtual DbSet<BussinessActivity> BussinessActivity { get; set; }
        public virtual DbSet<MainStage> MainStage { get; set; }
        public virtual DbSet<ReportAuditWorkYear> ReportAuditWorkYear { get; set; }
        public virtual DbSet<ApprovalConfig> ApprovalConfig { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<CatRiskLevel> CatRiskLevel { get; set; }
        public virtual DbSet<SystemCategory> SystemCategory { get; set; }
        public virtual DbSet<ConfigDocument> ConfigDocument { get; set; }
        public virtual DbSet<RatingScale> RatingScale { get; set; }
        public virtual DbSet<ScoreBoard> ScoreBoards { get; set; }
        public virtual DbSet<AssessmentResult> AssessmentResults { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseNpgsql(ConnectionService.connstring);
            optionBuilder.UseLoggerFactory(GetLoggerFactory());       // bật logger
        }
        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }
        private ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                    builder.AddConsole()
                           .AddFilter(DbLoggerCategory.Database.Command.Name,
                                    LogLevel.Information));
            return serviceCollection.BuildServiceProvider()
                    .GetService<ILoggerFactory>();
        }
    }
}
