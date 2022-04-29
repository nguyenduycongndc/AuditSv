using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("AUDIT_PROCESS")]
    public class AuditProcess
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("FacilityId")]
        [JsonPropertyName("facility_id")]
        public int FacilityId { get; set; }

        [Column("ActivityId")]
        [JsonPropertyName("activity_id")]
        public int ActivityId { get; set; }

        [Column("PersonCharge")]
        [JsonPropertyName("person_charge")]
        public string PersonCharge { get; set; }

        [Column("PersonChargeEmail")]
        [JsonPropertyName("person_charge_email")]

        public string PersonChargeEmail { get; set; }

        [MaxLength(254)]
        [Column("Code")]
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [MaxLength(500)]
        [Column("Name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Column("Status")]
        [JsonPropertyName("status")]
        public bool Status { get; set; } = true;

        [Column("Description")]
        [JsonPropertyName("description")] 
        public string Description { get; set; }

        [Column("CreateDate")]
        [JsonPropertyName("create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("UserCreate")]
        [JsonPropertyName("user_create")]
        public int? UserCreate { get; set; }

        [Column("LastModified")]
        [JsonPropertyName("last_modified")]
        public DateTime? LastModified { get; set; }

        [Column("ModifiedBy")]
        [JsonPropertyName("modified_by")]
        public int? ModifiedBy { get; set; }

        [Column("DomainId")]
        [JsonPropertyName("domainid")]
        public int DomainId { get; set; }

        [Column("ActivityName")]
        [JsonPropertyName("activity_name")]
        public string ActivityName { get; set; }

        [Column("FacilityName")]
        [JsonPropertyName("facility_name")]
        public string FacilityName { get; set; }

        [Column("Deleted")]
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; } = false;


    }
}
