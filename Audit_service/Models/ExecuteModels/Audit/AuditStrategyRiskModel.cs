using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels.Audit
{
    public class AuditStrategyRiskModel
    {

        [JsonPropertyName("id")]
        public int id { get; set; }

        //[JsonPropertyName("path")]
        //public string path { get; set; }// file

        [JsonPropertyName("brief_review")]
        public string brief_review { get; set; }//Đánh giá sơ bộ

        [JsonPropertyName("listauditstrategyrisk")]
        public List<ListAuditStrategyRisk> ListAuditStrategyRisk { get; set; }//tab numb5
        [JsonPropertyName("list_file")]
        public List<AuditWorkScopeFacilityFileModel> ListFile { get; set; }
    }
    public class AuditWorkScopeFacilityFileModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
    }

    public class ListAuditStrategyRisk
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }

        [JsonPropertyName("auditwork_scope_id")]
        public int? auditwork_scope_id { get; set; }

        [JsonPropertyName("auditwork_scope_name")]
        public string auditwork_scope_name { get; set; }

        [JsonPropertyName("name_risk")]
        public string name_risk { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("risk_level")]
        public int? risk_level { get; set; }

        [JsonPropertyName("audit_strategy")]
        public string audit_strategy { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? is_deleted { get; set; }

        [JsonPropertyName("is_active")]
        public bool? is_active { get; set; }
    }

    public class AuditStrategyRiskCreateModel
    {
        [JsonPropertyName("auditwork_scope_id")]
        public int auditwork_scope_id { get; set; }
        [JsonPropertyName("name_risk")]
        public string name_risk { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
        [JsonPropertyName("risk_level")]
        public int risk_level { get; set; }
        [JsonPropertyName("audit_strategy")]
        public string audit_strategy { get; set; }
    }
    public class AuditStrategyRiskDetailModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("auditwork_scope_id")]
        public int auditwork_scope_id { get; set; }
        [JsonPropertyName("name_risk")]
        public string name_risk { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
        [JsonPropertyName("risk_level")]
        public int risk_level { get; set; }
        [JsonPropertyName("audit_strategy")]
        public string audit_strategy { get; set; }
    }
    public class AuditStrategyRiskEditModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("auditwork_scope_id")]
        public int auditwork_scope_id { get; set; }
        [JsonPropertyName("name_risk")]
        public string name_risk { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
        [JsonPropertyName("risk_level")]
        public int risk_level { get; set; }
        [JsonPropertyName("audit_strategy")]
        public string audit_strategy { get; set; }
    }
}
