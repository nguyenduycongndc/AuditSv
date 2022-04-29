using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("CONTROL_ASSESSMENT")]
    public class ControlAssessment
    {
        public ControlAssessment()
        {
            this.ControlAssessmentFile = new HashSet<ControlAssessmentFile>();
            this.ProceduresAssessment = new HashSet<ProceduresAssessment>();
        }
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        //đây là cột quy định đánh giá kiểm soát thuộc giấy tờ nào
        [Column("working_paper_id")]
        [JsonPropertyName("workingpaperid")]
        public int? workingpaperid { get; set; }

        //đây Đây là cột quy định đánh giá thuộc rủi ro nào
        [Column("risk_id")]
        [JsonPropertyName("riskid")]
        public int? riskid { get; set; }

        //đây Đây là cột quy định đánh giá thuộc kiểm soát nào
        [Column("control_id")]
        [JsonPropertyName("controlid")]
        public int? controlid { get; set; }

        [Column("design_assessment")]
        [JsonPropertyName("designassessment")]
        public string designassessment { get; set; }

        [Column("design_conclusion")]
        [JsonPropertyName("designconclusion")]
        public string designconclusion { get; set; }

        [Column("effective_assessment")]
        [JsonPropertyName("effectiveassessment")]
        public string effectiveassessment { get; set; }

        [Column("effective_conclusion")]
        [JsonPropertyName("effectiveconclusion")]
        public string effectiveconclusion { get; set; }

        [Column("sample_conclusion")]
        [JsonPropertyName("sampleconclusion")]
        public string sampleconclusion { get; set; }

        [ForeignKey("risk_id")]
        public virtual CatRisk CatRisk { get; set; }

        [ForeignKey("control_id")]
        public virtual CatControl CatControl { get; set; }

        public virtual ICollection<ControlAssessmentFile> ControlAssessmentFile { get; set; }
        public virtual ICollection<ProceduresAssessment> ProceduresAssessment { get; set; }
    }
}
