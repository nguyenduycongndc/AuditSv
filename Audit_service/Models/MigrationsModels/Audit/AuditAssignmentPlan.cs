using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_ASSIGNMENT_PLAN")]
    public class AuditAssignmentPlan
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }

        [ForeignKey("auditwork_id")]
        public virtual AuditWorkPlan AuditWorkPlan { get; set; }

        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual Users Users { get; set; }

        [Column("start_date")]
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("is_active")]
        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("created_by")]
        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

        [Column("modified_at")]
        [JsonPropertyName("modified_at")]
        public DateTime? ModifiedAt { get; set; }

        [Column("modified_by")]
        [JsonPropertyName("modified_by")]
        public int? ModifiedBy { get; set; }

        [Column("deleted_at")]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("deleted_by")]
        [JsonPropertyName("deleted_by")]
        public int? DeletedBy { get; set; }

        [JsonPropertyName("fullName")]
        public string fullName { get; set; }
    }
}
