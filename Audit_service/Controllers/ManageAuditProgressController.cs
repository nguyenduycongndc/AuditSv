using Audit_service.DataAccess;
using Audit_service.Models.ExecuteModels;
using Audit_service.Models.ExecuteModels.Audit;
using Audit_service.Models.MigrationsModels;
using Audit_service.Repositories;
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

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class ManageAuditProgressController : BaseController
    {
        protected readonly IConfiguration _config;
        public ManageAuditProgressController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }
        [HttpGet("SearchMageAuditProgress/{auditwork_id}")]
        public IActionResult SearchMageAuditProgress(int auditwork_id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == auditwork_id);
                var auditSchedule = _uow.Repository<Schedule>().Include(a => a.Users).Where(a => a.auditwork_id == auditwork_id && a.is_deleted != true).OrderBy(x=>x.work).ToArray();
                var auditMainStage = _uow.Repository<MainStage>().GetAll().Where(a => a.auditwork_id == auditwork_id).OrderBy(x => x.index).ToArray();
                var res = new ManageAuditProgressSearchModel()
                {
                    id = checkAuditWorkDetail.Id,
                    ListSchedule = auditSchedule.Select(a => new ManageAuditProgressListSchedule
                    {
                        id = a.id,
                        work = a.work,
                        user_id = a.user_id,
                        user_name = a.user_name,
                        expected_date_schedule = a.expected_date.HasValue ? a.expected_date.Value.ToString("dd/MM/yyyy") : "",
                        actual_date_schedule = a.actual_date.HasValue ? a.actual_date.Value.ToString("dd/MM/yyyy") : "",
                    }).ToList(),
                    MainStage = auditMainStage.Select(a => new ManageAuditProgressMainStage
                    {
                        id = a.id,
                        stage = a.stage,
                        expected_date_mainstage = a.expected_date.HasValue ? a.expected_date.Value.ToString("dd/MM/yyyy") : "",
                        actual_date_mainstage = a.actual_date.HasValue ? a.actual_date.Value.ToString("dd/MM/yyyy") : "",
                        status = a.status,
                    }).ToList(),
                    
                };
                return Ok(new { code = "1", msg = "success", data = res, });
            }
            catch (Exception e)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels(), total = 0 });
            }
        }
        [HttpGet("DetailSchedule/{id}")]
        public IActionResult DetailSchedule(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkSchedule = _uow.Repository<Schedule>().FirstOrDefault(a => a.id == id);

                if (checkSchedule != null)
                {
                    var schedule = new DetailScheduleModel()
                    {
                        id = checkSchedule.id,
                        actual_date = checkSchedule.actual_date.HasValue ? checkSchedule.actual_date.Value.ToString("yyyy-MM-dd") : null,
                    };
                    return Ok(new { code = "1", msg = "success", data = schedule });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPut("EditSchedule")]
        public IActionResult EditSchedule([FromBody] EditScheduleModel editScheduleModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkSchedule = _uow.Repository<Schedule>().FirstOrDefault(a => a.id == editScheduleModel.id);
                if (checkSchedule == null) { return NotFound(); }
                if (DateTime.Parse(editScheduleModel.actual_date) > DateTime.Now)
                {
                    return Ok(new { code = "-1", msg = "Không được phép chọn trong tương lai!" });
                }

                checkSchedule.actual_date = DateTime.Parse(editScheduleModel.actual_date);
                checkSchedule.modified_at = DateTime.Now;
                checkSchedule.modified_by = _userInfo.Id;
                _uow.Repository<Schedule>().Update(checkSchedule);
                //var auditWord
                return Ok(new { code = "1", msg = "success"});
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("DetailMainStage/{id}")]
        public IActionResult DetailMainStage(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkMainStage = _uow.Repository<MainStage>().FirstOrDefault(a => a.id == id);

                if (checkMainStage != null)
                {
                    var mainstage = new DetailMainStageModel()
                    {
                        id = checkMainStage.id,
                        stage = checkMainStage.stage,
                        status = checkMainStage.status,
                        actual_date = checkMainStage.actual_date.HasValue ? checkMainStage.actual_date.Value.ToString("yyyy-MM-dd") : null,
                    };
                    return Ok(new { code = "1", msg = "success", data = mainstage });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPut("EditMainStage")]
        public IActionResult EditMainStage([FromBody] EditMainStageModel editMainStageModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkMainStage = _uow.Repository<MainStage>().FirstOrDefault(a => a.id == editMainStageModel.id);
                var checkAuditWord = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == checkMainStage.auditwork_id);
                
                if (checkMainStage == null) { return NotFound(); }
                if (DateTime.Parse(editMainStageModel.actual_date) > DateTime.Now)
                {
                    return Ok(new { code = "-1", msg = "Không được phép chọn trong tương lai!" });
                }
                checkMainStage.status = editMainStageModel.status;
                checkMainStage.actual_date = DateTime.Parse(editMainStageModel.actual_date);
                checkMainStage.modified_at = DateTime.Now;
                checkMainStage.modified_by = _userInfo.Id;
                _uow.Repository<MainStage>().Update(checkMainStage);
                var checkListMainStage5 = _uow.Repository<MainStage>().FirstOrDefault(a => a.auditwork_id == checkMainStage.auditwork_id
                && checkMainStage.index == 5 && a.status != "Chưa thực hiện" && a.status != "");
                var checkListMainStage5Detail = _uow.Repository<MainStage>().FirstOrDefault(a => a.auditwork_id == checkMainStage.auditwork_id
                && a.index == 5);

                var checkListMainStage4 = _uow.Repository<MainStage>().FirstOrDefault(a => a.auditwork_id == checkMainStage.auditwork_id
                && checkMainStage.index == 4 && a.status != "Chưa thực hiện" && a.status != "");
                var checkListMainStage4Detail = _uow.Repository<MainStage>().FirstOrDefault(a => a.auditwork_id == checkMainStage.auditwork_id
                && a.index == 4);

                var checkListMainStage3 = _uow.Repository<MainStage>().FirstOrDefault(a => a.auditwork_id == checkMainStage.auditwork_id
                && checkMainStage.index == 3 && a.status != "Chưa thực hiện" && a.status != "");
                var checkListMainStage3Detail = _uow.Repository<MainStage>().FirstOrDefault(a => a.auditwork_id == checkMainStage.auditwork_id
                && a.index == 3);

                var checkListMainStage2 = _uow.Repository<MainStage>().FirstOrDefault(a => a.auditwork_id == checkMainStage.auditwork_id
                && checkMainStage.index == 2 && a.status != "Chưa thực hiện" && a.status != "");
                var checkListMainStage2Detail = _uow.Repository<MainStage>().FirstOrDefault(a => a.auditwork_id == checkMainStage.auditwork_id
                && a.index == 2);

                var checkListMainStage1 = _uow.Repository<MainStage>().FirstOrDefault(a => a.auditwork_id == checkMainStage.auditwork_id
                && checkMainStage.index == 1 && a.status != "Chưa thực hiện" && a.status != "");
                var checkListMainStage1Detail = _uow.Repository<MainStage>().FirstOrDefault(a => a.auditwork_id == checkMainStage.auditwork_id
                && a.index == 1);
                if (checkListMainStage5 != null)
                {
                    checkAuditWord.ExecutionStatusStr = editMainStageModel.status;
                    checkListMainStage4Detail.status = checkMainStage.index == 4 ? editMainStageModel.status : "";
                    checkListMainStage3Detail.status = checkMainStage.index == 3 ? editMainStageModel.status : "";
                    checkListMainStage2Detail.status = checkMainStage.index == 2 ? editMainStageModel.status : "";
                    checkListMainStage1Detail.status = checkMainStage.index == 1 ? editMainStageModel.status : "";
                    _uow.Repository<AuditWork>().Update(checkAuditWord);
                _uow.Repository<MainStage>().Update(checkListMainStage4Detail);
                _uow.Repository<MainStage>().Update(checkListMainStage3Detail);
                _uow.Repository<MainStage>().Update(checkListMainStage2Detail);
                _uow.Repository<MainStage>().Update(checkListMainStage1Detail);
                } if (checkListMainStage4 != null)
                {
                    checkAuditWord.ExecutionStatusStr = (checkListMainStage5Detail.status != "Chưa thực hiện" && checkListMainStage5Detail.status != "") ? checkListMainStage5Detail.status
                        : (checkListMainStage4Detail.status != "Chưa thực hiện" && checkListMainStage4Detail.status != "") ? checkListMainStage4Detail.status
                        : editMainStageModel.status;
                    checkListMainStage3Detail.status = checkMainStage.index == 3 ? editMainStageModel.status : "";
                    checkListMainStage2Detail.status = checkMainStage.index == 2 ? editMainStageModel.status : "";
                    checkListMainStage1Detail.status = checkMainStage.index == 1 ? editMainStageModel.status : "";

                    checkListMainStage5Detail.status = checkListMainStage5Detail.status == "Chưa thực hiện" ? checkListMainStage5Detail.status = ""
                        : checkListMainStage5Detail.status;
                    _uow.Repository<AuditWork>().Update(checkAuditWord);
                    _uow.Repository<MainStage>().Update(checkListMainStage3Detail);
                    _uow.Repository<MainStage>().Update(checkListMainStage2Detail);
                    _uow.Repository<MainStage>().Update(checkListMainStage1Detail);
                } if (checkListMainStage3 != null)
                {
                    checkAuditWord.ExecutionStatusStr = (checkListMainStage5Detail.status != "Chưa thực hiện" && checkListMainStage5Detail.status != "") ? checkListMainStage5Detail.status
                        : (checkListMainStage4Detail.status != "Chưa thực hiện" && checkListMainStage4Detail.status != "") ? checkListMainStage4Detail.status
                        : (checkListMainStage3Detail.status != "Chưa thực hiện" && checkListMainStage3Detail.status != "") ? checkListMainStage3Detail.status
                        : editMainStageModel.status;
                    checkListMainStage2Detail.status = checkMainStage.index == 2 ? editMainStageModel.status : "";
                    checkListMainStage1Detail.status = checkMainStage.index == 1 ? editMainStageModel.status : "";

                    checkListMainStage5Detail.status = checkListMainStage5Detail.status == "Chưa thực hiện" ? checkListMainStage5Detail.status = ""
                        : checkListMainStage5Detail.status;
                    checkListMainStage4Detail.status = checkListMainStage4Detail.status == "Chưa thực hiện" ? checkListMainStage4Detail.status = ""
                        : checkListMainStage4Detail.status;
                    _uow.Repository<AuditWork>().Update(checkAuditWord);
                    _uow.Repository<MainStage>().Update(checkListMainStage2Detail);
                    _uow.Repository<MainStage>().Update(checkListMainStage1Detail);
                } if (checkListMainStage2 != null)
                {
                    checkAuditWord.ExecutionStatusStr = (checkListMainStage5Detail.status != "Chưa thực hiện" && checkListMainStage5Detail.status != "") ? checkListMainStage5Detail.status
                        : (checkListMainStage4Detail.status != "Chưa thực hiện" && checkListMainStage4Detail.status != "") ? checkListMainStage4Detail.status
                        : (checkListMainStage3Detail.status != "Chưa thực hiện" && checkListMainStage3Detail.status != "") ? checkListMainStage3Detail.status
                        : (checkListMainStage2Detail.status != "Chưa thực hiện" && checkListMainStage2Detail.status != "") ? checkListMainStage2Detail.status
                        : editMainStageModel.status;
                    checkListMainStage1Detail.status = checkMainStage.index == 1 ? editMainStageModel.status : "";

                    checkListMainStage5Detail.status = checkListMainStage5Detail.status == "Chưa thực hiện" ? checkListMainStage5Detail.status = ""
                        : checkListMainStage5Detail.status;
                    checkListMainStage4Detail.status = checkListMainStage4Detail.status == "Chưa thực hiện" ? checkListMainStage4Detail.status = ""
                        : checkListMainStage4Detail.status;
                    checkListMainStage3Detail.status = checkListMainStage3Detail.status == "Chưa thực hiện" ? checkListMainStage3Detail.status = ""
                        : checkListMainStage3Detail.status;
                    _uow.Repository<AuditWork>().Update(checkAuditWord);
                    _uow.Repository<MainStage>().Update(checkListMainStage1Detail);
                } if (checkListMainStage1 != null)
                {
                    checkAuditWord.ExecutionStatusStr = (checkListMainStage5Detail.status != "Chưa thực hiện" && checkListMainStage5Detail.status != "") ? checkListMainStage5Detail.status
                        : (checkListMainStage4Detail.status != "Chưa thực hiện" && checkListMainStage4Detail.status != "") ? checkListMainStage4Detail.status
                        : (checkListMainStage3Detail.status != "Chưa thực hiện" && checkListMainStage3Detail.status != "") ? checkListMainStage3Detail.status
                        : (checkListMainStage2Detail.status != "Chưa thực hiện" && checkListMainStage2Detail.status != "") ? checkListMainStage2Detail.status
                        : (checkListMainStage1Detail.status != "Chưa thực hiện" && checkListMainStage1Detail.status != "") ? checkListMainStage1Detail.status
                        : editMainStageModel.status;
                    checkListMainStage5Detail.status = checkListMainStage5Detail.status == "Chưa thực hiện" ? checkListMainStage5Detail.status = ""
                        : checkListMainStage5Detail.status;
                    checkListMainStage4Detail.status = checkListMainStage4Detail.status == "Chưa thực hiện" ? checkListMainStage4Detail.status = ""
                        : checkListMainStage4Detail.status;
                    checkListMainStage3Detail.status = checkListMainStage3Detail.status == "Chưa thực hiện" ? checkListMainStage3Detail.status = ""
                        : checkListMainStage3Detail.status;
                    checkListMainStage2Detail.status = checkListMainStage2Detail.status == "Chưa thực hiện" ? checkListMainStage2Detail.status = ""
                        : checkListMainStage2Detail.status;
                    
                    _uow.Repository<AuditWork>().Update(checkAuditWord);
                }
                return Ok(new { code = "1", msg = "success"});
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
