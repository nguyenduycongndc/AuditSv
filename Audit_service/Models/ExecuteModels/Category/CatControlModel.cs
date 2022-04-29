using Audit_service.DataAccess;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace Audit_service.Models.ExecuteModels
{
    public class CatControlModel
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
        public int? Status { get; set; }
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

        [JsonPropertyName("activename")]
        public string activename { get; set; }

        [JsonPropertyName("processname")]
        public string processname { get; set; }

        [JsonPropertyName("listrisk")]
        public List<ListRisk> ListRisk { get; set; }

        [JsonPropertyName("listdocument")]
        public List<ListDocument> ListDocument { get; set; }

        [JsonPropertyName("actualcontrol")]
        public string ActualControl { get; set; }

        [JsonPropertyName("controlfrequency")]
        public string controlfrequency { get; set; }

        [JsonPropertyName("controltype")]
        public string controltype { get; set; }

        [JsonPropertyName("controlformat")]
        public string controlformat { get; set; }

        [JsonPropertyName("statusname")]
        public string statusname { get; set; }
    }

    public class ListRisk
    {
        [JsonPropertyName("risk_id")]
        public string riskid { get; set; }
    }

    public class ListDocument
    {
        [JsonPropertyName("document_id")]
        public Guid document_id { get; set; }
    }

    public class CatControlSearchModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("unitid")]
        public string Unitid { get; set; }
        [JsonPropertyName("activationid")]
        public string Activationid { get; set; }
        [JsonPropertyName("processid")]
        public string Processid { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }

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

        [JsonPropertyName("actualcontrol")]
        public string ActualControl { get; set; }

        [JsonPropertyName("controltype")]
        public int? Controltype { get; set; }

        [JsonPropertyName("controlfrequency")]
        public int? Controlfrequency { get; set; }
        [JsonPropertyName("controlformat")]
        public int? Controlformat { get; set; }

        [JsonPropertyName("controlassassment")]
        public List<ControlAssessmentModel> controlassassment { get; set; }

        [JsonPropertyName("riskcontrolid")]
        public int? Riskcontrolid { get; set; }
        [JsonPropertyName("riskcode")]
        public string RiskCode { get; set; }
    }

    public class CatControlModifiModel
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
    }
    public class CatControlCreateModel
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
    }

    public class CatControlDetailModel
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
        [JsonPropertyName("actualcontrol")]
        public string ActualControl { get; set; }

        [JsonPropertyName("controltype")]
        public int? Controltype { get; set; }

        [JsonPropertyName("controlfrequency")]
        public int? Controlfrequency { get; set; }

        [JsonPropertyName("controlformat")]
        public int? Controlformat { get; set; }

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
    }

    public class CatControlModifyModel
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
        [JsonPropertyName("actualcontrol")]
        public string ActualControl { get; set; }

        [JsonPropertyName("controltype")]
        public int? Controltype { get; set; }

        [JsonPropertyName("controlfrequency")]
        public int? Controlfrequency { get; set; }
        [JsonPropertyName("controlformat")]
        public int? Controlformat { get; set; }

        [JsonPropertyName("listriskedit")]
        public List<ListRiskEdit> ListRiskEdit { get; set; }

        [JsonPropertyName("listdocumentedit")]
        public List<ListDocumentEdit> ListDocumentEdit { get; set; }

        [JsonPropertyName("editby")]
        public int? Editby { get; set; }

        [JsonPropertyName("editdate")]
        public DateTime? Editdate { get; set; }
    }

    public class ListRiskEdit
    {
        [JsonPropertyName("risk_id")]
        public string riskid { get; set; }
    }

    public class ListDocumentEdit
    {
        [JsonPropertyName("document_id")]
        public Guid document_id { get; set; }
    }
}