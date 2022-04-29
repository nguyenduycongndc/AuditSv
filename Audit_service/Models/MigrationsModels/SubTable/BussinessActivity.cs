using Audit_service.Models.MigrationsModels.Audit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("BUSSINESS_ACTIVITY")]
    public class BussinessActivity
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int ID { get; set; }
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
        [Column("Deleted")]
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; } = false;
        [Column("Description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [Column("CreateDate")]
        [JsonPropertyName("createDate")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Column("UserCreate")]
        [JsonPropertyName("userCreate")]
        public int? UserCreate { get; set; }
        [Column("LastModified")]
        [JsonPropertyName("lastModified")]
        public DateTime? LastModified { get; set; }
        [Column("ModifiedBy")]
        [JsonPropertyName("modifiedBy")]
        public int? ModifiedBy { get; set; }
        [Column("DomainId")]
        [JsonPropertyName("domainId")]
        public int DomainId { get; set; }
        [Column("ParentId")]
        [JsonPropertyName("parentId")]
        public int? ParentId { get; set; }
    }
}
