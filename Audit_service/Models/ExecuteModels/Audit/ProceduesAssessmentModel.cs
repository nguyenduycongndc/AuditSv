using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Audit_service.Models.ExecuteModels
{
    public class ProceduesAssessmentModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("control_assessment_id")]
        public Guid? control_assessment_id { get; set; }

        [JsonPropertyName("procedures_id")]
        public int? procedures_id { get; set; }

        [JsonPropertyName("result")]
        public string result { get; set; }

    }
}
