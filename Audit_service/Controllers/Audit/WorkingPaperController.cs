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
using KitanoUserService.API.Models.MigrationsModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using StackExchange.Redis;

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class WorkingPaperController : BaseController
    {
        protected readonly IConfiguration _config;

        public WorkingPaperController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            this._config = config;
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
               
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var obj = JsonSerializer.Deserialize<WorkingPaperSearchModel>(jsonData);
                var process = string.IsNullOrEmpty(obj.processid) ? 0 : Convert.ToInt32(obj.processid);
                var status = string.IsNullOrEmpty(obj.status) ? "" : obj.status;
                var year = string.IsNullOrEmpty(obj.year) ? 0 : Convert.ToInt32(obj.year);
                var unit = string.IsNullOrEmpty(obj.unitid) ? 0 : Convert.ToInt32(obj.unitid);
                var audit = string.IsNullOrEmpty(obj.auditworkid) ? 0 : Convert.ToInt32(obj.auditworkid);

                //lấy tên đơn vị

                var _unit = _uow.Repository<AuditFacility>().Find(a => a.IsActive == true).ToArray();

                //lấy tên cuộc kiểm toán

                var _process = _uow.Repository<AuditProcess>().Find(a => !a.Deleted).ToArray();

                //lấy tên quy trình kiểm toán

                var _audit = _uow.Repository<AuditWork>().Find(a => a.IsDeleted == false).ToArray();
                var approval_config = _uow.Repository<ApprovalConfig>().GetAll(a => a.item_code == "M_WP").ToArray().OrderBy(x => x.StatusCode);

                var list_auditrequest = (from a in _uow.Repository<WorkingPaper>().GetAll()
                                         join b in _uow.Repository<ApprovalFunction>().Find(x => x.function_code == "M_WP") on a.id equals b.item_id into g
                                         from s in g.DefaultIfEmpty()
                                         where (string.IsNullOrEmpty(obj.code) || a.code.ToLower().Contains(obj.code.ToLower())) && (string.IsNullOrEmpty(obj.processid) || process == 0 ||
                                         a.processid == process) && (string.IsNullOrEmpty(obj.status) || (s.StatusCode ?? "1.0") == status) && (string.IsNullOrEmpty(obj.year) || year == 0 ||
                                         a.year == year) && (string.IsNullOrEmpty(obj.unitid) || unit == 0 || a.unitid == unit) && (string.IsNullOrEmpty(obj.auditworkid) || audit == 0 ||
                                         a.auditworkid == audit) && (a.isdelete != true)
                                         select a).OrderByDescending(p => p.id);
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_WP").ToArray();
                IEnumerable<WorkingPaper> data = list_auditrequest;

                var count = list_auditrequest.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var lst = data.Select(a => new WorkingPaperSearchResultModel()
                {
                    id = a.id,
                    code = a.code,
                    auditworkid = Convert.ToString(a.auditworkid),
                    processid = Convert.ToString(a.processid),
                    unitid = Convert.ToString(a.unitid),
                    status = approval_status.FirstOrDefault(x => x.item_id == a.id)?.StatusCode ?? "1.0",
                    statusname = approval_config.FirstOrDefault(x => x.StatusCode == (approval_status.FirstOrDefault(x => x.item_id == a.id)?.StatusCode ?? "1.0"))?.StatusName ?? "",
                    unitname = a.unitid.HasValue ? _unit.FirstOrDefault(u => u.Id == a.unitid)?.Name : "",
                    processname = a.processid.HasValue ? _process.FirstOrDefault(u => u.Id == a.processid)?.Name : "",
                    auditworkname = a.auditworkid.HasValue ? _audit.FirstOrDefault(u => u.Id == a.auditworkid)?.Name : "",
                    year = Convert.ToString(a.year),
                    //reviewerid = (approval_status.FirstOrDefault(x => x.item_id == a.id)?.approver_last ?? 0) != 0 ? approval_status.FirstOrDefault(x => x.item_id == a.id)?.approver_last : approval_status.FirstOrDefault(x => x.item_id == a.id)?.approver,
                    ApprovalUser = approval_status.FirstOrDefault(x => x.item_id == a.id)?.approver,
                    ApprovalUserLast = approval_status.FirstOrDefault(x => x.item_id == a.id)?.approver_last


                });
                return Ok(new { code = "1", msg = "success", data = lst, total = count });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create([FromForm] WorkingPaperModel wp)
        {
            try
            {
                using var context = new KitanoSqlContext();

                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }

                try
                {
                    var checkcode = context.WorkingPaper.FirstOrDefault(p => p.code == wp.code && p.isdelete == false);
                    if (checkcode != null)
                    {
                        return Ok(new { code = "-1" });
                    }
                    var workingPaperCreate = new WorkingPaper
                    {
                        code = wp.code,
                        asigneeid = _userInfo.Id,
                        auditworkid = wp.auditworkid,
                        conclusion = wp.conclusion,
                        processid = wp.processid,
                        prototype = wp.prototype,
                        riskid = wp.riskid.ToString(),
                        //status = wp.status,
                        unitid = wp.unitid,
                        year = wp.year,
                        create_date = DateTime.Now,
                        isdelete = false,
                    };
                    context.WorkingPaper.Add(workingPaperCreate);
                    context.SaveChanges();

                    if (wp.listcontrol != null)
                        foreach (var controlAssessment in wp.listcontrol)
                        {
                            var listimg = new List<ControlAssessmentFile>();
                            if (controlAssessment.image1 != null)
                            {
                                var image1 = controlAssessment.image1.Select(p => new ControlAssessmentFile()
                                {
                                    control_assessment_id = controlAssessment.Id,
                                    file_type = p.ContentType,
                                    Path = CreateUploadURL(p, "WorkingPaper/"),
                                    isdelete = false,
                                    type = 1
                                }).ToList();
                                listimg.AddRange(image1);
                            }
                            if (controlAssessment.image2 != null)
                            {
                                var image2 = controlAssessment.image2.Select(p => new ControlAssessmentFile()
                                {
                                    control_assessment_id = controlAssessment.Id,
                                    file_type = p.ContentType,
                                    Path = CreateUploadURL(p, "WorkingPaper/"),
                                    isdelete = false,
                                    type = 2
                                }).ToList();
                                listimg.AddRange(image2);
                            }

                            var control = new ControlAssessment()
                            {
                                workingpaperid = workingPaperCreate.id,
                                riskid = controlAssessment.riskid,
                                controlid = controlAssessment.controlid,

                                designassessment = controlAssessment.designassessment,

                                designconclusion = controlAssessment.designconclusion,
                                effectiveassessment = controlAssessment.effectiveassessment,
                                effectiveconclusion = controlAssessment.effectiveconclusion,
                                ControlAssessmentFile = listimg,
                                ProceduresAssessment = controlAssessment.listprocedures != null ? controlAssessment.listprocedures.Select(p => new ProceduresAssessment()
                                {
                                    procedures_id = p.procedures_id,
                                    result = p.result,
                                }).ToList() : new List<ProceduresAssessment>(),
                            };
                            context.ControlAssessment.Add(control);
                            context.SaveChanges();
                        }
                    if (wp.listobserve != null)
                    {
                        foreach (var auditobserve in wp.listobserve)
                        {
                            var observe = new AuditObserve()
                            {
                                code = GetCodeAuditObserve(),
                                name = auditobserve.name,
                                discoverer = _userInfo.Id.Value,
                                year = auditobserve.year,
                                auditwork_id = auditobserve.auditwork_id,
                                auditwork_name = auditobserve.auditwork_name,
                                working_paper_id = workingPaperCreate.id,
                                CreatedAt = DateTime.Now,
                                CreatedBy = _userInfo.Id,
                                working_paper_code = workingPaperCreate.code,
                                Description = auditobserve.description,
                                select_state = false,
                                note = auditobserve.note,
                                type = auditobserve.type,
                                control_id = auditobserve.controlid,
                                //RiskId = auditobserve.riskid,
                                AuditObserveFile = auditobserve.evidence != null ? auditobserve.evidence.Select(p => new AuditObserveFile()
                                {
                                    path = CreateUploadURL(p, "WorkingPaper/"),
                                    file_type = p.ContentType,
                                    isdelete = false,
                                }).ToList() : new List<AuditObserveFile>(),
                            };
                            context.AuditObserve.Add(observe);
                            context.SaveChanges();
                        }
                    }
                    return Ok(new { code = "1", msg = "success", id = workingPaperCreate.id });
                }
                catch
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("Update")]
        public IActionResult Edit([FromForm] WorkingPaperModel wp)
        {
            try
            {
                using var context = new KitanoSqlContext();


                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }


                try
                {
                    var workingPaperEdit = context.WorkingPaper.Find(wp.id);
                    if (workingPaperEdit == null)
                        return NotFound();
                    workingPaperEdit.auditworkid = wp.auditworkid;
                    workingPaperEdit.conclusion = wp.conclusion;
                    workingPaperEdit.processid = wp.processid;
                    workingPaperEdit.prototype = wp.prototype;
                    workingPaperEdit.riskid = wp.riskid.ToString();
                    ////workingPaperEdit.status = wp.status;
                    workingPaperEdit.unitid = wp.unitid;
                    workingPaperEdit.year = wp.year;
                    workingPaperEdit.isdelete = false;
                    if (wp.listcontrol != null)
                    {
                        foreach (var controlAssessment in wp.listcontrol)
                        {
                            var listimg = new List<ControlAssessmentFile>();
                            var listriskwp = workingPaperEdit.riskid.Split(",").ToList();
                            var control = context.ControlAssessment.FirstOrDefault(p => p.workingpaperid == workingPaperEdit.id /*&& listriskwp.Contains((p.riskid ?? 0).ToString()) */&& p.controlid == controlAssessment.controlid);
                            if (control == null)
                            {
                                if (controlAssessment.image1 != null)
                                {
                                    var image1 = controlAssessment.image1.Select(p => new ControlAssessmentFile()
                                    {
                                        file_type = p.ContentType,
                                        Path = CreateUploadURL(p, "WorkingPaper/"),
                                        isdelete = false,
                                        type = 1
                                    }).ToList();
                                    listimg.AddRange(image1);
                                }
                                if (controlAssessment.image2 != null)
                                {
                                    var image2 = controlAssessment.image2.Select(p => new ControlAssessmentFile()
                                    {
                                        file_type = p.ContentType,
                                        Path = CreateUploadURL(p, "WorkingPaper/"),
                                        isdelete = false,
                                        type = 2
                                    }).ToList();
                                    listimg.AddRange(image2);
                                }

                                control = new ControlAssessment()
                                {

                                    workingpaperid = workingPaperEdit.id,
                                    //riskid = controlAssessment.riskid,
                                    controlid = controlAssessment.controlid,
                                    designassessment = controlAssessment.designassessment,
                                    designconclusion = controlAssessment.designconclusion,
                                    effectiveassessment = controlAssessment.effectiveassessment,
                                    effectiveconclusion = controlAssessment.effectiveconclusion,
                                    ControlAssessmentFile = listimg,
                                    sampleconclusion = controlAssessment.sampleConclusion,
                                    ProceduresAssessment = controlAssessment.listprocedures != null ? controlAssessment.listprocedures.Select(p => new ProceduresAssessment()
                                    {
                                        procedures_id = p.procedures_id,
                                        result = p.result,
                                    }).ToList() : new List<ProceduresAssessment>(),
                                };
                                context.ControlAssessment.Add(control);
                                context.SaveChanges();
                            }
                            else
                            {
                                if (controlAssessment.image1 != null)
                                {
                                    var image1 = controlAssessment.image1.Select(p => new ControlAssessmentFile()
                                    {
                                        control_assessment_id = control.Id,
                                        file_type = p.ContentType,
                                        Path = CreateUploadURL(p, "WorkingPaper/"),
                                        isdelete = false,
                                        type = 1
                                    }).ToList();
                                    listimg.AddRange(image1);
                                }
                                if (controlAssessment.image2 != null)
                                {
                                    var image2 = controlAssessment.image2.Select(p => new ControlAssessmentFile()
                                    {
                                        control_assessment_id = control.Id,
                                        file_type = p.ContentType,
                                        Path = CreateUploadURL(p, "WorkingPaper/"),
                                        isdelete = false,
                                        type = 2
                                    }).ToList();
                                    listimg.AddRange(image2);
                                }

                                control.designassessment = controlAssessment.designassessment;
                                control.designconclusion = controlAssessment.designconclusion;
                                control.effectiveassessment = controlAssessment.effectiveassessment;
                                control.effectiveconclusion = controlAssessment.effectiveconclusion;
                                control.sampleconclusion = controlAssessment.sampleConclusion;
                                if (listimg.Any()) context.ControllerAssessmentFile.AddRange(listimg);
                                if (controlAssessment.listprocedures != null)
                                {
                                    foreach (var item in controlAssessment.listprocedures)
                                    {
                                        var pa = context.ProceduresAssessment.FirstOrDefault(p => p.control_assessment_id == control.Id && p.procedures_id == item.procedures_id);
                                        if (pa != null)
                                        {
                                            context.ProceduresAssessment.FirstOrDefault(p => p.control_assessment_id == control.Id && p.procedures_id == item.procedures_id).result = item.result;
                                        }
                                        else
                                        {
                                            var newpa = new ProceduresAssessment()
                                            {
                                                control_assessment_id = control.Id,
                                                procedures_id = item.procedures_id,
                                                result = item.result,
                                            };
                                            context.ProceduresAssessment.Add(newpa);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (wp.listobserve != null)
                    {
                        var listid = wp.listobserve.Select(p => p.id).ToList();
                        context.AuditObserve.Where(p => !listid.Contains(p.id) && p.working_paper_id == workingPaperEdit.id && p.auditwork_id == workingPaperEdit.auditworkid).ToList().ForEach(p =>
                        {
                            p.IsDeleted = true;
                        });

                        foreach (var auditobserve in wp.listobserve)
                        {
                            var observe = context.AuditObserve.FirstOrDefault(p => p.working_paper_id == workingPaperEdit.id && p.auditwork_id == workingPaperEdit.auditworkid && p.control_id == auditobserve.controlid /*&& p.RiskId == auditobserve.riskid*/ && p.id == auditobserve.id);
                            if (observe == null)
                            {
                                observe = new AuditObserve()
                                {
                                    code = GetCodeAuditObserve(),
                                    name = auditobserve.name,
                                    discoverer = _userInfo.Id.Value,
                                    year = auditobserve.year,
                                    auditwork_id = auditobserve.auditwork_id,
                                    auditwork_name = auditobserve.auditwork_name,
                                    working_paper_id = workingPaperEdit.id,
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = _userInfo.Id,
                                    working_paper_code = workingPaperEdit.code,
                                    Description = auditobserve.description,
                                    select_state = false,
                                    note = auditobserve.note,
                                    type = auditobserve.type,
                                    control_id = auditobserve.controlid,
                                    //RiskId = auditobserve.riskid,
                                    AuditObserveFile = auditobserve.evidence != null ? auditobserve.evidence.Select(p => new AuditObserveFile()
                                    {
                                        path = CreateUploadURL(p, "WorkingPaper/" + workingPaperEdit.code + "/"),
                                        file_type = p.ContentType,
                                        isdelete = false,
                                    }).ToList() : new List<AuditObserveFile>(),
                                };
                                context.AuditObserve.Add(observe);
                                context.SaveChanges();
                            }
                            else
                            {
                                observe.name = auditobserve.name;
                                observe.year = auditobserve.year;
                                observe.auditwork_id = auditobserve.auditwork_id;
                                observe.auditwork_name = auditobserve.auditwork_name;
                                observe.working_paper_id = workingPaperEdit.id;
                                observe.working_paper_code = workingPaperEdit.code;
                                observe.Description = auditobserve.description;
                                observe.note = auditobserve.note;
                                observe.type = auditobserve.type;
                                observe.control_id = auditobserve.controlid;
                                //observe.RiskId = auditobserve.riskid;
                                if (auditobserve.evidence != null && auditobserve.evidence.Any())
                                {
                                    context.AuditObserveFile.AddRange(auditobserve.evidence.Select(p => new AuditObserveFile()
                                    {
                                        observe_id = observe.id,
                                        path = CreateUploadURL(p, "WorkingPaper/" + workingPaperEdit.code + "/"),
                                        file_type = p.ContentType,
                                        isdelete = false,
                                    }).ToList());
                                }
                            }
                        }
                    }
                    else
                    {
                        context.AuditObserve.Where(p => p.working_paper_id == wp.id).ToList().ForEach(p =>
                        {
                            p.IsDeleted = true;
                        });
                    }
                    context.SaveChanges();
                    return Ok(new { code = "1", msg = "success", id = workingPaperEdit.id });
                }
                catch
                {
                    return BadRequest();
                }
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
                using var context = new KitanoSqlContext();

                var working_paper = (from a in context.WorkingPaper
                                     join b in context.Department on a.unitid equals b.Id
                                     join c in context.AuditWork on a.auditworkid equals c.Id
                                     join d in context.AuditProcess on a.processid equals d.Id
                                     where a.id == id
                                     select new WorkingPaperModel()
                                     {
                                         id = a.id,
                                         year = a.year.Value,
                                         unitid = a.unitid.Value,
                                         unitname = b.Name,
                                         auditworkid = a.auditworkid.Value,
                                         auditworkname = c.Name,
                                         asigneeid = a.asigneeid ?? 0,
                                         processid = a.processid.Value,
                                         processname = d.Name,
                                         prototype = a.prototype ?? "",
                                         code = a.code,
                                         conclusion = a.conclusion,
                                         riskid = a.riskid,
                                         risk_str = a.risk_str,
                                     }).FirstOrDefault();

                var approval_status = _uow.Repository<ApprovalFunction>().Include(x => x.Users).FirstOrDefault(a => a.function_code == "M_WP" && working_paper.id == a.item_id);
                if (working_paper != null)
                {
                    working_paper.riskname = string.Join(",", context.CatRisk.Where(p => working_paper.risk_str.Contains(p.Id.ToString())).Select(p => p.Name).ToList());
                    var control_assessment = _uow.Repository<ControlAssessment>().Include(x => x.ControlAssessmentFile).Where(x => x.workingpaperid == working_paper.id /*&& working_paper.risk_str.Contains((x.riskid ?? 0).ToString())*/ ).Select(ca => new ControlAssessmentModel()
                    {
                        Id = ca.Id,
                        workingpaperid = ca.workingpaperid.Value,
                        //riskid = ca.riskid.Value,
                        controlid = ca.controlid.Value,
                        designassessment = ca.designassessment,
                        designconclusion = ca.designconclusion,
                        effectiveassessment = ca.effectiveassessment,
                        effectiveconclusion = ca.effectiveconclusion,
                        sampleConclusion = ca.sampleconclusion,
                        image1name = ca.ControlAssessmentFile.Where(cf => cf.type == 1 && cf.isdelete == false).Select(cf => new ControlAssessmentFileModal { id = cf.id, name = cf.Path }).ToList(),
                        image2name = ca.ControlAssessmentFile.Where(cf => cf.type == 2 && cf.isdelete == false).Select(cf => new ControlAssessmentFileModal { id = cf.id, name = cf.Path }).ToList(),
                        listprocedures = ca.ProceduresAssessment.Select(pa => new ProceduesAssessmentModel()
                        {
                            Id = pa.Id,
                            control_assessment_id = pa.control_assessment_id.Value,
                            result = pa.result,
                            procedures_id = pa.procedures_id,
                        }).ToList()
                    }).ToList();
                    var audit_observe = _uow.Repository<AuditObserve>().Include(x => x.AuditObserveFile).Where(x => x.auditwork_id == working_paper.auditworkid && x.working_paper_id == working_paper.id && x.IsDeleted != true /*&& working_paper.risk_str.Contains((x.RiskId ?? 0).ToString())*/).Select(ao => new AuditObserveModel()
                    {
                        code = ao.code,
                        id = ao.id,
                        controlid = ao.control_id.Value,
                        //riskid = ao.RiskId.Value,
                        name = ao.name,
                        year = ao.year,
                        auditwork_id = ao.auditwork_id.Value,
                        auditwork_name = ao.auditwork_name,
                        description = ao.Description,
                        note = ao.note,
                        evidencename = ao.AuditObserveFile.Where(p => p.isdelete == false).Select(af => new AuditObserveFileModal { id = af.id, name = af.path }).ToList(),
                        type = ao.type.Value,
                        workingpaperid = ao.working_paper_id.Value,
                    }).ToList();
                    var assign = _uow.Repository<Users>().FirstOrDefault(a => a.Id == working_paper.asigneeid)?.FullName;
                    var review = "";
                    if (approval_status != null)
                    {
                        working_paper.reviewerid = approval_status.approver_last.HasValue ? approval_status.approver_last.Value : approval_status.approver.Value;
                        review = approval_status != null ? (approval_status.approver_last.HasValue && (approval_status.StatusCode == "3.1" || approval_status.StatusCode == "3.2" || approval_status.StatusCode == "0.0") ? approval_status.Users_Last.FullName : (approval_status.approver.HasValue ? approval_status.Users.FullName : "")) : "";
                    }
                    var leader = _uow.Repository<AuditWorkScopeUserMapping>().Include(x => x.AuditWorkScope).FirstOrDefault(a => a.AuditWorkScope.auditwork_id == working_paper.auditworkid && a.AuditWorkScope.auditprocess_id == working_paper.processid && a.AuditWorkScope.auditfacilities_id == working_paper.unitid);
                    working_paper.asigneename = assign ?? "";
                    working_paper.reviewername = review ?? "";
                    working_paper.listcontrol = control_assessment;
                    working_paper.listobserve = audit_observe;
                    working_paper.leader = leader != null ? (leader?.id) + ":" + (leader.full_name) : "";
                    working_paper.status = approval_status?.StatusCode ?? "1.0";
                    working_paper.approvedate = approval_status?.ApprovalDate?.ToString("dd/MM/yyyy") ?? "";
                    var result = working_paper;
                    return Ok(new { code = "1", msg = "success", data = result });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
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
                var workingpaper = _uow.Repository<WorkingPaper>().FirstOrDefault(a => a.id == id);
                if (workingpaper == null)
                {
                    return NotFound();
                }
                var observe = _uow.Repository<AuditObserve>().Include(p => p.AuditDetect).Where(p => p.working_paper_id == id && p.audit_detect_id.HasValue && !(p.AuditDetect.IsDeleted ?? false) && !(p.IsDeleted ?? false));
                if (observe.Any())
                {
                    return Ok(new { code = "7", msg = "Error" });
                }

                _uow.Repository<AuditObserve>().Find(p => p.working_paper_id == id).ToList().ForEach(p => p.IsDeleted = true);
                workingpaper.isdelete = true;
                _uow.Repository<WorkingPaper>().Update(workingpaper);
                return Ok(new { code = "1", msg = "success", id = workingpaper.id });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("GetRiskWorkingPaper")]
        public IActionResult GetRiskWorkingPaper(int auditid, int processid, int unitid, bool detail)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var workingpaper_catrisk = detail ? new List<string>() : _uow.Repository<WorkingPaper>().Find(p => p.auditworkid == auditid && p.processid == processid && p.unitid == unitid && p.isdelete == false).AsEnumerable().SelectMany(p => p.risk_str).ToList();
                var auditprogram = _uow.Repository<AuditProgram>().FirstOrDefault(a => a.auditwork_id == auditid && a.auditprocess_id == processid && a.auditfacilities_id == unitid && a.IsDeleted != true);
                if (auditprogram != null)
                {
                    var ProcessLevelRiskScoring = _uow.Repository<ProcessLevelRiskScoring>().Include(a => a.CatRisk).Where(a => a.auditprogram_id == auditprogram.Id && a.IsDeleted != true && a.AuditProposal == true && !workingpaper_catrisk.Contains(a.catrisk_id.ToString())).ToArray();
                    if (ProcessLevelRiskScoring.Any())
                    {
                        IEnumerable<ProcessLevelRiskScoring> data = ProcessLevelRiskScoring;


                        var user = data.Select(a => new CatRiskModel()
                        {
                            Id = a.catrisk_id,
                            Name = a.catrisk_id.HasValue ? a.CatRisk.Name : "",
                        }).GroupBy(a => a.Id).Select(a => new AuditWorkScopeUnitModel()
                        {
                            id = a.Key,
                            name = a.FirstOrDefault()?.Name,
                        });

                        return Ok(new { code = "1", msg = "success", data = user });
                    }
                    return Ok(new { code = "1", msg = "success", data = "" });
                }
                return Ok(new { code = "1", msg = "success", data = "" });
            }
            catch (Exception)
            {
                return Ok(new { code = "1", msg = "success", data = "" });
            }
        }

        [HttpGet("GetControlWorkingPaper/{id}")]
        public IActionResult SearchControl(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                using var context = new KitanoSqlContext();

                Expression<Func<RiskControl, bool>> _filter = c => (id == 0 || c.riskid == id);
                var _list_risk = _uow.Repository<RiskControl>().Include(a => a.CatControl).Where(_filter).OrderByDescending(a => a.Id).Select(a => new CatControlSearchModel()
                {
                    Id = a.CatControl.Id,
                    Name = a.CatControl.Name,
                    Description = a.CatControl.Description,
                    Code = a.CatControl.Code,
                    Controlfrequency = a.CatControl.Controlfrequency,
                    Controltype = a.CatControl.Controltype,
                    Controlformat = a.CatControl.Controlformat,
                    Riskcontrolid = a.riskid
                }).ToList();

                var list_controlid = _list_risk.Select(p => p.Id).ToList();
                var workingpaper = (from a in context.ControlAssessment
                                    join b in context.WorkingPaper on a.workingpaperid equals b.id
                                    where list_controlid.Contains(a.controlid.Value) && !(b.isdelete ?? false)
                                    select new ControlAssessmentModel
                                    {
                                        workingpaperid = a.workingpaperid.Value,
                                        controlid = a.controlid.Value,
                                        workingpapercode = b.code,
                                        riskid = id,

                                    });
                foreach (var item in _list_risk)
                {
                    var l = workingpaper.Where(p => p.controlid == item.Id).ToList();
                    if (l.Any())
                    {
                        item.controlassassment = l;
                    }
                }
                return Ok(new { code = "1", msg = "success", data = _list_risk });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("GetControlWorkingPaper_str")]
        public IActionResult SearchControl(string strid)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                using var context = new KitanoSqlContext();
                var list = string.IsNullOrEmpty(strid) ? new List<int>() : strid.Split(',').Select(p => Convert.ToInt32(p)).ToList();
                Expression<Func<RiskControl, bool>> _filter = c => list.Contains(c.riskid) && c.isdeleted != true;
                var _list_risk = _uow.Repository<RiskControl>().Include(a => a.CatControl, a => a.CatRisk).Where(_filter).OrderByDescending(a => a.Id).Select(a => new CatControlSearchModel()
                {
                    Id = a.CatControl.Id,
                    Name = a.CatControl.Name,
                    Description = a.CatControl.Description,
                    Code = a.CatControl.Code,
                    Controlfrequency = a.CatControl.Controlfrequency,
                    Controltype = a.CatControl.Controltype,
                    Controlformat = a.CatControl.Controlformat,
                    Riskcontrolid = a.riskid,
                    RiskCode = a.CatRisk.Code
                }).ToList();

                var list_controlid = _list_risk.Select(p => p.Id).ToList();
                var workingpaper = (from a in context.ControlAssessment
                                    join b in context.WorkingPaper on a.workingpaperid equals b.id
                                    where list_controlid.Contains(a.controlid.Value) && !(b.isdelete ?? false)
                                    select new ControlAssessmentModel
                                    {
                                        workingpaperid = a.workingpaperid.Value,
                                        controlid = a.controlid.Value,
                                        workingpapercode = b.code,
                                        riskid = a.riskid.Value,

                                    });
                foreach (var item in _list_risk)
                {
                    var l = workingpaper.Where(p => p.controlid == item.Id).ToList();
                    if (l.Any())
                    {
                        item.controlassassment = l;
                    }
                }
                return Ok(new { code = "1", msg = "success", data = _list_risk });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("GetControlFromRisk_str")]
        public IActionResult SearchControlFormRisk(string strid)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                using var context = new KitanoSqlContext();
                var list = string.IsNullOrEmpty(strid) ? new List<int>() : strid.Split(',').Select(p => Convert.ToInt32(p)).ToList();
                Expression<Func<RiskControl, bool>> _filter = c => list.Contains(c.riskid) && c.isdeleted != true;

                var _list_risk_temp = _uow.Repository<RiskControl>().Include(x => x.CatControl, x => x.CatRisk).Where(_filter).ToArray();
                var _list_risk_all = _uow.Repository<CatControl>().Find(a => a.RiskControl.Any(x => list.Contains(x.riskid) && x.isdeleted != true) && a.IsDeleted != true).ToArray();
                var _list_risk = _list_risk_all.Select(a => new CatControlSearchModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Code = a.Code,
                    Controlfrequency = a.Controlfrequency,
                    Controltype = a.Controltype,
                    Controlformat = a.Controlformat,
                    RiskCode = string.Join(", ", _list_risk_temp.Where(x => x.controlid == a.Id).Select(x => x.CatRisk.Code).Distinct()),
                }).ToList();

                var list_controlid = _list_risk.Select(p => p.Id).ToList();
                var workingpaper = (from a in context.ControlAssessment
                                    join b in context.WorkingPaper on a.workingpaperid equals b.id
                                    where list_controlid.Contains(a.controlid.Value) && !(b.isdelete ?? false)
                                    select new ControlAssessmentModel
                                    {
                                        workingpaperid = a.workingpaperid.Value,
                                        controlid = a.controlid.Value,
                                        workingpapercode = b.code,
                                    });
                foreach (var item in _list_risk)
                {
                    var l = workingpaper.Where(p => p.controlid == item.Id).ToList();
                    if (l.Any())
                    {
                        item.controlassassment = l;
                    }
                }
                return Ok(new { code = "1", msg = "success", data = _list_risk });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        ////[HttpPost("RequestApproval")]
        ////public IActionResult RequestApproval(WorkingPaperRequestApprovalModel model)
        ////{
        ////    try
        ////    {
        ////        if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
        ////        {
        ////            return Unauthorized();
        ////        }
        ////        var checkWorkingPaper = _uow.Repository<WorkingPaper>().FirstOrDefault(a => a.id == model.working_paper_id);
        ////        if (checkWorkingPaper == null)
        ////        {
        ////            return NotFound();
        ////        }
        ////        if (checkWorkingPaper.status == 1 || checkWorkingPaper.status == 4)
        ////        {
        ////            checkWorkingPaper.status = 2;
        ////            checkWorkingPaper.reviewerid = model.approvaluser;
        ////            _uow.Repository<WorkingPaper>().Update(checkWorkingPaper);
        ////            return Ok(new { code = "1", msg = "success", id = model.working_paper_id });
        ////        }
        ////        else
        ////        {
        ////            return BadRequest();
        ////        }

        ////    }
        ////    catch (Exception)
        ////    {
        ////        return BadRequest();
        ////    }
        ////}

        ////[HttpPost("SubmitApproval/{id}")]
        ////public IActionResult SubmitApproval(int id)
        ////{
        ////    try
        ////    {
        ////        var _id = id;
        ////        if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
        ////        {
        ////            return Unauthorized();
        ////        }
        ////        var checkWorkingPaper = _uow.Repository<WorkingPaper>().FirstOrDefault(a => a.id == id);
        ////        if (checkWorkingPaper == null)
        ////        {
        ////            return NotFound();
        ////        }
        ////        checkWorkingPaper.status = 3;
        ////        checkWorkingPaper.approvedate = DateTime.Now;
        ////        _uow.Repository<WorkingPaper>().Update(checkWorkingPaper);
        ////        return Ok(new { code = "1", msg = "success", id = _id });
        ////    }
        ////    catch (Exception)
        ////    {
        ////        return BadRequest();
        ////    }
        ////}

        ////[HttpPost("RejectApproval")]
        ////public IActionResult RejectApproval(WorkingPaperRejectApprovalModel model)
        ////{
        ////    try
        ////    {
        ////        if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
        ////        {
        ////            return Unauthorized();
        ////        }
        ////        var checkWorkingPaper = _uow.Repository<WorkingPaper>().FirstOrDefault(a => a.id == model.working_paper_id);
        ////        if (checkWorkingPaper == null)
        ////        {
        ////            return NotFound();
        ////        }
        ////        checkWorkingPaper.status = 4;
        ////        checkWorkingPaper.reason_reject = model.reason_reject;
        ////        _uow.Repository<WorkingPaper>().Update(checkWorkingPaper);
        ////        return Ok(new { code = "1", msg = model.reason_reject, id = model.working_paper_id });
        ////    }
        ////    catch (Exception)
        ////    {
        ////        return BadRequest();
        ////    }
        ////}

        //tự sinh mã WorkingPaper
        public static string GetCodeWorkingPaper()
        {
            string EID = string.Empty;
            try
            {
                var yearnow = DateTime.Now.Year;
                string str_start = "WP." + yearnow;

                using var context = new KitanoSqlContext();

                var list = context.WorkingPaper;
                var data = list.Where(a => !string.IsNullOrEmpty(a.code) && a.code.StartsWith(str_start));
                var value = data.Any() ? data.Count() + 1 : 1;

                for (int i = value; ; i++)
                {
                    if (i <= 9)
                    {
                        EID = str_start + ".000" + i.ToString();
                    }
                    else if (i > 9 && i <= 99)
                    {
                        EID = str_start + ".00" + i.ToString();
                    }
                    else if (i > 99 && i <= 999)
                    {
                        EID = str_start + ".0" + i.ToString();
                    }
                    var data_ = list.Where(a => a.code == EID);
                    if (!data_.Any())
                    {
                        break;
                    }
                }


                return EID;

            }
            catch (Exception)
            {

                return EID;
            }

        }

        //tự sinh mã AuditObserve
        public static string GetCodeAuditObserve()
        {
            string EID = string.Empty;
            try
            {
                var yearnow = DateTime.Now.Year;
                string str_start = "QS." + yearnow;

                using var context = new KitanoSqlContext();

                var list = context.AuditObserve;
                var data = list.Where(a => !string.IsNullOrEmpty(a.code) && a.code.StartsWith(str_start));
                var value = data.Any() ? data.Count() + 1 : 1;
                for (int i = value; ; i++)
                {
                    if (i <= 9)
                    {
                        EID = str_start + ".000" + i.ToString();
                    }
                    else if (i > 9 && i <= 99)
                    {
                        EID = str_start + ".00" + i.ToString();
                    }
                    else if (i > 99 && i <= 999)
                    {
                        EID = str_start + ".0" + i.ToString();
                    }
                    var data_ = list.Where(a => a.code == EID);
                    if (!data_.Any())
                    {
                        break;
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
                using var fileStream = new FileStream(filePath, FileMode.Create);
                imageFile.CopyTo(fileStream);

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

        [HttpGet("PrivateCode")]
        public IActionResult PrivateCode()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var auditDetect = _uow.Repository<WorkingPaper>().GetAll();
                if (auditDetect != null)
                {
                    return Ok(new { code = "1", msg = "success", privatecode = GetCodeWorkingPaper() });
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

        [HttpGet("ObserveCode")]
        public IActionResult ObserveCode()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var yearnow = DateTime.Now.Year;
                string str_start = "QS." + yearnow;

                using var context = new KitanoSqlContext();
                var auditObserve = _uow.Repository<AuditObserve>().GetAll(a => !string.IsNullOrEmpty(a.code) && a.code.StartsWith(str_start)).Count() + 1;
                return Ok(new { code = "1", msg = "success", observecode = auditObserve });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("DownloadAttach")]
        public IActionResult DonwloadFile(int id, string table)
        {
            try
            {
                var path = "";
                var filetype = "";
                if (table == "auditobservefile")
                {
                    var self = _uow.Repository<AuditObserveFile>().FirstOrDefault(o => o.id == id);
                    if (self == null)
                    {
                        return NotFound();
                    }
                    path = self.path;
                    filetype = self.file_type;
                }
                if (table == "controlassessmentfile")
                {
                    var self = _uow.Repository<ControlAssessmentFile>().FirstOrDefault(o => o.id == id);
                    if (self == null)
                    {
                        return NotFound();
                    }
                    path = self.Path;
                    filetype = self.file_type;
                }

                var fullPath = Path.Combine(_config["Upload:AuditDocsPath"], path);
                var name = "DownLoadFile";
                if (!string.IsNullOrEmpty(path))
                {
                    var _array = path.Split("\\");
                    name = _array[^1];
                }
                var fs = new FileStream(fullPath, FileMode.Open);

                return File(fs, filetype, name);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet("DeleteAttach")]
        public IActionResult DeleteFile(int id, string table, string form)
        {
            try
            {
                var _id = id;
                var _table = table;
                var _form = form;
                if (table == "auditobservefile")
                {
                    var self = _uow.Repository<AuditObserveFile>().FirstOrDefault(o => o.id == id);
                    if (self == null)
                    {
                        return NotFound();
                    }
                    var check = false;
                    if (!string.IsNullOrEmpty(self.path))
                    {
                        var fullPath = Path.Combine(_config["Upload:AuditDocsPath"], self.path);
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
                    if (check)
                    {
                        self.isdelete = true;
                        _uow.Repository<AuditObserveFile>().Update(self);
                    }
                }
                if (table == "controlassessmentfile")
                {
                    var self = _uow.Repository<ControlAssessmentFile>().FirstOrDefault(o => o.id == id);
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
                    if (check)
                    {
                        self.isdelete = true;
                        _uow.Repository<ControlAssessmentFile>().Update(self);
                    }
                }

                return Ok(new { code = "1", msg = "success", id = _id, table = _table, form = _form });
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpGet("ListStatusWorkingPaper")]
        public IActionResult ListStatusWorkingPaper()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalConfig>().GetAll(a => a.item_code == "M_WP").ToArray().OrderBy(x => x.StatusCode);

                var dt = approval_status.Select(a => new DropListStatusPrepareAuditPlanModel()
                {
                    id = a.id,
                    status_code = a.StatusCode,
                    status_name = a.StatusName,
                });
                return Ok(new { code = "1", msg = "success", data = dt });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new DropListYearApprovedModel() });
            }
        }

        public class SystemParam
        {
            public int? id { get; set; }
            public string name { get; set; }
            public string value { get; set; }
        }

        [HttpPost("ExportExcel/{id}")]
        public IActionResult ExportExcel(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                using var context = new KitanoSqlContext();
                var working_paper = _uow.Repository<WorkingPaper>().FirstOrDefault(a => a.id == id && a.isdelete != true);
                if (working_paper == null)
                {
                    return NotFound();
                }
                var user = _uow.Repository<Users>().FirstOrDefault(a => working_paper.asigneeid == a.Id && a.IsDeleted != true);
                var reviewuser = _uow.Repository<Users>().FirstOrDefault(a => working_paper.reviewerid == a.Id && a.IsDeleted != true);

                var list = string.IsNullOrEmpty(working_paper.riskid) ? new List<int>() : working_paper.riskid.Split(',').Select(p => Convert.ToInt32(p)).ToList();
                Expression<Func<RiskControl, bool>> _filter = c => list.Contains(c.riskid) && c.isdeleted != true;

                var _list_control_temp = _uow.Repository<RiskControl>().Include(x => x.CatControl, x => x.CatRisk).Where(_filter).ToArray();
                var _list_control_all = _uow.Repository<CatControl>().Find(a => a.RiskControl.Any(x => list.Contains(x.riskid)) && a.IsDeleted != true).ToArray();
                var _list_control = _list_control_all.Select(a => new CatControlSearchModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Code = a.Code,
                    Controlfrequency = a.Controlfrequency,
                    Controltype = a.Controltype,
                    Controlformat = a.Controlformat,
                    RiskCode = string.Join(", ", _list_control_temp.Where(x => x.controlid == a.Id).Select(x => x.CatRisk.Code).Distinct()),
                }).ToList();

                var list_controlid = _list_control.Select(p => p.Id).ToList();
                var workingpaper = (from a in context.ControlAssessment
                                    join b in context.WorkingPaper on a.workingpaperid equals b.id
                                    where list_controlid.Contains(a.controlid.Value) && !(b.isdelete ?? false)
                                    select new ControlAssessmentModel
                                    {
                                        workingpaperid = a.workingpaperid.Value,
                                        controlid = a.controlid.Value,
                                        workingpapercode = b.code,
                                        designassessment = a.designassessment,
                                        designconclusion = a.designconclusion,
                                        effectiveassessment = a.effectiveassessment,
                                        effectiveconclusion = a.effectiveconclusion,
                                    });
                foreach (var item in _list_control)
                {
                    var l = workingpaper.Where(p => p.controlid == item.Id).ToList();
                    if (l.Any())
                    {
                        item.controlassassment = l;
                    }
                }

                var audit_observe = _uow.Repository<AuditObserve>().Include(x => x.AuditObserveFile).Where(x => x.auditwork_id == working_paper.auditworkid && x.working_paper_id == working_paper.id && x.IsDeleted != true /*&& working_paper.risk_str.Contains((x.RiskId ?? 0).ToString())*/).Select(ao => new AuditObserveModel()
                {
                    code = ao.code,
                    id = ao.id,
                    controlid = ao.control_id.Value,
                    //riskid = ao.RiskId.Value,
                    name = ao.name,
                    year = ao.year,
                    auditwork_id = ao.auditwork_id.Value,
                    auditwork_name = ao.auditwork_name,
                    description = ao.Description,
                    note = ao.note,
                    evidencename = ao.AuditObserveFile.Where(p => p.isdelete == false).Select(af => new AuditObserveFileModal { id = af.id, name = af.path }).ToList(),
                    type = ao.type.Value,
                    workingpaperid = ao.working_paper_id.Value,
                }).ToList();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_GiayToLamViec_v02.xlsx");
                var template = new FileInfo(fullPath);
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var _paramInfoPrefix = "Param.SystemInfo";
                    var worksheet = excelPackage.Workbook.Worksheets["Danh_gia_kiem_soat"];
                    var worksheet2 = excelPackage.Workbook.Worksheets["Phat_hien_kiem_toan"];
                    var iCache = (IConnectionMultiplexer)HttpContext.RequestServices
                     .GetService(typeof(IConnectionMultiplexer));
                    if (!iCache.IsConnected)
                    {
                        return BadRequest();
                    }
                    var redisDb = iCache.GetDatabase();
                    var value_get = redisDb.StringGet(_paramInfoPrefix);
                    string papercode = working_paper.code;

                    var company = "";
                    if (value_get.HasValue)
                    {
                        var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
                        company = list_param.FirstOrDefault(a => a.name == "COMPANY_NAME")?.value;
                    }
                    ExcelRange cellCompany = worksheet.Cells[1, 2];
                    cellCompany.Value = company;

                    ExcelRange cellCode = worksheet.Cells[2, 3];
                    cellCode.Value = working_paper.code;
                    cellCode.Style.Font.Size = 11;

                    ExcelRange cellYear = worksheet.Cells[4, 3];
                    cellYear.Value = working_paper.year;
                    cellYear.Style.Font.Size = 11;

                    ExcelRange cellAuditWork = worksheet.Cells[5, 3];
                    cellAuditWork.Value = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == working_paper.auditworkid && a.IsDeleted != true).Code;
                    cellAuditWork.Style.Font.Size = 11;

                    ExcelRange cellUnit = worksheet.Cells[6, 3];
                    cellUnit.Value = _uow.Repository<AuditFacility>().FirstOrDefault(a => a.Id == working_paper.unitid).Name;
                    cellUnit.Style.Font.Size = 11;

                    ExcelRange cellProcess = worksheet.Cells[7, 3];
                    cellProcess.Value = _uow.Repository<AuditProcess>().FirstOrDefault(a => a.Id == working_paper.processid).Name;
                    cellProcess.Style.Font.Size = 11;

                    ExcelRange cellAssignee = worksheet.Cells[5, 8];
                    cellAssignee.Value = user != null ? user.FullName : "";
                    cellAssignee.Style.Font.Size = 11;

                    ExcelRange cellCreateDate = worksheet.Cells[5, 9];
                    cellCreateDate.Value = working_paper.create_date != null ? working_paper.create_date?.ToString("dd/MM/yyyy") : "";
                    cellCreateDate.Style.Font.Size = 11;

                    ExcelRange cellReviewer = worksheet.Cells[6, 8];
                    cellReviewer.Value = reviewuser != null ? reviewuser.FullName : "";
                    cellReviewer.Style.Font.Size = 11;

                    ExcelRange cellReviewDate = worksheet.Cells[6, 9];
                    cellReviewDate.Value = working_paper.approvedate != null ? working_paper.approvedate?.ToString("dd/MM/yyyy") : "";
                    cellReviewDate.Style.Font.Size = 11;

                    var _startrow = 12;
                    var startrow = 12;
                    var startcol = 1;
                    var count = 0;

                    foreach (var a in _list_control)
                    {
                        count++;
                        ExcelRange cellNo = worksheet.Cells[startrow, startcol];
                        cellNo.Value = count;

                        var _list_risk = _uow.Repository<RiskControl>().Include(a => a.CatRisk).Where(c => (c.controlid == a.Id)).OrderByDescending(a => a.Id).Select(a => new CatRiskSearchModel()
                        {
                            Riskcontrolid = a.Id,
                            Id = a.CatRisk.Id,
                            name = a.CatRisk.Code + ":\n" + a.CatRisk.Description,
                            Description = a.CatRisk.Description,
                            code = a.CatRisk.Code
                        });

                        ExcelRange cellRisk = worksheet.Cells[startrow, startcol + 1];
                        cellRisk.Value = string.Join("\n", _list_risk.Select(x => x.name).Distinct());

                        ExcelRange cellControlcode = worksheet.Cells[startrow, startcol + 2];
                        cellControlcode.Value = _uow.Repository<CatControl>().FirstOrDefault(x => x.Id == a.Id).Code;

                        ExcelRange cellControlText = worksheet.Cells[startrow, startcol + 3];
                        cellControlText.Value = _uow.Repository<CatControl>().FirstOrDefault(x => x.Id == a.Id).Description;

                        var controlfrequency = _uow.Repository<CatControl>().FirstOrDefault(x => x.Id == a.Id).Controlfrequency ?? 0;
                        var controltype = _uow.Repository<CatControl>().FirstOrDefault(x => x.Id == a.Id).Controltype ?? 0;
                        var controlformat = _uow.Repository<CatControl>().FirstOrDefault(x => x.Id == a.Id).Controlformat ?? 0;
                        var _tansuat = "";
                        var _loai = "";
                        var _hinhthuc = "";

                        switch (controlfrequency)
                        {
                            case 1:
                                _tansuat = "Mỗi khi phát sinh";
                                break;
                            case 2:
                                _tansuat = "Nhiều lần trong ngày";
                                break;
                            case 3:
                                _tansuat = "Hàng ngày";
                                break;
                            case 4:
                                _tansuat = "Hàng tuần";
                                break;
                            case 5:
                                _tansuat = "Hàng tháng";
                                break;
                            case 6:
                                _tansuat = "Hàng quý";
                                break;
                            case 7:
                                _tansuat = "Hàng năm";
                                break;
                        }

                        switch (controltype)
                        {
                            case 1:
                                _loai = "Phòng ngừa";
                                break;
                            case 2:
                                _loai = "Phát hiện";
                                break;
                        }

                        switch (controlformat)
                        {
                            case 1:
                                _hinhthuc = "Tự động";
                                break;
                            case 2:
                                _hinhthuc = "Bán tự động";
                                break;
                            case 3:
                                _hinhthuc = "Thủ công";
                                break;
                        }

                        ExcelRange cellControlFrequency = worksheet.Cells[startrow, startcol + 4];
                        cellControlFrequency.Value = _tansuat;

                        ExcelRange cellControlType = worksheet.Cells[startrow, startcol + 5];
                        cellControlType.Value = _loai;

                        ExcelRange cellControlFormat = worksheet.Cells[startrow, startcol + 6];
                        cellControlFormat.Value = _hinhthuc;

                        var _list_procedure = _uow.Repository<CatAuditProcedures>().Include(a => a.CatControl).Where(c => (c.cat_control_id == a.Id)).OrderByDescending(a => a.Id).Select(a => new CatAuditProceduresModel()
                        {
                            Id = a.Id,
                            Name = a.Code + ":\n" + a.Description + "\n",
                            Code = a.Code,
                        });

                        ExcelRange cellProcedure = worksheet.Cells[startrow, startcol + 7];
                        cellProcedure.Value = string.Join(",\n", _list_procedure.Select(x => x.Name).Distinct());

                        var _list_result = _uow.Repository<ProceduresAssessment>().Include(d => d.ControlAssessment).Where(d => d.ControlAssessment.controlid == a.Id && d.ControlAssessment.workingpaperid == id).Select(a => new ProceduesAssessmentModel()
                        {
                            result = a.result + ":\n",
                        });

                        ExcelRange cellResult = worksheet.Cells[startrow, startcol + 8];
                        cellResult.Value = string.Join(",\n", _list_result.Select(x => x.result).Distinct());

                        ExcelRange cellSample = worksheet.Cells[startrow, startcol + 9];
                        var sample = _uow.Repository<ControlAssessment>().FirstOrDefault(x => x.controlid == a.Id);
                        if (sample != null)
                        {
                            cellSample.Value = sample.sampleconclusion;
                        }
                        else
                        {
                            cellSample.Value = "";
                        }


                        ExcelRange cellDesignControlText = worksheet.Cells[startrow, startcol + 10];
                        cellDesignControlText.Value = string.Join(", ", workingpaper.Where(x => x.controlid == a.Id && x.workingpaperid == id).Select(x => x.designassessment));

                        ExcelRange cellDesignControlConclusion = worksheet.Cells[startrow, startcol + 11];
                        cellDesignControlConclusion.Value = string.Join(", ", workingpaper.Where(x => x.controlid == a.Id && x.workingpaperid == id).Select(x => x.designconclusion));

                        ExcelRange cellEffectControlText = worksheet.Cells[startrow, startcol + 12];
                        cellEffectControlText.Value = string.Join(", ", workingpaper.Where(x => x.controlid == a.Id && x.workingpaperid == id).Select(x => x.effectiveassessment));

                        ExcelRange cellEffectControlConclusion = worksheet.Cells[startrow, startcol + 13];
                        cellEffectControlConclusion.Value = string.Join(", ", workingpaper.Where(x => x.controlid == a.Id && x.workingpaperid == id).Select(x => x.effectiveconclusion));

                        ExcelRange cellAuditDetect = worksheet.Cells[startrow, startcol + 14];
                        cellAuditDetect.Value = string.Join(",\n ", audit_observe.Where(x => x.controlid == a.Id && x.workingpaperid == id).Select(x => x.code));
                        startrow++;
                    }

                    var _startrow2 = 3;
                    var startrow2 = 3;
                    var startcol2 = 1;
                    var count2 = 0;

                    foreach (var b in audit_observe)
                    {
                        count2++;
                        ExcelRange cellNo2 = worksheet2.Cells[startrow2, startcol2];
                        cellNo2.Value = count2;

                        ExcelRange cellControlcode2 = worksheet2.Cells[startrow2, startcol2 + 1];
                        cellControlcode2.Value = _uow.Repository<CatControl>().FirstOrDefault(x => x.Id == b.controlid).Code;

                        ExcelRange cellDetectcode2 = worksheet2.Cells[startrow2, startcol2 + 2];
                        cellDetectcode2.Value = b.code;

                        ExcelRange cellDescription2 = worksheet2.Cells[startrow2, startcol2 + 3];
                        cellDescription2.Value = b.description;
                        startrow2++;
                    }

                    using ExcelRange r = worksheet.Cells[_startrow, startcol, _startrow == startrow ? startrow : startrow - 1, startcol + 14];
                    using ExcelRange r2 = worksheet2.Cells[_startrow2, startcol2, _startrow2 == startrow2 ? startrow2 : startrow2 - 1, startcol2 + 3];
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                    r2.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r2.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r2.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r2.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r2.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r2.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r2.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r2.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                    Bytes = excelPackage.GetAsByteArray();
                }
                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kitano_GiayToLamViec_v02.xlsx");
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
}