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
    public class EvaluationComplianceController : BaseController
    {
        public EvaluationComplianceController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }


        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<EvaluationComplianceSearchModel>(jsonData);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            var evaluationcompliance = _uow.Repository<EvaluationCompliance>().FirstOrDefault(p => p.year == obj.year && ((obj.stage ?? 3) == 3 ? p.audit_id == obj.audit_id : p.stage == obj.stage));
            if (evaluationcompliance == null)
            {
                return Ok(new { code = "002" });
            }

            var query = _uow.Repository<EvaluationCompliance>().Include(p => p.EvaluationStandard, p => p.Users).Where(e => e.year == obj.year && ((obj.stage ?? 3) == 3 ? e.audit_id == obj.audit_id : e.stage == obj.stage) && (e.isdelete ?? false) == false).OrderBy(p => p.evaluation_standard_code)
            .Select(e => new EvaluationComplianceModel
            {
                id = e.id,
                evaluation_standard_code = e.evaluation_standard_code,
                evaluation_standard_id = e.evaluation_standard_id,
                evaluation_standard_request = e.evaluation_standard_request,
                evaluation_standard_title = e.evaluation_standard_title,
                compliance = e.compliance
            });
            var count = query.Count();

            var result = new List<EvaluationComplianceModel>();
            if (obj.StartNumber >= 0 && obj.PageSize > 0)
                result = query.Skip(obj.StartNumber).Take(obj.PageSize).ToList();

            return Ok(new { code = "001", msg = "success", data = result, total = count });
        }

        [HttpPost("InitEvaluationCompliance")]
        public IActionResult InitEvaluationCompliance(EvaluationComplianceSearchModel obj)
        {
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }

            var listevaluationcompliance = new List<EvaluationCompliance>();
            var evaluationstandard = _uow.Repository<EvaluationStandard>().Find(p => p.status == 1 && (p.isdelete ?? false) == false);
            foreach (var item in evaluationstandard)
            {
                EvaluationCompliance evaluation = new EvaluationCompliance();
                if (obj.audit_id != null)
                    evaluation.audit_id = obj.audit_id;
                evaluation.stage = obj.stage;
                evaluation.year = obj.year;
                evaluation.evaluation_standard_code = item.code;
                evaluation.evaluation_standard_id = item.id;
                evaluation.evaluation_standard_request = item.request;
                evaluation.evaluation_standard_title = item.title;
                evaluation.compliance = true;
                evaluation.created_at = DateTime.Now;
                evaluation.created_by = _userInfo.Id;
                listevaluationcompliance.Add(evaluation);
            }

            _uow.Repository<EvaluationCompliance>().Insert(listevaluationcompliance);
            _uow.Repository<EvaluationCompliance>().Save();

            return Ok(new { code = "001" });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var item = _uow.Repository<EvaluationCompliance>().Include(p => p.EvaluationStandard, p => p.Users).FirstOrDefault(o => o.id == id);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            if (item != null)
            {
                var result = new EvaluationComplianceModel();
                result.id = item.id;
                result.evaluation_standard_title = item.evaluation_standard_title;
                result.evaluation_standard_request = item.evaluation_standard_request;
                result.compliance = item.compliance;
                result.reason = item.reason;
                result.plan = item.plan;
                result.time = item.time.HasValue ? item.time.Value.ToString("yyyy-MM-dd") : "";
                if (item.reponsible != null)
                {
                    result.reponsible = item.reponsible;
                    result.reponsible_name = item.Users.FullName;
                }
                return Ok(new { code = "001", msg = "success", data = result });
            }
            return NotFound();
        }

        [HttpPost("UpdateCompliance")]
        public IActionResult UpdateCompliance(EvaluationComplianceModel obj)
        {
            var item = _uow.Repository<EvaluationCompliance>().FirstOrDefault(o => o.id == obj.id);
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }
            if (item != null)
            {
                item.compliance = obj.compliance;
                if (!obj.compliance)
                {
                    item.reason = obj.reason;
                    item.plan = obj.plan;
                    item.time = obj.time != "" ? DateTime.Parse(obj.time) : null;
                    item.reponsible = obj.reponsible;
                }
                else
                {
                    item.reason = null;
                    item.plan = null;
                    item.time = null;
                    item.reponsible = null;
                }
                _uow.Repository<EvaluationCompliance>().Update(item);
                return Ok(new { code = "001" });
            }

            return NotFound();
        }

        [HttpPost("RefreshEvaluation")]
        public IActionResult RefreshEvaluation(EvaluationComplianceSearchModel obj)
        {
            if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
            {
                return Unauthorized();
            }

            var listevaluationcompliance_new = new List<EvaluationCompliance>();
            var listevaluationcompliance_old = new List<EvaluationCompliance>();
            var evaluationcompliance_current = _uow.Repository<EvaluationCompliance>().Find(p => p.year == obj.year && ((obj.stage ?? 3) == 3 ? p.audit_id == obj.audit_id : p.stage == obj.stage) && (p.isdelete ?? false) == false).ToList();
            var list_standard_id = evaluationcompliance_current.Select(p => p.evaluation_standard_id.Value);
            var evaluationstandard = _uow.Repository<EvaluationStandard>().GetAll(p => (p.status == 1 && (p.isdelete ?? false) == false) || list_standard_id.Contains(p.id));

            foreach (var item in evaluationstandard)
            {
                var checkitem = evaluationcompliance_current.FirstOrDefault(p => p.evaluation_standard_id == item.id);
                if (checkitem != null) // update if exist
                {
                    checkitem.evaluation_standard_id = item.id;
                    checkitem.evaluation_standard_code = item.code;
                    checkitem.evaluation_standard_request = item.request;
                    checkitem.evaluation_standard_title = item.title;
                    checkitem.modified_at = DateTime.Now;
                    checkitem.modified_by = _userInfo.Id;
                    if (item.status != 1 || (item.isdelete ?? false) == true)
                    {
                        checkitem.isdelete = true;
                    }
                    listevaluationcompliance_old.Add(checkitem);
                }
                else
                {

                    EvaluationCompliance evaluationcompliance = new EvaluationCompliance();
                    if (obj.audit_id != null)
                        evaluationcompliance.audit_id = obj.audit_id;
                    evaluationcompliance.stage = obj.stage;
                    evaluationcompliance.year = obj.year;
                    evaluationcompliance.evaluation_standard_id = item.id;
                    evaluationcompliance.evaluation_standard_code = item.code;
                    evaluationcompliance.evaluation_standard_request = item.request;
                    evaluationcompliance.evaluation_standard_title = item.title;
                    evaluationcompliance.created_at = DateTime.Now;
                    evaluationcompliance.created_by = _userInfo.Id;
                    evaluationcompliance.compliance = true;
                    listevaluationcompliance_new.Add(evaluationcompliance);

                }
            }
            _uow.Repository<EvaluationCompliance>().Update(listevaluationcompliance_old);
            _uow.Repository<EvaluationCompliance>().Insert(listevaluationcompliance_new);
            _uow.Repository<EvaluationCompliance>().Save();

            return Ok(new { code = "001" });
        }
    }
}
