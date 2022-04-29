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
    public class EvaluationScaleController : BaseController
    {
        public EvaluationScaleController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }

        [HttpPost]
        public IActionResult UpdateItem(EvaluationScaleModel obj)
        {
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }


            var checkByCode = _uow.Repository<EvaluationScale>().Find(p => p.point == obj.point && (p.isdelete ?? false) == false && p.id != obj.id).FirstOrDefault();
            if (checkByCode != null)
                return Ok(new { code = "002", msg = "success", data = obj });

            if (obj.id > 0)
            {
                EvaluationScale item = _uow.Repository<EvaluationScale>().FirstOrDefault(o => o.id == obj.id);

                item.description = obj.description;
                item.status = obj.status;
                item.point = obj.point;
                item.modified_by = _userInfo.Id;
                item.modified_at = DateTime.Now;
                _uow.Repository<EvaluationScale>().Update(item);
            }
            else
            {
                EvaluationScale item = new EvaluationScale();
                item.description = obj.description;
                item.status = obj.status;
                item.point = obj.point;
                item.created_by = _userInfo.Id;
                item.created_at = DateTime.Now;
                _uow.Repository<EvaluationScale>().Add(item);

            }
            return Ok(new { code = "001", msg = "success", data = obj });
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<EvaluationScaleSearchModel>(jsonData);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            var point = string.IsNullOrEmpty(obj.key) ? 0 : double.Parse(obj.key);
            var query = _uow.Repository<EvaluationScale>().Find(e => (e.isdelete ?? false) == false
            && (obj.status == -1 || e.status == obj.status)
            && (string.IsNullOrEmpty(obj.key) ? true : e.point == point)).OrderBy(o => o.point).Select(e => new EvaluationScaleModel
            {
                id = e.id,
                description = e.description,
                status = e.status,
                point = e.point,
            });
            var count = query.Count();

            var result = new List<EvaluationScaleModel>();
            if (obj.StartNumber >= 0 && obj.PageSize > 0)
                result = query.Skip(obj.StartNumber).Take(obj.PageSize).ToList();

            return Ok(new { code = "001", msg = "success", data = result, total = count });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var item = _uow.Repository<EvaluationScale>().FirstOrDefault(o => o.id == id);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            var result = new EvaluationScaleModel();
            result.id = item.id;
            result.description = item.description;
            result.status = item.status;
            result.point = item.point;
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
        public IActionResult DeleteItem(EvaluationScaleModel obj)
        {
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }

            var item = _uow.Repository<EvaluationScale>().FirstOrDefault(o => o.id == obj.id);

            //Không được xóa khi nếu đã được dùng để đánh giá chất lượng
            //
            var evaluation = _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Find(p => (p.isdelete ?? false) == false && p.evaluation_scale_id == item.id);
            if (evaluation.Any())
            {
                return Ok(new { code = "007", msg = "success", data = obj });
            }
            item.isdelete = true;
            item.deleted_by = _userInfo.Id;
            item.deleted_at = DateTime.Now;
            _uow.Repository<EvaluationScale>().Update(item);
            return Ok(new { code = "001", msg = "success", data = obj });
        }
    }
}
