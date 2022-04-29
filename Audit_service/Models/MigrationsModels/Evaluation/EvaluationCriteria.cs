using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Audit_service.Models.MigrationsModels
{
    [Table("EVALUATION_CRITERIA")]
    public class EvaluationCriteria
    {
        public EvaluationCriteria()
        {

        }

        //id : phân biệt
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        //code : Mã tiêu chí
        [Column("code")]
        [JsonPropertyName("code")]
        public string code { get; set; }

        //name : Tên tiêu chí
        [Column("name")]
        [JsonPropertyName("name")]
        public string name { get; set; }

        //description : mô tả tiêu chí
        [Column("description")]
        [JsonPropertyName("description")]
        public string description { get; set; }

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

        //parent_id : Tiêu chí cha
        [Column("parent_id")]
        [JsonPropertyName("parent_id")]
        public int? parent_id { get; set; }

    }
}
