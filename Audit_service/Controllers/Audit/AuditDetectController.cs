using Audit_service.DataAccess;
using Audit_service.Models.ExecuteModels;
using Audit_service.Models.ExecuteModels.Audit;
using Audit_service.Models.MigrationsModels;
using Audit_service.Models.MigrationsModels.Audit;
using Audit_service.Repositories;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Models.MigrationsModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
    public class AuditDetectController : BaseController
    {
        protected readonly IConfiguration _config;
        public AuditDetectController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditDetectSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_AD").ToArray();
                var auditdetect_id = approval_status.Select(a => a.item_id).ToList();


                Expression<Func<AuditDetect, bool>> filter = c => (obj.year == null || c.year.Equals(obj.year))
                && ((obj.auditwork_id == null) || c.auditwork_id.Equals(obj.auditwork_id))
                && ((obj.auditprocess_id == null) || c.auditprocess_id.Equals(obj.auditprocess_id))
                && ((obj.auditfacilities_id == null) || c.auditfacilities_id.Equals(obj.auditfacilities_id))
                && ((obj.audit_report == -1) || (obj.audit_report == 1 ? c.audit_report == true : c.audit_report == false))
                && (string.IsNullOrEmpty(obj.code) || c.code.Contains(obj.code.Trim()))
                && (string.IsNullOrEmpty(obj.title) || c.title.Contains(obj.title.Trim()))
                && (string.IsNullOrEmpty(obj.working_paper_code)
                    || c.AuditObserve.Any(x => x.working_paper_code == obj.working_paper_code))
                && c.IsDeleted.Equals(false);
                //&& c.CreatedBy == userInfo.Id;
                var list_auditdetect = _uow.Repository<AuditDetect>().Include(x => x.AuditObserve).Where(filter).OrderByDescending(a => a.CreatedAt);

                IEnumerable<AuditDetect> data = list_auditdetect;
                var count = list_auditdetect.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var result = data.Where(a => a.IsDeleted != true).Select(a => new AuditDetectModel()
                {
                    id = a.id,
                    year = a.year,
                    auditwork_id = a.auditwork_id,
                    auditwork_name = a.auditwork_name,
                    auditprocess_id = a.auditprocess_id,
                    auditprocess_name = a.auditprocess_name,
                    auditfacilities_id = a.auditfacilities_id,
                    auditfacilities_name = a.auditfacilities_name,
                    rating_risk = a.rating_risk,
                    code = a.code,
                    title = a.title,
                    status = approval_status.FirstOrDefault(x => x.item_id == a.id)?.StatusCode ?? "1.0",
                    flag_user_id = a.flag_user_id,
                    audit_report = a.audit_report == true ? 1 : 0,
                    working_paper_code = string.Join(", ", a.AuditObserve.ToList().Select(x => x.working_paper_code).Distinct()),
                    user_login_id = userInfo.Id,
                    CreatedBy = a.CreatedBy,
                    ApprovalUser = approval_status.FirstOrDefault(x => x.item_id == a.id)?.approver,
                    ApprovalUserLast = approval_status.FirstOrDefault(x => x.item_id == a.id)?.approver_last
                });

                return Ok(new { code = "1", msg = "success", data = result, total = count });

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

                //checkstatus
                var status_wait_name = "";
                var status_browser_name = "";
                var status_refuse_name = "";
                var approval_status = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == "M_AD" && a.StatusCode == "1.0");
                var approval_status_user_last = _uow.Repository<ApprovalFunction>().Include(a => a.Users, a => a.Users_Last).FirstOrDefault(a => a.item_id == id && a.function_code == "M_AD");
                var approval_status_wait = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.function_code == "M_AD"
                && (a.StatusCode == "1.1" || a.StatusCode == "2.1")
                && a.item_id == id);
                if (approval_status_wait != null)
                {
                    var approval_status_wait_name = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == "M_AD"
                    && a.StatusCode == approval_status_wait.StatusCode);
                    status_wait_name = approval_status_wait_name.StatusName;
                }
                var approval_status_browser = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.function_code == "M_AD"
                && a.StatusCode == "3.1"
                && a.item_id == id);
                if (approval_status_browser != null)
                {
                    var approval_status_browser_name = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == "M_AD"
                    && a.StatusCode == approval_status_browser.StatusCode);
                    status_browser_name = approval_status_browser_name.StatusName;
                }
                var approval_status_refuse = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.function_code == "M_AD"
                && (a.StatusCode == "3.2" || a.StatusCode == "2.2")
                && a.item_id == id);
                if (approval_status_refuse != null)
                {
                    var approval_status_refuse_name = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == "M_AD"
                    && a.StatusCode == approval_status_refuse.StatusCode);
                    status_refuse_name = approval_status_refuse_name.StatusName;
                }

                var auditDetect = _uow.Repository<AuditDetect>().Include(x => x.Users, x => x.AuditDetectFile).FirstOrDefault(x => x.id == id);
                var auditDetectCat = _uow.Repository<AuditDetect>().Include(x => x.CatDetectType).FirstOrDefault(x => x.id == id && x.CatDetectType.IsDeleted != true);
                var list_auditdetect = _uow.Repository<AuditDetect>().Include(x => x.AuditObserve).FirstOrDefault(a => a.id == id && a.IsDeleted != true);
                var auditObserve = _uow.Repository<AuditObserve>().Include(a => a.AuditDetect).Where(a => a.audit_detect_id == id).ToArray();
                var auditAuditRequestMonitor = _uow.Repository<AuditRequestMonitor>().Include(a => a.AuditDetect).Where(a => a.detectid == id && a.is_deleted.Equals(false) && a.is_active.Equals(true)).ToArray();
                var catauditrequest = _uow.Repository<AuditRequestMonitor>().Include(x => x.CatAuditRequest).Where(x => x.detectid == id && x.is_deleted.Equals(false) && x.is_active.Equals(true)).ToArray();
                var user = _uow.Repository<AuditRequestMonitor>().Include(x => x.Users).Where(a => a.detectid == id).ToArray();
                var auditfacilitymapping = _uow.Repository<AuditRequestMonitor>().Include(x => x.FacilityRequestMonitorMapping).Where(a => a.detectid == id && a.is_deleted.Equals(false) && a.is_active.Equals(true)).ToArray();
                //var indexPath = auditDetect.path_audit_detect != null ? auditDetect.path_audit_detect.ToString().LastIndexOf("\\") : 0;
                //string name = auditDetect.path_audit_detect != null ? auditDetect.path_audit_detect.ToString().Remove(0, indexPath + 1) : "";
                //string name = auditDetect.path_audit_detect.ToString().Replace("\\", "");
                //name = name.ToString().Replace("UploadsDocument", "");

                if (auditDetect != null)
                {
                    var AuditDetect = new AuditDetectDetail()
                    {
                        id = auditDetect.id,
                        code = auditDetect.code,
                        //name = auditDetect.name,
                        //status = auditDetect.status,
                        status = approval_status_user_last?.StatusCode ?? "1.0",
                        statusName = approval_status_wait != null ? status_wait_name
                        : approval_status_browser != null ? status_browser_name
                        : approval_status_refuse != null ? status_refuse_name
                        : approval_status.StatusName,
                        title = auditDetect.title,
                        short_title = auditDetect.short_title,
                        description = auditDetect.description,
                        evidence = auditDetect.evidence,
                        //path_audit_detect = auditDetect.path_audit_detect,
                        //filetype = auditDetect.FileType,
                        //filename = name,
                        affect = auditDetect.affect,
                        rating_risk = auditDetect.rating_risk,
                        admin_framework = auditDetect.admin_framework,
                        cause = auditDetect.cause,
                        audit_report = auditDetect.audit_report,
                        classify_audit_detect = auditDetect.classify_audit_detect,
                        summary_audit_detect = auditDetect.summary_audit_detect,
                        followers = auditDetect.followers,
                        str_followers = auditDetect.followers.HasValue ? auditDetect.Users.Id + ":" + auditDetect.Users.FullName : "",
                        str_classify_audit_detect = auditDetectCat != null ? (auditDetectCat.classify_audit_detect.HasValue ? auditDetectCat.CatDetectType.Id + ":" + auditDetectCat.CatDetectType.Name : "") : "",
                        str_year = auditDetect.year.HasValue ? auditDetect.year + ":" + auditDetect.year : "",
                        opinion_audit = auditDetect.opinion_audit,
                        reason = auditDetect.reason,
                        str_auditwork_name = auditDetect.auditwork_id.HasValue ? auditDetect.auditwork_id + ":" + auditDetect.auditwork_name : "",
                        str_auditprocess_name = auditDetect.auditprocess_id.HasValue ? auditDetect.auditprocess_id + ":" + auditDetect.auditprocess_name : "",
                        str_auditfacilities_name = auditDetect.auditfacilities_id.HasValue ? auditDetect.auditfacilities_id + ":" + auditDetect.auditfacilities_name : "",
                        working_paper_code = string.Join(", ", list_auditdetect.AuditObserve.ToList().Select(x => x.working_paper_code).Distinct()),
                        IsActive = auditDetect.IsActive,
                        IsDeleted = auditDetect.IsDeleted,
                        CreatedAt = auditDetect.CreatedAt,
                        CreatedBy = auditDetect.CreatedBy,
                        ModifiedAt = auditDetect.ModifiedAt,
                        ModifiedBy = auditDetect.ModifiedBy,
                        DeletedAt = auditDetect.DeletedAt,
                        DeletedBy = auditDetect.DeletedBy,
                        ListAuditObserve = auditObserve.Select(a => new ListAuditObserve
                        {
                            id = a.id,
                            code = a.code,
                            name = a.name,
                            description = a.Description,
                            working_paper_code = a.working_paper_code,
                        }).ToList(),
                        ListAuditRequestMonitor = auditfacilitymapping.Select(a => new ListAuditRequestMonitor
                        {
                            id = a.Id,
                            code = a.Code,
                            content = a.Content,
                            auditrequesttypeid = a.auditrequesttypeid,
                            auditrequesttype_name = a.CatAuditRequest.Name,
                            user_id = a.userid,
                            user_name = a.Users.FullName,
                            unit_name = (auditfacilitymapping != null ? string.Join(", ", a.FacilityRequestMonitorMapping.ToList().Where(c => c.type == 1).Select(x => x.audit_facility_name).Distinct()) : ""),
                            cooperateunit_name = (auditfacilitymapping != null ? string.Join(", ", a.FacilityRequestMonitorMapping.ToList().Where(c => c.type == 2).Select(x => x.audit_facility_name).Distinct()) : ""),
                            completeat = a.CompleteAt.HasValue ? a.CompleteAt.Value.ToString("dd-MM-yyyy") : null,
                        }).ToList(),
                        ListFile = auditDetect.AuditDetectFile.Where(a => a.IsDelete != true).Select(x => new AudiDetectFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = AuditDetect });
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
        //xoa quan sát liên quan
        [HttpDelete("Unchecked/{id}")]
        public IActionResult Unchecked(int id)
        {
            try
            {

                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkObserve = _uow.Repository<AuditObserve>().FirstOrDefault(a => a.id == id);
                if (checkObserve == null) { return NotFound(); }

                checkObserve.audit_detect_id = null;
                checkObserve.select_state = false;
                _uow.Repository<AuditObserve>().Update(checkObserve);
                var listauditobserve = _uow.Repository<AuditObserve>().GetAll(a => a.audit_detect_id == null && a.select_state == true);
                IEnumerable<AuditObserve> data = listauditobserve;
                return Ok(new { code = "1", msg = "success", data = data });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("ListCatDetectType")]
        public IActionResult ListCatDetectType(string q)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                string KeyWord = q;
                Expression<Func<CatDetectType, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Name.ToLower().Contains(q.ToLower()))
                && c.IsActive == true && c.IsDeleted != true;
                var listcatdetecttype = _uow.Repository<CatDetectType>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<CatDetectType> data = listcatdetecttype;

                //var listcatdetecttype = _uow.Repository<CatDetectType>().GetAll(a => a.IsDeleted != true);
                //IEnumerable<CatDetectType> data = listcatdetecttype;
                var res = data.Select(a => new DropListCatDetectTypeModel()
                {
                    id = a.Id,
                    name = a.Name,
                });
                return Ok(new { code = "1", msg = "success", data = res, total = res.Count() });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = "", total = 0 });
            }
        }
        //xóa phát hiện kiểm toán
        [HttpDelete("DeleteDetect/{id}")]
        public IActionResult DeleteDetect(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var _AuditDetect = _uow.Repository<AuditDetect>().FirstOrDefault(a => a.id == id);
                if (_AuditDetect == null)
                {
                    return NotFound();
                }

                _AuditDetect.IsDeleted = true;
                _AuditDetect.DeletedAt = DateTime.Now;
                _AuditDetect.DeletedBy = userInfo.Id;
                _uow.Repository<AuditDetect>().Update(_AuditDetect);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("CreateAuditDetect")]// thêm mới phát hiện
        public IActionResult CreateAuditDetect()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }


                var createAuditDetectModel = new CreateAuditDetectModel();
                var data = Request.Form["data"];
                //var pathSave = "";
                //var file_type = "";
                if (!string.IsNullOrEmpty(data))
                {
                    createAuditDetectModel = JsonSerializer.Deserialize<CreateAuditDetectModel>(data);
                    //var file = Request.Form.Files;
                    //file_type = file.FirstOrDefault()?.ContentType;
                    //pathSave = CreateUploadFile(file, "AuditDetect");
                }
                else
                {
                    return BadRequest();
                }
                var checkAuditWork = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == Int32.Parse(createAuditDetectModel.auditwork_id));
                var checkAuditFacility = _uow.Repository<AuditFacility>().FirstOrDefault(a => a.Id == Int32.Parse(createAuditDetectModel.auditfacilities_id));
                var checkAuditProcess = _uow.Repository<AuditProcess>().FirstOrDefault(a => a.Id == Int32.Parse(createAuditDetectModel.auditprocess_id));

                var _auditdetect = new AuditDetect
                {
                    year = Int32.Parse(createAuditDetectModel.year),
                    auditwork_id = Int32.Parse(createAuditDetectModel.auditwork_id),
                    auditwork_name = (checkAuditWork != null ? checkAuditWork.Name : ""),
                    auditfacilities_id = Int32.Parse(createAuditDetectModel.auditfacilities_id),
                    auditfacilities_name = (checkAuditFacility != null ? checkAuditFacility.Name : ""),
                    auditprocess_id = Int32.Parse(createAuditDetectModel.auditprocess_id),
                    auditprocess_name = (checkAuditProcess != null ? checkAuditProcess.Name : ""),
                    title = createAuditDetectModel.title,
                    short_title = createAuditDetectModel.short_title,
                    description = createAuditDetectModel.description,
                    evidence = createAuditDetectModel.evidence,
                    //path_audit_detect = pathSave,
                    affect = createAuditDetectModel.affect,
                    rating_risk = Int32.Parse(createAuditDetectModel.rating_risk),
                    admin_framework = Int32.Parse(createAuditDetectModel.admin_framework),
                    cause = createAuditDetectModel.cause,
                    audit_report = createAuditDetectModel.audit_report,
                    classify_audit_detect = Int32.Parse(createAuditDetectModel.classify_audit_detect),
                    summary_audit_detect = createAuditDetectModel.summary_audit_detect,
                    followers = Int32.Parse(createAuditDetectModel.followers),
                    opinion_audit = createAuditDetectModel.opinion_audit,
                    reason = createAuditDetectModel.reason,
                    IsDeleted = false,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userInfo.Id,
                    //status = 1,
                    //FileType = file_type,
                };
                _auditdetect.code = GetCode();
                _uow.Repository<AuditDetect>().Add(_auditdetect);

                var checkAuditDetectCreate = _uow.Repository<AuditDetect>().FirstOrDefault(a => a.IsDeleted != true && a.code.Equals(_auditdetect.code));
                var checkAuditObserveChoose = _uow.Repository<AuditObserve>().Find(a => a.select_state == true
                && a.audit_detect_id == null).ToArray();
                if (checkAuditDetectCreate != null && checkAuditObserveChoose.Count() > 0)
                {
                    for (int i = 0; i < checkAuditObserveChoose.Length; i++)
                    {
                        checkAuditObserveChoose[i].audit_detect_id = checkAuditDetectCreate.id;
                        _uow.Repository<AuditObserve>().Update(checkAuditObserveChoose[i]);
                    }
                }
                var checkAuditRequestMonitor = _uow.Repository<AuditRequestMonitor>().Find(a => a.flag == false && a.detectid == null && a.is_active == true && a.is_deleted == false && a.detect_code_new == _auditdetect.code).ToArray();
                if (checkAuditDetectCreate != null && checkAuditRequestMonitor.Count() > 0)
                {
                    for (int i = 0; i < checkAuditRequestMonitor.Length; i++)
                    {
                        checkAuditRequestMonitor[i].detectid = checkAuditDetectCreate.id;
                        checkAuditRequestMonitor[i].flag = true;
                        checkAuditRequestMonitor[i].detect_code_new = null;
                        _uow.Repository<AuditRequestMonitor>().Update(checkAuditRequestMonitor[i]);
                    }
                }

                var file = Request.Form.Files;
                var _listFile = new List<AuditDetectFile>();
                foreach (var item in file)
                {
                    var file_type = item.ContentType;
                    var pathSave = CreateUploadURL(item, "AuditDetect");
                    var audit_detect_file = new AuditDetectFile()
                    {
                        AuditDetect = _auditdetect,
                        IsDelete = false,
                        FileType = file_type,
                        Path = pathSave,
                    };
                    _listFile.Add(audit_detect_file);
                }
                foreach (var item in _listFile)
                {
                    _uow.Repository<AuditDetectFile>().AddWithoutSave(item);
                }
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //tự sinh mã AuditDetect
        public static string GetCode()
        {
            string EID = string.Empty;
            try
            {
                var yearnow = DateTime.Now.Year;
                string str_start = "PH." + yearnow;
                using (var context = new KitanoSqlContext())
                {
                    var list = context.AuditDetect.Where(a => a.IsDeleted != true).ToArray();
                    var data = list.Where(a => !string.IsNullOrEmpty(a.code) && a.code.StartsWith(str_start));
                    for (int i = data.Count() + 1; ; i++)
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
                        else if (i > 999 && i <= 9999)
                        {
                            EID = str_start + "." + i.ToString();
                        }
                        var data_ = list.Where(a => a.code == EID);
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
        //tự sinh mã AuditRequestMonitor
        public static string GetCodeAuditRequestMonitor()
        {
            string EID = string.Empty;
            try
            {
                var yearnow = DateTime.Now.Year;
                string str_start = "KN." + yearnow;
                using (var context = new KitanoSqlContext())
                {
                    var list = context.AuditRequest.Where(a => a.is_deleted != true).ToArray();
                    var data = list.Where(a => !string.IsNullOrEmpty(a.Code) && a.Code.StartsWith(str_start));
                    for (int i = data.Count() + 1; ; i++)
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
                        else if (i > 999 && i <= 9999)
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
        [HttpGet("PrivateCode")]
        public IActionResult PrivateCode()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var auditDetect = _uow.Repository<AuditDetect>().GetAll();
                var approval_status = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == "M_AD" && a.StatusCode == "1.0");
                if (auditDetect != null)
                {
                    return Ok(new { code = "1", msg = "success", privatecode = GetCode(), statuscreate = approval_status.StatusName });
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
        [HttpGet("SearchModalObserve")]
        public IActionResult SearchModalObserve(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<SearchModalObserveModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }


                Expression<Func<AuditObserve, bool>> filter = c => ((obj.discoverer == null) || c.discoverer.Equals(obj.discoverer))
                && ((obj.year == null) || c.year.Equals(obj.year))
                && ((obj.auditwork_id == null) || c.auditwork_id.Equals(obj.auditwork_id))
                && ((obj.auditfacilities_id == null) || c.WorkingPaper.unitid.Equals(obj.auditfacilities_id))
                && ((obj.auditprocess_id == null) || c.WorkingPaper.processid.Equals(obj.auditprocess_id))
                && (string.IsNullOrEmpty(obj.name) || c.name.ToLower().Contains(obj.name.ToLower()))
                && (string.IsNullOrEmpty(obj.working_paper_code) || c.working_paper_code.Contains(obj.working_paper_code.Trim()))
                && c.select_state.Equals(false) && c.IsDeleted != true;
                var list_auditobserve = _uow.Repository<AuditObserve>().Include(x => x.Users).Where(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditObserve> data = list_auditobserve;
                var count = list_auditobserve.Count();

                var result = data.Where(a => a.select_state != true).Select(a => new AuditObserveModalModel()
                {
                    id = a.id,
                    year = a.year,
                    code = a.code,
                    name = a.name,
                    description = a.Description,
                    discoverer_id = a.Users.Id,
                    discoverer_name = a.Users.FullName,
                    working_paper_code = a.working_paper_code,
                });

                return Ok(new { code = "1", msg = "success", data = result, total = count });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("ChooseObserve")]
        public IActionResult ChooseObserve(ChooseAll obj)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                if (string.IsNullOrEmpty(obj.listID))
                {
                    return NotFound();
                }
                var list = obj.listID.Split(",").ToList();
                var observe = _uow.Repository<AuditObserve>().Find(a => list.Contains(a.id.ToString())).ToArray();
                for (int i = 0; i < observe.Length; i++)
                {
                    observe[i].select_state = true;
                    _uow.Repository<AuditObserve>().Update(observe[i]);
                }
                return Ok(new { code = "1", msg = "success", data = observe, total = observe.Count() });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("ChooseObserveUpdate/{id}")]
        public IActionResult ChooseObserveUpdate(ChooseAll obj, int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                if (string.IsNullOrEmpty(obj.listID))
                {
                    return NotFound();
                }
                var list = obj.listID.Split(",").ToList();
                var observe = _uow.Repository<AuditObserve>().Find(a => list.Contains(a.id.ToString())).ToArray();
                for (int i = 0; i < observe.Length; i++)
                {
                    observe[i].select_state = true;
                    observe[i].audit_detect_id = id;
                    _uow.Repository<AuditObserve>().Update(observe[i]);
                }
                return Ok(new { code = "1", msg = "success", data = observe, total = observe.Count() });
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
        [HttpPost("CreateAuditRequestMonitor")]
        public IActionResult CreateAuditRequestMonitor([FromBody] CreateAuditRequestMonitorModel createAuditRequestMonitorModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var list_user = _uow.Repository<Users>().FirstOrDefault(x => x.Id == createAuditRequestMonitorModel.user_id);

                var _auditRequestMonitor = new AuditRequestMonitor
                {
                    detectid = createAuditRequestMonitorModel.id == 0 ? null : createAuditRequestMonitorModel.id,
                    Content = createAuditRequestMonitorModel.content,
                    auditrequesttypeid = createAuditRequestMonitorModel.audit_request_type_id,
                    is_deleted = false,
                    is_active = true,
                    created_at = DateTime.Now,
                    created_by = _userInfo.Id,
                    flag = false,
                    userid = list_user.Id,
                    note = createAuditRequestMonitorModel.note,
                    CompleteAt = createAuditRequestMonitorModel.complete_at != "" ? DateTime.Parse(createAuditRequestMonitorModel.complete_at) : null,
                };
                var _code = "";
                if (createAuditRequestMonitorModel.id != 0)
                {
                    var checkDetect = _uow.Repository<AuditDetect>().FirstOrDefault(x => x.id == createAuditRequestMonitorModel.id);
                    _code = checkDetect.code;
                }
                _auditRequestMonitor.Code = GetCodeAuditRequestMonitor();
                _auditRequestMonitor.detect_code_new = createAuditRequestMonitorModel.id != 0 ? _code : GetCode();
                if (_auditRequestMonitor.Content == "" || _auditRequestMonitor.Content == null) { return Ok(new { code = "0", msg = "fail" }); }
                _uow.Repository<AuditRequestMonitor>().Add(_auditRequestMonitor);
                //Thêm dữ liệu vào bảng mapping
                var _detailAuditRequestMonitor = _uow.Repository<AuditRequestMonitor>().FirstOrDefault(a => a.Code == _auditRequestMonitor.Code && a.is_deleted != true);
                var list_auditfacility_one = _uow.Repository<AuditFacility>().FirstOrDefault(a => a.Id == createAuditRequestMonitorModel.unit_id);
                var checkUnit = _uow.Repository<FacilityRequestMonitorMapping>().Find(z => z.type == 1
                && z.audit_request_monitor_id.Equals(_detailAuditRequestMonitor.Id));
                if (checkUnit.Any())
                {
                    _uow.Repository<FacilityRequestMonitorMapping>().Delete(checkUnit);
                    _uow.SaveChanges();
                }
                var checkCooperateunit = _uow.Repository<FacilityRequestMonitorMapping>().Find(z => z.type == 2 && z.audit_request_monitor_id.Equals(_detailAuditRequestMonitor.Id)).ToArray();
                if (checkCooperateunit.Any())
                {
                    _uow.Repository<FacilityRequestMonitorMapping>().Delete(checkCooperateunit);
                    _uow.SaveChanges();
                }

                List<FacilityRequestMonitorMapping> _unitMapping = new();
                var mapping = new FacilityRequestMonitorMapping
                {
                    type = 1,
                    audit_request_monitor_id = _detailAuditRequestMonitor.Id,
                    audit_facility_id = list_auditfacility_one.Id,
                    audit_facility_name = list_auditfacility_one.Name,

                };
                _unitMapping.Add(mapping);
                if (createAuditRequestMonitorModel.cooperateunit_id.Count > 0)
                {
                    foreach (var item in createAuditRequestMonitorModel.cooperateunit_id)
                    {
                        var list_auditfacility_mutil = _uow.Repository<AuditFacility>().FirstOrDefault(x => x.Id == item);
                        var mappingCooperateunit = new FacilityRequestMonitorMapping
                        {
                            type = 2,
                            audit_request_monitor_id = _detailAuditRequestMonitor.Id,
                            audit_facility_id = list_auditfacility_mutil.Id,
                            audit_facility_name = list_auditfacility_mutil.Name,
                        };
                        _unitMapping.Add(mappingCooperateunit);
                    }
                }
                _uow.Repository<FacilityRequestMonitorMapping>().Insert(_unitMapping);
                _uow.SaveChanges();

                return Ok(new { code = "1", msg = "success", idAuditRequestMonitor = _detailAuditRequestMonitor.Id });

            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        //xóa kiến nghị kt trong phát hiện kt
        [HttpDelete("DeleteAuditRequestMonitor/{id}")]
        public IActionResult DeleteAuditRequestMonitor(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var _AuditRequestMonitor = _uow.Repository<AuditRequestMonitor>().FirstOrDefault(a => a.Id == id);
                if (_AuditRequestMonitor == null)
                {
                    return NotFound();
                }

                _AuditRequestMonitor.is_deleted = true;
                _AuditRequestMonitor.deleted_at = DateTime.Now;
                _AuditRequestMonitor.deleted_by = userInfo.Id;
                _AuditRequestMonitor.detect_code_new = null;
                _AuditRequestMonitor.detectid = null;
                _AuditRequestMonitor.flag = false;
                _uow.Repository<AuditRequestMonitor>().Update(_AuditRequestMonitor);
                var listauditrequestmonitor = _uow.Repository<AuditRequestMonitor>().GetAll(a => a.detectid == null && a.flag == false && a.is_deleted != true && a.detect_code_new == GetCode());
                IEnumerable<AuditRequestMonitor> data = listauditrequestmonitor;
                return Ok(new { code = "1", msg = "success", data = data });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("DetailAuditRequestMonitor/{id}")]
        public IActionResult DetailAuditRequestMonitor(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditRequestMonitor = _uow.Repository<AuditRequestMonitor>().FirstOrDefault(a => a.Id == id && a.is_deleted.Equals(false));
                var checkAuditRequestMonitorUser = _uow.Repository<AuditRequestMonitor>().Include(x => x.Users).FirstOrDefault(a => a.Id == id && a.is_deleted.Equals(false));
                var auditrequesttype = _uow.Repository<AuditRequestMonitor>().Include(a => a.CatAuditRequest).FirstOrDefault(a => a.Id == id && a.is_deleted != true);
                var auditrequestmonitor = _uow.Repository<AuditRequestMonitor>().Include(a => a.FacilityRequestMonitorMapping).Where(a => a.Id == id && a.is_deleted != true).ToArray();

                if (checkAuditRequestMonitor != null)
                {
                    var _auditrequestmonitor = new DetailAuditRequestMonitorModel()
                    {
                        id = checkAuditRequestMonitor.Id,
                        code = checkAuditRequestMonitor.Code,
                        content = checkAuditRequestMonitor.Content,
                        audit_request_type_id = checkAuditRequestMonitor.auditrequesttypeid,
                        str_audit_request_type_id = checkAuditRequestMonitor.auditrequesttypeid.HasValue ? checkAuditRequestMonitor.CatAuditRequest.Id + ":" + checkAuditRequestMonitor.CatAuditRequest.Name : "",
                        audit_request_type_name = checkAuditRequestMonitor.auditrequesttypeid.HasValue ? checkAuditRequestMonitor.CatAuditRequest.Name : "",
                        str_unit_name = (auditrequestmonitor != null ? string.Join(", ", auditrequestmonitor[0].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 1 && c.audit_request_monitor_id.Equals(auditrequestmonitor[0].Id)).Select(x => x.audit_facility_id + ":" + x.audit_facility_name).Distinct()) : ""),
                        unit_name = (auditrequestmonitor != null ? string.Join(", ", auditrequestmonitor[0].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 1 && c.audit_request_monitor_id.Equals(auditrequestmonitor[0].Id)).Select(x => x.audit_facility_name).Distinct()) : ""),
                        str_cooperateunit_name = (auditrequestmonitor != null ? string.Join(", ", auditrequestmonitor[0].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 2 && c.audit_request_monitor_id.Equals(auditrequestmonitor[0].Id)).Select(x => x.audit_facility_id + ":" + x.audit_facility_name).Distinct()) : ""),
                        cooperateunit_name = (auditrequestmonitor != null ? string.Join(", ", auditrequestmonitor[0].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 2 && c.audit_request_monitor_id.Equals(auditrequestmonitor[0].Id)).Select(x => x.audit_facility_name).Distinct()) : ""),
                        str_user_name = checkAuditRequestMonitorUser != null ? checkAuditRequestMonitorUser.Users.Id + ":" + checkAuditRequestMonitorUser.Users.FullName : "",
                        responsible_person = checkAuditRequestMonitorUser != null ? checkAuditRequestMonitorUser.Users.FullName : "",
                        complete_at = checkAuditRequestMonitor.CompleteAt.HasValue ? checkAuditRequestMonitor.CompleteAt.Value.ToString("yyyy-MM-dd") : null,
                        note = checkAuditRequestMonitor.note,
                    };
                    return Ok(new { code = "1", msg = "success", data = _auditrequestmonitor });
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
        [HttpPut("EditAuditRequestMonitor")]
        public IActionResult EditAuditRequestMonitor([FromBody] EditAuditRequestMonitorModel editAuditRequestMonitorModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkEditAuditRequestMonitor = _uow.Repository<AuditRequestMonitor>().FirstOrDefault(a => a.Id == editAuditRequestMonitorModel.id && a.is_deleted.Equals(false));

                if (checkEditAuditRequestMonitor == null) { return NotFound(); }

                //var checkContent = _uow.Repository<AuditRequestMonitor>().FirstOrDefault(a => a.Content == editAuditRequestMonitorModel.content && a.is_deleted != true);
                //if (checkEditAuditRequestMonitor.Id != (checkContent != null ? checkContent.Id : null) && checkContent != null) { return Ok(new { code = "-1", msg = "fail" }); }
                checkEditAuditRequestMonitor.Content = editAuditRequestMonitorModel.content;
                checkEditAuditRequestMonitor.auditrequesttypeid = editAuditRequestMonitorModel.audit_request_type_id;
                checkEditAuditRequestMonitor.userid = editAuditRequestMonitorModel.user_id;
                checkEditAuditRequestMonitor.CompleteAt = editAuditRequestMonitorModel.complete_at;
                checkEditAuditRequestMonitor.note = editAuditRequestMonitorModel.note;

                //checkMapping
                var list_auditfacility_one = _uow.Repository<AuditFacility>().FirstOrDefault(a => a.Id == editAuditRequestMonitorModel.unit_id);
                var checkUnit = _uow.Repository<FacilityRequestMonitorMapping>().Find(z => z.type == 1
                && z.audit_request_monitor_id.Equals(editAuditRequestMonitorModel.id));
                if (checkUnit.Any())
                {
                    _uow.Repository<FacilityRequestMonitorMapping>().Delete(checkUnit);
                    _uow.SaveChanges();
                }
                var checkCooperateunit = _uow.Repository<FacilityRequestMonitorMapping>().Find(z => z.type == 2 && z.audit_request_monitor_id.Equals(editAuditRequestMonitorModel.id)).ToArray();
                if (checkCooperateunit.Any())
                {
                    _uow.Repository<FacilityRequestMonitorMapping>().Delete(checkCooperateunit);
                    _uow.SaveChanges();
                }

                List<FacilityRequestMonitorMapping> _unitMapping = new();
                var mapping = new FacilityRequestMonitorMapping
                {
                    type = 1,
                    audit_request_monitor_id = editAuditRequestMonitorModel.id,
                    audit_facility_id = list_auditfacility_one.Id,
                    audit_facility_name = list_auditfacility_one.Name,

                };
                _unitMapping.Add(mapping);
                if (editAuditRequestMonitorModel.cooperateunit_id.Count > 0)
                {
                    foreach (var item in editAuditRequestMonitorModel.cooperateunit_id)
                    {
                        var list_auditfacility_mutil = _uow.Repository<AuditFacility>().FirstOrDefault(x => x.Id == item);
                        var mappingCooperateunit = new FacilityRequestMonitorMapping
                        {
                            type = 2,
                            audit_request_monitor_id = editAuditRequestMonitorModel.id,
                            audit_facility_id = list_auditfacility_mutil.Id,
                            audit_facility_name = list_auditfacility_mutil.Name,
                        };
                        _unitMapping.Add(mappingCooperateunit);
                    }
                }
                _uow.Repository<FacilityRequestMonitorMapping>().Insert(_unitMapping);
                _uow.SaveChanges();

                return Ok(new { code = "1", msg = "success", idAuditRequestMonitor = checkEditAuditRequestMonitor.Id });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("ListFacility")]//list •	Đơn vị 
        public IActionResult ListFacility(string q)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = q;
                Expression<Func<AuditFacility, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Name.ToLower().Contains(q.ToLower())) && c.IsActive == true && c.Deleted != true;
                var listfacility = _uow.Repository<AuditFacility>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditFacility> data = listfacility;

                //var listfacility = _uow.Repository<AuditFacility>().GetAll(a => a.IsActive == true && a.Deleted != true);
                //IEnumerable<AuditFacility> data = listfacility;
                var res = data.Select(a => new DropListFacilityModel()
                {
                    id = a.Id,
                    name = a.Name,
                });
                return Ok(new { code = "1", msg = "success", data = res, total = res.Count() });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = "", total = 0 });
            }
        }
        [HttpGet("ListUserResponsible")]//list •người chịu trách nghiệm
        public IActionResult ListUserResponsible(string q, int? facility_id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = q;
                Expression<Func<Users, bool>> filter = c => (string.IsNullOrEmpty(q) || c.FullName.Contains(q) || c.UserName.Contains(q))
                && c.IsDeleted != true
                && c.IsActive == true
                && c.DepartmentId == facility_id;
                if (facility_id == null)
                {
                    return Ok(new { code = "1", msg = "success", data = "" });
                }
                else
                {
                    var list_user = _uow.Repository<Users>().Find(filter);
                    IEnumerable<Users> data = list_user;
                    var user = data.Select(a => new ListUsersModels()
                    {
                        id = a.Id,
                        FullName = a.FullName,
                    });
                    return Ok(new { code = "1", msg = "success", data = user });
                }
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = "", total = 0 });
            }
        }
        [HttpGet("ListCatAuditRequest")]//list•	Phân loại 
        public IActionResult ListCatAuditRequest(string q)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                string KeyWord = q;
                Expression<Func<CatAuditRequest, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Name.ToLower().Contains(q.ToLower())) && c.IsActive == true && c.IsDeleted != true && c.Status == true;
                var listcatauditrequestmodel = _uow.Repository<CatAuditRequest>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<CatAuditRequest> data = listcatauditrequestmodel;

                //var listcatauditrequestmodel = _uow.Repository<CatAuditRequest>().GetAll(a => a.IsActive == true && a.IsDeleted == false);
                //IEnumerable<CatAuditRequest> data = listcatauditrequestmodel;
                var res = data.Select(a => new DropListCatAuditRequestModel()
                {
                    id = a.Id,
                    name = a.Name,
                });
                return Ok(new { code = "1", msg = "success", data = res, total = res.Count() });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = "", total = 0 });
            }
        }
        [HttpGet("PrivateCodeAuditRequestMonitor")]
        public IActionResult PrivateCodeAuditRequestMonitor()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var auditRequestMonitor = _uow.Repository<AuditRequestMonitor>().GetAll();
                if (auditRequestMonitor != null)
                {
                    return Ok(new { code = "1", msg = "success", privatecode = GetCodeAuditRequestMonitor() });
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
        [HttpGet("ClearDetectCodeNew")]
        public IActionResult ClearDetectCodeNew()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var _auditrequestmonitor = _uow.Repository<AuditRequestMonitor>().Find(a => a.detect_code_new != null).ToArray();
                var _auditobserve = _uow.Repository<AuditObserve>().Find(a => a.audit_detect_id == null && a.select_state == true).ToArray();
                for (int i = 0; i < _auditrequestmonitor.Length; i++)
                {
                    _auditrequestmonitor[i].detectid = null;
                    _auditrequestmonitor[i].detect_code_new = null;
                    _auditrequestmonitor[i].flag = false;
                    _uow.Repository<AuditRequestMonitor>().Update(_auditrequestmonitor[i]);
                }
                for (int j = 0; j < _auditobserve.Length; j++)
                {
                    _auditobserve[j].audit_detect_id = null;
                    _auditobserve[j].select_state = false;
                    _uow.Repository<AuditObserve>().Update(_auditobserve[j]);
                }
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        //cập nhật auditdetect
        [HttpPost("EditAuditDetect")]
        public IActionResult EditAuditDetect()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }

                var editAuditDetectModel = new EditAuditDetectModel();
                var data = Request.Form["data"];
                //var pathSave = "";
                //var file_type = "";
                if (!string.IsNullOrEmpty(data))
                {
                    editAuditDetectModel = JsonSerializer.Deserialize<EditAuditDetectModel>(data);
                    //var file = Request.Form.Files;
                    //file_type = file.FirstOrDefault()?.ContentType;
                    //pathSave = CreateUploadFile(file, "AuditDetect");
                }
                else
                {
                    return BadRequest();
                }
                var CheckIdAuditDetect = _uow.Repository<AuditDetect>().FirstOrDefault(a => a.id == editAuditDetectModel.id);
                if (CheckIdAuditDetect == null)
                {
                    return NotFound();
                }

                var checkAuditWork = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == Int32.Parse(editAuditDetectModel.auditwork_id));
                var checkAuditFacility = _uow.Repository<AuditFacility>().FirstOrDefault(a => a.Id == Int32.Parse(editAuditDetectModel.auditfacilities_id));
                var checkAuditProcess = _uow.Repository<AuditProcess>().FirstOrDefault(a => a.Id == Int32.Parse(editAuditDetectModel.auditprocess_id));


                CheckIdAuditDetect.year = Int32.Parse(editAuditDetectModel.year);
                CheckIdAuditDetect.auditwork_id = Int32.Parse(editAuditDetectModel.auditwork_id);
                CheckIdAuditDetect.auditwork_name = (checkAuditWork != null ? checkAuditWork.Name : "");
                CheckIdAuditDetect.auditfacilities_id = Int32.Parse(editAuditDetectModel.auditfacilities_id);
                CheckIdAuditDetect.auditfacilities_name = (checkAuditFacility != null ? checkAuditFacility.Name : "");
                CheckIdAuditDetect.auditprocess_id = Int32.Parse(editAuditDetectModel.auditprocess_id);
                CheckIdAuditDetect.auditprocess_name = (checkAuditProcess != null ? checkAuditProcess.Name : "");
                CheckIdAuditDetect.title = editAuditDetectModel.title;
                CheckIdAuditDetect.short_title = editAuditDetectModel.short_title;
                CheckIdAuditDetect.description = editAuditDetectModel.description;
                CheckIdAuditDetect.evidence = editAuditDetectModel.evidence;
                //CheckIdAuditDetect.path_audit_detect = pathSave != "" ? pathSave : CheckIdAuditDetect.path_audit_detect;
                CheckIdAuditDetect.affect = editAuditDetectModel.affect;
                CheckIdAuditDetect.rating_risk = Int32.Parse(editAuditDetectModel.rating_risk);
                CheckIdAuditDetect.admin_framework = Int32.Parse(editAuditDetectModel.admin_framework);
                CheckIdAuditDetect.cause = editAuditDetectModel.cause;
                CheckIdAuditDetect.audit_report = editAuditDetectModel.audit_report;
                CheckIdAuditDetect.classify_audit_detect = Int32.Parse(editAuditDetectModel.classify_audit_detect);
                CheckIdAuditDetect.summary_audit_detect = editAuditDetectModel.summary_audit_detect;
                CheckIdAuditDetect.followers = Int32.Parse(editAuditDetectModel.followers);
                CheckIdAuditDetect.opinion_audit = editAuditDetectModel.opinion_audit;
                CheckIdAuditDetect.reason = editAuditDetectModel.reason;
                CheckIdAuditDetect.IsDeleted = false;
                CheckIdAuditDetect.IsActive = true;
                CheckIdAuditDetect.CreatedAt = DateTime.Now;
                CheckIdAuditDetect.CreatedBy = _userInfo.Id;
                CheckIdAuditDetect.status = 1;
                //CheckIdAuditDetect.FileType = file_type != null ? file_type : CheckIdAuditDetect.FileType;
                _uow.Repository<AuditDetect>().Update(CheckIdAuditDetect);

                var checkAuditDetectCreate = _uow.Repository<AuditDetect>().FirstOrDefault(a => a.IsDeleted != true && a.code.Equals(CheckIdAuditDetect.code));
                var checkAuditObserveChoose = _uow.Repository<AuditObserve>().Find(a => a.select_state == true
                && a.audit_detect_id == null
                && a.year == Int32.Parse(editAuditDetectModel.year)
                && a.auditwork_id == Int32.Parse(editAuditDetectModel.auditwork_id)).ToArray();
                if (checkAuditDetectCreate != null && checkAuditObserveChoose.Count() > 0)
                {
                    for (int i = 0; i < checkAuditObserveChoose.Length; i++)
                    {
                        checkAuditObserveChoose[i].audit_detect_id = checkAuditDetectCreate.id;
                        _uow.Repository<AuditObserve>().Update(checkAuditObserveChoose[i]);
                    }
                }
                var checkAuditRequestMonitor = _uow.Repository<AuditRequestMonitor>().Find(a => a.flag == false && a.detectid != null && a.is_active == true && a.is_deleted == false && a.detect_code_new != null).ToArray();
                if (checkAuditDetectCreate != null && checkAuditRequestMonitor.Count() > 0)
                {
                    for (int i = 0; i < checkAuditRequestMonitor.Length; i++)
                    {
                        checkAuditRequestMonitor[i].detectid = checkAuditDetectCreate.id;
                        checkAuditRequestMonitor[i].flag = true;
                        checkAuditRequestMonitor[i].detect_code_new = null;
                        _uow.Repository<AuditRequestMonitor>().Update(checkAuditRequestMonitor[i]);
                    }
                }
                var file = Request.Form.Files;
                var _listFile = new List<AuditDetectFile>();
                foreach (var item in file)
                {
                    var file_type = item.ContentType;
                    var pathSave = CreateUploadURL(item, "AuditDetect");
                    var audit_detect_file = new AuditDetectFile()
                    {
                        AuditDetect = CheckIdAuditDetect,
                        IsDelete = false,
                        FileType = file_type,
                        Path = pathSave,
                    };
                    _listFile.Add(audit_detect_file);
                }
                foreach (var item in _listFile)
                {
                    _uow.Repository<AuditDetectFile>().AddWithoutSave(item);
                }
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        //download file
        [AllowAnonymous]
        [HttpGet("DownloadAttachAuditDetect")]
        public IActionResult DonwloadFileAuditDetect(int id)
        {
            try
            {
                var self = _uow.Repository<AuditDetectFile>().FirstOrDefault(o => o.id == id);
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
                var self = _uow.Repository<AuditDetectFile>().FirstOrDefault(o => o.id == id);
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
                    _uow.Repository<AuditDetectFile>().Update(self);
                }

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        [HttpPost("SendBrowseAuditDetect/{id}")]//gửi duyệt
        public IActionResult SendBrowseAuditDetect(int id, int? user_id)//gửi duyệt cuộc kiểm toán"
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditDetect = _uow.Repository<AuditDetect>().FirstOrDefault(a => a.id == id && a.flag_user_id == null && a.IsDeleted != true && a.IsActive == true);
                if (checkAuditDetect == null)
                {
                    return NotFound();
                }
                if (checkAuditDetect.status != 1 && checkAuditDetect.status != 4)
                {
                    return Ok(new { code = "403", msg = "Cuộc kiểm toán này không thuộc trường hợp gửi duyệt" });
                }
                checkAuditDetect.status = 2;
                checkAuditDetect.flag_user_id = user_id;
                _uow.Repository<AuditDetect>().Update(checkAuditDetect);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = "" });
            }
        }
        [HttpPost("CensorshipAuditDetect/{id}")]//Kiểm duyệt
        public IActionResult CensorshipAuditDetect(int id, int? param)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditDetect = _uow.Repository<AuditDetect>().FirstOrDefault(a => a.id == id && a.flag_user_id != null && a.IsDeleted != true && a.IsActive == true);
                if (checkAuditDetect == null)
                {
                    return NotFound();
                }
                if (userInfo.Id != checkAuditDetect.flag_user_id)
                {
                    return Ok(new { code = "403", msg = "Người dùng này không được phép thao tác kiểm duyệt với cuộc kiểm toán này" });
                }
                if (checkAuditDetect.status != 2)
                {
                    return Ok(new { code = "403", msg = "Cuộc kiểm toán này không thuộc trường hợp kiểm duyệt" });
                }
                checkAuditDetect.status = (param == 1 ? 3 : 4);
                checkAuditDetect.flag_user_id = (param != 1 ? null : checkAuditDetect.flag_user_id);
                _uow.Repository<AuditDetect>().Update(checkAuditDetect);
                return Ok(new { code = "1", msg = "success", status = checkAuditDetect.status });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = "" });
            }
        }
        [HttpGet("DataDropDownPersonnel")]
        public IActionResult DataDropDownPersonnel()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var list_user = _uow.Repository<Users>().GetAll().Where(c => c.IsActive == true && c.IsDeleted != true && c.UsersType == 1).OrderByDescending(a => a.CreatedAt);
                IEnumerable<Users> data = list_user;
                var user = data.Select(a => new UsersInfoModels()
                {
                    Id = a.Id,
                    FullName = a.FullName,
                });
                return Ok(new { code = "1", msg = "success", data = user });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels() });
            }
        }

        //list kiến nghị khi thêm mới phát hiện
        [HttpGet("ListAuditRequestMonitorCreateAuditDetect")]
        public IActionResult ListAuditRequestMonitorCreateAuditDetect()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var arrData = new List<object>();
                //var arrData = [object];

                var auditRequestMonitorCreateAuditDetect = _uow.Repository<AuditRequestMonitor>().Find(x => x.is_deleted != true
                && x.flag == false
                && x.detect_code_new != null).ToArray();

                for (int i = 0; i < auditRequestMonitorCreateAuditDetect.Length; i++)
                {
                    var checkAuditRequestMonitor = _uow.Repository<AuditRequestMonitor>().FirstOrDefault(a => a.Id == auditRequestMonitorCreateAuditDetect[i].Id && a.is_deleted.Equals(false));
                    var checkAuditRequestMonitorUser = _uow.Repository<AuditRequestMonitor>().Include(x => x.Users).FirstOrDefault(a => a.Id == auditRequestMonitorCreateAuditDetect[i].Id && a.is_deleted.Equals(false));
                    var auditrequesttype = _uow.Repository<AuditRequestMonitor>().Include(a => a.CatAuditRequest).FirstOrDefault(a => a.Id == auditRequestMonitorCreateAuditDetect[i].Id && a.is_deleted != true);
                    var auditrequestmonitor = _uow.Repository<AuditRequestMonitor>().Include(a => a.FacilityRequestMonitorMapping).Where(a => a.Id == auditRequestMonitorCreateAuditDetect[i].Id && a.is_deleted != true).ToArray();
                    if (checkAuditRequestMonitor != null)
                    {
                        var _auditrequestmonitor = new DetailAuditRequestMonitorModel()
                        {
                            id = checkAuditRequestMonitor.Id,
                            code = checkAuditRequestMonitor.Code,
                            content = checkAuditRequestMonitor.Content,
                            audit_request_type_id = checkAuditRequestMonitor.auditrequesttypeid,
                            str_audit_request_type_id = checkAuditRequestMonitor.auditrequesttypeid.HasValue ? checkAuditRequestMonitor.CatAuditRequest.Id + ":" + checkAuditRequestMonitor.CatAuditRequest.Name : "",
                            audit_request_type_name = checkAuditRequestMonitor.auditrequesttypeid.HasValue ? checkAuditRequestMonitor.CatAuditRequest.Name : "",
                            str_unit_name = (auditrequestmonitor != null ? string.Join(", ", auditrequestmonitor[0].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 1 && c.audit_request_monitor_id.Equals(auditrequestmonitor[0].Id)).Select(x => x.audit_facility_id + ":" + x.audit_facility_name).Distinct()) : ""),
                            unit_name = (auditrequestmonitor != null ? string.Join(", ", auditrequestmonitor[0].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 1 && c.audit_request_monitor_id.Equals(auditrequestmonitor[0].Id)).Select(x => x.audit_facility_name).Distinct()) : ""),
                            str_cooperateunit_name = (auditrequestmonitor != null ? string.Join(", ", auditrequestmonitor[0].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 2 && c.audit_request_monitor_id.Equals(auditrequestmonitor[0].Id)).Select(x => x.audit_facility_id + ":" + x.audit_facility_name).Distinct()) : ""),
                            cooperateunit_name = (auditrequestmonitor != null ? string.Join(", ", auditrequestmonitor[0].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 2 && c.audit_request_monitor_id.Equals(auditrequestmonitor[0].Id)).Select(x => x.audit_facility_name).Distinct()) : ""),
                            str_user_name = checkAuditRequestMonitorUser != null ? checkAuditRequestMonitorUser.Users.Id + ":" + checkAuditRequestMonitorUser.Users.FullName : "",
                            responsible_person = checkAuditRequestMonitorUser != null ? checkAuditRequestMonitorUser.Users.FullName : "",
                            complete_at = checkAuditRequestMonitor.CompleteAt.HasValue ? checkAuditRequestMonitor.CompleteAt.Value.ToString("yyyy-MM-dd") : null,
                            note = checkAuditRequestMonitor.note,
                        };
                        arrData.Add(_auditrequestmonitor);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                if (arrData != null)
                {
                    return Ok(new { code = "1", msg = "success", data = arrData });
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
        //xuất excel
        [HttpGet("ExportExcel")]
        public IActionResult Export(string jsonData)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var obj = JsonSerializer.Deserialize<AuditDetectSearchModel>(jsonData);
                var code = obj.code;

                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_AD").ToArray();
                var auditdetect_id = approval_status.Select(a => a.item_id).ToList();
                var approval_status_config = _uow.Repository<ApprovalConfig>().Find(a => a.item_code == "M_AD").ToArray();

                var check_auditwork_name = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == obj.auditwork_id);

                Expression<Func<AuditDetect, bool>> filter = c => (obj.year == null || c.year.Equals(obj.year))
                && ((obj.auditwork_id == null) || c.auditwork_id.Equals(obj.auditwork_id))
                && ((obj.auditprocess_id == null) || c.auditprocess_id.Equals(obj.auditprocess_id))
                && ((obj.auditfacilities_id == null) || c.auditfacilities_id.Equals(obj.auditfacilities_id))
                && ((obj.audit_report == -1) || (obj.audit_report == 1 ? c.audit_report == true : c.audit_report == false))
                && (string.IsNullOrEmpty(obj.code) || c.code.Contains(obj.code.Trim()))
                && (string.IsNullOrEmpty(obj.title) || c.title.Contains(obj.title.Trim()))
                && (string.IsNullOrEmpty(obj.working_paper_code)
                    || c.AuditObserve.Any(x => x.working_paper_code == obj.working_paper_code))
                && c.IsDeleted.Equals(false);
                //&& c.CreatedBy == userInfo.Id;
                var list_auditdetect = _uow.Repository<AuditDetect>().Include(x => x.AuditObserve, x => x.CatDetectType, x => x.AuditRequestMonitor).Where(filter).OrderByDescending(a => a.CreatedAt);

                IEnumerable<AuditDetect> data = list_auditdetect;

                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_ThongKePhatHien_v0.2.xlsx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                var template = new FileInfo(fullPath);

                //var template = new FileInfo(@"D:\test\Kitano_ThongKePhatHien_v0.2.xlsx");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Sheet1"];
                    ExcelRange cellHeader = worksheet.Cells[1, 1];
                    cellHeader.Value = "Danh sách phát hiện kiểm toán";
                    cellHeader.Style.Font.Size = 16;
                    cellHeader.Style.Font.Bold = true;
                    ExcelRange cellYear = worksheet.Cells[2, 1];
                    cellYear.Value = "Năm kiểm toán:" + " " + obj.year;
                    //ExcelRange cellName = worksheet.Cells[3, 1];
                    //cellName.Value = "Năm kiểm toán:" + " " + (check_auditwork_name != null ? check_auditwork_name.Name : "");
                    //var _startrow = 4;
                    var startrow = 5;
                    var startcol = 1;
                    var count = 0;

                    var checkAuditRequestMonitor = _uow.Repository<AuditRequestMonitor>().Find(z => z.is_deleted == false).ToArray();
                    var checkAuditObserve = _uow.Repository<AuditObserve>().Find(z => z.IsDeleted != true).ToArray();
                    foreach (var a in data)
                    {
                        var statusCode = approval_status.FirstOrDefault(x => x.item_id == a.id)?.StatusCode ?? "1.0";
                        var statusName = approval_status_config.FirstOrDefault(x => x.StatusCode == statusCode);
                        var arrARM = new List<string>();
                        var arrAO = new List<string>();
                        string codeARM = "";
                        string codeAO = "";
                        var _auditRM = checkAuditRequestMonitor.Where(x => x.detectid == a.id).ToArray();
                        if (_auditRM != null)
                        {
                            for (int i = 0; i < _auditRM.Length; i++)
                            {
                                arrARM.Add(_auditRM[i].Code);
                            }
                            codeARM = string.Join(", ", arrARM);
                        }
                        var _auditO = checkAuditObserve.Where(x => x.audit_detect_id == a.id).ToArray();
                        if (_auditO != null)
                        {
                            for (int i = 0; i < _auditO.Length; i++)
                            {
                                arrAO.Add(_auditO[i].working_paper_code);
                            }
                            codeAO = string.Join(", ", arrAO);
                        }
                        count++;
                        ExcelRange cellNo = worksheet.Cells[startrow, startcol];
                        cellNo.Value = count;
                        //
                        ExcelRange AuditWorkNameAuditDetect = worksheet.Cells[startrow, startcol + 1];
                        AuditWorkNameAuditDetect.Value = string.Join(", ", a.auditwork_name);
                        //
                        ExcelRange AdminFrameworkAuditDetect = worksheet.Cells[startrow, startcol + 2];
                        AdminFrameworkAuditDetect.Value = string.Join(", ", (a.admin_framework == 1 ? "Quản trị/Tổ chức/Số liệu tài chính" :
                            a.admin_framework == 2 ? "Hoạt động/Quy trình/Quy định" :
                            a.admin_framework == 3 ? "Kiểm sát vận hành/Thực thi" :
                            a.admin_framework == 4 ? "Công nghệ thông tin" : ""));
                        //
                        ExcelRange ClassifyAuditDetect = worksheet.Cells[startrow, startcol + 3];
                        ClassifyAuditDetect.Value = string.Join(", ", a.CatDetectType.Name);
                        //
                        ExcelRange RatingRiskAuditDetect = worksheet.Cells[startrow, startcol + 4];
                        RatingRiskAuditDetect.Value = string.Join(", ", (a.rating_risk == 1 ? "Cao/Quan trọng" :
                            a.rating_risk == 2 ? "Trung bình" :
                            a.rating_risk == 3 ? "Thấp/Ít quan trọng" : ""));
                        //
                        ExcelRange CodeAuditDetect = worksheet.Cells[startrow, startcol + 5];
                        CodeAuditDetect.Value = string.Join(", ", a.code);
                        //
                        ExcelRange TitleAuditDetect = worksheet.Cells[startrow, startcol + 6];
                        TitleAuditDetect.Value = string.Join(", ", a.title);
                        //
                        ExcelRange DescriptionAuditDetect = worksheet.Cells[startrow, startcol + 7];
                        DescriptionAuditDetect.Value = string.Join(", ", a.description);
                        //
                        ExcelRange EvidenceAuditDetect = worksheet.Cells[startrow, startcol + 8];
                        EvidenceAuditDetect.Value = string.Join(", ", a.evidence);
                        //
                        ExcelRange AffectAuditDetect = worksheet.Cells[startrow, startcol + 9];
                        AffectAuditDetect.Value = string.Join(", ", a.affect);
                        //
                        ExcelRange CauseAuditDetect = worksheet.Cells[startrow, startcol + 10];
                        CauseAuditDetect.Value = string.Join(", ", a.cause);
                        //
                        ExcelRange CodeARM = worksheet.Cells[startrow, startcol + 11];
                        CodeARM.Value = string.Join(", ", codeARM);
                        //
                        ExcelRange ReeportAuditDetect = worksheet.Cells[startrow, startcol + 12];
                        ReeportAuditDetect.Value = string.Join(", ", (a.audit_report == true ? "Có" : "Không"));
                        //
                        ExcelRange OpinionAuditDetect = worksheet.Cells[startrow, startcol + 13];
                        OpinionAuditDetect.Value = string.Join(", ", (a.opinion_audit == true ? "Đồng ý " : "Không đồng ý"));
                        //
                        ExcelRange ReasonAuditDetect = worksheet.Cells[startrow, startcol + 14];
                        ReasonAuditDetect.Value = string.Join(", ", a.reason);
                        //
                        ExcelRange PaperCodeAuditDetect = worksheet.Cells[startrow, startcol + 15];
                        PaperCodeAuditDetect.Value = string.Join(", ", codeAO);
                        //
                        ExcelRange AuditprocessNameAuditDetect = worksheet.Cells[startrow, startcol + 16];
                        AuditprocessNameAuditDetect.Value = string.Join(", ", a.auditprocess_name);
                        //
                        ExcelRange AuditfacilitiesNameAuditDetect = worksheet.Cells[startrow, startcol + 17];
                        AuditfacilitiesNameAuditDetect.Value = string.Join(", ", a.auditfacilities_name);
                        //
                        ExcelRange StatusNameAuditDetect = worksheet.Cells[startrow, startcol + 18];
                        StatusNameAuditDetect.Value = string.Join(", ", statusName.StatusName);
                        startrow++;
                    }
                    using ExcelRange r = worksheet.Cells[5, 1, startrow, 19];
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
                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kitano_ThongKePhatHien_v0.2.xlsx");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}

