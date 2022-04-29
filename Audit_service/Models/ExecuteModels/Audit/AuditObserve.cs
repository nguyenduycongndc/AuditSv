using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class AuditObserveSearchModel
    {
        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("code")]
        public string code { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("working_paper_code")]
        public string working_paper_code { get; set; }
        [JsonPropertyName("discoverer")]
        public int? discoverer { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
    public class AuditObserveModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("code")]
        public string code { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("auditwork_name")]
        public string auditwork_name { get; set; }
        [JsonPropertyName("working_paper_code")]
        public string working_paper_code { get; set; }
        [JsonPropertyName("discoverer_name")]
        public string discoverer_name { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
        [JsonPropertyName("note")]
        public string note { get; set; }
        [JsonPropertyName("type")]
        public int type { get; set; }
        [JsonPropertyName("controlid")]
        public int controlid { get; set; }
        [JsonPropertyName("evidence")]
        public IFormFileCollection evidence { get; set; }
        [JsonPropertyName("evidencename")]
        public List<AuditObserveFileModal> evidencename { get; set; }
        [JsonPropertyName("workingpaperid")]
        public int workingpaperid { get; set; }
        [JsonPropertyName("riskid")]
        public int riskid { get; set; }
    }

    public class AuditObserveFileModal
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
