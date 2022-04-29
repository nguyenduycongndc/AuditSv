using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("CONTROL_ASSESSMENT_FILE")]
    public class ControlAssessmentFile
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("control_assessment_id")]
        public Guid control_assessment_id { get; set; }
        [ForeignKey("control_assessment_id")]
        public ControlAssessment ControlAssessment { get; set; }
        [Column("path")]
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [Column("file_type")]
        [JsonPropertyName("file_type")]
        public string file_type { get; set; }
        [Column("isdelete")]
        [JsonPropertyName("isdelete")]
        public bool? isdelete { get; set; }
        [Column("type")]
        [JsonPropertyName("type")]
        public int? type { get; set; }
    }
}
