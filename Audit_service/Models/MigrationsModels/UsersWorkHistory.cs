using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("USERS_WORK_HISTORY")]
    public class UsersWorkHistory
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        public int? users_id { get; set; }

        [ForeignKey("users_id")]
        public Users Users { get; set; }

        [Column("department_id")]
        [JsonPropertyName("department_id")]
        public int? DepartmentID { get; set; }

        [Column("date_of_joining")]
        [JsonPropertyName("date_of_joining")]
        public DateTime? DateOfJoining { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("created_by")]
        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

    }
}
