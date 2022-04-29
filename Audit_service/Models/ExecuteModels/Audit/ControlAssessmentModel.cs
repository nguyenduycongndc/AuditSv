using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Audit_service.Models.ExecuteModels
{
    public class ControlAssessmentModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        //đây là cột quy định đánh giá kiểm soát thuộc giấy tờ nào
        [JsonPropertyName("workingpaperid")]
        public int workingpaperid { get; set; }
        [JsonPropertyName("workingpapercode")]
        public string workingpapercode { get; set; }

        //đây Đây là cột quy định đánh giá thuộc rủi ro nào
        [JsonPropertyName("riskid")]
        public int riskid { get; set; }

        //đây Đây là cột quy định đánh giá thuộc kiểm soát nào
        [JsonPropertyName("controlid")]
        public int controlid { get; set; }

        [JsonPropertyName("designassessment")]
        public string designassessment { get; set; }

        [JsonPropertyName("designconclusion")]
        public string designconclusion { get; set; }

        [JsonPropertyName("effectiveassessment")]
        public string effectiveassessment { get; set; }

        [JsonPropertyName("effectiveconclusion")]
        public string effectiveconclusion { get; set; }

        [JsonPropertyName("image1")]
        public IFormFileCollection image1 { get; set; }

        [JsonPropertyName("image1name")]
        public List<ControlAssessmentFileModal> image1name { get; set; }

        [JsonPropertyName("image2")]
        public IFormFileCollection image2 { get; set; }

        [JsonPropertyName("image2name")]
        public List<ControlAssessmentFileModal> image2name { get; set; }
        [JsonPropertyName("listprocedures")]

        public List<ProceduesAssessmentModel> listprocedures { get; set; }
        [JsonPropertyName("sampleconclusion")]
        public string sampleConclusion { get; set; }
    }

    public class ControlAssessmentExportModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        //đây là cột quy định đánh giá kiểm soát thuộc giấy tờ nào
        [JsonPropertyName("workingpaperid")]
        public int workingpaperid { get; set; }
        [JsonPropertyName("workingpapercode")]
        public string workingpapercode { get; set; }

        //đây Đây là cột quy định đánh giá thuộc rủi ro nào
        [JsonPropertyName("riskid")]
        public int riskid { get; set; }

        //đây Đây là cột quy định đánh giá thuộc kiểm soát nào
        [JsonPropertyName("controlid")]
        public int controlid { get; set; }

        [JsonPropertyName("designassessment")]
        public string designassessment { get; set; }

        [JsonPropertyName("designconclusion")]
        public string designconclusion { get; set; }

        [JsonPropertyName("effectiveassessment")]
        public string effectiveassessment { get; set; }

        [JsonPropertyName("effectiveconclusion")]
        public string effectiveconclusion { get; set; }
        
        [JsonPropertyName("listprocedures")]
        public List<ProceduesAssessmentModel> listprocedures { get; set; }
        [JsonPropertyName("sampleconclusion")]
        public string sampleConclusion { get; set; }
    }


    public class ControlAssessmentFileModal
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
