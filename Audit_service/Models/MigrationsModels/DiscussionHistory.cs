using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("DISCUSSION_HISTORY")]
    public class DiscussionHistory
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [Column("item_id")]//(Vd: chức năng phát hiện kiểm toán thì item_id là id của bảng Detect)
        [JsonPropertyName("item_id")]
        public int item_id { get; set; }

        [Column("item_name")]//(Vd: chức năng phát hiện kiểm toán thì item_name là Phát hiện kiểm toán)
        [JsonPropertyName("item_name")]
        public string item_name { get; set; }

        [Column("item_code")]//(Vd: chức năng phát hiện kiểm toán thì item_code là PH.2021.0001)
        [JsonPropertyName("item_code")]
        public string item_code { get; set; }

        [Column("item_type")]//Kế hoạch kiểm toán năm(1)-Kế hoạch cuộc kiểm toán (2)-Giấy tờ làm việc(3)-Phát hiện kiểm toán(4)-Báo cáo cuộc kiểm toán (5)-Chương trình kiểm toán (6)
        [JsonPropertyName("item_type")]
        public int item_type { get; set; } 

        [Column("type")]//Gửi duyệt(1)-Từ chối(2)-Đã duyệt (3)
        [JsonPropertyName("type")]
        public string type { get; set; } 

        [Column("content")]// Từ chối duyệt: nhập input,Nếu duyệt: (Default: Đã duyệt),Nếu Gửi duyệt: (Default: Đã gửi duyệt)
        [JsonPropertyName("content")]
        public string content { get; set; } 

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; }

        [Column("version")]
        [JsonPropertyName("version")]
        public string version { get; set; }

        [Column("person_perform")]
        [JsonPropertyName("person_perform")]
        public string person_perform { get; set; }
    }
}