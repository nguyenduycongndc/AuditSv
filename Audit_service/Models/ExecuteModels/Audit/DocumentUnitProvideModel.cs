using Audit_service.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class DocumentUnitProvideModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("year")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? year { get; set; }

        [JsonPropertyName("auditworkid")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? auditworkid { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("unitid")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? unitid { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        //en-wrong
        [JsonPropertyName("expridate")]
        public string expridate { get; set; }

        [JsonPropertyName("providedate")]
        public string providedate { get; set; }

        [JsonPropertyName("path")]
        public string path { get; set; }

        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

        [JsonPropertyName("status")]
        public bool? status { get; set; }

        [JsonPropertyName("providestatus")]
        public string? providestatus { get; set; }

        [JsonPropertyName("createdate")]
        public DateTime? createDate { get; set; }

        [JsonPropertyName("modifiedat")]
        public DateTime? modifiedAt { get; set; }

        [JsonPropertyName("deletedat")]
        public DateTime? deletedAt { get; set; }

        [JsonPropertyName("createdby")]
        public int? createdBy { get; set; }

        [JsonPropertyName("modifiedby")]
        public int? modifiedBy { get; set; }

        [JsonPropertyName("deletedby")]
        public int? deletedBy { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? isDeleted { get; set; }

        [JsonPropertyName("unitname")]
        public string unitname { get; set; }

        [JsonPropertyName("auditname")]
        public string auditname { get; set; }

        [JsonPropertyName("auditcode")]
        public string auditcode { get; set; }

        [JsonPropertyName("filename")]
        public string filename { get; set; }

        public bool Valid { get; set; }

        [JsonConverter(typeof(FormatString))]
        public string Note { get; set; }

        [JsonPropertyName("list_file")]
        public List<DocumentUnitProvideFileModel> ListFile { get; set; }
    }
    public class DocumentUnitProvideSearchModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("year")]
        public string year { get; set; }

        [JsonPropertyName("unitid")]
        public string unitid { get; set; }

        [JsonPropertyName("auditworkid")]
        public string auditworkid { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }

        [JsonPropertyName("providestatus")]
        public string providestatus { get; set; }

        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }

    public class DocumentUnitProvideFileModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("path")]
        public string path { get; set; }
        [JsonPropertyName("file_type")]
        public string file_type { get; set; }
    }
}
