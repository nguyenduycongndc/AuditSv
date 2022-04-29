using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("CAT_AUDIT_PROCEDURES")]
    public class CatAuditProcedures
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("code")]
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Column("unitid")]
        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }

        [Column("activationid")]
        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }

        [Column("processid")]
        [JsonPropertyName("processid")]
        public int? Processid { get; set; }

        [Column("status")]
        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [Column("createdate")]
        [JsonPropertyName("createdate")]
        public DateTime? Createdate { get; set; }

        [Column("isdeleted")]
        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [Column("relatestep")]
        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [Column("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("cat_control_id")]
        public int? cat_control_id { get; set; }

        [ForeignKey("cat_control_id")]
        public virtual CatControl CatControl { get; set; }

        [Column("createby")]
        [JsonPropertyName("createby")]
        public int? Createby { get; set; }

        [Column("editby")]
        [JsonPropertyName("editby")]
        public int? Editby { get; set; }

        [Column("editdate")]
        [JsonPropertyName("editdate")]
        public DateTime? Editdate { get; set; }
    }
}
