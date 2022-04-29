using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("UNIT_COMMENT")]
    public class UnitComment
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        //đây là trường đại diện cho đơn vị phối hợp
        [Column("counitid")]
        [JsonPropertyName("counitid")]
        public int? counitid { get; set; }

        [ForeignKey("counitid")]
        public virtual AuditFacility AuditFacility { get; set; }

        //đây là trường đại diện cho kiến nghị kiểm toán
        [Column("auditequestid")]
        [JsonPropertyName("auditequestid")]
        public int? auditequestid { get; set; }

        [ForeignKey("auditequestid")]
        public virtual AuditRequestMonitor AuditRequestMonitor { get; set; }

        //đây là trường đại diện cho ý kiến của đơn vị phối hợp
        [Column("comment")]
        [JsonPropertyName("comment")]
        public string comment { get; set; }

        //đây là trường đại diện cho ý kiến trạng thái thực hiện
        [Column("processstatus")]
        [JsonPropertyName("processstatus")]
        public int? processstatus { get; set; }
    }
}
