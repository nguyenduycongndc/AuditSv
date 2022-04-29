using KitanoUserService.API.Models.MigrationsModels.Category;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_DETECT")]
    public class AuditDetect
    {
        public AuditDetect()
        {
            this.AuditObserve = new HashSet<AuditObserve>();
            this.AuditRequestMonitor = new HashSet<AuditRequestMonitor>();
            this.AuditDetectFile = new HashSet<AuditDetectFile>();
        }
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [Column("code")]
        [JsonPropertyName("code")]
        public string code { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string name { get; set; }

        [Column("status")]
        [JsonPropertyName("status")]
        public int? status { get; set; }

        [Column("title")]
        [JsonPropertyName("title")]
        public string title { get; set; }//Tiêu đề phát hiện kiểm toán

        [Column("short_title")]
        [JsonPropertyName("short_title")]
        public string short_title { get; set; }//Tiêu đề rút gọn phát hiện kiểm toán

        [Column("description")]
        [JsonPropertyName("description")]
        public string description { get; set; }//mô tả

        [Column("evidence")]
        [JsonPropertyName("evidence")]
        public string evidence { get; set; }//Bằng chứng phất hiện KT

        [Column("path_audit_detect")]
        [JsonPropertyName("path_audit_detect")]
        public string path_audit_detect { get; set; }//File

        [Column("affect")]
        [JsonPropertyName("affect")]
        public string affect { get; set; }//ảnh hưởng

        [Column("rating_risk")]
        [JsonPropertyName("rating_risk")]
        public int? rating_risk { get; set; }//xếp hạng rủi ro

        [Column("admin_framework")]
        [JsonPropertyName("admin_framework")]
        public int? admin_framework { get; set; }//xếp hạng rủi ro

        [Column("cause")]
        [JsonPropertyName("cause")]
        public string cause { get; set; }//Nguyên nhân

        [Column("audit_report")]
        [JsonPropertyName("audit_report")]
        public bool audit_report { get; set; }//Đưa vào báo cáo kiểm toán

        [Column("classify_audit_detect")]
        [JsonPropertyName("classify_audit_detect")]
        public int? classify_audit_detect { get; set; }//Phân loại phát hiện

        [ForeignKey("classify_audit_detect")]
        public virtual CatDetectType CatDetectType { get; set; }

        [Column("summary_audit_detect")]
        [JsonPropertyName("summary_audit_detect")]
        public string summary_audit_detect { get; set; }//Tóm tắt phát hiện

        [Column("followers")]
        [JsonPropertyName("followers")]
        public int? followers { get; set; }//Người theo dõi

        [ForeignKey("followers")]
        public virtual Users Users { get; set; }

        [Column("year")]
        [JsonPropertyName("year")]
        public int? year { get; set; }

        [Column("opinion_audit")]
        [JsonPropertyName("opinion_audit")]
        public bool opinion_audit { get; set; }//Ý kiến của ĐVĐKT

        [Column("reason")]
        [JsonPropertyName("reason")]
        public string reason { get; set; }//lý do

        [Column("auditwork_id")]
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }//id của cuộc kiểm toán ở trạng thái "Đã duyệt" thuộc năm đã chọn

        [Column("auditwork_name")]
        [JsonPropertyName("auditwork_name")]
        public string auditwork_name { get; set; }

        [ForeignKey("auditwork_id")]
        public virtual AuditWork AuditWork { get; set; }

        [Column("auditprocess_id")]
        [JsonPropertyName("auditprocess_id")]
        public int? auditprocess_id { get; set; }//id của quy trình

        [Column("auditprocess_name")]
        [JsonPropertyName("auditprocess_name")]
        public string auditprocess_name { get; set; }

        [ForeignKey("auditprocess_id")]
        public virtual AuditProcess AuditProcess { get; set; }

        [Column("auditfacilities_id")]
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }//id của đơn vị 

        [Column("auditfacilities_name")]
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }

        [ForeignKey("auditfacilities_id")]
        public virtual AuditFacility AuditFacility { get; set; }

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

        [Column("file_type")]
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }

        [Column("flag_user_id")]
        [JsonPropertyName("flag_user_id")]
        public int? flag_user_id { get; set; } //cờ để biết người được phép duyệt

        //[Column("control_assessment_id")]
        //[JsonPropertyName("control_assessment_id")]
        //public Guid ControlAssessmentId { get; set; }

        //[ForeignKey("control_assessment_id")]
        //public virtual ControlAssessment ControlAssessment { get; set; }


        public virtual ICollection<AuditObserve> AuditObserve { get; set; }
        public virtual ICollection<AuditRequestMonitor> AuditRequestMonitor { get; set; }
        public virtual ICollection<AuditDetectFile> AuditDetectFile { get; set; }
    }
}
