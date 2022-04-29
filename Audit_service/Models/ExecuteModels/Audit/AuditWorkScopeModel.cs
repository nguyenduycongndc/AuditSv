using Audit_service.DataAccess;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels.Audit
{
    public class AuditWorkScopeUnitModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }
    }
    public class AuditWorkScopeProcessModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }        

        [JsonPropertyName("name")]
        public string name { get; set; }
    }
}
