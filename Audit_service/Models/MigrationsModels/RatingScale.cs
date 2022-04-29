using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.MigrationsModels
{
    [Table("RATING_SCALE")]
    public class RatingScale
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(254)]
        public string Code { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public bool Status { get; set; } = true;
        public bool Deleted { get; set; } = false;
        public string Description { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int? UserCreate { get; set; }
        public DateTime? LastModified { get; set; }
        public int? ModifiedBy { get; set; }
        public int DomainId { get; set; }

        public int RiskLevel { get; set; }
        public string RiskLevelName { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string MinFunction { get; set; }
        public string MaxFunction { get; set; }
        public string ApplyFor { get; set; }
    }
}
