using Audit_service.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class ConfigDocumentModel
    {
        [JsonPropertyName("item_id")]
        public int? item_id { get; set; }

        [JsonPropertyName("item_name")]
        public string item_name { get; set; }

        [JsonPropertyName("item_code")]
        public string item_code { get; set; }
    }
    public class ListConfigDocumentModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }

        [JsonPropertyName("item_id")]
        public int? item_id { get; set; }

        [JsonPropertyName("item_name")]
        public string item_name { get; set; }

        [JsonPropertyName("item_code")]
        public string item_code { get; set; }

        [JsonPropertyName("content")]
        public string content { get; set; }

        [JsonPropertyName("status")]
        public bool? status { get; set; }

        [JsonPropertyName("is_show")]
        public bool? isShow { get; set; }
    }
    public class ConfigDocumentActiveModel
    {
        public int id { get; set; }
        public int status { get; set; }
    }
}
