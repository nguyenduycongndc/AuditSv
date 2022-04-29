using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("SYSTEM_CATEGORY")]
    public class SystemCategory
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("ID")]
        public int Id { get; set; }
        [Column("Code")]
        [JsonPropertyName("Code")]
        public string Code { get; set; }
        [Column("Name")]
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        [Column("Status")]
        [JsonPropertyName("Status")]
        public bool Status { get; set; } = true;
        [Column("Deleted")]
        [JsonPropertyName("Deleted")]
        public bool Deleted { get; set; } = false;
        [Column("Description")]
        [JsonPropertyName("Description")]
        public string Description { get; set; }
        [Column("CreateDate")]
        [JsonPropertyName("CreateDate")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Column("UserCreate")]
        [JsonPropertyName("UserCreate")]
        public int? UserCreate { get; set; }
        [Column("LastModified")]
        [JsonPropertyName("LastModified")]
        public DateTime? LastModified { get; set; }
        [Column("ModifiedBy")]
        [JsonPropertyName("ModifiedBy")]
        public int? ModifiedBy { get; set; }
        [Column("DomainId")]
        [JsonPropertyName("DomainId")]
        public int DomainId { get; set; }
        [Column("ParentGroup")]
        [JsonPropertyName("ParentGroup")]
        public string ParentGroup { get; set; }
    }
}
