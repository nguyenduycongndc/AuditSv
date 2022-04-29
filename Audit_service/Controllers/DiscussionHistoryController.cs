using Audit_service.DataAccess;
using Audit_service.Models.ExecuteModels;
using Audit_service.Models.ExecuteModels.Audit;
using Audit_service.Models.MigrationsModels;
using Audit_service.Repositories;
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
    public class DiscussionHistoryController : BaseController
    {
        protected readonly IConfiguration _config;
        public DiscussionHistoryController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }
        [HttpPost("SaveDiscussionHistory")]
        public IActionResult SaveDiscussionHistory([FromBody] DiscussionHistoryModel discussionHistoryModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var AuditPlanCode = "";
                var AuditWorkCode = "";
                var WorkingPaperCode = "";
                var AuditDetectCode = "";
                var ReportAuditWorkCode = "";
                var AuditProgramCode = "";
                if (discussionHistoryModel.item_type == 1)
                {
                    //check Kế hoạch kiểm toán năm
                    var checkAuditPlan = _uow.Repository<AuditPlan>().FirstOrDefault(x => x.Id == discussionHistoryModel.item_id);
                    AuditPlanCode = checkAuditPlan.Code;
                }
                else if (discussionHistoryModel.item_type == 2)
                {
                    //check Kế hoạch cuộc kiểm toán
                    var checkAuditWork = _uow.Repository<AuditWork>().FirstOrDefault(x => x.Id == discussionHistoryModel.item_id);
                    AuditWorkCode = checkAuditWork.Code;
                }
                else if (discussionHistoryModel.item_type == 3)
                {
                    //check Giấy tờ làm việc
                    var checkWorkingPaper = _uow.Repository<WorkingPaper>().FirstOrDefault(x => x.id == discussionHistoryModel.item_id);
                    WorkingPaperCode = checkWorkingPaper.code;
                }
                else if (discussionHistoryModel.item_type == 4)
                {
                    //check Phát hiện kiểm toán
                    var checkAuditDetect = _uow.Repository<AuditDetect>().FirstOrDefault(x => x.id == discussionHistoryModel.item_id);
                    AuditDetectCode = checkAuditDetect.code;
                }
                else if (discussionHistoryModel.item_type == 5)
                {
                    //check Báo cáo cuộc kiểm toán
                    var checkReportAuditWork = _uow.Repository<ReportAuditWork>().FirstOrDefault(x => x.Id == discussionHistoryModel.item_id);
                    ReportAuditWorkCode = checkReportAuditWork.AuditWorkCode;
                }
                else if (discussionHistoryModel.item_type == 6)
                {
                    //check Chương trình kiểm toán
                    var checkAuditProgram = _uow.Repository<AuditProgram>().Include(a=>a.AuditWork).FirstOrDefault(x => x.Id == discussionHistoryModel.item_id);
                    AuditProgramCode = checkAuditProgram.AuditWork.Code;
                }
                else if (discussionHistoryModel.item_type == 7)
                {
                    //check Chương trình kiểm toán
                    var checkreportauditworkyear = _uow.Repository<ReportAuditWorkYear>().FirstOrDefault(x => x.Id == discussionHistoryModel.item_id);
                }

                var _auditRequestMonitor = new DiscussionHistory
                {
                    item_id = discussionHistoryModel.item_id,
                    item_name = discussionHistoryModel.item_type == 1 ? "Kế hoạch kiểm toán năm"
                    : discussionHistoryModel.item_type == 2 ? "Kế hoạch cuộc kiểm toán"
                    : discussionHistoryModel.item_type == 3 ? "Giấy tờ làm việc"
                    : discussionHistoryModel.item_type == 4 ? "Phát hiện kiểm toán"
                    : discussionHistoryModel.item_type == 5 ? "Báo cáo cuộc kiểm toán" 
                    : discussionHistoryModel.item_type == 6 ? "Chương trình kiểm toán"
                    : discussionHistoryModel.item_type == 7 ? "Báo cáo kiểm toán năm" : null,
                    item_code = AuditPlanCode != "" ? AuditPlanCode
                    : AuditWorkCode != "" ? AuditWorkCode
                    : WorkingPaperCode != "" ? WorkingPaperCode
                    : AuditDetectCode != "" ? AuditDetectCode
                    : ReportAuditWorkCode != "" ? ReportAuditWorkCode 
                    : AuditProgramCode != "" ? AuditProgramCode  : null,
                    item_type = discussionHistoryModel.item_type,
                    type = discussionHistoryModel.type,
                    content = discussionHistoryModel.content != null ? discussionHistoryModel.content : "",
                    created_at = DateTime.Now,
                    version = discussionHistoryModel.version != null ? discussionHistoryModel.version : "",
                    person_perform = _userInfo.FullName,
                };
                _uow.Repository<DiscussionHistory>().Add(_auditRequestMonitor);

                return Ok(new { code = "1", msg = "success", data = _auditRequestMonitor });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<SearchDiscussionHistoryModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }


                Expression<Func<DiscussionHistory, bool>> filter = c => (obj.item_id == null || c.item_id.Equals(obj.item_id) && c.item_type.Equals(obj.item_type));
                var list_discussionhistory = _uow.Repository<DiscussionHistory>().GetAll().Where(filter).OrderByDescending(x => x.created_at);
                IEnumerable<DiscussionHistory> data = list_discussionhistory;
                var count = list_discussionhistory.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var result = data.Select(a => new ListDiscussionHistoryModel()
                {
                    id = a.id,
                    item_id = a.item_id,
                    item_name = a.item_name,
                    item_code = a.item_code,
                    item_type = a.item_type,
                    type = a.type,
                    content = a.content,
                    created_at = a.created_at.HasValue ? a.created_at.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    version = a.version,
                    person_perform = a.person_perform != null ? a.person_perform : "",
                });

                return Ok(new { code = "1", msg = "success", data = result, total = count });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
