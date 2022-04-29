using System;
using System.Collections.Generic;
using System.Linq;
using Audit_service.DataAccess;
using Audit_service.Models.ExecuteModels;
using Audit_service.Models.MigrationsModels;
using Audit_service.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;
using Audit_service.Models.ExecuteModels.Audit;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class AuditAssignmentController : BaseController
    {
        public AuditAssignmentController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditAssignmentSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                Expression<Func<AuditAssignment, bool>> filter = c => ((obj.auditwork_id == null) || c.auditwork_id.Equals(obj.auditwork_id))
                                               && c.IsActive == true && c.IsDeleted != true;
                var list_auditwork = _uow.Repository<AuditAssignment>().Find(filter);
                IEnumerable<AuditAssignment> data = list_auditwork;
                var count = list_auditwork.Count();

                var _data = _uow.Repository<AuditAssignment>().Include(x => x.Users);
                var lst = _data.Select(a => new AuditAssignmentModel()
                {
                    Id = a.Id,
                    user_id = a.user_id,
                    email = a.Users.Email,
                    auditwork_id = a.auditwork_id,
                    IsActive = a.IsActive,
                    IsDeleted = a.IsDeleted,
                });
                return Ok(new { code = "1", msg = "success", data = lst, total = count });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var AuditAssignment = _uow.Repository<AuditAssignment>().FirstOrDefault(a => a.Id == id);
                if (AuditAssignment == null)
                {
                    return NotFound();
                }

                AuditAssignment.IsDeleted = true;
                _uow.Repository<AuditAssignment>().Update(AuditAssignment);
                return Ok(new { code = "1",id= AuditAssignment.auditwork_id, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}