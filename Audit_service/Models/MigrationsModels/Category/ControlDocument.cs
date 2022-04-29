using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("CONTROL_DOCUMENT")]
    public class ControlDocument
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }        

        [JsonPropertyName("controlid")]
        public int controlid { get; set; }
        [ForeignKey("controlid")]
        public virtual CatControl CatControl { get; set; }

        
        [JsonPropertyName("documentid")]
        public Guid documentid { get; set; }
        [ForeignKey("documentid")]
        public virtual Document Document { get; set; }

        [Column("isdeleted")]
        [JsonPropertyName("isdeleted")]
        public bool? isdeleted { get; set; }

        [Column("flag")]
        [JsonPropertyName("flag")]
        public int? flag { get; set; }

    }
}
