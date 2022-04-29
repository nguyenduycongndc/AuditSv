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
using Audit_service.Models.ExecuteModels.Audit;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class AuditWorkController : BaseController
    {
        protected readonly IConfiguration _config;
        public AuditWorkController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }
        [HttpGet("Select")]
        public IActionResult Select(string q)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = q;
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_PAP" && a.StatusCode == "3.1").ToArray();
                var prepareaudit_id = approval_status.Select(a => a.item_id).ToList();
                Expression<Func<AuditWork, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Name.Contains(q) || c.Name.Contains(q)) && c.IsActive == true && c.IsDeleted != true && prepareaudit_id.Contains(c.Id);
                var list_auditwork = _uow.Repository<AuditWork>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditWork> data = list_auditwork;
                var user = data.Select(a => new AuditWorkListModel()
                {
                    Id = a.Id,
                    Name = a.Code + "_" + a.Name,
                });
                return Ok(new { code = "1", msg = "success", data = user });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new AuditWorkListModel() });
            }
        }
        [HttpGet("SelectAuditor")]
        public IActionResult SelectAuditor(string q, int? auditworkid)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = q;
                var AuditAssignment = _uow.Repository<AuditAssignment>().Include(a => a.Users).Where(a => a.IsDeleted != true && a.auditwork_id == auditworkid && a.user_id.HasValue && (string.IsNullOrEmpty(q) || a.Users.FullName.Contains(q)));
                IEnumerable<AuditAssignment> data = AuditAssignment;
                var user = data.Select(a => new AuditWorkListModel()
                {
                    Id = a.user_id,
                    Name = a.user_id.HasValue ? a.Users.FullName : "",
                });
                return Ok(new { code = "1", msg = "success", data = user });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new AuditWorkListModel() });
            }
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditWorkSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                Expression<Func<AuditWork, bool>> filter = c => (string.IsNullOrEmpty(obj.Year) || c.Year.Contains(obj.Year))
                                               && (string.IsNullOrEmpty(obj.Name) || c.Name.Contains(obj.Name))
                                               && c.IsActive == true && c.IsDeleted != true && ((obj.execution_status ?? 0) == 0 || c.ExecutionStatus == obj.execution_status);
                var list_auditwork = _uow.Repository<AuditWork>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditWork> data = list_auditwork;
                var count = list_auditwork.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var lst = data.Select(a => new AuditWorkListModel()
                {
                    Id = a.Id,
                    Code = a.Code,
                    Name = a.Name,
                });
                return Ok(new { code = "1", msg = "success", data = lst, total = count });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("SearchAuditWork")]
        public IActionResult SearchAuditWork(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditWorkSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_PAP" && a.StatusCode == "3.1").ToArray();
                var prepareaudit_id = approval_status.Select(a => a.item_id).ToList();
                Expression<Func<AuditWork, bool>> filter = c => (string.IsNullOrEmpty(obj.Year) || c.Year.Contains(obj.Year))
                                               && (string.IsNullOrEmpty(obj.Name) || c.Name.Contains(obj.Name))
                                               && c.IsActive == true && c.IsDeleted != true && ((obj.execution_status ?? 0) == 0 || c.ExecutionStatus == obj.execution_status) && prepareaudit_id.Contains(c.Id);
                var list_auditwork = _uow.Repository<AuditWork>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditWork> data = list_auditwork;
                var count = list_auditwork.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var lst = data.Select(a => new AuditWorkListModel()
                {
                    Id = a.Id,
                    Code = a.Code,
                    Name = a.Name,
                });
                return Ok(new { code = "1", msg = "success", data = lst, total = count });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create()
        {
            try
            {

                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var auditworkinfo = new AuditWorkModifyModel();
                var data = Request.Form["data"];

                if (!string.IsNullOrEmpty(data))
                {
                    auditworkinfo = JsonSerializer.Deserialize<AuditWorkModifyModel>(data);
                }
                else
                {
                    return BadRequest();
                }
                var AuditWork = new AuditWorkPlan
                {
                    Name = auditworkinfo.Name,
                    Target = auditworkinfo.Target,
                    ReqSkillForAudit = auditworkinfo.ReqSkillForAudit,
                    ReqOutsourcing = auditworkinfo.ReqOutsourcing,
                    ReqOther = auditworkinfo.ReqOther,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userInfo.Id,
                    //Path = pathSave,
                    Year = auditworkinfo.Year,
                    Status = 1,
                    AuditScope = auditworkinfo.AuditScope,
                };
                if (!string.IsNullOrEmpty(auditworkinfo.NumOfWorkdays))
                    AuditWork.NumOfWorkdays = Convert.ToInt32(auditworkinfo.NumOfWorkdays);
                if (!string.IsNullOrEmpty(auditworkinfo.person_in_charge))
                    AuditWork.person_in_charge = Convert.ToInt32(auditworkinfo.person_in_charge);
                if (!string.IsNullOrEmpty(auditworkinfo.NumOfAuditor))
                    AuditWork.NumOfAuditor = Convert.ToInt32(auditworkinfo.NumOfAuditor);
                if (!string.IsNullOrEmpty(auditworkinfo.ScaleOfAudit))
                    AuditWork.ScaleOfAudit = Convert.ToInt32(auditworkinfo.ScaleOfAudit);
                if (!string.IsNullOrEmpty(auditworkinfo.auditplan_id))
                    AuditWork.auditplan_id = Convert.ToInt32(auditworkinfo.auditplan_id);
                AuditWork.Code = GetCode(auditworkinfo.Year);
                if (string.IsNullOrEmpty(AuditWork.Code))
                {
                    return BadRequest();
                }
                AuditWork.StartDate = auditworkinfo.StartDate;
                AuditWork.EndDate = auditworkinfo.EndDate;
                //if (!string.IsNullOrEmpty(auditworkinfo.StartDate))
                //{
                //    AuditWork.StartDate = DateTime.ParseExact(auditworkinfo.StartDate, "dd/MM/yyyy", null);
                //}
                //if (!string.IsNullOrEmpty(auditworkinfo.EndDate))
                //{
                //    AuditWork.EndDate = DateTime.ParseExact(auditworkinfo.EndDate, "dd/MM/yyyy", null);
                //}
                var _listassign = new List<AuditAssignmentPlan>();
                if (auditworkinfo.ListAssign.Count > 0)
                {
                    foreach (var item in auditworkinfo.ListAssign)
                    {
                        var _auditAssignment = new AuditAssignmentPlan
                        {
                            AuditWorkPlan = AuditWork,
                            user_id = Convert.ToInt32(item.UserId),
                            fullName = item.FullName,
                            IsActive = true,
                            IsDeleted = false,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                        };
                        //if (!string.IsNullOrEmpty(item.StartDate))
                        //{
                        //    _auditAssignment.StartDate = DateTime.ParseExact(item.StartDate, "dd/MM/yyyy", null);
                        //}
                        //if (!string.IsNullOrEmpty(item.EndDate))
                        //{
                        //    _auditAssignment.EndDate = DateTime.ParseExact(item.EndDate, "dd/MM/yyyy", null);
                        //}
                        _listassign.Add(_auditAssignment);
                    }
                }

                var _listscope = new List<AuditWorkScopePlan>();
                if (auditworkinfo.ListScope.Count > 0)
                {
                    foreach (var item in auditworkinfo.ListScope)
                    {
                        var _auditWorkScope = new AuditWorkScopePlan
                        {
                            AuditWorkPlan = AuditWork,
                            auditprocess_name = item.auditprocess_name,
                            auditfacilities_name = item.auditfacilities_name,
                            bussinessactivities_name = item.bussinessactivities_name,
                            ReasonNote = item.reason,
                            RiskRatingName = item.risk_rating_name,
                            IsDeleted = false,
                        };
                        if (!string.IsNullOrEmpty(item.Board_id))
                            _auditWorkScope.BoardId = Convert.ToInt32(item.Board_id);
                        if (!string.IsNullOrEmpty(item.auditprocess_id))
                            _auditWorkScope.auditprocess_id = Convert.ToInt32(item.auditprocess_id);
                        if (!string.IsNullOrEmpty(item.auditfacilities_id))
                            _auditWorkScope.auditfacilities_id = Convert.ToInt32(item.auditfacilities_id);
                        if (!string.IsNullOrEmpty(item.bussinessactivities_id))
                            _auditWorkScope.bussinessactivities_id = Convert.ToInt32(item.bussinessactivities_id);
                        if (!string.IsNullOrEmpty(item.risk_rating))
                            _auditWorkScope.RiskRating = Convert.ToInt32(item.risk_rating);
                        if (!string.IsNullOrEmpty(auditworkinfo.Year))
                            _auditWorkScope.Year = Convert.ToInt32(auditworkinfo.Year);
                        if (!string.IsNullOrEmpty(item.auditing_time_nearest))
                            _auditWorkScope.AuditingTimeNearest = DateTime.ParseExact(item.auditing_time_nearest, "MM/yyyy", null);
                        _listscope.Add(_auditWorkScope);
                    }
                }

                var _listscopeFacility = new List<AuditWorkScopePlanFacility>();
                if (auditworkinfo.ListScopeFacility.Count > 0)
                {
                    foreach (var item in auditworkinfo.ListScopeFacility)
                    {
                        var _auditWorkScopeFacility = new AuditWorkScopePlanFacility
                        {
                            AuditWorkPlan = AuditWork,
                            auditfacilities_name = item.auditfacilities_name,
                            ReasonNote = item.reason,
                            RiskRatingName = item.risk_rating_name,
                            IsDeleted = false,
                        };
                        if (!string.IsNullOrEmpty(item.Board_id))
                            _auditWorkScopeFacility.BoardId = Convert.ToInt32(item.Board_id);
                        if (!string.IsNullOrEmpty(item.auditfacilities_id))
                            _auditWorkScopeFacility.auditfacilities_id = Convert.ToInt32(item.auditfacilities_id);
                        if (!string.IsNullOrEmpty(item.risk_rating))
                            _auditWorkScopeFacility.RiskRating = Convert.ToInt32(item.risk_rating);
                        if (!string.IsNullOrEmpty(auditworkinfo.Year))
                            _auditWorkScopeFacility.Year = Convert.ToInt32(auditworkinfo.Year);
                        if (!string.IsNullOrEmpty(item.auditing_time_nearest))
                            _auditWorkScopeFacility.AuditingTimeNearest = DateTime.ParseExact(item.auditing_time_nearest, "MM/yyyy", null);
                        _listscopeFacility.Add(_auditWorkScopeFacility);
                    }
                }

                var file = Request.Form.Files;
                var _listFile = new List<AuditWorkPlanFile>();
                foreach (var item in file)
                {
                    var file_type = item.ContentType;
                    var pathSave = CreateUploadURL(item, "AuditWork");
                    var audit_work_file = new AuditWorkPlanFile()
                    {
                        AuditWorkPlan = AuditWork,
                        IsDelete = false,
                        FileType = file_type,
                        Path = pathSave,
                    };
                    _listFile.Add(audit_work_file);
                }
                _uow.Repository<AuditWorkPlan>().AddWithoutSave(AuditWork);
                foreach (var item in _listscope)
                {
                    _uow.Repository<AuditWorkScopePlan>().AddWithoutSave(item);
                }
                foreach (var item in _listscopeFacility)
                {
                    _uow.Repository<AuditWorkScopePlanFacility>().AddWithoutSave(item);
                }
                foreach (var item in _listassign)
                {
                    _uow.Repository<AuditAssignmentPlan>().AddWithoutSave(item);
                }
                foreach (var item in _listFile)
                {
                    _uow.Repository<AuditWorkPlanFile>().AddWithoutSave(item);
                }

                _uow.SaveChanges();
                var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == AuditWork.auditplan_id);
                var result = new AuditPlanListModel()
                {
                    Id = auditplan?.Id,
                    Version = auditplan?.Version,
                };
                return Ok(new { code = "1", data = result, msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("Update")]
        public IActionResult Update()
        {
            try
            {

                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var auditworkinfo = new AuditWorkModifyModel();
                var data = Request.Form["data"];
                if (!string.IsNullOrEmpty(data))
                {
                    auditworkinfo = JsonSerializer.Deserialize<AuditWorkModifyModel>(data);
                }
                else
                {
                    return BadRequest();
                }
                var AuditWork = _uow.Repository<AuditWorkPlan>().Include(a => a.AuditAssignmentPlan, a => a.AuditWorkScopePlan).FirstOrDefault(a => a.Id == auditworkinfo.Id);
                if (AuditWork == null)
                {
                    return NotFound();
                }
                AuditWork.Name = auditworkinfo.Name;
                AuditWork.Target = auditworkinfo.Target;
                AuditWork.ReqSkillForAudit = auditworkinfo.ReqSkillForAudit;
                AuditWork.ReqOutsourcing = auditworkinfo.ReqOutsourcing;
                AuditWork.ReqOther = auditworkinfo.ReqOther;
                AuditWork.AuditScope = auditworkinfo.AuditScope;
                AuditWork.ModifiedAt = DateTime.Now;
                AuditWork.ModifiedBy = _userInfo.Id;
                //AuditWork.Path = pathSave;
                AuditWork.Year = auditworkinfo.Year;
                if (!string.IsNullOrEmpty(auditworkinfo.NumOfWorkdays))
                    AuditWork.NumOfWorkdays = Convert.ToInt32(auditworkinfo.NumOfWorkdays);
                if (!string.IsNullOrEmpty(auditworkinfo.person_in_charge))
                    AuditWork.person_in_charge = Convert.ToInt32(auditworkinfo.person_in_charge);
                if (!string.IsNullOrEmpty(auditworkinfo.NumOfAuditor))
                    AuditWork.NumOfAuditor = Convert.ToInt32(auditworkinfo.NumOfAuditor);
                if (!string.IsNullOrEmpty(auditworkinfo.ScaleOfAudit))
                    AuditWork.ScaleOfAudit = Convert.ToInt32(auditworkinfo.ScaleOfAudit);
                if (!string.IsNullOrEmpty(auditworkinfo.auditplan_id))
                    AuditWork.auditplan_id = Convert.ToInt32(auditworkinfo.auditplan_id);
                AuditWork.StartDate = auditworkinfo.StartDate;
                AuditWork.EndDate = auditworkinfo.EndDate;
                //if (!string.IsNullOrEmpty(auditworkinfo.StartDate))
                //{
                //    AuditWork.StartDate = DateTime.ParseExact(auditworkinfo.StartDate, "dd/MM/yyyy", null);
                //}

                //if (!string.IsNullOrEmpty(auditworkinfo.EndDate))
                //{
                //    AuditWork.EndDate = DateTime.ParseExact(auditworkinfo.EndDate, "dd/MM/yyyy", null);
                //}
                var _listassign = new List<AuditAssignmentPlan>();
                var _listassignRemove = new List<AuditAssignmentPlan>();
                var recordRemove = AuditWork.AuditAssignmentPlan.Where(a => a.IsDeleted != true).ToArray();
                foreach (var item in recordRemove)
                {
                    item.IsDeleted = true;
                    item.DeletedAt = DateTime.Now;
                    item.DeletedBy = _userInfo.Id;
                    _listassignRemove.Add(item);
                }
                if (auditworkinfo.ListAssign.Count > 0)
                {
                    foreach (var item in auditworkinfo.ListAssign)
                    {
                        var _auditAssignment = new AuditAssignmentPlan
                        {
                            AuditWorkPlan = AuditWork,
                            user_id = Convert.ToInt32(item.UserId),
                            fullName = item.FullName,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _userInfo.Id,
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                        };
                        //if (!string.IsNullOrEmpty(item.StartDate))
                        //{
                        //    _auditAssignment.StartDate = DateTime.ParseExact(item.StartDate, "dd/MM/yyyy", null);
                        //}
                        //if (!string.IsNullOrEmpty(item.EndDate))
                        //{
                        //    _auditAssignment.EndDate = DateTime.ParseExact(item.EndDate, "dd/MM/yyyy", null);
                        //}
                        _listassign.Add(_auditAssignment);
                    }
                }

                var _listscope = new List<AuditWorkScopePlan>();
                if (auditworkinfo.ListScope.Count > 0)
                {
                    foreach (var item in auditworkinfo.ListScope)
                    {
                        var _auditWorkScope = new AuditWorkScopePlan
                        {
                            AuditWorkPlan = AuditWork,
                            auditprocess_name = item.auditprocess_name,
                            auditfacilities_name = item.auditfacilities_name,
                            bussinessactivities_name = item.bussinessactivities_name,
                            ReasonNote = item.reason,
                            RiskRatingName = item.risk_rating_name,
                            IsDeleted = false,
                        };
                        if (!string.IsNullOrEmpty(item.Board_id))
                            _auditWorkScope.BoardId = Convert.ToInt32(item.Board_id);
                        if (!string.IsNullOrEmpty(item.auditprocess_id))
                            _auditWorkScope.auditprocess_id = Convert.ToInt32(item.auditprocess_id);
                        if (!string.IsNullOrEmpty(item.auditfacilities_id))
                            _auditWorkScope.auditfacilities_id = Convert.ToInt32(item.auditfacilities_id);
                        if (!string.IsNullOrEmpty(item.bussinessactivities_id))
                            _auditWorkScope.bussinessactivities_id = Convert.ToInt32(item.bussinessactivities_id);
                        if (!string.IsNullOrEmpty(item.risk_rating))
                            _auditWorkScope.RiskRating = Convert.ToInt32(item.risk_rating);
                        if (!string.IsNullOrEmpty(auditworkinfo.Year))
                            _auditWorkScope.Year = Convert.ToInt32(auditworkinfo.Year);
                        if (!string.IsNullOrEmpty(item.auditing_time_nearest))
                            _auditWorkScope.AuditingTimeNearest = DateTime.ParseExact(item.auditing_time_nearest, "MM/yyyy", null);
                        _listscope.Add(_auditWorkScope);
                    }
                }

                var _listscopeFacility = new List<AuditWorkScopePlanFacility>();
                if (auditworkinfo.ListScopeFacility.Count > 0)
                {
                    foreach (var item in auditworkinfo.ListScopeFacility)
                    {
                        var _auditWorkScopeFacility = new AuditWorkScopePlanFacility
                        {
                            AuditWorkPlan = AuditWork,
                            auditfacilities_name = item.auditfacilities_name,
                            ReasonNote = item.reason,
                            RiskRatingName = item.risk_rating_name,
                            IsDeleted = false,
                        };
                        if (!string.IsNullOrEmpty(item.Board_id))
                            _auditWorkScopeFacility.BoardId = Convert.ToInt32(item.Board_id);
                        if (!string.IsNullOrEmpty(item.auditfacilities_id))
                            _auditWorkScopeFacility.auditfacilities_id = Convert.ToInt32(item.auditfacilities_id);
                        if (!string.IsNullOrEmpty(item.risk_rating))
                            _auditWorkScopeFacility.RiskRating = Convert.ToInt32(item.risk_rating);
                        if (!string.IsNullOrEmpty(auditworkinfo.Year))
                            _auditWorkScopeFacility.Year = Convert.ToInt32(auditworkinfo.Year);
                        if (!string.IsNullOrEmpty(item.auditing_time_nearest))
                            _auditWorkScopeFacility.AuditingTimeNearest = DateTime.ParseExact(item.auditing_time_nearest, "MM/yyyy", null);
                        _listscopeFacility.Add(_auditWorkScopeFacility);
                    }
                }

                var file = Request.Form.Files;
                var _listFile = new List<AuditWorkPlanFile>();
                foreach (var item in file)
                {
                    var file_type = item.ContentType;
                    var pathSave = CreateUploadURL(item, "AuditWork");
                    var audit_work_file = new AuditWorkPlanFile()
                    {
                        AuditWorkPlan = AuditWork,
                        IsDelete = false,
                        FileType = file_type,
                        Path = pathSave,
                    };
                    _listFile.Add(audit_work_file);
                }
                _uow.Repository<AuditWorkPlan>().UpdateWithoutSave(AuditWork);
                foreach (var item in _listscope)
                {
                    _uow.Repository<AuditWorkScopePlan>().AddWithoutSave(item);
                }
                foreach (var item in _listscopeFacility)
                {
                    _uow.Repository<AuditWorkScopePlanFacility>().AddWithoutSave(item);
                }
                foreach (var item in _listassignRemove)
                {
                    _uow.Repository<AuditAssignmentPlan>().UpdateWithoutSave(item);
                }
                foreach (var item in _listassign)
                {
                    _uow.Repository<AuditAssignmentPlan>().AddWithoutSave(item);
                }
                foreach (var item in _listFile)
                {
                    _uow.Repository<AuditWorkPlanFile>().AddWithoutSave(item);
                }
                _uow.SaveChanges();

                var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == AuditWork.auditplan_id);
                var result = new AuditPlanListModel()
                {
                    Id = auditplan?.Id,
                    Version = auditplan?.Version,
                };
                return Ok(new { code = "1", data = result, msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var AuditPlan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == id);
                if (AuditPlan == null)
                {
                    return NotFound();
                }

                AuditPlan.IsDelete = true;
                _uow.Repository<AuditPlan>().Update(AuditPlan);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpDelete("DeleteAuditWork/{id}")]
        public IActionResult DeleteAuditWork(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var _AuditWork = _uow.Repository<AuditWorkPlan>().FirstOrDefault(a => a.Id == id);
                if (_AuditWork == null)
                {
                    return NotFound();
                }
                if (_AuditWork.Status == 3)
                {
                    return Ok(new { code = "0", msg = "success" });
                }
                _AuditWork.IsDeleted = true;
                _AuditWork.DeletedAt = DateTime.Now;
                _AuditWork.DeletedBy = userInfo.Id;
                _uow.Repository<AuditWorkPlan>().Update(_AuditWork);

                var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == _AuditWork.auditplan_id);
                if (auditplan == null)
                {
                    return Ok(new { code = "1", msg = "success" });
                }
                else
                {
                    var result = new AuditPlanListModel()
                    {
                        Id = auditplan?.Id,
                        Version = auditplan?.Version,
                    };
                    return Ok(new { code = "1", data = result, msg = "success" });
                }

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpDelete("DeleteAuditWorkScope/{id}/{all}")]
        public IActionResult DeleteAuditWorkScope(int id, int all)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var AuditWorkScope = _uow.Repository<AuditWorkScopePlan>().FirstOrDefault(a => a.Id == id);
                if (AuditWorkScope == null)
                {
                    return NotFound();
                }

                AuditWorkScope.IsDeleted = true;
                AuditWorkScope.DeletedAt = DateTime.Now;
                AuditWorkScope.DeletedBy = userInfo.Id;
                var list_remove = new List<AuditWorkScopePlanFacility>();
                if (all == 1)
                {
                    var Facility = _uow.Repository<AuditWorkScopePlanFacility>().Find(a => a.auditfacilities_id == AuditWorkScope.auditfacilities_id).ToArray();

                    if (Facility.Length > 0)
                    {
                        foreach (var item in Facility)
                        {
                            item.IsDeleted = true;
                            item.DeletedAt = DateTime.Now;
                            item.DeletedBy = userInfo.Id;
                            list_remove.Add(item);
                        }

                    }
                }
                _uow.Repository<AuditWorkScopePlan>().Update(AuditWorkScope);
                _uow.Repository<AuditWorkScopePlanFacility>().Update(list_remove);
                return Ok(new { code = "1", id = AuditWorkScope.auditwork_id, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("DeleteFacilityScope/{id}")]
        public IActionResult DeleteFacilityScope(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var Facility = _uow.Repository<AuditWorkScopePlanFacility>().FirstOrDefault(a => a.Id == id);
                if (Facility == null)
                {
                    return NotFound();
                }

                Facility.IsDeleted = true;
                Facility.DeletedAt = DateTime.Now;
                Facility.DeletedBy = userInfo.Id;

                var auditscope = _uow.Repository<AuditWorkScopePlan>().Find(a => a.auditwork_id == Facility.auditwork_id && Facility.auditfacilities_id == a.auditfacilities_id).ToArray();
                var list_remove = new List<AuditWorkScopePlan>();
                if (auditscope.Length > 0)
                {
                    foreach (var item in auditscope)
                    {
                        item.IsDeleted = true;
                        item.DeletedAt = DateTime.Now;
                        item.DeletedBy = userInfo.Id;
                        list_remove.Add(item);
                    }

                }

                _uow.Repository<AuditWorkScopePlanFacility>().Update(Facility);
                _uow.Repository<AuditWorkScopePlan>().Update(list_remove);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var AuditWork = _uow.Repository<AuditWorkPlan>().Include(a => a.Users, a => a.AuditWorkPlanFile, a => a.AuditWorkScopePlanFacility).FirstOrDefault(a => a.Id == id);
                if (AuditWork == null)
                {
                    return NotFound();
                }
                var auditAssignment = _uow.Repository<AuditAssignmentPlan>().Include(a => a.Users).Where(a => a.auditwork_id == id && a.IsDeleted != true).ToArray();
                var auditWorkScope = _uow.Repository<AuditWorkScopePlan>().Find(a => a.auditwork_id == id && a.IsDeleted != true).ToArray();

                var result = new AuditWorkPlanDetailModel()
                {
                    Id = AuditWork.Id,
                    Name = AuditWork.Name,
                    Target = AuditWork.Target,
                    StartDate = AuditWork.StartDate.HasValue ? AuditWork.StartDate.Value.ToString("yyyy-MM-dd") : null,
                    EndDate = AuditWork.EndDate.HasValue ? AuditWork.EndDate.Value.ToString("yyyy-MM-dd") : null,
                    NumOfWorkdays = AuditWork.NumOfWorkdays,
                    person_in_charge = AuditWork.person_in_charge,
                    str_person_in_charge = AuditWork.person_in_charge.HasValue ? AuditWork.Users.Id + ":" + AuditWork.Users.FullName : "",
                    NumOfAuditor = AuditWork.NumOfAuditor,
                    ReqSkillForAudit = AuditWork.ReqSkillForAudit,
                    ReqOutsourcing = AuditWork.ReqOutsourcing,
                    ReqOther = AuditWork.ReqOther,
                    ScaleOfAudit = AuditWork.ScaleOfAudit,
                    AuditScope = AuditWork.AuditScope,
                    ListAuditor = auditAssignment.Select(x => new ListAuditorModel()
                    {
                        Id = x.Id,
                        Auditor = x.user_id.HasValue ? x.Users.Id + ":" + x.Users.FullName : "",
                        StartDate = x.StartDate.HasValue ? x.StartDate.Value.ToString("yyyy-MM-dd") : "",
                        EndDate = x.EndDate.HasValue ? x.EndDate.Value.ToString("yyyy-MM-dd") : "",
                    }).ToList(),
                    Path = AuditWork.Path,
                    ListAuditWorkScope = auditWorkScope.Select(y => new AuditScopeDetailModel()
                    {
                        Id = y.Id,
                        Board_id = y.BoardId,
                        auditfacilities_id = y.auditfacilities_id,
                        auditfacilities_name = y.auditfacilities_name,
                        auditprocess_id = y.auditprocess_id,
                        auditprocess_name = y.auditprocess_name,
                        bussinessactivities_id = y.bussinessactivities_id,
                        bussinessactivities_name = y.bussinessactivities_name,
                        reason = y.ReasonNote,
                        auditing_time_nearest = y.AuditingTimeNearest.HasValue ? y.AuditingTimeNearest.Value.ToString("MM/yyyy") : "",
                        risk_rating = y.RiskRating,
                        risk_rating_name = y.RiskRatingName,
                    }).ToList(),
                    ListFile = AuditWork.AuditWorkPlanFile.Where(a => a.IsDelete != true).Select(x => new AuditWorkPlanFileModel()
                    {
                        id = x.id,
                        Path = x.Path,
                        FileType = x.FileType,
                    }).ToList(),
                    ListAuditWorkScopeFacility = AuditWork.AuditWorkScopePlanFacility.Where(a => a.IsDeleted != true).Select(y => new AuditScopeDetailModel()
                    {
                        Id = y.Id,
                        Board_id = y.BoardId,
                        auditfacilities_id = y.auditfacilities_id,
                        auditfacilities_name = y.auditfacilities_name,
                        reason = y.ReasonNote,
                        auditing_time_nearest = y.AuditingTimeNearest.HasValue ? y.AuditingTimeNearest.Value.ToString("MM/yyyy") : "",
                        risk_rating = y.RiskRating,
                        risk_rating_name = y.RiskRatingName,
                    }).ToList(),
                };
                return Ok(new { code = "1", msg = "success", data = result });
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        public static string GetCode(string Year)
        {
            string EID = string.Empty;
            try
            {

                var yearnow = DateTime.Now.Year;
                if (string.IsNullOrEmpty(Year)) Year = yearnow + "";
                string str_start = Year + ".CKT";
                using (var context = new KitanoSqlContext())
                {
                    var list = context.AuditWorkPlan.Where(a => a.IsDeleted != true).ToArray();
                    var data = list.Where(a => !string.IsNullOrEmpty(a.Code) && a.Code.StartsWith(str_start)).GroupBy(a => a.Code).Select(g => g.FirstOrDefault());
                    for (int i = data.Count() + 1; ; i++)
                    {
                        if (i <= 9)
                        {
                            EID = str_start + ".00" + i.ToString();
                        }
                        else if (i > 9 && i <= 99)
                        {
                            EID = str_start + ".0" + i.ToString();
                        }
                        else if (i > 99 && i <= 999)
                        {
                            EID = str_start + "." + i.ToString();
                        }
                        var data_ = list.Where(a => a.Code == EID);
                        if (!data_.Any())
                        {
                            break;
                        }
                    }
                }

                return EID;
            }
            catch (Exception)
            {

                return EID;
            }

        }
        protected string CreateUploadURL(IFormFile imageFile, string folder = "")
        {
            var pathSave = "";
            var pathconfig = _config["Upload:AuditDocsPath"];
            if (imageFile != null)
            {
                if (string.IsNullOrEmpty(folder)) folder = "Public";
                var pathToSave = Path.Combine(pathconfig, folder);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName)?.Trim();
                var extension = Path.GetExtension(imageFile.FileName);
                var new_folder = DateTime.Now.ToString("yyyyMM");
                var fileName = fileNameWithoutExtension + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                var fullPathroot = Path.Combine(pathToSave, new_folder);
                if (!Directory.Exists(fullPathroot))
                {
                    Directory.CreateDirectory(fullPathroot);
                }
                pathSave = Path.Combine(folder, new_folder, fileName);
                var filePath = Path.Combine(fullPathroot, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }
            }
            return pathSave;

        }
        protected string CreateUploadFile(IFormFileCollection list_ImageFile, string folder = "")
        {
            var lstStr = new List<string>();
            foreach (var item in list_ImageFile)
            {
                string imgURL = CreateUploadURL(item, folder);
                if (!string.IsNullOrEmpty(imgURL)) lstStr.Add(imgURL);
            }
            return string.Join(",", lstStr);
        }
        [HttpGet("SearchPrepareAudit")]
        public IActionResult SearchPrepareAudit(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<SearchPrepareAuditModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                Expression<Func<AuditWork, bool>> filter = c => (string.IsNullOrEmpty(obj.Year) || c.Year.Contains(obj.Year))
                                               && (string.IsNullOrEmpty(obj.Code) || c.Code.Contains(obj.Code) || c.Name.Contains(obj.Code))
                                               && c.IsDeleted != true;
                var list_auditwork = _uow.Repository<AuditWork>().Include(x => x.Users).Where(filter);
                IEnumerable<AuditWork> data = list_auditwork;
                var count = list_auditwork.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }
                //var _data = _uow.Repository<AuditWork>().Include(x => x.Users);
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }

                var auditWork = data.Select(a => new ListAuditWorkModel()
                {
                    Id = a.Id,
                    user_login_id = userInfo.Id,
                    flag_user_id = a.flag_user_id,
                    Code = a.Code,
                    Name = a.Name,
                    Target = a.Target,
                    StartDate = a.StartDate.HasValue ? a.StartDate.Value.ToString("dd/MM/yyyy") : "",
                    EndDate = a.EndDate.HasValue ? a.EndDate.Value.ToString("dd/MM/yyyy") : "",
                    NumOfWorkdays = a.NumOfWorkdays,
                    person_in_charge = a.person_in_charge,
                    NumOfAuditor = a.NumOfAuditor,
                    ReqSkillForAudit = a.ReqSkillForAudit,
                    ReqOutsourcing = a.ReqOutsourcing,
                    ReqOther = a.ReqOther,
                    ScaleOfAudit = a.ScaleOfAudit,
                    ExecutionStatusStr = a.ExecutionStatusStr,
                    IsActive = a.IsActive,
                    IsDeleted = a.IsDeleted,
                    CreatedAt = a.CreatedAt.HasValue ? a.CreatedAt.Value.ToString("dd/MM/yyyy") : "",
                    CreatedBy = a.CreatedBy,
                    ModifiedAt = a.ModifiedAt.HasValue ? a.ModifiedAt.Value.ToString("dd/MM/yyyy") : "",
                    ModifiedBy = a.ModifiedBy,
                    DeletedAt = a.DeletedAt.HasValue ? a.DeletedAt.Value.ToString("dd/MM/yyyy") : "",
                    DeletedBy = a.DeletedBy,
                    Path = a.Path,
                    Classify = a.Classify,
                    Year = a.Year,
                    auditplan_id = a.auditplan_id,
                    name_person_in_charge = a.Users != null ? a.Users.FullName : "",
                    ExtensionTime = a.ExtensionTime.HasValue ? a.ExtensionTime.Value.ToString("dd/MM/yyyy") : "",
                });
                return Ok(new { code = "1", msg = "success", data = auditWork, total = count });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("SearchPrepareAuditApproved")]
        public IActionResult SearchPrepareAuditApprove(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<SearchPrepareAuditModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                Expression<Func<AuditWork, bool>> filter = c => (string.IsNullOrEmpty(obj.Year) || c.Year.Contains(obj.Year))
                                               && (string.IsNullOrEmpty(obj.Code) || c.Code.Contains(obj.Code) || c.Name.Contains(obj.Code))
                                               && c.IsDeleted != true;
                var list_auditwork = (from a in _uow.Repository<AuditWork>().Include(x => x.Users)
                                      join b in _uow.Repository<ApprovalFunction>().Find(x => x.function_code == "M_PAP" && x.StatusCode == "3.1") on a.Id equals b.item_id
                                      select a).Where(filter);
                IEnumerable<AuditWork> data = list_auditwork;
                var count = list_auditwork.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }

                var auditWork = data.Select(a => new ListAuditWorkModel()
                {
                    Id = a.Id,
                    Code = a.Code,
                    Name = a.Name,
                    Target = a.Target,
                    StartDate = a.StartDate.HasValue ? a.StartDate.Value.ToString("dd/MM/yyyy") : "",
                    EndDate = a.EndDate.HasValue ? a.EndDate.Value.ToString("dd/MM/yyyy") : "",
                    NumOfWorkdays = a.NumOfWorkdays,
                    person_in_charge = a.person_in_charge,
                    NumOfAuditor = a.NumOfAuditor,
                    ReqSkillForAudit = a.ReqSkillForAudit,
                    ReqOutsourcing = a.ReqOutsourcing,
                    ReqOther = a.ReqOther,
                    ScaleOfAudit = a.ScaleOfAudit,
                    //Status = a.Status,
                    ExecutionStatus = a.ExecutionStatus,
                    IsActive = a.IsActive,
                    IsDeleted = a.IsDeleted,
                    CreatedAt = a.CreatedAt.HasValue ? a.CreatedAt.Value.ToString("dd/MM/yyyy") : "",
                    CreatedBy = a.CreatedBy,
                    ModifiedAt = a.ModifiedAt.HasValue ? a.ModifiedAt.Value.ToString("dd/MM/yyyy") : "",
                    ModifiedBy = a.ModifiedBy,
                    DeletedAt = a.DeletedAt.HasValue ? a.DeletedAt.Value.ToString("dd/MM/yyyy") : "",
                    DeletedBy = a.DeletedBy,
                    Path = a.Path,
                    Classify = a.Classify,
                    Year = a.Year,
                    auditplan_id = a.auditplan_id,
                    name_person_in_charge = a.person_in_charge.HasValue ? a.Users.FullName : "",
                    ExtensionTime = a.ExtensionTime.HasValue ? a.ExtensionTime.Value.ToString("dd/MM/yyyy") : "",
                });
                return Ok(new { code = "1", msg = "success", data = auditWork, total = count });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("SearchYear")]
        public IActionResult SearchYear(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditWorkSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                Expression<Func<AuditWork, bool>> filter = c => c.IsDeleted != true && !string.IsNullOrEmpty(c.Year);
                var list_auditwork = _uow.Repository<AuditWork>().Find(filter);
                IEnumerable<AuditWork> data = list_auditwork;
                var count = list_auditwork.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var lst = data.Select(a => new { year = Convert.ToInt32(a.Year) }).Distinct().OrderByDescending(x => x.year);


                return Ok(new { code = "1", msg = "success", data = lst, total = count });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("SelectYearApproved")]//list •năm đã duyệt ở cuộc KT
        public IActionResult SelectYearApproved(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditWorkSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_PAP" && a.StatusCode == "3.1").ToArray();
                var audiplan_id = approval_status.Select(a => a.item_id).ToList();
                Expression<Func<AuditWork, bool>> filter = c => c.IsDeleted != true && audiplan_id.Contains(c.Id);
                var list_yearapproved = _uow.Repository<AuditWork>().Find(filter).OrderByDescending(a => a.Year);
                IEnumerable<AuditWork> data = list_yearapproved;
                var lst = data.Select(a => new { year = Convert.ToInt32(a.Year) }).Distinct().OrderByDescending(x => x.year);
                return Ok(new { code = "1", msg = "success", data = lst});
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new DropListYearApprovedModel() });
            }
        }
        [AllowAnonymous]
        [HttpGet("DownloadAttach")]
        public IActionResult DonwloadFile(int id)
        {
            try
            {
                var self = _uow.Repository<AuditWorkPlanFile>().FirstOrDefault(o => o.id == id);
                if (self == null)
                {
                    return NotFound();
                }
                var fullPath = Path.Combine(_config["Upload:AuditDocsPath"], self.Path);
                var name = "DownLoadFile";
                if (!string.IsNullOrEmpty(self.Path))
                {
                    var _array = self.Path.Replace("/", "\\").Split("\\");
                    name = _array[_array.Length - 1];
                }
                var fs = new FileStream(fullPath, FileMode.Open);

                return File(fs, self.FileType, name);
            }
            catch (Exception)
            {

                return NotFound();
            }
        }
        [HttpGet("DeleteAttach/{id}")]
        public IActionResult DeleteFile(int id)
        {
            try
            {
                var self = _uow.Repository<AuditWorkPlanFile>().FirstOrDefault(o => o.id == id);
                if (self == null)
                {
                    return NotFound();
                }
                var check = false;
                if (!string.IsNullOrEmpty(self.Path))
                {
                    var fullPath = Path.Combine(_config["Upload:AuditDocsPath"], self.Path);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                        check = true;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                if (check == true)
                {
                    self.IsDelete = true;
                    _uow.Repository<AuditWorkPlanFile>().Update(self);
                }

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
    }
}