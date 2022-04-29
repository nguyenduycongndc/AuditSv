using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Audit_service.Models.ExecuteModels
{
    public class CatAuditProceduresModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }
        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }
        [JsonPropertyName("processid")]
        public int? Processid { get; set; }
        [JsonPropertyName("status")]
        public int? Status { get; set; }
        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("cat_control_id")]
        public int? cat_control_id { get; set; }
    }
    public class CatAuditProceduresSearchModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("unitid")]
        public string Unitid { get; set; }
        [JsonPropertyName("activationid")]
        public string Activationid { get; set; }
        [JsonPropertyName("processid")]
        public string Processid { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("cat_control_id")]
        public int? cat_control_id { get; set; }
    }

    public class CatAuditProceduresModifiModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }
        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }
        [JsonPropertyName("processid")]
        public int? Processid { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("cat_control_id")]
        public int? cat_control_id { get; set; }
    }
    public class CatAuditProceduresCreateModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }
        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }
        [JsonPropertyName("processid")]
        public int? Processid { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("cat_control_id")]
        public int? cat_control_id { get; set; }
    }

    public class CatAuditProceduresDetailModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }
        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }
        [JsonPropertyName("processid")]
        public int? Processid { get; set; }
        [JsonPropertyName("status")]

        public int? Status { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("unitname")]
        public string Unitname { get; set; }
        //[JsonPropertyName("activationid")]
        //public string Activationname { get; set; }
        //[JsonPropertyName("processid")]
        //public string Processname { get; set; }
        [JsonPropertyName("cat_control_id")]
        public int? cat_control_id { get; set; }
        [JsonPropertyName("controlname")]
        public string ControlName { get; set; }

        [JsonPropertyName("createdate")]
        public string Createdate { get; set; }

        [JsonPropertyName("createdby")]
        public int? CreatedBy { get; set; }

        [JsonPropertyName("editby")]
        public int? Editby { get; set; }

        [JsonPropertyName("editdate")]
        public string Editdate { get; set; }

        [JsonPropertyName("createname")]
        public string createname { get; set; }

        [JsonPropertyName("editname")]
        public string editname { get; set; }
    }

    public class CatAuditProceduresModifyModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("unitid")]
        public int? Unitid { get; set; }
        [JsonPropertyName("activationid")]
        public int? Activationid { get; set; }
        [JsonPropertyName("processid")]
        public int? Processid { get; set; }
        [JsonPropertyName("status")]

        public int? Status { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("relatestep")]
        public string RelateStep { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("unitname")]
        public string Unitname { get; set; }
        //[JsonPropertyName("activationid")]
        //public string Activationname { get; set; }
        //[JsonPropertyName("processid")]
        //public string Processname { get; set; }
        [JsonPropertyName("cat_control_id")]
        public int? cat_control_id { get; set; }
        [JsonPropertyName("controlname")]
        public string ControlName { get; set; }
    }

}