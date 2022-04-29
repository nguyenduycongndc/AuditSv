using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Audit_service.Models.MigrationsModels
{
    [Table("EVALUATION_COMPLIANCE")]
    public class EvaluationCompliance
    {
        public EvaluationCompliance()
        {

        }

        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        //year : năm kiểm toán
        [Column("year")]
        [JsonPropertyName("year")]
        public int? year { get; set; }

        //audit_id : cuộc kiểm toán
        [Column("audit_id")]
        [JsonPropertyName("audit_id")]
        public int? audit_id { get; set; }

        [Column("stage")]
        [JsonPropertyName("stage")]
        public int? stage { get; set; }

        [Column("evaluation_standard_id")]
        [JsonPropertyName("evaluation_standard_id")]
        public int? evaluation_standard_id { get; set; }

        //evaluation_standard_code : số hiệu
        [Column("evaluation_standard_code")]
        [JsonPropertyName("evaluation_standard_code")]
        public string evaluation_standard_code { get; set; }

        //evaluation_standard_title : Tiêu đề chuẩn mực
        [Column("evaluation_standard_title")]
        [JsonPropertyName("evaluation_standard_title")]
        public string evaluation_standard_title { get; set; }

        //evaluation_standard_request : Yêu cầu chuẩn mực
        [Column("evaluation_standard_request")]
        [JsonPropertyName("evaluation_standard_request")]
        public string evaluation_standard_request { get; set; }

        //compliance : Tuân thủ
        [Column("compliance")]
        [JsonPropertyName("compliance")]
        public bool compliance { get; set; }

        //reason : Lý do không tuân thủ
        [Column("reason")]
        [JsonPropertyName("reason")]
        public string reason { get; set; }

        //plan : Kế hoạch khắc phục
        [Column("plan")]
        [JsonPropertyName("plan")]
        public string plan { get; set; }

        //time : Thời gian cam kết hoàn thành
        [Column("time")]
        [JsonPropertyName("time")]
        public DateTime? time { get; set; }

        //reponsible : Người chịu trách nhiệm
        [Column("reponsible")]
        [JsonPropertyName("reponsible")]
        public int? reponsible { get; set; }

        [Column("isdelete")]
        [JsonPropertyName("isdelete")]
        public bool? isdelete { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? created_at { get; set; }

        [Column("created_by")]
        [JsonPropertyName("created_by")]
        public int? created_by { get; set; }

        [Column("modified_at")]
        [JsonPropertyName("modified_at")]
        public DateTime? modified_at { get; set; }

        [Column("modified_by")]
        [JsonPropertyName("modified_by")]
        public int? modified_by { get; set; }

        [Column("deleted_at")]
        [JsonPropertyName("deleted_at")]
        public DateTime? deleted_at { get; set; }

        [Column("deleted_by")]
        [JsonPropertyName("deleted_by")]
        public int? deleted_by { get; set; }

        [ForeignKey("evaluation_standard_id")]
        public EvaluationStandard EvaluationStandard { get; set; }

        [ForeignKey("reponsible")]
        public Users Users { get; set; }
    }
}
