using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("PROCEDURES_ASSESSMENT")]
    public class ProceduresAssessment
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }


        [Column("control_assessment_id")]
        [JsonPropertyName("control_assessment_id")]
        public Guid? control_assessment_id { get; set; }

        [Column("procedures_id")]
        [JsonPropertyName("procedures_id")]
        public int? procedures_id { get; set; }

        [Column("result")]
        [JsonPropertyName("result")]
        public string result { get; set; }

        [ForeignKey("control_assessment_id")]
        public virtual ControlAssessment ControlAssessment { get; set; }
    }
}
