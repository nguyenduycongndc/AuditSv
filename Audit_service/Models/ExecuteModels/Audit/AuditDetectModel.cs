using Audit_service.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class AuditDetectSearchModel
    {
        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public int? auditprocess_id { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }
        [JsonPropertyName("code")]
        public string code { get; set; }
        [JsonPropertyName("title")]
        public string title { get; set; }
        [JsonPropertyName("working_paper_code")]
        public string working_paper_code { get; set; }
        [JsonPropertyName("audit_report")]
        public int audit_report { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
    public class AuditDetectModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("auditwork_name")]
        public string auditwork_name { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public int? auditprocess_id { get; set; }
        [JsonPropertyName("auditprocess_name")]
        public string auditprocess_name { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }
        [JsonPropertyName("code")]
        public string code { get; set; }
        [JsonPropertyName("title")]
        public string title { get; set; }
        [JsonPropertyName("status")]
        public string status { get; set; }
        [JsonPropertyName("working_paper_code")]
        public string working_paper_code { get; set; }
        [JsonPropertyName("audit_report")]
        public int? audit_report { get; set; }
        //Xếp hạng rủi ro: 1-Cao, 2-Trung bình, 3-Thấp
        [JsonPropertyName("rating_risk")]
        public int? rating_risk { get; set; }
        public int? flag_user_id { get; set; }
        [JsonPropertyName("user_login_id")]
        public int? user_login_id { get; set; }

        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }
        [JsonPropertyName("approval_user")]
        public int? ApprovalUser { get; set; } // người duyệt

        [JsonPropertyName("approval_user_last")]
        public int? ApprovalUserLast { get; set; } // người duyệt
    }
    public class AuditDetectDetail
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("code")]
        public string code { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("statusName")]
        public string statusName { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }//có 5 trạng thái là 1 bản nháp , 2 chờ duyệt , 3 đã duyệt , 4 từ chối duyệt , 5 ngưng sử dụng 

        [JsonPropertyName("title")]
        public string title { get; set; }//Tiêu đề phát hiện kiểm toán

        [JsonPropertyName("short_title")]
        public string short_title { get; set; }//Tiêu đề rút gọn phát hiện kiểm toán

        [JsonPropertyName("description")]
        public string description { get; set; }//mô tả

        [JsonPropertyName("evidence")]
        public string evidence { get; set; }//Bằng chứng phất hiện KT

        [JsonPropertyName("path_audit_detect")]
        public string path_audit_detect { get; set; }//File

        [JsonPropertyName("affect")]
        public string affect { get; set; }//ảnh hưởng

        [JsonPropertyName("rating_risk")]
        public int? rating_risk { get; set; }//xếp hạng rủi ro
        [JsonPropertyName("admin_framework")]
        public int? admin_framework { get; set; }//xếp hạng rủi ro

        [JsonPropertyName("cause")]
        public string cause { get; set; }//Nguyên nhân

        [JsonPropertyName("audit_report")]
        public bool audit_report { get; set; }//Đưa vào báo cáo kiểm toán

        [JsonPropertyName("classify_audit_detect")]
        public int? classify_audit_detect { get; set; }//Phân loại phát hiện
        [JsonPropertyName("str_classify_audit_detect")]
        public string str_classify_audit_detect { get; set; }//Phân loại phát hiện

        [JsonPropertyName("summary_audit_detect")]
        public string summary_audit_detect { get; set; }//Tóm tắt phát hiện

        [JsonPropertyName("followers")]
        public int? followers { get; set; }//Người theo dõi

        [JsonPropertyName("str_followers")]
        public string str_followers { get; set; } //Người theo dõi id+ name

        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("str_year")]
        public string str_year { get; set; }//Năm id+ name

        [JsonPropertyName("opinion_audit")]
        public bool opinion_audit { get; set; }//Ý kiến của ĐVĐKT

        [JsonPropertyName("reason")]
        public string reason { get; set; }//lý do

        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }//id của cuộc kiểm toán ở trạng thái "Đã duyệt" thuộc năm đã chọn

        [JsonPropertyName("auditwork_name")]
        public string auditwork_name { get; set; }
        [JsonPropertyName("str_auditwork_name")]
        public string str_auditwork_name { get; set; }

        [JsonPropertyName("auditprocess_id")]
        public int? auditprocess_id { get; set; }//id của quy trình

        [JsonPropertyName("auditprocess_name")]
        public string auditprocess_name { get; set; }
        [JsonPropertyName("str_auditprocess_name")]
        public string str_auditprocess_name { get; set; }

        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }//id của đơn vị 

        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }
        [JsonPropertyName("str_auditfacilities_name")]
        public string str_auditfacilities_name { get; set; }
        [JsonPropertyName("working_paper_code")]
        public string working_paper_code { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime? ModifiedAt { get; set; }

        [JsonPropertyName("modified_by")]
        public int? ModifiedBy { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("deleted_by")]
        public int? DeletedBy { get; set; }

        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

        [JsonPropertyName("filename")]
        public string filename { get; set; }

        [JsonPropertyName("listauditobserve")]
        public List<ListAuditObserve> ListAuditObserve { get; set; }

        [JsonPropertyName("listauditrequestmonitor")]
        public List<ListAuditRequestMonitor> ListAuditRequestMonitor { get; set; }

        [JsonPropertyName("list_file")]
        public List<AudiDetectFileModel> ListFile { get; set; }
    }

    public class AudiDetectFileModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
    }
    public class ListAuditObserve
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("code")]
        public string code { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
        [JsonPropertyName("working_paper_code")]
        public string working_paper_code { get; set; }
    }
    public class ListAuditRequestMonitor
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("code")]
        public string code { get; set; }
        [JsonPropertyName("content")]
        public string content { get; set; }
        [JsonPropertyName("auditrequesttypeid")]
        public int? auditrequesttypeid { get; set; }
        [JsonPropertyName("auditrequesttype_name")]
        public string auditrequesttype_name { get; set; }//phân loại kiến nghị
        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }
        [JsonPropertyName("user_name")]
        public string user_name { get; set; }//người chịu trách nghiệm
        [JsonPropertyName("unit_id")]
        public int? unit_id { get; set; }
        [JsonPropertyName("unit_name")]
        public string unit_name { get; set; }//Đơn vị đầu mối
        [JsonPropertyName("cooperateunit_id")]
        public int? cooperateunit_id { get; set; }
        [JsonPropertyName("cooperateunit_name")]
        public string cooperateunit_name { get; set; }//Đơn vị phối hợp
        [JsonPropertyName("completeat")]
        public string completeat { get; set; }//Thời hạn hoàn thành
    }

    public class UncheckedModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("audit_detect_id")]
        public int? audit_detect_id { get; set; }
    }
    public class DropListCatDetectTypeModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
    }
    public class CreateAuditDetectModel
    {
        [JsonPropertyName("year")]
        public string year { get; set; }
        [JsonPropertyName("auditwork_id")]
        public string auditwork_id { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public string auditfacilities_id { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public string auditprocess_id { get; set; }
        [JsonPropertyName("title")]
        public string title { get; set; }
        [JsonPropertyName("short_title")]
        public string short_title { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
        [JsonPropertyName("evidence")]
        public string evidence { get; set; }
        [JsonPropertyName("affect")]
        public string affect { get; set; }
        [JsonPropertyName("rating_risk")]
        public string rating_risk { get; set; }
        [JsonPropertyName("admin_framework")]
        public string admin_framework { get; set; }
        [JsonPropertyName("cause")]
        public string cause { get; set; }
        [JsonPropertyName("audit_report")]
        public bool audit_report { get; set; }
        [JsonPropertyName("classify_audit_detect")]
        public string classify_audit_detect { get; set; }
        [JsonPropertyName("summary_audit_detect")]
        public string summary_audit_detect { get; set; }
        [JsonPropertyName("followers")]
        public string followers { get; set; }
        [JsonPropertyName("opinion_audit")]
        public bool opinion_audit { get; set; }
        [JsonPropertyName("reason")]
        public string reason { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
    }
    //public class CreateAuditDetectModel
    //{
    //    [JsonPropertyName("year")]
    //    public int? year { get; set; }
    //    [JsonPropertyName("auditwork_id")]
    //    public int? auditwork_id { get; set; }
    //    [JsonPropertyName("auditfacilities_id")]
    //    public int? auditfacilities_id { get; set; }
    //    [JsonPropertyName("auditprocess_id")]
    //    public int? auditprocess_id { get; set; }
    //    [JsonPropertyName("title")]
    //    public string title { get; set; }
    //    [JsonPropertyName("short_title")]
    //    public string short_title { get; set; }
    //    [JsonPropertyName("description")]
    //    public string description { get; set; }
    //    [JsonPropertyName("evidence")]
    //    public string evidence { get; set; }
    //    //[JsonPropertyName("path_audit_detect")]
    //    //public string path_audit_detect { get; set; }
    //    [JsonPropertyName("affect")]
    //    public string affect { get; set; }
    //    [JsonPropertyName("rating_risk")]
    //    public int rating_risk { get; set; }
    //    [JsonPropertyName("cause")]
    //    public string cause { get; set; }
    //    [JsonPropertyName("audit_report")]
    //    public bool audit_report { get; set; }
    //    [JsonPropertyName("classify_audit_detect")]
    //    public int? classify_audit_detect { get; set; }
    //    [JsonPropertyName("summary_audit_detect")]
    //    public string summary_audit_detect { get; set; }
    //    [JsonPropertyName("followers")]
    //    public int followers { get; set; }
    //    [JsonPropertyName("opinion_audit")]
    //    public bool opinion_audit { get; set; }
    //    [JsonPropertyName("reason")]
    //    public string reason { get; set; }
    //    [JsonPropertyName("name")]
    //    public string name { get; set; }
    //}
    public class SearchModalObserveModel
    {

        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public int? auditprocess_id { get; set; }
        [JsonPropertyName("discoverer")]
        public int? discoverer { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("working_paper_code")]
        public string working_paper_code { get; set; }
    }
    public class AuditObserveModalModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("code")]
        public string code { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
        [JsonPropertyName("auditwork_name")]
        public string auditwork_name { get; set; }
        [JsonPropertyName("working_paper_code")]
        public string working_paper_code { get; set; }
        [JsonPropertyName("discoverer_name")]
        public string discoverer_name { get; set; }
        [JsonPropertyName("discoverer_id")]
        public int discoverer_id { get; set; }
    }

    public class ChooseAll
    {
        public string listID { get; set; }
    }
    public class CreateAuditRequestMonitorModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("content")]
        public string content { get; set; }
        [JsonPropertyName("audit_request_type_id")]
        public int? audit_request_type_id { get; set; }//phân loại kiến nghị 
        [JsonPropertyName("unit_id")]
        public int? unit_id { get; set; }//Đơn vị đầu mối
        [JsonPropertyName("cooperateunit_id")]
        public List<int?> cooperateunit_id { get; set; }//Đơn vị phối hợp
        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }//người chịu trách nghiệm
        [JsonPropertyName("complete_at")]
        public string complete_at { get; set; }//thời gian hoàn thành
        [JsonPropertyName("note")]
        public string note { get; set; }//ghi chú
    }
    public class DetailAuditRequestMonitorModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("code")]
        public string code { get; set; }
        [JsonPropertyName("content")]
        public string content { get; set; }
        [JsonPropertyName("audit_request_type_id")]
        public int? audit_request_type_id { get; set; }//phân loại kiến nghị 
        [JsonPropertyName("str_audit_request_type_id")]
        public string str_audit_request_type_id { get; set; }//phân loại kiến nghị 
        [JsonPropertyName("audit_request_type_name")]
        public string audit_request_type_name { get; set; }// tên phân loại kiến nghị  
        [JsonPropertyName("str_unit_name")]
        public string str_unit_name { get; set; }//Đơn vị đầu mối
        [JsonPropertyName("unit_name")]
        public string unit_name { get; set; }// tên Đơn vị đầu mối
        [JsonPropertyName("str_cooperateunit_name")]
        public string str_cooperateunit_name { get; set; }//Đơn vị phối hợp
        [JsonPropertyName("cooperateunit_name")]
        public string cooperateunit_name { get; set; }//tên Đơn vị phối hợp
        [JsonPropertyName("str_user_name")]
        public string str_user_name { get; set; }//người chịu trách nghiệm
        [JsonPropertyName("responsible_person")]
        public string responsible_person { get; set; }//tên người chịu trách nghiệm
        [JsonPropertyName("complete_at")]
        public string complete_at { get; set; }//thời gian hoàn thành
        [JsonPropertyName("note")]
        public string note { get; set; }//ghi chú
    }
    public class EditAuditRequestMonitorModel
    {

        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("content")]
        public string content { get; set; }
        [JsonPropertyName("audit_request_type_id")]
        public int? audit_request_type_id { get; set; }//phân loại kiến nghị 
        [JsonPropertyName("unit_id")]
        public int? unit_id { get; set; }//Đơn vị đầu mối
        [JsonPropertyName("cooperateunit_id")]
        public List<int?> cooperateunit_id { get; set; }//Đơn vị phối hợp
        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }//người chịu trách nghiệm
        [JsonPropertyName("complete_at")]
        public DateTime? complete_at { get; set; }//thời gian hoàn thành
        [JsonPropertyName("note")]
        public string note { get; set; }//ghi chú
    }
    public class DropListFacilityModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
    }
    public class DropListCatAuditRequestModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
    }
    //Model cap nhat auditDetect
    public class EditAuditDetectModel
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? id { get; set; }
        [JsonPropertyName("year")]
        public string year { get; set; }
        [JsonPropertyName("auditwork_id")]
        public string auditwork_id { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public string auditfacilities_id { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public string auditprocess_id { get; set; }
        [JsonPropertyName("title")]
        public string title { get; set; }
        [JsonPropertyName("short_title")]
        public string short_title { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
        [JsonPropertyName("evidence")]
        public string evidence { get; set; }
        [JsonPropertyName("affect")]
        public string affect { get; set; }
        [JsonPropertyName("rating_risk")]
        public string rating_risk { get; set; }
        [JsonPropertyName("admin_framework")]
        public string admin_framework { get; set; }
        [JsonPropertyName("cause")]
        public string cause { get; set; }
        [JsonPropertyName("audit_report")]
        public bool audit_report { get; set; }
        [JsonPropertyName("classify_audit_detect")]
        public string classify_audit_detect { get; set; }
        [JsonPropertyName("summary_audit_detect")]
        public string summary_audit_detect { get; set; }
        [JsonPropertyName("followers")]
        public string followers { get; set; }
        [JsonPropertyName("opinion_audit")]
        public bool opinion_audit { get; set; }
        [JsonPropertyName("reason")]
        public string reason { get; set; }
    }
    public class ListUsersModels
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
    }
    public class UsersInfoModels
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
    }
}
