using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_OBSERVE_FILE")]
    public class AuditObserveFile
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("observe_id")]
        public int observe_id { get; set; }
        [ForeignKey("observe_id")]
        public AuditObserve AuditObserve { get; set; }
        [Column("path")]
        [JsonPropertyName("path")]
        public string path { get; set; }
        [Column("file_type")]
        [JsonPropertyName("file_type")]
        public string file_type { get; set; }
        [Column("isdelete")]
        [JsonPropertyName("isdelete")]
        public bool? isdelete { get; set; }
    }
}
