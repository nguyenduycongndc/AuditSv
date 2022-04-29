using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
	[Table("USERS")]
	public class Users
    {
        public Users()
        {
            this.UsersGroupMappings = new HashSet<UsersGroupMapping>();
            this.UsersRoles = new HashSet<UsersRoles>();
            this.AuditWorkScopeUserMapping = new HashSet<AuditWorkScopeUserMapping>();
            this.AuditMinutes = new HashSet<AuditMinutes>();
        }
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }
       
        [Column("full_name")]
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [Column("user_name")]
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        [Column("users_type")]
        [JsonPropertyName("users_type")]
        public int? UsersType { get; set; }

        [Column("department_id")]
        [JsonPropertyName("department_id")]
        public int? DepartmentId { get; set; }

        [Column("date_of_joining")]
        [JsonPropertyName("date_of_joining")]
        public DateTime? DateOfJoining { get; set; }

        [Column("is_active")]
        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [Column("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Column("password")]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [Column("salt")]
        [JsonPropertyName("salt")]
        public string SaltKey { get; set; }

        [Column("avartar")]
        [JsonPropertyName("avartar")]
        public string Avartar { get; set; }

        [Column("role_id")]
        [JsonPropertyName("role_id")]
        public int? RoleId { get; set; }

        [Column("last_online_at")]
        [JsonPropertyName("last_online_at")]
        public DateTime? LastOnline { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("modified_at")]
        [JsonPropertyName("modified_at")]
        public DateTime? ModifiedAt { get; set; }

        [Column("deleted_at")]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("domain_Id")]
        [JsonPropertyName("domain_Id")]
        public int? DomainId { get; set; }
        public virtual ICollection<UsersGroupMapping> UsersGroupMappings { get; set; }
        public virtual ICollection<UsersRoles> UsersRoles { get; set; }
        public virtual ICollection<AuditWorkScopeUserMapping> AuditWorkScopeUserMapping { get; set; }
        public virtual ICollection<AuditMinutes> AuditMinutes { get; set; }
    }
}
