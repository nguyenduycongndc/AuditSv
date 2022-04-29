using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("ASSESSMENT_RESULT")]
    public class AssessmentResult
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(254)]
        public string Code { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public bool Status { get; set; } = true;
        public bool Deleted { get; set; } = false;
        public string Description { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int? UserCreate { get; set; }
        public DateTime? LastModified { get; set; }
        public int? ModifiedBy { get; set; }
        public int DomainId { get; set; }
        [JsonPropertyName("ScoreBoardId")]
        public int ScoreBoardId { get; set; }
        [ForeignKey("ScoreBoardId")]
        public ScoreBoard ScoreBoard { get; set; }
        public int StageStatus { get; set; } = 0;
        public int RiskLevel { get; set; } = 0;
        public int RiskLevelChange { get; set; } = 0;
        public string RiskLevelChangeName { get; set; }
        public bool Audit { get; set; } = false;
        public int? AuditReason { get; set; }
        public string PassAuditReason { get; set; }
        public DateTime? AuditDate { get; set; }
        public DateTime? LastAudit { get; set; }
        public string LastRiskLevel { get; set; }
        public int? AssessmentStatus { get; set; }
    }
}
