using Audit_service.Models.MigrationsModels.Audit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_WORK_SCOPE_FACILITY")]
    public class AuditWorkScopeFacility
    {
        public AuditWorkScopeFacility()
        {
            this.AuditStrategyRisk = new HashSet<AuditStrategyRisk>();
            this.AuditWorkScopeFacilityFile = new HashSet<AuditWorkScopeFacilityFile>();
        }
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }

        [ForeignKey("auditwork_id")]
        public virtual AuditWork AuditWork { get; set; }

        [Column("year")]
        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [Column("auditfacilities_id")]
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }

        [Column("auditfacilities_name")]
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }

        [Column("reason")]
        [JsonPropertyName("reason")]
        public string ReasonNote { get; set; }

        [Column("risk_rating")]
        [JsonPropertyName("risk_rating")]
        public int? RiskRating { get; set; }

        [Column("risk_rating_name")]
        [JsonPropertyName("risk_rating_name")]
        public string RiskRatingName { get; set; }

        [Column("auditing_time_nearest")]
        [JsonPropertyName("auditing_time_nearest")]
        public DateTime? AuditingTimeNearest { get; set; }

        [Column("audit_rating_level_report")]
        [JsonPropertyName("audit_rating_level_report")]
        public int? AuditRatingLevelReport { get; set; }

        [Column("base_rating_report")]
        [JsonPropertyName("base_rating_report")]
        public string BaseRatingReport { get; set; }

        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [Column("deleted_at")]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("deleted_by")]
        [JsonPropertyName("deleted_by")]
        public int? DeletedBy { get; set; }

        [Column("score_board_id")]
        [JsonPropertyName("score_board_id")]
        public int? BoardId { get; set; }

        [JsonPropertyName("path")]
        public string path { get; set; }// file

        [Column("file_type")]
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }

        [Column("brief_review")]
        [JsonPropertyName("brief_review")]
        public string brief_review { get; set; }//Đánh giá sơ bộ

        public virtual ICollection<AuditStrategyRisk> AuditStrategyRisk { get; set; }
        public virtual ICollection<AuditWorkScopeFacilityFile> AuditWorkScopeFacilityFile { get; set; }
    }
}
