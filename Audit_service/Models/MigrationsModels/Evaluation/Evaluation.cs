using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Audit_service.Models.MigrationsModels
{
    [Table("EVALUATION")]
    public class Evaluation
    {
        public Evaluation()
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

        //audit_id : cuộc kiểm toán
        [Column("audit_code")]
        [JsonPropertyName("audit_code")]
        public string audit_code { get; set; }

        //point : điểm đánh giá
        [Column("evaluation_scale_id")]
        [JsonPropertyName("evaluation_scale_id")]
        public int? evaluation_scale_id { get; set; }

        [Column("evaluation_scale_point")]
        [JsonPropertyName("evaluation_scale_point")]
        public double? evaluation_scale_point { get; set; }

        //plan : kế hoạch
        [Column("plan")]
        [JsonPropertyName("plan")]
        public string plan { get; set; }

        //actual : thực tế
        [Column("actual")]
        [JsonPropertyName("actual")]
        public string actual { get; set; }

        //actual : thực tế
        [Column("explain")]
        [JsonPropertyName("explain")]
        public string explain { get; set; }


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

        [Column("evaluation_criteria_id")]
        [JsonPropertyName("evaluation_criteria_id")]
        public int? evaluation_criteria_id { get; set; }

        [Column("evaluation_criteria_parent")]
        [JsonPropertyName("evaluation_criteria_parent")]
        public int? evaluation_criteria_parent { get; set; }

        [Column("evaluation_criteria_name")]
        [JsonPropertyName("evaluation_criteria_name")]
        public string evaluation_criteria_name { get; set; }

        [ForeignKey("evaluation_scale_id")]
        public EvaluationScale EvaluationScale { get; set; }

        [ForeignKey("evaluation_criteria_id")]
        public EvaluationCriteria EvaluationCriteria { get; set; }

    }
}
