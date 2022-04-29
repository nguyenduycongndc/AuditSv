using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    // Bảng này có tác dụng lưu trữ thông tin về kế hoạch kiểm toán năm
    [Table("AUDIT_PLAN")]
    public class AuditPlan
    {
        public AuditPlan()
        {
            //this.AuditWork = new HashSet<AuditWork>();
            this.AuditWorkPlan = new HashSet<AuditWorkPlan>();
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
        [Column("year")]
        [JsonPropertyName("year")]
        public int Year { get; set; }
        [Column("status")]
        //Status : Trạng thái . có 5 trạng thái là 1 bản nháp , 2 chờ duyệt , 3 đã duyệt , 4 từ chối duyệt , 5 ngưng sử dụng
        [JsonPropertyName("status")]
        public int? Status { get; set; }
        [Column("target")]
        [JsonPropertyName("target")]
        public string Target { get; set; }
        [Column("createdate")]
        [JsonPropertyName("createdate")]
        public DateTime? Createdate { get; set; }
        [Column("browsedate")]
        //Browsedate là ngày phê duyệt , chon ngày khi phê duyệt
        [JsonPropertyName("browsedate")]
        public DateTime? Browsedate { get; set; }
        [Column("userid")]
        [JsonPropertyName("userid")]
        public int? UserId { get; set; }
        [Column("version")]
        [JsonPropertyName("version")]
        public int? Version { get; set; }
        [Column("note")]
        [JsonPropertyName("note")]
        public string Note { get; set; }
        [Column("isdelete")]
        [JsonPropertyName("isdelete")]
        public bool? IsDelete { get; set; }

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

        [Column("reason_reject")]
        [JsonPropertyName("reason_reject")]
        public string ReasonReject { get; set; }

        [Column("is_copy")]
        [JsonPropertyName("is_copy")]
        public bool? IsCopy { get; set; }

        [Column("other_information")]
        [JsonPropertyName("other_information")]
        public string OtherInformation { get; set; }

        //public virtual ICollection<AuditWork> AuditWork { get; set; }
        public virtual ICollection<AuditWorkPlan> AuditWorkPlan { get; set; }
    }       
}