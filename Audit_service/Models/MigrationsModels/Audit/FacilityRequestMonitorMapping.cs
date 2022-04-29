using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("FACILITY_REQUEST_MONITOR_MAPPING")]
    public class FacilityRequestMonitorMapping
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("audit_request_monitor_id")]
        public int audit_request_monitor_id { get; set; }

        [ForeignKey("audit_request_monitor_id")]
        public AuditRequestMonitor AuditRequestMonitor { get; set; }

        [JsonPropertyName("audit_facility_id")]
        public int audit_facility_id { get; set; }

        [ForeignKey("audit_facility_id")]
        public AuditFacility AuditFacility { get; set; }

        [JsonPropertyName("audit_facility_name")]
        public string audit_facility_name { get; set; }
        [JsonPropertyName("type")]
        public int type { get; set; }//1 là Đơn vị đầu mối, 2 là Đơn vị phối hợp
        [JsonPropertyName("comment")]
        public string comment { get; set; }

        [JsonPropertyName("process_status")]
        public string process_status { get; set; }//1 là Đơn vị đầu mối, 2 là Đơn vị phối hợp
    }
}
