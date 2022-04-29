using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.MigrationsModels
{
    [Table("APPROVAL_CONFIG")]
    public class ApprovalConfig
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("item_id")]
        public int? item_id { get; set; }

        [ForeignKey("item_id")]
        public Menu Menu { get; set; }

        [Column("item_name")]
        [JsonPropertyName("item_name")]
        public string item_name { get; set; }

        [Column("item_code")]
        [JsonPropertyName("item_code")]
        public string item_code { get; set; }

        [Column("approval_level")]//cấp duyệt
        [JsonPropertyName("approval_level")]
        public int? ApprovalLevel { get; set; }

        [Column("status_code")]
        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }

        [Column("status_description")]
        [JsonPropertyName("status_description")]
        public string StatusDescription { get; set; }

        [Column("status_name")]
        [JsonPropertyName("status_name")]
        public string StatusName { get; set; }

        [Column("is_outside")]
        [JsonPropertyName("is_outside")]
        public bool? IsOutside { get; set; }

        [Column("is_show")]
        [JsonPropertyName("is_show")]
        public bool? IsShow { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; }
    }
}
