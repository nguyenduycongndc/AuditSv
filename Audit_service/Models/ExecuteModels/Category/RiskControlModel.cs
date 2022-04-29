using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels
{
    public class RiskControlModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("riskid")]
        public int RiskId { get; set; }
        
        [JsonPropertyName("controlid")]
        public int ControlId { get; set; }
        [JsonPropertyName("isdeleted")]
        public bool? isdeleted { get; set; }
    }

    public class RiskControlGetRiskModel
    {
        [JsonPropertyName("riskid")]
        public int RiskId { get; set; }

        [JsonPropertyName("controlid")]
        public int ControlId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        public List<RiskControlGetRiskModel> GetRiskModel { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? isdeleted { get; set; }
    }

    public class RiskControlGetControlModel
    {
        [JsonPropertyName("riskid")]
        public int RiskId { get; set; }

        [JsonPropertyName("controlid")]
        public int ControlId { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        public List<RiskControlGetControlModel> GetControlModel { get; set; }
        [JsonPropertyName("isdeleted")]
        public bool? isdeleted { get; set; }
    }
}
