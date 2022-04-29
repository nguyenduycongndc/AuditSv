using Audit_service.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels
{
    public class CatRiskModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }

        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }

        [JsonPropertyName("processid")]
        public int? Processid { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("note")]
        [JsonConverter(typeof(FormatString))]
        public string Note { get; set; }

        [JsonPropertyName("valid")]
        public bool Valid { get; set; }


        [JsonPropertyName("unitname")]
        public string unitname { get; set; }

        [JsonPropertyName("processname")]
        public string processname { get; set; }

        [JsonPropertyName("activename")]
        public string activename { get; set; }

        [JsonPropertyName("statusname")]
        public string statusname { get; set; }

        [JsonPropertyName("risktypename")]
        public string risktypename { get; set; }

        [JsonPropertyName("risktype")]
        public int? risktype { get; set; }

        [JsonPropertyName("processcode")]
        public string processcode { get; set; }

        [JsonPropertyName("activecode")]
        public string activecode { get; set; }

        [JsonPropertyName("unitcode")]
        public string unitcode { get; set; }
    }

    public class CatRiskSearchModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string code { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("unitid")]
        public string unitid { get; set; }

        [JsonPropertyName("controlid")]
        public string controlid { get; set; }

        [JsonPropertyName("activationid")]
        public string activationid { get; set; }

        [JsonPropertyName("processid")]
        public string processid { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }

        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("riskcontrolid")]
        public int? Riskcontrolid { get; set; }

        [JsonPropertyName("risktype")]
        public int? RiskType { get; set; }

    }

    public class CatRiskModifiModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }

        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }

        [JsonPropertyName("processid")]
        public int? Processid { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }        

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("risktype")]
        public int? RiskType { get; set; }
    }

    public class CatRiskCreateModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }

        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }

        [JsonPropertyName("processid")]
        public int? Processid { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }        

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("risktype")]
        public int? RiskType { get; set; }
    }

    public class CatRiskDetailModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }

        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }

        [JsonPropertyName("processid")]
        public int? Processid { get; set; }

        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("unitname")]
        public string Unitname { get; set; }

        [JsonPropertyName("createdate")]
        public string Createdate { get; set; }

        [JsonPropertyName("createdby")]
        public int? CreatedBy { get; set; }

        [JsonPropertyName("editby")]
        public int? Editby { get; set; }

        [JsonPropertyName("editdate")]
        public string Editdate { get; set; }

        [JsonPropertyName("createname")]
        public string createname { get; set; }

        [JsonPropertyName("editname")]
        public string editname { get; set; }

        [JsonPropertyName("risktype")]
        public int? RiskType { get; set; }
    }

    public class CatRiskModifyModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }

        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }

        [JsonPropertyName("processid")]
        public int? Processid { get; set; }

        [JsonPropertyName("status")]

        public int? Status { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("unitname")]
        public string Unitname { get; set; }

        [JsonPropertyName("risktype")]
        public int? RiskType { get; set; }
    }
}