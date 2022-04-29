using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("SYSTEM_PARAMETER")]
    public class SystemParameter
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [Column("sub_system")]
        [JsonPropertyName("sub_system")]
        public string Sub_System { get; set; }
        [Column("parameter_name")]
        [JsonPropertyName("parameter_name")]
        public string Parameter_Name { get; set; }
        [Column("value")]
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [Column("note")]
        [JsonPropertyName("note")]
        public string Note { get; set; }
        [Column("modified_at")]
        [JsonPropertyName("modified_at")]
        public DateTime? Modified_At { get; set; }
        [Column("modified_by")]
        [JsonPropertyName("modified_by")]
        public int? Modified_By { get; set; }
        [Column("reset_at")]
        [JsonPropertyName("reset_at")]
        public DateTime? Reset_At { get; set; }
        [Column("default_value")]
        [JsonPropertyName("default_value")]
        public string Default_Value { get; set; }
        [Column("default_note")]
        [JsonPropertyName("default_note")]
        public string Default_Note { get; set; }
    }
}
