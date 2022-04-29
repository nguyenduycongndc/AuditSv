using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("USERS_GROUP_MAPPING")]
    public class UsersGroupMapping
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("users_id")]
        public int? users_id { get; set; }
       
        [ForeignKey("users_id")]
        public Users Users { get; set; }

        [JsonPropertyName("group_id")]
        public int? group_id { get; set; }
       
        [ForeignKey("group_id")]
        public UsersGroup UsersGroup { get; set; }
    }
}
