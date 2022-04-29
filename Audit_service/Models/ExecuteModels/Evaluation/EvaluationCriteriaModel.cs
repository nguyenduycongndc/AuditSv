using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Evaluation
{
    public class EvaluationCriteriaSearchModel
    {

        [JsonPropertyName("status")]
        public int status { get; set; }
        [JsonPropertyName("key")]
        public string key { get; set; }
        [JsonPropertyName("start_number")]
        public int? StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int? PageSize { get; set; }
    }
    public class EvaluationCriteriaModel
    {

        [JsonPropertyName("id")]
        public int id { get; set; }

        //code : Mã tiêu chí
        [JsonPropertyName("code")]
        public string code { get; set; }

        //name : Tên tiêu chí
        [JsonPropertyName("name")]
        public string name { get; set; }

        //description : mô tả tiêu chí
        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("status")]
        public int? status { get; set; }

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

        //parent_id : Tiêu chí cha
        [JsonPropertyName("parent_id")]
        public int? parent_id { get; set; }

        [JsonPropertyName("parent")]
        public EvaluationCriteriaModel Parent { get; set; }

        public List<EvaluationCriteriaModel> Childs { get; set; } = new List<EvaluationCriteriaModel>();

        [JsonPropertyName("ancestor")]
        public string ancestor { get; set; }

    }
}
