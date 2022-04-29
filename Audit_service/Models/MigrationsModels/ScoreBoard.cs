using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("SCORE_BOARD")]
    public class ScoreBoard
    {
        public ScoreBoard()
        {
            this.AssessmentResult = new HashSet<AssessmentResult>();
        }
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

        public int AssessmentStageId { get; set; }
        public string ObjectName { get; set; }
        public int ObjectId { get; set; }
        public string ApplyFor { get; set; }
        public string ObjectCode { get; set; }

        //for assessment stage
        public int Year { get; set; }
        public int Stage { get; set; }
        public int? StageValue { get; set; }
        public int CurrentStatus { get; set; }

        //general info
        public string StateInfo { get; set; }
        public double? Point { get; set; }
        public string RiskLevel { get; set; }
        public int? RatingScaleId { get; set; }
        public string AuditCycleName { get; set; }

        //object info
        public string Target { get; set; }
        public string MainProcess { get; set; }
        public string ITSystem { get; set; }
        public string Project { get; set; }
        public string Outsourcing { get; set; }
        public string Customer { get; set; }
        public string Supplier { get; set; }
        public string InternalRegulations { get; set; }
        public string LawRegulations { get; set; }
        public string AttachFile { get; set; }
        public string AttachName { get; set; }
        public virtual ICollection<AssessmentResult> AssessmentResult { get; set; }
    }
}
