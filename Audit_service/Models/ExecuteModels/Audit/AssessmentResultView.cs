using Audit_service.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class AssessmentResultView
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("code")]
        [JsonConverter(typeof(FormatString))]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        [JsonConverter(typeof(FormatString))]
        public string Name { get; set; }
        [JsonPropertyName("assessment_object")]
        [JsonConverter(typeof(FormatString))]
        public string ObjectName { get; set; }
        [JsonConverter(typeof(IntJsonConverter))]
        [JsonPropertyName("object_id")]
        public int ObjectId { get; set; }
        [JsonConverter(typeof(FormatString))]
        [JsonPropertyName("object_code")]
        public string ObjectCode { get; set; }
        [JsonPropertyName("apply_for")]
        [JsonConverter(typeof(FormatString))]
        public string ApplyFor { get; set; }

        [JsonPropertyName("apply_for_id")]
        [JsonConverter(typeof(FormatString))]
        public string ApplyForId { get; set; }

        [JsonConverter(typeof(DoubleNullableJsonConverter))]
        [JsonPropertyName("assessment_point")]
        public double? Point { get; set; }
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevel { get; set; }
        [JsonPropertyName("risk_level_change")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int RiskLevelChange { get; set; } = 0;
        [JsonPropertyName("risk_level_name")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevelChangeName { get; set; }
        ///

        [JsonPropertyName("last_audit")]
        public string LastAudit { get; set; }
        [JsonConverter(typeof(FormatString))]
        [JsonPropertyName("last_risklevel")]
        public string LastRiskLevel { get; set; }
        [JsonConverter(typeof(FormatString))]
        [JsonPropertyName("audit_cycle")]
        public string AuditCycle { get; set; }
        [JsonPropertyName("audit")]
        public bool Audit { get; set; } = false;
        [JsonPropertyName("audit_reason")]
        [JsonConverter(typeof(FormatString))]
        public string AuditReason { get; set; }
        [JsonConverter(typeof(IntNullableJsonConverter))]
        [JsonPropertyName("audit_result_id")]
        public int? AuditReasonId { get; set; }
        [JsonPropertyName("pass_audit_reason")]
        [JsonConverter(typeof(FormatString))]
        public string PassAuditReason { get; set; }
        [JsonPropertyName("stage_status")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int StageStatus { get; set; } = 0;
        [JsonPropertyName("assessment_status")]
        public int? AssessmentStatus { get; set; } = 0;
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
