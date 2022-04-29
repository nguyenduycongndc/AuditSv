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

namespace Audit_service.Controllers.Category
{
    [Route("[controller]")]
    [ApiController]
    public class ControlDocumentController : BaseController
    {
        public ControlDocumentController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }

        [HttpGet("GetDocument/{id}")]
        public IActionResult SearchDocument(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                Expression<Func<ControlDocument, bool>> _filter = c => (id == 0 || c.controlid == id) && (c.isdeleted !=true);
                var _list_document = _uow.Repository<ControlDocument>().Include(a => a.Document).Where(_filter).Select(a => new DocumentModel()
                {
                    controldocumentid = a.Id,
                    Id = a.Document.id,
                    name = a.Document.name,
                    Code = a.Document.code
                });

                return Ok(new { code = "1", msg = "success", data = _list_document });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("GetNotSelectDocument/{id}")]
        public IActionResult SearchNotSelectDocument(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                Expression<Func<ControlDocument, bool>> _filter = c => (id == 0 || c.controlid == id);
                var _list_document = _uow.Repository<ControlDocument>().Include(a => a.Document).Where(_filter).Select(a => new DocumentModel()
                {
                    Id = a.Document.id,
                    name = a.Document.name,
                    Code = a.Document.code
                });

                return Ok(new { code = "1", msg = "success", data = _list_document });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var _risk = _uow.Repository<ControlDocument>().FirstOrDefault(a => a.Id == id);
                _risk.flag = 1;
                _uow.Repository<ControlDocument>().Update(_risk);
                return Ok(new { code = "1", msg = "success" });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}