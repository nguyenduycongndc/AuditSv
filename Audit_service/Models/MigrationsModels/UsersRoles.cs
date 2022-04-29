using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("USERS_ROLES")]
    public class UsersRoles
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("users_id")]
        public int? users_id { get; set; }
      
        [ForeignKey("users_id")]
        public virtual Users Users { get; set; }

        [JsonPropertyName("roles_id")]
        public int? roles_id { get; set; }
       
        [ForeignKey("roles_id")]
        public virtual Roles Roles { get; set; }
    }
}
