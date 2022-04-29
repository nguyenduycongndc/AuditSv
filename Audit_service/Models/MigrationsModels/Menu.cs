using Audit_service.Models.MigrationsModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.MigrationsModels
{
    [Table("MENU")]
    public class Menu
    {
        public Menu()
        {
            this.ApprovalConfig = new HashSet<ApprovalConfig>();
            this.ConfigDocument = new HashSet<ConfigDocument>();
        }
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("parent_id")]
        [JsonPropertyName("parent_id")]
        public int? ParentID { get; set; }

        [Column("code_name")]
        [JsonPropertyName("code_name")]
        public string CodeName { get; set; }

        [Column("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [Column("controller")]
        [JsonPropertyName("controller")]
        public string Controller { get; set; }

        [Column("action")]
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [Column("default_url")]
        [JsonPropertyName("default_url")]
        public string DefaultUrl { get; set; }

        [Column("icon")]
        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [Column("ancestor")]
        [JsonPropertyName("ancestor")]
        public string Ancestor { get; set; }

        [Column("sort_order")]
        [JsonPropertyName("sort_order")]
        public int? SortOrder { get; set; }

        [Column("is_menu")]
        [JsonPropertyName("is_menu")]
        public bool? IsMenu { get; set; }

        [Column("is_active")]
        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [Column("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [Column("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("created_by")]
        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }

        [Column("modified_at")]
        [JsonPropertyName("modified_at")]
        public DateTime? ModifiedAt { get; set; }

        [Column("modified_by")]
        [JsonPropertyName("modified_by")]
        public int? ModifiedBy { get; set; }

        [Column("deleted_at")]
        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("deleted_by")]
        [JsonPropertyName("deleted_by")]
        public int? DeletedBy { get; set; }

        [Column("is_config")]
        [JsonPropertyName("is_config")]
        public bool? IsConfigApproval { get; set; }

        [Column("is_config_document")]
        [JsonPropertyName("is_config_document")]
        public bool? IsConfigDocument { get; set; }

        public virtual ICollection<ApprovalConfig> ApprovalConfig { get; set; }
        public virtual ICollection<ConfigDocument> ConfigDocument { get; set; }
    }
}
