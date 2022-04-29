using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_WORK_PLAN_FILE")]
    public class AuditWorkPlanFile
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int auditwork_id { get; set; }
        [ForeignKey("auditwork_id")]
        public AuditWorkPlan AuditWorkPlan { get; set; }
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
