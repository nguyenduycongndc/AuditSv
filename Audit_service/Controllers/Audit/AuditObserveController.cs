using Audit_service.DataAccess;
using Audit_service.Models.ExecuteModels;
using Audit_service.Models.ExecuteModels.Audit;
using Audit_service.Models.MigrationsModels;
using Audit_service.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class AuditObserveController : BaseController
    {
        public AuditObserveController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }
        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditObserveSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }


                Expression<Func<AuditObserve, bool>> filter = c => (obj.year == null || c.year.Equals(obj.year))
                && ((obj.auditwork_id == null) || c.auditwork_id.Equals(obj.auditwork_id))
                && ((obj.discoverer == null) || c.discoverer.Equals(obj.discoverer))
                && (string.IsNullOrEmpty(obj.name) || c.name.ToLower().Contains(obj.name.ToLower().Trim()))
                && (string.IsNullOrEmpty(obj.code) || c.code.Contains(obj.code.Trim()))
                && ( c.IsDeleted != true)
                && (string.IsNullOrEmpty(obj.working_paper_code) || c.working_paper_code.Contains(obj.working_paper_code.Trim()));
                var list_auditobserve = _uow.Repository<AuditObserve>().Include(x => x.Users).Where(filter).OrderByDescending(x=>x.CreatedAt);
                IEnumerable<AuditObserve> data = list_auditobserve;
                var count = list_auditobserve.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var result = data.Select(a => new AuditObserveModel()
                {
                    id = a.id,
                    code = a.code,
                    name = a.name,
                    year = a.year,
                    auditwork_id = a.auditwork_id,
                    auditwork_name = a.auditwork_name,
                    working_paper_code = a.working_paper_code == null ? "" : a.working_paper_code,
                    discoverer_name = a.Users.FullName,
                });

                return Ok(new { code = "1", msg = "success", data = result, total = count });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("ListYearAuditWorkDefault")]//list •	Cuộc năm của kiểm toán lấy năm mặc định
        public IActionResult ListYearAuditWorkDefault()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var listyear = _uow.Repository<AuditWork>().GetAll(a => a.IsDeleted != true);
                IEnumerable<AuditWork> data = listyear;
                var res = data.Select(a => new DropListYearAuditWorkModel()
                {
                    id = Int32.Parse(a.Year),
                    year = a.Year,
                    current_year = (a.Year == DateTime.Now.Year.ToString() ? true : false),
                });
                var lst = res.GroupBy(a => a.year).Select(x => x.FirstOrDefault()).ToList();
                var defaulYear = lst.FirstOrDefault(x => x.current_year == true);
                return Ok(new { code = "1", msg = "success", data = defaulYear });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new DropListYearAuditWorkModel(), total = 0 });
            }
        }
        [HttpGet("ListAuditWorkObserve")] //list •	Cuộc kiểm toán
        public IActionResult ListAuditWorkObserve(string q, string year)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_PAP"
                && a.StatusCode == "3.1").ToArray();
                var audiplan_id = approval_status.Select(a => a.item_id).ToList();

                string KeyWord = q;
                Expression<Func<AuditWork, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Name.ToLower().Contains(q.ToLower()) || c.Name.ToLower().Contains(q.ToLower()))
                && c.IsActive == true && c.IsDeleted != true && c.Year == year && audiplan_id.Contains(c.Id);
                var listauditwork = _uow.Repository<AuditWork>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditWork> dropauditwork = listauditwork;
                var res = dropauditwork.Select(a => new DropListAuditWorkModel()
                {
                    id = a.Id,
                    name = a.Name,
                    year = a.Year,
                });
                return Ok(new { code = "1", msg = "success", data = res, total = res.Count() });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = "", total = 0 });
            }
        }
    }
}
