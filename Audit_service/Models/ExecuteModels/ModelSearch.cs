using Audit_service.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels
{
    public class ModelSearch
    {
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("department_id")]
        public string DepartmentId { get; set; }
        [JsonPropertyName("users_type")]
        public string UsersType { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
    public class ActiveClass
    {
        public int id { get; set; }
        public int status { get; set; }
    }
    public class ChangePass
    {
        public int id { get; set; }
        public string password { get; set; }
    }
    public class ChangeWorkplace
    {
        public int id { get; set; }
        public int departmentId { get; set; }
        public string dateofjoining { get; set; }
    }
    public class DeleteAll
    {
        public string listID { get; set; }
    }
    public class AuditProgramModelSearch
    {
        [JsonPropertyName("year")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Year { get; set; }
        [JsonPropertyName("audit_process")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? AuditProcess { get; set; }
        [JsonPropertyName("audit_work")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? AuditWork { get; set; }
        [JsonPropertyName("facility")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Facility { get; set; }
        [JsonPropertyName("status")]
       // [JsonConverter(typeof(IntNullableJsonConverter))]
        public string Status { get; set; }
        [JsonPropertyName("activity")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Activity { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }

    public class AuditProgramRiskModelSearch
    {
        [JsonPropertyName("audit_work_scope_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? AuditWorkScopeId { get; set; }
        [JsonPropertyName("risk_code")]
        public string RiskCode { get; set; }
        [JsonPropertyName("risk_name")]
        public string RiskName { get; set; }
        [JsonPropertyName("audit_proposal")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? AuditProposal { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
}
