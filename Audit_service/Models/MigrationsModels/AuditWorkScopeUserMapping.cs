using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_WORK_SCOPE_USER_MAPPING ")]
    public class AuditWorkScopeUserMapping
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("type")]
        public int type { get; set; }//1: Trưởng nhóm kiểm toán, 2: Kiểm toán viên

        [JsonPropertyName("auditwork_scope_id")]
        public int auditwork_scope_id { get; set; }

        [ForeignKey("auditwork_scope_id")]
        public AuditWorkScope AuditWorkScope { get; set; }

        [JsonPropertyName("user_id")]
        public int user_id { get; set; }

        [ForeignKey("user_id")]
        public Users Users { get; set; }

        [JsonPropertyName("full_name")]
        public string full_name { get; set; }
    }
}
