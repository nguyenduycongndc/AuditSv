using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Evaluation
{
    public class EvaluationStandardSearchModel
    {

        [JsonPropertyName("title")]
        public string title { get; set; }
        [JsonPropertyName("request")]
        public string request { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }

    public class EvaluationStandardModel
    {

        [JsonPropertyName("id")]
        public int id { get; set; }

        //code : mã hiệu chuẩn mực KTNB
        [JsonPropertyName("code")]
        public string code { get; set; }

        //title : tiêu đề chuẩn mực KTNB
        [JsonPropertyName("title")]
        public string title { get; set; }

        //request : yêu cầu chuẩn mực KTNB
        [JsonPropertyName("request")]
        public string request { get; set; }

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
    }
}
