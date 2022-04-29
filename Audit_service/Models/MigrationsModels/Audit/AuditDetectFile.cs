using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_DETECT_FILE")]
    public class AuditDetectFile
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("auditdetect_id")]
        public int auditdetect_id { get; set; }
        [ForeignKey("auditdetect_id")]
        public AuditDetect AuditDetect { get; set; }
        [Column("path")]
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [Column("file_type")]
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
        [Column("isdelete")]
        [JsonPropertyName("isdelete")]
        public bool? IsDelete { get; set; }
    }
}
