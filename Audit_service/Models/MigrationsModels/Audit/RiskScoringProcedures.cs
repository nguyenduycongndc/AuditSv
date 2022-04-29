using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("RISK_SCORING_PROCEDURES")]
    public class RiskScoringProcedures
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("risk_scoring_id")]
        public int? risk_scoring_id { get; set; }

        [ForeignKey("risk_scoring_id")]
        public virtual ProcessLevelRiskScoring ProcessLevelRiskScoring { get; set; }

        [JsonPropertyName("catprocedures_id")]
        public int? catprocedures_id { get; set; }

        [ForeignKey("catprocedures_id")]
        public virtual CatAuditProcedures CatAuditProcedures { get; set; }

        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual Users Users { get; set; }

        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("created_by")]
        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

        [Column("deleted_at")]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("deleted_by")]
        [JsonPropertyName("deleted_by")]
        public int? DeletedBy { get; set; }
    }
}
