using Audit_service.DataAccess;
using Audit_service.Models.ExecuteModels;
using Audit_service.Models.ExecuteModels.Audit;
using Audit_service.Models.ExecuteModels.Evaluation;
using Audit_service.Models.MigrationsModels;
using Audit_service.Models.MigrationsModels.Audit;
using Audit_service.Repositories;
using KitanoUserService.API.Models.MigrationsModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;


namespace Audit_service.Controllers.Evaluation
{
    [Route("[controller]")]
    [ApiController]
    public class EvaluationStandardController : BaseController
    {
        public EvaluationStandardController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }

        [HttpPost]
        public IActionResult UpdateItem(EvaluationStandardModel obj)
        {
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }


            var checkByCode = _uow.Repository<EvaluationStandard>().Find(p => p.code.ToLower().Trim().Equals(obj.code.ToLower().Trim()) && (p.isdelete ?? false) == false && p.id != obj.id).FirstOrDefault();
            if (checkByCode != null)
                return Ok(new { code = "003", msg = "success", data = obj });

            if (obj.id > 0)
            {
                EvaluationStandard item = _uow.Repository<EvaluationStandard>().FirstOrDefault(o => o.id == obj.id);

                item.code = obj.code;
                item.status = obj.status;
                item.title = obj.title;
                item.request = obj.request;
                item.modified_by = _userInfo.Id;
                item.modified_at = DateTime.Now;
                _uow.Repository<EvaluationStandard>().Update(item);
            }
            else
            {
                EvaluationStandard item = new EvaluationStandard();
                item.code = obj.code;
                item.status = obj.status;
                item.title = obj.title;
                item.request = obj.request;
                item.created_by = _userInfo.Id;
                item.created_at = DateTime.Now;
                _uow.Repository<EvaluationStandard>().Add(item);

            }
            return Ok(new { code = "001", msg = "success", data = obj });
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<EvaluationStandardSearchModel>(jsonData);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            var query = _uow.Repository<EvaluationStandard>().Find(e => (e.isdelete ?? false) == false
            && (string.IsNullOrEmpty(obj.title) || e.title.ToLower().Trim().Contains(obj.title.ToLower().Trim()))
            && (string.IsNullOrEmpty(obj.request) || e.request.ToLower().Trim().Contains(obj.request.ToLower().Trim()))).OrderBy(o => o.code).Select(e => new EvaluationStandardModel
            {
                id = e.id,
                code = e.code,
                title = e.title,
                status = e.status,
                request = e.request,
            });
            var count = query.Count();

            var result = new List<EvaluationStandardModel>();
            if (obj.StartNumber >= 0 && obj.PageSize > 0)
                result = query.Skip(obj.StartNumber).Take(obj.PageSize).ToList();

            return Ok(new { code = "001", msg = "success", data = result, total = count });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var item = _uow.Repository<EvaluationStandard>().FirstOrDefault(o => o.id == id);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            var result = new EvaluationStandardModel();
            result.id = item.id;
            result.code = item.code;
            result.title = item.title;
            result.request = item.request;
            result.status = item.status;
            if (item.created_by != null)
            {
                result.created_by = _uow.Repository<Users>().FirstOrDefault(p => p.Id == item.created_by).FullName ?? "";
                result.created_at = item.created_at.HasValue ? item.created_at.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
            }
            if (item.modified_by != null)
            {
                result.modified_by = _uow.Repository<Users>().FirstOrDefault(p => p.Id == item.modified_by).FullName ?? "";
                result.modified_at = item.modified_at.HasValue ? item.modified_at.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
            }

            return Ok(new { code = "001", msg = "success", data = result });
        }

        [HttpPost("Delete")]
        public IActionResult DeleteItem(EvaluationStandardModel obj)
        {
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }

            var item = _uow.Repository<EvaluationStandard>().FirstOrDefault(o => o.id == obj.id);

            //Không được xóa khi nếu đã được dùng để đánh giá chất lượng
            //
            var evaluation = _uow.Repository<EvaluationCompliance>().Find(p => (p.isdelete ?? false) == false && p.evaluation_standard_id == item.id);
            if (evaluation.Any())
            {
                return Ok(new { code = "007", msg = "success", data = obj });
            }
            item.isdelete = true;
            item.deleted_by = _userInfo.Id;
            item.deleted_at = DateTime.Now;
            _uow.Repository<EvaluationStandard>().Update(item);
            return Ok(new { code = "001", msg = "success", data = obj });
        }
    }
}
