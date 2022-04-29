using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels.Audit
{
    [Table("AUDIT_STRATEGY_RISK")]
    public class AuditStrategyRisk
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("auditwork_scope_id")]
        public int auditwork_scope_id { get; set; }

        [ForeignKey("auditwork_scope_id")]
        public AuditWorkScopeFacility AuditWorkScopeFacility { get; set; }

        [JsonPropertyName("name_risk")]
        public string name_risk { get; set; }//tên rủi ro

        [JsonPropertyName("description")]
        public string description { get; set; }//mô tả

        [JsonPropertyName("risk_level")]
        public int risk_level { get; set; }//Mức độ rủi ro

        [JsonPropertyName("audit_strategy")]
        public string audit_strategy { get; set; }// chiến lược KT

        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime? Modified_at { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? deleted_at { get; set; }

        [JsonPropertyName("created_by")]
        public int? created_by { get; set; }

        [JsonPropertyName("modified_by")]
        public int? modified_by { get; set; }
        [JsonPropertyName("deleted_by")]
        public int? deleted_by { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? is_deleted { get; set; }

        [JsonPropertyName("is_active")]
        public bool? is_active { get; set; }
    }
}
