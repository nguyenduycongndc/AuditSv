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
    [Table("AUDIT_MINUTES")]
    public class AuditMinutes
    {
        public AuditMinutes()
        {
            this.AuditMinutesFile = new HashSet<AuditMinutesFile>();
        }
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [Column("year")]
        [JsonPropertyName("year")]
        public int? year { get; set; }

        [ForeignKey("auditwork_id")]
        public virtual AuditWork AuditWork { get; set; }//khóa AuditWork

        [Column("auditwork_id")]
        [JsonPropertyName("auditwork_id")]
        public int auditwork_id { get; set; }//id của cuộc kiểm toán ở trạng thái "Đã duyệt" thuộc năm đã chọn

        [Column("auditwork_name")]
        [JsonPropertyName("auditwork_name")]
        public string auditwork_name { get; set; }

        [Column("auditwork_code")]
        [JsonPropertyName("auditwork_code")]
        public string auditwork_code { get; set; }

        [Column("audit_work_taget")]
        [JsonPropertyName("audit_work_taget")]
        public string audit_work_taget { get; set; }//Mục đích kiểm toán

        [ForeignKey("audit_work_person")]
        public virtual Users Users { get; set; }//Khóa user

        [Column("audit_work_person")] 
        [JsonPropertyName("audit_work_person")]
        public int audit_work_person { get; set; }//Người phụ trách
        
        [Column("audit_work_classify")] 
        [JsonPropertyName("audit_work_classify")]
        public int audit_work_classify { get; set; }//Phân loại :1- theo kế hoạch; 2- đột xuất

        [ForeignKey("auditfacilities_id")]
        public virtual AuditFacility AuditFacility { get; set; }//khóa AuditFacility

        [Column("auditfacilities_id")]
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }//id của đơn vị 

        [Column("auditfacilities_name")]
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }

        [Column("status")] 
        [JsonPropertyName("status")]
        public int? status { get; set; }//1 chưa xác nhận, 2 đã xác nhận

        [Column("path")]
        [JsonPropertyName("path")]
        public string path { get; set; }//File

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

        [Column("rating")]
        [JsonPropertyName("rating")]
        public string rating { get; set; }

        [Column("problem")]
        [JsonPropertyName("problem")]
        public string problem { get; set; }

        [Column("idea")]
        [JsonPropertyName("idea")]
        public string idea { get; set; }

        [Column("other_content")]
        [JsonPropertyName("other_content")]
        public string OtherContent { get; set; }

        [Column("rating_level_total")]
        [JsonPropertyName("rating_level_total")]
        public int? rating_level_total { get; set; }
        [ForeignKey("rating_level_total")]
        public SystemCategory SystemCategory { get; set; }
        public virtual ICollection<AuditMinutesFile> AuditMinutesFile { get; set; }
    }
}
