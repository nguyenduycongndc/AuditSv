using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_REQUEST_MONITOR_FiLE")]
    public class AuditRequestMonitorFile
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("audit_request_monitor_id")]
        public int audit_request_monitor_id { get; set; }
        [ForeignKey("audit_request_monitor_id")]
        public virtual AuditRequestMonitor AuditRequestMonitor { get; set; }
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
