using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Audit_service.Models.ExecuteModels.Evaluation
{
    public class EvaluationComplianceSearchModel
    {

        [JsonPropertyName("audit_id")]
        public int? audit_id { get; set; }
        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("stage")]
        public int? stage { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }


    public class EvaluationComplianceModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("year")]
        public int? year { get; set; }

        [JsonPropertyName("audit_id")]
        public int? audit_id { get; set; }

        [JsonPropertyName("stage")]
        public int? stage { get; set; }

        [JsonPropertyName("evaluation_standard_id")]
        public int? evaluation_standard_id { get; set; }

        [JsonPropertyName("evaluation_standard_code")]
        public string evaluation_standard_code { get; set; }

        [JsonPropertyName("evaluation_standard_title")]
        public string evaluation_standard_title { get; set; }

        [JsonPropertyName("evaluation_standard_request")]
        public string evaluation_standard_request { get; set; }

        [JsonPropertyName("compliance")]
        public bool compliance { get; set; }

        [JsonPropertyName("reason")]
        public string reason { get; set; }

        [JsonPropertyName("plan")]
        public string plan { get; set; }

        [JsonPropertyName("time")]
        public string time { get; set; }

        [JsonPropertyName("reponsible")]
        public int? reponsible { get; set; }
        [JsonPropertyName("reponsible_name")]
        public string reponsible_name { get; set; }

        [JsonPropertyName("isdelete")]
        public bool? isdelete { get; set; }

        [JsonPropertyName("created_at")]
        public string created_at { get; set; }

        [JsonPropertyName("created_by")]
        public string created_by { get; set; }

        [JsonPropertyName("modified_at")]
        public string modified_at { get; set; }

        [JsonPropertyName("modified_by")]
        public string modified_by { get; set; }

        [JsonPropertyName("deleted_at")]
        public string deleted_at { get; set; }

        [JsonPropertyName("deleted_by")]
        public string deleted_by { get; set; }

    }
}
