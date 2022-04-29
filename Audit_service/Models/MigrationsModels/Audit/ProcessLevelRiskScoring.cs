using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("PROCESS_LEVEL_RISK_SCORING")]
    public class ProcessLevelRiskScoring
    {
        public ProcessLevelRiskScoring()
        {
            this.RiskScoringProcedures = new HashSet<RiskScoringProcedures>();
        }
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("auditprogram_id")]
        public int? auditprogram_id { get; set; }

        [ForeignKey("auditprogram_id")]
        public virtual AuditProgram AuditProgram { get; set; }

        [JsonPropertyName("catrisk_id")]
        public int? catrisk_id { get; set; }

        [ForeignKey("catrisk_id")]
        public virtual CatRisk CatRisk { get; set; }

        [Column("potential_possibility")]
        [JsonPropertyName("potential_possibility")]
        public int? Potential_Possibility { get; set; } // Khả năng xảy ra

        [Column("potential_infulence_level")]
        [JsonPropertyName("potential_infulence_level")]
        public int? Potential_InfulenceLevel { get; set; } // Mức độ ảnh hưởng

        [Column("potential_risk_rating")]
        [JsonPropertyName("potential_risk_rating")]
        public int? Potential_RiskRating { get; set; } // Xếp hạng rủi ro

        [Column("potential_risk_rating_name")]
        [JsonPropertyName("potential_risk_rating_name")]
        public string Potential_RiskRating_Name { get; set; } // Xếp hạng rủi ro name

        [Column("potential_reason_rating")]
        [JsonPropertyName("potential_reason_rating")]
        public string Potential_ReasonRating { get; set; } // Lý do

        [Column("audit_proposal")]
        [JsonPropertyName("audit_proposal")]
        public bool? AuditProposal { get; set; } // Đề xuất kiểm toán

        [Column("remaining_possibility")]
        [JsonPropertyName("remaining_possibility")]
        public int? Remaining_Possibility { get; set; } // Khả năng xảy ra

        [Column("remaining_infulence_level")]
        [JsonPropertyName("remaining_infulence_level")]
        public int? Remaining_InfulenceLevel { get; set; } // Mức độ ảnh hưởng

        [Column("remaining_risk_rating")]
        [JsonPropertyName("remaining_risk_rating")]
        public int? Remaining_RiskRating { get; set; } // Xếp hạng rủi ro

        [Column("remaining_risk_rating_name")]
        [JsonPropertyName("remaining_risk_rating_name")]
        public string Remaining_RiskRating_Name { get; set; } // Xếp hạng rủi ro name

        [Column("remaining_reason_rating")]
        [JsonPropertyName("remaining_reason_rating")]
        public string Remaining_ReasonRating { get; set; } // lý do

        [Column("is_active")]
        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("created_by")]
        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

        [Column("modified_at")]
        [JsonPropertyName("modified_at")]
        public DateTime? ModifiedAt { get; set; }

        [Column("modified_by")]
        [JsonPropertyName("modified_by")]
        public int? ModifiedBy { get; set; }

        [Column("deleted_at")]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("deleted_by")]
        [JsonPropertyName("deleted_by")]
        public int? DeletedBy { get; set; }

        public virtual ICollection<RiskScoringProcedures> RiskScoringProcedures { get; set; }
    }
}
