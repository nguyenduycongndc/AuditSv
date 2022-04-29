using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class AuditAssignmentSearchModel
    {
        [JsonPropertyName("auditwork_id")]
        public int auditwork_id { get; set; }
    }
    public class AuditAssignmentModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }

        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }
        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime? ModifiedAt { get; set; }

        [JsonPropertyName("modified_by")]
        public int? ModifiedBy { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("deleted_by")]
        public int? DeletedBy { get; set; }
    }
}