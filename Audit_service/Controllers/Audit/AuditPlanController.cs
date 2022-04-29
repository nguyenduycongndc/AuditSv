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
using Audit_service.Models.MigrationsModels.Audit;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using KitanoUserService.API.Models.MigrationsModels;
//using Syncfusion.DocIO.DLS;
using Spire.Doc;
using Spire.Pdf;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Formatting;
using System.Drawing;
using Document = Spire.Doc.Document;
using StackExchange.Redis;
using NPOI.XWPF.UserModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using BreakType = Spire.Doc.Documents.BreakType;
using Audit_service.Utils;
using Spire.Doc.Reporting;

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class AuditPlanController : BaseController
    {

        private const string REPORT_HEADER = "REPORT_HEADER";
        private const string COMPANY_NAME = "COMPANY_NAME";
        protected readonly IConfiguration _config;
        public AuditPlanController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditPlanSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = obj.Status;
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_AP" && (string.IsNullOrEmpty(status) || a.StatusCode == status)).ToArray();
                var list_appoval_id = approval_status.Select(a => a.item_id).ToList();
                Expression<Func<AuditPlan, bool>> filter = c => (string.IsNullOrEmpty(obj.Year) || Convert.ToString(c.Year).Contains(obj.Year))
                                               && (string.IsNullOrEmpty(status) || list_appoval_id.Contains(c.Id) || (status == "1.0" && !list_appoval_id.Contains(c.Id))) && c.IsDelete != true;
                var list_auditplan = _uow.Repository<AuditPlan>().Find(filter).OrderByDescending(a => a.Year);//.ThenByDescending(a => a.Status == 3).ThenByDescending(a => a.Status == 1).ThenByDescending(a => a.Status == 2).ThenByDescending(a => a.Status == 4);

                IEnumerable<AuditPlan> data = list_auditplan;
                var count = list_auditplan.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var result = data.Where(a => a.IsDelete != true).Select(a => new AuditPlanListModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Status = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.StatusCode ?? "1.0",
                    Year = a.Year,
                    Version = a.Version,
                    ApprovalUser = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.approver,
                    ApprovalUserLast = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.approver_last
                });

                //var lst = new List<AuditPlanModel>();
                var lst = result.GroupBy(a => a.Year).Select(g => g.OrderByDescending(x => x.Status == "3.1").FirstOrDefault()).ToList();
                lst.ForEach(o =>
                {
                    var child = result.Where(a => a.Year == o.Year && a.Id != o.Id).ToList();
                    o.SubActivities = child;
                });
                //var r
                return Ok(new { code = "1", msg = "success", data = lst, total = count });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] AuditPlanModel auditplaninfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _allAuditPlan = _uow.Repository<AuditPlan>().Find(a => a.IsDelete != true && a.Year == auditplaninfo.Year).ToArray();
                var check_isCopy = false;
                if (_allAuditPlan.Length > 0)
                {
                    //var checkStatus = _allAuditPlan.Where(a => a.Status == 1).ToArray();
                    //if (checkStatus.Length > 0)
                    //{
                    //    return Ok(new { code = "0", msg = "success" });
                    //}
                    //check_isCopy = true;
                    return Ok(new { code = "0", msg = "success" });
                }
                var AuditPlan = new AuditPlan
                {
                    Name = auditplaninfo.Name,
                    Year = auditplaninfo.Year,
                    Code = "KHKT-" + auditplaninfo.Year,
                    //AuditPlan.Version = 1;
                    Status = 1,
                    IsDelete = false,
                    Createdate = DateTime.Now
                };
                if (check_isCopy == true)
                {
                    AuditPlan.IsCopy = true;
                }
                _uow.Repository<AuditPlan>().Add(AuditPlan);
                var result = new AuditPlanListModel()
                {
                    Id = AuditPlan?.Id,
                    Version = AuditPlan?.Version,
                };
                return Ok(new { code = "1", data = result, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Edit([FromBody] AuditPlanModifyModel audiplaninfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var list_all = _uow.Repository<AuditPlan>().Find(a => a.IsDelete != true).ToArray();
                if (audiplaninfo.StatusCode == "1.0" || audiplaninfo.StatusCode == "1.1" || audiplaninfo.StatusCode == "2.1")
                {
                    var getallauditplan_year = list_all.Where(a => a.Year == audiplaninfo.Year && a.Id != audiplaninfo.Id).Select(a => a.Id).ToList();
                    var approval_status_list = _uow.Repository<ApprovalFunction>().Find(a => getallauditplan_year.Contains(a.item_id.Value) && a.function_code == "M_AP").ToArray();
                    var approval_status = approval_status_list.FirstOrDefault(a => (string.IsNullOrEmpty(a.StatusCode) || a.StatusCode == "1.0" || a.StatusCode == "1.1" || a.StatusCode == "2.1"));
                    if (approval_status != null)
                    {
                        return Ok(new { code = "0", msg = "success" });
                    }
                }

                var AuditPlan = list_all.FirstOrDefault(a => a.Id == audiplaninfo.Id);
                if (AuditPlan == null)
                {
                    return NotFound();
                }

                AuditPlan.Name = audiplaninfo.Name;
                AuditPlan.Target = audiplaninfo.Target;
                AuditPlan.Note = audiplaninfo.Note;
                AuditPlan.Year = audiplaninfo.Year ?? 0;
                AuditPlan.Status = audiplaninfo.Status;
                AuditPlan.Browsedate = audiplaninfo.Browsedate;
                AuditPlan.OtherInformation = audiplaninfo.OtherInfo;
                _uow.Repository<AuditPlan>().Update(AuditPlan);
                var result = new AuditPlanListModel()
                {
                    Id = AuditPlan?.Id,
                    Version = AuditPlan?.Version,
                };
                return Ok(new { code = "1", data = result, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("UpdateAuditPlan")]
        public IActionResult UpdateAuditPlan()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var audiplaninfo = new AuditPlanModifyModel();
                var data = Request.Form["data"];
                if (!string.IsNullOrEmpty(data))
                {
                    audiplaninfo = JsonSerializer.Deserialize<AuditPlanModifyModel>(data);
                }
                else
                {
                    return BadRequest();
                }
                var list_all = _uow.Repository<AuditPlan>().Find(a => a.IsDelete != true).ToArray();
                if (audiplaninfo.StatusCode == "1.0" || audiplaninfo.StatusCode == "1.1" || audiplaninfo.StatusCode == "2.1")
                {
                    var getallauditplan_year = list_all.Where(a => a.Year == audiplaninfo.Year && a.Id != audiplaninfo.Id).Select(a => a.Id).ToList();
                    var approval_status_list = _uow.Repository<ApprovalFunction>().Find(a => getallauditplan_year.Contains(a.item_id.Value) && a.function_code == "M_AP").ToArray();
                    var approval_status = approval_status_list.FirstOrDefault(a => (string.IsNullOrEmpty(a.StatusCode) || a.StatusCode == "1.0" || a.StatusCode == "1.1" || a.StatusCode == "2.1"));
                    if (approval_status != null)
                    {
                        return Ok(new { code = "0", msg = "success" });
                    }
                }

                var AuditPlan = list_all.FirstOrDefault(a => a.Id == audiplaninfo.Id);
                if (AuditPlan == null)
                {
                    return NotFound();
                }

                AuditPlan.Name = audiplaninfo.Name;
                AuditPlan.Target = audiplaninfo.Target;
                AuditPlan.Note = audiplaninfo.Note;
                AuditPlan.Year = audiplaninfo.Year ?? 0;
                AuditPlan.Status = audiplaninfo.Status;
                AuditPlan.Browsedate = audiplaninfo.Browsedate;
                AuditPlan.OtherInformation = audiplaninfo.OtherInfo;

                var file = Request.Form.Files;
                var _listFile = new List<ApprovalFunctionFile>();

                foreach (var item in file)
                {
                    var file_type = item.ContentType;
                    var folder = "M_AP" + "_" + "ApprvalOutside";
                    var pathSave = CreateUploadURLPlan(item, folder);
                    var function_file = new ApprovalFunctionFile()
                    {
                        item_id = AuditPlan.Id,
                        function_code = "M_AP",
                        function_name = "Kế hoạch kiểm toán năm",
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        FileType = file_type,
                        Path = pathSave,
                    };
                    _listFile.Add(function_file);
                }
                foreach (var item in _listFile)
                {
                    _uow.Repository<ApprovalFunctionFile>().AddWithoutSave(item);
                }
                _uow.Repository<AuditPlan>().UpdateWithoutSave(AuditPlan);
                _uow.SaveChanges();
                var result = new AuditPlanListModel()
                {
                    Id = AuditPlan?.Id,
                    Version = AuditPlan?.Version,
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
                var result = new AuditPlanListModel()
                {
                    Id = AuditPlan?.Id,
                    Version = AuditPlan?.Version,
                };
                return Ok(new { code = "1", data = result, msg = "success" });
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
                var AuditPlan = _uow.Repository<AuditPlan>().Include(a => a.Users).FirstOrDefault(a => a.Id == id);
                if (AuditPlan == null)
                {
                    return NotFound();
                }
                var approval_status = _uow.Repository<ApprovalFunction>().Include(a => a.Users, a => a.Users_Last).FirstOrDefault(a => a.item_id == id && a.function_code == "M_AP");
                var auditplan_file = _uow.Repository<ApprovalFunctionFile>().Find(a => a.item_id == id && a.function_code == "M_AP" && a.IsDeleted != true).ToArray();
                var auditwork = _uow.Repository<AuditWorkPlan>().Include(a => a.AuditWorkScopePlan, a => a.Users, a => a.AuditWorkScopePlanFacility).Where(a => a.auditplan_id == id && a.IsDeleted != true).ToArray();
                var _auditwork_id = auditwork.Select(a => a.Id).ToArray();
                //var _user = _uow.Repository<Users>().Find(a => a.UsersType == 1).ToArray();
                //var audit_work_scope = _uow.Repository<AuditWorkScopePlan>().Include(a => a.AuditWorkPlan).Where(a => a.auditprocess_id.HasValue && a.auditwork_id.HasValue && _auditwork_id.Contains(a.auditwork_id.Value) && a.IsDeleted != true).AsEnumerable().GroupBy(a => a.auditwork_id);
                List<AuditWorkListModel> _lstauditwork = new List<AuditWorkListModel>();
                _lstauditwork = auditwork.Select(a => new AuditWorkListModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    PersonInCharge = a.person_in_charge.HasValue ? a.Users?.FullName : "",
                    AuditFacility = string.Join(", ", a.AuditWorkScopePlanFacility.Where(x => x.IsDeleted != true).Select(y => y.auditfacilities_name).Distinct()),
                    AuditProcess = "",//string.Join(", ", a.Select(x => x.auditprocess_name)),
                    CreateAt = a.CreatedAt,
                }).ToList();

                //var _auditwork_remain = auditwork.Where(a => !a.AuditWorkScopePlan.Any() || a.AuditWorkScopePlan.All(x => x.IsDeleted == true)).ToArray();
                //var _lstauditworktemp = _auditwork_remain.Select(a => new AuditWorkListModel
                //{
                //    Id = a.Id,
                //    Name = a.Name,
                //    Code = a.Code,
                //    StartDate = a.StartDate,
                //    EndDate = a.EndDate,
                //    PersonInCharge = a.person_in_charge.HasValue ? a.Users.FullName : "",
                //    AuditFacility = "",
                //    AuditProcess = "",
                //    CreateAt = a.CreatedAt,
                //}).ToList();
                //_lstauditwork.AddRange(_lstauditworktemp);
                var deparmentinfo = new AuditPlanModel()
                {
                    Id = AuditPlan.Id,
                    Name = AuditPlan.Name,
                    Year = AuditPlan.Year,
                    Note = AuditPlan.Note,
                    Target = AuditPlan.Target,
                    Status = approval_status?.StatusCode ?? "1.0",
                    AuditWorkList = _lstauditwork.OrderByDescending(a => a.CreateAt).ToList(),
                    IsCopy = AuditPlan.IsCopy ?? false,
                    Version = AuditPlan.Version,
                    OtherInfo = AuditPlan.OtherInformation,
                    //Path = AuditPlan.Path,
                    ListFile = auditplan_file.Select(x => new AuditWorkPlanFileModel()
                    {
                        id = x.id,
                        Path = x.Path,
                        FileType = x.FileType,
                    }).ToList(),
                    approval_user = approval_status != null ? (approval_status.approver_last.HasValue && (approval_status.StatusCode == "3.1" || approval_status.StatusCode == "3.2" || approval_status.StatusCode == "0.0") ? approval_status.Users_Last.FullName : (approval_status.approver.HasValue ? approval_status.Users.FullName : "")) : "",
                    Browsedate = approval_status?.ApprovalDate,
                };
                return Ok(new { code = "1", msg = "success", data = deparmentinfo });

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("{id}")]
        public IActionResult Copy(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var list_all = _uow.Repository<AuditPlan>().Find(a => a.IsDelete != true).ToArray();
                var AuditPlan = list_all.FirstOrDefault(a => a.Id == id);
                if (AuditPlan == null)
                {
                    return NotFound();
                }

                var getallauditplan_year = list_all.Where(a => a.Year == AuditPlan.Year).Select(a => a.Id).ToList();
                var approval_status_list = _uow.Repository<ApprovalFunction>().Find(a => getallauditplan_year.Contains(a.item_id.Value) && a.function_code == "M_AP").ToArray();
                var check_status = list_all.FirstOrDefault(a => a.Year == AuditPlan.Year && a.Status == 1 && !approval_status_list.Any(x => x.item_id == a.Id));
                if (check_status != null)
                {
                    return Ok(new { code = "0", msg = "failed" });
                }
                var approval_status = approval_status_list.FirstOrDefault(a => (string.IsNullOrEmpty(a.StatusCode) || a.StatusCode == "1.0" || a.StatusCode == "1.1" || a.StatusCode == "2.1" || a.StatusCode == "2.2" || a.StatusCode == "3.2"));
                if (approval_status != null)
                {
                    return Ok(new { code = "0", msg = "failed" });
                }
                //var ver = list_all.Where(a => a.Year == AuditPlan.Year).Max(a => a.Version);
                var copyinfor = new AuditPlan
                {
                    Name = AuditPlan.Name,
                    Year = AuditPlan.Year,
                    Note = AuditPlan.Note,
                    Target = AuditPlan.Target,
                    Code = "KHKT-" + AuditPlan.Year + 1,
                    //Version = !ver.HasValue ? 1 : ver + 1,
                    Status = 1,
                    IsDelete = false,
                    Createdate = DateTime.Now,
                    UserId = userInfo.Id,
                    IsCopy = true,
                    OtherInformation = AuditPlan.OtherInformation,
                };

                _uow.Repository<AuditPlan>().Add(copyinfor);

                var auditwork_plan = _uow.Repository<AuditWorkPlan>().Include(a => a.AuditAssignmentPlan, a => a.AuditWorkScopePlan, a => a.AuditWorkScopePlanFacility, a => a.AuditWorkPlanFile).Where(a => a.IsDeleted != true && a.auditplan_id == id).ToArray();
                for (int i = 0; i < auditwork_plan.Length; i++)
                {
                    var auditwork_old = auditwork_plan[i];
                    var AuditWork = new AuditWorkPlan
                    {
                        Name = auditwork_old.Name,
                        Target = auditwork_old.Target,
                        ReqSkillForAudit = auditwork_old.ReqSkillForAudit,
                        ReqOutsourcing = auditwork_old.ReqOutsourcing,
                        ReqOther = auditwork_old.ReqOther,
                        IsActive = auditwork_old.IsActive,
                        IsDeleted = auditwork_old.IsDeleted,
                        CreatedAt = auditwork_old.CreatedAt,
                        CreatedBy = auditwork_old.CreatedBy,
                        Path = auditwork_old.Path,
                        Year = auditwork_old.Year,
                        NumOfAuditor = auditwork_old.NumOfAuditor,
                        NumOfWorkdays = auditwork_old.NumOfWorkdays,
                        person_in_charge = auditwork_old.person_in_charge,
                        ScaleOfAudit = auditwork_old.ScaleOfAudit,
                        auditplan_id = copyinfor.Id,
                        Code = auditwork_old.Code,
                        StartDate = auditwork_old.StartDate,
                        EndDate = auditwork_old.EndDate,
                        AuditScope = auditwork_old.AuditScope,
                    };
                    var _listassign = new List<AuditAssignmentPlan>();
                    if (auditwork_old.AuditAssignmentPlan.Count > 0)
                    {
                        var lst_AuditAssignmentPlan = auditwork_old.AuditAssignmentPlan.Where(x => x.IsDeleted != true).ToList();
                        foreach (var item in lst_AuditAssignmentPlan)
                        {
                            var _auditAssignment = new AuditAssignmentPlan
                            {
                                AuditWorkPlan = AuditWork,
                                user_id = item.user_id,
                                fullName = item.fullName,
                                IsActive = item.IsActive,
                                IsDeleted = item.IsDeleted,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                            };
                            _listassign.Add(_auditAssignment);
                        }
                    }

                    var _listscope = new List<AuditWorkScopePlan>();
                    if (auditwork_old.AuditWorkScopePlan.Count > 0)
                    {
                        var lst_AuditWorkScopePlan = auditwork_old.AuditWorkScopePlan.Where(x => x.IsDeleted != true).ToList();
                        foreach (var item in lst_AuditWorkScopePlan)
                        {
                            var _auditWorkScope = new AuditWorkScopePlan
                            {
                                AuditWorkPlan = AuditWork,
                                auditprocess_name = item.auditprocess_name,
                                auditfacilities_name = item.auditfacilities_name,
                                bussinessactivities_name = item.bussinessactivities_name,
                                ReasonNote = item.ReasonNote,
                                RiskRatingName = item.RiskRatingName,
                                IsDeleted = item.IsDeleted,
                                BoardId = item.BoardId,
                                auditprocess_id = item.auditprocess_id,
                                auditfacilities_id = item.auditfacilities_id,
                                bussinessactivities_id = item.bussinessactivities_id,
                                RiskRating = item.RiskRating,
                                Year = item.Year,
                                AuditingTimeNearest = item.AuditingTimeNearest,
                            };
                            _listscope.Add(_auditWorkScope);
                        }
                    }
                    var _listscopeFacility = new List<AuditWorkScopePlanFacility>();
                    if (auditwork_old.AuditWorkScopePlanFacility.Count > 0)
                    {
                        var lst_AuditWorkScopePlan = auditwork_old.AuditWorkScopePlanFacility.Where(x => x.IsDeleted != true).ToList();
                        foreach (var item in lst_AuditWorkScopePlan)
                        {
                            var _auditWorkScopeFacility = new AuditWorkScopePlanFacility
                            {
                                AuditWorkPlan = AuditWork,
                                auditfacilities_name = item.auditfacilities_name,
                                ReasonNote = item.ReasonNote,
                                RiskRatingName = item.RiskRatingName,
                                IsDeleted = false,
                                BoardId = item.BoardId,
                                auditfacilities_id = item.auditfacilities_id,
                                RiskRating = item.RiskRating,
                                Year = item.Year,
                                AuditingTimeNearest = item.AuditingTimeNearest
                            };
                            _listscopeFacility.Add(_auditWorkScopeFacility);
                        }
                    }
                    var _listFile = new List<AuditWorkPlanFile>();
                    if (auditwork_old.AuditWorkPlanFile.Count > 0)
                    {
                        var file = auditwork_old.AuditWorkPlanFile.Where(x => x.IsDelete != true).ToList();
                        foreach (var item in file)
                        {
                            var audit_work_file = new AuditWorkPlanFile()
                            {
                                AuditWorkPlan = AuditWork,
                                IsDelete = false,
                                FileType = item.FileType,
                                Path = item.Path,
                            };
                            _listFile.Add(audit_work_file);
                        }
                    }
                    _uow.Repository<AuditWorkPlan>().AddWithoutSave(AuditWork);
                    foreach (var item in _listscope)
                    {
                        _uow.Repository<AuditWorkScopePlan>().AddWithoutSave(item);
                    }
                    foreach (var item in _listassign)
                    {
                        _uow.Repository<AuditAssignmentPlan>().AddWithoutSave(item);
                    }
                    foreach (var item in _listscopeFacility)
                    {
                        _uow.Repository<AuditWorkScopePlanFacility>().AddWithoutSave(item);
                    }
                    foreach (var item in _listFile)
                    {
                        _uow.Repository<AuditWorkPlanFile>().AddWithoutSave(item);
                    }
                    _uow.SaveChanges();
                }
                var result = new AuditPlanListModel()
                {
                    Id = AuditPlan?.Id,
                    Version = AuditPlan?.Version,
                };
                return Ok(new { code = "1", data = result, msg = "success" });

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        //chi tiết kế hoạch cuộc kiểm toán
        [HttpGet("AuditWorkDetail/{id}")]
        public IActionResult AuditWorkDetail(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                //checkstatus
                var status_wait_name = "";
                var status_browser_name = "";
                var status_refuse_name = "";
                var status_wait = "";
                var status_browser = "";
                var status_refuse = "";
                var approval_status1 = _uow.Repository<ApprovalFunction>().Include(a => a.Users, a => a.Users_Last).FirstOrDefault(a => a.item_id == id && a.function_code == "M_PAP");
                var approval_status = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == "M_PAP" && a.StatusCode == "1.0");
                //var approval_status_c2 = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == "M_PAP"
                //&& a.StatusCode == "2.1" && a.IsShow == true);
                var approval_status_wait = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.function_code == "M_PAP"
                && (a.StatusCode == "1.1" || a.StatusCode == "2.1")
                && a.item_id == id);
                if (approval_status_wait != null)
                {
                    var approval_status_wait_name = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == "M_PAP"
                    && a.StatusCode == approval_status_wait.StatusCode);
                    status_wait_name = approval_status_wait_name.StatusName;
                    status_wait = approval_status_wait.StatusCode;
                }
                var approval_status_browser = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.function_code == "M_PAP"
                && a.StatusCode == "3.1" && a.item_id == id);
                if (approval_status_browser != null)
                {
                    var approval_status_browser_name = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == "M_PAP"
                    && a.StatusCode == approval_status_browser.StatusCode);
                    status_browser_name = approval_status_browser_name.StatusName;
                    status_browser = approval_status_browser.StatusCode;
                }
                var approval_status_refuse = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.function_code == "M_PAP"
                && (a.StatusCode == "3.2" || a.StatusCode == "2.2")
                && a.item_id == id);
                if (approval_status_refuse != null)
                {
                    var approval_status_refuse_name = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == "M_PAP"
                    && a.StatusCode == approval_status_refuse.StatusCode);
                    status_refuse_name = approval_status_refuse_name.StatusName;
                    status_refuse = approval_status_refuse.StatusCode;
                }

                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == id);

                var checkApprovalFunctionFile = _uow.Repository<ApprovalFunctionFile>().Find(a => a.item_id == id).ToArray();

                //check tab 2
                var checkAuditAssignment = _uow.Repository<AuditAssignment>().Include(x => x.AuditWork).FirstOrDefault(x => x.auditwork_id == id);
                var auditAssignment = _uow.Repository<AuditAssignment>().Include(a => a.Users).Where(a => a.auditwork_id == id && a.IsDeleted != true).ToArray();

                //check tab 3
                var checkAuditWorkScope = _uow.Repository<AuditWorkScope>().Include(x => x.AuditWork).FirstOrDefault(x => x.auditwork_id == id);
                var auditWorkScope = _uow.Repository<AuditWorkScope>().Include(a => a.AuditWorkScopeUserMapping).Where(a => a.auditwork_id == id && a.IsDeleted != true).ToArray();
                for (int i = 0; i < auditWorkScope.Length; i++)
                {
                    var checkAuditWorkScopeUserMapping = _uow.Repository<AuditWorkScopeUserMapping>().Include(a => a.Users).Where(a => a.auditwork_scope_id == auditWorkScope[i].Id).ToArray();
                }
                //check AuditWorkScopePlanFacility
                var checkAuditWorkScopeFacility = _uow.Repository<AuditWorkScopeFacility>().Include(x => x.AuditWorkScopeFacilityFile).Where(a => a.IsDeleted != true
                && a.auditwork_id == id).ToArray();

                //check tab 4
                var checkSchedule = _uow.Repository<Schedule>().Include(x => x.AuditWork).FirstOrDefault(x => x.auditwork_id == id);
                var auditSchedule = _uow.Repository<Schedule>().Include(a => a.Users).Where(a => a.auditwork_id == id && a.is_deleted != true).ToArray();

                if (checkAuditAssignment != null && checkAuditWorkScope == null && checkSchedule == null)
                {
                    var AuditWork = new AuditWorkDetailModel()
                    {
                        Id = checkAuditAssignment.AuditWork.Id,
                        Code = checkAuditAssignment.AuditWork.Code,
                        Name = checkAuditAssignment.AuditWork.Name,
                        Target = checkAuditAssignment.AuditWork.Target,
                        StartDate = checkAuditAssignment.AuditWork.StartDate.HasValue ? checkAuditAssignment.AuditWork.StartDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDate = checkAuditAssignment.AuditWork.EndDate.HasValue ? checkAuditAssignment.AuditWork.EndDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDatePlanning = checkAuditAssignment.AuditWork.EndDatePlanning.HasValue ? checkAuditAssignment.AuditWork.EndDatePlanning.Value.ToString("yyyy-MM-dd") : null,
                        StartDateReal = checkAuditAssignment.AuditWork.StartDateReal.HasValue ? checkAuditAssignment.AuditWork.StartDateReal.Value.ToString("yyyy-MM-dd") : null,
                        ReleaseDate = checkAuditAssignment.AuditWork.ReleaseDate.HasValue ? checkAuditAssignment.AuditWork.ReleaseDate.Value.ToString("yyyy-MM-dd") : null,
                        from_date = checkAuditAssignment.AuditWork.from_date.HasValue ? checkAuditAssignment.AuditWork.from_date.Value.ToString("yyyy-MM-dd") : null,
                        to_date = checkAuditAssignment.AuditWork.to_date.HasValue ? checkAuditAssignment.AuditWork.to_date.Value.ToString("yyyy-MM-dd") : null,
                        NumOfWorkdays = checkAuditAssignment.AuditWork.NumOfWorkdays,
                        person_in_charge = checkAuditAssignment.AuditWork.person_in_charge,
                        str_person_in_charge = checkAuditAssignment.AuditWork.person_in_charge.HasValue ? checkAuditAssignment.AuditWork.Users.Id + ":" + checkAuditAssignment.AuditWork.Users.FullName : "",
                        NumOfAuditor = /*checkAuditAssignment.AuditWork.NumOfAuditor*/auditAssignment.Length,
                        ReqSkillForAudit = checkAuditAssignment.AuditWork.ReqSkillForAudit,
                        ReqOutsourcing = checkAuditAssignment.AuditWork.ReqOutsourcing,
                        ReqOther = checkAuditAssignment.AuditWork.ReqOther,
                        ScaleOfAudit = checkAuditAssignment.AuditWork.ScaleOfAudit,
                        IsActive = checkAuditAssignment.AuditWork.IsActive,
                        IsDeleted = checkAuditAssignment.AuditWork.IsDeleted,
                        CreatedAt = checkAuditAssignment.AuditWork.CreatedAt.HasValue ? checkAuditAssignment.AuditWork.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                        CreatedBy = checkAuditAssignment.AuditWork.CreatedBy,
                        ModifiedAt = checkAuditAssignment.AuditWork.ModifiedAt.HasValue ? checkAuditAssignment.AuditWork.ModifiedAt.Value.ToString("yyyy-MM-dd") : null,
                        ModifiedBy = checkAuditAssignment.AuditWork.ModifiedBy,
                        DeletedAt = checkAuditAssignment.AuditWork.DeletedAt.HasValue ? checkAuditAssignment.AuditWork.DeletedAt.Value.ToString("yyyy-MM-dd") : null,
                        DeletedBy = checkAuditAssignment.AuditWork.DeletedBy,
                        Classify = checkAuditAssignment.AuditWork.Classify,
                        Year = checkAuditAssignment.AuditWork.Year,
                        Status = approval_status1?.StatusCode ?? "1.0",
                        //Status = approval_status_c2 != null ? 1 : 0,
                        statusName = approval_status_wait != null ? status_wait_name
                        : approval_status_browser != null ? status_browser_name
                        : approval_status_refuse != null ? status_refuse_name
                        : approval_status.StatusName,
                        AuditScopeOutside = checkAuditAssignment.AuditWork.AuditScopeOutside,
                        ExecutionStatusStr = checkAuditAssignment.AuditWork.ExecutionStatusStr,
                        AuditplanId = checkAuditAssignment.AuditWork.auditplan_id,
                        AuditScopeNew = checkAuditAssignment.AuditWork.AuditScope,
                        ListAuditPersonnel = auditAssignment.Select(a => new ListAuditPersonnelModel
                        {
                            Id = a.Id,
                            User_id = a.user_id,
                            FullName = a.user_id.HasValue ? a.Users.FullName : "",
                            Email = a.Users.Email,
                            str_fullName = a.user_id.HasValue ? a.user_id + ":" + a.Users.FullName : "",
                        }).ToList(),
                        ListAuditWorkScope = null,
                        ListSchedule = null,
                        ListAuditFacility = checkAuditWorkScopeFacility.Length == 0 ? null : checkAuditWorkScopeFacility.Select(a => new ListAuditFacility
                        {
                            id = a.Id,
                            auditfacilities_id = a.auditfacilities_id,
                            auditfacilities_name = a.auditfacilities_name == null ? "" : a.auditfacilities_name,
                            reason = a.ReasonNote == null ? "" : a.ReasonNote,
                            risk_rating_id = a.RiskRating,
                            risk_rating = a.RiskRatingName == null ? "" : a.RiskRatingName,
                            AuditingTimeNearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd/MM/yyyy") : "",
                            brief_review = a.brief_review == null ? "" : a.brief_review,
                            //path = a.path == null ? "" : a.path,
                            //filetype = a.FileType == null ? "" : a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                            ListFiles = a.AuditWorkScopeFacilityFile.Where(x => x.IsDelete != true).Select(x => new AuditWorkScopeFacilityFilesModel()
                            {
                                id = x.id,
                                Path = x.Path,
                                FileType = x.FileType,
                            }).ToList(),
                        }).ToList(),
                        ListApprovalFunctionFile = checkApprovalFunctionFile.Where(a => a.IsDeleted != true).Select(x => new ListApprovalFunctionFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = AuditWork });
                }
                else if (checkAuditAssignment != null && checkAuditWorkScope == null && checkSchedule != null)
                {
                    var AuditWork = new AuditWorkDetailModel()
                    {
                        Id = checkAuditAssignment.AuditWork.Id,
                        Code = checkAuditAssignment.AuditWork.Code,
                        Name = checkAuditAssignment.AuditWork.Name,
                        Target = checkAuditAssignment.AuditWork.Target,
                        StartDate = checkAuditAssignment.AuditWork.StartDate.HasValue ? checkAuditAssignment.AuditWork.StartDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDate = checkAuditAssignment.AuditWork.EndDate.HasValue ? checkAuditAssignment.AuditWork.EndDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDatePlanning = checkAuditAssignment.AuditWork.EndDatePlanning.HasValue ? checkAuditAssignment.AuditWork.EndDatePlanning.Value.ToString("yyyy-MM-dd") : null,
                        StartDateReal = checkAuditAssignment.AuditWork.StartDateReal.HasValue ? checkAuditAssignment.AuditWork.StartDateReal.Value.ToString("yyyy-MM-dd") : null,
                        ReleaseDate = checkAuditAssignment.AuditWork.ReleaseDate.HasValue ? checkAuditAssignment.AuditWork.ReleaseDate.Value.ToString("yyyy-MM-dd") : null,
                        from_date = checkAuditAssignment.AuditWork.from_date.HasValue ? checkAuditAssignment.AuditWork.from_date.Value.ToString("yyyy-MM-dd") : null,
                        to_date = checkAuditAssignment.AuditWork.to_date.HasValue ? checkAuditAssignment.AuditWork.to_date.Value.ToString("yyyy-MM-dd") : null,
                        NumOfWorkdays = checkAuditAssignment.AuditWork.NumOfWorkdays,
                        person_in_charge = checkAuditAssignment.AuditWork.person_in_charge,
                        str_person_in_charge = checkAuditAssignment.AuditWork.person_in_charge.HasValue ? checkAuditAssignment.AuditWork.Users.Id + ":" + checkAuditAssignment.AuditWork.Users.FullName : "",
                        NumOfAuditor = /*checkAuditAssignment.AuditWork.NumOfAuditor*/auditAssignment.Length,
                        ReqSkillForAudit = checkAuditAssignment.AuditWork.ReqSkillForAudit,
                        ReqOutsourcing = checkAuditAssignment.AuditWork.ReqOutsourcing,
                        ReqOther = checkAuditAssignment.AuditWork.ReqOther,
                        ScaleOfAudit = checkAuditAssignment.AuditWork.ScaleOfAudit,
                        IsActive = checkAuditAssignment.AuditWork.IsActive,
                        IsDeleted = checkAuditAssignment.AuditWork.IsDeleted,
                        CreatedAt = checkAuditAssignment.AuditWork.CreatedAt.HasValue ? checkAuditAssignment.AuditWork.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                        CreatedBy = checkAuditAssignment.AuditWork.CreatedBy,
                        ModifiedAt = checkAuditAssignment.AuditWork.ModifiedAt.HasValue ? checkAuditAssignment.AuditWork.ModifiedAt.Value.ToString("yyyy-MM-dd") : null,
                        ModifiedBy = checkAuditAssignment.AuditWork.ModifiedBy,
                        DeletedAt = checkAuditAssignment.AuditWork.DeletedAt.HasValue ? checkAuditAssignment.AuditWork.DeletedAt.Value.ToString("yyyy-MM-dd") : null,
                        DeletedBy = checkAuditAssignment.AuditWork.DeletedBy,
                        Classify = checkAuditAssignment.AuditWork.Classify,
                        Year = checkAuditAssignment.AuditWork.Year,
                        //Status = checkAuditAssignment.AuditWork.Status,
                        Status = approval_status1?.StatusCode ?? "1.0",
                        statusName = approval_status_wait != null ? status_wait_name
                        : approval_status_browser != null ? status_browser_name
                        : approval_status_refuse != null ? status_refuse_name
                        : approval_status.StatusName,
                        AuditScopeOutside = checkAuditAssignment.AuditWork.AuditScopeOutside,
                        ExecutionStatusStr = checkAuditAssignment.AuditWork.ExecutionStatusStr,
                        AuditplanId = checkAuditAssignment.AuditWork.auditplan_id,
                        AuditScopeNew = checkAuditAssignment.AuditWork.AuditScope,
                        ListAuditPersonnel = auditAssignment.Select(a => new ListAuditPersonnelModel
                        {
                            Id = a.Id,
                            User_id = a.user_id,
                            FullName = a.user_id.HasValue ? a.Users.FullName : "",
                            Email = a.Users.Email,
                            str_fullName = a.user_id.HasValue ? a.user_id + ":" + a.Users.FullName : "",
                        }).ToList(),
                        ListAuditWorkScope = null,
                        ListSchedule = auditSchedule.Select(a => new ListSchedule
                        {
                            id = a.id,
                            user_id = a.user_id,
                            user_name = a.user_id.HasValue ? a.Users.FullName : "",
                            auditwork_id = a.auditwork_id,
                            work = a.work,
                            expected_date = a.expected_date.HasValue ? a.expected_date.Value.ToString("yyyy-MM-dd") : null,
                            actual_date = a.actual_date.HasValue ? a.actual_date.Value.ToString("yyyy-MM-dd") : null,
                            str_person_in_charge = a.user_id.HasValue ? a.user_id + ":" + a.user_name : "",
                        }).ToList(),
                        ListAuditFacility = checkAuditWorkScopeFacility.Length == 0 ? null : checkAuditWorkScopeFacility.Select(a => new ListAuditFacility
                        {
                            id = a.Id,
                            auditfacilities_id = a.auditfacilities_id,
                            auditfacilities_name = a.auditfacilities_name == null ? "" : a.auditfacilities_name,
                            reason = a.ReasonNote == null ? "" : a.ReasonNote,
                            risk_rating_id = a.RiskRating,
                            risk_rating = a.RiskRatingName == null ? "" : a.RiskRatingName,
                            AuditingTimeNearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd/MM/yyyy") : "",
                            brief_review = a.brief_review == null ? "" : a.brief_review,
                            //path = a.path == null ? "" : a.path,
                            //filetype = a.FileType == null ? "" : a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                            ListFiles = a.AuditWorkScopeFacilityFile.Where(x => x.IsDelete != true).Select(x => new AuditWorkScopeFacilityFilesModel()
                            {
                                id = x.id,
                                Path = x.Path,
                                FileType = x.FileType,
                            }).ToList(),
                        }).ToList(),
                        ListApprovalFunctionFile = checkApprovalFunctionFile.Where(a => a.IsDeleted != true).Select(x => new ListApprovalFunctionFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = AuditWork });
                }
                else if (checkAuditAssignment != null && checkAuditWorkScope != null && checkSchedule != null)
                {
                    var AuditWork = new AuditWorkDetailModel()
                    {
                        Id = checkAuditWorkDetail.Id,
                        Code = checkAuditWorkDetail.Code,
                        Name = checkAuditWorkDetail.Name,
                        Target = checkAuditWorkDetail.Target,
                        StartDate = checkAuditWorkDetail.StartDate.HasValue ? checkAuditWorkDetail.StartDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDate = checkAuditWorkDetail.EndDate.HasValue ? checkAuditWorkDetail.EndDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDatePlanning = checkAuditWorkDetail.EndDatePlanning.HasValue ? checkAuditWorkDetail.EndDatePlanning.Value.ToString("yyyy-MM-dd") : null,
                        StartDateReal = checkAuditWorkDetail.StartDateReal.HasValue ? checkAuditWorkDetail.StartDateReal.Value.ToString("yyyy-MM-dd") : null,
                        ReleaseDate = checkAuditWorkDetail.ReleaseDate.HasValue ? checkAuditWorkDetail.ReleaseDate.Value.ToString("yyyy-MM-dd") : null,
                        from_date = checkAuditWorkDetail.from_date.HasValue ? checkAuditWorkDetail.from_date.Value.ToString("yyyy-MM-dd") : null,
                        to_date = checkAuditWorkDetail.to_date.HasValue ? checkAuditWorkDetail.to_date.Value.ToString("yyyy-MM-dd") : null,
                        NumOfWorkdays = checkAuditWorkDetail.NumOfWorkdays,
                        person_in_charge = checkAuditWorkDetail.person_in_charge,
                        str_person_in_charge = checkAuditWorkDetail.person_in_charge.HasValue ? checkAuditWorkDetail.Users.Id + ":" + checkAuditWorkDetail.Users.FullName : "",
                        NumOfAuditor = /*checkAuditWorkDetail.NumOfAuditor*/auditAssignment.Length,
                        ReqSkillForAudit = checkAuditWorkDetail.ReqSkillForAudit,
                        ReqOutsourcing = checkAuditWorkDetail.ReqOutsourcing,
                        ReqOther = checkAuditWorkDetail.ReqOther,
                        ScaleOfAudit = checkAuditWorkDetail.ScaleOfAudit,
                        IsActive = checkAuditWorkDetail.IsActive,
                        IsDeleted = checkAuditWorkDetail.IsDeleted,
                        CreatedAt = checkAuditWorkDetail.CreatedAt.HasValue ? checkAuditWorkDetail.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                        CreatedBy = checkAuditWorkDetail.CreatedBy,
                        ModifiedAt = checkAuditWorkDetail.ModifiedAt.HasValue ? checkAuditWorkDetail.ModifiedAt.Value.ToString("yyyy-MM-dd") : null,
                        ModifiedBy = checkAuditWorkDetail.ModifiedBy,
                        DeletedAt = checkAuditWorkDetail.DeletedAt.HasValue ? checkAuditWorkDetail.DeletedAt.Value.ToString("yyyy-MM-dd") : null,
                        DeletedBy = checkAuditWorkDetail.DeletedBy,
                        Classify = checkAuditWorkDetail.Classify,
                        Year = checkAuditWorkDetail.Year,
                        //Status = checkAuditWorkDetail.Status,
                        Status = approval_status1?.StatusCode ?? "1.0",
                        statusName = approval_status_wait != null ? status_wait_name
                        : approval_status_browser != null ? status_browser_name
                        : approval_status_refuse != null ? status_refuse_name
                        : approval_status.StatusName,
                        AuditScopeOutside = checkAuditWorkDetail.AuditScopeOutside,
                        ExecutionStatusStr = checkAuditWorkDetail.ExecutionStatusStr,
                        AuditplanId = checkAuditWorkDetail.auditplan_id,
                        AuditScopeNew = checkAuditWorkDetail.AuditScope,
                        ListAuditPersonnel = auditAssignment.Select(a => new ListAuditPersonnelModel
                        {
                            Id = a.Id,
                            User_id = a.user_id,
                            FullName = a.user_id.HasValue ? a.Users.FullName : "",
                            Email = a.Users.Email,
                            str_fullName = a.user_id.HasValue ? a.user_id + ":" + a.Users.FullName : "",
                        }).ToList(),
                        ListAuditWorkScope = auditWorkScope.Select(a => new ListAuditWorkScope
                        {
                            id = a.Id,
                            auditwork_id = a.auditwork_id,
                            auditprocess_id = a.auditprocess_id,
                            bussinessactivities_id = a.bussinessactivities_id,
                            auditfacilities_id = a.auditfacilities_id,
                            reason = a.ReasonNote,//lý do kiểm toán
                            //risk_rating = a.RiskRating,//xếp hạng rủi ro
                            risk_rating = a.RiskRatingName,//xếp hạng rủi ro
                            auditing_time_nearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd-MM-yyyy") : null,//Thời gian kiểm toán gần nhất
                            auditfacilities_name = a.auditfacilities_name,//đơn vị kiểm toán
                            auditprocess_name = a.auditprocess_name,//Quy trình kiểm toán
                            bussinessactivities_name = a.bussinessactivities_name,//Hoạt động được kiểm toán
                            year = a.Year,
                            audit_team_leader = (auditWorkScope != null ? string.Join(", ", a.AuditWorkScopeUserMapping.ToList().Where(c => c.type == 1 && c.auditwork_scope_id == a.Id).Select(x => x.user_id + ":" + x.full_name).Distinct()) : ""),
                            auditor = (auditWorkScope != null ? string.Join(", ", a.AuditWorkScopeUserMapping.ToList().Where(c => c.type == 2 && c.auditwork_scope_id == a.Id).Select(x => x.user_id + ":" + x.full_name).Distinct()) : ""),
                            //brief_review = a.brief_review,
                            //path = a.path == null ? "" : a.path,
                            //filetype = a.FileType == null ? "" : a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                        }).ToList(),
                        ListSchedule = auditSchedule.Select(a => new ListSchedule
                        {
                            id = a.id,
                            user_id = a.user_id,
                            user_name = a.user_id.HasValue ? a.Users.FullName : "",
                            auditwork_id = a.auditwork_id,
                            work = a.work,
                            expected_date = a.expected_date.HasValue ? a.expected_date.Value.ToString("yyyy-MM-dd") : null,
                            actual_date = a.actual_date.HasValue ? a.actual_date.Value.ToString("yyyy-MM-dd") : null,
                            str_person_in_charge = a.user_id.HasValue ? a.user_id + ":" + a.user_name : "",
                        }).ToList(),
                        ListAuditFacility = checkAuditWorkScopeFacility.Length == 0 ? null : checkAuditWorkScopeFacility.Select(a => new ListAuditFacility
                        {
                            id = a.Id,
                            auditfacilities_id = a.auditfacilities_id,
                            auditfacilities_name = a.auditfacilities_name == null ? "" : a.auditfacilities_name,
                            reason = a.ReasonNote == null ? "" : a.ReasonNote,
                            risk_rating_id = a.RiskRating,
                            risk_rating = a.RiskRatingName == null ? "" : a.RiskRatingName,
                            AuditingTimeNearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd/MM/yyyy") : "",
                            brief_review = a.brief_review == null ? "" : a.brief_review,
                            //path = a.path == null ? "" : a.path,
                            //filetype = a.FileType == null ? "" : a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                            ListFiles = a.AuditWorkScopeFacilityFile.Where(x => x.IsDelete != true).Select(x => new AuditWorkScopeFacilityFilesModel()
                            {
                                id = x.id,
                                Path = x.Path,
                                FileType = x.FileType,
                            }).ToList(),
                        }).ToList(),
                        ListApprovalFunctionFile = checkApprovalFunctionFile.Where(a => a.IsDeleted != true).Select(x => new ListApprovalFunctionFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = AuditWork });
                }
                else if (checkAuditAssignment != null && checkAuditWorkScope != null && checkSchedule == null)
                {
                    var AuditWork = new AuditWorkDetailModel()
                    {
                        Id = checkAuditWorkDetail.Id,
                        Code = checkAuditWorkDetail.Code,
                        Name = checkAuditWorkDetail.Name,
                        Target = checkAuditWorkDetail.Target,
                        StartDate = checkAuditWorkDetail.StartDate.HasValue ? checkAuditWorkDetail.StartDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDate = checkAuditWorkDetail.EndDate.HasValue ? checkAuditWorkDetail.EndDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDatePlanning = checkAuditWorkDetail.EndDatePlanning.HasValue ? checkAuditWorkDetail.EndDatePlanning.Value.ToString("yyyy-MM-dd") : null,
                        StartDateReal = checkAuditWorkDetail.StartDateReal.HasValue ? checkAuditWorkDetail.StartDateReal.Value.ToString("yyyy-MM-dd") : null,
                        ReleaseDate = checkAuditWorkDetail.ReleaseDate.HasValue ? checkAuditWorkDetail.ReleaseDate.Value.ToString("yyyy-MM-dd") : null,
                        from_date = checkAuditWorkDetail.from_date.HasValue ? checkAuditWorkDetail.from_date.Value.ToString("yyyy-MM-dd") : null,
                        to_date = checkAuditWorkDetail.to_date.HasValue ? checkAuditWorkDetail.to_date.Value.ToString("yyyy-MM-dd") : null,
                        NumOfWorkdays = checkAuditWorkDetail.NumOfWorkdays,
                        person_in_charge = checkAuditWorkDetail.person_in_charge,
                        str_person_in_charge = checkAuditWorkDetail.person_in_charge.HasValue ? checkAuditWorkDetail.Users.Id + ":" + checkAuditWorkDetail.Users.FullName : "",
                        NumOfAuditor = /*checkAuditWorkDetail.NumOfAuditor*/auditAssignment.Length,
                        ReqSkillForAudit = checkAuditWorkDetail.ReqSkillForAudit,
                        ReqOutsourcing = checkAuditWorkDetail.ReqOutsourcing,
                        ReqOther = checkAuditWorkDetail.ReqOther,
                        ScaleOfAudit = checkAuditWorkDetail.ScaleOfAudit,
                        IsActive = checkAuditWorkDetail.IsActive,
                        IsDeleted = checkAuditWorkDetail.IsDeleted,
                        CreatedAt = checkAuditWorkDetail.CreatedAt.HasValue ? checkAuditWorkDetail.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                        CreatedBy = checkAuditWorkDetail.CreatedBy,
                        ModifiedAt = checkAuditWorkDetail.ModifiedAt.HasValue ? checkAuditWorkDetail.ModifiedAt.Value.ToString("yyyy-MM-dd") : null,
                        ModifiedBy = checkAuditWorkDetail.ModifiedBy,
                        DeletedAt = checkAuditWorkDetail.DeletedAt.HasValue ? checkAuditWorkDetail.DeletedAt.Value.ToString("yyyy-MM-dd") : null,
                        DeletedBy = checkAuditWorkDetail.DeletedBy,
                        Classify = checkAuditWorkDetail.Classify,
                        Year = checkAuditWorkDetail.Year,
                        //Status = checkAuditWorkDetail.Status,
                        Status = approval_status1?.StatusCode ?? "1.0",
                        statusName = approval_status_wait != null ? status_wait_name
                        : approval_status_browser != null ? status_browser_name
                        : approval_status_refuse != null ? status_refuse_name
                        : approval_status.StatusName,
                        AuditScopeOutside = checkAuditWorkDetail.AuditScopeOutside,
                        ExecutionStatusStr = checkAuditWorkDetail.ExecutionStatusStr,
                        AuditplanId = checkAuditWorkDetail.auditplan_id,
                        AuditScopeNew = checkAuditWorkDetail.AuditScope,
                        ListAuditPersonnel = auditAssignment.Select(a => new ListAuditPersonnelModel
                        {
                            Id = a.Id,
                            User_id = a.user_id,
                            FullName = a.user_id.HasValue ? a.Users.FullName : "",
                            Email = a.Users.Email,
                            str_fullName = a.user_id.HasValue ? a.user_id + ":" + a.Users.FullName : "",
                        }).ToList(),
                        ListAuditWorkScope = auditWorkScope.Select(a => new ListAuditWorkScope
                        {
                            id = a.Id,
                            auditwork_id = a.auditwork_id,
                            auditprocess_id = a.auditprocess_id,
                            bussinessactivities_id = a.bussinessactivities_id,
                            auditfacilities_id = a.auditfacilities_id,
                            reason = a.ReasonNote,
                            risk_rating = a.RiskRatingName,//xếp hạng rủi ro
                            //risk_rating = a.RiskRating,
                            auditing_time_nearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd-MM-yyyy") : null,
                            auditfacilities_name = a.auditfacilities_name,
                            auditprocess_name = a.auditprocess_name,
                            bussinessactivities_name = a.bussinessactivities_name,
                            year = a.Year,
                            audit_team_leader = (auditWorkScope != null ? string.Join(", ", a.AuditWorkScopeUserMapping.ToList().Where(c => c.type == 1 && c.auditwork_scope_id == a.Id).Select(x => x.user_id + ":" + x.full_name).Distinct()) : ""),
                            auditor = (auditWorkScope != null ? string.Join(", ", a.AuditWorkScopeUserMapping.ToList().Where(c => c.type == 2 && c.auditwork_scope_id == a.Id).Select(x => x.user_id + ":" + x.full_name).Distinct()) : ""),
                            //brief_review = a.brief_review,
                            //path = a.path,
                            //filetype = a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                        }).ToList(),
                        ListSchedule = null,
                        ListAuditFacility = checkAuditWorkScopeFacility.Length == 0 ? null : checkAuditWorkScopeFacility.Select(a => new ListAuditFacility
                        {
                            id = a.Id,
                            auditfacilities_id = a.auditfacilities_id,
                            auditfacilities_name = a.auditfacilities_name == null ? "" : a.auditfacilities_name,
                            reason = a.ReasonNote == null ? "" : a.ReasonNote,
                            risk_rating_id = a.RiskRating,
                            risk_rating = a.RiskRatingName == null ? "" : a.RiskRatingName,
                            AuditingTimeNearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd/MM/yyyy") : "",
                            brief_review = a.brief_review == null ? "" : a.brief_review,
                            //path = a.path == null ? "" : a.path,
                            //filetype = a.FileType == null ? "" : a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                            ListFiles = a.AuditWorkScopeFacilityFile.Where(x => x.IsDelete != true).Select(x => new AuditWorkScopeFacilityFilesModel()
                            {
                                id = x.id,
                                Path = x.Path,
                                FileType = x.FileType,
                            }).ToList(),
                        }).ToList(),
                        ListApprovalFunctionFile = checkApprovalFunctionFile.Where(a => a.IsDeleted != true).Select(x => new ListApprovalFunctionFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = AuditWork });
                }
                else if (checkAuditAssignment == null && checkAuditWorkScope != null && checkSchedule != null)
                {
                    var AuditWork = new AuditWorkDetailModel()
                    {
                        Id = checkAuditWorkDetail.Id,
                        Code = checkAuditWorkDetail.Code,
                        Name = checkAuditWorkDetail.Name,
                        Target = checkAuditWorkDetail.Target,
                        StartDate = checkAuditWorkDetail.StartDate.HasValue ? checkAuditWorkDetail.StartDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDate = checkAuditWorkDetail.EndDate.HasValue ? checkAuditWorkDetail.EndDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDatePlanning = checkAuditWorkDetail.EndDatePlanning.HasValue ? checkAuditWorkDetail.EndDatePlanning.Value.ToString("yyyy-MM-dd") : null,
                        StartDateReal = checkAuditWorkDetail.StartDateReal.HasValue ? checkAuditWorkDetail.StartDateReal.Value.ToString("yyyy-MM-dd") : null,
                        ReleaseDate = checkAuditWorkDetail.ReleaseDate.HasValue ? checkAuditWorkDetail.ReleaseDate.Value.ToString("yyyy-MM-dd") : null,
                        from_date = checkAuditWorkDetail.from_date.HasValue ? checkAuditWorkDetail.from_date.Value.ToString("yyyy-MM-dd") : null,
                        to_date = checkAuditWorkDetail.to_date.HasValue ? checkAuditWorkDetail.to_date.Value.ToString("yyyy-MM-dd") : null,
                        NumOfWorkdays = checkAuditWorkDetail.NumOfWorkdays,
                        person_in_charge = checkAuditWorkDetail.person_in_charge,
                        str_person_in_charge = checkAuditWorkDetail.person_in_charge.HasValue ? checkAuditWorkDetail.Users.Id + ":" + checkAuditWorkDetail.Users.FullName : "",
                        NumOfAuditor = /*checkAuditWorkDetail.NumOfAuditor*/auditAssignment.Length,
                        ReqSkillForAudit = checkAuditWorkDetail.ReqSkillForAudit,
                        ReqOutsourcing = checkAuditWorkDetail.ReqOutsourcing,
                        ReqOther = checkAuditWorkDetail.ReqOther,
                        ScaleOfAudit = checkAuditWorkDetail.ScaleOfAudit,
                        IsActive = checkAuditWorkDetail.IsActive,
                        IsDeleted = checkAuditWorkDetail.IsDeleted,
                        CreatedAt = checkAuditWorkDetail.CreatedAt.HasValue ? checkAuditWorkDetail.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                        CreatedBy = checkAuditWorkDetail.CreatedBy,
                        ModifiedAt = checkAuditWorkDetail.ModifiedAt.HasValue ? checkAuditWorkDetail.ModifiedAt.Value.ToString("yyyy-MM-dd") : null,
                        ModifiedBy = checkAuditWorkDetail.ModifiedBy,
                        DeletedAt = checkAuditWorkDetail.DeletedAt.HasValue ? checkAuditWorkDetail.DeletedAt.Value.ToString("yyyy-MM-dd") : null,
                        DeletedBy = checkAuditWorkDetail.DeletedBy,
                        Classify = checkAuditWorkDetail.Classify,
                        Year = checkAuditWorkDetail.Year,
                        //Status = checkAuditWorkDetail.Status,
                        Status = approval_status1?.StatusCode ?? "1.0",
                        statusName = approval_status_wait != null ? status_wait_name
                        : approval_status_browser != null ? status_browser_name
                        : approval_status_refuse != null ? status_refuse_name
                        : approval_status.StatusName,
                        AuditScopeOutside = checkAuditWorkDetail.AuditScopeOutside,
                        ExecutionStatusStr = checkAuditWorkDetail.ExecutionStatusStr,
                        AuditplanId = checkAuditWorkDetail.auditplan_id,
                        AuditScopeNew = checkAuditWorkDetail.AuditScope,
                        ListAuditPersonnel = null,
                        ListAuditWorkScope = auditWorkScope.Select(a => new ListAuditWorkScope
                        {
                            id = a.Id,
                            auditwork_id = a.auditwork_id,
                            auditprocess_id = a.auditprocess_id,
                            bussinessactivities_id = a.bussinessactivities_id,
                            auditfacilities_id = a.auditfacilities_id,
                            reason = a.ReasonNote,
                            risk_rating = a.RiskRatingName,//xếp hạng rủi ro
                            //risk_rating = a.RiskRating,
                            auditing_time_nearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd-MM-yyyy") : null,
                            auditprocess_name = a.auditprocess_name,
                            auditfacilities_name = a.auditfacilities_name,
                            bussinessactivities_name = a.bussinessactivities_name,
                            year = a.Year,
                            audit_team_leader = (auditWorkScope != null ? string.Join(", ", a.AuditWorkScopeUserMapping.ToList().Where(c => c.type == 1 && c.auditwork_scope_id == a.Id).Select(x => x.user_id + ":" + x.full_name).Distinct()) : ""),
                            auditor = (auditWorkScope != null ? string.Join(", ", a.AuditWorkScopeUserMapping.ToList().Where(c => c.type == 2 && c.auditwork_scope_id == a.Id).Select(x => x.user_id + ":" + x.full_name).Distinct()) : ""),
                            //brief_review = a.brief_review,
                            //path = a.path,
                            //filetype = a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",

                            //audit_team_leader = (checkAuditWorkScopeUserMapping != null ? string.Join(", ", checkAuditWorkScopeUserMapping.AuditWorkScopeUserMapping.ToList().Where(c => c.type == 1 && c.auditwork_scope_id == a.Id).Select(x => x.user_id + ":" + x.full_name).Distinct()) : ""),
                            //auditor = (checkAuditWorkScopeUserMapping != null ? string.Join(", ", checkAuditWorkScopeUserMapping.AuditWorkScopeUserMapping.ToList().Where(c => c.type == 2 && c.auditwork_scope_id == a.Id).Select(x => x.user_id + ":" + x.full_name).Distinct()) : ""),
                        }).ToList(),
                        ListSchedule = auditSchedule.Select(a => new ListSchedule
                        {
                            id = a.id,
                            user_id = a.user_id,
                            user_name = a.user_id.HasValue ? a.Users.FullName : "",
                            auditwork_id = a.auditwork_id,
                            work = a.work,
                            expected_date = a.expected_date.HasValue ? a.expected_date.Value.ToString("yyyy-MM-dd") : null,
                            actual_date = a.actual_date.HasValue ? a.actual_date.Value.ToString("yyyy-MM-dd") : null,
                            str_person_in_charge = a.user_id.HasValue ? a.user_id + ":" + a.user_name : "",
                        }).ToList(),
                        ListAuditFacility = checkAuditWorkScopeFacility.Length == 0 ? null : checkAuditWorkScopeFacility.Select(a => new ListAuditFacility
                        {
                            id = a.Id,
                            auditfacilities_id = a.auditfacilities_id,
                            auditfacilities_name = a.auditfacilities_name == null ? "" : a.auditfacilities_name,
                            reason = a.ReasonNote == null ? "" : a.ReasonNote,
                            risk_rating_id = a.RiskRating,
                            risk_rating = a.RiskRatingName == null ? "" : a.RiskRatingName,
                            AuditingTimeNearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd/MM/yyyy") : "",
                            brief_review = a.brief_review == null ? "" : a.brief_review,
                            //path = a.path == null ? "" : a.path,
                            //filetype = a.FileType == null ? "" : a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                            ListFiles = a.AuditWorkScopeFacilityFile.Where(x => x.IsDelete != true).Select(x => new AuditWorkScopeFacilityFilesModel()
                            {
                                id = x.id,
                                Path = x.Path,
                                FileType = x.FileType,
                            }).ToList(),
                        }).ToList(),
                        ListApprovalFunctionFile = checkApprovalFunctionFile.Where(a => a.IsDeleted != true).Select(x => new ListApprovalFunctionFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = AuditWork });
                }
                else if (checkAuditAssignment == null && checkAuditWorkScope == null && checkSchedule != null)
                {
                    var AuditWork = new AuditWorkDetailModel()
                    {
                        Id = checkAuditWorkDetail.Id,
                        Code = checkAuditWorkDetail.Code,
                        Name = checkAuditWorkDetail.Name,
                        Target = checkAuditWorkDetail.Target,
                        StartDate = checkAuditWorkDetail.StartDate.HasValue ? checkAuditWorkDetail.StartDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDate = checkAuditWorkDetail.EndDate.HasValue ? checkAuditWorkDetail.EndDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDatePlanning = checkAuditWorkDetail.EndDatePlanning.HasValue ? checkAuditWorkDetail.EndDatePlanning.Value.ToString("yyyy-MM-dd") : null,
                        StartDateReal = checkAuditWorkDetail.StartDateReal.HasValue ? checkAuditWorkDetail.StartDateReal.Value.ToString("yyyy-MM-dd") : null,
                        ReleaseDate = checkAuditWorkDetail.ReleaseDate.HasValue ? checkAuditWorkDetail.ReleaseDate.Value.ToString("yyyy-MM-dd") : null,
                        from_date = checkAuditWorkDetail.from_date.HasValue ? checkAuditWorkDetail.from_date.Value.ToString("yyyy-MM-dd") : null,
                        to_date = checkAuditWorkDetail.to_date.HasValue ? checkAuditWorkDetail.to_date.Value.ToString("yyyy-MM-dd") : null,
                        NumOfWorkdays = checkAuditWorkDetail.NumOfWorkdays,
                        person_in_charge = checkAuditWorkDetail.person_in_charge,
                        str_person_in_charge = checkAuditWorkDetail.person_in_charge.HasValue ? checkAuditWorkDetail.Users.Id + ":" + checkAuditWorkDetail.Users.FullName : "",
                        NumOfAuditor = /*checkAuditWorkDetail.NumOfAuditor*/auditAssignment.Length,
                        ReqSkillForAudit = checkAuditWorkDetail.ReqSkillForAudit,
                        ReqOutsourcing = checkAuditWorkDetail.ReqOutsourcing,
                        ReqOther = checkAuditWorkDetail.ReqOther,
                        ScaleOfAudit = checkAuditWorkDetail.ScaleOfAudit,
                        IsActive = checkAuditWorkDetail.IsActive,
                        IsDeleted = checkAuditWorkDetail.IsDeleted,
                        CreatedAt = checkAuditWorkDetail.CreatedAt.HasValue ? checkAuditWorkDetail.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                        CreatedBy = checkAuditWorkDetail.CreatedBy,
                        ModifiedAt = checkAuditWorkDetail.ModifiedAt.HasValue ? checkAuditWorkDetail.ModifiedAt.Value.ToString("yyyy-MM-dd") : null,
                        ModifiedBy = checkAuditWorkDetail.ModifiedBy,
                        DeletedAt = checkAuditWorkDetail.DeletedAt.HasValue ? checkAuditWorkDetail.DeletedAt.Value.ToString("yyyy-MM-dd") : null,
                        DeletedBy = checkAuditWorkDetail.DeletedBy,
                        Classify = checkAuditWorkDetail.Classify,
                        Year = checkAuditWorkDetail.Year,
                        //Status = checkAuditWorkDetail.Status,
                        Status = approval_status1?.StatusCode ?? "1.0",
                        statusName = approval_status_wait != null ? status_wait_name
                        : approval_status_browser != null ? status_browser_name
                        : approval_status_refuse != null ? status_refuse_name
                        : approval_status.StatusName,
                        AuditScopeOutside = checkAuditWorkDetail.AuditScopeOutside,
                        ExecutionStatusStr = checkAuditWorkDetail.ExecutionStatusStr,
                        AuditplanId = checkAuditWorkDetail.auditplan_id,
                        AuditScopeNew = checkAuditWorkDetail.AuditScope,
                        ListAuditPersonnel = null,
                        ListAuditWorkScope = null,
                        ListSchedule = auditSchedule.Select(a => new ListSchedule
                        {
                            id = a.id,
                            user_id = a.user_id,
                            user_name = a.user_id.HasValue ? a.Users.FullName : "",
                            auditwork_id = a.auditwork_id,
                            work = a.work,
                            expected_date = a.expected_date.HasValue ? a.expected_date.Value.ToString("yyyy-MM-dd") : null,
                            actual_date = a.actual_date.HasValue ? a.actual_date.Value.ToString("yyyy-MM-dd") : null,
                            str_person_in_charge = a.user_id.HasValue ? a.user_id + ":" + a.user_name : "",
                        }).ToList(),
                        ListAuditFacility = checkAuditWorkScopeFacility.Length == 0 ? null : checkAuditWorkScopeFacility.Select(a => new ListAuditFacility
                        {
                            id = a.Id,
                            auditfacilities_id = a.auditfacilities_id,
                            auditfacilities_name = a.auditfacilities_name == null ? "" : a.auditfacilities_name,
                            reason = a.ReasonNote == null ? "" : a.ReasonNote,
                            risk_rating_id = a.RiskRating,
                            risk_rating = a.RiskRatingName == null ? "" : a.RiskRatingName,
                            AuditingTimeNearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd/MM/yyyy") : "",
                            brief_review = a.brief_review == null ? "" : a.brief_review,
                            //path = a.path == null ? "" : a.path,
                            //filetype = a.FileType == null ? "" : a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                            ListFiles = a.AuditWorkScopeFacilityFile.Where(x => x.IsDelete != true).Select(x => new AuditWorkScopeFacilityFilesModel()
                            {
                                id = x.id,
                                Path = x.Path,
                                FileType = x.FileType,
                            }).ToList(),
                        }).ToList(),
                        ListApprovalFunctionFile = checkApprovalFunctionFile.Where(a => a.IsDeleted != true).Select(x => new ListApprovalFunctionFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = AuditWork });
                }
                else if (checkAuditAssignment == null && checkAuditWorkScope != null && checkSchedule == null)
                {
                    var AuditWork = new AuditWorkDetailModel()
                    {
                        Id = checkAuditWorkDetail.Id,
                        Code = checkAuditWorkDetail.Code,
                        Name = checkAuditWorkDetail.Name,
                        Target = checkAuditWorkDetail.Target,
                        StartDate = checkAuditWorkDetail.StartDate.HasValue ? checkAuditWorkDetail.StartDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDate = checkAuditWorkDetail.EndDate.HasValue ? checkAuditWorkDetail.EndDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDatePlanning = checkAuditWorkDetail.EndDatePlanning.HasValue ? checkAuditWorkDetail.EndDatePlanning.Value.ToString("yyyy-MM-dd") : null,
                        StartDateReal = checkAuditWorkDetail.StartDateReal.HasValue ? checkAuditWorkDetail.StartDateReal.Value.ToString("yyyy-MM-dd") : null,
                        ReleaseDate = checkAuditWorkDetail.ReleaseDate.HasValue ? checkAuditWorkDetail.ReleaseDate.Value.ToString("yyyy-MM-dd") : null,
                        from_date = checkAuditWorkDetail.from_date.HasValue ? checkAuditWorkDetail.from_date.Value.ToString("yyyy-MM-dd") : null,
                        to_date = checkAuditWorkDetail.to_date.HasValue ? checkAuditWorkDetail.to_date.Value.ToString("yyyy-MM-dd") : null,
                        NumOfWorkdays = checkAuditWorkDetail.NumOfWorkdays,
                        person_in_charge = checkAuditWorkDetail.person_in_charge,
                        str_person_in_charge = checkAuditWorkDetail.person_in_charge.HasValue ? checkAuditWorkDetail.Users.Id + ":" + checkAuditWorkDetail.Users.FullName : "",
                        NumOfAuditor = /*checkAuditWorkDetail.NumOfAuditor*/auditAssignment.Length,
                        ReqSkillForAudit = checkAuditWorkDetail.ReqSkillForAudit,
                        ReqOutsourcing = checkAuditWorkDetail.ReqOutsourcing,
                        ReqOther = checkAuditWorkDetail.ReqOther,
                        ScaleOfAudit = checkAuditWorkDetail.ScaleOfAudit,
                        IsActive = checkAuditWorkDetail.IsActive,
                        IsDeleted = checkAuditWorkDetail.IsDeleted,
                        CreatedAt = checkAuditWorkDetail.CreatedAt.HasValue ? checkAuditWorkDetail.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                        CreatedBy = checkAuditWorkDetail.CreatedBy,
                        ModifiedAt = checkAuditWorkDetail.ModifiedAt.HasValue ? checkAuditWorkDetail.ModifiedAt.Value.ToString("yyyy-MM-dd") : null,
                        ModifiedBy = checkAuditWorkDetail.ModifiedBy,
                        DeletedAt = checkAuditWorkDetail.DeletedAt.HasValue ? checkAuditWorkDetail.DeletedAt.Value.ToString("yyyy-MM-dd") : null,
                        DeletedBy = checkAuditWorkDetail.DeletedBy,
                        Classify = checkAuditWorkDetail.Classify,
                        Year = checkAuditWorkDetail.Year,
                        //Status = checkAuditWorkDetail.Status,
                        Status = approval_status1?.StatusCode ?? "1.0",
                        statusName = approval_status_wait != null ? status_wait_name
                        : approval_status_browser != null ? status_browser_name
                        : approval_status_refuse != null ? status_refuse_name
                        : approval_status.StatusName,
                        AuditScopeOutside = checkAuditWorkDetail.AuditScopeOutside,
                        ExecutionStatusStr = checkAuditWorkDetail.ExecutionStatusStr,
                        AuditplanId = checkAuditWorkDetail.auditplan_id,
                        AuditScopeNew = checkAuditWorkDetail.AuditScope,
                        ListAuditPersonnel = null,
                        ListAuditWorkScope = auditWorkScope.Select(a => new ListAuditWorkScope
                        {
                            id = a.Id,
                            auditwork_id = a.auditwork_id,
                            auditprocess_id = a.auditprocess_id,
                            bussinessactivities_id = a.bussinessactivities_id,
                            auditfacilities_id = a.auditfacilities_id,
                            reason = a.ReasonNote,
                            risk_rating = a.RiskRatingName,//xếp hạng rủi ro
                            //risk_rating = a.RiskRating,
                            auditing_time_nearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd-MM-yyyy") : null,
                            auditprocess_name = a.auditprocess_name,
                            auditfacilities_name = a.auditfacilities_name,
                            bussinessactivities_name = a.bussinessactivities_name,
                            year = a.Year,
                            audit_team_leader = (auditWorkScope != null ? string.Join(", ", a.AuditWorkScopeUserMapping.ToList().Where(c => c.type == 1 && c.auditwork_scope_id == a.Id).Select(x => x.user_id + ":" + x.full_name).Distinct()) : ""),
                            auditor = (auditWorkScope != null ? string.Join(", ", a.AuditWorkScopeUserMapping.ToList().Where(c => c.type == 2 && c.auditwork_scope_id == a.Id).Select(x => x.user_id + ":" + x.full_name).Distinct()) : ""),
                            //brief_review = a.brief_review,
                            //path = a.path,
                            //filetype = a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                        }).ToList(),
                        ListSchedule = null,
                        ListAuditFacility = checkAuditWorkScopeFacility.Length == 0 ? null : checkAuditWorkScopeFacility.Select(a => new ListAuditFacility
                        {
                            id = a.Id,
                            auditfacilities_id = a.auditfacilities_id,
                            auditfacilities_name = a.auditfacilities_name == null ? "" : a.auditfacilities_name,
                            reason = a.ReasonNote == null ? "" : a.ReasonNote,
                            risk_rating_id = a.RiskRating,
                            risk_rating = a.RiskRatingName == null ? "" : a.RiskRatingName,
                            AuditingTimeNearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd/MM/yyyy") : "",
                            brief_review = a.brief_review == null ? "" : a.brief_review,
                            //path = a.path == null ? "" : a.path,
                            //filetype = a.FileType == null ? "" : a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                            ListFiles = a.AuditWorkScopeFacilityFile.Where(x => x.IsDelete != true).Select(x => new AuditWorkScopeFacilityFilesModel()
                            {
                                id = x.id,
                                Path = x.Path,
                                FileType = x.FileType,
                            }).ToList(),
                        }).ToList(),
                        ListApprovalFunctionFile = checkApprovalFunctionFile.Where(a => a.IsDeleted != true).Select(x => new ListApprovalFunctionFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = AuditWork });
                }
                else if (checkAuditWorkDetail != null && checkAuditAssignment == null && checkAuditWorkScope == null && checkSchedule == null)
                {
                    var AuditWork = new AuditWorkDetailModel()
                    {
                        Id = checkAuditWorkDetail.Id,
                        Code = checkAuditWorkDetail.Code,
                        Name = checkAuditWorkDetail.Name,
                        Target = checkAuditWorkDetail.Target,
                        StartDate = checkAuditWorkDetail.StartDate.HasValue ? checkAuditWorkDetail.StartDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDate = checkAuditWorkDetail.EndDate.HasValue ? checkAuditWorkDetail.EndDate.Value.ToString("yyyy-MM-dd") : null,
                        EndDatePlanning = checkAuditWorkDetail.EndDatePlanning.HasValue ? checkAuditWorkDetail.EndDatePlanning.Value.ToString("yyyy-MM-dd") : null,
                        StartDateReal = checkAuditWorkDetail.StartDateReal.HasValue ? checkAuditWorkDetail.StartDateReal.Value.ToString("yyyy-MM-dd") : null,
                        ReleaseDate = checkAuditWorkDetail.ReleaseDate.HasValue ? checkAuditWorkDetail.ReleaseDate.Value.ToString("yyyy-MM-dd") : null,
                        from_date = checkAuditWorkDetail.from_date.HasValue ? checkAuditWorkDetail.from_date.Value.ToString("yyyy-MM-dd") : null,
                        to_date = checkAuditWorkDetail.to_date.HasValue ? checkAuditWorkDetail.to_date.Value.ToString("yyyy-MM-dd") : null,
                        NumOfWorkdays = checkAuditWorkDetail.NumOfWorkdays,
                        person_in_charge = checkAuditWorkDetail.person_in_charge,
                        str_person_in_charge = checkAuditWorkDetail.person_in_charge.HasValue ? checkAuditWorkDetail.Users.Id + ":" + checkAuditWorkDetail.Users.FullName : "",
                        NumOfAuditor = /*checkAuditWorkDetail.NumOfAuditor*/auditAssignment.Length,
                        ReqSkillForAudit = checkAuditWorkDetail.ReqSkillForAudit,
                        ReqOutsourcing = checkAuditWorkDetail.ReqOutsourcing,
                        ReqOther = checkAuditWorkDetail.ReqOther,
                        ScaleOfAudit = checkAuditWorkDetail.ScaleOfAudit,
                        IsActive = checkAuditWorkDetail.IsActive,
                        IsDeleted = checkAuditWorkDetail.IsDeleted,
                        CreatedAt = checkAuditWorkDetail.CreatedAt.HasValue ? checkAuditWorkDetail.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                        CreatedBy = checkAuditWorkDetail.CreatedBy,
                        ModifiedAt = checkAuditWorkDetail.ModifiedAt.HasValue ? checkAuditWorkDetail.ModifiedAt.Value.ToString("yyyy-MM-dd") : null,
                        ModifiedBy = checkAuditWorkDetail.ModifiedBy,
                        DeletedAt = checkAuditWorkDetail.DeletedAt.HasValue ? checkAuditWorkDetail.DeletedAt.Value.ToString("yyyy-MM-dd") : null,
                        DeletedBy = checkAuditWorkDetail.DeletedBy,
                        Classify = checkAuditWorkDetail.Classify,
                        Year = checkAuditWorkDetail.Year,
                        //Status = checkAuditWorkDetail.Status,
                        Status = approval_status1?.StatusCode ?? "1.0",
                        statusName = approval_status_wait != null ? status_wait_name
                        : approval_status_browser != null ? status_browser_name
                        : approval_status_refuse != null ? status_refuse_name
                        : approval_status.StatusName,
                        AuditScopeOutside = checkAuditWorkDetail.AuditScopeOutside,
                        ExecutionStatusStr = checkAuditWorkDetail.ExecutionStatusStr,
                        AuditplanId = checkAuditWorkDetail.auditplan_id,
                        AuditScopeNew = checkAuditWorkDetail.AuditScope,
                        ListAuditPersonnel = null,
                        ListAuditWorkScope = null,
                        ListSchedule = null,
                        ListAuditFacility = checkAuditWorkScopeFacility.Length == 0 ? null : checkAuditWorkScopeFacility.Select(a => new ListAuditFacility
                        {
                            id = a.Id,
                            auditfacilities_id = a.auditfacilities_id,
                            auditfacilities_name = a.auditfacilities_name == null ? "" : a.auditfacilities_name,
                            reason = a.ReasonNote == null ? "" : a.ReasonNote,
                            risk_rating_id = a.RiskRating,
                            risk_rating = a.RiskRatingName == null ? "" : a.RiskRatingName,
                            AuditingTimeNearest = a.AuditingTimeNearest.HasValue ? a.AuditingTimeNearest.Value.ToString("dd/MM/yyyy") : "",
                            brief_review = a.brief_review == null ? "" : a.brief_review,
                            //path = a.path == null ? "" : a.path,
                            //filetype = a.FileType == null ? "" : a.FileType,
                            //filename = a.path != null ? a.path.ToString().Remove(0, (a.path.ToString().LastIndexOf("\\")) + 1) : "",
                            ListFiles = a.AuditWorkScopeFacilityFile.Where(x => x.IsDelete != true).Select(x => new AuditWorkScopeFacilityFilesModel()
                            {
                                id = x.id,
                                Path = x.Path,
                                FileType = x.FileType,
                            }).ToList(),
                        }).ToList(),
                        ListApprovalFunctionFile = checkApprovalFunctionFile.Where(a => a.IsDeleted != true).Select(x => new ListApprovalFunctionFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = AuditWork });
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
        [HttpPost("RequestApproval")]
        public IActionResult RequestApproval(ApprovalModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditPlan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == model.auditplanid);
                if (checkAuditPlan == null)
                {
                    return NotFound();
                }
                if (checkAuditPlan.Status == 1 || checkAuditPlan.Status == 4)
                {
                    checkAuditPlan.Status = 2;
                    checkAuditPlan.approval_user = model.approvaluser;
                    _uow.Repository<AuditPlan>().Update(checkAuditPlan);
                    var result = new AuditPlanListModel()
                    {
                        Id = checkAuditPlan?.Id,
                        Version = checkAuditPlan?.Version,
                    };
                    return Ok(new { code = "1", data = result, msg = "success" });
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("SubmitApproval/{id}")]
        public IActionResult SubmitApproval(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditPlan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == id);
                if (checkAuditPlan == null)
                {
                    return NotFound();
                }
                var list_all = _uow.Repository<AuditPlan>().Find(a => a.IsDelete != true).ToArray();
                checkAuditPlan.Status = 3;
                checkAuditPlan.Browsedate = DateTime.Now;
                var list_old = list_all.FirstOrDefault(a => a.Year == checkAuditPlan.Year && a.Id != checkAuditPlan.Id && a.Status == 3);
                if (list_old != null)
                {
                    list_old.Status = 5;
                    _uow.Repository<AuditPlan>().Update(list_old);
                }

                var check_count = list_all.Count(a => a.Year == checkAuditPlan.Year);
                if (check_count == 1)
                {
                    checkAuditPlan.Version = 1;
                }
                else
                {
                    var ver = list_all.Where(a => a.Year == checkAuditPlan.Year).Max(a => a.Version);
                    checkAuditPlan.Version = !ver.HasValue ? 1 : ver + 1;
                }
                _uow.Repository<AuditPlan>().Update(checkAuditPlan);
                var result = new AuditPlanListModel()
                {
                    Id = checkAuditPlan?.Id,
                    Version = checkAuditPlan?.Version,
                };
                return Ok(new { code = "1", data = result, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("RejectApproval")]
        public IActionResult RejectApproval(RejectApprovalModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditPlan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == model.id);
                if (checkAuditPlan == null)
                {
                    return NotFound();
                }
                checkAuditPlan.Status = 4;
                checkAuditPlan.ReasonReject = model.reason_note;
                _uow.Repository<AuditPlan>().Update(checkAuditPlan);
                var result = new AuditPlanListModel()
                {
                    Id = checkAuditPlan?.Id,
                    Version = checkAuditPlan?.Version,
                };
                return Ok(new { code = "1", data = result, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("AditUpdateStatus")]
        public IActionResult updateStatus()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _changeStatus = new AuditPlanUpdateStatusModel();
                var data = Request.Form["data"];
                var pathSave = "";
                var file_type = "";
                if (!string.IsNullOrEmpty(data))
                {
                    _changeStatus = JsonSerializer.Deserialize<AuditPlanUpdateStatusModel>(data);
                    var file = Request.Form.Files;
                    file_type = file.FirstOrDefault()?.ContentType;
                    pathSave = CreateUploadFile(file, "AuditWorkPlan");
                }
                else
                {
                    return BadRequest();
                }
                var list_all = _uow.Repository<AuditPlan>().Find(a => a.IsDelete != true).ToArray();
                var updateStatus = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == _changeStatus.Id && a.IsDelete.Equals(false));
                if (updateStatus == null)
                {
                    return NotFound();
                }
                var checkStatus = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == _changeStatus.Id && a.Status.Equals(2) && a.IsDelete.Equals(false));
                if (checkStatus == null)
                {
                    return BadRequest();
                }
                updateStatus.Status = Int32.Parse(_changeStatus.Status);
                if (updateStatus.Status == 3)
                {
                    var list_old = list_all.FirstOrDefault(a => a.Year == updateStatus.Year && a.Id != updateStatus.Id && a.Status == 3);
                    if (list_old != null)
                    {
                        list_old.Status = 5;
                        _uow.Repository<AuditPlan>().Update(list_old);
                    }

                    var check_count = list_all.Count(a => a.Year == updateStatus.Year);
                    if (check_count == 1)
                    {
                        updateStatus.Version = 1;
                    }
                    else
                    {
                        var ver = list_all.Where(a => a.Year == updateStatus.Year).Max(a => a.Version);
                        updateStatus.Version = !ver.HasValue ? 1 : ver + 1;
                    }
                }

                if (!string.IsNullOrEmpty(_changeStatus.Browsedate))
                {
                    updateStatus.Browsedate = DateTime.ParseExact(_changeStatus.Browsedate, "yyyy-MM-dd", null);
                }
                //updateStatus.Browsedate = _changeStatus.Browsedate;
                updateStatus.Path = pathSave;
                updateStatus.FileType = file_type;
                _uow.Repository<AuditPlan>().Update(updateStatus);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //tìm kiếm Chuẩn bị cho kiểm toán > kế hoạch cuộc kiểm toán
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
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_PAP").ToArray();
                var prepareaudit_id = approval_status.Select(a => a.item_id).ToList();

                var approval_status_code = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_PAP"
                && a.StatusCode == obj.Status).ToArray();
                var pre_id = approval_status_code.Select(a => a.item_id).ToList();
                Expression<Func<AuditWork, bool>> filter = c => (string.IsNullOrEmpty(obj.Year) || Convert.ToString(c.Year).Contains(obj.Year))
                                               && ((obj.Classify == -1) || c.Classify.Equals(obj.Classify))
                                               && (string.IsNullOrEmpty(obj.Code) || Convert.ToString(c.Code.ToLower()).Contains(obj.Code.ToLower()))
                                               && (string.IsNullOrEmpty(obj.Name) || Convert.ToString(c.Name.ToLower()).Contains(obj.Name.ToLower()))
                                               && ((obj.PersonInCharge == null) || c.person_in_charge.Equals(obj.PersonInCharge))
                                               && ((obj.Status == "-1")
                                               || ((obj.Status != "1.0" && obj.Status != "-1") ? pre_id.Contains(c.Id)
                                               : (obj.Status == "1.0" && obj.Status != "-1") ? !prepareaudit_id.Contains(c.Id)
                                               : prepareaudit_id.Contains(c.Id)))
                                               //&& ((obj.ExecutionStatus == -1) || c.ExecutionStatus.Equals(obj.ExecutionStatus))
                                               && (string.IsNullOrEmpty(obj.ExecutionStatusStr)
                                               || Convert.ToString(c.ExecutionStatusStr.ToLower()).Contains(obj.ExecutionStatusStr.ToLower()))
                                               && c.IsDeleted != true;
                //&& c.CreatedBy == userInfo.Id;
                var list_auditwork = _uow.Repository<AuditWork>().Include(x => x.Users).Where(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditWork> data = list_auditwork;
                var count = list_auditwork.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }

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
                    Status = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.StatusCode ?? "1.0",
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
                    ApprovalUser = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.approver,
                    ApprovalUserLast = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.approver_last
                });
                return Ok(new { code = "1", msg = "success", data = auditWork, total = count });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //Cập nhật kế hoạch cuộc kiểm toán
        [HttpPut("PrepareAuditUpdate")]
        public IActionResult PrepareAuditUpdate([FromBody] AuditWorkEditModel auditWorkEditModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }

                var updatePrepareAudit = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == auditWorkEditModel.Id && a.IsDeleted.Equals(false));
                if (updatePrepareAudit == null)
                {
                    return NotFound();
                }
                //comment tạm khi có quyền mở lại
                //if (updatePrepareAudit.CreatedBy != _userInfo.Id)
                //{
                //    return Ok(new { code = "403", msg = "Người dùng này không có quyền sửa dữ liệu" });
                //}
                if (auditWorkEditModel.StartDate > auditWorkEditModel.EndDate)
                {
                    return Ok(new { code = "499", msg = "Ngày bắt đầu không được lớn hơn ngày kết thúc" });
                }
                if (auditWorkEditModel.from_date > auditWorkEditModel.to_date)
                {
                    return Ok(new { code = "498", msg = "Ngày bắt đầu không được lớn hơn ngày kết thúc" });
                }
                if (auditWorkEditModel.ReleaseDate < auditWorkEditModel.EndDate)
                {
                    return Ok(new { code = "497", msg = "Ngày phát hành báo cáo không được nhỏ hơn ngày kết thúc thực địa" });
                }
                //tab 2
                if (auditWorkEditModel.ListAuditAssignment.Count() > 0)
                {
                    var checkAuditAssignment = _uow.Repository<AuditAssignment>().Find(z => z.auditwork_id.Equals(auditWorkEditModel.Id)).ToArray();
                    if (checkAuditAssignment.Any())
                    {
                        _uow.Repository<AuditAssignment>().Delete(checkAuditAssignment);
                        _uow.SaveChanges();
                    }
                    for (int i = 0; i < auditWorkEditModel.ListAuditAssignment.Count(); i++)
                    {
                        //var checkAuditAssignment = _uow.Repository<AuditAssignment>().Find(z => z.auditwork_id.Equals(auditWorkEditModel.ListAuditAssignment[i].auditwork_id)/* && z.user_id.Equals(auditWorkEditModel.ListAuditAssignment[i].user_id)*/);
                        //if (checkAuditAssignment.Any())
                        //{
                        //    _uow.Repository<AuditAssignment>().Delete(checkAuditAssignment);
                        //    _uow.SaveChanges();
                        //}
                        var list_user = _uow.Repository<Users>().FirstOrDefault(x => x.Id == auditWorkEditModel.ListAuditAssignment[i].user_id);
                        if (auditWorkEditModel.ListAuditAssignment[i].user_id != null)
                        {
                            var auditAssignment = new AuditAssignment
                            {
                                user_id = auditWorkEditModel.ListAuditAssignment[i].user_id,
                                auditwork_id = auditWorkEditModel.ListAuditAssignment[i].auditwork_id,
                                fullName = list_user.FullName,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedAt = DateTime.Now,
                                CreatedBy = _userInfo.Id,
                            };
                            _uow.Repository<AuditAssignment>().Add(auditAssignment);
                            _uow.SaveChanges();
                        }
                    }
                }
                //tab 3
                if (auditWorkEditModel.ListAuditWorkScope != null)
                {
                    for (int i = 0; i < auditWorkEditModel.ListAuditWorkScope.Count(); i++)
                    {
                        var list_user = _uow.Repository<Users>().FirstOrDefault(x => x.Id == auditWorkEditModel.ListAuditWorkScope[i].audit_team_leader);

                        var updateAuditWorkScope = _uow.Repository<AuditWorkScope>().FirstOrDefault(a => a.Id == auditWorkEditModel.ListAuditWorkScope[i].id);
                        var checkAuditTeamLeader = _uow.Repository<AuditWorkScopeUserMapping>().Find(z => z.type == 1
                        && z.auditwork_scope_id.Equals(auditWorkEditModel.ListAuditWorkScope[i].id));
                        if (checkAuditTeamLeader.Any())
                        {
                            _uow.Repository<AuditWorkScopeUserMapping>().Delete(checkAuditTeamLeader);
                            _uow.SaveChanges();
                        }
                        var checkAuditor = _uow.Repository<AuditWorkScopeUserMapping>().Find(z => z.type == 2 && z.auditwork_scope_id.Equals(auditWorkEditModel.ListAuditWorkScope[i].id)).ToArray();
                        if (checkAuditor.Any())
                        {
                            _uow.Repository<AuditWorkScopeUserMapping>().Delete(checkAuditor);
                            _uow.SaveChanges();
                        }
                        List<AuditWorkScopeUserMapping> _leaderMapping = new();
                        if (list_user != null)
                        {
                            var mapping = new AuditWorkScopeUserMapping
                            {
                                type = 1,
                                auditwork_scope_id = (int)auditWorkEditModel.ListAuditWorkScope[i].id,
                                user_id = list_user.Id,
                                full_name = list_user.FullName
                            };
                            _leaderMapping.Add(mapping);
                        }

                        if (auditWorkEditModel.ListAuditWorkScope[i].auditor.Count > 0)
                        {
                            foreach (var item in auditWorkEditModel.ListAuditWorkScope[i].auditor)
                            {
                                var list_user_auditor = _uow.Repository<Users>().FirstOrDefault(x => x.Id == item);
                                var mappingAuditor = new AuditWorkScopeUserMapping
                                {
                                    type = 2,
                                    auditwork_scope_id = (int)auditWorkEditModel.ListAuditWorkScope[i].id,
                                    user_id = (int)item,
                                    full_name = list_user_auditor.FullName,
                                };
                                _leaderMapping.Add(mappingAuditor);
                            }
                        }
                        _uow.Repository<AuditWorkScopeUserMapping>().Insert(_leaderMapping);
                        _uow.SaveChanges();
                        //}
                    }
                }
                //tab 4
                if (auditWorkEditModel.ListScheduleEditModel != null)
                {
                    for (int z = 0; z < auditWorkEditModel.ListScheduleEditModel.Count(); z++)
                    {
                        var checkSchedule = _uow.Repository<Schedule>().Find(x => x.auditwork_id == auditWorkEditModel.ListScheduleEditModel[z].auditwork_id);
                        if (checkSchedule.Any())
                        {
                            _uow.Repository<Schedule>().Delete(checkSchedule);
                            _uow.SaveChanges();
                        }
                    }
                    for (int i = 0; i < auditWorkEditModel.ListScheduleEditModel.Count(); i++)
                    {
                        var list_user = _uow.Repository<Users>().FirstOrDefault(x => x.Id == auditWorkEditModel.ListScheduleEditModel[i].user_id);

                        if (list_user != null)
                        {
                            List<Schedule> _schedule = new();
                            var mapping_schedule = new Schedule
                            {
                                work = auditWorkEditModel.ListScheduleEditModel[i].work,
                                auditwork_id = (int)auditWorkEditModel.ListScheduleEditModel[i].auditwork_id,
                                user_id = list_user.Id,
                                user_name = list_user.FullName,
                                expected_date = DateTime.Parse(auditWorkEditModel.ListScheduleEditModel[i].expected_date),
                                //actual_date = DateTime.Parse(auditWorkEditModel.ListScheduleEditModel[i].actual_date),
                                is_deleted = false,
                            };
                            _schedule.Add(mapping_schedule);
                            _uow.Repository<Schedule>().Insert(_schedule);
                            _uow.SaveChanges();
                        }
                    }
                }
                updatePrepareAudit.Year = auditWorkEditModel.Year;
                updatePrepareAudit.Code = auditWorkEditModel.Code;
                updatePrepareAudit.Name = auditWorkEditModel.Name;
                updatePrepareAudit.Target = auditWorkEditModel.Target;
                updatePrepareAudit.person_in_charge = auditWorkEditModel.person_in_charge;
                //updatePrepareAudit.Budget = auditWorkEditModel.budget;
                var checkAuditAssignmentAny = _uow.Repository<AuditAssignment>().Find(z => z.auditwork_id.Equals(auditWorkEditModel.Id) && z.user_id.Equals(auditWorkEditModel.person_in_charge));
                if (checkAuditAssignmentAny.Any())
                {
                    _uow.Repository<AuditAssignment>().Delete(checkAuditAssignmentAny);
                    _uow.SaveChanges();
                }
                var list_person = _uow.Repository<Users>().FirstOrDefault(x => x.Id == auditWorkEditModel.person_in_charge);
                if (auditWorkEditModel.person_in_charge != null)
                {
                    var auditAssignmentcreate = new AuditAssignment
                    {
                        user_id = auditWorkEditModel.person_in_charge,
                        auditwork_id = auditWorkEditModel.Id,
                        fullName = list_person.FullName,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _userInfo.Id,
                    };
                    _uow.Repository<AuditAssignment>().Add(auditAssignmentcreate);
                    _uow.SaveChanges();
                }
                updatePrepareAudit.Classify = auditWorkEditModel.Classify;
                updatePrepareAudit.StartDate = auditWorkEditModel.StartDate;
                updatePrepareAudit.EndDatePlanning = auditWorkEditModel.EndDatePlanning;
                updatePrepareAudit.StartDateReal = auditWorkEditModel.StartDateReal;
                updatePrepareAudit.ReleaseDate = auditWorkEditModel.ReleaseDate;
                updatePrepareAudit.EndDate = auditWorkEditModel.EndDate;
                updatePrepareAudit.from_date = auditWorkEditModel.from_date;
                updatePrepareAudit.to_date = auditWorkEditModel.to_date;
                updatePrepareAudit.NumOfAuditor = auditWorkEditModel.NumOfAuditor.HasValue ? auditWorkEditModel.NumOfAuditor : null;
                updatePrepareAudit.NumOfWorkdays = auditWorkEditModel.NumOfWorkdays.HasValue ? auditWorkEditModel.NumOfWorkdays : null;
                //updatePrepareAudit.Status = auditWorkEditModel.Status;
                updatePrepareAudit.ExecutionStatusStr = auditWorkEditModel.ExecutionStatusStr;
                updatePrepareAudit.ModifiedAt = DateTime.Now;
                updatePrepareAudit.ModifiedBy = _userInfo.Id;
                updatePrepareAudit.auditplan_id = auditWorkEditModel.auditplan_id;
                updatePrepareAudit.AuditScopeOutside = auditWorkEditModel.AuditScopeOutside;
                updatePrepareAudit.AuditScope = auditWorkEditModel.AuditScope;
                //updatePrepareAudit.ExtensionTime = auditWorkEditModel.ExtensionTime;
                var checkMainStage = _uow.Repository<MainStage>().Find(x => x.auditwork_id == updatePrepareAudit.Id);
                if (checkMainStage.Any())
                {
                    _uow.Repository<MainStage>().Delete(checkMainStage);
                    _uow.SaveChanges();
                }
                List<MainStage> _mainstage = new();
                var mapping_startdate = new MainStage
                {
                    auditwork_id = updatePrepareAudit.Id,
                    stage = "Bắt đầu lập kế hoạch",
                    expected_date = auditWorkEditModel.StartDate,
                    status = "",
                    created_at = DateTime.Now,
                    created_by = _userInfo.Id,
                    index = 1,
                };
                _mainstage.Add(mapping_startdate);
                var mapping_enddateplanning = new MainStage
                {
                    auditwork_id = updatePrepareAudit.Id,
                    stage = "Kết thúc lập kế hoạch",
                    expected_date = auditWorkEditModel.EndDatePlanning,
                    status = "Chưa thực hiện",
                    created_at = DateTime.Now,
                    created_by = _userInfo.Id,
                    index = 2,
                };
                _mainstage.Add(mapping_enddateplanning);
                var mapping_startdatereal = new MainStage
                {
                    auditwork_id = updatePrepareAudit.Id,
                    stage = "Bắt đầu thực địa",
                    expected_date = auditWorkEditModel.StartDateReal,
                    status = "Chưa thực hiện",
                    created_at = DateTime.Now,
                    created_by = _userInfo.Id,
                    index = 3,
                };
                _mainstage.Add(mapping_startdatereal);
                var mapping_releasedate = new MainStage
                {
                    auditwork_id = updatePrepareAudit.Id,
                    stage = "Kết thúc thực địa",
                    expected_date = auditWorkEditModel.EndDate,
                    status = "Chưa thực hiện",
                    created_at = DateTime.Now,
                    created_by = _userInfo.Id,
                    index = 4,
                };
                _mainstage.Add(mapping_releasedate);
                var mapping_enddate = new MainStage
                {
                    auditwork_id = updatePrepareAudit.Id,
                    stage = "Phát hành báo cáo",
                    expected_date = auditWorkEditModel.ReleaseDate,
                    status = "Chưa thực hiện",
                    created_at = DateTime.Now,
                    created_by = _userInfo.Id,
                    index = 5,
                };
                _mainstage.Add(mapping_enddate);
                _uow.Repository<MainStage>().Insert(_mainstage);
                _uow.SaveChanges();


                _uow.Repository<AuditWork>().Update(updatePrepareAudit);

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("ListYearAuditWork")]//list •	Cuộc năm của kiểm toán
        public IActionResult ListYearAuditWork(string q)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = q;
                Expression<Func<AuditWork, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Year.Contains(q) || c.Year.Contains(q)) && c.IsActive == true && c.IsDeleted != true;
                var listyear = _uow.Repository<AuditWork>().Find(filter)/*.OrderByDescending(a => a.Year)*/;
                IEnumerable<AuditWork> data = listyear;
                var res = data.Select(a => new DropListYearAuditWorkModel()
                {
                    year = a.Year,
                    current_year = (a.Year == DateTime.Now.Year.ToString() ? true : false),
                    year_sort = Int32.Parse(a.Year),
                });
                var lst = res.GroupBy(a => a.year).Select(x => x.FirstOrDefault()).ToList();
                lst.Sort((x, y) => y.year_sort.CompareTo(x.year_sort));
                return Ok(new { code = "1", msg = "success", data = lst, total = lst.Count() });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = "", total = 0 });
            }
        }
        [HttpGet("ListAuditWork")] //list •	Cuộc kiểm toán
        public IActionResult ListAuditWork(string q, string year)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_PAP" && a.StatusCode == "3.1").ToArray();
                var audiplan_id = approval_status.Select(a => a.item_id).ToList();

                string KeyWord = q;
                Expression<Func<AuditWork, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Name.ToLower().Contains(q.ToLower()) || c.Name.ToLower().Contains(q.ToLower()))
                && c.IsActive == true && c.IsDeleted != true && c.Year == year && audiplan_id.Contains(c.Id);
                var listauditwork = _uow.Repository<AuditWork>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditWork> dropauditwork = listauditwork;
                //var dropauditwork = _uow.Repository<AuditWork>().GetAll(a => a.IsDeleted != true && a.Year == year && a.Status.Equals(3));
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
        [HttpGet("ListAuditFacility")] //list •	Đơn vị được KT
        public IActionResult ListAuditFacility(string q, int? auditwork_id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = q;
                Expression<Func<AuditWorkScopeFacility, bool>> filter = c => (string.IsNullOrEmpty(q) || c.auditfacilities_name.ToLower().Contains(q.ToLower()) || c.auditfacilities_name.ToLower().Contains(q.ToLower()))
                && c.IsDeleted != true
                && c.auditwork_id == auditwork_id;
                var listauditfacility = _uow.Repository<AuditWorkScopeFacility>().Find(filter);
                IEnumerable<AuditWorkScopeFacility> dropauditfacility = listauditfacility;

                //var dropauditfacility = _uow.Repository<AuditWorkScope>().GetAll(a => a.auditwork_id == auditwork_id && a.IsDeleted.Equals(false));
                var res = dropauditfacility.Select(a => new DropListAuditFacilityModel()
                {
                    id = a.Id,
                    auditfacilities_id = a.auditfacilities_id,
                    auditfacilities_name = a.auditfacilities_name,
                });
                return Ok(new { code = "1", msg = "success", data = res, total = res.Count() });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = "", total = 0 });
            }
        }
        [HttpGet("ListAuditProcess")] //list •	Quy trình được KT
        public IActionResult ListAuditProcess(string q, int? auditwork_id, int? auditfacilities_id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = q;
                Expression<Func<AuditWorkScope, bool>> filter = c => (string.IsNullOrEmpty(q) || c.auditprocess_name.ToLower().Contains(q.ToLower()) || c.auditprocess_name.ToLower().Contains(q.ToLower()))
                && c.IsDeleted != true && c.auditwork_id == auditwork_id && c.auditfacilities_id == auditfacilities_id;
                var listauditprocess = _uow.Repository<AuditWorkScope>().Find(filter);
                if (listauditprocess == null)
                {
                    return Ok(new { code = "1", msg = "success", data = "" });
                }
                IEnumerable<AuditWorkScope> dropauditprocess = listauditprocess;

                //var dropauditprocess = _uow.Repository<AuditWorkScope>().GetAll(a => a.auditwork_id == auditwork_id && a.auditfacilities_id == auditfacilities_id && a.IsDeleted.Equals(false));

                var res = dropauditprocess.Select(a => new DropListAuditProcessModel()
                {
                    id = a.Id,
                    auditprocess_id = a.auditprocess_id,
                    auditprocess_name = a.auditprocess_name,
                });
                return Ok(new { code = "1", msg = "success", data = res, total = res.Count() });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = "", total = 0 });
            }
        }
        [HttpGet("SelectAudiAssignment")]//list •	nhân sự KT
        public IActionResult SelectAudiAssignment(string q, int? auditwork_id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = q;
                Expression<Func<AuditAssignment, bool>> filter = c => (string.IsNullOrEmpty(q) || c.fullName.ToLower().Contains(q.ToLower()))
                && c.IsActive == true && c.IsDeleted != true && c.auditwork_id == auditwork_id;
                var list_auditassignment = _uow.Repository<AuditAssignment>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditAssignment> data = list_auditassignment;
                var dt = data.Select(a => new DropListAuditAssignmentModel()
                {
                    id = a.Id,
                    user_id = a.user_id,
                    fullName = a.fullName,
                });
                return Ok(new { code = "1", msg = "success", data = dt });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new DropListAuditAssignmentModel() });
            }
        }
        //xóa AuditWorkScope tab 3
        [HttpDelete("DeleteAuditWorkScope/{id}")]
        public IActionResult DeleteAuditWorkScope(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var AuditWorkScope = _uow.Repository<AuditWorkScope>().FirstOrDefault(a => a.Id == id && a.IsDeleted != true);
                if (AuditWorkScope == null)
                {
                    return NotFound();
                }
                var checkAuditProgram = _uow.Repository<AuditProgram>().FirstOrDefault(a => a.auditwork_id == AuditWorkScope.auditwork_id
                && a.auditprocess_id == AuditWorkScope.auditprocess_id
                && a.bussinessactivities_id == AuditWorkScope.bussinessactivities_id
                && a.auditfacilities_id == AuditWorkScope.auditfacilities_id
                && a.Year == AuditWorkScope.Year
                && a.IsDeleted != true);
                if (checkAuditProgram != null)
                {
                    return Ok(new { code = "417", msg = "fail" });
                }
                var checkAuditWorkScope = _uow.Repository<AuditWorkScope>().Find(a => a.auditwork_id == AuditWorkScope.auditwork_id
                && a.IsDeleted != true
                && a.auditfacilities_id == AuditWorkScope.auditfacilities_id
                && a.Year == AuditWorkScope.Year).ToArray();

                var checkAuditWorkScopeFacility = _uow.Repository<AuditWorkScopeFacility>().FirstOrDefault(a => a.auditwork_id == AuditWorkScope.auditwork_id
                && a.IsDeleted != true
                && a.auditfacilities_id == AuditWorkScope.auditfacilities_id
                && a.Year == AuditWorkScope.Year);

                AuditWorkScope.IsDeleted = true;
                AuditWorkScope.DeletedAt = DateTime.Now;
                AuditWorkScope.DeletedBy = userInfo.Id;
                _uow.Repository<AuditWorkScope>().Update(AuditWorkScope);

                if (checkAuditWorkScope.Length < 2)
                {
                    checkAuditWorkScopeFacility.IsDeleted = true;
                    checkAuditWorkScopeFacility.DeletedAt = DateTime.Now;
                    checkAuditWorkScopeFacility.DeletedBy = userInfo.Id;
                    _uow.Repository<AuditWorkScopeFacility>().Update(checkAuditWorkScopeFacility);
                }
                return Ok(new { code = "1", id = AuditWorkScope.auditwork_id, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //Xóa table Đơn vị kiểm toán
        [HttpDelete("DeleteAuditWorkScopeFacility/{id}")]
        public IActionResult DeleteAuditWorkScopeFacility(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var AuditWorkScopeFacility = _uow.Repository<AuditWorkScopeFacility>().FirstOrDefault(a => a.Id == id && a.IsDeleted != true);
                if (AuditWorkScopeFacility == null)
                {
                    return NotFound();
                }
                var checkAuditProgram = _uow.Repository<AuditProgram>().FirstOrDefault(a => a.auditwork_id == AuditWorkScopeFacility.auditwork_id
                && a.auditfacilities_id == AuditWorkScopeFacility.auditfacilities_id
                && a.Year == AuditWorkScopeFacility.Year
                && a.IsDeleted != true);
                if (checkAuditProgram != null)
                {
                    return Ok(new { code = "417", msg = "fail" });
                }
                var AuditWorkScope = _uow.Repository<AuditWorkScope>().Find(a => a.Year == AuditWorkScopeFacility.Year
                && a.auditfacilities_id == AuditWorkScopeFacility.auditfacilities_id
                && a.IsDeleted != true
                && a.auditwork_id == AuditWorkScopeFacility.auditwork_id).ToArray();

                AuditWorkScopeFacility.IsDeleted = true;
                AuditWorkScopeFacility.DeletedAt = DateTime.Now;
                AuditWorkScopeFacility.DeletedBy = userInfo.Id;
                _uow.Repository<AuditWorkScopeFacility>().Update(AuditWorkScopeFacility);
                for (int i = 0; i < AuditWorkScope.Length; i++)
                {
                    AuditWorkScope[i].IsDeleted = true;
                    AuditWorkScope[i].DeletedAt = DateTime.Now;
                    AuditWorkScope[i].DeletedBy = userInfo.Id;
                    _uow.Repository<AuditWorkScope>().Update(AuditWorkScope[i]);
                }
                return Ok(new { code = "1", id = AuditWorkScopeFacility.auditwork_id, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //xóa Schedule tab 4
        [HttpDelete("DeleteSchedule/{id}")]
        public IActionResult DeleteSchedule(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var Schedule = _uow.Repository<Schedule>().FirstOrDefault(a => a.id == id);
                if (Schedule == null)
                {
                    return NotFound();
                }

                Schedule.is_deleted = true;
                Schedule.deleted_at = DateTime.Now;
                Schedule.deleted_by = userInfo.Id;
                _uow.Repository<Schedule>().Update(Schedule);
                return Ok(new { code = "1", id = Schedule.auditwork_id, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //chi tiết  AuditWork tab 5
        [HttpGet("AuditWorkDetailCollapse/{id}")]
        public IActionResult AuditWorkDetailCollapse(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkScopeDetail = _uow.Repository<AuditWorkScopeFacility>().Include(a => a.AuditStrategyRisk, a => a.AuditWorkScopeFacilityFile).FirstOrDefault(a => a.Id == id);
                var _auditstrategyrisk = _uow.Repository<AuditStrategyRisk>().Include(a => a.AuditWorkScopeFacility).Where(a => a.auditwork_scope_id == id && a.is_deleted != true).ToArray();
                if (checkAuditWorkScopeDetail != null)
                {
                    var auditstrategyrisk = new AuditStrategyRiskModel()
                    {
                        id = checkAuditWorkScopeDetail.Id,
                        //path = checkAuditWorkScopeDetail.path == null ? "" : checkAuditWorkScopeDetail.path,
                        brief_review = checkAuditWorkScopeDetail.brief_review == null ? "" : checkAuditWorkScopeDetail.brief_review,
                        ListAuditStrategyRisk = _auditstrategyrisk.Select(a => new ListAuditStrategyRisk
                        {
                            id = a.id,
                            auditwork_scope_id = a.auditwork_scope_id,
                            auditwork_scope_name = checkAuditWorkScopeDetail.auditfacilities_name,
                            name_risk = a.name_risk,
                            description = a.description,
                            risk_level = a.risk_level,
                            audit_strategy = a.audit_strategy,
                            is_deleted = a.is_deleted,
                            is_active = a.is_active,
                        }).ToList(),
                        ListFile = checkAuditWorkScopeDetail.AuditWorkScopeFacilityFile.Where(a => a.IsDelete != true).Select(x => new AuditWorkScopeFacilityFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = auditstrategyrisk });
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

        //thêm mới rủi ro của kế hoạch kiểm toán tab 5
        [HttpPost("CreateAuditStrategyRisk")]
        public IActionResult CreateAuditStrategyRisk([FromBody] AuditStrategyRiskCreateModel auditStrategyRiskCreateModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditStrategyRiskCreateModel = _uow.Repository<AuditStrategyRisk>().Find(a => a.is_deleted != true
                && a.auditwork_scope_id.Equals(auditStrategyRiskCreateModel.auditwork_scope_id)
                && a.name_risk.Equals(auditStrategyRiskCreateModel.name_risk)).ToArray();
                if (checkAuditStrategyRiskCreateModel.Length > 0)
                {
                    return Ok(new { code = "416", msg = "fail" });
                }
                var _AuditStrategyRisk = new AuditStrategyRisk
                {
                    auditwork_scope_id = auditStrategyRiskCreateModel.auditwork_scope_id,
                    name_risk = auditStrategyRiskCreateModel.name_risk,
                    description = auditStrategyRiskCreateModel.description,
                    risk_level = auditStrategyRiskCreateModel.risk_level,
                    audit_strategy = auditStrategyRiskCreateModel.audit_strategy,
                    is_deleted = false,
                    is_active = true,
                    created_at = DateTime.Now,
                    created_by = _userInfo.Id,
                };
                if (_AuditStrategyRisk.name_risk == "" || _AuditStrategyRisk.name_risk == null) { return Ok(new { code = "0", msg = "fail" }); }
                _uow.Repository<AuditStrategyRisk>().Add(_AuditStrategyRisk);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //xóa AuditStrategyRisk tab 5
        [HttpDelete("DeleteAuditStrategyRisk/{id}")]
        public IActionResult DeleteAuditStrategyRisk(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var _AuditStrategyRisk = _uow.Repository<AuditStrategyRisk>().FirstOrDefault(a => a.id == id);
                if (_AuditStrategyRisk == null)
                {
                    return NotFound();
                }

                _AuditStrategyRisk.is_deleted = true;
                _AuditStrategyRisk.deleted_at = DateTime.Now;
                _AuditStrategyRisk.deleted_by = userInfo.Id;
                _uow.Repository<AuditStrategyRisk>().Update(_AuditStrategyRisk);
                return Ok(new { code = "1", auditwork_scope_id = _AuditStrategyRisk.auditwork_scope_id, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("DetailAuditStrategyRisk/{id}")]
        public IActionResult DetailAuditStrategyRisk(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditStrategyRisk = _uow.Repository<AuditStrategyRisk>().FirstOrDefault(a => a.id == id && a.is_deleted.Equals(false));

                if (checkAuditStrategyRisk != null)
                {
                    var _auditstrategyrisk = new AuditStrategyRiskDetailModel()
                    {
                        id = checkAuditStrategyRisk.id,
                        auditwork_scope_id = checkAuditStrategyRisk.auditwork_scope_id,
                        name_risk = checkAuditStrategyRisk.name_risk,
                        description = checkAuditStrategyRisk.description,
                        risk_level = checkAuditStrategyRisk.risk_level,
                        audit_strategy = checkAuditStrategyRisk.audit_strategy,
                    };
                    return Ok(new { code = "1", msg = "success", data = _auditstrategyrisk });
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
        [HttpPut("EditAuditStrategyRisk")]
        public IActionResult EditAuditStrategyRisk([FromBody] AuditStrategyRiskEditModel auditStrategyRiskEditModel)
        {
            try
            {

                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditStrategyRisk = _uow.Repository<AuditStrategyRisk>().FirstOrDefault(a => a.id == auditStrategyRiskEditModel.id && a.is_deleted.Equals(false));
                if (checkAuditStrategyRisk == null) { return NotFound(); }
                if (auditStrategyRiskEditModel.name_risk == "" || auditStrategyRiskEditModel.name_risk == null) { return Ok(new { code = "0", msg = "fail" }); }
                var checkNameAuditStrategyRisk = _uow.Repository<AuditStrategyRisk>().FirstOrDefault(a => a.name_risk == auditStrategyRiskEditModel.name_risk && a.is_deleted != true
                && a.auditwork_scope_id.Equals(auditStrategyRiskEditModel.auditwork_scope_id));
                if (checkAuditStrategyRisk.id != (checkNameAuditStrategyRisk != null ? checkNameAuditStrategyRisk.id : null)
                    && checkNameAuditStrategyRisk != null) { return Ok(new { code = "416", msg = "fail" }); }

                checkAuditStrategyRisk.auditwork_scope_id = auditStrategyRiskEditModel.auditwork_scope_id;
                checkAuditStrategyRisk.name_risk = auditStrategyRiskEditModel.name_risk;
                checkAuditStrategyRisk.description = auditStrategyRiskEditModel.description;
                checkAuditStrategyRisk.risk_level = auditStrategyRiskEditModel.risk_level;
                checkAuditStrategyRisk.audit_strategy = auditStrategyRiskEditModel.audit_strategy;
                checkAuditStrategyRisk.Modified_at = DateTime.Now;
                checkAuditStrategyRisk.modified_by = _userInfo.Id;
                _uow.Repository<AuditStrategyRisk>().Update(checkAuditStrategyRisk);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
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
        protected string CreateUploadURLPlan(IFormFile imageFile, string folder = "")
        {
            var pathSave = "";
            var pathconfig = _config["Upload:ApprovalOutSideDocPath"];
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
        [HttpPost("UploadFile")]
        public IActionResult UploadFile()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }



                var _upfiledata = new UploadFileModel();
                var data = Request.Form["data"];
                //var pathSave = "";
                //var file_type = "";
                if (!string.IsNullOrEmpty(data))
                {
                    _upfiledata = JsonSerializer.Deserialize<UploadFileModel>(data);
                    //var file = Request.Form.Files;
                    //file_type = file.FirstOrDefault()?.ContentType;
                    //pathSave = CreateUploadFile(file, "AuditWorkScope");
                }
                else
                {
                    return BadRequest();
                }
                var checkworkscopefacility = _uow.Repository<AuditWorkScopeFacility>().FirstOrDefault(a => a.Id == _upfiledata.id);
                if (checkworkscopefacility == null)
                {
                    return NotFound();
                }
                checkworkscopefacility.Id = (int)_upfiledata.id;
                checkworkscopefacility.brief_review = _upfiledata.brief_review == null ? "" : _upfiledata.brief_review;
                //checkworkscopefacility.path = pathSave != "" ? pathSave : checkworkscopefacility.path;
                //checkworkscopefacility.FileType = file_type != null ? file_type : checkworkscopefacility.FileType;
                _uow.Repository<AuditWorkScopeFacility>().Update(checkworkscopefacility);


                var file = Request.Form.Files;
                var _listFile = new List<AuditWorkScopeFacilityFile>();
                foreach (var item in file)
                {
                    var file_type = item.ContentType;
                    var pathSave = CreateUploadURL(item, "AuditWorkScopeFacility");
                    var audit_work_scope_facility_file = new AuditWorkScopeFacilityFile()
                    {
                        AuditWorkScopeFacility = checkworkscopefacility,
                        IsDelete = false,
                        FileType = file_type,
                        Path = pathSave,
                    };
                    _listFile.Add(audit_work_scope_facility_file);
                }
                foreach (var item in _listFile)
                {
                    _uow.Repository<AuditWorkScopeFacilityFile>().AddWithoutSave(item);
                }
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //xóa kế hoạch cuộc kiểm toán
        [HttpDelete("DeleteAuditWork/{id}")]
        public IActionResult DeleteAuditWork(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var _AuditWork = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == id);
                if (_AuditWork == null)
                {
                    return NotFound();
                }
                var checkAuditWorkScope = _uow.Repository<AuditWorkScope>().Find(a => a.auditwork_id == id && a.IsDeleted != true).ToArray();
                var _checkauditworkscope = checkAuditWorkScope.Select(a => a.Id).ToList();
                for (int i = 0; i < _checkauditworkscope.Count(); i++)
                {
                    var _auditworkscope = _uow.Repository<AuditWorkScope>().FirstOrDefault(a => a.Id == _checkauditworkscope[i]);
                    var checkAuditProgram = _uow.Repository<AuditProgram>().FirstOrDefault(a => a.auditwork_id == id
                    && a.auditfacilities_id == _auditworkscope.auditfacilities_id
                    && a.Year == _auditworkscope.Year
                    && a.IsDeleted != true);
                    if (checkAuditProgram != null)
                    {
                        return Ok(new { code = "417", msg = "fail" });
                    }
                }
                //var checkAuditWorkScopeFacility = _uow.Repository<AuditWorkScopeFacility>().Find(a => a.auditwork_id == id && a.IsDeleted != true).ToArray();
                //var _checkauditworkscopefacility = checkAuditWorkScopeFacility.Select(a => a.Id).ToList();
                //for (int i = 0; i < _checkauditworkscopefacility.Count(); i++)
                //{
                //    var _auditworkscopefacility = _uow.Repository<AuditWorkScopeFacility>().FirstOrDefault(a => a.Id == _checkauditworkscope[i]);
                //    var _checkAuditProgram = _uow.Repository<AuditProgram>().FirstOrDefault(a => a.auditwork_id == id
                //    && a.auditfacilities_id == _auditworkscopefacility.auditfacilities_id
                //    && a.Year == _auditworkscopefacility.Year
                //    && a.IsDeleted != true);
                //    if (_checkAuditProgram != null)
                //    {
                //        return Ok(new { code = "417", msg = "fail" });
                //    }
                //}

                //if (_AuditWork.Status == 3)
                //{
                //    return Ok(new { code = "0", msg = "success" });
                //}

                //if (_AuditWork.CreatedBy != userInfo.Id)
                //{
                //    return Ok(new { code = "403", msg = "Người dùng này không có quyền xóa dữ liệu" });
                //}
                _AuditWork.IsDeleted = true;
                _AuditWork.DeletedAt = DateTime.Now;
                _AuditWork.DeletedBy = userInfo.Id;
                _uow.Repository<AuditWork>().Update(_AuditWork);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //lưu auditworkscope khi chọn phạm vi
        [HttpPost("CreateAuditWorkScope")]
        public IActionResult CreateAuditWorkScope([FromBody] CreateAuditWorkScopeModel createAuditWorkScopeModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkScope = _uow.Repository<AuditWorkScope>().FirstOrDefault(a => a.IsDeleted != true
                && a.auditwork_id.Equals(createAuditWorkScopeModel.auditwork_id)
                && a.auditprocess_id.Equals(createAuditWorkScopeModel.auditprocess_id)
                && a.bussinessactivities_id.Equals(createAuditWorkScopeModel.bussinessactivities_id)
                && a.auditfacilities_id.Equals(createAuditWorkScopeModel.auditfacilities_id));
                if (checkAuditWorkScope != null)
                {
                    checkAuditWorkScope.auditwork_id = createAuditWorkScopeModel.auditwork_id;
                    checkAuditWorkScope.Year = createAuditWorkScopeModel.year;
                    checkAuditWorkScope.BoardId = createAuditWorkScopeModel.score_board_id;
                    checkAuditWorkScope.auditprocess_id = createAuditWorkScopeModel.auditprocess_id;
                    checkAuditWorkScope.bussinessactivities_id = createAuditWorkScopeModel.bussinessactivities_id;
                    checkAuditWorkScope.auditfacilities_id = createAuditWorkScopeModel.auditfacilities_id;
                    checkAuditWorkScope.auditprocess_name = createAuditWorkScopeModel.auditprocess_name;
                    checkAuditWorkScope.auditfacilities_name = createAuditWorkScopeModel.auditfacilities_name;
                    checkAuditWorkScope.bussinessactivities_name = createAuditWorkScopeModel.bussinessactivities_name;
                    checkAuditWorkScope.ReasonNote = createAuditWorkScopeModel.reason;
                    //checkAuditWorkScope.RiskRatingName = createAuditWorkScopeModel.risk_rating_name;
                    checkAuditWorkScope.RiskRatingName = createAuditWorkScopeModel.risk_level;
                    checkAuditWorkScope.RiskRating = (createAuditWorkScopeModel.risk_level == "Cao" ? 1 : createAuditWorkScopeModel.risk_level == "Trung bình" ? 2 : createAuditWorkScopeModel.risk_level == "Thấp" ? 3 : null);
                    checkAuditWorkScope.IsDeleted = false;
                    _uow.Repository<AuditWorkScope>().Update(checkAuditWorkScope);
                    return Ok(new { code = "1", msg = "success" });
                }
                else
                {
                    var _auditworkscope = new AuditWorkScope
                    {
                        auditwork_id = createAuditWorkScopeModel.auditwork_id,
                        Year = createAuditWorkScopeModel.year,
                        BoardId = createAuditWorkScopeModel.score_board_id,
                        auditprocess_id = createAuditWorkScopeModel.auditprocess_id,
                        bussinessactivities_id = createAuditWorkScopeModel.bussinessactivities_id,
                        auditfacilities_id = createAuditWorkScopeModel.auditfacilities_id,
                        auditprocess_name = createAuditWorkScopeModel.auditprocess_name,
                        auditfacilities_name = createAuditWorkScopeModel.auditfacilities_name,
                        bussinessactivities_name = createAuditWorkScopeModel.bussinessactivities_name,
                        ReasonNote = createAuditWorkScopeModel.reason,
                        //RiskRatingName = createAuditWorkScopeModel.risk_rating_name,
                        RiskRatingName = createAuditWorkScopeModel.risk_level,
                        RiskRating = (createAuditWorkScopeModel.risk_level == "Cao" ? 1 : createAuditWorkScopeModel.risk_level == "Trung bình" ? 2 : createAuditWorkScopeModel.risk_level == "Thấp" ? 3 : null),
                        IsDeleted = false,
                    };
                    _uow.Repository<AuditWorkScope>().Add(_auditworkscope);
                    return Ok(new { code = "1", msg = "success" });
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //lưu auditworkscopefacility khi chọn phạm vi
        [HttpPost("CreateAuditWorkScopeFacility")]
        public IActionResult CreateAuditWorkScopeFacility([FromBody] CreateAuditWorkScopeFacilityModel createAuditWorkScopeFacilityModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkScopeFacility = _uow.Repository<AuditWorkScopeFacility>().FirstOrDefault(a => a.IsDeleted != true
                && a.auditwork_id.Equals(createAuditWorkScopeFacilityModel.auditwork_id)
                && a.auditfacilities_id.Equals(createAuditWorkScopeFacilityModel.auditfacilities_id));
                if (checkAuditWorkScopeFacility != null)
                {
                    checkAuditWorkScopeFacility.auditwork_id = createAuditWorkScopeFacilityModel.auditwork_id;
                    checkAuditWorkScopeFacility.Year = createAuditWorkScopeFacilityModel.year;
                    checkAuditWorkScopeFacility.BoardId = createAuditWorkScopeFacilityModel.score_board_id;
                    checkAuditWorkScopeFacility.auditfacilities_id = createAuditWorkScopeFacilityModel.auditfacilities_id;
                    checkAuditWorkScopeFacility.auditfacilities_name = createAuditWorkScopeFacilityModel.auditfacilities_name;
                    checkAuditWorkScopeFacility.ReasonNote = createAuditWorkScopeFacilityModel.reason;
                    checkAuditWorkScopeFacility.AuditingTimeNearest = createAuditWorkScopeFacilityModel.last_audit_time;
                    checkAuditWorkScopeFacility.RiskRatingName = createAuditWorkScopeFacilityModel.risk_rating_name;
                    checkAuditWorkScopeFacility.RiskRating = (createAuditWorkScopeFacilityModel.risk_rating_name == "Cao" ? 1 : createAuditWorkScopeFacilityModel.risk_rating_name == "Trung bình" ? 2 : createAuditWorkScopeFacilityModel.risk_rating_name == "Thấp" ? 3 : null);
                    checkAuditWorkScopeFacility.IsDeleted = false;
                    _uow.Repository<AuditWorkScopeFacility>().Update(checkAuditWorkScopeFacility);
                    return Ok(new { code = "1", msg = "success" });
                }
                else
                {
                    var _checkAuditWorkScopeFacility = new AuditWorkScopeFacility
                    {
                        auditwork_id = createAuditWorkScopeFacilityModel.auditwork_id,
                        Year = createAuditWorkScopeFacilityModel.year,
                        BoardId = createAuditWorkScopeFacilityModel.score_board_id,
                        auditfacilities_id = createAuditWorkScopeFacilityModel.auditfacilities_id,
                        auditfacilities_name = createAuditWorkScopeFacilityModel.auditfacilities_name,
                        ReasonNote = createAuditWorkScopeFacilityModel.reason,
                        AuditingTimeNearest = createAuditWorkScopeFacilityModel.last_audit_time,
                        RiskRatingName = createAuditWorkScopeFacilityModel.risk_rating_name,
                        RiskRating = (createAuditWorkScopeFacilityModel.risk_rating_name == "Cao" ? 1 : createAuditWorkScopeFacilityModel.risk_rating_name == "Trung bình" ? 2 : createAuditWorkScopeFacilityModel.risk_rating_name == "Thấp" ? 3 : null),
                        IsDeleted = false,
                    };
                    _uow.Repository<AuditWorkScopeFacility>().Add(_checkAuditWorkScopeFacility);
                    return Ok(new { code = "1", msg = "success" });
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("SelectYearApproved")]//list •năm đã duyệt ở cuộc KT năm
        public IActionResult SelectYearApproved(string q)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_AP" && a.StatusCode == "3.1").ToArray();
                var audiplan_id = approval_status.Select(a => a.item_id).ToList();
                string KeyWord = q;
                Expression<Func<AuditPlan, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Year == (Int32.Parse(q))) && c.IsDelete != true && audiplan_id.Contains(c.Id);
                var list_yearapproved = _uow.Repository<AuditPlan>().Find(filter).OrderByDescending(a => a.Year);
                IEnumerable<AuditPlan> data = list_yearapproved;
                var dt = data.Select(a => new DropListYearApprovedModel()
                {
                    id = a.Id,
                    year = a.Year.ToString(),
                });
                return Ok(new { code = "1", msg = "success", data = dt });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new DropListYearApprovedModel() });
            }
        }
        [HttpGet("SelectNameAuditWork")]//list •Tên cuộc kiểm toán
        public IActionResult SelectNameAuditWork(string q, string year)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_AP" && a.StatusCode == "3.1").ToArray();
                var audiplan_id = approval_status.Select(a => a.item_id).ToList();
                var checkAuditPlan = _uow.Repository<AuditPlan>().FirstOrDefault(c => c.Year == (Int32.Parse(year)) && audiplan_id.Contains(c.Id) && c.IsDelete.Equals(false));

                string KeyWord = q;
                Expression<Func<AuditWorkPlan, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Name.ToLower().Contains(q.ToLower()) || c.Name.ToLower().Contains(q.ToLower()))
                && c.IsActive == true && c.IsDeleted != true && c.Year == year && c.auditplan_id == checkAuditPlan.Id;
                var listnameauditwork = _uow.Repository<AuditWorkPlan>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditWorkPlan> data = listnameauditwork;

                //var checkAuditPlan = _uow.Repository<AuditPlan>().FirstOrDefault(c => c.Year == (Int32.Parse(year)) && c.Status.Equals(3) && c.IsDelete.Equals(false));
                //var listnameauditwork = _uow.Repository<AuditWorkPlan>().GetAll(a => a.IsDeleted != true && a.Year == year && a.auditplan_id == checkAuditPlan.Id /*&& a.Status.Equals(1)*/);
                //IEnumerable<AuditWorkPlan> data = listnameauditwork;
                var dt = data.Select(a => new DropListNameAuditWorkModel()
                {
                    id = a.Id,
                    name = a.Name,
                });
                var lst = dt.GroupBy(a => a.name).Select(x => x.FirstOrDefault()).ToList();
                return Ok(new { code = "1", msg = "success", data = lst, total = lst.Count() });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new DropListNameAuditWorkModel() });
            }
        }
        [HttpPost("CreateAuditWork")]// thêm mới cuộc kiểm toán
        public IActionResult CreateAuditWork([FromBody] CreateAuditWorkModel createauditworkmodel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_AP" && a.StatusCode == "3.1").ToArray();
                var audiplan_id = approval_status.Select(a => a.item_id).ToList();

                var _allAuditWork = _uow.Repository<AuditWork>().Find(a => a.IsDeleted != true && a.Year == createauditworkmodel.year && a.Name == createauditworkmodel.name).ToArray();
                if (_allAuditWork.Length > 0)
                {
                    return Ok(new { code = "416", msg = "fail" });
                }
                //check AuditPlan
                var checkauditplan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.IsDelete != true
                 && audiplan_id.Contains(a.Id)
                && a.Year == Int32.Parse(createauditworkmodel.year));
                //check AuditWorkPlan
                var checkauditwork = _uow.Repository<AuditWorkPlan>().FirstOrDefault(a => a.IsDeleted != true
                //&& a.Status.Equals(1)
                && a.auditplan_id == (checkauditplan != null ? checkauditplan.Id : null)
                && a.Name == createauditworkmodel.name);
                //check AuditAssignmentPlan
                var checkauditassignmentplan = _uow.Repository<AuditAssignmentPlan>().Find(a => a.IsDeleted != true
                && a.auditwork_id == (checkauditwork != null ? checkauditwork.Id : null)).ToArray();
                //check AuditWorkScopePlan
                var checkauditworkscopeplan = _uow.Repository<AuditWorkScopePlan>().Find(a => a.IsDeleted != true
                && a.auditwork_id == (checkauditwork != null ? checkauditwork.Id : null)
                && a.Year == Int32.Parse(createauditworkmodel.year)).ToArray();
                //check AuditWorkScopePlanFacility
                var checkAuditWorkScopePlanFacility = _uow.Repository<AuditWorkScopePlanFacility>().Find(a => a.IsDeleted != true
                && a.auditwork_id == (checkauditwork != null ? checkauditwork.Id : null)).ToArray();

                var auditwork = new AuditWork
                {
                    Name = (createauditworkmodel.classify == 1 ? checkauditwork.Name : createauditworkmodel.name),
                    Target = (createauditworkmodel.classify == 1 ? checkauditwork.Target : null),
                    StartDate = (createauditworkmodel.classify == 1 ? checkauditwork.StartDate : null),
                    EndDate = (createauditworkmodel.classify == 1 ? checkauditwork.EndDate : null),
                    NumOfWorkdays = (createauditworkmodel.classify == 1 ? checkauditwork.NumOfWorkdays : null),
                    person_in_charge = (createauditworkmodel.classify == 1 ? checkauditwork.person_in_charge : null),
                    NumOfAuditor = (createauditworkmodel.classify == 1 ? checkauditwork.NumOfAuditor : null),
                    ReqSkillForAudit = (createauditworkmodel.classify == 1 ? checkauditwork.ReqSkillForAudit : null),
                    ReqOutsourcing = (createauditworkmodel.classify == 1 ? checkauditwork.ReqOutsourcing : null),
                    ReqOther = (createauditworkmodel.classify == 1 ? checkauditwork.ReqOther : null),
                    ScaleOfAudit = (createauditworkmodel.classify == 1 ? checkauditwork.ScaleOfAudit : null),
                    AuditScope = (createauditworkmodel.classify == 1 ? checkauditwork.AuditScope : null),
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    //ModifiedAt = DateTime.Now,
                    CreatedBy = _userInfo.Id,
                    //ModifiedAt = null,
                    //ModifiedBy = null,
                    //DeletedAt = null,
                    //DeletedBy = null,
                    Path = (createauditworkmodel.classify == 1 ? checkauditwork.Path : null),
                    auditplan_id = (createauditworkmodel.classify == 1 ? checkauditwork.auditplan_id : null),
                    //Status = 1,
                    Classify = createauditworkmodel.classify,
                    Year = createauditworkmodel.year,
                    ExecutionStatusStr = "Chưa thực hiện",
                    AuditScopeOutside = (createauditworkmodel.classify == 1 ? checkauditwork.AuditScopeOutside : null),
                    AuditWorkScope = null,
                    AuditAssignment = null,
                };
                auditwork.Code = GetCodeAuditWork(createauditworkmodel.year);

                _uow.Repository<AuditWork>().Add(auditwork);
                if (checkauditassignmentplan != null)
                {
                    for (int i = 0; i < checkauditassignmentplan.Count(); i++)
                    {
                        var checkAuditAssignment = _uow.Repository<AuditAssignment>().Find(z => z.auditwork_id.Equals(checkauditassignmentplan[i].auditwork_id)
                        && z.user_id.Equals(checkauditassignmentplan[i].user_id)
                        && z.IsDeleted.Equals(true));
                        if (checkAuditAssignment.Any())
                        {
                            _uow.Repository<AuditAssignment>().Delete(checkAuditAssignment);
                            _uow.SaveChanges();
                        }
                        var auditAssignment = new AuditAssignment
                        {
                            user_id = checkauditassignmentplan[i].user_id,
                            auditwork_id = auditwork.Id,
                            fullName = checkauditassignmentplan[i].fullName,
                            StartDate = checkauditassignmentplan[i].StartDate,
                            EndDate = checkauditassignmentplan[i].EndDate,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.Now,
                            CreatedBy = _userInfo.Id,
                        };
                        _uow.Repository<AuditAssignment>().Add(auditAssignment);
                        _uow.SaveChanges();
                    }
                }
                if (checkauditworkscopeplan != null)
                {
                    for (int i = 0; i < checkauditworkscopeplan.Count(); i++)
                    {
                        var checkAuditWorkScope = _uow.Repository<AuditWorkScope>().Find(z => z.auditwork_id.Equals(checkauditworkscopeplan[i].auditwork_id) && z.IsDeleted.Equals(true));
                        if (checkAuditWorkScope.Any())
                        {
                            _uow.Repository<AuditWorkScope>().Delete(checkAuditWorkScope);
                            _uow.SaveChanges();
                        }
                        var auditworkscope = new AuditWorkScope
                        {
                            auditwork_id = auditwork.Id,
                            auditprocess_id = checkauditworkscopeplan[i].auditprocess_id,
                            bussinessactivities_id = checkauditworkscopeplan[i].bussinessactivities_id,
                            auditfacilities_id = checkauditworkscopeplan[i].auditfacilities_id,
                            ReasonNote = checkauditworkscopeplan[i].ReasonNote,
                            RiskRating = checkauditworkscopeplan[i].RiskRating,
                            RiskRatingName = checkauditworkscopeplan[i].RiskRatingName,
                            AuditingTimeNearest = checkauditworkscopeplan[i].AuditingTimeNearest,
                            IsDeleted = false,
                            DeletedAt = null,
                            DeletedBy = null,
                            auditfacilities_name = checkauditworkscopeplan[i].auditfacilities_name,
                            auditprocess_name = checkauditworkscopeplan[i].auditprocess_name,
                            bussinessactivities_name = checkauditworkscopeplan[i].bussinessactivities_name,
                            Year = checkauditworkscopeplan[i].Year,
                            AuditRatingLevelReport = checkauditworkscopeplan[i].AuditRatingLevelReport,
                            BaseRatingReport = checkauditworkscopeplan[i].BaseRatingReport,
                            brief_review = checkauditworkscopeplan[i].brief_review == null ? "" : checkauditworkscopeplan[i].brief_review/* : null*/,
                            path = checkauditworkscopeplan[i].path == null ? "" : checkauditworkscopeplan[i].path/* : null*/,
                            BoardId = checkauditworkscopeplan[i].BoardId,
                            AuditWorkScopeUserMapping = null,
                        };
                        _uow.Repository<AuditWorkScope>().Add(auditworkscope);
                        _uow.SaveChanges();
                    }
                }
                if (checkAuditWorkScopePlanFacility != null)
                {
                    for (int i = 0; i < checkAuditWorkScopePlanFacility.Count(); i++)
                    {
                        var checkAuditWorkScopeFacility = _uow.Repository<AuditWorkScopeFacility>().Find(z => z.auditwork_id.Equals(checkAuditWorkScopePlanFacility[i].auditwork_id) && z.IsDeleted.Equals(true));
                        if (checkAuditWorkScopeFacility.Any())
                        {
                            _uow.Repository<AuditWorkScopeFacility>().Delete(checkAuditWorkScopeFacility);
                            _uow.SaveChanges();
                        }
                        var auditworkscopefacility = new AuditWorkScopeFacility
                        {
                            auditwork_id = auditwork.Id,
                            Year = checkAuditWorkScopePlanFacility[i].Year,
                            auditfacilities_id = checkAuditWorkScopePlanFacility[i].auditfacilities_id,
                            auditfacilities_name = checkAuditWorkScopePlanFacility[i].auditfacilities_name,
                            ReasonNote = checkAuditWorkScopePlanFacility[i].ReasonNote,
                            RiskRating = checkAuditWorkScopePlanFacility[i].RiskRating,
                            RiskRatingName = checkAuditWorkScopePlanFacility[i].RiskRatingName,
                            AuditingTimeNearest = checkAuditWorkScopePlanFacility[i].AuditingTimeNearest,
                            AuditRatingLevelReport = checkAuditWorkScopePlanFacility[i].AuditRatingLevelReport,
                            BaseRatingReport = checkAuditWorkScopePlanFacility[i].BaseRatingReport,
                            DeletedAt = checkAuditWorkScopePlanFacility[i].DeletedAt,
                            IsDeleted = checkAuditWorkScopePlanFacility[i].IsDeleted,
                            DeletedBy = checkAuditWorkScopePlanFacility[i].DeletedBy,
                            BoardId = checkAuditWorkScopePlanFacility[i].BoardId,
                            AuditStrategyRisk = null,
                        };
                        _uow.Repository<AuditWorkScopeFacility>().Add(auditworkscopefacility);
                        _uow.SaveChanges();
                    }
                }
                return Ok(new { code = "1", idAuditWork = auditwork.Id, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //tự sinh mã AuditWork
        public static string GetCodeAuditWork(string year)
        {
            string EID = string.Empty;
            try
            {
                var yearnow = year;
                string str_start = yearnow + ".CKT";
                using (var context = new KitanoSqlContext())
                {
                    var list = context.AuditWork.Where(a => a.IsDeleted != true).ToArray();
                    var data = list.Where(a => !string.IsNullOrEmpty(a.Code) && a.Code.StartsWith(str_start));
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
        [HttpGet("SelectUserType")]
        public IActionResult SelectUserType(string q)//list •Tên người dùng có loại là "kiểm toán nội bộ"
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = q;
                Expression<Func<Users, bool>> filter = c => (string.IsNullOrEmpty(q)
                || c.FullName.ToLower().Contains(q.ToLower())
                || c.UserName.ToLower().Contains(q.ToLower()))
                && c.IsActive == true && c.IsDeleted != true && c.UsersType.Equals(1);
                var list_user = _uow.Repository<Users>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<Users> data = list_user;
                var user = data.Select(a => new DropListUserTypeModel()
                {
                    id = a.Id,
                    name = a.FullName,
                });
                return Ok(new { code = "1", msg = "success", data = user });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new DropListUserTypeModel() });
            }
        }
        //[HttpPost("SendBrowseAuditWork/{id}")]//gửi duyệt
        //public IActionResult SendBrowseAuditWork(int id, int? user_id)//gửi duyệt cuộc kiểm toán"
        //{
        //    try
        //    {
        //        if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
        //        {
        //            return Unauthorized();
        //        }
        //        var checkAuditWork = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == id && a.flag_user_id == null && a.IsDeleted != true && a.IsActive == true);
        //        if (checkAuditWork == null)
        //        {
        //            return NotFound();
        //        }
        //        if (checkAuditWork.Status != 1 && checkAuditWork.Status != 4)
        //        {
        //            return Ok(new { code = "403", msg = "Cuộc kiểm toán này không thuộc trường hợp gửi duyệt" });
        //        }
        //        checkAuditWork.Status = 2;
        //        checkAuditWork.flag_user_id = user_id;
        //        _uow.Repository<AuditWork>().Update(checkAuditWork);
        //        return Ok(new { code = "1", msg = "success" });
        //    }
        //    catch (Exception)
        //    {
        //        return Ok(new { code = "0", msg = "fail", data = "" });
        //    }
        //}
        [AllowAnonymous]
        [HttpGet("DownloadAttach")]
        public IActionResult DonwloadFile(int id)
        {
            try
            {
                var self = _uow.Repository<AuditPlan>().FirstOrDefault(o => o.Id == id);
                if (self == null)
                {
                    return NotFound();
                }
                var fullPath = Path.Combine(_config["Upload:AuditDocsPath"], self.Path);
                var name = "DownLoadFile";
                if (!string.IsNullOrEmpty(self.Path))
                {
                    var _array = self.Path.Split("\\");
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
        //[HttpPost("CensorshipAuditWork/{id}")]//Kiểm duyệt
        //public IActionResult CensorshipAuditWork(int id, int? param)
        //{
        //    try
        //    {
        //        if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
        //        {
        //            return Unauthorized();
        //        }
        //        var checkAuditWork = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == id && a.flag_user_id != null && a.IsDeleted != true && a.IsActive == true);
        //        if (checkAuditWork == null)
        //        {
        //            return NotFound();
        //        }
        //        if (userInfo.Id != checkAuditWork.flag_user_id)
        //        {
        //            return Ok(new { code = "403", msg = "Người dùng này không được phép thao tác kiểm duyệt với cuộc kiểm toán này" });
        //        }
        //        if (checkAuditWork.Status != 2)
        //        {
        //            return Ok(new { code = "403", msg = "Cuộc kiểm toán này không thuộc trường hợp kiểm duyệt" });
        //        }
        //        checkAuditWork.Status = (param == 1 ? 3 : 4);
        //        checkAuditWork.flag_user_id = (param != 1 ? null : checkAuditWork.flag_user_id);
        //        _uow.Repository<AuditWork>().Update(checkAuditWork);
        //        return Ok(new { code = "1", msg = "success", status = checkAuditWork.Status });
        //    }
        //    catch (Exception)
        //    {
        //        return Ok(new { code = "0", msg = "fail", data = "" });
        //    }
        //}
        [AllowAnonymous]
        [HttpGet("DownloadFileAuditWorkScope")]
        public IActionResult DownloadFileAuditWorkScope(int id)
        {
            try
            {
                var self = _uow.Repository<AuditWorkScopeFacilityFile>().FirstOrDefault(o => o.id == id);
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
        [HttpPost("ExportExcelTest/{id}")]
        public IActionResult ExportTest(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == id && a.IsDelete != true);
                if (auditplan == null)
                {
                    return NotFound();
                }
                var _auditwork_id = _uow.Repository<AuditWorkPlan>().Find(a => a.auditplan_id == id && a.IsDeleted != true).Select(a => a.Id).ToArray();
                var _user = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsActive == true).ToArray();
                var audit_work_scope = _uow.Repository<AuditWorkScopePlan>().Include(a => a.AuditWorkPlan).Where(a => a.auditprocess_id.HasValue && a.auditwork_id.HasValue && _auditwork_id.Contains(a.auditwork_id.Value) && a.IsDeleted != true).AsEnumerable().GroupBy(a => a.auditwork_id);
                var auditwork_assign = _uow.Repository<AuditAssignment>().Find(a => a.auditwork_id.HasValue && _auditwork_id.Contains(a.auditwork_id.Value) && a.IsDeleted != true).ToArray();
                var _lstauditwork = audit_work_scope.Select(a => new AuditWorkExportModel
                {
                    Id = a.Key,
                    Name = a.FirstOrDefault()?.AuditWorkPlan?.Name,
                    Code = a.FirstOrDefault()?.AuditWorkPlan?.Code,
                    StartDate = a.FirstOrDefault()?.AuditWorkPlan?.StartDate,
                    EndDate = a.FirstOrDefault()?.AuditWorkPlan?.EndDate,
                    PersonInCharge = a.FirstOrDefault().AuditWorkPlan.person_in_charge.HasValue ? _user.FirstOrDefault(u => u.Id == a.FirstOrDefault()?.AuditWorkPlan.person_in_charge)?.FullName : "",
                    AuditFacility = string.Join(", ", a.Select(x => x.auditfacilities_name).Distinct()),
                    AuditProcess = string.Join(", ", a.Select(x => x.auditprocess_name).Distinct()),
                    reason = string.Join(", ", a.Select(x => x.ReasonNote).Distinct()),
                    NumOfAuditor = a.FirstOrDefault()?.AuditWorkPlan?.NumOfAuditor,
                    NumOfWorkdays = a.FirstOrDefault()?.AuditWorkPlan?.NumOfWorkdays,
                    ReqOther = a.FirstOrDefault()?.AuditWorkPlan?.ReqOther,
                    ReqOutsourcing = a.FirstOrDefault()?.AuditWorkPlan?.ReqOutsourcing,
                    ReqSkillForAudit = a.FirstOrDefault()?.AuditWorkPlan?.ReqSkillForAudit,
                    ScaleOfAudit = a.FirstOrDefault()?.AuditWorkPlan?.ScaleOfAudit,
                    ListAssign = string.Join(",", auditwork_assign.Where(c => c.auditwork_id == a.Key).Select(c => c.Users.FullName).Distinct()),
                    risk_rating = string.Join("", a.Select(x => x.auditfacilities_name + ":" + x.RiskRatingName + Environment.NewLine).Distinct()),
                    auditing_time_nearest = string.Join("", a.Select(x => x.auditfacilities_name + (x.AuditingTimeNearest.HasValue ? ":" + x.AuditingTimeNearest.Value.Year : "") + Environment.NewLine).Distinct()),

                });
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Ke_hoach_kiem_toan_nam.xlsx");
                var template = new FileInfo(fullPath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Sheet1"];
                    ExcelRange cellHeader = worksheet.Cells[2, 1];
                    cellHeader.Value = "KẾ HOẠCH KIỂM TOÁN NĂM " + auditplan.Year;
                    cellHeader.Style.Font.Size = 11;
                    cellHeader.Style.Font.Bold = true;
                    var _startrow = 6;
                    var startrow = 6;
                    var startcol = 1;
                    var count = 0;

                    foreach (var item in _lstauditwork)
                    {
                        count++;
                        ExcelRange cellNo = worksheet.Cells[startrow, startcol];
                        cellNo.Value = count;

                        ExcelRange cellCode = worksheet.Cells[startrow, startcol + 1];
                        cellCode.Value = item.Code;
                        ExcelRange cellName = worksheet.Cells[startrow, startcol + 2];
                        cellName.Value = item.Name;
                        ExcelRange cellReason = worksheet.Cells[startrow, startcol + 3];
                        cellReason.Value = item.reason;
                        ExcelRange cellAuditFacility = worksheet.Cells[startrow, startcol + 4];
                        cellAuditFacility.Value = item.AuditFacility;
                        ExcelRange cellRiskrating = worksheet.Cells[startrow, startcol + 5];
                        cellRiskrating.Value = item.risk_rating;
                        ExcelRange cellLastestAudit = worksheet.Cells[startrow, startcol + 6];
                        cellLastestAudit.Value = item.auditing_time_nearest;
                        ExcelRange cellAuditProcess = worksheet.Cells[startrow, startcol + 7];
                        cellAuditProcess.Value = item.AuditProcess;
                        var _scaleOfAudit = "";
                        switch (item.ScaleOfAudit)
                        {
                            case 0:
                                _scaleOfAudit = "Rất lớn";
                                break;
                            case 1:
                                _scaleOfAudit = "Lớn";
                                break;
                            case 2:
                                _scaleOfAudit = "Trung bình";
                                break;
                            case 3:
                                _scaleOfAudit = "Nhỏ";
                                break;
                        }
                        ExcelRange cellScaleOfAudit = worksheet.Cells[startrow, startcol + 8];
                        cellScaleOfAudit.Value = _scaleOfAudit;
                        ExcelRange cellStartDate = worksheet.Cells[startrow, startcol + 9];
                        cellStartDate.Value = item.StartDate.HasValue ? item.StartDate.Value.ToString("dd/MM/yyyy") : "";
                        ExcelRange cellEnddate = worksheet.Cells[startrow, startcol + 10];
                        cellEnddate.Value = item.EndDate.HasValue ? item.EndDate.Value.ToString("dd/MM/yyyy") : "";
                        ExcelRange cellNumOfAuditor = worksheet.Cells[startrow, startcol + 11];
                        cellNumOfAuditor.Value = item.NumOfAuditor;
                        ExcelRange cellNumOfWorkdays = worksheet.Cells[startrow, startcol + 12];
                        cellNumOfWorkdays.Value = item.NumOfWorkdays;
                        ExcelRange cellPersonInCharge = worksheet.Cells[startrow, startcol + 13];
                        cellPersonInCharge.Value = item.PersonInCharge;
                        ExcelRange cellListAssign = worksheet.Cells[startrow, startcol + 14];
                        cellListAssign.Value = item.ListAssign;
                        ExcelRange cellReqSkillForAudit = worksheet.Cells[startrow, startcol + 15];
                        cellReqSkillForAudit.Value = item.ReqSkillForAudit;
                        ExcelRange cellReqOutsourcing = worksheet.Cells[startrow, startcol + 16];
                        cellReqOutsourcing.Value = item.ReqOutsourcing;
                        ExcelRange cellReqOther = worksheet.Cells[startrow, startcol + 17];
                        cellReqOther.Value = item.ReqOther;

                        startrow++;
                    }
                    using ExcelRange r = worksheet.Cells[_startrow, startcol, startrow - 1, startcol + 17];
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    Bytes = excelPackage.GetAsByteArray();
                }
                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Ke_hoach_kiem_toan_nam.xlsx");
                //return Ok(new { code = "1", data = Bytes, file_name = "Ke_hoach_kiem_toan_nam.xlsx",   msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("ExportExcel/{id}")]
        public IActionResult Export(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == id && a.IsDelete != true);
                if (auditplan == null)
                {
                    return NotFound();
                }

                var _auditwork_id = _uow.Repository<AuditWorkPlan>().Find(a => a.auditplan_id == id && a.IsDeleted != true).Select(a => a.Id).ToArray();
                var _auditWorkScopePlanFacility = _uow.Repository<AuditWorkScopePlanFacility>().Find(a => a.auditwork_id.HasValue && _auditwork_id.Contains(a.auditwork_id.Value) && a.IsDeleted != true).ToArray();
                var _user = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsActive == true).ToArray();
                var audit_work_scope = _uow.Repository<AuditWorkScopePlan>().Include(a => a.AuditWorkPlan).Where(a => a.auditprocess_id.HasValue && a.auditwork_id.HasValue && _auditwork_id.Contains(a.auditwork_id.Value) && a.IsDeleted != true).AsEnumerable().GroupBy(a => a.auditwork_id);
                var auditwork_assign = _uow.Repository<AuditAssignmentPlan>().Find(a => a.auditwork_id.HasValue && _auditwork_id.Contains(a.auditwork_id.Value) && a.IsDeleted != true).ToArray();
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Ke_hoach_kiem_toan_nam.xlsx");
                var template = new FileInfo(fullPath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Sheet1"];
                    ExcelRange cellHeader = worksheet.Cells[2, 1];
                    cellHeader.Value = "KẾ HOẠCH KIỂM TOÁN NĂM " + auditplan.Year;
                    cellHeader.Style.Font.Size = 11;
                    cellHeader.Style.Font.Bold = true;
                    var _startrow = 6;
                    var startrow = 6;
                    var startcol = 1;
                    var count = 0;

                    foreach (var a in audit_work_scope)
                    {
                        count++;
                        ExcelRange cellNo = worksheet.Cells[startrow, startcol];
                        cellNo.Value = count;

                        ExcelRange cellCode = worksheet.Cells[startrow, startcol + 1];
                        cellCode.Value = a.FirstOrDefault()?.AuditWorkPlan?.Code;
                        ExcelRange cellName = worksheet.Cells[startrow, startcol + 2];
                        cellName.Value = a.FirstOrDefault()?.AuditWorkPlan?.Name;


                        var _auditWorkScopePlanFacilityItem = _auditWorkScopePlanFacility.Where(x => x.auditwork_id == a.Key).ToArray();
                        var list_risk_rating = "";
                        var LastestAudit = "";

                        ExcelRange cellReason = worksheet.Cells[startrow, startcol + 3];
                        cellReason.Value = string.Join(", ", _auditWorkScopePlanFacilityItem.Where(x => !string.IsNullOrEmpty(x.ReasonNote)).Select(p => p.ReasonNote).Distinct());

                        ExcelRange cellAuditFacility = worksheet.Cells[startrow, startcol + 4];
                        cellAuditFacility.Value = string.Join(", ", _auditWorkScopePlanFacilityItem.Select(x => x.auditfacilities_name).Distinct());

                        foreach (var x in _auditWorkScopePlanFacilityItem)
                        {
                            if (!string.IsNullOrEmpty(x.RiskRatingName))
                            {
                                list_risk_rating += x.auditfacilities_name + ":" + x.RiskRatingName + Environment.NewLine;
                            }
                            if (x.AuditingTimeNearest.HasValue)
                            {
                                LastestAudit += x.auditfacilities_name + ":" + x.AuditingTimeNearest.Value.Year + Environment.NewLine;
                            }
                        }
                        ExcelRange cellRiskrating = worksheet.Cells[startrow, startcol + 5];
                        cellRiskrating.Value = list_risk_rating;

                        ExcelRange cellLastestAudit = worksheet.Cells[startrow, startcol + 6];
                        cellLastestAudit.Value = LastestAudit;

                        ExcelRange cellAuditProcess = worksheet.Cells[startrow, startcol + 7];
                        cellAuditProcess.Value = string.Join(", ", a.Select(x => x.auditprocess_name).Distinct());
                        var ScaleOfAudit = a.FirstOrDefault()?.AuditWorkPlan?.ScaleOfAudit;
                        var _scaleOfAudit = "";
                        switch (ScaleOfAudit)
                        {
                            case 0:
                                _scaleOfAudit = "Rất lớn";
                                break;
                            case 1:
                                _scaleOfAudit = "Lớn";
                                break;
                            case 2:
                                _scaleOfAudit = "Trung bình";
                                break;
                            case 3:
                                _scaleOfAudit = "Nhỏ";
                                break;
                        }
                        ExcelRange cellScaleOfAudit = worksheet.Cells[startrow, startcol + 8];
                        cellScaleOfAudit.Value = _scaleOfAudit;
                        ExcelRange cellStartDate = worksheet.Cells[startrow, startcol + 9];
                        var StartDate = a.FirstOrDefault()?.AuditWorkPlan?.StartDate;
                        cellStartDate.Value = StartDate.HasValue ? StartDate.Value.ToString("dd/MM/yyyy") : "";
                        ExcelRange cellEnddate = worksheet.Cells[startrow, startcol + 10];
                        var EndDate = a.FirstOrDefault()?.AuditWorkPlan?.EndDate;
                        cellEnddate.Value = EndDate.HasValue ? EndDate.Value.ToString("dd/MM/yyyy") : "";
                        ExcelRange cellNumOfAuditor = worksheet.Cells[startrow, startcol + 11];
                        cellNumOfAuditor.Value = a.FirstOrDefault()?.AuditWorkPlan?.NumOfAuditor;
                        ExcelRange cellNumOfWorkdays = worksheet.Cells[startrow, startcol + 12];
                        cellNumOfWorkdays.Value = a.FirstOrDefault()?.AuditWorkPlan?.NumOfWorkdays;
                        ExcelRange cellPersonInCharge = worksheet.Cells[startrow, startcol + 13];
                        var person_in_charge = a.FirstOrDefault()?.AuditWorkPlan?.person_in_charge;
                        cellPersonInCharge.Value = person_in_charge.HasValue ? _user.FirstOrDefault(u => u.Id == person_in_charge)?.FullName : "";
                        ExcelRange cellListAssign = worksheet.Cells[startrow, startcol + 14];
                        cellListAssign.Value = string.Join(", ", auditwork_assign.Where(c => c.auditwork_id == a.Key).Select(c => c.Users.FullName).Distinct());
                        ExcelRange cellReqSkillForAudit = worksheet.Cells[startrow, startcol + 15];
                        cellReqSkillForAudit.Value = a.FirstOrDefault()?.AuditWorkPlan?.ReqSkillForAudit;
                        ExcelRange cellReqOutsourcing = worksheet.Cells[startrow, startcol + 16];
                        cellReqOutsourcing.Value = a.FirstOrDefault()?.AuditWorkPlan?.ReqOutsourcing;
                        ExcelRange cellReqOther = worksheet.Cells[startrow, startcol + 17];
                        cellReqOther.Value = a.FirstOrDefault()?.AuditWorkPlan?.ReqOther;

                        startrow++;
                    }
                    using ExcelRange r = worksheet.Cells[_startrow, startcol, _startrow == startrow ? startrow : startrow - 1, startcol + 17];
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                    Bytes = excelPackage.GetAsByteArray();
                }
                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Ke_hoach_kiem_toan_nam.xlsx");
                //return Ok(new { code = "1", data = Bytes, file_name = "Ke_hoach_kiem_toan_nam.xlsx",   msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpGet("DeleteAuditWorkScopeFacilityFile/{id}")]
        public IActionResult DeleteAuditWorkScopeFacilityFile(int id)
        {
            try
            {
                var self = _uow.Repository<AuditWorkScopeFacilityFile>().FirstOrDefault(o => o.id == id);
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
                    _uow.Repository<AuditWorkScopeFacilityFile>().Update(self);
                }

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        //drop trạng thái Cuộc kiểm toán
        [HttpGet("ListStatusPrepareAuditPlan")]
        public IActionResult ListStatusPrepareAuditPlan()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalConfig>().GetAll(a => a.item_code == "M_PAP").ToArray().OrderBy(x => x.StatusCode);

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
                return Ok(new { code = "0", msg = "fail", data = new DropListStatusPrepareAuditPlanModel() });
            }

        }
        //drop trạng thái Cuộc kiểm toán khi cập nhật trạng thái
        [HttpGet("ListUpdateStatusPrepareAuditPlan")]
        public IActionResult ListUpdateStatusPrepareAuditPlan()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalConfig>().Find(a => a.item_code == "M_PAP"
                && (a.StatusCode == "3.1" || a.StatusCode == "3.2")).ToArray().OrderBy(x => x.StatusCode);

                var dt = approval_status.Select(a => new DropListUpdateStatusPrepareAuditPlanModel()
                {
                    id = a.id,
                    status_code = a.StatusCode,
                    status_name = a.StatusName,
                });
                return Ok(new { code = "1", msg = "success", data = dt });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new DropListUpdateStatusPrepareAuditPlanModel() });
            }

        }
        //xuất thông báo word MIC
        [HttpGet("ExportWordMIC/{id}")]
        public IActionResult ExportWordMIC(int id/*, int id_dv*/)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == id && a.IsDeleted != true);
                //data
                //var nameauditworkscope = _uow.Repository<AuditWorkScopeFacility>().FirstOrDefault(x => x.auditfacilities_id == id_dv && x.auditwork_id == id && x.IsDeleted != true);
                var _auditWorkScope = _uow.Repository<AuditWorkScope>().Include(a => a.AuditWorkScopeUserMapping).Where(a => a.auditwork_id == id
                && a.IsDeleted != true /*&& a.auditfacilities_id == id_dv*/).ToArray();
                var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "REPORT_HEADER");
                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");
                var day = DateTime.Now.ToString("dd");
                var month = DateTime.Now.ToString("MM");
                var year = DateTime.Now.ToString("yyyy");
                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_ThongBaoKiemToan.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\Kitano_ThongBaoKiemToan.docx"))
                {
                    //Header
                    doc.Sections[0].PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = doc.Sections[0].HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem.Value);

                    doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1",/* "TEN_DON_VI_1",*/ "ten_cong_ty_1",/* "ten_cuoc_kiem_toan_1",*/ "muc_tieu_cuoc_kiem_toan_1", "thoi_hieu_kt_1", "thoi_hieu_kt_2", "NGAY_BD_LAP_KH", "NGAY_KT_LAP_KH", "NGAY_BD_TD", "NGAY_PHAT_HANH_BC", "NGAY_KT_TD", "pham_vi_kt_new_1" },
                        new string[] { day, month, year,/* nameauditworkscope.auditfacilities_name,*/ dataCt.Value,/* checkAuditWorkDetail.Name,*/
                            checkAuditWorkDetail.Target,
                        checkAuditWorkDetail.from_date == null ? "dd/MM/yyyy": checkAuditWorkDetail.from_date.Value.ToString("dd/MM/yyyy"),
                        checkAuditWorkDetail.to_date == null ? "dd/MM/yyyy": checkAuditWorkDetail.to_date.Value.ToString("dd/MM/yyyy"),
                        checkAuditWorkDetail.StartDate != null ? checkAuditWorkDetail.StartDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.EndDatePlanning != null ? checkAuditWorkDetail.EndDatePlanning.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.StartDateReal != null ? checkAuditWorkDetail.StartDateReal.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.ReleaseDate != null ? checkAuditWorkDetail.ReleaseDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.EndDate != null ? checkAuditWorkDetail.EndDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.AuditScope});
                    //Table table = doc.Sections[0].Tables[1] as Table;
                    //table.ResetCells(_auditWorkScope.Count() + 1, 2);
                    //TextRange txtTable = table[0, 0].AddParagraph().AppendText("STT");
                    //txtTable.CharacterFormat.FontName = "Times New Roman";
                    //txtTable.CharacterFormat.FontSize = 13;
                    //txtTable.CharacterFormat.Bold = true;
                    //table[0, 0].Width = 20;
                    //txtTable = table[0, 1].AddParagraph().AppendText("Quy trình");
                    //txtTable.CharacterFormat.FontName = "Times New Roman";
                    //txtTable.CharacterFormat.FontSize = 13;
                    //txtTable.CharacterFormat.Bold = true;
                    //table[0, 1].Width = 500;
                    //for (int x = 0; x < _auditWorkScope.Length; x++)
                    //{
                    //    txtTable = table[x + 1, 0].AddParagraph().AppendText((x + 1).ToString());
                    //    txtTable.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable.CharacterFormat.FontSize = 13;
                    //    table[x + 1, 0].Width = 20;

                    //    txtTable = table[x + 1, 1].AddParagraph().AppendText(_auditWorkScope[x].auditprocess_name);
                    //    txtTable.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable.CharacterFormat.FontSize = 13;
                    //    table[x + 1, 1].Width = 500;
                    //}

                    var arrData = new List<AuditWorkScopeUserMapping>();
                    for (int x = 0; x < _auditWorkScope.Length; x++)
                    {
                        var checkAuditWorkScopeUserMapping = _uow.Repository<AuditWorkScopeUserMapping>().Include(a => a.Users).Where(a => a.auditwork_scope_id == _auditWorkScope[x].Id).ToArray();
                        for (int j = 0; j < checkAuditWorkScopeUserMapping.Length; j++)
                        {
                            arrData.Add(checkAuditWorkScopeUserMapping[j]);
                        }
                    }
                    //Table table1 = doc.Sections[0].Tables[3] as Table;
                    Table table1 = doc.Sections[0].Tables[2] as Table;
                    table1.ResetCells(arrData.Count() + 2, 3);
                    TextRange txtTable1 = table1[0, 0].AddParagraph().AppendText("STT");
                    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    txtTable1.CharacterFormat.FontSize = 13;
                    txtTable1.CharacterFormat.Bold = true;
                    table1[0, 0].Width = 20;
                    txtTable1 = table1[0, 1].AddParagraph().AppendText("Họ và tên");
                    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    txtTable1.CharacterFormat.FontSize = 13;
                    txtTable1.CharacterFormat.Bold = true;
                    table1[0, 1].Width = 200;
                    txtTable1 = table1[0, 2].AddParagraph().AppendText("Vai trò");
                    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    txtTable1.CharacterFormat.FontSize = 13;
                    txtTable1.CharacterFormat.Bold = true;
                    table1[0, 2].Width = 300;

                    txtTable1 = table1[1, 0].AddParagraph().AppendText("1");
                    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    txtTable1.CharacterFormat.FontSize = 13;
                    table1[1, 0].Width = 20;
                    txtTable1 = table1[1, 1].AddParagraph().AppendText(checkAuditWorkDetail.Users != null ? checkAuditWorkDetail.Users.FullName : "");
                    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    txtTable1.CharacterFormat.FontSize = 13;
                    table1[1, 1].Width = 200;
                    txtTable1 = table1[1, 2].AddParagraph().AppendText("Trưởng đoàn kiểm toán");
                    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    txtTable1.CharacterFormat.FontSize = 13;
                    table1[1, 2].Width = 300;

                    var arrDataNew = arrData.Select(x => new { x.full_name, x.type, x.Users.Email }).Distinct().ToList();

                    for (int i = 0; i < arrDataNew.Count(); i++)
                    {
                        txtTable1 = table1[i + 2, 0].AddParagraph().AppendText((i + 2).ToString());
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 13;
                        table1[i + 2, 0].Width = 20;
                        txtTable1 = table1[i + 2, 1].AddParagraph().AppendText(arrDataNew[i].full_name);
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 13;
                        table1[i + 2, 1].Width = 200;

                        if (arrDataNew[i].type == 1)
                        {
                            txtTable1 = table1[i + 2, 2].AddParagraph().AppendText("Trưởng nhóm kiểm toán");
                            txtTable1.CharacterFormat.FontName = "Times New Roman";
                            txtTable1.CharacterFormat.FontSize = 13;
                            table1[i + 2, 2].Width = 300;
                        }
                        else if (arrDataNew[i].type == 2)
                        {
                            txtTable1 = table1[i + 2, 2].AddParagraph().AppendText("Kiểm toán viên – Thành viên");
                            txtTable1.CharacterFormat.FontName = "Times New Roman";
                            txtTable1.CharacterFormat.FontSize = 13;
                            table1[i + 2, 2].Width = 300;
                        }
                    }

                    //for (int i = 0; i < arrData.Count(); i++)
                    //{
                    //    txtTable1 = table1[i + 2, 0].AddParagraph().AppendText((i + 2).ToString());
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 13;
                    //    table1[i + 2, 0].Width = 20;
                    //    txtTable1 = table1[i + 2, 1].AddParagraph().AppendText(arrData[i].full_name);
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 13;
                    //    table1[i + 2, 1].Width = 200;

                    //    if (arrData[i].type == 1)
                    //    {
                    //        txtTable1 = table1[i + 2, 2].AddParagraph().AppendText("Trưởng nhóm kiểm toán");
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 13;
                    //        table1[i + 2, 2].Width = 300;
                    //    }
                    //    else if (arrData[i].type == 2)
                    //    {
                    //        txtTable1 = table1[i + 2, 2].AddParagraph().AppendText("Kiểm toán viên – Thành viên");
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 13;
                    //        table1[i + 2, 2].Width = 300;
                    //    }
                    //}
                    foreach (Section section1 in doc.Sections)
                    {
                        for (int i = 0; i < section1.Body.ChildObjects.Count; i++)
                        {
                            if (section1.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
                            {
                                if (String.IsNullOrEmpty((section1.Body.ChildObjects[i] as Paragraph).Text.Trim()))
                                {
                                    section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[i]);
                                    i--;
                                }
                            }

                        }
                    }
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_ThongBaoKiemToan.docx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        //xuất thông báo word MBS
        [HttpGet("ExportWordMBS/{id}")]
        public IActionResult ExportWordMBS(int id/*, int id_dv*/)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == id && a.IsDeleted != true);
                //data
                var nameauditworkscope = _uow.Repository<AuditWorkScopeFacility>().Find(x => x.auditwork_id == id && x.IsDeleted != true).ToArray();
                var arrName = string.Join(", ", nameauditworkscope.Select(x => x.auditfacilities_name).Distinct());
                var NDC2 = _uow.Repository<ApprovalFunction>().Include(a => a.Users).FirstOrDefault(a => a.item_id == id && a.StatusCode == "3.1");
                var TPDKT = _uow.Repository<AuditAssignment>().Find(z => z.auditwork_id.Equals(id) && z.user_id != (checkAuditWorkDetail.Users != null ? checkAuditWorkDetail.Users.Id : 0)).ToArray();
                var _auditWorkScope = _uow.Repository<AuditWorkScope>().Include(a => a.AuditWorkScopeUserMapping).Where(a => a.auditwork_id == id
                && a.IsDeleted != true /*&& a.auditfacilities_id == id_dv*/).ToArray();
                var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "REPORT_HEADER");
                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");
                var day = DateTime.Now.ToString("dd");
                var month = DateTime.Now.ToString("MM");
                var year = DateTime.Now.ToString("yyyy");
                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "MBS_Kitano_ThongBaoKiemToan_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\MBS_Kitano_ThongBaoKiemToan_v0.1.docx"))
                {
                    //Header
                    doc.Sections[0].PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = doc.Sections[0].HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem.Value);

                    doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1", "ten_cuoc_kiem_toan_1", "nam_kt_1", "ten_cac_dv", "ten_cuoc_kiem_toan_1",
                        "NDC2", "muc_tieu_cuoc_kiem_toan_1", "thoi_hieu_kt_1", "thoi_hieu_kt_2", "NGAY_BD_LAP_KH"/*, "NGAY_KT_LAP_KH", "NGAY_BD_TD", "NGAY_PHAT_HANH_BC"*/, "NGAY_KT_TD"/*, "pham_vi_kt_new_1"*/ },
                        new string[] { day, month, year, checkAuditWorkDetail.Name, checkAuditWorkDetail.Year, arrName, checkAuditWorkDetail.Name,
                        (NDC2 != null ? NDC2.Users != null ? NDC2.Users.FullName: "" : ""),
                            checkAuditWorkDetail.Target,
                        checkAuditWorkDetail.from_date == null ? "dd/MM/yyyy": checkAuditWorkDetail.from_date.Value.ToString("dd/MM/yyyy"),
                        checkAuditWorkDetail.to_date == null ? "dd/MM/yyyy": checkAuditWorkDetail.to_date.Value.ToString("dd/MM/yyyy"),
                        checkAuditWorkDetail.StartDate != null ? checkAuditWorkDetail.StartDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        //checkAuditWorkDetail.EndDatePlanning != null ? checkAuditWorkDetail.EndDatePlanning.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        //checkAuditWorkDetail.StartDateReal != null ? checkAuditWorkDetail.StartDateReal.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        //checkAuditWorkDetail.ReleaseDate != null ? checkAuditWorkDetail.ReleaseDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.EndDate != null ? checkAuditWorkDetail.EndDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        //checkAuditWorkDetail.AuditScope
                        });
                    Table table = doc.Sections[0].Tables[1] as Table;
                    table.ResetCells(TPDKT.Length + 1, 2);
                    TextRange txtTable = table[0, 0].AddParagraph().AppendText("Ông/Bà: " + (checkAuditWorkDetail.Users != null ? checkAuditWorkDetail.Users.FullName : ""));
                    txtTable.CharacterFormat.FontName = "Times New Roman";
                    txtTable.CharacterFormat.FontSize = 12;

                    txtTable = table[0, 1].AddParagraph().AppendText("Chức danh: Trưởng đoàn");
                    txtTable.CharacterFormat.FontName = "Times New Roman";
                    txtTable.CharacterFormat.FontSize = 12;
                    for (int x = 0; x < TPDKT.Length; x++)
                    {
                        txtTable = table[x + 1, 0].AddParagraph().AppendText("Ông/Bà: " + "" + TPDKT[x].fullName);
                        txtTable.CharacterFormat.FontName = "Times New Roman";
                        txtTable.CharacterFormat.FontSize = 12;

                        txtTable = table[x + 1, 1].AddParagraph().AppendText("Chức danh: Thành viên");
                        txtTable.CharacterFormat.FontName = "Times New Roman";
                        txtTable.CharacterFormat.FontSize = 12;
                    }
                    //var arrData = new List<AuditWorkScopeUserMapping>();
                    //for (int x = 0; x < _auditWorkScope.Length; x++)
                    //{
                    //    var checkAuditWorkScopeUserMapping = _uow.Repository<AuditWorkScopeUserMapping>().Include(a => a.Users).Where(a => a.auditwork_scope_id == _auditWorkScope[x].Id).ToArray();
                    //    for (int j = 0; j < checkAuditWorkScopeUserMapping.Length; j++)
                    //    {
                    //        arrData.Add(checkAuditWorkScopeUserMapping[j]);
                    //    }
                    //}
                    ////Table table1 = doc.Sections[0].Tables[3] as Table;
                    //Table table1 = doc.Sections[0].Tables[2] as Table;
                    //table1.ResetCells(arrData.Count() + 2, 3);
                    //TextRange txtTable1 = table1[0, 0].AddParagraph().AppendText("STT");
                    //txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //txtTable1.CharacterFormat.FontSize = 13;
                    //txtTable1.CharacterFormat.Bold = true;
                    //table1[0, 0].Width = 20;
                    //txtTable1 = table1[0, 1].AddParagraph().AppendText("Họ và tên");
                    //txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //txtTable1.CharacterFormat.FontSize = 13;
                    //txtTable1.CharacterFormat.Bold = true;
                    //table1[0, 1].Width = 200;
                    //txtTable1 = table1[0, 2].AddParagraph().AppendText("Vai trò");
                    //txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //txtTable1.CharacterFormat.FontSize = 13;
                    //txtTable1.CharacterFormat.Bold = true;
                    //table1[0, 2].Width = 300;

                    //txtTable1 = table1[1, 0].AddParagraph().AppendText("1");
                    //txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //txtTable1.CharacterFormat.FontSize = 13;
                    //table1[1, 0].Width = 20;
                    //txtTable1 = table1[1, 1].AddParagraph().AppendText(checkAuditWorkDetail.Users != null ? checkAuditWorkDetail.Users.FullName : "");
                    //txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //txtTable1.CharacterFormat.FontSize = 13;
                    //table1[1, 1].Width = 200;
                    //txtTable1 = table1[1, 2].AddParagraph().AppendText("Trưởng đoàn kiểm toán");
                    //txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //txtTable1.CharacterFormat.FontSize = 13;
                    //table1[1, 2].Width = 300;

                    //var arrDataNew = arrData.Select(x => new { x.full_name, x.type, x.Users.Email }).Distinct().ToList();

                    //for (int i = 0; i < arrDataNew.Count(); i++)
                    //{
                    //    txtTable1 = table1[i + 2, 0].AddParagraph().AppendText((i + 2).ToString());
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 13;
                    //    table1[i + 2, 0].Width = 20;
                    //    txtTable1 = table1[i + 2, 1].AddParagraph().AppendText(arrDataNew[i].full_name);
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 13;
                    //    table1[i + 2, 1].Width = 200;

                    //    if (arrDataNew[i].type == 1)
                    //    {
                    //        txtTable1 = table1[i + 2, 2].AddParagraph().AppendText("Trưởng nhóm kiểm toán");
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 13;
                    //        table1[i + 2, 2].Width = 300;
                    //    }
                    //    else if (arrDataNew[i].type == 2)
                    //    {
                    //        txtTable1 = table1[i + 2, 2].AddParagraph().AppendText("Kiểm toán viên – Thành viên");
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 13;
                    //        table1[i + 2, 2].Width = 300;
                    //    }
                    //}


                    //foreach (Section section1 in doc.Sections)
                    //{
                    //    for (int i = 0; i < section1.Body.ChildObjects.Count; i++)
                    //    {
                    //        if (section1.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
                    //        {
                    //            if (String.IsNullOrEmpty((section1.Body.ChildObjects[i] as Paragraph).Text.Trim()))
                    //            {
                    //                section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[i]);
                    //                i--;
                    //            }
                    //        }

                    //    }
                    //}
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_ThongBaoKiemToan.docx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// xuất thông báo kiểm toán đã được customize cho AMC
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id_dv"></param>
        /// <returns></returns>
        [HttpGet("ExportWordAMC/{id}")]
        public IActionResult ExportWordAMC(int id/*, int id_dv*/)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(x => x.AuditWorkScopeFacility).FirstOrDefault(a => a.Id == id);
                var auditAssignment = _uow.Repository<AuditAssignment>().Include(a => a.Users).Where(a => a.auditwork_id == id && a.IsDeleted != true).OrderByDescending(x => x.user_id == checkAuditWorkDetail.person_in_charge).ToArray();
                var checkAuditWorkScopeFacilityName = checkAuditWorkDetail.AuditWorkScopeFacility.Where(x => x.IsDeleted != true).Select(x => x.auditfacilities_name).ToArray();
                var day = DateTime.Now.ToString("dd");
                var month = DateTime.Now.ToString("MM");
                var year = DateTime.Now.ToString("yyyy");
                //data

                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");
                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "AMC_Kitano_QuyetDinhKiemToan_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\Kitano_DeCuongKiemToan_v0.1.docx"))
                {
                    var font = "Times New Roman";

                    Table table1 = doc.Sections[0].Tables[1] as Table;
                    ListStyle listStyle = new ListStyle(doc, ListType.Bulleted);
                    listStyle.Name = "levelstyle";
                    listStyle.Levels[0].BulletCharacter = "-";
                    listStyle.Levels[0].CharacterFormat.FontName = font;
                    listStyle.Levels[0].CharacterFormat.FontSize = 11;
                    listStyle.Levels[0].TextPosition = 28.08f;//28.08 = 0.39 inch trong word
                    listStyle.Levels[0].NumberPosition = 0;
                    doc.ListStyles.Add(listStyle);

                    doc.MailMerge.MergeField += new MergeFieldEventHandler(MailMerge_MergeField);
                    //remove empty paragraphs
                    doc.MailMerge.HideEmptyParagraphs = true;
                    //remove empty group
                    doc.MailMerge.HideEmptyGroup = true;
                    doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1", "ten_cuoc_kt",
                        "muc_tieu_kt_1",
                        "pham_vi_kt_1",
                        "so_ngay_kt",
                        "ngay_bat_dau_kt",
                        "thoi_hieu_kt_tu_1",
                        "thoi_hieu_kt_den_1",
                        "html_de_cuong_kt",
                        "ten_dv_duoc_kt",
                    },
                    new string[] { day, month, year, checkAuditWorkDetail.Name,
                        checkAuditWorkDetail.Target,
                        checkAuditWorkDetail.AuditScope,
                        (checkAuditWorkDetail.EndDate != null && checkAuditWorkDetail.StartDate != null) ? GetWorkingDays((DateTime)checkAuditWorkDetail.StartDate, (DateTime)checkAuditWorkDetail.EndDate).ToString() : "Chưa xác định",
                        checkAuditWorkDetail.StartDate!= null ? checkAuditWorkDetail.StartDate.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                        checkAuditWorkDetail.from_date!= null ? checkAuditWorkDetail.from_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                        checkAuditWorkDetail.to_date != null ? checkAuditWorkDetail.to_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                        checkAuditWorkDetail.other,
                        string.Join(", ", checkAuditWorkScopeFacilityName),
                    });

                    foreach (var (item, i) in auditAssignment.Select((item, i) => (item, i)))
                    {
                        var row = table1.AddRow();
                        string role = item.user_id == checkAuditWorkDetail.person_in_charge ? "Trưởng đoàn" : "Thành viên";

                        var paragraphTable1 = row.Cells[0].AddParagraph();
                        var txtParagraphTable1 = paragraphTable1.AppendText("Ông/bà: " + item.Users.FullName);
                        paragraphTable1.ListFormat.ApplyStyle("levelstyle");
                        paragraphTable1.ApplyStyle(BuiltinStyle.Normal);
                        paragraphTable1.Format.FirstLineIndent = -28.08f; //28.08 = 0.39 inch trong word

                        paragraphTable1 = row.Cells[1].AddParagraph();
                        txtParagraphTable1 = paragraphTable1.AppendText(role);
                        paragraphTable1.ApplyStyle(BuiltinStyle.Normal);
                    }
                    table1.Rows.RemoveAt(0);

                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_DeCuongKiemToan_v0.1.docx");
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //xuất word
        [HttpGet("ExportFileWord/{id}")]
        public IActionResult ExportFileWord(int id)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == id);
                //check người tạo
                var planPerson = _uow.Repository<Users>().FirstOrDefault(x => x.Id == checkAuditWorkDetail.CreatedBy);
                //check người duyệt
                var personAppro = _uow.Repository<ApprovalFunction>().Include(a => a.Users).FirstOrDefault(x => x.item_id == checkAuditWorkDetail.Id
                && x.function_code == "M_PAP" && x.StatusCode == "3.1");
                //if (personAppro != null)
                //{
                //    var namePersonAppro = _uow.Repository<Users>().FirstOrDefault(u=>u.Id == personAppro.approver);
                //}
                //check tab 2
                var checkAuditAssignment = _uow.Repository<AuditAssignment>().Include(x => x.AuditWork).FirstOrDefault(x => x.auditwork_id == id);
                var auditAssignment = _uow.Repository<AuditAssignment>().Include(a => a.Users).Where(a => a.auditwork_id == id && a.IsDeleted != true).ToArray();

                //check tab 3
                var checkAuditWorkScope = _uow.Repository<AuditWorkScope>().Include(x => x.AuditWork).FirstOrDefault(x => x.auditwork_id == id);
                var auditWorkScope = _uow.Repository<AuditWorkScope>().Include(a => a.AuditWorkScopeUserMapping).Where(a => a.auditwork_id == id && a.IsDeleted != true).ToArray();
                for (int i = 0; i < auditWorkScope.Length; i++)
                {
                    var checkAuditWorkScopeUserMapping = _uow.Repository<AuditWorkScopeUserMapping>().Include(a => a.Users).Where(a => a.auditwork_scope_id == auditWorkScope[i].Id).ToArray();
                }
                //check AuditWorkScopePlanFacility
                var checkAuditWorkScopeFacility = _uow.Repository<AuditWorkScopeFacility>().Include(x => x.AuditWorkScopeFacilityFile).Where(a => a.IsDeleted != true
                && a.auditwork_id == id).ToArray();

                //for (int i = 0; i < checkAuditWorkScopeFacility.Length; i++)
                //{
                //    var auditStrategyRisk = _uow.Repository<AuditStrategyRisk>().Include(a => a.AuditWorkScopeFacility).Where(a => a.auditwork_scope_id == checkAuditWorkScopeFacility[i].Id && a.is_deleted != true).ToArray();
                //}

                //check tab 4
                var checkSchedule = _uow.Repository<Schedule>().Include(x => x.AuditWork).FirstOrDefault(x => x.auditwork_id == id);
                var auditSchedule = _uow.Repository<Schedule>().Include(a => a.Users).Where(a => a.auditwork_id == id && a.is_deleted != true).ToArray();

                //data

                var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "REPORT_HEADER");
                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");

                var day = DateTime.Now.ToString("dd");
                var month = DateTime.Now.ToString("MM");
                var year = DateTime.Now.ToString("yyyy");

                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_KeHoachCuocKT_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\Kitano_KeHoachCuocKT_v0.1.docx"))
                {

                    //Header
                    doc.Sections[0].PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = doc.Sections[0].HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem.Value);

                    //Remove header 2
                    doc.Sections[1].HeadersFooters.Header.ChildObjects.Clear();
                    //Add line breaks
                    Paragraph headerParagraph = doc.Sections[1].HeadersFooters.FirstPageHeader.AddParagraph();
                    headerParagraph.AppendBreak(BreakType.LineBreak);
                    //Remove header 3
                    doc.Sections[2].HeadersFooters.Header.ChildObjects.Clear();
                    //Add line breaks
                    Paragraph headerParagraph2 = doc.Sections[2].HeadersFooters.FirstPageHeader.AddParagraph();
                    headerParagraph2.AppendBreak(BreakType.LineBreak);

                    //ngân sách
                    if (checkAuditWorkDetail.Budget != null)
                    {
                        Paragraph paraBudget = doc.Sections[3].AddParagraph();// para cuar Editor
                        paraBudget.AppendHTML(checkAuditWorkDetail.Budget);
                    }
                    //khác
                    if (checkAuditWorkDetail.other != null)
                    {
                        Paragraph paraOther = doc.Sections[2].AddParagraph();// para cuar Editor
                        paraOther.AppendHTML(checkAuditWorkDetail.other);
                    }

                    doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1", "nguoi_tao_ke_hoach", "ngay_tao_ke_hoach", "nguoi_duyet_ke_hoach", "ngay_duyet_ke_hoach", "ten_cuoc_kiem_toan_1", "ma_cuoc_kiem_toan_1", "muc_tieu_kiem_toan_1", "pham_vi_kt_new_1", "ngoai_pham_vi_kiem_toan_1", "ngay_bat_dau_ke_hoach_1", "ngay_ket_thuc_thuc_dia_1", "thoi_hieu_kt_tu_1", "thoi_hieu_kt_den_1" },
                        new string[] {day, month, year, planPerson.FullName,
                            checkAuditWorkDetail.CreatedAt == null ? "dd/MM/yyyy" : checkAuditWorkDetail.CreatedAt.Value.ToString("dd/MM/yyyy"),
                            personAppro != null ? (personAppro.Users != null ? personAppro.Users.FullName : "") : "",
                            (personAppro != null ? (personAppro.ApprovalDate == null ? "dd/MM/yyyy" : personAppro.ApprovalDate.Value.ToString("dd/MM/yyyy")) : ""),
                            checkAuditWorkDetail.Name, checkAuditWorkDetail.Code, checkAuditWorkDetail.Target, checkAuditWorkDetail.AuditScope,
                            checkAuditWorkDetail.AuditScopeOutside,
                            (checkAuditWorkDetail.StartDate == null ? "dd/MM/yyyy": checkAuditWorkDetail.StartDate.Value.ToString("dd/MM/yyyy")),
                            (checkAuditWorkDetail.EndDate == null ? "dd/MM/yyyy" : checkAuditWorkDetail.EndDate.Value.ToString("dd/MM/yyyy")),
                            (checkAuditWorkDetail.from_date == null ? "dd/MM/yyyy": checkAuditWorkDetail.from_date.Value.ToString("dd/MM/yyyy")),
                            (checkAuditWorkDetail.to_date == null ? "dd/MM/yyyy": checkAuditWorkDetail.to_date.Value.ToString("dd/MM/yyyy")) });


                    //Table table = doc.Sections[0].Tables[1] as Table;
                    //table.ResetCells(auditWorkScope.Count() + 1, 3);
                    //TextRange txtTable = table[0, 0].AddParagraph().AppendText("Quy trình được KT");
                    //txtTable.CharacterFormat.FontName = "Times New Roman";
                    //txtTable.CharacterFormat.FontSize = 12;
                    //txtTable.CharacterFormat.Bold = true;

                    //txtTable = table[0, 1].AddParagraph().AppendText("Đơn vị được KT");
                    //txtTable.CharacterFormat.FontName = "Times New Roman";
                    //txtTable.CharacterFormat.FontSize = 12;
                    //txtTable.CharacterFormat.Bold = true;

                    //txtTable = table[0, 2].AddParagraph().AppendText("Hoạt động được KT");
                    //txtTable.CharacterFormat.FontName = "Times New Roman";
                    //txtTable.CharacterFormat.FontSize = 12;
                    //txtTable.CharacterFormat.Bold = true;

                    //for (int x = 0; x < auditWorkScope.Length; x++)
                    //{
                    //    txtTable = table[x + 1, 0].AddParagraph().AppendText(auditWorkScope[x].auditprocess_name);
                    //    txtTable.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable.CharacterFormat.FontSize = 12;

                    //    txtTable = table[x + 1, 1].AddParagraph().AppendText(auditWorkScope[x].auditfacilities_name);
                    //    txtTable.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable.CharacterFormat.FontSize = 12;

                    //    txtTable = table[x + 1, 2].AddParagraph().AppendText(auditWorkScope[x].bussinessactivities_name);
                    //    txtTable.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable.CharacterFormat.FontSize = 12;
                    //}

                    var arrData = new List<AuditStrategyRisk>();
                    for (int x = 0; x < checkAuditWorkScopeFacility.Length; x++)
                    {
                        var _auditstrategyrisk = _uow.Repository<AuditStrategyRisk>().Include(a => a.AuditWorkScopeFacility).Where(a => a.auditwork_scope_id == checkAuditWorkScopeFacility[x].Id && a.is_deleted != true).ToArray();
                        for (int j = 0; j < _auditstrategyrisk.Length; j++)
                        {
                            arrData.Add(_auditstrategyrisk[j]);
                        }
                    }
                    //
                    Table table1 = doc.Sections[1].Tables[0] as Table;
                    table1.ResetCells(arrData.Count() + 2, 3);
                    TextRange txtTable1 = table1[0, 0].AddParagraph().AppendText("Đơn vị được kiểm toán");
                    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    txtTable1.CharacterFormat.FontSize = 12;
                    txtTable1.CharacterFormat.Bold = true;

                    txtTable1 = table1[0, 1].AddParagraph().AppendText("Các rủi ro chính");
                    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    txtTable1.CharacterFormat.FontSize = 12;
                    txtTable1.CharacterFormat.Bold = true;

                    txtTable1 = table1[0, 2].AddParagraph().AppendText("Phương pháp kiểm toán");
                    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    txtTable1.CharacterFormat.FontSize = 12;
                    txtTable1.CharacterFormat.Bold = true;
                    for (int x = 0; x < arrData.Count(); x++)
                    {
                        txtTable1 = table1[x + 1, 0].AddParagraph().AppendText(arrData[x].AuditWorkScopeFacility.auditfacilities_name);
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;

                        txtTable1 = table1[x + 1, 1].AddParagraph().AppendText(arrData[x].name_risk);
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;

                        txtTable1 = table1[x + 1, 2].AddParagraph().AppendText(arrData[x].audit_strategy);
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                    }
                    var arrData1 = new List<AuditWorkScopeUserMapping>();
                    for (int x = 0; x < auditWorkScope.Length; x++)
                    {
                        var checkAuditWorkScopeUserMapping = _uow.Repository<AuditWorkScopeUserMapping>().Include(a => a.Users).Where(a => a.auditwork_scope_id == auditWorkScope[x].Id).ToArray();
                        for (int j = 0; j < checkAuditWorkScopeUserMapping.Length; j++)
                        {
                            arrData1.Add(checkAuditWorkScopeUserMapping[j]);
                        }
                    }
                    //
                    Table table2 = doc.Sections[3].Tables[1] as Table;
                    table2.ResetCells(arrData1.Count() + 2, 3);
                    TextRange txtTable2 = table2[0, 0].AddParagraph().AppendText("Họ và tên");
                    txtTable2.CharacterFormat.FontName = "Times New Roman";
                    txtTable2.CharacterFormat.FontSize = 12;
                    txtTable2.CharacterFormat.Bold = true;

                    txtTable2 = table2[0, 1].AddParagraph().AppendText("Vai trò");
                    txtTable2.CharacterFormat.FontName = "Times New Roman";
                    txtTable2.CharacterFormat.FontSize = 12;
                    txtTable2.CharacterFormat.Bold = true;

                    txtTable2 = table2[0, 2].AddParagraph().AppendText("Email");
                    txtTable2.CharacterFormat.FontName = "Times New Roman";
                    txtTable2.CharacterFormat.FontSize = 12;
                    txtTable2.CharacterFormat.Bold = true;

                    txtTable2 = table2[1, 0].AddParagraph().AppendText(checkAuditWorkDetail.Users != null ? checkAuditWorkDetail.Users.FullName : "");
                    txtTable2.CharacterFormat.FontName = "Times New Roman";
                    txtTable2.CharacterFormat.FontSize = 12;

                    txtTable2 = table2[1, 1].AddParagraph().AppendText("Trưởng đoàn kiểm toán");
                    txtTable2.CharacterFormat.FontName = "Times New Roman";
                    txtTable2.CharacterFormat.FontSize = 12;

                    txtTable2 = table2[1, 2].AddParagraph().AppendText(checkAuditWorkDetail.Users != null ? checkAuditWorkDetail.Users.Email : "");
                    txtTable2.CharacterFormat.FontName = "Times New Roman";
                    txtTable2.CharacterFormat.FontSize = 12;

                    //test
                    //var arDataNew = arrData1.GroupBy(x => x.full_name).Select(x => x.First()).ToList();
                    var arDataNew = arrData1.Select(x => new { x.full_name, x.type, x.Users.Email }).Distinct().ToList();

                    for (int i = 0; i < arDataNew.Count(); i++)
                    {
                        txtTable2 = table2[i + 2, 0].AddParagraph().AppendText(arDataNew[i].full_name);
                        txtTable2.CharacterFormat.FontName = "Times New Roman";
                        txtTable2.CharacterFormat.FontSize = 12;

                        if (arDataNew[i].type == 1)
                        {
                            txtTable2 = table2[i + 2, 1].AddParagraph().AppendText("Trưởng nhóm kiểm toán");
                            txtTable2.CharacterFormat.FontName = "Times New Roman";
                            txtTable2.CharacterFormat.FontSize = 12;
                        }
                        else if (arDataNew[i].type == 2)
                        {
                            txtTable2 = table2[i + 2, 1].AddParagraph().AppendText("Kiểm toán viên – Thành viên");
                            txtTable2.CharacterFormat.FontName = "Times New Roman";
                            txtTable2.CharacterFormat.FontSize = 12;
                        }
                        txtTable2 = table2[i + 2, 2].AddParagraph().AppendText(arDataNew[i].Email);
                        txtTable2.CharacterFormat.FontName = "Times New Roman";
                        txtTable2.CharacterFormat.FontSize = 12;
                    }
                    //
                    //for (int i = 0; i < arrData1.Count(); i++)
                    //{
                    //    txtTable2 = table2[i + 2, 0].AddParagraph().AppendText(arrData1[i].full_name);
                    //    txtTable2.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable2.CharacterFormat.FontSize = 12;

                    //    if (arrData1[i].type == 1)
                    //    {
                    //        txtTable2 = table2[i + 2, 1].AddParagraph().AppendText("Trưởng nhóm kiểm toán");
                    //        txtTable2.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable2.CharacterFormat.FontSize = 12;
                    //    }
                    //    else if (arrData1[i].type == 2)
                    //    {
                    //        txtTable2 = table2[i + 2, 1].AddParagraph().AppendText("Kiểm toán viên – Thành viên");
                    //        txtTable2.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable2.CharacterFormat.FontSize = 12;
                    //    }
                    //    txtTable2 = table2[i + 2, 2].AddParagraph().AppendText(arrData1[i].Users.Email);
                    //    txtTable2.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable2.CharacterFormat.FontSize = 12;
                    //}
                    //
                    Table table3 = doc.Sections[3].Tables[2] as Table;
                    table3.ResetCells(auditSchedule.Count() + 1, 3);
                    TextRange txtTable3 = table3[0, 0].AddParagraph().AppendText("Công việc");
                    txtTable3.CharacterFormat.FontName = "Times New Roman";
                    txtTable3.CharacterFormat.FontSize = 12;
                    txtTable3.CharacterFormat.Bold = true;

                    txtTable3 = table3[0, 1].AddParagraph().AppendText("Thời gian dự kiến thực hiện");
                    txtTable3.CharacterFormat.FontName = "Times New Roman";
                    txtTable3.CharacterFormat.FontSize = 12;
                    txtTable3.CharacterFormat.Bold = true;

                    txtTable3 = table3[0, 2].AddParagraph().AppendText("Người chịu trách nhiêm");
                    txtTable3.CharacterFormat.FontName = "Times New Roman";
                    txtTable3.CharacterFormat.FontSize = 12;
                    txtTable3.CharacterFormat.Bold = true;
                    for (int x = 0; x < auditSchedule.Length; x++)
                    {
                        txtTable3 = table3[x + 1, 0].AddParagraph().AppendText(auditSchedule[x].work);
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        table3[x + 1, 0].Width = 500;

                        txtTable3 = table3[x + 1, 1].AddParagraph().AppendText(auditSchedule[x].expected_date != null ? auditSchedule[x].expected_date.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy");
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        //table[x + 1, 1].Width = 500;

                        txtTable3 = table3[x + 1, 2].AddParagraph().AppendText(auditSchedule[x].Users.FullName);
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        //table[x + 1, 1].Width = 500;
                    }
                    //Doạn này là t lấy được số 
                    Section section = doc.Sections[2];
                    ListStyle listStyle = new ListStyle(doc, ListType.Numbered);
                    Paragraph paraInserted = section.AddParagraph();// para cuar Editor
                    //string mailing = "";
                    string bodyMailing = "";

                    for (int c = 0; c < checkAuditWorkScopeFacility.Length; c++)
                    {
                        bodyMailing += $"{c + 1}. {checkAuditWorkScopeFacility[c].auditfacilities_name}<br>";
                        bodyMailing += $"{checkAuditWorkScopeFacility[c].brief_review}<br>";
                    }
                    paraInserted.AppendHTML(bodyMailing);

                    //for (int i = 0; i < section.Paragraphs.Count; i++)
                    //{
                    //    var a = section.Paragraphs[i];
                    //}

                    //paraInserted.Format.LeftIndent = 30;
                    //TextSelection textSelection = new TextSelection(paraInserted, 0, paraInserted.Items.Count);

                    //doc.Replace("{TEST_NHANH}", textSelection, true, true);
                    foreach (Section section1 in doc.Sections)
                    {
                        for (int i = 0; i < section1.Body.ChildObjects.Count; i++)
                        {
                            if (section1.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
                            {
                                if (String.IsNullOrEmpty((section1.Body.ChildObjects[i] as Paragraph).Text.Trim()))
                                {
                                    section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[i]);
                                    i--;
                                }
                            }

                        }
                    }
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_KeHoachCuocKT_v0.1.docx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        //Chi tiết tab ngân sách
        [HttpGet("BudgetDetail/{id}")]
        public IActionResult BudgetDetail(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWork = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == id && a.IsDeleted.Equals(false));

                if (checkAuditWork != null)
                {
                    var _budget = new AuditBudgetDetailModel()
                    {
                        id = checkAuditWork.Id,
                        budget = checkAuditWork.Budget == null ? "" : checkAuditWork.Budget,
                    };
                    return Ok(new { code = "1", msg = "success", data = _budget });
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
        //Cập nhật tab ngân sách
        [HttpPut("BudgetUpdate")]
        public IActionResult BudgetUpdate([FromBody] AuditBudgetEditModel auditBudgetEditModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkBudget = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == auditBudgetEditModel.Id && a.IsDeleted.Equals(false));
                if (checkAuditWorkBudget == null) { return NotFound(); }

                checkAuditWorkBudget.Budget = auditBudgetEditModel.budget;
                _uow.Repository<AuditWork>().Update(checkAuditWorkBudget);
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        public class SystemParam
        {
            public int? id { get; set; }
            public string name { get; set; }
            public string value { get; set; }
        }
        //xuất word
        [HttpPost("ExportFileWordPlanOld/{id}")]
        public IActionResult ExportFileWordPlanOld(int id)
        {
            byte[] Bytes = null;
            var _paramInfoPrefix = "Param.SystemInfo";
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(x => x.Id == id);
                if (auditplan == null)
                {
                    return NotFound();
                }
                var iCache = (IConnectionMultiplexer)HttpContext.RequestServices
                      .GetService(typeof(IConnectionMultiplexer));
                if (!iCache.IsConnected)
                {
                    return BadRequest();
                }
                var redisDb = iCache.GetDatabase();
                var value_get = redisDb.StringGet(_paramInfoPrefix);
                var company = "";
                var hearderSystem = "";
                if (value_get.HasValue)
                {
                    var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
                    company = list_param.FirstOrDefault(a => a.name == "COMPANY_NAME")?.value;
                    hearderSystem = list_param.FirstOrDefault(a => a.name == "REPORT_HEADER")?.value;
                }
                //data
                var auditworkplan = _uow.Repository<AuditWorkPlan>().Include(a => a.AuditAssignmentPlan, a => a.AuditWorkScopePlan, a => a.AuditWorkScopePlanFacility).Where(a => a.auditplan_id == id && a.IsDeleted != true).ToArray();
                var users = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsDeleted != true).ToArray();
                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_KeHoachNam_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                var stream = new FileStream(fullPath, FileMode.Open);
                XWPFDocument doc = new XWPFDocument(stream);
                var now = DateTime.Now.Date;
                var tt = new
                {
                    ngay_bc_1 = now.Day,
                    thang_bc_1 = now.Month,
                    nam_bc_1 = now.Year,
                    nam_kh_1 = auditplan.Year,
                    thong_tin_khac_1 = !string.IsNullOrEmpty(auditplan.OtherInformation) ? Regex.Replace(HttpUtility.HtmlDecode(auditplan.OtherInformation), "<.*?>", string.Empty) : "",
                    ten_cong_ty_1 = company,
                };
                //Traverse paragraphs                  
                foreach (var para in doc.Paragraphs)
                {
                    ReplaceKey(para, tt);
                }

                //Add Row table
                var tables = doc.Tables;
                if (tables != null && tables.Count > 0)
                {
                    var oprTable = tables[0];
                    var oprTable2 = tables[2];
                    if (auditworkplan.Length > 0)
                    {
                        var i = 1;
                        var ii = 1;
                        foreach (var item in auditworkplan)
                        {
                            /// table 1
                            XWPFTableRow m_Row = oprTable.CreateRow();
                            m_Row.GetCell(0)?.SetText(i + "");

                            var Cell1 = m_Row.GetCell(1);
                            var parah1 = Cell1.AddParagraph();
                            if (Cell1.Paragraphs.Count() > 0)
                                Cell1.RemoveParagraph(0);
                            var run1 = parah1.CreateRun();
                            run1.SetText(item.Name);
                            run1.FontSize = 12;
                            run1.FontFamily = "Times New Roman";

                            var quy = item.StartDate.HasValue ? ConvertPrecious(item.StartDate.Value) : "";
                            var Cell2 = m_Row.GetCell(2);
                            var parah2 = Cell2.AddParagraph();
                            if (Cell2.Paragraphs.Count() > 0)
                                Cell2.RemoveParagraph(0);
                            var run2 = parah2.CreateRun();
                            run2.SetText(quy);
                            run2.FontSize = 12;
                            run2.FontFamily = "Times New Roman";

                            /// table 2
                            XWPFTableRow m_Row3 = oprTable2.CreateRow();
                            m_Row3.GetCell(0)?.SetText(ii + "");

                            var Cell11 = m_Row3.GetCell(1);
                            var parah11 = Cell11.AddParagraph();
                            if (Cell11.Paragraphs.Count() > 0)
                                Cell11.RemoveParagraph(0);
                            var run11 = parah11.CreateRun();
                            run11.SetText(item.Name);
                            run11.FontSize = 12;
                            run11.FontFamily = "Times New Roman";

                            var Cell22 = m_Row3.GetCell(2);
                            var parah22 = Cell22.AddParagraph();
                            if (Cell22.Paragraphs.Count() > 0)
                                Cell22.RemoveParagraph(0);
                            var run22 = parah22.CreateRun();
                            run22.SetText(item.Target);
                            run22.FontSize = 12;
                            run22.FontFamily = "Times New Roman";

                            var Cell3 = m_Row3.GetCell(3);
                            var parah3 = Cell3.AddParagraph();
                            if (Cell3.Paragraphs.Count() > 0)
                                Cell3.RemoveParagraph(0);
                            var run3 = parah3.CreateRun();
                            run3.SetText(item.AuditScope);
                            run3.FontSize = 12;
                            run3.FontFamily = "Times New Roman";

                            m_Row3.GetCell(4)?.SetText("");
                            var Cell4 = m_Row3.GetCell(4);

                            if (Cell4.Paragraphs.Count() > 0)
                                Cell4.RemoveParagraph(0);

                            var auditscopeFacility = item.AuditWorkScopePlanFacility.Where(x => x.IsDeleted != true).ToArray();
                            var auditscope = item.AuditWorkScopePlan.Where(x => x.IsDeleted != true).ToArray();
                            if (auditscopeFacility.Length == 0 && auditscope.Length == 0)
                            {
                                var parah4 = Cell4.AddParagraph();
                                var run4 = parah4.CreateRun();
                                run4.SetText("");
                                run4.FontSize = 12;
                                run4.FontFamily = "Times New Roman";
                            }
                            var index_f = 0;
                            foreach (var item_facility in auditscopeFacility)
                            {
                                if (index_f == 0)
                                {
                                    var parah4 = Cell4.AddParagraph();
                                    var run4 = parah4.CreateRun();
                                    run4.SetText("Đơn vị được kiểm toán:");
                                    run4.FontSize = 12;
                                    run4.FontFamily = "Times New Roman";
                                }
                                if (!string.IsNullOrEmpty(item_facility.auditfacilities_name))
                                {
                                    var parah44 = Cell4.AddParagraph();
                                    var run44 = parah44.CreateRun();
                                    run44.SetText("- " + item_facility.auditfacilities_name);
                                    run44.FontSize = 12;
                                    run44.FontFamily = "Times New Roman";
                                }

                                index_f++;
                            }

                            var index_p = 0;
                            foreach (var item_process in auditscope)
                            {
                                if (index_p == 0)
                                {
                                    var parah4 = Cell4.AddParagraph();
                                    var run4 = parah4.CreateRun();
                                    run4.SetText("Quy trình được kiểm toán:");
                                    run4.FontSize = 12;
                                    run4.FontFamily = "Times New Roman";
                                }
                                if (!string.IsNullOrEmpty(item_process.auditprocess_name))
                                {
                                    var parah44 = Cell4.AddParagraph();
                                    var run44 = parah44.CreateRun();
                                    run44.SetText("- " + item_process.auditprocess_name);
                                    run44.FontSize = 12;
                                    run44.FontFamily = "Times New Roman";
                                }

                                index_p++;
                            }

                            var assignment = "";
                            var lstassignment = item.AuditAssignmentPlan.Where(a => a.IsDeleted != true);
                            foreach (var item_asign in lstassignment)
                            {
                                assignment += item_asign.user_id.HasValue ? (users.FirstOrDefault(a => a.Id == item_asign.user_id)?.FullName + "\n" ?? "") : "";
                            }
                            if (!string.IsNullOrEmpty(assignment) && assignment.Contains("\n"))
                            {
                                String[] lines = assignment.Split("\n");
                                var Cell5 = m_Row3.GetCell(5);
                                if (Cell5.Paragraphs.Count() > 0)
                                    Cell5.RemoveParagraph(0);
                                foreach (String str in assignment.Split("\n".ToCharArray()))
                                {
                                    var parah = Cell5.AddParagraph();
                                    var run = parah.CreateRun();
                                    run.SetText(str);
                                    run.FontSize = 12;
                                    run.FontFamily = "Times New Roman";
                                }
                            }
                            else
                            {
                                var Cell5 = m_Row3.GetCell(5);
                                var parah = Cell5.AddParagraph();
                                if (Cell5.Paragraphs.Count() > 0)
                                    Cell5.RemoveParagraph(0);
                                var run = parah.CreateRun();
                                run.SetText(assignment);
                                run.FontSize = 12;
                                run.FontFamily = "Times New Roman";
                            }
                            i++;
                            ii++;
                        }
                    }
                    else
                    {
                        XWPFTableRow m_Row = oprTable.CreateRow();
                        m_Row.AddNewTableCell().SetText("");

                        XWPFTableRow m_Row2 = oprTable2.CreateRow();
                        m_Row2.AddNewTableCell().SetText("");
                    }
                }
                MemoryStream fs = new MemoryStream();
                fs.Position = 0;
                doc.Write(fs);
                Bytes = fs.ToArray();
                return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Grid.doc");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #region Download Word năm comment
        //[HttpPost("ExportFileWordPlan/{id}")]
        //public IActionResult ExportFileWordPlan(int id)
        //{
        //    //byte[] Bytes = null;
        //    //var _paramInfoPrefix = "Param.SystemInfo";
        //    try
        //    {
        //        if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
        //        {
        //            return Unauthorized();
        //        }
        //        #region comment code


        //        return MIC(id);
        //        #endregion

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest();
        //    }
        //}
        #endregion
        #region Download Word năm MIC
        [HttpPost("ExportFileWordPlanMIC/{id}")]
        public IActionResult ExportFileWordPlanMIC(int id)
        {
            byte[] Bytes = null;
            var _paramInfoPrefix = "Param.SystemInfo";
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(x => x.Id == id);
                if (auditplan == null)
                {
                    return NotFound();
                }
                var iCache = (IConnectionMultiplexer)HttpContext.RequestServices
                      .GetService(typeof(IConnectionMultiplexer));
                if (!iCache.IsConnected)
                {
                    return BadRequest();
                }
                var redisDb = iCache.GetDatabase();
                var value_get = redisDb.StringGet(_paramInfoPrefix);
                var company = "";
                var hearderSystem = "";
                if (value_get.HasValue)
                {
                    var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
                    company = list_param.FirstOrDefault(a => a.name == "COMPANY_NAME")?.value;
                    hearderSystem = list_param.FirstOrDefault(a => a.name == "REPORT_HEADER")?.value;
                }
                //data
                var auditworkplan = _uow.Repository<AuditWorkPlan>().Include(a => a.AuditAssignmentPlan, a => a.AuditWorkScopePlan, a => a.AuditWorkScopePlanFacility).Where(a => a.auditplan_id == id && a.IsDeleted != true).ToArray();
                var users = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsDeleted != true).ToArray();

                var now = DateTime.Now.Date;
                var tt = new
                {
                    ngay_bc_1 = now.Day + "",
                    thang_bc_1 = now.Month + "",
                    nam_bc_1 = now.Year + "",
                    nam_kh_1 = auditplan.Year + "",
                    thong_tin_khac_1 = "",
                    ten_cong_ty_1 = company,
                };
                //Export word
                var fullPathMIC = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_KeHoachNam_v0.1.docx");
                fullPathMIC = fullPathMIC.ToString().Replace("\\", "/");
                //var fullPathMIC = @"D:\test\Kitano_KeHoachNam_v0.1.docx";
                if (System.IO.File.Exists(fullPathMIC))
                {
                    Document doc = new Document(fullPathMIC);
                    var listsection = doc.Sections[0];
                    listsection.PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = listsection.HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem);

                    TextSelection selections = doc.FindAllString("thong_tin_khac_1", false, true)?.FirstOrDefault();
                    if (selections != null)
                    {
                        if (!string.IsNullOrEmpty(auditplan.OtherInformation))
                        {
                            var paragraph = selections?.GetAsOneRange()?.OwnerParagraph;
                            var object_arr = paragraph.ChildObjects;
                            if (object_arr.Count > 0)
                            {
                                for (int j = 0; j < object_arr.Count; j++)
                                {
                                    paragraph.ChildObjects.RemoveAt(j);
                                }
                                paragraph.AppendHTML(auditplan.OtherInformation);
                            }
                        }
                    }

                    doc.MailMerge.Execute(new string[] { "ngay_bc_1", "thang_bc_1", "nam_bc_1", "nam_kh_1", "ten_cong_ty_1" }, new string[] { tt.ngay_bc_1, tt.thang_bc_1, tt.nam_bc_1, tt.nam_kh_1, tt.ten_cong_ty_1 });
                    //doc.MailMerge.Execute(new string[] { "«»" }, new string[] { "" });
                    Table table1 = doc.Sections[0].Tables[0] as Table;
                    Table table2 = doc.Sections[1].Tables[0] as Table;
                    var i = 1;
                    var ii = 1;
                    foreach (var item in auditworkplan)
                    {
                        // Table 1
                        var row = table1.AddRow();
                        var txtTable1 = row.Cells[0].AddParagraph().AppendText(i + "");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        row.Cells[0].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable1 = row.Cells[1].AddParagraph().AppendText(item.Name);
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        row.Cells[1].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        var quy = item.StartDate.HasValue ? ConvertPrecious(item.StartDate.Value) : "";
                        txtTable1 = row.Cells[2].AddParagraph().AppendText(quy);
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        row.Cells[2].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        //tablw 2
                        var row2 = table2.AddRow();
                        var txtTable2 = row2.Cells[0].AddParagraph().AppendText(i + "");
                        txtTable2.CharacterFormat.FontName = "Times New Roman";
                        txtTable2.CharacterFormat.FontSize = 12;

                        txtTable2 = row2.Cells[1].AddParagraph().AppendText(item.Name);
                        txtTable2.CharacterFormat.FontName = "Times New Roman";
                        txtTable2.CharacterFormat.FontSize = 12;

                        txtTable2 = row2.Cells[2].AddParagraph().AppendText(item.Target);
                        txtTable2.CharacterFormat.FontName = "Times New Roman";
                        txtTable2.CharacterFormat.FontSize = 12;

                        txtTable2 = row2.Cells[3].AddParagraph().AppendText(item.AuditScope);
                        txtTable2.CharacterFormat.FontName = "Times New Roman";
                        txtTable2.CharacterFormat.FontSize = 12;

                        var auditscopeFacility = item.AuditWorkScopePlanFacility.Where(x => x.IsDeleted != true).ToArray();
                        var auditscope = item.AuditWorkScopePlan.Where(x => x.IsDeleted != true).ToArray();
                        var index_f = 0;
                        var Cell4 = row2.Cells[4];

                        foreach (var item_facility in auditscopeFacility)
                        {
                            if (index_f == 0)
                            {
                                var parah4 = Cell4.AddParagraph();
                                txtTable2 = parah4.AppendText("Đơn vị được kiểm toán:");
                                txtTable2.CharacterFormat.FontName = "Times New Roman";
                                txtTable2.CharacterFormat.FontSize = 12;
                            }
                            if (!string.IsNullOrEmpty(item_facility.auditfacilities_name))
                            {
                                var parah44 = Cell4.AddParagraph();
                                txtTable2 = parah44.AppendText("- " + item_facility.auditfacilities_name);
                                txtTable2.CharacterFormat.FontName = "Times New Roman";
                                txtTable2.CharacterFormat.FontSize = 12;
                            }
                            index_f++;

                            var index_p = 0;
                            foreach (var item_process in auditscope)
                            {
                                if (index_p == 0)
                                {
                                    var parah4 = Cell4.AddParagraph();
                                    txtTable2 = parah4.AppendText("Quy trình được kiểm toán:");
                                    txtTable2.CharacterFormat.FontName = "Times New Roman";
                                    txtTable2.CharacterFormat.FontSize = 12;
                                }
                                if (!string.IsNullOrEmpty(item_process.auditprocess_name))
                                {
                                    var parah44 = Cell4.AddParagraph();
                                    txtTable2 = parah44.AppendText("- " + item_process.auditprocess_name);
                                    txtTable2.CharacterFormat.FontName = "Times New Roman";
                                    txtTable2.CharacterFormat.FontSize = 12;
                                }

                                index_p++;
                            }
                        }

                        var assignment = "";
                        var lstassignment = item.AuditAssignmentPlan.Where(a => a.IsDeleted != true);
                        foreach (var item_asign in lstassignment)
                        {
                            assignment += item_asign.user_id.HasValue ? (users.FirstOrDefault(a => a.Id == item_asign.user_id)?.FullName + "\n" ?? "") : "";
                        }
                        if (!string.IsNullOrEmpty(assignment) && assignment.Contains("\n"))
                        {
                            String[] lines = assignment.Split("\n");
                            var Cell5 = row2.Cells[5];
                            foreach (String str in assignment.Split("\n".ToCharArray()))
                            {
                                var parah = Cell5.AddParagraph();
                                txtTable2 = parah.AppendText(str);
                                txtTable2.CharacterFormat.FontName = "Times New Roman";
                                txtTable2.CharacterFormat.FontSize = 12;
                            }
                        }

                        i++;
                        ii++;
                    }
                    foreach (Section section1 in doc.Sections)
                    {
                        for (int jj = 0; jj < section1.Body.ChildObjects.Count; jj++)
                        {
                            if (section1.Body.ChildObjects[jj].DocumentObjectType == DocumentObjectType.Paragraph)
                            {
                                if (String.IsNullOrEmpty((section1.Body.ChildObjects[jj] as Paragraph).Text.Trim()))
                                {
                                    section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[jj]);
                                    jj--;
                                }
                            }

                        }
                    }
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                }
                else
                {
                    return null;
                }
                return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Grid.doc");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion
        #region MIC
        //private IActionResult MIC(int id)
        //{
        //    byte[] Bytes = null;
        //    var _paramInfoPrefix = "Param.SystemInfo";
        //    var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(x => x.Id == id);
        //    if (auditplan == null)
        //    {
        //        return NotFound();
        //    }
        //    var iCache = (IConnectionMultiplexer)HttpContext.RequestServices
        //          .GetService(typeof(IConnectionMultiplexer));
        //    if (!iCache.IsConnected)
        //    {
        //        return BadRequest();
        //    }
        //    var redisDb = iCache.GetDatabase();
        //    var value_get = redisDb.StringGet(_paramInfoPrefix);
        //    var company = "";
        //    var hearderSystem = "";
        //    if (value_get.HasValue)
        //    {
        //        var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
        //        company = list_param.FirstOrDefault(a => a.name == "COMPANY_NAME")?.value;
        //        hearderSystem = list_param.FirstOrDefault(a => a.name == "REPORT_HEADER")?.value;
        //    }
        //    //data
        //    var auditworkplan = _uow.Repository<AuditWorkPlan>().Include(a => a.AuditAssignmentPlan, a => a.AuditWorkScopePlan, a => a.AuditWorkScopePlanFacility).Where(a => a.auditplan_id == id && a.IsDeleted != true).ToArray();
        //    var users = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsDeleted != true).ToArray();

        //    var now = DateTime.Now.Date;
        //    var tt = new
        //    {
        //        ngay_bc_1 = now.Day + "",
        //        thang_bc_1 = now.Month + "",
        //        nam_bc_1 = now.Year + "",
        //        nam_kh_1 = auditplan.Year + "",
        //        thong_tin_khac_1 = "",
        //        ten_cong_ty_1 = company,
        //    };
        //    //Export word
        //    var fullPathMIC = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_KeHoachNam_v0.1.docx");
        //    fullPathMIC = fullPathMIC.ToString().Replace("\\", "/");
        //    //var fullPathMIC = @"D:\test\Kitano_KeHoachNam_v0.1.docx";
        //    if (System.IO.File.Exists(fullPathMIC))
        //    {
        //        Document doc = new Document(fullPathMIC);
        //        var listsection = doc.Sections[0];
        //        listsection.PageSetup.DifferentFirstPageHeaderFooter = true;
        //        Paragraph paragraph_header = listsection.HeadersFooters.FirstPageHeader.AddParagraph();
        //        paragraph_header.AppendHTML(hearderSystem);

        //        TextSelection selections = doc.FindAllString("thong_tin_khac_1", false, true)?.FirstOrDefault();
        //        if (selections != null)
        //        {
        //            if (!string.IsNullOrEmpty(auditplan.OtherInformation))
        //            {
        //                var paragraph = selections?.GetAsOneRange()?.OwnerParagraph;
        //                var object_arr = paragraph.ChildObjects;
        //                if (object_arr.Count > 0)
        //                {
        //                    for (int j = 0; j < object_arr.Count; j++)
        //                    {
        //                        paragraph.ChildObjects.RemoveAt(j);
        //                    }
        //                    paragraph.AppendHTML(auditplan.OtherInformation);
        //                }
        //            }
        //        }

        //        doc.MailMerge.Execute(new string[] { "ngay_bc_1", "thang_bc_1", "nam_bc_1", "nam_kh_1", "ten_cong_ty_1" }, new string[] { tt.ngay_bc_1, tt.thang_bc_1, tt.nam_bc_1, tt.nam_kh_1, tt.ten_cong_ty_1 });
        //        //doc.MailMerge.Execute(new string[] { "«»" }, new string[] { "" });
        //        Table table1 = doc.Sections[0].Tables[0] as Table;
        //        Table table2 = doc.Sections[1].Tables[0] as Table;
        //        var i = 1;
        //        var ii = 1;
        //        foreach (var item in auditworkplan)
        //        {
        //            // Table 1
        //            var row = table1.AddRow();
        //            var txtTable1 = row.Cells[0].AddParagraph().AppendText(i + "");
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 12;
        //            row.Cells[0].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable1 = row.Cells[1].AddParagraph().AppendText(item.Name);
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 12;
        //            row.Cells[1].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            var quy = item.StartDate.HasValue ? ConvertPrecious(item.StartDate.Value) : "";
        //            txtTable1 = row.Cells[2].AddParagraph().AppendText(quy);
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 12;
        //            row.Cells[2].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            //tablw 2
        //            var row2 = table2.AddRow();
        //            var txtTable2 = row2.Cells[0].AddParagraph().AppendText(i + "");
        //            txtTable2.CharacterFormat.FontName = "Times New Roman";
        //            txtTable2.CharacterFormat.FontSize = 12;

        //            txtTable2 = row2.Cells[1].AddParagraph().AppendText(item.Name);
        //            txtTable2.CharacterFormat.FontName = "Times New Roman";
        //            txtTable2.CharacterFormat.FontSize = 12;

        //            txtTable2 = row2.Cells[2].AddParagraph().AppendText(item.Target);
        //            txtTable2.CharacterFormat.FontName = "Times New Roman";
        //            txtTable2.CharacterFormat.FontSize = 12;

        //            txtTable2 = row2.Cells[3].AddParagraph().AppendText(item.AuditScope);
        //            txtTable2.CharacterFormat.FontName = "Times New Roman";
        //            txtTable2.CharacterFormat.FontSize = 12;

        //            var auditscopeFacility = item.AuditWorkScopePlanFacility.Where(x => x.IsDeleted != true).ToArray();
        //            var auditscope = item.AuditWorkScopePlan.Where(x => x.IsDeleted != true).ToArray();
        //            var index_f = 0;
        //            var Cell4 = row2.Cells[4];

        //            foreach (var item_facility in auditscopeFacility)
        //            {
        //                if (index_f == 0)
        //                {
        //                    var parah4 = Cell4.AddParagraph();
        //                    txtTable2 = parah4.AppendText("Đơn vị được kiểm toán:");
        //                    txtTable2.CharacterFormat.FontName = "Times New Roman";
        //                    txtTable2.CharacterFormat.FontSize = 12;
        //                }
        //                if (!string.IsNullOrEmpty(item_facility.auditfacilities_name))
        //                {
        //                    var parah44 = Cell4.AddParagraph();
        //                    txtTable2 = parah44.AppendText("- " + item_facility.auditfacilities_name);
        //                    txtTable2.CharacterFormat.FontName = "Times New Roman";
        //                    txtTable2.CharacterFormat.FontSize = 12;
        //                }
        //                index_f++;

        //                var index_p = 0;
        //                foreach (var item_process in auditscope)
        //                {
        //                    if (index_p == 0)
        //                    {
        //                        var parah4 = Cell4.AddParagraph();
        //                        txtTable2 = parah4.AppendText("Quy trình được kiểm toán:");
        //                        txtTable2.CharacterFormat.FontName = "Times New Roman";
        //                        txtTable2.CharacterFormat.FontSize = 12;
        //                    }
        //                    if (!string.IsNullOrEmpty(item_process.auditprocess_name))
        //                    {
        //                        var parah44 = Cell4.AddParagraph();
        //                        txtTable2 = parah44.AppendText("- " + item_process.auditprocess_name);
        //                        txtTable2.CharacterFormat.FontName = "Times New Roman";
        //                        txtTable2.CharacterFormat.FontSize = 12;
        //                    }

        //                    index_p++;
        //                }
        //            }

        //            var assignment = "";
        //            var lstassignment = item.AuditAssignmentPlan.Where(a => a.IsDeleted != true);
        //            foreach (var item_asign in lstassignment)
        //            {
        //                assignment += item_asign.user_id.HasValue ? (users.FirstOrDefault(a => a.Id == item_asign.user_id)?.FullName + "\n" ?? "") : "";
        //            }
        //            if (!string.IsNullOrEmpty(assignment) && assignment.Contains("\n"))
        //            {
        //                String[] lines = assignment.Split("\n");
        //                var Cell5 = row2.Cells[5];
        //                foreach (String str in assignment.Split("\n".ToCharArray()))
        //                {
        //                    var parah = Cell5.AddParagraph();
        //                    txtTable2 = parah.AppendText(str);
        //                    txtTable2.CharacterFormat.FontName = "Times New Roman";
        //                    txtTable2.CharacterFormat.FontSize = 12;
        //                }
        //            }

        //            i++;
        //            ii++;
        //        }
        //        foreach (Section section1 in doc.Sections)
        //        {
        //            for (int jj = 0; jj < section1.Body.ChildObjects.Count; jj++)
        //            {
        //                if (section1.Body.ChildObjects[jj].DocumentObjectType == DocumentObjectType.Paragraph)
        //                {
        //                    if (String.IsNullOrEmpty((section1.Body.ChildObjects[jj] as Paragraph).Text.Trim()))
        //                    {
        //                        section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[jj]);
        //                        jj--;
        //                    }
        //                }

        //            }
        //        }
        //        MemoryStream stream = new MemoryStream();
        //        stream.Position = 0;
        //        doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
        //        Bytes = stream.ToArray();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Grid.doc");
        //}
        #endregion
        #region Download Word năm MBS
        [HttpPost("ExportFileWordPlanMBS/{id}")]
        public IActionResult ExportFileWordPlanMBS(int id)
        {
            byte[] Bytes = null;
            var _paramInfoPrefix = "Param.SystemInfo";
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(x => x.Id == id);
                if (auditplan == null)
                {
                    return NotFound();
                }
                var iCache = (IConnectionMultiplexer)HttpContext.RequestServices
                      .GetService(typeof(IConnectionMultiplexer));
                if (!iCache.IsConnected)
                {
                    return BadRequest();
                }
                var redisDb = iCache.GetDatabase();
                var value_get = redisDb.StringGet(_paramInfoPrefix);
                var company = "";
                var hearderSystem = "";
                if (value_get.HasValue)
                {
                    var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
                    company = list_param.FirstOrDefault(a => a.name == "COMPANY_NAME")?.value;
                    hearderSystem = list_param.FirstOrDefault(a => a.name == "REPORT_HEADER")?.value;
                }
                //data
                var auditworkplan = _uow.Repository<AuditWorkPlan>().Include(a => a.AuditAssignmentPlan, a => a.AuditWorkScopePlan, a => a.AuditWorkScopePlanFacility).Where(a => a.auditplan_id == id && a.IsDeleted != true).ToArray();
                var users = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsDeleted != true).ToArray();

                var now = DateTime.Now.Date;
                var tt = new
                {
                    ngay_bc_1 = now.Day + "",
                    thang_bc_1 = now.Month + "",
                    nam_bc_1 = now.Year + "",
                    nam_kh_1 = auditplan.Year + "",
                    thong_tin_khac_1 = "",
                    ten_cong_ty_1 = company,
                };
                //Document doc = new Document(fullPath);
                // Export word
                var fullPathMBS = Path.Combine(_config["Template:AuditDocsTemplate"], "MBS_Kitano_KeHoachNam_v0.1.docx");
                fullPathMBS = fullPathMBS.ToString().Replace("\\", "/");
                //var fullPathMBS = @"D:\test\MBS_Kitano_KeHoachNam_v0.1.docx";
                if (System.IO.File.Exists(fullPathMBS))
                {
                    Document docMBS = new Document(fullPathMBS);
                    var listsection = docMBS.Sections[0];
                    listsection.PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = listsection.HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem);
                    //Remove header 2
                    docMBS.Sections[1].HeadersFooters.Header.ChildObjects.Clear();
                    //Add line breaks
                    Paragraph headerParagraph = docMBS.Sections[1].HeadersFooters.FirstPageHeader.AddParagraph();
                    headerParagraph.AppendBreak(BreakType.LineBreak);
                    docMBS.MailMerge.Execute(new string[] { "ngay_bc_1", "thang_bc_1", "nam_bc_1", "nam_kh_1" }, new string[] { tt.ngay_bc_1, tt.thang_bc_1, tt.nam_bc_1, tt.nam_kh_1 });
                    Table table1 = docMBS.Sections[0].Tables[0] as Table;
                    var i = 1;
                    foreach (var item in auditworkplan)
                    {
                        // Table 1
                        var row = table1.AddRow();
                        var txtTable1 = row.Cells[0].AddParagraph().AppendText(i + "");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        row.Cells[0].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable1 = row.Cells[1].AddParagraph().AppendText(item.Name);
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        row.Cells[1].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable1 = row.Cells[2].AddParagraph().AppendText(item.AuditScope);
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        row.Cells[2].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        var quy = item.StartDate.HasValue ? ConvertPrecious(item.StartDate.Value) : "";
                        txtTable1 = row.Cells[3].AddParagraph().AppendText(quy);
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        row.Cells[3].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        i++;
                    }
                    if (auditplan.OtherInformation != null)
                    {
                        Paragraph paraOther = docMBS.Sections[1].AddParagraph();// para cuar Editor
                        paraOther.AppendHTML(auditplan.OtherInformation);
                    }
                    var RatingScaleList = _uow.Repository<RatingScale>().GetAll().Where(a => a.ApplyFor == "HDKD"
                    && a.Status == true
                    && a.Deleted == false).ToArray();
                    Table table2 = docMBS.Sections[2].Tables[0] as Table;
                    foreach (var item in RatingScaleList)
                    {
                        var row = table2.AddRow();
                        var txtTable2 = row.Cells[0].AddParagraph().AppendText(item.RiskLevelName);
                        txtTable2.CharacterFormat.FontName = "Times New Roman";
                        txtTable2.CharacterFormat.FontSize = 12;
                        row.Cells[0].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable2 = row.Cells[1].AddParagraph().AppendText(item.Min + (item.MinFunction == "gt" ? " < " : " <= ") + " Điểm " + (item.Max != null ? (item.MinFunction == "lt" ? " < " : " <= ") + item.Max : ""));
                        txtTable2.CharacterFormat.FontName = "Times New Roman";
                        txtTable2.CharacterFormat.FontSize = 12;
                        row.Cells[1].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                    }
                    var yearDG = (auditplan != null ? (auditplan.Year) : 0);
                    var dataKQRR = _uow.Repository<ScoreBoard>().Include(a => a.AssessmentResult/*, a => a.SystemCategory*/).Where(a => a.Year == yearDG && a.Deleted != true && a.Stage == 1).ToArray();

                    Table table3 = docMBS.Sections[2].Tables[1] as Table;
                    foreach (var item in dataKQRR)
                    {
                        var dtSystemCategory = _uow.Repository<SystemCategory>().FirstOrDefault(x => x.Code == item.ApplyFor);
                        var dtAuditReason = _uow.Repository<SystemCategory>().FirstOrDefault(x => x.Id == item.AssessmentResult.Select(x => x.AuditReason).FirstOrDefault());
                        var date = (item.AssessmentResult.Select(x => x.LastAudit).FirstOrDefault() == null ? "" : item.AssessmentResult.Select(x => x.LastAudit).FirstOrDefault().Value.ToString("MM/yyyy"));
                        var _audit = (item.AssessmentResult.Select(x => x.Audit).FirstOrDefault() == true ? "Có" : "Không");
                        var row = table3.AddRow();
                        var txtTable3 = row.Cells[0].AddParagraph().AppendText(item.ObjectName);
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        row.Cells[0].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable3 = row.Cells[1].AddParagraph().AppendText(dtSystemCategory.Name);
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        row.Cells[1].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable3 = row.Cells[2].AddParagraph().AppendText(item.Point.ToString());
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        row.Cells[2].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable3 = row.Cells[3].AddParagraph().AppendText(item.RiskLevel);
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        row.Cells[3].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable3 = row.Cells[4].AddParagraph().AppendText(item.AssessmentResult.Select(x => x.RiskLevelChangeName).FirstOrDefault());
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        row.Cells[4].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable3 = row.Cells[5].AddParagraph().AppendText(item.AssessmentResult.Select(x => x.LastRiskLevel).FirstOrDefault());
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        row.Cells[5].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable3 = row.Cells[6].AddParagraph().AppendText(date);
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        row.Cells[6].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable3 = row.Cells[7].AddParagraph().AppendText(item.AuditCycleName);
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        row.Cells[7].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable3 = row.Cells[8].AddParagraph().AppendText(_audit);
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        row.Cells[8].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                        txtTable3 = row.Cells[9].AddParagraph().AppendText(dtAuditReason == null ? "" : dtAuditReason.Name);
                        txtTable3.CharacterFormat.FontName = "Times New Roman";
                        txtTable3.CharacterFormat.FontSize = 12;
                        row.Cells[9].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                    }
                    //foreach (Section section1 in docMBS.Sections)
                    //{
                    //    for (int x = 0; x < section1.Body.ChildObjects.Count; x++)
                    //    {
                    //        if (section1.Body.ChildObjects[x].DocumentObjectType == DocumentObjectType.Paragraph)
                    //        {
                    //            if (String.IsNullOrEmpty((section1.Body.ChildObjects[x] as Paragraph).Text.Trim()))
                    //            {
                    //                section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[x]);
                    //                x--;
                    //            }
                    //        }

                    //    }
                    //}
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    docMBS.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                }
                else
                {
                    return null;
                }
                return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Grid.doc");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion
        #region MBS
        //private IActionResult MBS(int id)
        //{
        //    byte[] Bytes = null;
        //    var _paramInfoPrefix = "Param.SystemInfo";
        //    var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(x => x.Id == id);
        //    if (auditplan == null)
        //    {
        //        return NotFound();
        //    }
        //    var iCache = (IConnectionMultiplexer)HttpContext.RequestServices
        //          .GetService(typeof(IConnectionMultiplexer));
        //    if (!iCache.IsConnected)
        //    {
        //        return BadRequest();
        //    }
        //    var redisDb = iCache.GetDatabase();
        //    var value_get = redisDb.StringGet(_paramInfoPrefix);
        //    var company = "";
        //    var hearderSystem = "";
        //    if (value_get.HasValue)
        //    {
        //        var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
        //        company = list_param.FirstOrDefault(a => a.name == "COMPANY_NAME")?.value;
        //        hearderSystem = list_param.FirstOrDefault(a => a.name == "REPORT_HEADER")?.value;
        //    }
        //    //data
        //    var auditworkplan = _uow.Repository<AuditWorkPlan>().Include(a => a.AuditAssignmentPlan, a => a.AuditWorkScopePlan, a => a.AuditWorkScopePlanFacility).Where(a => a.auditplan_id == id && a.IsDeleted != true).ToArray();
        //    var users = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsDeleted != true).ToArray();

        //    var now = DateTime.Now.Date;
        //    var tt = new
        //    {
        //        ngay_bc_1 = now.Day + "",
        //        thang_bc_1 = now.Month + "",
        //        nam_bc_1 = now.Year + "",
        //        nam_kh_1 = auditplan.Year + "",
        //        thong_tin_khac_1 = "",
        //        ten_cong_ty_1 = company,
        //    };
        //    //Document doc = new Document(fullPath);
        //    // Export word
        //    var fullPathMBS = Path.Combine(_config["Template:AuditDocsTemplate"], "MBS_Kitano_KeHoachNam_v0.1.docx");
        //    fullPathMBS = fullPathMBS.ToString().Replace("\\", "/");
        //    //var fullPathMBS = @"D:\test\MBS_Kitano_KeHoachNam_v0.1.docx";
        //    if (System.IO.File.Exists(fullPathMBS))
        //    {
        //        Document docMBS = new Document(fullPathMBS);
        //        var listsection = docMBS.Sections[0];
        //        listsection.PageSetup.DifferentFirstPageHeaderFooter = true;
        //        Paragraph paragraph_header = listsection.HeadersFooters.FirstPageHeader.AddParagraph();
        //        paragraph_header.AppendHTML(hearderSystem);
        //        docMBS.MailMerge.Execute(new string[] { "ngay_bc_1", "thang_bc_1", "nam_bc_1", "nam_kh_1" }, new string[] { tt.ngay_bc_1, tt.thang_bc_1, tt.nam_bc_1, tt.nam_kh_1 });
        //        Table table1 = docMBS.Sections[0].Tables[0] as Table;
        //        var i = 1;
        //        foreach (var item in auditworkplan)
        //        {
        //            // Table 1
        //            var row = table1.AddRow();
        //            var txtTable1 = row.Cells[0].AddParagraph().AppendText(i + "");
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 12;
        //            row.Cells[0].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable1 = row.Cells[1].AddParagraph().AppendText(item.Name);
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 12;
        //            row.Cells[1].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable1 = row.Cells[2].AddParagraph().AppendText(item.AuditScope);
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 12;
        //            row.Cells[2].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            var quy = item.StartDate.HasValue ? ConvertPrecious(item.StartDate.Value) : "";
        //            txtTable1 = row.Cells[3].AddParagraph().AppendText(quy);
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 12;
        //            row.Cells[3].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            i++;
        //        }
        //        if (auditplan.OtherInformation != null)
        //        {
        //            Paragraph paraOther = docMBS.Sections[0].AddParagraph();// para cuar Editor
        //            paraOther.AppendHTML(auditplan.OtherInformation);
        //        }
        //        var RatingScaleList = _uow.Repository<RatingScale>().GetAll().Where(a => a.ApplyFor == "HDKD"
        //        && a.Status == true
        //        && a.Deleted == false).ToArray();
        //        Table table2 = docMBS.Sections[1].Tables[0] as Table;
        //        foreach (var item in RatingScaleList)
        //        {
        //            var row = table2.AddRow();
        //            var txtTable2 = row.Cells[0].AddParagraph().AppendText(item.RiskLevelName);
        //            txtTable2.CharacterFormat.FontName = "Times New Roman";
        //            txtTable2.CharacterFormat.FontSize = 12;
        //            row.Cells[0].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable2 = row.Cells[1].AddParagraph().AppendText(item.Min + (item.MinFunction == "gt" ? " < " : " <= ") + " Điểm " + (item.Max != null ? (item.MinFunction == "lt" ? " < " : " <= ") + item.Max : ""));
        //            txtTable2.CharacterFormat.FontName = "Times New Roman";
        //            txtTable2.CharacterFormat.FontSize = 12;
        //            row.Cells[1].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");
        //        }
        //        var yearDG = (auditplan != null ? (auditplan.Year - 1) : 0);
        //        var dataKQRR = _uow.Repository<ScoreBoard>().Include(a => a.AssessmentResult/*, a => a.SystemCategory*/).Where(a => a.Year == yearDG).ToArray();

        //        Table table3 = docMBS.Sections[1].Tables[1] as Table;
        //        foreach (var item in dataKQRR)
        //        {
        //            var dtSystemCategory = _uow.Repository<SystemCategory>().FirstOrDefault(x => x.Code == item.ApplyFor);
        //            var dtAuditReason = _uow.Repository<SystemCategory>().FirstOrDefault(x => x.Id == item.AssessmentResult.Select(x => x.AuditReason).FirstOrDefault());
        //            var date = (item.AssessmentResult.Select(x => x.LastAudit).FirstOrDefault() == null ? "" : item.AssessmentResult.Select(x => x.LastAudit).FirstOrDefault().Value.ToString("MM/yyyy"));
        //            var _audit = (item.AssessmentResult.Select(x => x.Audit).FirstOrDefault() == true ? "Có" : "Không");
        //            var row = table3.AddRow();
        //            var txtTable3 = row.Cells[0].AddParagraph().AppendText(item.ObjectName);
        //            txtTable3.CharacterFormat.FontName = "Times New Roman";
        //            txtTable3.CharacterFormat.FontSize = 12;
        //            row.Cells[0].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable3 = row.Cells[1].AddParagraph().AppendText(dtSystemCategory.Name);
        //            txtTable3.CharacterFormat.FontName = "Times New Roman";
        //            txtTable3.CharacterFormat.FontSize = 12;
        //            row.Cells[1].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable3 = row.Cells[2].AddParagraph().AppendText(item.Point.ToString());
        //            txtTable3.CharacterFormat.FontName = "Times New Roman";
        //            txtTable3.CharacterFormat.FontSize = 12;
        //            row.Cells[2].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable3 = row.Cells[3].AddParagraph().AppendText(item.RiskLevel);
        //            txtTable3.CharacterFormat.FontName = "Times New Roman";
        //            txtTable3.CharacterFormat.FontSize = 12;
        //            row.Cells[3].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable3 = row.Cells[4].AddParagraph().AppendText(item.AssessmentResult.Select(x => x.RiskLevelChangeName).FirstOrDefault());
        //            txtTable3.CharacterFormat.FontName = "Times New Roman";
        //            txtTable3.CharacterFormat.FontSize = 12;
        //            row.Cells[4].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable3 = row.Cells[5].AddParagraph().AppendText(item.AssessmentResult.Select(x => x.LastRiskLevel).FirstOrDefault());
        //            txtTable3.CharacterFormat.FontName = "Times New Roman";
        //            txtTable3.CharacterFormat.FontSize = 12;
        //            row.Cells[5].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable3 = row.Cells[6].AddParagraph().AppendText(date);
        //            txtTable3.CharacterFormat.FontName = "Times New Roman";
        //            txtTable3.CharacterFormat.FontSize = 12;
        //            row.Cells[6].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable3 = row.Cells[7].AddParagraph().AppendText(item.AuditCycleName);
        //            txtTable3.CharacterFormat.FontName = "Times New Roman";
        //            txtTable3.CharacterFormat.FontSize = 12;
        //            row.Cells[7].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable3 = row.Cells[8].AddParagraph().AppendText(_audit);
        //            txtTable3.CharacterFormat.FontName = "Times New Roman";
        //            txtTable3.CharacterFormat.FontSize = 12;
        //            row.Cells[8].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

        //            txtTable3 = row.Cells[9].AddParagraph().AppendText(dtAuditReason == null ? "" : dtAuditReason.Name);
        //            txtTable3.CharacterFormat.FontName = "Times New Roman";
        //            txtTable3.CharacterFormat.FontSize = 12;
        //            row.Cells[9].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");
        //        }
        //        MemoryStream stream = new MemoryStream();
        //        stream.Position = 0;
        //        docMBS.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
        //        Bytes = stream.ToArray();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Grid.doc");
        //}
        #endregion
        [HttpPost("ExportFileWordPlanMCREDIT/{id}")]
        public IActionResult ExportFileWordPlanMCREDIT(int id)
        {
            var fullPathMCREDIT = Path.Combine(_config["Template:AuditDocsTemplate"], "MCredit_Kitano_KeHoachNam_v0.1.docx");
            fullPathMCREDIT = fullPathMCREDIT.ToString().Replace("\\", "/");
            byte[] Bytes = null;
            var _paramInfoPrefix = "Param.SystemInfo";
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var auditplan = _uow.Repository<AuditPlan>().FirstOrDefault(x => x.Id == id);


                if (auditplan == null)
                {
                    return NotFound();
                }
                var iCache = (IConnectionMultiplexer)HttpContext.RequestServices
                      .GetService(typeof(IConnectionMultiplexer));
                if (!iCache.IsConnected)
                {
                    return BadRequest();
                }

                var auditwork = _uow.Repository<AuditWorkPlan>().Include(a => a.AuditAssignmentPlan, a => a.AuditWorkScopePlan, a => a.AuditWorkScopePlanFacility).Where(a => a.auditplan_id == id && a.IsDeleted != true).OrderBy(x => x.StartDate).ToArray();
                var auditWorkScope = _uow.Repository<AuditWorkScopePlan>().Find(a => a.Year == auditplan.Year && a.IsDeleted != true).ToArray();
                var AllscoreBoard = _uow.Repository<ScoreBoard>().Find(x => x.ApplyFor == "HDKD" && x.Stage == 1 && x.Deleted != true).ToArray();
                var scoreBoardCurYear = AllscoreBoard.Where(x => x.Year == auditplan.Year && x.ApplyFor == "HDKD" && x.Deleted != true).OrderByDescending(x => x.CreateDate).ToArray();
                var scoreBoardPrevYear = AllscoreBoard.Where(x => x.Year == auditplan.Year - 1 && x.ApplyFor == "HDKD"  && x.Deleted != true).OrderByDescending(x=>x.CreateDate).ToArray();
                var allScoreBoardId = AllscoreBoard.Select(x => x.ID.ToString()).ToArray();
                var allScoreBoardIdPrevYear = scoreBoardPrevYear.Select(x => x.ID.ToString()).ToArray();
                var assesmentResult = _uow.Repository<AssessmentResult>().Find(x => allScoreBoardIdPrevYear.Contains(x.ScoreBoardId.ToString()) && x.Deleted != true).ToArray();
                var reportAuditWork = _uow.Repository<ReportAuditWork>().Include(x => x.AuditWork, x => x.AuditWork.AuditWorkScope).Where(x => x.IsDeleted != true).ToArray();
                var ratings = _uow.Repository<SystemCategory>().Find(x => x.ParentGroup == "MucXepHangKiemToan" && x.Deleted != true).ToArray();
                var auditReasons = _uow.Repository<SystemCategory>().Find(x => x.ParentGroup == "LyDoKiemToan" && x.Deleted != true).ToArray();
                var curentScoreboard = _uow.Repository<ScoreBoard>().Find(x => x.Year == auditplan.Year && x.ApplyFor == "HDKD" && x.Stage == 1 && x.Deleted != true).ToArray();

                var redisDb = iCache.GetDatabase();
                var value_get = redisDb.StringGet(_paramInfoPrefix);
                var company = "";
                var hearderSystem = "";
                if (value_get.HasValue)
                {
                    var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
                    company = list_param.FirstOrDefault(a => a.name == "COMPANY_NAME")?.value;
                    hearderSystem = list_param.FirstOrDefault(a => a.name == "REPORT_HEADER")?.value;
                }

                var users = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsDeleted != true).ToArray();
                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "MCredit_Kitano_KeHoachNam_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                var now = DateTime.Now.Date;
                var tt = new
                {
                    cur_day_vi = now.Day + "",
                    cur_month_vi = now.Month + "",
                    cur_year_vi = now.Year + "",
                    cur_month_en = now.ToString("MMMM"),
                    cur_day_en = now.Day + "",
                    cur_year_en = now.Year + "",
                    au_plan_year = auditplan.Year + "",
                    au_count_in_year = auditwork.Count() + "",
                };
                Document doc = new Document(fullPath);

                string font = "Be Vietnam Pro";
                var listsection = doc.Sections[0];
                listsection.PageSetup.DifferentFirstPageHeaderFooter = true;
                Paragraph paragraph_header = listsection.HeadersFooters.FirstPageHeader.AddParagraph();
                paragraph_header.AppendHTML(hearderSystem);
                doc.MailMerge.Execute(new string[] { "cur_day_vi", "cur_month_vi", "cur_year_vi", "cur_month_en", "cur_day_en", "cur_year_en", "au_plan_year", "au_count_in_year" },
                                        new string[] { tt.cur_day_vi, tt.cur_month_vi, tt.cur_year_vi, tt.cur_month_en, tt.cur_day_en, tt.cur_year_en, tt.au_plan_year, tt.au_count_in_year });

                Table table1 = doc.Sections[0].Tables[2] as Table;
                Table table2 = doc.Sections[1].Tables[0] as Table;
                Table table3 = doc.Sections[2].Tables[0] as Table;
                var i = 1;
                var auditWorkScopePlanIndex = 1;

                foreach (var item in auditwork)
                {
                    // Table 1
                    var row = table1.AddRow();
                    row.RowFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                    var quy = item.StartDate.HasValue ? QuaterNumberToRomanNumerals(item.StartDate.Value.Month) : "";
                    var txtTable1 = row.Cells[0].AddParagraph().AppendText("Quý " + quy + "/ Quarter " + quy);
                    txtTable1.CharacterFormat.FontName = font;
                    txtTable1.CharacterFormat.FontSize = 10;
                    row.Cells[0].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                    txtTable1 = row.Cells[1].AddParagraph().AppendText(item.Name);
                    txtTable1.CharacterFormat.FontName = font;
                    txtTable1.CharacterFormat.FontSize = 10;
                    row.Cells[1].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                    var auditscopeFacility = item.AuditWorkScopePlanFacility.Where(x => x.IsDeleted != true).ToArray();
                    var auditscopePlan = item.AuditWorkScopePlan.Where(x => x.IsDeleted != true).ToArray();

                    var auditscopeFacilityNames = auditscopeFacility.Select(x => x.auditfacilities_name).ToArray();
                    var auditscopePlanActivity = item.AuditWorkScopePlan.Where(x => x.IsDeleted != true && x.BoardId != null ? allScoreBoardId.Contains(x.BoardId.ToString()) : false).ToArray();
                    List<string> auditscopePlanActivityRiskName = new List<string>();
                    foreach (var activityPlan in auditscopePlanActivity)
                    {
                        var score = scoreBoardPrevYear.Where(x => x.ObjectId == activityPlan.bussinessactivities_id).FirstOrDefault();
                        var result = assesmentResult.Where(x => x.ScoreBoardId == score.ID).FirstOrDefault();
                        auditscopePlanActivityRiskName.Add((result != null &&  !string.IsNullOrEmpty(result.RiskLevelChangeName)) ? result.RiskLevelChangeName :(score != null && !string.IsNullOrEmpty(score.RiskLevel)) ? score.RiskLevel : "Chưa có đánh giá");
                    }

                    var auditscope = item.AuditWorkScopePlan.Where(x => x.IsDeleted != true).ToArray();

                    row = table2.AddRow();
                    row.RowFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                    var txtTable2 = row.Cells[0].AddParagraph().AppendText(i + ".");
                    txtTable2.CharacterFormat.FontName = font;
                    txtTable2.CharacterFormat.FontSize = 9;
                    row.Cells[0].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                    txtTable2 = row.Cells[1].AddParagraph().AppendText(item.Name);
                    txtTable2.CharacterFormat.FontName = font;
                    txtTable2.CharacterFormat.FontSize = 9;
                    row.Cells[1].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                    txtTable2 = row.Cells[2].AddParagraph().AppendText(item.Target);
                    txtTable2.CharacterFormat.FontName = font;
                    txtTable2.CharacterFormat.FontSize = 9;
                    row.Cells[2].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                    txtTable2 = row.Cells[3].AddParagraph().AppendText(item.AuditScope);
                    txtTable2.CharacterFormat.FontName = font;
                    txtTable2.CharacterFormat.FontSize = 9;
                    row.Cells[3].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");


                    if (auditscopeFacilityNames != null)
                    {
                        var name = string.Join(", ", auditscopeFacilityNames);
                        txtTable2 = row.Cells[4].AddParagraph().AppendText(name);
                        txtTable2.CharacterFormat.FontName = font;
                        txtTable2.CharacterFormat.FontSize = 9;
                        row.Cells[4].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                    }

                    var partxtTable2 = row.Cells[5].AddParagraph();
                    txtTable2 = partxtTable2.AppendText(item.NumOfAuditor.ToString());
                    txtTable2.CharacterFormat.FontName = font;
                    txtTable2.CharacterFormat.FontSize = 9;
                    partxtTable2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                    row.Cells[5].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                    if (auditscopePlanActivityRiskName != null)
                    {
                        partxtTable2 = row.Cells[6].AddParagraph();
                        txtTable2 = partxtTable2.AppendText(string.Join(", ", auditscopePlanActivityRiskName));
                        txtTable2.CharacterFormat.FontName = font;
                        txtTable2.CharacterFormat.FontSize = 9;
                        row.Cells[6].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                        partxtTable2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                    }

                    txtTable2 = row.Cells[7].AddParagraph().AppendText("");
                    txtTable2.CharacterFormat.FontName = font;
                    txtTable2.CharacterFormat.FontSize = 9;
                    row.Cells[7].CellFormat.BackColor = ColorTranslator.FromHtml("#FFFFFF");

                    i++;
                }

                foreach (var score in scoreBoardPrevYear)
                {
                    var result = assesmentResult.Where(x => (score != null) ? x.ScoreBoardId == score.ID : false && x.Deleted != true).FirstOrDefault();
                    var report = reportAuditWork.Where(x => x.AuditWork.AuditWorkScope.Any(y => y.bussinessactivities_id == score.ObjectId && allScoreBoardId.Contains(y.BoardId.ToString()))).OrderByDescending(x => x.EndDate).FirstOrDefault();
                    var rate = ratings.Where(x => (report != null && report?.AuditRatingLevelTotal != null) ? x.Code == report.AuditRatingLevelTotal.ToString() : false ).FirstOrDefault();

                    var reason = auditReasons.Where(x => (result != null && result.AuditReason != null) ? (x.Id == (int)result.AuditReason) : false).FirstOrDefault()?.Name ?? "";
                    var row = table3.AddRow();
                    var txtTable3 = row.Cells[0].AddParagraph().AppendText(auditWorkScopePlanIndex + ".");
                    txtTable3.CharacterFormat.FontName = font;
                    txtTable3.CharacterFormat.FontSize = 9;


                    txtTable3 = row.Cells[1].AddParagraph().AppendText(score.ObjectName);
                    txtTable3.CharacterFormat.FontName = font;
                    txtTable3.CharacterFormat.FontSize = 9;

                    var partxtTable3 = row.Cells[2].AddParagraph();
                    txtTable3 = partxtTable3.AppendText(score?.Point.ToString() ?? "");
                    txtTable3.CharacterFormat.FontName = font;
                    txtTable3.CharacterFormat.FontSize = 9;
                    partxtTable3.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    partxtTable3 = row.Cells[3].AddParagraph();
                    txtTable3 = partxtTable3.AppendText(score?.RiskLevel ?? "");
                    txtTable3.CharacterFormat.FontName = font;
                    txtTable3.CharacterFormat.FontSize = 9;
                    partxtTable3.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    partxtTable3 = row.Cells[4].AddParagraph();
                    txtTable3 = partxtTable3.AppendText(result?.RiskLevelChangeName ?? "");
                    txtTable3.CharacterFormat.FontName = font;
                    txtTable3.CharacterFormat.FontSize = 9;
                    partxtTable3.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    partxtTable3 = row.Cells[5].AddParagraph();
                    txtTable3 = partxtTable3.AppendText(score?.AuditCycleName ?? "");
                    txtTable3.CharacterFormat.FontName = font;
                    txtTable3.CharacterFormat.FontSize = 9;
                    partxtTable3.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    partxtTable3 = row.Cells[6].AddParagraph();
                    txtTable3 = partxtTable3.AppendText((result != null && result.LastAudit != null) ? result.LastAudit.Value.ToString("dd/MM/yyyy") : "");
                    txtTable3.CharacterFormat.FontName = font;
                    txtTable3.CharacterFormat.FontSize = 9;
                    partxtTable3.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    partxtTable3 = row.Cells[7].AddParagraph();
                    txtTable3 = partxtTable3.AppendText(rate?.Name ?? "");
                    txtTable3.CharacterFormat.FontName = font;
                    txtTable3.CharacterFormat.FontSize = 9;
                    partxtTable3.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    partxtTable3 = row.Cells[8].AddParagraph();
                    txtTable3 = partxtTable3.AppendText(result != null ? result.Audit ? "Có" : "Không" : "");
                    txtTable3.CharacterFormat.FontName = font;
                    txtTable3.CharacterFormat.FontSize = 9;
                    partxtTable3.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    txtTable3 = row.Cells[9].AddParagraph().AppendText(reason);
                    txtTable3.CharacterFormat.FontName = font;
                    txtTable3.CharacterFormat.FontSize = 9;
                    auditWorkScopePlanIndex++;

                }

                table3.Rows.RemoveAt(1);
                table1.Rows.RemoveAt(0);
                Paragraph paraOther = doc.Sections[3].AddParagraph();
                paraOther.AppendHTML(auditplan.OtherInformation != null ? auditplan.OtherInformation : "");
                MemoryStream stream = new MemoryStream();
                stream.Position = 0;
                doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                Bytes = stream.ToArray();
                return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Grid.doc");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        private string QuaterNumberToRomanNumerals(int quater)
        {
            switch (quater)
            {
                case 1:
                case 2:
                case 3:
                    return "I";
                case 4:
                case 5:
                case 6:
                    return "II";
                case 7:
                case 8:
                case 9:
                    return "III";
                case 10:
                case 11:
                case 12:
                    return "IV";
                default:
                    return "";

            }
        }
        [HttpGet]
        private int GetWorkingDays(DateTime from, DateTime to)
        {
            var dayDifference = (int)to.Subtract(from).TotalDays;
            return Enumerable
                .Range(1, dayDifference)
                .Select(x => from.AddDays(x))
                .Count(x => x.DayOfWeek != DayOfWeek.Saturday && x.DayOfWeek != DayOfWeek.Sunday) + 1;
        }

        //[HttpGet("ExportFileWordPlanMIC/{id}")]
        //public IActionResult ExportFileWordPlanMIC(int id)
        //{
        //    byte[] Bytes = null;
        //    try
        //    {
        //        if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
        //        {
        //            return Unauthorized();
        //        }
        //        var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == id && a.IsDeleted != true);
        //        //data
        //        var _auditWorkScope = _uow.Repository<AuditWorkScope>().Include(a => a.AuditWorkScopeUserMapping).Where(a => a.auditwork_id == id
        //        && a.IsDeleted != true).ToArray();

        //        var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name.ToLower().Equals(REPORT_HEADER.ToLower()));
        //        var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name.ToLower().Equals(COMPANY_NAME.ToLower()));

        //        var day = DateTime.Now.ToString("dd");
        //        var month = DateTime.Now.ToString("MM");
        //        var year = DateTime.Now.ToString("yyyy");
        //        // Export word
        //        var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "MCredit_Kitano_ThongBaoKiemToan_v0.1.docx");
        //        fullPath = fullPath.ToString().Replace("\\", "/");
        //        using (Document doc = new Document(fullPath))
        //        {
        //            //Header
        //            doc.Sections[0].PageSetup.DifferentFirstPageHeaderFooter = true;
        //            Paragraph paragraph_header = doc.Sections[0].HeadersFooters.FirstPageHeader.AddParagraph();
        //            paragraph_header.AppendHTML(hearderSystem.Value);

        //            doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1",/* "TEN_DON_VI_1",*/ "ten_cong_ty_1",/* "ten_cuoc_kiem_toan_1",*/ "muc_tieu_cuoc_kiem_toan_1", "thoi_hieu_kt_1", "thoi_hieu_kt_2", "NGAY_BD_LAP_KH", "NGAY_KT_LAP_KH", "NGAY_BD_TD", "NGAY_PHAT_HANH_BC", "NGAY_KT_TD", "pham_vi_kt_new_1" },
        //                new string[] { day, month, year,/* nameauditworkscope.auditfacilities_name,*/ dataCt.Value,/* checkAuditWorkDetail.Name,*/
        //                    checkAuditWorkDetail.Target,
        //                checkAuditWorkDetail.from_date == null ? "dd/MM/yyyy": checkAuditWorkDetail.from_date.Value.ToString("dd/MM/yyyy"),
        //                checkAuditWorkDetail.to_date == null ? "dd/MM/yyyy": checkAuditWorkDetail.to_date.Value.ToString("dd/MM/yyyy"),
        //                checkAuditWorkDetail.StartDate != null ? checkAuditWorkDetail.StartDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
        //                checkAuditWorkDetail.EndDatePlanning != null ? checkAuditWorkDetail.EndDatePlanning.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
        //                checkAuditWorkDetail.StartDateReal != null ? checkAuditWorkDetail.StartDateReal.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
        //                checkAuditWorkDetail.ReleaseDate != null ? checkAuditWorkDetail.ReleaseDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
        //                checkAuditWorkDetail.EndDate != null ? checkAuditWorkDetail.EndDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
        //                checkAuditWorkDetail.AuditScope});

        //            var arrData = new List<AuditWorkScopeUserMapping>();
        //            for (int x = 0; x < _auditWorkScope.Length; x++)
        //            {
        //                var checkAuditWorkScopeUserMapping = _uow.Repository<AuditWorkScopeUserMapping>().Include(a => a.Users).Where(a => a.auditwork_scope_id == _auditWorkScope[x].Id).ToArray();
        //                for (int j = 0; j < checkAuditWorkScopeUserMapping.Length; j++)
        //                {
        //                    arrData.Add(checkAuditWorkScopeUserMapping[j]);
        //                }
        //            }
        //            Table table1 = doc.Sections[0].Tables[1] as Table;
        //            table1.ResetCells(arrData.Count() + 2, 3);
        //            TextRange txtTable1 = table1[0, 0].AddParagraph().AppendText("STT");
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 13;
        //            txtTable1.CharacterFormat.Bold = true;
        //            table1[0, 0].Width = 20;
        //            txtTable1 = table1[0, 1].AddParagraph().AppendText("Họ và tên/Name");
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 13;
        //            txtTable1.CharacterFormat.Bold = true;
        //            table1[0, 1].Width = 200;
        //            txtTable1 = table1[0, 2].AddParagraph().AppendText("Vai trò/Role");
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 13;
        //            txtTable1.CharacterFormat.Bold = true;
        //            table1[0, 2].Width = 300;
        //            txtTable1 = table1[1, 0].AddParagraph().AppendText("1");
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 13;
        //            table1[1, 0].Width = 20;
        //            txtTable1 = table1[1, 1].AddParagraph().AppendText(checkAuditWorkDetail.Users != null ? checkAuditWorkDetail.Users.FullName : string.Empty);
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 13;
        //            table1[1, 1].Width = 200;
        //            txtTable1 = table1[1, 2].AddParagraph().AppendText("Trưởng đoàn kiểm toán");
        //            txtTable1.CharacterFormat.FontName = "Times New Roman";
        //            txtTable1.CharacterFormat.FontSize = 13;
        //            table1[1, 2].Width = 300;
        //            //user_id//role
        //            var arrDataNew = arrData.Select(x => new { x.user_id, x.full_name, x.type, x.Users.Email }).Distinct().OrderBy(o => o.type).ToList();
        //            var auditingLeadIds = arrDataNew.Where(x => x.type == 1).Select(x => x.user_id).Distinct().ToList();

        //            for (int i = 0; i < arrDataNew.Count(); i++)
        //            {
        //                txtTable1 = table1[i + 2, 0].AddParagraph().AppendText((i + 2).ToString());
        //                txtTable1.CharacterFormat.FontName = "Times New Roman";
        //                txtTable1.CharacterFormat.FontSize = 13;
        //                table1[i + 2, 0].Width = 20;

        //                if (arrDataNew[i].type == 1)
        //                {
        //                    txtTable1 = table1[i + 2, 2].AddParagraph().AppendText("Trưởng nhóm kiểm toán");
        //                    txtTable1.CharacterFormat.FontName = "Times New Roman";
        //                    txtTable1.CharacterFormat.FontSize = 13;
        //                    table1[i + 2, 2].Width = 300;

        //                    txtTable1 = table1[i + 2, 1].AddParagraph().AppendText(arrDataNew[i].full_name);
        //                    txtTable1.CharacterFormat.FontName = "Times New Roman";
        //                    txtTable1.CharacterFormat.FontSize = 13;
        //                    table1[i + 2, 1].Width = 200;
        //                }
        //                else if (arrDataNew[i].type == 2 && (bool)auditingLeadIds?.Exists(f => f != arrDataNew[i].user_id))
        //                {
        //                    txtTable1 = table1[i + 2, 2].AddParagraph().AppendText("Kiểm toán viên – Thành viên");
        //                    txtTable1.CharacterFormat.FontName = "Times New Roman";
        //                    txtTable1.CharacterFormat.FontSize = 13;
        //                    table1[i + 2, 2].Width = 300;

        //                    txtTable1 = table1[i + 2, 1].AddParagraph().AppendText(arrDataNew[i].full_name);
        //                    txtTable1.CharacterFormat.FontName = "Times New Roman";
        //                    txtTable1.CharacterFormat.FontSize = 13;
        //                    table1[i + 2, 1].Width = 200;
        //                }
        //            }

        //            foreach (Section section1 in doc.Sections)
        //            {
        //                for (int i = 0; i < section1.Body.ChildObjects.Count; i++)
        //                {
        //                    if (section1.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
        //                    {
        //                        if (String.IsNullOrEmpty((section1.Body.ChildObjects[i] as Paragraph).Text.Trim()))
        //                        {
        //                            section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[i]);
        //                            i--;
        //                        }
        //                    }

        //                }
        //            }
        //            MemoryStream stream = new MemoryStream();
        //            stream.Position = 0;
        //            doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
        //            Bytes = stream.ToArray();
        //            return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "MCredit_Kitano_ThongBaoKiemToan_v0.1.docx");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest();
        //    }
        //}


        /// <summary>
        ///  Export the notification audit word.
        /// </summary>
        [HttpGet("ExportWordMCREDIT/{id}")]
        public IActionResult ExportWordMCREDIT(int id/*, int id_dv*/)
        {
            byte[] Bytes = null;
            var _paramInfoPrefix = "Param.SystemInfo";
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(x => x.Id == id);
                if (checkAuditWorkDetail == null)
                {
                    return NotFound();
                }
                var iCache = (IConnectionMultiplexer)HttpContext.RequestServices
                    .GetService(typeof(IConnectionMultiplexer));
                if (!iCache.IsConnected)
                {
                    return BadRequest();
                }
                var redisDb = iCache.GetDatabase();
                var value_get = redisDb.StringGet(_paramInfoPrefix);
                var companyName = string.Empty;
                var hearderSystem = string.Empty;
                if (value_get.HasValue)
                {
                    var list_param = JsonSerializerUtil.Deserialize<List<SystemParam>>(value_get);
                    companyName = list_param.FirstOrDefault(a => a.name.ToLower().Equals(COMPANY_NAME.ToLower()))?.value;
                    hearderSystem = list_param.FirstOrDefault(a => a.name.ToLower().Equals(REPORT_HEADER.ToLower()))?.value;
                }
                //data
                var _auditWorkScope = _uow.Repository<AuditWorkScope>().Include(a => a.AuditWorkScopeUserMapping).Where(a => a.auditwork_id == id && a.IsDeleted != true).ToArray();
                
                var day_vi = DateTime.Now.ToString("dd");
                var month_vi = DateTime.Now.ToString("MM");
                var year_vi = DateTime.Now.ToString("yyyy");
                var day_en = DateTime.Now.ToString("dd");
                var month_en = DateTime.Now.ToString("MMMM");
                var year_en = DateTime.Now.ToString("yyyy");

                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "MCredit_Kitano_ThongBaoKiemToan_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                {
                    //Header
                    doc.Sections[0].PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = doc.Sections[0].HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem);
                    doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1", "month_en", "day_en", "year_en",/* "TEN_DON_VI_1",*/ /*"ten_cong_ty_1",*/ "ten_cuoc_kiem_toan_1", "muc_tieu_cuoc_kiem_toan_1", "thoi_hieu_kt_1", "thoi_hieu_kt_2", "NGAY_BD_LAP_KH", "NGAY_KT_LAP_KH", "NGAY_BD_TD", "NGAY_PHAT_HANH_BC", "NGAY_KT_TD", "pham_vi_kt_new_1" },
                        
                        new string[] { day_vi, month_vi, year_vi, month_en, day_en, year_en,/* nameauditworkscope.auditfacilities_name,*/ /*companyName,*/ checkAuditWorkDetail.Name,
                        checkAuditWorkDetail.Target,
                        checkAuditWorkDetail.from_date == null ? "dd/MM/yyyy": checkAuditWorkDetail.from_date.Value.ToString("dd/MM/yyyy"),
                        checkAuditWorkDetail.to_date == null ? "dd/MM/yyyy": checkAuditWorkDetail.to_date.Value.ToString("dd/MM/yyyy"),
                        checkAuditWorkDetail.StartDate != null ? checkAuditWorkDetail.StartDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.EndDatePlanning != null ? checkAuditWorkDetail.EndDatePlanning.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.StartDateReal != null ? checkAuditWorkDetail.StartDateReal.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.ReleaseDate != null ? checkAuditWorkDetail.ReleaseDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.EndDate != null ? checkAuditWorkDetail.EndDate.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditWorkDetail.AuditScope});

                    var arrData = new List<AuditWorkScopeUserMapping>();
                    for (int x = 0; x < _auditWorkScope.Length; x++)
                    {
                        var checkAuditWorkScopeUserMapping = _uow.Repository<AuditWorkScopeUserMapping>().Include(a => a.Users).Where(a => a.auditwork_scope_id == _auditWorkScope[x].Id).ToArray();
                        for (int j = 0; j < checkAuditWorkScopeUserMapping.Length; j++)
                        {
                            arrData.Add(checkAuditWorkScopeUserMapping[j]);
                        }
                    }
                    Table table1 = doc.Sections[0].Tables[1] as Table;
                    string font = "Be Vietnam Pro";
                    Int32 size = 10;
                    table1.ResetCells(4, 3);
                    TextRange txtTable1 = table1[0, 0].AddParagraph().AppendText("STT");
                    txtTable1.CharacterFormat.FontName = font;
                    txtTable1.CharacterFormat.FontSize = size;
                    txtTable1.CharacterFormat.Bold = true;
                    table1[0, 0].Width = 20;
                    txtTable1 = table1[0, 1].AddParagraph().AppendText("Họ và tên/Name");
                    txtTable1.CharacterFormat.FontName = font;
                    txtTable1.CharacterFormat.FontSize = size;
                    txtTable1.CharacterFormat.Bold = true;
                    table1[0, 1].Width = 200;
                    txtTable1 = table1[0, 2].AddParagraph().AppendText("Vai trò/Role");
                    txtTable1.CharacterFormat.FontName = font;
                    txtTable1.CharacterFormat.FontSize = size;
                    txtTable1.CharacterFormat.Bold = true;
                    table1[0, 2].Width = 300;
                    txtTable1 = table1[1, 0].AddParagraph().AppendText("1.");
                    txtTable1.CharacterFormat.FontName = font;
                    txtTable1.CharacterFormat.FontSize = size;
                    table1[1, 0].Width = 20;
                    txtTable1 = table1[1, 1].AddParagraph().AppendText(checkAuditWorkDetail.Users != null ? checkAuditWorkDetail.Users.FullName : string.Empty);
                    txtTable1.CharacterFormat.FontName = font;
                    txtTable1.CharacterFormat.FontSize = size;
                    table1[1, 1].Width = 200;
                    txtTable1 = table1[1, 2].AddParagraph().AppendText("Trưởng đoàn kiểm toán");
                    txtTable1.CharacterFormat.FontName = font;
                    txtTable1.CharacterFormat.FontSize = size;
                    table1[1, 2].Width = 300;
                    
                    var arrDataNew = arrData.Select(x => new { x.user_id, x.full_name, x.type, x.Users.Email }).Distinct().OrderBy(o => o.type).ToList();
                    var auditingLs = arrDataNew.Where(x => x.type == 1).Select(x => new { x.user_id, x.full_name }).ToList();
                    var auditors = arrDataNew.Where(x => x.type == 2 && !(auditingLs.Exists(t => t.user_id == x.user_id))).Select(x => new { x.user_id, x.full_name }).ToList();

                    var isExistingRow2 = false;
                    if (auditingLs.Count() > 0)
                    {
                        txtTable1 = table1[2, 0].AddParagraph().AppendText("2.");
                        txtTable1.CharacterFormat.FontName = font;
                        txtTable1.CharacterFormat.FontSize = size;
                        table1[2, 0].Width = 20;

                        var names = string.Join(", ", auditingLs.Select(y => y.full_name).ToArray());
                        txtTable1 = table1[2, 1].AddParagraph().AppendText(names);
                        txtTable1.CharacterFormat.FontName = font;
                        txtTable1.CharacterFormat.FontSize = size;
                        table1[2, 1].Width = 200;

                        txtTable1 = table1[2, 2].AddParagraph().AppendText("Trưởng nhóm kiểm toán");
                        txtTable1.CharacterFormat.FontName = font;
                        txtTable1.CharacterFormat.FontSize = size;
                        table1[2, 2].Width = 300;
                    }
                    else
                    {
                        table1?.Rows.RemoveAt(2);
                        isExistingRow2 = true;
                    }

                    if (auditors.Count() > 0)
                    {
                        txtTable1 = table1[3, 0].AddParagraph().AppendText("3.");
                        txtTable1.CharacterFormat.FontName = font;
                        txtTable1.CharacterFormat.FontSize = size;
                        table1[3, 0].Width = 20;
                        var names = string.Join(", ", auditors.Select(y => y.full_name).ToArray());

                        txtTable1 = table1[3, 1].AddParagraph().AppendText(names);
                        txtTable1.CharacterFormat.FontName = font;
                        txtTable1.CharacterFormat.FontSize = size;
                        table1[3, 1].Width = 200;

                        txtTable1 = table1[3, 2].AddParagraph().AppendText("Kiểm toán viên – Thành viên");
                        txtTable1.CharacterFormat.FontName = font;
                        txtTable1.CharacterFormat.FontSize = size;
                        table1[3, 2].Width = 300;
                    }
                    else
                    {
                        if (table1?.Rows.Count > 3)
                        {
                            table1?.Rows.RemoveAt(3);
                        } else if (isExistingRow2)
                        {
                            table1?.Rows.RemoveAt(2);
                        }
                    }

                    foreach (Section section1 in doc.Sections)
                    {
                        for (int i = 0; i < section1.Body.ChildObjects.Count; i++)
                        {
                            if (section1.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
                            {
                                if (String.IsNullOrEmpty((section1.Body.ChildObjects[i] as Paragraph).Text.Trim()))
                                {
                                    section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[i]);
                                    i--;
                                }
                            }

                        }
                    }
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "MCredit_Kitano_ThongBaoKiemToan_v0.1.docx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private static void ReplaceKey(XWPFParagraph para, object model)
        {
            string text = para.ParagraphText;
            var runs = para.Runs;
            string styleid = para.Style;
            for (int i = 0; i < runs.Count; i++)
            {
                var run = runs[i];
                text = run.ToString();
                Type t = model.GetType();
                PropertyInfo[] pi = t.GetProperties();
                foreach (PropertyInfo p in pi)
                {
                    //$$ corresponds to $$ in the template, and can also be changed to other symbols, such as {$name}, must be unique
                    if (text.Contains("«" + p.Name + "»"))
                    {
                        text = text.Replace("«" + p.Name + "»", p.GetValue(model, null).ToString());
                    }
                }
                runs[i].SetText(text, 0);
            }
        }
        private static string ConvertPrecious(DateTime datatime)
        {
            string text = "";
            var get_month = datatime.Month;
            switch (get_month)
            {
                case 1:
                case 2:
                case 3:
                    text = "Quý 1";
                    break;
                case 4:
                case 5:
                case 6:
                    text = "Quý 2";
                    break;
                case 7:
                case 8:
                case 9:
                    text = "Quý 3";
                    break;
                case 10:
                case 11:
                case 12:
                    text = "Quý 4";
                    break;
            }
            return text;
        }

        //Chi tiết tab khác
        [HttpGet("OtherDetail/{id}")]
        public IActionResult OtherDetail(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWork = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == id && a.IsDeleted.Equals(false));

                if (checkAuditWork != null)
                {
                    var _budget = new AuditOtherDetailModel()
                    {
                        id = checkAuditWork.Id,
                        other = checkAuditWork.other == null ? "" : checkAuditWork.other,
                    };
                    return Ok(new { code = "1", msg = "success", data = _budget });
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
        //Cập nhật tab Khác
        [HttpPut("OtherUpdate")]
        public IActionResult OtherUpdate([FromBody] AuditOtherEditModel auditBudgetEditModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkOther = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == auditBudgetEditModel.id && a.IsDeleted.Equals(false));
                if (checkAuditWorkOther == null) { return NotFound(); }

                checkAuditWorkOther.other = auditBudgetEditModel.other;
                _uow.Repository<AuditWork>().Update(checkAuditWorkOther);
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //xuất word đề cương
        [HttpGet("ExportFileWordOther/{id}")]
        public IActionResult ExportFileWordOther(int id)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == id);

                var day = DateTime.Now.ToString("dd");
                var month = DateTime.Now.ToString("MM");
                var year = DateTime.Now.ToString("yyyy");
                //data

                var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "REPORT_HEADER");
                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");
                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_DeCuongKiemToan_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\Kitano_DeCuongKiemToan_v0.1.docx"))
                {

                    //Header
                    doc.Sections[0].PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = doc.Sections[0].HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem.Value);

                    //Remove header 2
                    doc.Sections[1].HeadersFooters.Header.ChildObjects.Clear();
                    //Add line breaks
                    Paragraph headerParagraph = doc.Sections[1].HeadersFooters.FirstPageHeader.AddParagraph();
                    headerParagraph.AppendBreak(BreakType.LineBreak);

                    //
                    if (checkAuditWorkDetail.other != null)
                    {
                        Paragraph paraOther = doc.Sections[0].AddParagraph();// para cuar Editor
                        paraOther.AppendHTML(checkAuditWorkDetail.other);
                    }

                    doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1", "ten_cuoc_kt",
                        "muc_tieu_kt_1", "pham_vi_kt_1", "gioi_han_pv_kt",
                        "thoi_hieu_kt_tu_1",
                        "thoi_hieu_kt_den_1"},
                        new string[] { day, month, year, checkAuditWorkDetail.Name,
                        checkAuditWorkDetail.Target, checkAuditWorkDetail.AuditScope, checkAuditWorkDetail.AuditScopeOutside,
                        checkAuditWorkDetail.from_date!= null ? checkAuditWorkDetail.from_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                        checkAuditWorkDetail.to_date != null ? checkAuditWorkDetail.to_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",});

                    foreach (Section section1 in doc.Sections)
                    {
                        for (int i = 0; i < section1.Body.ChildObjects.Count; i++)
                        {
                            if (section1.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
                            {
                                if (String.IsNullOrEmpty((section1.Body.ChildObjects[i] as Paragraph).Text.Trim()))
                                {
                                    section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[i]);
                                    i--;
                                }
                            }

                        }
                    }
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_DeCuongKiemToan_v0.1.docx");
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Xuất đề cương cho MBS, hàm này xuất phát từ hàm xuất đề cương dùng chung, đang chờ viết lại
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ExportFileWordOtherMBS/{id}")]
        public IActionResult ExportFileWordOtherMBS(int id)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == id);

                //data

                var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "REPORT_HEADER");
                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");
                //Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "MBS_Kitano_DeCuongKiemtoan_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\MBS_Kitano_DeCuongKiemtoan_v0.1.docx"))
                {

                    //Header
                    doc.Sections[0].PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = doc.Sections[0].HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem.Value);

                    //Remove header 2
                    doc.Sections[1].HeadersFooters.Header.ChildObjects.Clear();
                    //Add line breaks
                    Paragraph headerParagraph = doc.Sections[1].HeadersFooters.FirstPageHeader.AddParagraph();
                    headerParagraph.AppendBreak(BreakType.LineBreak);

                    //
                    if (checkAuditWorkDetail.other != null)
                    {
                        Paragraph paraOther = doc.Sections[0].AddParagraph();// para cuar Editor
                        paraOther.AppendHTML(checkAuditWorkDetail.other);
                    }

                    doc.MailMerge.Execute(new string[] { "ten_cuoc_kt",
                        "muc_tieu_kt_1", "pham_vi_kt_1", "gioi_han_pv_kt"},
                        new string[] { checkAuditWorkDetail.Name,
                        checkAuditWorkDetail.Target, checkAuditWorkDetail.AuditScope, checkAuditWorkDetail.AuditScopeOutside});

                    foreach (Section section1 in doc.Sections)
                    {
                        for (int i = 0; i < section1.Body.ChildObjects.Count; i++)
                        {
                            if (section1.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
                            {
                                if (String.IsNullOrEmpty((section1.Body.ChildObjects[i] as Paragraph).Text.Trim()))
                                {
                                    section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[i]);
                                    i--;
                                }
                            }

                        }
                    }
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_DeCuongKiemToan_v0.1.docx");
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Xuất đề cương cho MCREDIT, hàm này xuất phát từ hàm xuất đề cương dùng chung, đang chờ viết lại
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ExportFileWordOtherMCREDIT/{id}")]
        public IActionResult ExportFileWordOtherMCREDIT(int id)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == id);

                var day = DateTime.Now.ToString("dd");
                var month = DateTime.Now.ToString("MM");
                var year = DateTime.Now.ToString("yyyy");
                //data

                var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "REPORT_HEADER");
                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");
                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_DeCuongKiemToan_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\Kitano_DeCuongKiemToan_v0.1.docx"))
                {

                    //Header
                    doc.Sections[0].PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = doc.Sections[0].HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem.Value);

                    //Remove header 2
                    doc.Sections[1].HeadersFooters.Header.ChildObjects.Clear();
                    //Add line breaks
                    Paragraph headerParagraph = doc.Sections[1].HeadersFooters.FirstPageHeader.AddParagraph();
                    headerParagraph.AppendBreak(BreakType.LineBreak);

                    //
                    if (checkAuditWorkDetail.other != null)
                    {
                        Paragraph paraOther = doc.Sections[0].AddParagraph();// para cuar Editor
                        paraOther.AppendHTML(checkAuditWorkDetail.other);
                    }

                    doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1", "ten_cuoc_kt",
                        "muc_tieu_kt_1", "pham_vi_kt_1", "gioi_han_pv_kt",
                        "thoi_hieu_kt_tu_1",
                        "thoi_hieu_kt_den_1"},
                        new string[] { day, month, year, checkAuditWorkDetail.Name,
                        checkAuditWorkDetail.Target, checkAuditWorkDetail.AuditScope, checkAuditWorkDetail.AuditScopeOutside,
                        checkAuditWorkDetail.from_date!= null ? checkAuditWorkDetail.from_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                        checkAuditWorkDetail.to_date != null ? checkAuditWorkDetail.to_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",});

                    foreach (Section section1 in doc.Sections)
                    {
                        for (int i = 0; i < section1.Body.ChildObjects.Count; i++)
                        {
                            if (section1.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
                            {
                                if (String.IsNullOrEmpty((section1.Body.ChildObjects[i] as Paragraph).Text.Trim()))
                                {
                                    section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[i]);
                                    i--;
                                }
                            }

                        }
                    }
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_DeCuongKiemToan_v0.1.docx");
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Xuất đề cương cho MIC, hàm này xuất phát từ hàm xuất đề cương dùng chung, đang chờ viết lại
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ExportFileWordOtherMIC/{id}")]
        public IActionResult ExportFileWordOtherMIC(int id)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkDetail = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == id);

                var day = DateTime.Now.ToString("dd");
                var month = DateTime.Now.ToString("MM");
                var year = DateTime.Now.ToString("yyyy");
                //data

                var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "REPORT_HEADER");
                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");
                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_DeCuongKiemToan_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\Kitano_DeCuongKiemToan_v0.1.docx"))
                {

                    //Header
                    doc.Sections[0].PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = doc.Sections[0].HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem.Value);

                    //Remove header 2
                    doc.Sections[1].HeadersFooters.Header.ChildObjects.Clear();
                    //Add line breaks
                    Paragraph headerParagraph = doc.Sections[1].HeadersFooters.FirstPageHeader.AddParagraph();
                    headerParagraph.AppendBreak(BreakType.LineBreak);

                    //
                    if (checkAuditWorkDetail.other != null)
                    {
                        Paragraph paraOther = doc.Sections[0].AddParagraph();// para cuar Editor
                        paraOther.AppendHTML(checkAuditWorkDetail.other);
                    }

                    doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1", "ten_cuoc_kt",
                        "muc_tieu_kt_1", "pham_vi_kt_1", "gioi_han_pv_kt",
                        "thoi_hieu_kt_tu_1",
                        "thoi_hieu_kt_den_1"},
                        new string[] { day, month, year, checkAuditWorkDetail.Name,
                        checkAuditWorkDetail.Target, checkAuditWorkDetail.AuditScope, checkAuditWorkDetail.AuditScopeOutside,
                        checkAuditWorkDetail.from_date!= null ? checkAuditWorkDetail.from_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                        checkAuditWorkDetail.to_date != null ? checkAuditWorkDetail.to_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",});

                    foreach (Section section1 in doc.Sections)
                    {
                        for (int i = 0; i < section1.Body.ChildObjects.Count; i++)
                        {
                            if (section1.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
                            {
                                if (String.IsNullOrEmpty((section1.Body.ChildObjects[i] as Paragraph).Text.Trim()))
                                {
                                    section1.Body.ChildObjects.Remove(section1.Body.ChildObjects[i]);
                                    i--;
                                }
                            }

                        }
                    }
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_DeCuongKiemToan_v0.1.docx");
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Hàm này là hàm xuất đề cương đã được customize cho AMC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ExportFileWordOtherAMC/{id}")]
        public IActionResult ExportFileWordOtherAMC(int id)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditWorkDetail = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == id);
                var auditAssignment = _uow.Repository<AuditAssignment>().Include(a => a.Users).Where(a => a.auditwork_id == id && a.IsDeleted != true).OrderByDescending(x => x.user_id == checkAuditWorkDetail.person_in_charge).ToArray();

                var day = DateTime.Now.ToString("dd");
                var month = DateTime.Now.ToString("MM");
                var year = DateTime.Now.ToString("yyyy");
                //data

                var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "REPORT_HEADER");
                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");
                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "AMC_Kitano_ToTrinhKiemToan_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\Kitano_DeCuongKiemToan_v0.1.docx"))
                {

                    //Header
                    doc.Sections[0].PageSetup.DifferentFirstPageHeaderFooter = true;
                    Paragraph paragraph_header = doc.Sections[0].HeadersFooters.FirstPageHeader.AddParagraph();
                    paragraph_header.AppendHTML(hearderSystem.Value);

                    Table table1 = doc.Sections[0].Tables[1] as Table;

                    var font = "Times New Roman";

                    doc.MailMerge.MergeField += new MergeFieldEventHandler(MailMerge_MergeField);
                    //remove empty paragraphs
                    doc.MailMerge.HideEmptyParagraphs = true;
                    //remove empty group
                    doc.MailMerge.HideEmptyGroup = true;
                    doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1", "ten_cuoc_kt",
                        "muc_tieu_kt_1",
                        "pham_vi_kt_1",
                        "so_ngay_kt",
                        "ngay_bat_dau_kt",
                        "thoi_hieu_kt_tu_1",
                        "thoi_hieu_kt_den_1",
                        "html_de_cuong_kt"
                    },
                    new string[] { day, month, year, checkAuditWorkDetail.Name,
                        checkAuditWorkDetail.Target,
                        checkAuditWorkDetail.AuditScope,
                        (checkAuditWorkDetail.StartDate != null && checkAuditWorkDetail.EndDate != null) ? GetWorkingDays((DateTime)checkAuditWorkDetail.StartDate, (DateTime)checkAuditWorkDetail.EndDate).ToString() : "Chưa xác định",
                        checkAuditWorkDetail.StartDate!= null ? checkAuditWorkDetail.StartDate.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                        checkAuditWorkDetail.from_date!= null ? checkAuditWorkDetail.from_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                        checkAuditWorkDetail.to_date != null ? checkAuditWorkDetail.to_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                        checkAuditWorkDetail.other,
                    });

                    foreach (var (item, i) in auditAssignment.Select((item, i) => (item, i)))
                    {
                        var row = table1.AddRow();
                        string role = item.user_id == checkAuditWorkDetail.person_in_charge ? "Trưởng đoàn" : "Thành viên";

                        var paragraphTable1 = row.Cells[0].AddParagraph();
                        var txtParagraphTable1 = paragraphTable1.AppendText(i + 1 + ".");
                        paragraphTable1.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        txtParagraphTable1.CharacterFormat.FontName = font;
                        txtParagraphTable1.CharacterFormat.FontSize = 12;

                        paragraphTable1 = row.Cells[1].AddParagraph();
                        txtParagraphTable1 = paragraphTable1.AppendText(item.Users.FullName);
                        txtParagraphTable1.CharacterFormat.FontName = font;
                        txtParagraphTable1.CharacterFormat.FontSize = 12;

                        paragraphTable1 = row.Cells[3].AddParagraph();
                        txtParagraphTable1 = paragraphTable1.AppendText(role);
                        paragraphTable1.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        txtParagraphTable1.CharacterFormat.FontName = font;
                        txtParagraphTable1.CharacterFormat.FontSize = 12;
                    }
                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_DeCuongKiemToan_v0.1.docx");
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Hàm này dùng để giải quyết dữ liệu html cho vào docs, vì MailMerge không hỗ trợ html như hàm AppendHtml()
        /// Để kích hoạt hàm này, cho merge field bắt đầu bởi "html" và gán event handler là hàm này trước khi chạy MailMerge.Execute()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MailMerge_MergeField(object sender, MergeFieldEventArgs args)
        {
            if (args.FieldName.StartsWith("html"))
            {
                args.CurrentMergeField.OwnerParagraph.AppendHTML(args.FieldValue != null ? args.FieldValue.ToString() : "");
                args.Text = "";
                args.CurrentMergeField.OwnerParagraph.Format.LeftIndent = 0;
                args.CurrentMergeField.OwnerParagraph.Format.BeforeSpacing = 0;
                args.CurrentMergeField.OwnerParagraph.Format.AfterSpacing = 0;
                args.CurrentMergeField.OwnerParagraph.Format.BeforeAutoSpacing = false;
                args.CurrentMergeField.OwnerParagraph.Format.AfterAutoSpacing = false;
            }
        }
    }
}