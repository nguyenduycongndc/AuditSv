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
    public class EvaluationController : BaseController
    {
        public EvaluationController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }

        private void FillParent(EvaluationModel obj, int? year, int? audit, int? stage)
        {
            if (obj.evaluation_criteria_parent != null && obj.evaluation_criteria_parent != 0)
            {
                var p = _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Include(p => p.EvaluationScale).FirstOrDefault(a => a.year == year && ((stage ?? 3) == 3 ? a.audit_id == audit : a.stage == stage) && a.evaluation_criteria_id == obj.evaluation_criteria_parent && !(obj.isdelete ?? false));
                if (p != null)
                {
                    var pm = new EvaluationModel
                    {
                        id = p.id,
                        evaluation_criteria_name = p.evaluation_criteria_name,
                        evaluation_criteria_id = p.evaluation_criteria_id,
                        evaluation_criteria_parent = p.evaluation_criteria_parent,
                        evaluation_scale_point = p.evaluation_scale_point.HasValue ? p.evaluation_scale_point.Value.ToString() : "",
                    };
                    FillParent(pm, year, audit, stage);
                    obj.Parent = pm;
                }
            }
        }

        private EvaluationModel GetParent(EvaluationModel obj)
        {
            var item = obj;
            while (item.Parent != null)
                item = item.Parent;
            return item;
        }

        private List<EvaluationModel> GetChilds(EvaluationModel root, List<EvaluationModel> data)
        {
            var childs = new List<EvaluationModel>();
            if (root == null)
                return null;
            data.ForEach(o =>
            {
                var currentObj = o;
                var parent = o.Parent;
                while (parent != null && parent.evaluation_criteria_id != root.evaluation_criteria_id)
                {
                    currentObj = parent;
                    parent = parent.Parent;
                }

                if (parent != null && !childs.Exists(a => a.evaluation_criteria_id == currentObj.evaluation_criteria_id))
                {
                    childs.Add(currentObj);
                }

            });
            childs.ForEach(o =>
            {
                o.Childs = GetChilds(o, data);
                o.has_child = o.Childs.Any() ? true : false;
            });

            return childs;
        }

        private void ClearCycle(EvaluationModel o)
        {
            o.Parent = null;
            o.Childs.ForEach(a => ClearCycle(a));
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<EvaluationSearchModel>(jsonData);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            var evaluation = _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().FirstOrDefault(p => p.year == obj.year && ((obj.stage ?? 3) == 3 ? p.audit_id == obj.audit_id : p.stage == obj.stage));
            if (evaluation == null)
            {
                return Ok(new { code = "002" });
            }
            var query = _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Include(p => p.EvaluationScale).Where(e => e.year == obj.year && ((obj.stage ?? 3) == 3 ? e.audit_id == obj.audit_id : e.stage == obj.stage) && (e.isdelete ?? false) == false).OrderBy(o => o.evaluation_criteria_parent ?? 0).ThenBy(o => o.evaluation_criteria_name).Select(e => new EvaluationModel
            {
                id = e.id,
                evaluation_criteria_name = e.evaluation_criteria_name,
                evaluation_criteria_id = e.evaluation_criteria_id,
                evaluation_criteria_parent = e.evaluation_criteria_parent,
                evaluation_scale_point = e.evaluation_scale_point.HasValue ? e.evaluation_scale_point.Value.ToString() : "",
                stage = e.stage,
            });
            var count = query.Count();

            var result = new List<EvaluationModel>();
            //if (obj.StartNumber >= 0 && obj.PageSize > 0)
            result = query/*.Skip(obj.StartNumber).Take(obj.PageSize)*/.ToList();
            var roots = new List<EvaluationModel>();

            result.ForEach(o =>
            {
                o.Childs.Clear();
                FillParent(o, obj.year, obj.audit_id, o.stage);

                var root = GetParent(o);
                if (!roots.Exists(a => a.id == root.id))
                    roots.Add(root);
            });

            roots.ForEach(o =>
            {
                o.Childs = GetChilds(o, result);
                o.has_child = o.Childs.Any() ? true : false;
                ClearCycle(o);
            });

            return Ok(new { code = "001", msg = "success", data = roots, total = count });
        }

        [HttpPost("InitEvaluation")]
        public IActionResult InitBoard(EvaluationSearchModel obj)
        {
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }

            var listevaluation = new List<Audit_service.Models.MigrationsModels.Evaluation>();
            var evaluationcriteria = _uow.Repository<EvaluationCriteria>().Find(p => p.status == 1 && (p.isdelete ?? false) == false);
            foreach (var item in evaluationcriteria)
            {
                Audit_service.Models.MigrationsModels.Evaluation evaluation = new Models.MigrationsModels.Evaluation();
                if (obj.audit_id != null)
                    evaluation.audit_id = obj.audit_id;
                evaluation.stage = obj.stage;
                evaluation.year = obj.year;
                evaluation.evaluation_criteria_id = item.id;
                evaluation.evaluation_criteria_parent = item.parent_id;
                evaluation.evaluation_criteria_name = item.name;
                evaluation.created_at = DateTime.Now;
                evaluation.created_by = _userInfo.Id;
                listevaluation.Add(evaluation);
            }

            _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Insert(listevaluation);
            _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Save();

            return Ok(new { code = "001" });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var item = _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Include(p => p.EvaluationScale).FirstOrDefault(o => o.id == id);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            if (item != null)
            {
                var result = new EvaluationModel();
                result.id = item.id;
                result.evaluation_scale_point = item.evaluation_scale_point.HasValue ? item.evaluation_scale_point.Value.ToString() : "";
                result.evaluation_scale_id = item.evaluation_scale_id;
                result.plan = item.plan;
                result.explain = item.explain;
                result.actual = item.actual;
                result.evaluation_criteria_name = item.evaluation_criteria_name;
                var point = _uow.Repository<EvaluationScale>().Find(p => p.status == 1 && (p.isdelete ?? false) == false).OrderBy(p => p.point).ToList();
                return Ok(new { code = "001", msg = "success", data = result, point = point });
            }
            return NotFound();
        }

        [HttpPost("UpdatePoint")]
        public IActionResult UpdatePoint(EvaluationModel obj)
        {
            var item = _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().FirstOrDefault(o => o.id == obj.id);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            if (item != null)
            {
                var point = _uow.Repository<EvaluationScale>().FirstOrDefault(p => p.id == obj.evaluation_scale_id);
                if (point == null)
                    return NotFound();
                item.plan = obj.plan;
                item.actual = obj.actual;
                item.evaluation_scale_id = obj.evaluation_scale_id;
                item.evaluation_scale_point = point.point;
                item.explain = obj.explain;
                _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Update(item);
                return Ok(new { code = "001" });
            }

            return NotFound();
        }

        [HttpPost("RefreshEvaluation")]
        public IActionResult RefreshEvaluation(EvaluationSearchModel obj)
        {
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }

            var listevaluation_new = new List<Audit_service.Models.MigrationsModels.Evaluation>();
            var listevaluation_old = new List<Audit_service.Models.MigrationsModels.Evaluation>();
            var evaluation_current = _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Find(p => p.year == obj.year && ((obj.stage ?? 3) == 3 ? p.audit_id == obj.audit_id : p.stage == obj.stage) && (p.isdelete ?? false) == false).ToList();
            var list_criteria_id = evaluation_current.Select(p => p.evaluation_criteria_id.Value);
            var evaluationcriteria = _uow.Repository<EvaluationCriteria>().GetAll(p => (p.status == 1 && (p.isdelete ?? false) == false) || list_criteria_id.Contains(p.id));

            foreach (var item in evaluationcriteria)
            {
                var checkitem = evaluation_current.FirstOrDefault(p => p.evaluation_criteria_id == item.id);
                if (checkitem != null) // update if exist
                {
                    checkitem.evaluation_criteria_id = item.id;
                    checkitem.evaluation_criteria_parent = item.parent_id;
                    checkitem.evaluation_criteria_name = item.name;
                    checkitem.modified_at = DateTime.Now;
                    checkitem.modified_by = _userInfo.Id;
                    if (item.status != 1 || (item.isdelete ?? false) == true)
                    {
                        checkitem.isdelete = true;
                    }
                    listevaluation_old.Add(checkitem);
                }
                else
                {

                    Audit_service.Models.MigrationsModels.Evaluation evaluation = new Models.MigrationsModels.Evaluation();
                    if (obj.audit_id != null)
                        evaluation.audit_id = obj.audit_id;
                    evaluation.stage = obj.stage;
                    evaluation.year = obj.year;
                    evaluation.evaluation_criteria_id = item.id;
                    evaluation.evaluation_criteria_parent = item.parent_id;
                    evaluation.evaluation_criteria_name = item.name;
                    evaluation.created_at = DateTime.Now;
                    evaluation.created_by = _userInfo.Id;
                    listevaluation_new.Add(evaluation);

                }
            }
            _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Update(listevaluation_old);
            _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Insert(listevaluation_new);
            _uow.Repository<Audit_service.Models.MigrationsModels.Evaluation>().Save();

            return Ok(new { code = "001" });
        }
    }
}
