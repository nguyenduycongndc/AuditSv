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
using System.Text.Json;
using System.Threading.Tasks;

namespace Audit_service.Controllers.Report
{
    [Route("[controller]")]
    [ApiController]
    public class ReportAuditWorkController : BaseController
    {
        public ReportAuditWorkController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<ReportAuditWorkSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var report = _uow.Repository<ReportAuditWork>().Include(a=>a.AuditWork).Where(a => (string.IsNullOrEmpty(obj.Year)  || a.Year == obj.Year)
                                                                     && (!obj.Status.HasValue || a.Status == obj.Status)
                                                                     && (string.IsNullOrEmpty(obj.Code) || a.AuditWorkCode.Contains(obj.Code))
                                                                     && (string.IsNullOrEmpty(obj.Name) || a.AuditWorkName.Contains(obj.Name))
                                                                     && (string.IsNullOrEmpty(obj.PersonInCharge) || (a.AuditWork.person_in_charge.HasValue && a.AuditWork.Users.FullName.Contains(obj.PersonInCharge)))
                                                                     ).ToArray();
                IEnumerable<ReportAuditWork> data = report;
                var count = data.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = _uow.Repository<Users>().Find(a => a.IsDeleted != true && a.IsActive == true).ToArray();
                var result = data.Select(a => new ReportAuditWorkListModel()
                {
                    Id = a.Id,
                    Year = a.Year,
                    Name = a.AuditWorkName,
                    Code = a.AuditWorkCode,
                    StartDate = a.AuditWork?.StartDate,
                    EndDate = a.AuditWork?.EndDate,
                    str_person_in_charge = user.FirstOrDefault(x=> a.AuditWork.person_in_charge == x.Id)?.FullName,
                    Status = a.Status,
                });
                //var r
                return Ok(new { code = "1", msg = "success", data = result, total = count });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public IActionResult Create([FromBody]ReportAuditWorkCreateModel reportauditworkinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _allcheck = _uow.Repository<ReportAuditWork>().Find(a => a.IsDeleted != true && a.auditwork_id == reportauditworkinfo.AuditWorkId).ToArray();
                if (_allcheck.Length > 0)
                {
                    return BadRequest();
                }
                var auditwork = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == reportauditworkinfo.AuditWorkId);
                if(auditwork != null)
                {
                    var report = new ReportAuditWork();
                    report.Year = reportauditworkinfo.Year;
                    report.Status = reportauditworkinfo.Status;
                    report.auditwork_id = auditwork.Id;
                    report.AuditWorkName = auditwork.Name;
                    report.AuditWorkCode = auditwork.Code;
                    report.IsActive = true;
                    report.IsDeleted = false;
                    report.CreatedAt = DateTime.Now;
                    report.CreatedBy = _userInfo.Id;
                    _uow.Repository<ReportAuditWork>().Add(report);
                }

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPut]
        public IActionResult Edit([FromBody] ReportAuditWorkModifyModel reportauditworkinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var report = _uow.Repository<ReportAuditWork>().FirstOrDefault(a => a.Id == reportauditworkinfo.Id);
                if (report == null)
                {
                    return NotFound();
                }
                //report.NumOfWorkdays = reportauditworkinfo.NumOfWorkdays;
                report.AuditRatingLevelTotal = reportauditworkinfo.AuditRatingLevelTotal;
                report.BaseRatingTotal = reportauditworkinfo.BaseRatingTotal;
                report.GeneralConclusions = reportauditworkinfo.GeneralConclusions;
                if (!string.IsNullOrEmpty(reportauditworkinfo.StartDate))
                    report.StartDate = DateTime.ParseExact(reportauditworkinfo.StartDate, "dd/MM/yyyy", null);
                if (!string.IsNullOrEmpty(reportauditworkinfo.EndDate))
                    report.EndDate = DateTime.ParseExact(reportauditworkinfo.EndDate, "dd/MM/yyyy", null);
                if (!string.IsNullOrEmpty(reportauditworkinfo.StartDateField))
                    report.StartDateField = DateTime.ParseExact(reportauditworkinfo.StartDateField, "dd/MM/yyyy", null);
                if (!string.IsNullOrEmpty(reportauditworkinfo.EndDateField))
                    report.EndDateField = DateTime.ParseExact(reportauditworkinfo.EndDateField, "dd/MM/yyyy", null);
                if (!string.IsNullOrEmpty(reportauditworkinfo.ReportDate))
                    report.ReportDate = DateTime.ParseExact(reportauditworkinfo.ReportDate, "dd/MM/yyyy", null);
                report.ModifiedAt = DateTime.Now;
                report.ModifiedBy = _userInfo.Id;
                var lstFacilityScope = new List<AuditWorkScope>();
                if (reportauditworkinfo.LisFacilityScope.Count > 0) {
                    var listScopeID = reportauditworkinfo.LisFacilityScope.Select(a => a.ScopeId).ToArray();
                    var auditscope = _uow.Repository<AuditWorkScope>().Find(a => listScopeID.Contains(a.Id)).ToArray();
                    foreach (var item in reportauditworkinfo.LisFacilityScope)
                    {
                        var auditscopeitem = auditscope.FirstOrDefault(a => a.Id == item.ScopeId);
                        if (auditscopeitem != null)
                        {
                            auditscopeitem.AuditRatingLevelReport = item.AuditRatingLevelItem;
                            auditscopeitem.BaseRatingReport = item.BaseRatingItem;
                            lstFacilityScope.Add(auditscopeitem);
                        }
                    }
                }
                foreach (var item in lstFacilityScope)
                {
                    _uow.Repository<AuditWorkScope>().UpdateWithoutSave(item);
                }
                _uow.Repository<ReportAuditWork>().UpdateWithoutSave(report);
                _uow.SaveChanges();
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

                var report = _uow.Repository<ReportAuditWork>().FirstOrDefault(a => a.Id == id);

                if (report != null)
                {
                    var auditwork = _uow.Repository<AuditWork>().Include(a=>a.AuditWorkScope,a=>a.Users).FirstOrDefault(a => a.Id == report.auditwork_id);
                    var auditdetect = _uow.Repository<AuditDetect>().GetAll().ToArray();
                    var auditassign = _uow.Repository<AuditAssignment>().Include(a=>a.Users).Where(a=>a.auditwork_id == auditwork.Id && a.user_id.HasValue).Select(a=>a.Users.FullName).ToArray();
                    var auditworkscope = _uow.Repository<AuditWorkScope>().Find(a => a.auditwork_id == auditwork.Id).ToArray();
                    var lstauditworkscope  = auditworkscope.Where(a=>a.auditprocess_id.HasValue).Select(a => new AuditWorkScopeModel()
                    {
                        ID = a.Id,
                        AuditProcess = a.auditprocess_name,
                        AuditFacility = a.auditfacilities_name,
                        AuditActivity = a.bussinessactivities_name,
                    }).ToList();

                    var lstAuditworkfacility = auditworkscope.Where(a => a.auditfacilities_id.HasValue).Select(a => new AuditWorkFacilityModel()
                    {
                        ID = a.Id,
                        AuditFacilityId= a.auditfacilities_id,
                        AuditFacility = a.auditfacilities_name,
                        AuditRatingLevelItem = a.AuditRatingLevelReport,
                        BaseRatingItem = a.BaseRatingReport,
                    }).ToList();
                    var lstDetectfacility = auditworkscope.Where(a => a.auditfacilities_id.HasValue).Select(a => new SumaryDetectModel()
                    {
                        AuditFacilityId = a.auditfacilities_id,
                        AuditFacility = a.auditfacilities_name,
                        Hight = auditdetect.Where(x=>x.auditfacilities_id == a.auditfacilities_id).Count(x=>x.rating_risk == 1),
                        Middle = auditdetect.Where(x => x.auditfacilities_id == a.auditfacilities_id).Count(x => x.rating_risk == 2),
                        Low = auditdetect.Where(x => x.auditfacilities_id == a.auditfacilities_id).Count(x => x.rating_risk == 5),
                    }).ToList();
                    var str_name = auditassign.Length > 0 ? string.Join(",", auditassign) : "";
                    var result = new ReportAuditWorkDetailModel()
                    {
                        Id = report.Id,
                        Year = report.Year,
                        Code = report.AuditWorkCode,
                        Name = report.AuditWorkName,
                        Target = auditwork?.Target,
                        Status = report.Status,
                        NumOfAuditor = auditwork?.NumOfAuditor,
                        str_person_in_charge = auditwork.person_in_charge.HasValue ? auditwork.Users.FullName : "",
                        StrAuditorName = str_name,
                        AuditWorkScopeList = lstauditworkscope,
                        AuditWorkFacilityList = lstAuditworkfacility,
                        StartDatePlaning = report.StartDate,
                        EndDatePlaning = report.EndDate,
                        StartDateField = report.StartDateField,
                        EndDateField = report.EndDateField,
                        ReportDate = report.ReportDate,
                        //NumOfWorkdays = report.NumOfWorkdays,
                        Classify = auditwork?.Classify,
                        RatingLevelAudit = report.AuditRatingLevelTotal,
                        RatingBaseAudit = report.BaseRatingTotal,
                        GeneralConclusions = report.GeneralConclusions,
                        SumaryFacilityList = lstDetectfacility,
                        AuditDetectList = auditdetect.Where(a=> a.auditwork_id == report.auditwork_id).Select(a=> new AuditDetectInfoModel() 
                        { 
                            ID = a.id,
                            code = a.code,
                            title = a.title,
                            Description = a.description,
                            auditfacilities_name = a.auditfacilities_name,
                            rating_risk = a.rating_risk,

                        }).ToList(),

                    };
                    return Ok(new { code = "1", msg = "success", data = result });
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
    }
}
