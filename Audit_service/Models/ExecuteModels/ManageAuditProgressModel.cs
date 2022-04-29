using Audit_service.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class EditMainStageModel
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? id { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }//1 chưa xác nhận, 2 đã xác nhận

        [JsonPropertyName("actual_date")]
        public string actual_date { get; set; } //Ngày thực tế thực hiện
    }
    public class DetailMainStageModel
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? id { get; set; }

        [JsonPropertyName("stage")]
        public string stage { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }//1 chưa xác nhận, 2 đã xác nhận

        [JsonPropertyName("actual_date")]
        public string actual_date { get; set; } //Ngày thực tế thực hiện
    }
    public class DetailScheduleModel
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? id { get; set; }

        [JsonPropertyName("actual_date")]
        public string actual_date { get; set; } //Ngày thực tế thực hiện
    }
    public class EditScheduleModel
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? id { get; set; }

        [JsonPropertyName("actual_date")]
        public string actual_date { get; set; } //Ngày thực tế thực hiện
    }
    public class ManageAuditProgressSearchModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("listschedule")]
        public List<ManageAuditProgressListSchedule> ListSchedule { get; set; }//tab 1
        [JsonPropertyName("mainstage")]
        public List<ManageAuditProgressMainStage> MainStage { get; set; }//tab 2
    }
    public class ManageAuditProgressMainStage
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("stage")]
        public string stage { get; set; }//giai đoạn

        [JsonPropertyName("expected_date_mainstage")]
        public string expected_date_mainstage { get; set; }//Ngày dự kiến thực hiện

        [JsonPropertyName("actual_date_mainstage")]
        public string actual_date_mainstage { get; set; }//Ngày thực tế thực hiện

        [JsonPropertyName("status")]
        public string status { get; set; }//trạng thái
    }
    public class ManageAuditProgressListSchedule
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }

        [JsonPropertyName("work")]
        public string work { get; set; }//công việc

        [JsonPropertyName("user_id")]
        public int? user_id { get; set; }

        [JsonPropertyName("user_name")]
        public string user_name { get; set; } // người phụ trách

        [JsonPropertyName("expected_date_schedule")]
        public string expected_date_schedule { get; set; } // ngày dự kiến

        [JsonPropertyName("actual_date_schedule")]
        public string actual_date_schedule { get; set; } //Ngày thực tế thực hiện

    }
}
