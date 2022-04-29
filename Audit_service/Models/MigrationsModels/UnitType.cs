using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("UNIT_TYPE")]
    public class UnitType
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [Column("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }
        [Column("status")]
        [JsonPropertyName("status")]
        public bool? Status { get; set; }
        [Column("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [Column("createdAt")]
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }
        [Column("modifiedAt")]
        [JsonPropertyName("modifiedAt")]
        public DateTime? ModifiedAt { get; set; }
        [Column("deletedAt")]
        [JsonPropertyName("deletedAt")]
        public DateTime DeletedAt { get; set; }
        [Column("createdBy")]
        [JsonPropertyName("createdBy")]
        public int? CreatedBy { get; set; }
        [Column("modifiedBy")]
        [JsonPropertyName("modifiedBy")]
        public int? ModifiedBy { get; set; }
        [Column("deletedBy")]
        [JsonPropertyName("deletedBy")]
        public int? DeletedBy { get; set; }
    }
}