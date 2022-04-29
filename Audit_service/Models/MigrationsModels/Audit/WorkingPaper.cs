using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("WORKING_PAPER")]
    public class WorkingPaper
    {
        public WorkingPaper()
        {
            this.AuditObserve = new HashSet<AuditObserve>();
        }

        //id : phân biệt
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        //code : Mã giấy tờ
        [Column("code")]
        [JsonPropertyName("code")]
        public string code { get; set; }

        public HashSet<AuditObserve> AuditObserve { get; }

        //year : Năm
        [Column("year")]
        [JsonPropertyName("year")]
        public int? year { get; set; }

        //auditworkid : id cuộc kiểm toán
        [Column("auditworkid")]
        [JsonPropertyName("auditworkid")]
        public int? auditworkid { get; set; }

        //processid : id quy trình được kiểm toán
        [Column("processid")]
        [JsonPropertyName("processid")]
        public int? processid { get; set; }

        //unitid : id đơn vị
        [Column("unitid")]
        [JsonPropertyName("unitid")]
        public int? unitid { get; set; }

        //status : trạng thái
        [Column("status")]
        [JsonPropertyName("status")]
        public int? status { get; set; }

        //asigneeid : id người thực hiện
        [Column("asigneeid")]
        [JsonPropertyName("asigneeid")]
        public int? asigneeid { get; set; }

        //reviewerid : id người rà soát
        [Column("reviewerid")]
        [JsonPropertyName("reviewerid")]
        public int? reviewerid { get; set; }

        //approvedate : ngày duyệt
        [Column("approvedate")]
        [JsonPropertyName("approvedate")]
        public DateTime? approvedate { get; set; }

        //riskid : id của rủi ro
        [Column("riskid")]
        [JsonPropertyName("riskid")]
        public string riskid { get; set; }

        //prototype : mẫu
        [Column("prototype")]
        [JsonPropertyName("prototype")]
        public string prototype { get; set; }

        //conclusion : Kết luận
        [Column("conclusion")]
        [JsonPropertyName("conclusion")]
        public string conclusion { get; set; }

        [Column("isdelete")]
        [JsonPropertyName("isdelete")]
        public bool? isdelete { get; set; }

        [Column("target")]
        [JsonPropertyName("target")]
        public string target { get; set; }

        [Column("reason_reject")]
        [JsonPropertyName("reason_reject")]
        public string reason_reject { get; set; }

        [Column("create_date")]
        [JsonPropertyName("create_date")]
        public DateTime? create_date { get; set; }

        [NotMapped]
        [JsonPropertyName("risk_str")]
        public List<string> risk_str => !string.IsNullOrEmpty(riskid) ? riskid.Split(',').ToList() : new List<string>();
    }
}
