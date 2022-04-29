using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Audit_service.Models.ExecuteModels.Evaluation
{
    public class EvaluationSearchModel
    {

        [JsonPropertyName("audit_id")]
        public int? audit_id { get; set; }
        [JsonPropertyName("year")]
        public int? year { get; set; }

        [JsonPropertyName("stage")]
        public int? stage { get; set; }

        [JsonPropertyName("start_number")]
        public int? StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int? PageSize { get; set; }
    }

    public class EvaluationModel
    {

        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("year")]
        public int? year { get; set; }

        [JsonPropertyName("audit_id")]
        public int? audit_id { get; set; }

        [JsonPropertyName("stage")]
        public int? stage { get; set; }

        [JsonPropertyName("audit_code")]
        public string audit_code { get; set; }

        [JsonPropertyName("evaluation_scale_id")]
        public int? evaluation_scale_id { get; set; }
        [JsonPropertyName("evaluation_scale_point")]
        public string evaluation_scale_point { get; set; }

        [JsonPropertyName("plan")]
        public string plan { get; set; }

        [JsonPropertyName("actual")]
        public string actual { get; set; }

        [JsonPropertyName("explain")]
        public string explain { get; set; }


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

        [JsonPropertyName("evaluation_criteria_id")]
        public int? evaluation_criteria_id { get; set; }

        [JsonPropertyName("evaluation_criteria_parent")]
        public int? evaluation_criteria_parent { get; set; }

        [JsonPropertyName("evaluation_criteria_name")]
        public string evaluation_criteria_name { get; set; }

        [JsonPropertyName("has_child")]
        public bool? has_child { get; set; }
        public List<EvaluationModel> Childs { get; set; } = new List<EvaluationModel>();
        public EvaluationModel Parent { get; set; }

    }
}
