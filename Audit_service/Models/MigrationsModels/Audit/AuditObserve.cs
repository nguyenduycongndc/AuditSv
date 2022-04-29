using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_OBSERVE")]
    public class AuditObserve
    {
        public AuditObserve()
        {
            this.AuditObserveFile = new HashSet<AuditObserveFile>();
        }
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [Column("code")]
        [JsonPropertyName("code")]
        public string code { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string name { get; set; }

        [Column("discoverer")]
        [JsonPropertyName("discoverer")]
        public int discoverer { get; set; }//người phát hiện

        [ForeignKey("discoverer")]
        public virtual Users Users { get; set; }

        [Column("year")]
        [JsonPropertyName("year")]
        public int? year { get; set; }

        [Column("auditwork_id")]
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }//id của cuộc kiểm toán ở trạng thái "Đã duyệt" thuộc năm đã chọn

        [Column("auditwork_name")]
        [JsonPropertyName("auditwork_name")]
        public string auditwork_name { get; set; }

        [ForeignKey("auditwork_id")]
        public virtual AuditWork AuditWork { get; set; }

        [Column("audit_detect_id")]
        [JsonPropertyName("audit_detect_id")]
        public int? audit_detect_id { get; set; }//id của phát hiện KT

        [ForeignKey("audit_detect_id")]
        public virtual AuditDetect AuditDetect { get; set; }

        [Column("working_paper_id")]
        [JsonPropertyName("working_paper_id")]
        public int? working_paper_id { get; set; }

        [Column("working_paper_code")]
        [JsonPropertyName("working_paper_code")]
        public string working_paper_code { get; set; }

        [ForeignKey("working_paper_id")]
        public virtual WorkingPaper WorkingPaper { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("created_by")]
        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

        [Column("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [Column("select_state")]
        [JsonPropertyName("select_state")]
        public bool select_state { get; set; }

        [Column("note")]
        [JsonPropertyName("note")]
        public string note { get; set; }

        [Column("type")]
        [JsonPropertyName("type")]
        public int? type { get; set; }

        [Column("control_id")]
        [JsonPropertyName("control_id")]
        public int? control_id { get; set; }

        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [Column("risk_id")]
        [JsonPropertyName("risk_id")]
        public int? RiskId { get; set; }

        public virtual ICollection<AuditObserveFile> AuditObserveFile { get; set; }

    }
}
