using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Audit_service.Models.MigrationsModels
{
    [Table("EVALUATION_SCALE")]
    public class EvaluationScale
    {
        public EvaluationScale()
        {

        }

        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        //point : điểm đánh giá
        [Column("point")]
        [JsonPropertyName("point")]
        public double? point { get; set; }

        //description : mô tả thang điểm đánh giá
        [Column("description")]
        [JsonPropertyName("description")]
        public string description { get; set; }

        //status : trạng thái thang điểm đánh giá 
        [Column("status")]
        [JsonPropertyName("status")]
        public int? status { get; set; }

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

    }
}
