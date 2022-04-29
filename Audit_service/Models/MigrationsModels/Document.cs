using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Audit_service.Models.MigrationsModels
{
    [Table("DOCUMENT")]
    public class Document
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public Guid id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("code")]
        public string code { get; set; }

        [JsonPropertyName("public_date")]
        public DateTime publicdate { get; set; }

        [JsonPropertyName("unit_id")]
        public int? unit_id { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("status")]
        public bool? status { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? isdeleted { get; set; }

        [JsonPropertyName("createdat")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("path")]
        public string path { get; set; }
        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

    }
}
