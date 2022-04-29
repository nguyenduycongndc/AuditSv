using Audit_service.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class AuditMinutesModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("year")]
        public int? year { get; set; }

        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }//id của cuộc kiểm toán ở trạng thái "Đã duyệt" thuộc năm đã chọn

        [JsonPropertyName("auditwork_name")]
        public string auditwork_name { get; set; }

        [JsonPropertyName("auditwork_code")]
        public string auditwork_code { get; set; }

        [JsonPropertyName("str_auditwork_name")]
        public string str_auditwork_name { get; set; }

        [JsonPropertyName("audit_work_taget")]
        public string audit_work_taget { get; set; }//Mục đích kiểm toán

        [JsonPropertyName("audit_work_person")]
        public int audit_work_person { get; set; }//Người phụ trách

        [JsonPropertyName("audit_work_person_name")]
        public string audit_work_person_name { get; set; }//Người phụ trách

        [JsonPropertyName("str_audit_work_person")]
        public string str_audit_work_person { get; set; }

        [JsonPropertyName("audit_work_classify")]
        public int audit_work_classify { get; set; }//Phân loại :1- theo kế hoạch; 2- đột xuất

        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }//id của đơn vị 

        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; }

        [JsonPropertyName("str_auditfacilities_name")]
        public string str_auditfacilities_name { get; set; }

        [JsonPropertyName("status")]
        public int? status { get; set; }//1 chưa xác nhận, 2 đã xác nhận

        [JsonPropertyName("path")]
        public string path { get; set; }//File

        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

        [JsonPropertyName("filename")]
        public string filename { get; set; }

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

        [JsonPropertyName("from_date")]
        public string from_date { get; set; }

        [JsonPropertyName("to_date")]
        public string to_date { get; set; }

        [JsonPropertyName("audit_scope_outside")]
        public string AuditScopeOutside { get; set; }

        [JsonPropertyName("rating")]
        public string rating { get; set; }
        [JsonPropertyName("str_rating")]
        public string str_rating { get; set; }

        [JsonPropertyName("problem")]
        public string problem { get; set; }

        [JsonPropertyName("idea")]
        public string idea { get; set; }

        [JsonPropertyName("audit_scope")]//Phạm vi kiểm toán
        public string audit_scope { get; set; }
        //[JsonPropertyName("risk_rating_hight")]
        //public int? risk_rating_hight { get; set; }//tổng Xếp hạng rủi ro Cao
        //[JsonPropertyName("risk_rating_medium")]
        //public int? risk_rating_medium { get; set; }//tổng Xếp hạng rủi ro Trung bình
        //[JsonPropertyName("risk_rating_low")]
        //public int? risk_rating_low { get; set; }//tổng Xếp hạng rủi ro: Thấp

        [JsonPropertyName("other_content")]//Phạm vi kiểm toán
        public string OtherContent { get; set; }

        [JsonPropertyName("listauditworkscope")]
        public List<ListAuditWorkScopeAuditMinutes> ListAuditWorkScopeAuditMinutes { get; set; }
        [JsonPropertyName("listStatisticsOfDetections")]
        public List<ListStatisticsOfDetections> ListStatisticsOfDetections { get; set; }
        [JsonPropertyName("listauditdetectauditminutes")]
        public List<ListAuditDetectAuditMinutes> ListAuditDetectAuditMinutes { get; set; }

        [JsonPropertyName("list_file")]
        public List<AuditMinutesFileModel> ListFile { get; set; }
    }

    public class AuditMinutesFileModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
    }
    public class ListAuditWorkScopeAuditMinutes
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("auditprocess_id")]
        public int? auditprocess_id { get; set; }//Quy trình kiểm toán id
        [JsonPropertyName("bussinessactivities_id")]
        public int? bussinessactivities_id { get; set; }//Hoạt động kiểm toán id
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }//Đơn vị được kiểm toán id
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; } //Đơn vị được kiểm toán
        [JsonPropertyName("auditprocess_name")]
        public string auditprocess_name { get; set; } //Quy trình kiểm toán
        [JsonPropertyName("bussinessactivities_name")]
        public string bussinessactivities_name { get; set; } //Hoạt động kiểm toán
        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("risk_rating")]
        public int? risk_rating { get; set; }//Xếp hạng rủi ro: 1-Cao, 2-Trung bình, 3-Thấp
        
        //[JsonPropertyName("id")]
        //public int? id { get; set; }
        //[JsonPropertyName("auditwork_id")]
        //public int? auditwork_id { get; set; }
        //[JsonPropertyName("auditfacilities_id")]
        //public int? auditfacilities_id { get; set; }//Đơn vị được kiểm toán id
        //[JsonPropertyName("auditfacilities_name")]
        //public string auditfacilities_name { get; set; } //Đơn vị được kiểm toán
        //[JsonPropertyName("audit_detect_code")]
        //public string audit_detect_code { get; set; } //Mã phát hiện

        //[JsonPropertyName("str_classify_audit_detect")]
        //public string str_classify_audit_detect { get; set; }//Phân loại phát hiện

        //[JsonPropertyName("reason")]
        //public string reason { get; set; }//lý do


        //[JsonPropertyName("opinion_audit")]
        //public bool opinion_audit { get; set; }//Ý kiến của ĐVĐKT
        //[JsonPropertyName("year")]
        //public int? year { get; set; }
        //[JsonPropertyName("risk_rating")]
        //public int? risk_rating { get; set; }//Xếp hạng rủi ro: 1-Cao, 2-Trung bình, 3-Thấp
        //[JsonPropertyName("title")]
        //public string title { get; set; }

        //[JsonPropertyName("risk_rating_hight")]
        //public int? risk_rating_hight { get; set; }//tổng Xếp hạng rủi ro Cao
        //[JsonPropertyName("risk_rating_medium")]
        //public int? risk_rating_medium { get; set; }//tổng Xếp hạng rủi ro Trung bình
        //[JsonPropertyName("risk_rating_low")]
        //public int? risk_rating_low { get; set; }//tổng Xếp hạng rủi ro: Thấp
    }
    public class ListStatisticsOfDetections
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }//Đơn vị được kiểm toán id
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; } //Đơn vị được kiểm toán
        [JsonPropertyName("audit_detect_code")]
        public string audit_detect_code { get; set; } //Mã phát hiện

        [JsonPropertyName("str_classify_audit_detect")]
        public string str_classify_audit_detect { get; set; }//Phân loại phát hiện

        [JsonPropertyName("reason")]
        public string reason { get; set; }//lý do


        [JsonPropertyName("opinion_audit_true")]
        public int? opinion_audit_true { get; set; }//Ý kiến của ĐVĐKT
        [JsonPropertyName("opinion_audit_false")]
        public int? opinion_audit_false { get; set; }//Ý kiến của ĐVĐKT

        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("risk_rating")]
        public int? risk_rating { get; set; }//Xếp hạng rủi ro: 1-Cao, 2-Trung bình, 3-Thấp
        [JsonPropertyName("title")]
        public string title { get; set; }

        [JsonPropertyName("risk_rating_hight")]
        public int? risk_rating_hight { get; set; }//tổng Xếp hạng rủi ro Cao
        [JsonPropertyName("risk_rating_medium")]
        public int? risk_rating_medium { get; set; }//tổng Xếp hạng rủi ro Trung bình
        [JsonPropertyName("risk_rating_low")]
        public int? risk_rating_low { get; set; }//tổng Xếp hạng rủi ro: Thấp
    }
    public class ListAuditDetectAuditMinutes
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }
        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }//Đơn vị được kiểm toán id
        [JsonPropertyName("auditfacilities_name")]
        public string auditfacilities_name { get; set; } //Đơn vị được kiểm toán
        [JsonPropertyName("audit_detect_code")]
        public string audit_detect_code { get; set; } //Mã phát hiện

        [JsonPropertyName("str_classify_audit_detect")]
        public string str_classify_audit_detect { get; set; }//Phân loại phát hiện

        [JsonPropertyName("reason")]
        public string reason { get; set; }//lý do


        [JsonPropertyName("opinion_audit")]
        public bool opinion_audit { get; set; }//Ý kiến của ĐVĐKT
        [JsonPropertyName("year")]
        public int? year { get; set; }
        [JsonPropertyName("risk_rating")]
        public int? risk_rating { get; set; }//Xếp hạng rủi ro: 1-Cao, 2-Trung bình, 3-Thấp
        [JsonPropertyName("title")]
        public string title { get; set; }
    }
    public class AuditMinutesCreateModel
    {
        [JsonPropertyName("year")]
        public int year { get; set; }

        [JsonPropertyName("auditwork_id")]
        public int auditwork_id { get; set; }//id của cuộc kiểm toán ở trạng thái "Đã duyệt" thuộc năm đã chọn (status == 3)

        [JsonPropertyName("auditfacilities_id")]
        public int auditfacilities_id { get; set; }//id của đơn vị 

        [JsonPropertyName("status")]
        public int status { get; set; }//1 chưa xác nhận, 2 đã xác nhận

        [JsonPropertyName("other_content")]
        public string other_content { get; set; }//Thông tin khác
    }
    public class AuditMinutesEditModel
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? id { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }//1 chưa xác nhận, 2 đã xác nhận

        [JsonPropertyName("rating")]
        public string rating { get; set; }

        [JsonPropertyName("problem")]
        public string problem { get; set; }

        [JsonPropertyName("idea")]
        public string idea { get; set; }

        [JsonPropertyName("other_content")]
        public string other_content { get; set; }//Thông tin khác
    }
    public class AuditMinutesSearchModel
    {
        [JsonPropertyName("year")]
        public int? year { get; set; }

        [JsonPropertyName("auditwork_id")]
        public int? auditwork_id { get; set; }//id của cuộc kiểm toán ở trạng thái "Đã duyệt" thuộc năm đã chọn (status == 3)

        [JsonPropertyName("auditfacilities_id")]
        public int? auditfacilities_id { get; set; }//id của đơn vị 

        [JsonPropertyName("status")]
        public string status { get; set; }//1 chưa xác nhận, 2 đã xác nhận

        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
    public class DropListRattingTypeModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }
    }
}
