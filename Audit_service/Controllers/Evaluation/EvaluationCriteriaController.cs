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
    public class EvaluationCriteriaController : BaseController
    {
        public EvaluationCriteriaController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }


        private void FillParent(EvaluationCriteriaModel obj)
        {
            if (obj.parent_id != null)
            {
                var p = _uow.Repository<EvaluationCriteria>().FirstOrDefault(a => a.id == obj.parent_id && !(obj.isdelete ?? false));
                if (p != null)
                {
                    var pm = new EvaluationCriteriaModel
                    {
                        id = p.id,
                        name = p.name,
                        code = p.code,
                        description = p.description,
                        status = p.status,
                        parent_id = p.parent_id,
                    };
                    FillParent(pm);
                    obj.Parent = pm;
                }
            }
        }

        private EvaluationCriteriaModel GetParent(EvaluationCriteriaModel obj)
        {
            var item = obj;
            while (item.Parent != null)
                item = item.Parent;
            return item;
        }

        private List<EvaluationCriteriaModel> GetChilds(EvaluationCriteriaModel root, List<EvaluationCriteriaModel> data)
        {
            var childs = new List<EvaluationCriteriaModel>();
            if (root == null)
                return null;
            data.ForEach(o =>
            {
                var currentObj = o;
                var parent = o.Parent;
                while (parent != null && parent.id != root.id)
                {
                    currentObj = parent;
                    parent = parent.Parent;
                }

                if (parent != null && !childs.Exists(a => a.id == currentObj.id))
                {
                    childs.Add(currentObj);
                }

            });
            var ancestor_parent = root.ancestor;
            childs.ForEach(o =>
            {
                o.ancestor = ancestor_parent + ">" + "|" + o.id + "|";
                o.Childs = GetChilds(o, data);
            });

            return childs;
        }

        private void ClearCycle(EvaluationCriteriaModel o)
        {
            o.Parent = null;
            o.Childs.ForEach(a => ClearCycle(a));
        }

        private void ActiveByParent(int itemId, int status, CurrentUserModel userInfo)
        {
            var c = _uow.Repository<EvaluationCriteria>().FirstOrDefault(o => o.id == itemId);
            if (c == null)
                return;
            c.status = status;
            c.modified_by = userInfo.Id;
            c.modified_at = DateTime.Now;
            _uow.Repository<EvaluationCriteria>().Update(c);
            var childs = _uow.Repository<EvaluationCriteria>().Find(o => o.parent_id == c.id && o.status == 1).ToList();
            foreach (var cd in childs)
            {
                ActiveByParent(cd.id, status, userInfo);
            }
        }

        [HttpPost]
        public IActionResult UpdateItem(EvaluationCriteriaModel obj)
        {
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }


            var checkByCode = _uow.Repository<EvaluationCriteria>().Find(p => p.code.ToLower().Equals(obj.code.ToLower()) && (p.isdelete ?? false) == false && p.id != obj.id).FirstOrDefault();
            if (checkByCode != null)
                return Ok(new { code = "003", msg = "success", data = obj });



            if (obj.id > 0)
            {
                EvaluationCriteria item = _uow.Repository<EvaluationCriteria>().FirstOrDefault(o => o.id == obj.id);

                item.description = obj.description;
                item.code = obj.code;
                item.name = obj.name;
                item.status = obj.status;
                item.parent_id = obj.parent_id;
                item.modified_by = _userInfo.Id;
                item.modified_at = DateTime.Now;
                _uow.Repository<EvaluationCriteria>().Update(item);
                if (obj.status == 0)
                {
                    ActiveByParent(obj.id, obj.status.Value, _userInfo);
                }
            }
            else
            {
                EvaluationCriteria item = new EvaluationCriteria();
                item.description = obj.description;
                item.code = obj.code;
                item.name = obj.name;
                item.status = obj.status;
                item.parent_id = obj.parent_id;
                item.created_by = _userInfo.Id;
                item.created_at = DateTime.Now;
                _uow.Repository<EvaluationCriteria>().Add(item);

            }
            return Ok(new { code = "001", msg = "success", data = obj });
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<EvaluationCriteriaSearchModel>(jsonData);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }

            var query = _uow.Repository<EvaluationCriteria>().Find(e => (e.isdelete ?? false) == false
            && (obj.status == -1 || e.status == obj.status)
            && (string.IsNullOrEmpty(obj.key) ? true : e.name.ToLower().Trim().Contains(obj.key.ToLower().Trim()))).OrderBy(o => o.parent_id ?? 0).ThenBy(o => o.name).Select(e => new EvaluationCriteriaModel
            {
                id = e.id,
                name = e.name,
                code = e.code,
                description = e.description,
                status = e.status,
                parent_id = e.parent_id,
            });
            var count = query.Count();

            var result = new List<EvaluationCriteriaModel>();
            //if (obj.StartNumber >= 0 && obj.PageSize > 0)
            result = query/*.Skip(obj.StartNumber).Take(obj.PageSize)*/.ToList();
            var roots = new List<EvaluationCriteriaModel>();

            result.ForEach(o =>
            {
                o.Childs.Clear();
                FillParent(o);

                var root = GetParent(o);
                if (!roots.Exists(a => a.id == root.id))
                    roots.Add(root);
            });

            roots.ForEach(o =>
            {
                o.ancestor = "|" + o.id + "|";
                o.Childs = GetChilds(o, result);
                ClearCycle(o);
            });

            return Ok(new { code = "001", msg = "success", data = roots, total = count });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var item = _uow.Repository<EvaluationCriteria>().FirstOrDefault(o => o.id == id);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            var result = new EvaluationCriteriaModel();
            result.id = item.id;
            result.code = item.code;
            result.name = item.name;
            result.description = item.description;
            result.status = item.status;
            result.parent_id = item.parent_id;
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
        public IActionResult DeleteItem(EvaluationCriteriaModel obj)
        {
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }

            var item = _uow.Repository<EvaluationCriteria>().FirstOrDefault(o => o.id == obj.id);
            var child = _uow.Repository<EvaluationCriteria>().Find(o => !(o.isdelete ?? false) && o.parent_id == obj.id).Count();
            if (child > 0)
                return Ok(new { code = "008" });

            //Không được xóa khi nếu đã được dùng để đánh giá chất lượng
            //
            var evaluation = _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Find(p => (p.isdelete ?? false) == false && p.evaluation_criteria_id == item.id);
            if (evaluation.Any())
            {
                return Ok(new { code = "007", msg = "success", data = obj });
            }
            item.isdelete = true;
            item.deleted_by = _userInfo.Id;
            item.deleted_at = DateTime.Now;
            _uow.Repository<EvaluationCriteria>().Update(item);
            return Ok(new { code = "001", msg = "success", data = obj });
        }
    }

}
