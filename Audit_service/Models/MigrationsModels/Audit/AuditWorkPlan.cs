using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_WORK_PLAN")]
    public class AuditWorkPlan
    {
        public AuditWorkPlan()
        {
            this.AuditWorkScopePlan = new HashSet<AuditWorkScopePlan>();
            this.AuditAssignmentPlan = new HashSet<AuditAssignmentPlan>();
            this.AuditWorkPlanFile = new HashSet<AuditWorkPlanFile>();
            this.AuditWorkScopePlanFacility = new HashSet<AuditWorkScopePlanFacility>();
        }
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("code")]
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Column("target")]
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [Column("start_date")]
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("num_of_workdays")]
        [JsonPropertyName("num_of_workdays")]
        public int? NumOfWorkdays { get; set; }

        [JsonPropertyName("person_in_charge")]
        public int? person_in_charge { get; set; } // người phụ trách

        [ForeignKey("person_in_charge")]
        public virtual Users Users { get; set; }

        [Column("num_of_auditor")]
        [JsonPropertyName("num_of_auditor")]
        public int? NumOfAuditor { get; set; } // số luowg KTV

        [Column("req_skill_audit")]
        [JsonPropertyName("req_skill_audit")]
        public string ReqSkillForAudit { get; set; } // yêu cầu về kỹ năng kiểm toán

        [Column("req_outsourcing")]
        [JsonPropertyName("req_outsourcing")]
        public string ReqOutsourcing { get; set; } // yêu cầu thuê ngoài

        [Column("req_other")]
        [JsonPropertyName("req_other")]
        public string ReqOther { get; set; } // yêu cầu khác

        [Column("scale_of_audit")]
        [JsonPropertyName("scale_of_audit")]
        public int? ScaleOfAudit { get; set; } // quy mô cuộc kiểm toán

        [Column("status")]
        //Status : Trạng thái . có 5 trạng thái là 1 bản nháp , 2 chờ duyệt , 3 đã duyệt , 4 từ chối duyệt , 5 ngưng sử dụng
        [JsonPropertyName("status")]
        public int? Status { get; set; }
        
        [Column("execution_status")]
        //ExecutionStatus : Trạng thái thực hiện 1 chưa thực hiện , 2 đang thực hiện , 3 hoàn thành
        [JsonPropertyName("execution_status")]
        public int? ExecutionStatus { get; set; }

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
        [Column("path")]
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [Column("classify")]
        [JsonPropertyName("classify")]
        public int? Classify { get; set; }
        [Column("year")]
        [JsonPropertyName("year")]
        public string Year { get; set; }

        [JsonPropertyName("auditplan_id")]
        public int? auditplan_id { get; set; }

        [Column("extension_time")]
        [JsonPropertyName("extension_time")]
        public DateTime? ExtensionTime { get; set; }

        [Column("audit_scope_outside")]
        [JsonPropertyName("audit_scope_outside")]
        public string AuditScopeOutside { get; set; }

        [Column("audit_scope")]
        [JsonPropertyName("audit_scope")]
        public string AuditScope { get; set; }

        [ForeignKey("auditplan_id")]
        public virtual AuditPlan AuditPlan { get; set; }

        public virtual ICollection<AuditWorkScopePlan> AuditWorkScopePlan { get; set; }

        public virtual ICollection<AuditAssignmentPlan> AuditAssignmentPlan { get; set; }
        public virtual ICollection<AuditWorkPlanFile> AuditWorkPlanFile { get; set; }
        public virtual ICollection<AuditWorkScopePlanFacility> AuditWorkScopePlanFacility { get; set; }
    }
}
