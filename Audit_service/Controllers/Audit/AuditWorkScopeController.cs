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
    public class AuditWorkScopeController : BaseController
    {
        public AuditWorkScopeController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }

        [HttpGet("GetUnit/{id}/{processid}")]
        public IActionResult SearchUnit(int id, int processid)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                Expression<Func<AuditWorkScope, bool>> _filter = c => processid > 0 ? (id == 0 || c.auditprocess_id == processid) && c.IsDeleted != true : (id == 0 || c.auditwork_id == id) && c.IsDeleted != true;
                var _list_Unit = _uow.Repository<AuditWorkScope>().Find(_filter).OrderByDescending(a => a.Id);

                IEnumerable<AuditWorkScope> data = _list_Unit;


                var user = data.Select(a => new AuditWorkScopeUnitModel()
                {
                    id = a.auditfacilities_id,
                    name = a.auditfacilities_name,
                }).GroupBy(a => a.id).Select(a => new AuditWorkScopeUnitModel()
                {
                    id = a.Key,
                    name = a.FirstOrDefault().name,
                });

                return Ok(new { code = "1", msg = "success", data = user });

            }
            catch (Exception)
            {
                return Ok(new { code = "1", msg = "success", data = "" });
            }
        }


        [HttpGet("GetProcess/{id}")]
        public IActionResult SearchProcess(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                Expression<Func<AuditWorkScope, bool>> _filter = c => (id == 0 || c.auditwork_id == id) && c.IsDeleted != true;
                var _list_Unit = _uow.Repository<AuditWorkScope>().Find(_filter).OrderByDescending(a => a.Id);

                IEnumerable<AuditWorkScope> data = _list_Unit;


                var user = data.Select(a => new AuditWorkScopeProcessModel()
                {
                    id = a.auditprocess_id,
                    name = a.auditprocess_name,
                }).GroupBy(a => a.id).Select(a => new AuditWorkScopeUnitModel()
                {
                    id = a.Key,
                    name = a.FirstOrDefault().name,
                });

                return Ok(new { code = "1", msg = "success", data = user });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("GetUnitByAudit/{id}")]
        public IActionResult SearchUnitByAudit(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                Expression<Func<AuditWorkScope, bool>> _filter = c => (id == 0 || c.auditwork_id == id) && c.IsDeleted != true;
                var _list_Unit = _uow.Repository<AuditWorkScope>().Find(_filter).OrderByDescending(a => a.Id);

                IEnumerable<AuditWorkScope> data = _list_Unit;


                var user = data.Select(a => new AuditWorkScopeUnitModel()
                {
                    id = a.auditfacilities_id,
                    name = a.auditfacilities_name,
                }).GroupBy(a => a.id).Select(a => new AuditWorkScopeUnitModel()
                {
                    id = a.Key,
                    name = a.FirstOrDefault()?.name,
                });

                return Ok(new { code = "1", msg = "success", data = user });

            }
            catch (Exception)
            {
                return Ok(new { code = "1", msg = "success", data = "" });
            }
        }
        [HttpGet("GetProcessByUnit/{id}/{unitid}")]
        public IActionResult SearchProcessByUnit(int id,int unitid)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                Expression<Func<AuditWorkScope, bool>> _filter = c => unitid > 0 ? (id == 0 || c.auditwork_id == id) && c.auditfacilities_id == unitid && c.IsDeleted != true : (id == 0 || c.auditwork_id == id) && c.IsDeleted != true;
                var _list_Unit = _uow.Repository<AuditWorkScope>().Find(_filter).OrderByDescending(a => a.Id);

                IEnumerable<AuditWorkScope> data = _list_Unit;


                var user = data.Select(a => new AuditWorkScopeProcessModel()
                {
                    id = a.auditprocess_id,
                    name = a.auditprocess_name,
                }).GroupBy(a => a.id).Select(a => new AuditWorkScopeUnitModel()
                {
                    id = a.Key,
                    name = a.FirstOrDefault()?.name,
                });

                return Ok(new { code = "1", msg = "success", data = user });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
