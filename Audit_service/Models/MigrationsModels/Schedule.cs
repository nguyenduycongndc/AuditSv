using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("SCHEDULE")]
    public class Schedule
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("auditwork_id")]
        public int auditwork_id { get; set; }

        [ForeignKey("auditwork_id")]
        public virtual AuditWork AuditWork { get; set; }

        [JsonPropertyName("work")]
        public string work { get; set; }//công việc

        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual Users Users { get; set; }

        [JsonPropertyName("user_name")]
        public string user_name { get; set; }

        [JsonPropertyName("expected_date")]
        public DateTime? expected_date { get; set; }//Ngày dự kiến thực hiện

        [JsonPropertyName("actual_date")]
        public DateTime? actual_date { get; set; }//Ngày thực tế thực hiện

        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime? modified_at { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime deleted_at { get; set; }

        [JsonPropertyName("created_by")]
        public int? created_by { get; set; }

        [JsonPropertyName("modified_by")]
        public int? modified_by { get; set; }

        [JsonPropertyName("deleted_by")]
        public int? deleted_by { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? is_deleted { get; set; }
    }
}
