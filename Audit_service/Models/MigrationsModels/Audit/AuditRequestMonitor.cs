using Audit_service.Models.MigrationsModels.Audit;
using KitanoUserService.API.Models.MigrationsModels.Category;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_REQUEST_MONITOR")]
    public class AuditRequestMonitor
    {
        public AuditRequestMonitor()
        {
            this.FacilityRequestMonitorMapping = new HashSet<FacilityRequestMonitorMapping>();
            this.AuditRequestMonitorFile = new HashSet<AuditRequestMonitorFile>();
        }
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("code")]
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [Column("content")]
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("auditrequesttypeid")]
        public int? auditrequesttypeid { get; set; }
        [ForeignKey("auditrequesttypeid")]
        public virtual CatAuditRequest CatAuditRequest { get; set; }

        [JsonPropertyName("unitid")]
        public int? unitid { get; set; }
        [ForeignKey("unitid")]
        public virtual AuditFacility AuditFacility { get; set; }

        [Column("complete_at")]
        [JsonPropertyName("completeat")]
        public DateTime? CompleteAt { get; set; }

        [Column("actual_complete_at")]
        [JsonPropertyName("actualcompleteat")]
        public DateTime? ActualCompleteAt { get; set; }

        [JsonPropertyName("userid")]
        public int? userid { get; set; }
        [ForeignKey("userid")]
        public virtual Users Users { get; set; }

        [Column("co_operate_unit_id")]
        [JsonPropertyName("cooperateunitid")]
        public int? CooperateUnitid { get; set; }

        [Column("note")]
        [JsonPropertyName("note")]
        public string note { get; set; }

        [Column("time_status")]
        [JsonPropertyName("timestatus")]
        public int? TimeStatus { get; set; }

        [Column("process_status")]
        [JsonPropertyName("processstatus")]
        public int? ProcessStatus { get; set; }

        [Column("conclusion")]
        [JsonPropertyName("conclusion")]
        public int? Conclusion { get; set; }

        [JsonPropertyName("detectid")]
        public int? detectid { get; set; }
        [ForeignKey("detectid")]
        public virtual AuditDetect AuditDetect { get; set; }

        [Column("evidence")]
        [JsonPropertyName("evidence")]
        public string Evidence { get; set; }

        [Column("unit_comment")]
        [JsonPropertyName("unitcomment")]
        public string Unitcomment { get; set; }

        [Column("audit_comment")]
        [JsonPropertyName("auditcomment")]
        public string Auditcomment { get; set; }

        [Column("captain_comment")]
        [JsonPropertyName("captaincomment")]
        public string Captaincomment { get; set; }

        [Column("leader_comment")]
        [JsonPropertyName("leader_comment")]
        public string Leadercomment { get; set; }

        [Column("reason")]
        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [Column("comment")]
        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [Column("flag")]
        [JsonPropertyName("flag")]
        public bool flag { get; set; }//false là những thằng được thêm mới có audit_detect_id == null / true là những thằng được thêm mới có audit_detect_id

        [Column("is_active")]
        [JsonPropertyName("is_active")]
        public bool? is_active { get; set; }

        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? is_deleted { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; }

        [Column("created_by")]
        [JsonPropertyName("created_by")]
        public int? created_by { get; set; }

        [Column("modified_at")]
        [JsonPropertyName("modified_at")]
        public DateTime? modified_at { get; set; }

        [Column("modified_by")]
        [JsonPropertyName("modified_by")]
        public int? modified_by { get; set; }

        [Column("deleted_at")]
        [JsonPropertyName("deleted_at")]
        public DateTime? deleted_at { get; set; }

        [Column("deleted_by")]
        [JsonPropertyName("deleted_by")]
        public int? deleted_by { get; set; }

        [Column("detect_code_new")]
        [JsonPropertyName("detect_code_new")]
        public string detect_code_new { get; set; }

        [Column("extend_at")]
        [JsonPropertyName("extend_at")]
        public DateTime? extend_at { get; set; }

        [Column("incomplete_reason")]
        [JsonPropertyName("incomplete_reason")]
        public string incomplete_reason { get; set; }

        public virtual ICollection<FacilityRequestMonitorMapping> FacilityRequestMonitorMapping { get; set; }
        public virtual ICollection<AuditRequestMonitorFile> AuditRequestMonitorFile { get; set; }
    }
}
