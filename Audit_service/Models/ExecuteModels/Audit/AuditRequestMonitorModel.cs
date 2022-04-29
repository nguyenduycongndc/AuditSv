using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class AuditRequestMonitorModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("auditrequesttypeid")]
        public int? AuditRequestTypeId { get; set; }
        [JsonPropertyName("auditrequesttypename")]
        public string auditrequesttypename { get; set; }

        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }

        [JsonPropertyName("completeat")]
        public string CompleteAt { get; set; }

        [JsonPropertyName("actualcompleteat")]
        public string ActualCompleteAt { get; set; }

        [JsonPropertyName("userid")]
        public int? userid { get; set; }

        [JsonPropertyName("cooperateunitid")]
        public int? CooperateUnitid { get; set; }

        [JsonPropertyName("note")]
        public string note { get; set; }

        [JsonPropertyName("timestatus")]
        public int? TimeStatus { get; set; }

        [JsonPropertyName("processstatus")]
        public int? ProcessStatus { get; set; }

        [JsonPropertyName("conclusion")]
        public int? Conclusion { get; set; }

        [JsonPropertyName("detectid")]
        public int? detectid { get; set; }

        [JsonPropertyName("unitname")]
        public string UnitName { get; set; }
        [JsonPropertyName("extendat")]
        public string ExtendAt { get; set; }
        [JsonPropertyName("extendatstring")]
        public string ExtendAtString { get; set; }
    }

    public class AuditRequestMonitorSearchModel
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("conclusion")]
        public string Conclusion { get; set; }
        //[JsonPropertyName("content")]
        //public string Content { get; set; }       

        [JsonPropertyName("unitid")]
        public string Unitid { get; set; }

        [JsonPropertyName("auditid")]
        public string AuditId { get; set; }

        //[JsonPropertyName("completeat")]
        //public string CompleteAt { get; set; }

        //[JsonPropertyName("actualcompleteat")]
        //public string ActualCompleteAt { get; set; }  
        [JsonPropertyName("processstatus")]
        public string ProcessStatus { get; set; }
        [JsonPropertyName("year")]
        public string Year { get; set; }

        [JsonPropertyName("completedatefrom")]
        public string CompleteDateFrom { get; set; }
        [JsonPropertyName("cooperateunitid")]
        public string CooperatateUnitId { get; set; }

        [JsonPropertyName("completedateTo")]
        public string CompleteDateTo { get; set; }

        [JsonPropertyName("completeactualfrom")]
        public string CompleteActualFrom { get; set; }

        [JsonPropertyName("completeactualto")]
        public string CompleteActualTo { get; set; }

        [JsonPropertyName("timestatus")]
        public string TimeStatus { get; set; }

        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }

        [JsonPropertyName("type")]
        public string AuditRequestType { get; set; }

    }

    public class AuditRequestMonitorDetailModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("auditrequesttypeid")]
        public int? AuditRequestTypeId { get; set; }

        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }

        [JsonPropertyName("completeat")]
        public string CompleteAt { get; set; }

        [JsonPropertyName("actualcompleteat")]
        public string ActualCompleteAt { get; set; }

        [JsonPropertyName("userid")]
        public int? userid { get; set; }

        [JsonPropertyName("cooperateunitid")]
        public int? CooperateUnitid { get; set; }

        [JsonPropertyName("note")]
        public string note { get; set; }

        [JsonPropertyName("timestatus")]
        public int? TimeStatus { get; set; }

        [JsonPropertyName("processstatus")]
        public int? ProcessStatus { get; set; }

        [JsonPropertyName("conclusion")]
        public int? Conclusion { get; set; }

        [JsonPropertyName("detectid")]
        public int? detectid { get; set; }

        [JsonPropertyName("unitname")]
        public string UnitName { get; set; }

        [JsonPropertyName("evidence")]
        public string Evidence { get; set; }

        [JsonPropertyName("unitcomment")]
        public string Unitcomment { get; set; }

        [JsonPropertyName("auditcomment")]
        public string Auditcomment { get; set; }

        [JsonPropertyName("captaincomment")]
        public string Captaincomment { get; set; }

        [JsonPropertyName("leadercomment")]
        public string Leadercomment { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("detectcode")]
        public string Detectcode { get; set; }

        [JsonPropertyName("detectdescription")]
        public string DetectDescription { get; set; }

        [JsonPropertyName("username")]
        public string username { get; set; }

        [JsonPropertyName("trackinguser")]
        public string trackinguser { get; set; }

        [JsonPropertyName("auditrequesttypename")]
        public string auditrequesttypename { get; set; }

        [JsonPropertyName("facilityrequestmonitor")]
        public List<FacilityRequestMonitorModel> facilityRequestMonitorModels { get; set; }

        [JsonPropertyName("auditrequestfile")]
        public List<AuditRequestMonitorFileModel> auditrequestfile { get; set; }

        [JsonPropertyName("extendat")]
        public string ExtendAt { get; set; }

        [JsonPropertyName("incomplete_reason")]
        public string incomplete_reason { get; set; }
    }

    public class AuditRequestMonitorModifyModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("auditrequesttypeid")]
        public string AuditRequestTypeId { get; set; }

        [JsonPropertyName("unitid")]
        public string Unitid { get; set; }

        [JsonPropertyName("completeat")]
        public string CompleteAt { get; set; }

        [JsonPropertyName("actualcompleteat")]
        public DateTime? ActualCompleteAt { get; set; }

        [JsonPropertyName("userid")]
        public string userid { get; set; }

        [JsonPropertyName("cooperateunitid")]
        public string CooperateUnitid { get; set; }

        [JsonPropertyName("note")]
        public string note { get; set; }

        [JsonPropertyName("timestatus")]
        public string TimeStatus { get; set; }

        [JsonPropertyName("processstatus")]
        public string ProcessStatus { get; set; }

        [JsonPropertyName("conclusion")]
        public string Conclusion { get; set; }

        [JsonPropertyName("detectid")]
        public string detectid { get; set; }

        [JsonPropertyName("unitname")]
        public string UnitName { get; set; }

        [JsonPropertyName("evidence")]
        public string Evidence { get; set; }

        [JsonPropertyName("unitcomment")]
        public string Unitcomment { get; set; }

        [JsonPropertyName("auditcomment")]
        public string Auditcomment { get; set; }

        [JsonPropertyName("captaincomment")]
        public string Captaincomment { get; set; }

        [JsonPropertyName("leadercomment")]
        public string Leadercomment { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("detectcode")]
        public string Detectcode { get; set; }

        [JsonPropertyName("detectdescription")]
        public string DetectDescription { get; set; }

        [JsonPropertyName("facilityrequestmonitor")]
        public List<FacilityRequestMonitorModel> facilityrequestmonitor { get; set; }

        [JsonPropertyName("auditrequestfile")]
        public IFormFileCollection auditrequestfile { get; set; }
        [JsonPropertyName("extendat")]
        public string ExtendAt { get; set; }

        [JsonPropertyName("incomplete_reason")]
        public string incomplete_reason { get; set; }

    }

    public class AuditRequestMonitorFileModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }

    }

    public class AuditRequestMonitorExportModel
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("conclusion")]
        public string Conclusion { get; set; }
        //[JsonPropertyName("content")]
        //public string Content { get; set; }       

        [JsonPropertyName("unitid")]
        public string Unitid { get; set; }

        //[JsonPropertyName("completeat")]
        //public string CompleteAt { get; set; }

        //[JsonPropertyName("actualcompleteat")]
        //public string ActualCompleteAt { get; set; }  
        [JsonPropertyName("processstatus")]
        public string ProcessStatus { get; set; }

        [JsonPropertyName("completedatefrom")]
        public string CompleteDateFrom { get; set; }

        [JsonPropertyName("completedateTo")]
        public string CompleteDateTo { get; set; }

        [JsonPropertyName("completeactualfrom")]
        public string CompleteActualFrom { get; set; }

        [JsonPropertyName("completeactualto")]
        public string CompleteActualTo { get; set; }

        [JsonPropertyName("timestatus")]
        public string TimeStatus { get; set; }

        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }

        [JsonPropertyName("type")]
        public string AuditRequestType { get; set; }

    }
}