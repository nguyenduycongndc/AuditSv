using Audit_service.DataAccess;
using Audit_service.Models.ExecuteModels;
using Audit_service.Models.ExecuteModels.Audit;
using Audit_service.Models.MigrationsModels;
using Audit_service.Repositories;
using KitanoUserService.API.Models.MigrationsModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Audit_service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigDocumentController : BaseController
    {
        protected readonly IConfiguration _config;
        public ConfigDocumentController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }
        [HttpGet("Search")]
        public IActionResult Search()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var list_menu = _uow.Repository<Menu>().Include(a => a.ConfigDocument).Where(a => a.IsConfigDocument == true && a.IsDeleted != true).ToArray();
                var list_config = list_menu.Select(a => new ConfigDocumentModel()
                {
                    item_id = a.Id,
                    item_code = a.CodeName,
                    item_name = a.Description,
                });
                return Ok(new { code = "1", msg = "success", data = list_config });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //chi tiết ConfigDocument
        [HttpGet("ConfigDocumentCollapse/{id}")]
        public IActionResult ConfigDocumentCollapse(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var _configdocument = _uow.Repository<ConfigDocument>().Find(a => a.item_id == id && a.isShow == true).ToArray();
                var configdocument = _configdocument.Select(a => new ListConfigDocumentModel
                {
                    id = a.id,
                    item_id = a.item_id,
                    item_name = a.item_name,
                    item_code = a.item_code,
                    content = a.content,
                    status = a.status,
                    isShow = a.isShow,
                }).ToList();
                return Ok(new { code = "1", msg = "success", data = configdocument });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("Active")]
        public IActionResult Active(ConfigDocumentActiveModel configDocumentActiveModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkConfigDocument = _uow.Repository<ConfigDocument>().FirstOrDefault(a => a.id == configDocumentActiveModel.id);
                if (checkConfigDocument == null)
                {
                    return NotFound();
                }
                if (configDocumentActiveModel.status == 1)
                {
                    checkConfigDocument.status = true;
                }
                else
                {
                    checkConfigDocument.status = false;
                }
                _uow.Repository<ConfigDocument>().Update(checkConfigDocument);
                return Ok(new { code = "1", msg = "success", data = checkConfigDocument });
            }
            catch (Exception ex)
            {
                _logger.LogError($"ACTIVE_USER - {configDocumentActiveModel.id} : {ex.Message}!");
                return BadRequest();
            }
        }
    }
}
