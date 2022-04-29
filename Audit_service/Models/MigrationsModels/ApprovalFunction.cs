using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("APPROVAL_FUNCTION_STATUS")]
    public class ApprovalFunction
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [Column("item_id")]
        [JsonPropertyName("item_id")]
        public int? item_id { get; set; } // id chức năng đang xử lý

        [Column("function_name")]
        [JsonPropertyName("function_name")]
        public string function_name { get; set; }

        [Column("function_code")]
        [JsonPropertyName("function_code")]
        public string function_code { get; set; } // mã của chức năng lấy theo menu

        [JsonPropertyName("approver")]
        public int? approver { get; set; } // người duyệt

        [ForeignKey("approver")]
        public Users Users { get; set; }

        [JsonPropertyName("approver_last")]
        public int? approver_last { get; set; } // người duyệt

        [ForeignKey("approver_last")]
        public Users Users_Last { get; set; }

        [Column("status_code")]
        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }

        [Column("status_name")]
        [JsonPropertyName("status_name")]
        public string StatusName { get; set; }

        [Column("reason")]
        [JsonPropertyName("reason")]
        public string Reason { get; set; } // lý do từ chối duyệt

        [Column("approval_date")]
        [JsonPropertyName("approval_date")]
        public DateTime? ApprovalDate { get; set; }

        [Column("path")]
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [Column("file_type")]
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
    }
}
