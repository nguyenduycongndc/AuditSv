using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Audit_service.DataAccess;

namespace Audit_service.Models.ExecuteModels
{
    public class DocumentModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("unitid")]
        public int? unit_id { get; set; }

        [JsonPropertyName("status")]
        public bool? status { get; set; }

        [JsonPropertyName("public_date")]
        public string publicdate { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? isdeleted { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("unitname")]
        public string unitname { get; set; }
        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

        [JsonPropertyName("controldocumentid")]
        public Guid controldocumentid { get; set; }
    }

    public class DocumentSearchModel
    {
        [JsonPropertyName("code")]
        public string code { get; set; }

        [JsonPropertyName("unitid")]
        public string unit_id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("controlid")]
        public string controlid { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
}
