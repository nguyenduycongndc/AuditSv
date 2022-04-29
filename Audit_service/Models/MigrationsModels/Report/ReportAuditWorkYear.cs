using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("REPORT_AUDIT_WORK_YEAR")]
    public class ReportAuditWorkYear
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        //year : năm kiểm toán
        [Column("year")]
        [JsonPropertyName("year")]
        public int? year { get; set; }

        //name : Tên báo cáo
        [Column("name")]
        [JsonPropertyName("name")]
        public string name { get; set; }

        //evaluation : Đánh giá tổng quan của KTNB
        [Column("evaluation")]
        [JsonPropertyName("evaluation")]
        public string evaluation { get; set; }

        //concers : Quan ngại chính của KTNB
        [Column("concerns")]
        [JsonPropertyName("concerns")]
        public string concerns { get; set; }

        //reason : Lý do không hoàn thành kế hoạch
        [Column("reason")]
        [JsonPropertyName("reason")]
        public string reason { get; set; }

        //note : Các vấn đề cần lưu ý
        [Column("note")]
        [JsonPropertyName("note")]
        public string note { get; set; }

        //quality : Chất lượng của hoạt động KTNB
        [Column("quality")]
        [JsonPropertyName("quality")]
        public string quality { get; set; }

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

        [Column("overcome")]
        [JsonPropertyName("overcome")]
        public string overcome { get; set; }

    }
}
