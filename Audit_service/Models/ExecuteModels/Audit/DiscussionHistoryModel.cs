using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class DiscussionHistoryModel
    {
        [JsonPropertyName("item_id")]
        public int item_id { get; set; }
        [JsonPropertyName("item_type")]//Kế hoạch kiểm toán năm(1)-Kế hoạch cuộc kiểm toán (2)-Giấy tờ làm việc(3)-Phát hiện kiểm toán(4)-Báo cáo cuộc kiểm toán (5)-Chương trình kiểm toán (6)
        public int item_type { get; set; }
        [JsonPropertyName("type")]//Freetext
        public string type { get; set; }
        [JsonPropertyName("content")]// Từ chối duyệt: nhập input,Nếu duyệt: (Default: Đã duyệt)
        public string content { get; set; }
        [JsonPropertyName("version")]
        public string version { get; set; }
    }
    public class SearchDiscussionHistoryModel
    {
        [JsonPropertyName("item_id")]
        public int? item_id { get; set; }
        [JsonPropertyName("item_type")]
        public int item_type { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    } 
    public class ListDiscussionHistoryModel
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("item_id")]
        public int? item_id { get; set; }
        [JsonPropertyName("item_name")]
        public string item_name { get; set; }
        [JsonPropertyName("item_code")]
        public string item_code { get; set; }
        [JsonPropertyName("item_type")]
        public int item_type { get; set; }
        [JsonPropertyName("type")]
        public string type { get; set; }
        [JsonPropertyName("content")]
        public string content { get; set; }
        [JsonPropertyName("created_at")]
        public string created_at { get; set; }
        [JsonPropertyName("version")]
        public string version { get; set; }
        [JsonPropertyName("person_perform")]
        public string person_perform { get; set; }
    }
}
