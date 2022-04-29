using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.MigrationsModels.Category
{
    [Table("CAT_AUDIT_REQUEST")]
    public class CatAuditRequest
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [Column("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [Column("code")]
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [Column("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [Column("status")]
        [JsonPropertyName("status")]
        public bool? Status { get; set; }
        [Column("createdate")]
        [JsonPropertyName("createdate")]
        public DateTime? CreateDate { get; set; }
        [Column("createdat")]
        [JsonPropertyName("createdat")]
        public DateTime? CreatedAt { get; set; }
        [Column("modifiedat")]
        [JsonPropertyName("modifiedat")]
        public DateTime? ModifiedAt { get; set; }
        [Column("deletedat")]
        [JsonPropertyName("deletedat")]
        public DateTime? DeletedAt { get; set; }
        [Column("createdby")]
        [JsonPropertyName("createdby")]
        public int? CreatedBy { get; set; }
        [Column("modifiedby")]
        [JsonPropertyName("modifiedby")]
        public int? ModifiedBy { get; set; }
        [Column("deletedby")]
        [JsonPropertyName("deletedby")]
        public int? DeletedBy { get; set; }
        [Column("is_active")]
        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }
        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }
    }
}