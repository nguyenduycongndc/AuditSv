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
    public class AuditPlanModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("year")]
        public int Year { get; set; }
        //Status : Trạng thái . có 3 trạng thái là 1 bản nháp , 2 chờ duyệt , 3 đã duyệt , 4 từ chối duyệt , 5 ngưng sử dụng
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("target")]
        public string Target { get; set; }
        [JsonPropertyName("createdate")]
        public DateTime? Createdate { get; set; }
        //Browsedate là ngày phê duyệt , chon ngày khi phê duyệt
        [JsonPropertyName("browsedate")]
        public DateTime? Browsedate { get; set; }
        [JsonPropertyName("userid")]
        public int? UserId { get; set; }
        [JsonPropertyName("version")]
        public int? Version { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }
        [JsonPropertyName("statustext")]
        public string Statustext { get; set; }
        [JsonPropertyName("auditworklist")]
        public List<AuditWorkListModel> AuditWorkList { get; set; }
        [JsonPropertyName("is_copy")]
        public bool? IsCopy { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("approval_user")]
        public string approval_user { get; set; } // người duyệt

        [JsonPropertyName("list_file")]
        public List<AuditWorkPlanFileModel> ListFile { get; set; }

        [JsonPropertyName("other_info")]
        public string OtherInfo { get; set; }
    }
    public class ApprovalModel
    {
        [JsonPropertyName("auditplanid")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? auditplanid { get; set; }
        [JsonPropertyName("approvaluser")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? approvaluser { get; set; }
    }
    public class RejectApprovalModel
    {
        [JsonPropertyName("auditplanid")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? id { get; set; }
        [JsonPropertyName("reason_note")]
        public string reason_note { get; set; }
    }
    public class AuditPlanSearchModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("year")]
        public string Year { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }

    public class AuditPlanListModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("year")]
        public int? Year { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("version")]
        public int? Version { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }
        [JsonPropertyName("statustext")]
        public string Statustext { get; set; }
        [JsonPropertyName("approval_user")]
        public int? ApprovalUser { get; set; } // người duyệt

        [JsonPropertyName("approval_user_last")]
        public int? ApprovalUserLast { get; set; } // người duyệt
        [JsonPropertyName("sub_activities")]
        public List<AuditPlanListModel> SubActivities { get; set; }
    }
    public class AuditPlanCreateModel
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("status")]
        public bool? Status { get; set; }
        [JsonPropertyName("year")]
        public int? Year { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }
    }
    public class AuditPlanActiveModel
    {
        public int id { get; set; }
        public int status { get; set; }
    }
    public class AuditPlanModifyModel
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("year")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Year { get; set; }
        [JsonPropertyName("target")]
        public string Target { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }
        [JsonPropertyName("status")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Status { get; set; }
        [JsonPropertyName("browsedate")]
        public DateTime? Browsedate { get; set; }

        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }
        [JsonPropertyName("other_info")]
        public string OtherInfo { get; set; }
    }
    public class AuditPlanCopyModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("year")]
        public int Year { get; set; }
        [JsonPropertyName("target")]
        public string Target { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }

    }
    public class AuditWorkDetailModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date_planning")]//Ngày kết thúc lập KH
        public string EndDatePlanning { get; set; }

        [JsonPropertyName("start_date_real")]//Ngày bắt đầu thực địa
        public string StartDateReal { get; set; }

        [JsonPropertyName("release_date")]//Ngày phát hành BC
        public string ReleaseDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("num_of_workdays")]
        public float? NumOfWorkdays { get; set; }

        [JsonPropertyName("person_in_charge")]
        public int? person_in_charge { get; set; } // người phụ trách
        [JsonPropertyName("str_person_in_charge")]
        public string str_person_in_charge { get; set; } // người phụ trách

        [JsonPropertyName("num_of_auditor")]
        public int? NumOfAuditor { get; set; } // số luowg KTV

        [JsonPropertyName("req_skill_audit")]
        public string ReqSkillForAudit { get; set; } // yêu cầu về kỹ năng kiểm toán

        [JsonPropertyName("req_outsourcing")]
        public string ReqOutsourcing { get; set; } // yêu cầu thuê ngoài

        [JsonPropertyName("req_other")]
        public string ReqOther { get; set; } // yêu cầu khác

        [JsonPropertyName("scale_of_audit")]
        public int? ScaleOfAudit { get; set; } // quy mô cuộc kiểm toán

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

        [JsonPropertyName("modified_at")]
        public string ModifiedAt { get; set; }

        [JsonPropertyName("modified_by")]
        public int? ModifiedBy { get; set; }

        [JsonPropertyName("deleted_at")]
        public string DeletedAt { get; set; }

        [JsonPropertyName("deleted_by")]
        public int? DeletedBy { get; set; }
        [JsonPropertyName("classify")]
        public int? Classify { get; set; }

        [JsonPropertyName("year")]
        public string Year { get; set; }

        //Status : Trạng thái . có 5 trạng thái là 1 bản nháp , 2 chờ duyệt , 3 đã duyệt , 4 từ chối duyệt , 5 ngưng sử dụng
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("statusName")]
        public string statusName { get; set; }

        [JsonPropertyName("auditplan_id")]
        public int? AuditplanId { get; set; }

        [JsonPropertyName("audit_scope_outside")]
        public string AuditScopeOutside { get; set; }

        [JsonPropertyName("execution_status_str")]
        public string ExecutionStatusStr { get; set; }


        [JsonPropertyName("from_date")]
        public string from_date { get; set; }

        [JsonPropertyName("to_date")]
        public string to_date { get; set; }

        [JsonPropertyName("listauditpersonnel")]
        public List<ListAuditPersonnelModel> ListAuditPersonnel { get; set; }//tab num2
        [JsonPropertyName("listauditworkscope")]
        public List<ListAuditWorkScope> ListAuditWorkScope { get; set; }//tab numb3 quy trình
        [JsonPropertyName("listauditfacility")]
        public List<ListAuditFacility> ListAuditFacility { get; set; }//tab numb3 đơn vị
        [JsonPropertyName("listschedule")]
        public List<ListSchedule> ListSchedule { get; set; }//tab numb4

        [JsonPropertyName("list_approval_function_file")]
        public List<ListApprovalFunctionFileModel> ListApprovalFunctionFile { get; set; }


        [JsonPropertyName("audit_scope")]
        public string AuditScopeNew { get; set; }
    }

    public class ListApprovalFunctionFileModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
    }
    public class AuditWorkPlanDetailModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("num_of_workdays")]
        public int? NumOfWorkdays { get; set; }

        [JsonPropertyName("person_in_charge")]
        public int? person_in_charge { get; set; } // người phụ trách
        [JsonPropertyName("str_person_in_charge")]
        public string str_person_in_charge { get; set; } // người phụ trách

        [JsonPropertyName("num_of_auditor")]
        public int? NumOfAuditor { get; set; } // số luowg KTV

        [JsonPropertyName("req_skill_audit")]
        public string ReqSkillForAudit { get; set; } // yêu cầu về kỹ năng kiểm toán

        [JsonPropertyName("req_outsourcing")]
        public string ReqOutsourcing { get; set; } // yêu cầu thuê ngoài

        [JsonPropertyName("req_other")]
        public string ReqOther { get; set; } // yêu cầu khác

        [JsonPropertyName("scale_of_audit")]
        public int? ScaleOfAudit { get; set; } // quy mô cuộc kiểm toán

        [JsonPropertyName("auditplan_id")]
        public int? AuditplanId { get; set; }

        [JsonPropertyName("listauditor")]
        public List<ListAuditorModel> ListAuditor { get; set; }

        [JsonPropertyName("listauditworkscope")]
        public List<AuditScopeDetailModel> ListAuditWorkScope { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("list_file")]
        public List<AuditWorkPlanFileModel> ListFile { get; set; }
        [JsonPropertyName("listauditworkscopefacility")]
        public List<AuditScopeDetailModel> ListAuditWorkScopeFacility { get; set; }
        [JsonPropertyName("audit_scope")]
        public string AuditScope { get; set; }
    }
    public class AuditWorkPlanFileModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
    }
    public class ListAuditWorkScope
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public int? auditprocess_id { get; set; }
        [JsonPropertyName("bussinessactivities_id")]
        public int? bussinessactivities_id { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }
        [JsonPropertyName("reason")]
        public string reason { get; set; } //Lý do kiểm toán
        //Xếp hạng rủi ro: 1-Cao, 2-Trung bình, 3-Thấp
        [JsonPropertyName("risk_rating_id")]
        public int? risk_rating_id { get; set; }//xếp hạng rủi ro
        [JsonPropertyName("risk_rating")]
        public string risk_rating { get; set; }//xếp hạng rủi ro
        [JsonPropertyName("auditing_time_nearest")]
        public string auditing_time_nearest { get; set; }// Thời gian kiểm toán gần nhất
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; } //Đơn vị được kiểm toán
        [JsonPropertyName("auditprocess_name")]
        public string auditprocess_name { get; set; } //Quy trình kiểm toán
        [JsonPropertyName("bussinessactivities_name")]
        public string bussinessactivities_name { get; set; } //Hoạt động kiểm toán
        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("brief_review")]
        public string brief_review { get; set; }//Đánh giá sơ bộ
        [JsonPropertyName("path")]
        public string path { get; set; }//file

        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

        [JsonPropertyName("filename")]
        public string filename { get; set; }

        //[JsonPropertyName("audit_team_leader")]
        //public IEnumerable<string> audit_team_leader { get; set; }

        //[JsonPropertyName("auditor")]
        //public IEnumerable<string> auditor { get; set; }


        [JsonPropertyName("audit_team_leader")]
        public string audit_team_leader { get; set; }
        [JsonPropertyName("auditor")]
        public string auditor { get; set; }
    }
    public class ListAuditFacility
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; } //Đơn vị được kiểm toán
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; } //Đơn vị được kiểm toán
        [JsonPropertyName("reason")]
        public string reason { get; set; } //Lý do kiểm toán
        //Xếp hạng rủi ro: 1-Cao, 2-Trung bình, 3-Thấp
        [JsonPropertyName("risk_rating_id")]
        public int? risk_rating_id { get; set; }//xếp hạng rủi ro
        [JsonPropertyName("risk_rating")]
        public string risk_rating { get; set; }//xếp hạng rủi ro
        [JsonPropertyName("auditing_time_nearest")]
        public string AuditingTimeNearest { get; set; }
        [JsonPropertyName("brief_review")]
        public string brief_review { get; set; }//Đánh giá sơ bộ
        [JsonPropertyName("path")]
        public string path { get; set; }//file

        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

        [JsonPropertyName("filename")]
        public string filename { get; set; }

        [JsonPropertyName("list_files")]
        public List<AuditWorkScopeFacilityFilesModel> ListFiles { get; set; }
    }
    public class AuditWorkScopeFacilityFilesModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
    }
    public class AuditWorkListModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("person_in_charge")]
        public string PersonInCharge { get; set; } // người phụ trách
        [JsonPropertyName("auditfacilities_name")]
        public string AuditFacility { get; set; } // đơn vị

        [JsonPropertyName("auditprocess_name")]
        public string AuditProcess { get; set; } // đơn vị

        [JsonPropertyName("year")]// nam
        public string Year { get; set; }

        [JsonPropertyName("create_at")]// nam
        public DateTime? CreateAt { get; set; }
    }
    public class AuditWorkModifyModel
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("num_of_workdays")]
        public string NumOfWorkdays { get; set; }

        [JsonPropertyName("person_in_charge")]
        public string person_in_charge { get; set; } // người phụ trách

        [JsonPropertyName("num_of_auditor")]
        public string NumOfAuditor { get; set; } // số luowg KTV

        [JsonPropertyName("req_skill_audit")]
        public string ReqSkillForAudit { get; set; } // yêu cầu về kỹ năng kiểm toán

        [JsonPropertyName("req_outsourcing")]
        public string ReqOutsourcing { get; set; } // yêu cầu thuê ngoài

        [JsonPropertyName("req_other")]
        public string ReqOther { get; set; } // yêu cầu khác

        [JsonPropertyName("scale_of_audit")]
        public string ScaleOfAudit { get; set; } // quy mô cuộc kiểm toán

        [JsonPropertyName("auditplan_id")]
        public string auditplan_id { get; set; }

        [JsonPropertyName("list_assign")]
        public List<AuditAssignmentUpdateModel> ListAssign { get; set; } // quy mô cuộc kiểm toán
        [JsonPropertyName("classify")]
        public int? Classify { get; set; }

        [JsonPropertyName("year_plan")]
        public string Year { get; set; }

        [JsonPropertyName("list_scope")]
        public List<AuditScopeModifyModel> ListScope { get; set; } // phạm vi cuộc kiểm toán

        [JsonPropertyName("list_scope_facility")]
        public List<AuditScopeModifyFacilityModel> ListScopeFacility { get; set; } // đươn vị cuộc kiểm toán

        [JsonPropertyName("audit_scope")]
        public string AuditScope { get; set; }
    }
    public class AuditPlanUpdateStatusModel
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Id { get; set; }
        //chỉ chuyển từ chờ duyệt-2 sang Đã duyệt-3 hoặc Từ chối duyệt-4 
        [JsonPropertyName("status")]
        public string Status { get; set; }
        //Browsedate là ngày phê duyệt/ ngày từ chối
        [JsonPropertyName("browsedate")]
        public string Browsedate { get; set; }
        ////file
        //[JsonPropertyName("path")]
        //public string Path { get; set; }
    }
    public class AuditWorkSearchModel
    {
        [JsonPropertyName("year")]
        public string Year { get; set; }
        [JsonPropertyName("execution_status")]
        public int? execution_status { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
    public class AuditAssignmentUpdateModel
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }
    }
    public class AuditScopeModifyModel
    {
        [JsonPropertyName("board_id")]
        public string Board_id { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public string auditprocess_id { get; set; }

        [JsonPropertyName("auditprocess_name")]
        public string auditprocess_name { get; set; }

        [JsonPropertyName("bussinessactivities_id")]
        public string bussinessactivities_id { get; set; }

        [JsonPropertyName("bussinessactivities_name")]
        public string bussinessactivities_name { get; set; }

        [JsonPropertyName("auditfacilities_id")]
        public string auditfacilities_id { get; set; }

        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }

        [JsonPropertyName("risk_rating")]
        public string risk_rating { get; set; }

        [JsonPropertyName("risk_rating_name")]
        public string risk_rating_name { get; set; }
        [JsonPropertyName("reason")]
        public string reason { get; set; }
        [JsonPropertyName("auditing_time_nearest")]
        public string auditing_time_nearest { get; set; }
    }
    public class AuditScopeModifyFacilityModel
    {
        [JsonPropertyName("board_id")]
        public string Board_id { get; set; }
        
        [JsonPropertyName("auditfacilities_id")]
        public string auditfacilities_id { get; set; }

        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }

        [JsonPropertyName("risk_rating")]
        public string risk_rating { get; set; }

        [JsonPropertyName("risk_rating_name")]
        public string risk_rating_name { get; set; }
        [JsonPropertyName("reason")]
        public string reason { get; set; }
        [JsonPropertyName("auditing_time_nearest")]
        public string auditing_time_nearest { get; set; }
    }
    public class AuditScopeDetailModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("board_id")]
        public int? Board_id { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public int? auditprocess_id { get; set; }

        [JsonPropertyName("auditprocess_name")]
        public string auditprocess_name { get; set; }

        [JsonPropertyName("bussinessactivities_id")]
        public int? bussinessactivities_id { get; set; }

        [JsonPropertyName("bussinessactivities_name")]
        public string bussinessactivities_name { get; set; }

        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }

        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }

        [JsonPropertyName("risk_rating")]
        public int? risk_rating { get; set; }
        [JsonPropertyName("risk_rating_name")]
        public string risk_rating_name { get; set; }
        [JsonPropertyName("reason")]
        public string reason { get; set; }
        [JsonPropertyName("auditing_time_nearest")]
        public string auditing_time_nearest { get; set; }
    }

    public class SearchPrepareAuditModel
    {
        [JsonPropertyName("year")]
        public string Year { get; set; }
        [JsonPropertyName("classify")]
        public int? Classify { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("person_in_charge")]
        public int? PersonInCharge { get; set; }
        [JsonPropertyName("status")]
        //BrowsingStatus : Trạng thái 2 chờ duyệt , 3 đã duyệt , 4 từ chối duyệt
        public string Status { get; set; }
        //ExecutionStatus : Trạng thái thực hiện mặc định chưa thực hiện
        [JsonPropertyName("execution_status_str")]
        public string ExecutionStatusStr { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
    public class ListAuditWorkModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("num_of_workdays")]
        public float? NumOfWorkdays { get; set; }

        [JsonPropertyName("person_in_charge")]
        public int? person_in_charge { get; set; } // người phụ trách

        [JsonPropertyName("name_person_in_charge")]
        public string name_person_in_charge { get; set; } // người phụ trách

        [JsonPropertyName("num_of_auditor")]
        public int? NumOfAuditor { get; set; } // số luowg KTV

        [JsonPropertyName("req_skill_audit")]
        public string ReqSkillForAudit { get; set; } // yêu cầu về kỹ năng kiểm toán

        [JsonPropertyName("req_outsourcing")]
        public string ReqOutsourcing { get; set; } // yêu cầu thuê ngoài

        [JsonPropertyName("req_other")]
        public string ReqOther { get; set; } // yêu cầu khác

        [JsonPropertyName("scale_of_audit")]
        public int? ScaleOfAudit { get; set; } // quy mô cuộc kiểm toán

        //Status : Trạng thái . có 5 trạng thái là 1 bản nháp , 2 chờ duyệt , 3 đã duyệt , 4 từ chối duyệt , 5 ngưng sử dụng
        [JsonPropertyName("status")]
        public string Status { get; set; }

        //ExecutionStatus : Trạng thái thực hiện mặc định chưa thực hiện
        [JsonPropertyName("execution_status")]
        public int? ExecutionStatus { get; set; }
        //ExecutionStatus : Trạng thái thực hiện mặc định chưa thực hiện
        [JsonPropertyName("execution_status_str")]
        public string ExecutionStatusStr { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

        [JsonPropertyName("modified_at")]
        public string ModifiedAt { get; set; }

        [JsonPropertyName("modified_by")]
        public int? ModifiedBy { get; set; }

        [JsonPropertyName("deleted_at")]
        public string DeletedAt { get; set; }

        [JsonPropertyName("deleted_by")]
        public int? DeletedBy { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("classify")]
        public int? Classify { get; set; }
        [JsonPropertyName("year")]
        public string Year { get; set; }

        [JsonPropertyName("auditplan_id")]
        public int? auditplan_id { get; set; }
        [JsonPropertyName("extension_time")]
        public string ExtensionTime { get; set; }
        [JsonPropertyName("user_login_id")]
        public int? user_login_id { get; set; }
        [JsonPropertyName("flag_user_id")]
        public int? flag_user_id { get; set; }
        [JsonPropertyName("approval_user")]
        public int? ApprovalUser { get; set; } // người duyệt

        [JsonPropertyName("approval_user_last")]
        public int? ApprovalUserLast { get; set; } // người duyệt
    }
    public class ListAuditPersonnelModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("user_id")]
        public int? User_id { get; set; }
        [JsonPropertyName("fullName")]
        public string FullName { get; set; } // người phụ trách
        [JsonPropertyName("email")]
        public string Email { get; set; } // người phụ trách
        [JsonPropertyName("str_fullName")]
        public string str_fullName { get; set; } // người phụ trách
    }
    public class ListAuditorModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("auditor")]
        public string Auditor { get; set; }
        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }
        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }
    }
    public class ListSchedule
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("work")]
        public string work { get; set; }
        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }
        [JsonPropertyName("user_name")]
        public string user_name { get; set; } // người phụ trách
        [JsonPropertyName("expected_date")]
        public string expected_date { get; set; } // ngày dự kiến
        [JsonPropertyName("actual_date")]
        public string actual_date { get; set; } //Ngày thực tế thực hiện

        [JsonPropertyName("str_person_in_charge")]
        public string str_person_in_charge { get; set; } // người phụ trách
    }
    public class AuditWorkEditModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date_planning")]//Ngày kết thúc lập KH
        public DateTime? EndDatePlanning { get; set; }

        [JsonPropertyName("start_date_real")]//Ngày bắt đầu thực địa
        public DateTime? StartDateReal { get; set; }

        [JsonPropertyName("release_date")]//Ngày phát hành BC
        public DateTime? ReleaseDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("num_of_workdays")]
        public float? NumOfWorkdays { get; set; }

        //[JsonPropertyName("status")]
        //public int? Status { get; set; }

        //ExecutionStatus : Trạng thái thực hiện mặc định chưa thực hiện
        [JsonPropertyName("execution_status_str")]
        public string ExecutionStatusStr { get; set; }

        [JsonPropertyName("person_in_charge")]
        public int? person_in_charge { get; set; } // người phụ trách

        [JsonPropertyName("num_of_auditor")]
        public int? NumOfAuditor { get; set; } // số luowg KTV

        [JsonPropertyName("req_skill_audit")]
        public string ReqSkillForAudit { get; set; } // yêu cầu về kỹ năng kiểm toán

        [JsonPropertyName("req_outsourcing")]
        public string ReqOutsourcing { get; set; } // yêu cầu thuê ngoài

        [JsonPropertyName("req_other")]
        public string ReqOther { get; set; } // yêu cầu khác

        [JsonPropertyName("scale_of_audit")]
        public int? ScaleOfAudit { get; set; } // quy mô cuộc kiểm toán

        [JsonPropertyName("modified_by")]
        public int? ModifiedBy { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("classify")]
        public int? Classify { get; set; }

        [JsonPropertyName("year")]
        public string Year { get; set; }

        [JsonPropertyName("audit_scope_outside")]
        public string AuditScopeOutside { get; set; }

        [JsonPropertyName("auditplan_id")]
        public int? auditplan_id { get; set; }

        [JsonPropertyName("extension_time")]
        public DateTime? ExtensionTime { get; set; }

        [JsonPropertyName("from_date")]
        public DateTime? from_date { get; set; }

        [JsonPropertyName("to_date")]
        public DateTime? to_date { get; set; }


        [JsonPropertyName("audit_scope")]
        public string AuditScope { get; set; }//Phạm vi kiểm toán
        //[JsonPropertyName("budget")]
        //public string budget { get; set; }//Ngân sách

        [JsonPropertyName("listauditpersonnel")]

        public List<ListAuditAssignmentModel> ListAuditAssignment { get; set; }//tab2
        [JsonPropertyName("listauditworkscope")]
        public List<ListAuditWorkScopeEditModel> ListAuditWorkScope { get; set; }//tab3
        [JsonPropertyName("listschedule")]
        public List<ListScheduleEditModel> ListScheduleEditModel { get; set; }//tab 4
        [JsonPropertyName("listworkscope")]
        public List<ListWorkScopeEditModel> ListWorkScope { get; set; }//tab 5
    }
    public class ListWorkScopeEditModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("brief_review")]
        public string brief_review { get; set; }
        [JsonPropertyName("path")]
        public string path { get; set; }
    }
    public class ListAuditAssignmentModel
    {
        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }
        //[JsonPropertyName("user_id_name")]
        //public string user_id_name { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
    }
    public class ListAuditWorkScopeEditModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("audit_team_leader")]
        public int? audit_team_leader { get; set; }
        [JsonPropertyName("auditor")]
        public List<int?> auditor { get; set; }
    }
    public class ListScheduleEditModel
    {
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("work")]
        public string work { get; set; }
        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }
        [JsonPropertyName("expected_date")]
        public string expected_date { get; set; } // ngày dự kiến
        [JsonPropertyName("actual_date")]
        public string actual_date { get; set; } //Ngày thực tế thực hiện
    }
    public class DropListYearAuditWorkModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("year")]
        public string year { get; set; }
        [JsonPropertyName("current_year")]
        public bool current_year { get; set; }
        [JsonPropertyName("year_sort")]
        public int year_sort { get; set; }
    }
    public class DropListAuditWorkModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("year")]
        public string year { get; set; }
    }
    public class DropListAuditFacilityModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }
    }
    public class DropListAuditProcessModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public int? auditprocess_id { get; set; }
        [JsonPropertyName("auditprocess_name")]
        public string auditprocess_name { get; set; }
    }
    public class DropListAuditAssignmentModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }
        [JsonPropertyName("fullName")]
        public string fullName { get; set; }
    }
    public class UploadFileModel
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? id { get; set; }
        [JsonPropertyName("brief_review")]
        public string brief_review { get; set; }
    }
    public class CreateAuditWorkScopeModel
    {
        [JsonPropertyName("auditwork_id")]
        public int auditwork_id { get; set; }
        [JsonPropertyName("year")]
        public int year { get; set; }
        [JsonPropertyName("score_board_id")]
        public int score_board_id { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public int auditprocess_id { get; set; }
        [JsonPropertyName("bussinessactivities_id")]
        public int bussinessactivities_id { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public int auditfacilities_id { get; set; }
        [JsonPropertyName("auditprocess_name")]
        public string auditprocess_name { get; set; }
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }
        [JsonPropertyName("bussinessactivities_name")]
        public string bussinessactivities_name { get; set; }
        [JsonPropertyName("reason")]
        public string reason { get; set; }
        [JsonPropertyName("risk_level")]
        public string risk_level { get; set; }
        [JsonPropertyName("risk_rating_name")]
        public string risk_rating_name { get; set; }
    }
    public class CreateAuditWorkScopeFacilityModel
    {
        [JsonPropertyName("auditwork_id")]
        public int auditwork_id { get; set; }
        [JsonPropertyName("year")]
        public int year { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public int auditfacilities_id { get; set; }
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }
        [JsonPropertyName("reason")]
        public string reason { get; set; }
        [JsonPropertyName("risk_rating_id")]
        public int risk_rating_id { get; set; }
        [JsonPropertyName("risk_rating_name")]
        public string risk_rating_name { get; set; }
        [JsonPropertyName("last_audit")]
        public string last_audit { get; set; }
        [JsonPropertyName("last_audit_time")]
        public DateTime? last_audit_time { get; set; }
        [JsonPropertyName("score_board_id")]
        public int? score_board_id { get; set; }
    }
    public class DropListYearApprovedModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("year")]
        public string year { get; set; }
    }
    public class DropListNameAuditWorkModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
    }
    public class CreateAuditWorkModel
    {
        [JsonPropertyName("classify")]
        public int classify { get; set; }
        [JsonPropertyName("year")]
        public string year { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
    }
    public class DropListUserTypeModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
    }
    public class AuditWorkExportModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("person_in_charge")]
        public string PersonInCharge { get; set; } // người phụ trách
        [JsonPropertyName("auditfacilities_name")]
        public string AuditFacility { get; set; } // đơn vị

        [JsonPropertyName("auditprocess_name")]
        public string AuditProcess { get; set; } // đơn vị

        [JsonPropertyName("num_of_workdays")]
        public int? NumOfWorkdays { get; set; }

        [JsonPropertyName("num_of_auditor")]
        public int? NumOfAuditor { get; set; } // số luowg KTV

        [JsonPropertyName("req_skill_audit")]
        public string ReqSkillForAudit { get; set; } // yêu cầu về kỹ năng kiểm toán

        [JsonPropertyName("req_outsourcing")]
        public string ReqOutsourcing { get; set; } // yêu cầu thuê ngoài

        [JsonPropertyName("req_other")]
        public string ReqOther { get; set; } // yêu cầu khác

        [JsonPropertyName("scale_of_audit")]
        public int? ScaleOfAudit { get; set; } // quy mô cuộc kiểm toán

        [JsonPropertyName("reason")]
        public string reason { get; set; }

        [JsonPropertyName("list_assign")]
        public string ListAssign { get; set; } // quy mô cuộc kiểm toán

        [JsonPropertyName("risk_rating")]
        public string risk_rating { get; set; }

        [JsonPropertyName("auditing_time_nearest")]
        public string auditing_time_nearest { get; set; }
    }

    public class DropListStatusPrepareAuditPlanModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("status_code")]
        public string status_code { get; set; }

        [JsonPropertyName("status_name")]
        public string status_name { get; set; }
    }
    public class DropListUpdateStatusPrepareAuditPlanModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("status_code")]
        public string status_code { get; set; }

        [JsonPropertyName("status_name")]
        public string status_name { get; set; }
    }
    public class AuditBudgetDetailModel
    {

        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("budget")]
        public string budget { get; set; }//Ngân sách
    }
    public class AuditBudgetEditModel
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("budget")]
        public string budget { get; set; }//Ngân sách
    }
    public class AuditOtherDetailModel
    {

        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("other")]
        public string other { get; set; }//khác
    }
    public class AuditOtherEditModel
    {

        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("other")]
        public string other { get; set; }//khác
    }
}