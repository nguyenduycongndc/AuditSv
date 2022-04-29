using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("REPORT_AUDIT_WORK")]
    public class ReportAuditWork
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }

        [ForeignKey("auditwork_id")]
        public virtual AuditWork AuditWork { get; set; }

        [Column("auditwork_code")]
        [JsonPropertyName("auditwork_code")]
        public string AuditWorkCode { get; set; }

        [Column("auditwork_name")]
        [JsonPropertyName("auditwork_name")]
        public string AuditWorkName { get; set; }

        [Column("year")]
        [JsonPropertyName("year")]
        public string Year { get; set; }

        [Column("status")]
        //Status : Trạng thái . có 5 trạng thái là 1 bản nháp , 2 chờ duyệt , 3 đã duyệt , 4 từ chối duyệt , 5 ngưng sử dụng
        [JsonPropertyName("status")]
        public int? Status { get; set; }
        [Column("start_date")]
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("start_date_field")]
        [JsonPropertyName("start_date_field")]
        public DateTime? StartDateField { get; set; }

        [Column("end_date_field")]
        [JsonPropertyName("end_date_field")]
        public DateTime? EndDateField { get; set; }

        [Column("report_date")]
        [JsonPropertyName("report_date")]
        public DateTime? ReportDate { get; set; }

        [Column("num_of_workdays")]
        [JsonPropertyName("num_of_workdays")]
        public string NumOfWorkdays { get; set; }

        [Column("audit_rating_level_total")]
        [JsonPropertyName("audit_rating_level_total")]
        public int? AuditRatingLevelTotal { get; set; }

        [Column("base_rating_total")]
        [JsonPropertyName("base_rating_total")]
        public string BaseRatingTotal { get; set; }

        [Column("general_conclusions")]
        [JsonPropertyName("general_conclusions")]
        public string GeneralConclusions { get; set; }

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

        [Column("file_type")]
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }

        [JsonPropertyName("approval_user")]
        public int? approval_user { get; set; } // người duyệt

        [ForeignKey("approval_user")]
        public virtual Users Users { get; set; }

        [Column("approval_date")]
        [JsonPropertyName("approval_date")]
        public DateTime? ApprovalDate { get; set; }

        [Column("reason_reject")]
        [JsonPropertyName("reason_reject")]
        public string ReasonReject { get; set; }

    }
}
