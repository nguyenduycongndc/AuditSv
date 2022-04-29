using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("APPROVAL_FUNCTION_FILE")]
    public class ApprovalFunctionFile
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
        
        [Column("path")]
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [Column("file_type")]
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [Column("deleted_by")]
        [JsonPropertyName("deleted_by")]
        public int? DeletedBy { get; set; }

        [Column("deleted_at")]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
}
