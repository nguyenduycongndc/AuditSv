using Audit_service.DataAccess;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class ReportAuditWorkSearchModel
    {
        [JsonPropertyName("year")]
        public string Year { get; set; }
        [JsonPropertyName("status")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Status { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("person_in_charge")]
        //[JsonConverter(typeof(IntNullableJsonConverter))]
        public string PersonInCharge { get; set; }

        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }

    public class ReportAuditWorkListModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("year")]
        public string Year { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("str_person_in_charge")]
        public string str_person_in_charge { get; set; } // người phụ trách

        [JsonPropertyName("status")]
        public int? Status { get; set; }
    }
    public class ReportAuditWorkCreateModel
    {
        [JsonPropertyName("auditWork_id")]
        public int? AuditWorkId { get; set; }
        [JsonPropertyName("status")]
        public int? Status { get; set; }
        [JsonPropertyName("year")]
        public string Year { get; set; }
    }
    public class ReportAuditWorkModifyModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }
        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }
        [JsonPropertyName("start_date_field")]
        public string StartDateField { get; set; }
        [JsonPropertyName("end_date_field")]
        public string EndDateField { get; set; }
        [JsonPropertyName("report_date")]
        public string ReportDate { get; set; }
        [JsonPropertyName("num_of_workdays")]
        public int? NumOfWorkdays { get; set; }
        [JsonPropertyName("audit_rating_level_total")]
        public int? AuditRatingLevelTotal { get; set; }

        [JsonPropertyName("base_rating_total")]
        public string BaseRatingTotal { get; set; }

        [JsonPropertyName("general_conclusions")]
        public string GeneralConclusions { get; set; }
        [JsonPropertyName("list_facility_scope")]
        public List<AuditWorkFacilityItemModel> LisFacilityScope { get; set; }
    }
    public class ReportAuditWorkDetailModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("year")]
        public string Year { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("person_in_charge")]
        public int? person_in_charge { get; set; } // người phụ trách

        [JsonPropertyName("str_person_in_charge")]
        public string str_person_in_charge { get; set; } // người phụ trách

        [JsonPropertyName("classify")]
        public int? Classify { get; set; }

        [JsonPropertyName("start_date_planning")]
        public DateTime? StartDatePlaning { get; set; }

        [JsonPropertyName("end_date_planning")]
        public DateTime? EndDatePlaning { get; set; }

        [JsonPropertyName("start_date_field")]
        public DateTime? StartDateField { get; set; }

        [JsonPropertyName("end_date_field")]
        public DateTime? EndDateField { get; set; }

        [JsonPropertyName("report_date")]
        public DateTime? ReportDate { get; set; }

        [JsonPropertyName("num_of_workdays")]
        public int? NumOfWorkdays { get; set; }

        //Status : Trạng thái . có 5 trạng thái là 1 bản nháp , 2 chờ duyệt , 3 đã duyệt , 4 từ chối duyệt , 5 ngưng sử dụng
        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("num_of_auditor")]
        public int? NumOfAuditor { get; set; } // số luowg KTV

        [JsonPropertyName("str_auditor_name")]
        public string StrAuditorName { get; set; }

        [JsonPropertyName("audit_work_scope_list")]
        public List<AuditWorkScopeModel> AuditWorkScopeList { get; set; }

        [JsonPropertyName("out_of_audit_scope")]
        public string OutOfAuditScope { get; set; }

        [JsonPropertyName("rating_level_audit")]
        public int? RatingLevelAudit { get; set; }

        [JsonPropertyName("rating_base_audit")]
        public string RatingBaseAudit { get; set; }

        [JsonPropertyName("general_conclusions")]
        public string GeneralConclusions { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("audit_detect_list")]
        public List<AuditDetectInfoModel> AuditDetectList { get; set; }

        [JsonPropertyName("audit_work_facility_list")]
        public List<AuditWorkFacilityModel> AuditWorkFacilityList { get; set; }

        [JsonPropertyName("sumary_facility_list")]
        public List<SumaryDetectModel> SumaryFacilityList { get; set; }

    }

    public class AuditWorkScopeModel
    {
        [JsonPropertyName("id")]
        public int? ID { get; set; }
        [JsonPropertyName("audit_process_name")]
        public string AuditProcess { get; set; }
        [JsonPropertyName("audit_facility_name")]
        public string AuditFacility { get; set; }
        [JsonPropertyName("audit_activity_name")]
        public string AuditActivity { get; set; }
    }
    public class AuditDetectInfoModel
    {
        [JsonPropertyName("id")]
        public int? ID { get; set; }
        [JsonPropertyName("code")]
        public string code { get; set; }
        [JsonPropertyName("title")]
        public string title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }
        [JsonPropertyName("rating_risk")]
        public int? rating_risk { get; set; }
    }
    public class AuditWorkFacilityModel
    {
        [JsonPropertyName("id")]
        public int? ID { get; set; }
        [JsonPropertyName("audit_facility_id")]
        public int? AuditFacilityId { get; set; }
        [JsonPropertyName("audit_facility_name")]
        public string AuditFacility { get; set; }
        [JsonPropertyName("audit_rating_level_item")]
        public int? AuditRatingLevelItem { get; set; }

        [JsonPropertyName("base_rating_item")]
        public string BaseRatingItem { get; set; }
    }

    public class AuditWorkFacilityItemModel
    {
        [JsonPropertyName("scopeid")]
        public int? ScopeId { get; set; }
        [JsonPropertyName("audit_rating_level_item")]
        public int? AuditRatingLevelItem { get; set; }
        [JsonPropertyName("base_rating_item")]
        public string BaseRatingItem { get; set; }
    }
    public class SumaryDetectModel
    {
        [JsonPropertyName("audit_facility_id")]
        public int? AuditFacilityId { get; set; }
        [JsonPropertyName("audit_facility_name")]
        public string AuditFacility { get; set; }
        [JsonPropertyName("hight")]
        public int? Hight { get; set; }
        [JsonPropertyName("middle")]
        public int? Middle { get; set; }
        [JsonPropertyName("low")]
        public int? Low { get; set; }
    }
}