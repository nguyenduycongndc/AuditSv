using KitanoUserService.API.Models.MigrationsModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("CONFIG_DOCUMENT")]
    public class ConfigDocument
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("item_id")]
        public int? item_id { get; set; }

        [ForeignKey("item_id")]
        public Menu menu { get; set; }

        [Column("item_name")]
        [JsonPropertyName("item_name")]
        public string item_name { get; set; }

        [Column("item_code")]
        [JsonPropertyName("item_code")]
        public string item_code { get; set; }

        [Column("content")]
        [JsonPropertyName("content")]
        public string content { get; set; }

        [Column("status")]
        [JsonPropertyName("status")]
        public bool? status { get; set; }

        [Column("is_show")]
        [JsonPropertyName("is_show")]
        public bool? isShow { get; set; }
    }
}
