using Audit_service.Models.MigrationsModels.Audit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_PROGRAM")]
    public class AuditProgram
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

        [Column("auditprocess_id")]
        [JsonPropertyName("auditprocess_id")]
        public int? auditprocess_id { get; set; }

        [Column("year")]
        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [Column("auditprocess_name")]
        [JsonPropertyName("auditprocess_name")]
        public string auditprocess_name { get; set; }

        [Column("bussinessactivities_id")]
        [JsonPropertyName("bussinessactivities_id")]
        public int? bussinessactivities_id { get; set; }

        [Column("bussinessactivities_name")]
        [JsonPropertyName("bussinessactivities_name")]
        public string bussinessactivities_name { get; set; }

        [Column("auditfacilities_id")]
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }

        [Column("auditfacilities_name")]
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }

        [Column("status")]
        //Status : Trạng thái . có 5 trạng thái là 1 bản nháp , 2 chờ duyệt , 3 đã duyệt , 4 từ chối duyệt , 5 ngưng sử dụng
        [JsonPropertyName("status")]
        public int? Status { get; set; }

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
    }
}
