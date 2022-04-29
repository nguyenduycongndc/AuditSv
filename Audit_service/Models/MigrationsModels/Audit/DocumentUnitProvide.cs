using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("DOCUMENT_UNIT_PROVIDE")]
    public class DocumentUnitProvide
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string name { get; set; }

        [Column("year")]
        [JsonPropertyName("year")]
        public int year { get; set; }

        [Column("auditworkid")]
        [JsonPropertyName("auditworkid")]
        public int auditworkid { get; set; }

        [Column("description")]
        [JsonPropertyName("description")]
        public string description { get; set; }

        [Column("unitid")]
        [JsonPropertyName("unitid")]
        public int unitid { get; set; }

        [Column("email")]
        [JsonPropertyName("email")]
        public string email { get; set; }

        [Column("expridate")]
        [JsonPropertyName("expridate")]
        public DateTime? expridate { get; set; }

        [Column("providedate")]
        [JsonPropertyName("providedate")]
        public DateTime? providedate { get; set; }

        [Column("path")]
        [JsonPropertyName("path")]
        public string path { get; set; }

        [Column("filetype")]
        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

        [Column("status")]
        [JsonPropertyName("status")]
        public bool? status { get; set; }

        [Column("providestatus")]
        [JsonPropertyName("providestatus")]
        public int? providestatus { get; set; }

        [Column("createdate")]
        [JsonPropertyName("createdate")]
        public DateTime? createDate { get; set; }

        [Column("modifiedat")]
        [JsonPropertyName("modifiedat")]
        public DateTime? modifiedAt { get; set; }

        [Column("deletedat")]
        [JsonPropertyName("deletedat")]
        public DateTime? deletedAt { get; set; }

        [Column("createdby")]
        [JsonPropertyName("createdby")]
        public int? createdBy { get; set; }

        [Column("modifiedby")]
        [JsonPropertyName("modifiedby")]
        public int? modifiedBy { get; set; }

        [Column("deletedby")]
        [JsonPropertyName("deletedby")]
        public int? deletedBy { get; set; }

        [Column("isdeleted")]
        [JsonPropertyName("isdeleted")]
        public bool? isDeleted { get; set; }

        public virtual ICollection<DocumentUnitProvideFile> DocumentUnitProvideFile { get; set; }
    }
}
