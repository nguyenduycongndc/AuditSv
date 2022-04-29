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
using Audit_service.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using KitanoUserService.API.Models.MigrationsModels.Category;

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class AuditRequestMonitorController : BaseController
    {
        protected readonly IConfiguration _config;

        public AuditRequestMonitorController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
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
                var obj = JsonSerializer.Deserialize<AuditRequestMonitorSearchModel>(jsonData);
                var code = obj.Code;
                var now = DateTime.Now.Date;
                var conclusion = (obj.Conclusion != "" ? Convert.ToInt32(obj.Conclusion) : 0);
                var unit = (obj.Unitid != "" ? Convert.ToInt32(obj.Unitid) : 0);
                var process = (obj.ProcessStatus != "" ? Convert.ToInt32(obj.ProcessStatus) : 0);
                var type = (obj.AuditRequestType != "" ? Convert.ToInt32(obj.AuditRequestType) : 0);
                var year = (!string.IsNullOrEmpty(obj.Year) ? Convert.ToInt32(obj.Year) : 0);
                var auditWorkId = (!string.IsNullOrEmpty(obj.AuditId) ? Convert.ToInt32(obj.AuditId) : 0);
                var cooperatingUnitId = (!string.IsNullOrEmpty(obj.CooperatateUnitId) ? Convert.ToInt32(obj.CooperatateUnitId) : 0);

                DateTime? completedatefrom = null;
                DateTime? completedateto = null;
                DateTime? completeactualfrom = null;
                DateTime? completeactualto = null;
                if (!string.IsNullOrEmpty(obj.CompleteDateFrom))
                    completedatefrom = Convert.ToDateTime(obj.CompleteDateFrom);
                if (!string.IsNullOrEmpty(obj.CompleteDateTo))
                    completedateto = Convert.ToDateTime(obj.CompleteDateTo);
                if (!string.IsNullOrEmpty(obj.CompleteActualFrom))
                    completeactualfrom = Convert.ToDateTime(obj.CompleteActualFrom);
                if (!string.IsNullOrEmpty(obj.CompleteActualTo))
                    completeactualto = Convert.ToDateTime(obj.CompleteActualTo);

                var timestatus = Convert.ToInt32(obj.TimeStatus) == 2;
                var _unit = _uow.Repository<AuditFacility>().Find(a => a.IsActive == true).ToArray();
                var _user = _uow.Repository<Users>().Find(c => c.Id == userInfo.Id).FirstOrDefault();
                var _roles = _uow.Repository<UsersRoles>().Find(c => c.Id == userInfo.Id).FirstOrDefault();
                var list_auditrequest = (from a in _uow.Repository<AuditRequestMonitor>().Include(p => p.AuditDetect, p => p.CatAuditRequest, p => p.FacilityRequestMonitorMapping)
                                         join b in _uow.Repository<ApprovalFunction>().Find(p => p.function_code == "M_AD" && p.StatusCode == "3.1") on a.AuditDetect.id equals b.item_id.Value
                                         let checkstatus = (((a.ProcessStatus ?? 1) != 3 && (a.extend_at.HasValue ? a.extend_at < now : a.CompleteAt.HasValue ? a.CompleteAt < now : false)) || (a.ActualCompleteAt.HasValue && (a.extend_at.HasValue ? a.ActualCompleteAt > a.extend_at : a.CompleteAt.HasValue ? a.ActualCompleteAt > a.CompleteAt : false)))
                                         where (code == "" || a.Code == code)
                && (year == 0 || a.AuditDetect.year == year)
                && (auditWorkId == 0 || a.AuditDetect.auditwork_id == auditWorkId)
                && (process == 0 || (a.ProcessStatus ?? 1) == process)
                && (type == 0 || (a.auditrequesttypeid ?? 1) == type)
                && (obj.TimeStatus == "0" || checkstatus == timestatus)
                && (conclusion == 0 || (a.Conclusion ?? 1) == conclusion)
                && (a.is_deleted != true)
                && ((!completedatefrom.HasValue || (completedatefrom.Value.Year <= a.CompleteAt.Value.Year && completedatefrom.Value.Month <= a.CompleteAt.Value.Month && completedatefrom.Value.Day <= a.CompleteAt.Value.Day)) && (!completedateto.HasValue || (a.CompleteAt.Value.Year <= completedateto.Value.Year && a.CompleteAt.Value.Month <= completedateto.Value.Month && a.CompleteAt.Value.Day <= completedateto.Value.Day)))
                && ((!completeactualfrom.HasValue || (completeactualfrom.Value.Year <= a.ActualCompleteAt.Value.Year && completeactualfrom.Value.Month <= a.ActualCompleteAt.Value.Month && completeactualfrom.Value.Day <= a.ActualCompleteAt.Value.Day)) && (!completeactualto.HasValue || (a.ActualCompleteAt.Value.Year <= completeactualto.Value.Year && a.ActualCompleteAt.Value.Month <= completeactualto.Value.Month && a.ActualCompleteAt.Value.Day <= completeactualto.Value.Day)))
                && (_user.UsersType != 2 || a.FacilityRequestMonitorMapping.FirstOrDefault(f => f.type == 1).audit_facility_id == _user.DepartmentId)
                && (unit == 0 || a.FacilityRequestMonitorMapping.FirstOrDefault(f => f.type == 1).audit_facility_id == unit)
                && (_user.UsersType != 2 || a.FacilityRequestMonitorMapping.FirstOrDefault(f => f.type == 2).audit_facility_id == _user.DepartmentId)
                && (cooperatingUnitId == 0 || a.FacilityRequestMonitorMapping.Any(f => f.type == 2 && f.audit_facility_id == cooperatingUnitId))
                                         select a).OrderByDescending(p => p.created_at);

                IEnumerable<AuditRequestMonitor> data = list_auditrequest;
                var count = list_auditrequest.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var lst = data.Select(a => new AuditRequestMonitorModel()
                {
                    Id = a.Id,
                    Code = a.Code,
                    userid = a.userid,
                    Content = a.Content,
                    Unitid = a.unitid,
                    ProcessStatus = a.ProcessStatus,
                    CompleteAt = a.CompleteAt.HasValue ? a.CompleteAt.Value.ToString("dd/MM/yyyy") : "",
                    ActualCompleteAt = a.ActualCompleteAt.HasValue ? a.ActualCompleteAt.Value.ToString("dd/MM/yyyy") : "",
                    TimeStatus = (((a.ProcessStatus ?? 1) != 3 && (a.extend_at.HasValue ? a.extend_at < now : a.CompleteAt.HasValue ? a.CompleteAt < now : false)) || (a.ActualCompleteAt.HasValue && (a.extend_at.HasValue ? a.ActualCompleteAt > a.extend_at : a.CompleteAt.HasValue ? a.ActualCompleteAt > a.CompleteAt : false))) ? 2 : 1,
                    Conclusion = a.Conclusion,
                    UnitName = _unit.FirstOrDefault(u => u.Id == a.FacilityRequestMonitorMapping.FirstOrDefault(f => f.type == 1)?.audit_facility_id)?.Name,
                    auditrequesttypename = a.CatAuditRequest?.Name,
                    ExtendAt = a.extend_at.HasValue ? a.extend_at.Value.ToString("yyyy-MM-dd") : "",
                    ExtendAtString = a.extend_at.HasValue ? a.extend_at.Value.ToString("dd/MM/yyyy") : "",
                });
                return Ok(new { code = "1", msg = "success", data = lst, total = count });

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

                var audit_request_monitor = _uow.Repository<AuditRequestMonitor>().Include(c => c.AuditDetect, c => c.AuditDetect.Users, o => o.Users, c => c.CatAuditRequest, c => c.FacilityRequestMonitorMapping, c => c.AuditRequestMonitorFile).FirstOrDefault(a => a.Id == id);
                var _unit = _uow.Repository<AuditFacility>().Find(a => a.IsActive == true).ToArray();
                if (audit_request_monitor != null)
                {
                    var result = new AuditRequestMonitorDetailModel()
                    {
                        Id = audit_request_monitor.Id,
                        Detectcode = audit_request_monitor.AuditDetect?.code,
                        DetectDescription = audit_request_monitor.AuditDetect?.description,
                        Code = audit_request_monitor.Code,
                        AuditRequestTypeId = audit_request_monitor.auditrequesttypeid,
                        Content = audit_request_monitor.Content,
                        UnitName = _unit.FirstOrDefault(p => p.Id == audit_request_monitor.FacilityRequestMonitorMapping?.FirstOrDefault(x => x.type == 1)?.audit_facility_id)?.Name,
                        Unitid = audit_request_monitor.FacilityRequestMonitorMapping.FirstOrDefault(x => x.type == 1).audit_facility_id,
                        username = audit_request_monitor.Users?.FullName,
                        trackinguser = audit_request_monitor.AuditDetect?.Users?.FullName,
                        CompleteAt = audit_request_monitor.CompleteAt.HasValue ? audit_request_monitor.CompleteAt.Value.ToString("yyyy-MM-dd") : null,
                        ActualCompleteAt = audit_request_monitor.ActualCompleteAt.HasValue ? audit_request_monitor.ActualCompleteAt.Value.ToString("yyyy-MM-dd") : null,
                        Conclusion = audit_request_monitor.Conclusion ?? 1,
                        TimeStatus = audit_request_monitor.TimeStatus,
                        ProcessStatus = audit_request_monitor.ProcessStatus ?? 1,
                        Evidence = audit_request_monitor.Evidence,
                        Comment = audit_request_monitor.Comment,
                        Auditcomment = audit_request_monitor.Auditcomment,
                        Captaincomment = audit_request_monitor.Captaincomment,
                        Leadercomment = audit_request_monitor.Leadercomment,
                        Reason = audit_request_monitor.Reason,
                        Unitcomment = audit_request_monitor.Unitcomment,
                        auditrequesttypename = audit_request_monitor.CatAuditRequest?.Name,
                        incomplete_reason = audit_request_monitor.incomplete_reason,
                        ExtendAt = audit_request_monitor.extend_at.HasValue ? audit_request_monitor.extend_at.Value.ToString("yyyy-MM-dd") : null,
                        facilityRequestMonitorModels = audit_request_monitor.FacilityRequestMonitorMapping.Where(p => p.type == 2).Select(x => new FacilityRequestMonitorModel
                        {
                            id = x.id,
                            audit_request_monitor_id = audit_request_monitor.Id,
                            audit_facility_id = x.audit_facility_id,
                            audit_facility_name = _unit.FirstOrDefault(p => p.Id == x.audit_facility_id)?.Name,
                            comment = x.comment ?? ""
                        }).ToList(),
                        auditrequestfile = audit_request_monitor.AuditRequestMonitorFile.Where(p =>
                        (!p.isdelete ?? false)).Select(p => new AuditRequestMonitorFileModel()
                        {
                            id = p.id,
                            name = p.path
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

        [HttpPost("Edit")]
        public IActionResult Edit([FromForm] AuditRequestMonitorModifyModel audiplaninfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var list_all = _uow.Repository<AuditRequestMonitor>().GetAll().ToArray();

                var AuditRequestMonitorInfo = list_all.FirstOrDefault(a => a.Id == audiplaninfo.Id);
                if (AuditRequestMonitorInfo == null)
                {
                    return NotFound();
                }

                AuditRequestMonitorInfo.ProcessStatus = Convert.ToInt32(audiplaninfo.ProcessStatus);

                if (audiplaninfo.ActualCompleteAt != null)
                {
                    AuditRequestMonitorInfo.ActualCompleteAt = audiplaninfo.ActualCompleteAt;
                }

                AuditRequestMonitorInfo.Evidence = audiplaninfo.Evidence;
                AuditRequestMonitorInfo.Auditcomment = audiplaninfo.Auditcomment;
                AuditRequestMonitorInfo.Captaincomment = audiplaninfo.Captaincomment;
                AuditRequestMonitorInfo.Leadercomment = audiplaninfo.Leadercomment;
                if (!string.IsNullOrEmpty(audiplaninfo.Conclusion))
                    AuditRequestMonitorInfo.Conclusion = int.TryParse(audiplaninfo.Conclusion, out int value) ? value : 1;
                AuditRequestMonitorInfo.Reason = audiplaninfo.Reason;
                AuditRequestMonitorInfo.Comment = audiplaninfo.Comment;
                AuditRequestMonitorInfo.incomplete_reason = audiplaninfo.incomplete_reason;
                AuditRequestMonitorInfo.Unitcomment = audiplaninfo.Unitcomment;
                AuditRequestMonitorInfo.modified_at = DateTime.Now;
                AuditRequestMonitorInfo.modified_by = _userInfo.Id;

                AuditRequestMonitorInfo.AuditRequestMonitorFile = audiplaninfo.auditrequestfile != null ? audiplaninfo.auditrequestfile.Select(p => new AuditRequestMonitorFile()
                {
                    path = CreateUploadURL(p, "AuditRequest/"),
                    file_type = p.ContentType,
                    isdelete = false,
                }).ToList() : new List<AuditRequestMonitorFile>();
                List<FacilityRequestMonitorMapping> ListFacilityMonitor = new List<FacilityRequestMonitorMapping>();
                if (audiplaninfo.facilityrequestmonitor != null)
                {
                    foreach (var facilitymonitor in audiplaninfo.facilityrequestmonitor)
                    {
                        var facilitirequestmonitormapping = _uow.Repository<FacilityRequestMonitorMapping>().FirstOrDefault(f => f.id == facilitymonitor.id);
                        facilitirequestmonitormapping.comment = facilitymonitor.comment;
                        facilitirequestmonitormapping.process_status = facilitymonitor.process_status;
                        ListFacilityMonitor.Add(facilitirequestmonitormapping);
                    }
                }
                _uow.Repository<FacilityRequestMonitorMapping>().Update(ListFacilityMonitor);
                _uow.Repository<AuditRequestMonitor>().Update(AuditRequestMonitorInfo);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
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
                var CatRiskLevel = _uow.Repository<AuditRequestMonitor>().FirstOrDefault(a => a.Id == id);
                if (CatRiskLevel == null)
                {
                    return NotFound();
                }

                CatRiskLevel.is_deleted = true;
                _uow.Repository<AuditRequestMonitor>().Update(CatRiskLevel);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("UpdateExtend")]
        public IActionResult UpdateExtend(int id, string extendat)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var auditrequest = _uow.Repository<AuditRequestMonitor>().FirstOrDefault(a => a.Id == id);
                if (auditrequest == null)
                {
                    return NotFound();
                }
                if (!string.IsNullOrEmpty(extendat))
                    auditrequest.extend_at = DateTime.Parse(extendat);
                auditrequest.modified_at = DateTime.Now;
                auditrequest.modified_by = userInfo.Id;
                _uow.Repository<AuditRequestMonitor>().Update(auditrequest);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("DownloadAttach")]
        public IActionResult DonwloadFile(int id)
        {
            try
            {
                var path = "";
                var filetype = "";
                var self = _uow.Repository<AuditRequestMonitorFile>().FirstOrDefault(o => o.id == id);
                if (self == null)
                {
                    return NotFound();
                }
                path = self.path;
                filetype = self.file_type;
                var fullPath = Path.Combine(_config["Upload:AuditDocsPath"], path);
                var name = "DownLoadFile";
                if (!string.IsNullOrEmpty(path))
                {
                    var _array = path.Split("\\");
                    name = _array[_array.Length - 1];
                }
                var fs = new FileStream(fullPath, FileMode.Open);

                return File(fs, filetype, name);
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
                var self = _uow.Repository<AuditRequestMonitorFile>().FirstOrDefault(o => o.id == id);
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
                    _uow.Repository<AuditRequestMonitorFile>().Update(self);
                }

                return Ok(new { code = "1", msg = "success", id = id });
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

        [HttpGet("ExportExcel")]
        public IActionResult Export(string jsonData)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var obj = JsonSerializer.Deserialize<AuditRequestMonitorExportModel>(jsonData);
                var code = obj.Code;
                var now = DateTime.Now.Date;
                var conclusion = (obj.Conclusion != "" ? Convert.ToInt32(obj.Conclusion) : 0);
                var unit = (obj.Unitid != "" ? Convert.ToInt32(obj.Unitid) : 0);
                var process = (obj.ProcessStatus != "" ? Convert.ToInt32(obj.ProcessStatus) : 0);
                var type = (obj.AuditRequestType != "" ? Convert.ToInt32(obj.AuditRequestType) : 0);
                DateTime? completedatefrom = null;
                DateTime? completedateto = null;
                DateTime? completeactualfrom = null;
                DateTime? completeactualto = null;
                if (!string.IsNullOrEmpty(obj.CompleteDateFrom))
                    completedatefrom = Convert.ToDateTime(obj.CompleteDateFrom);
                if (!string.IsNullOrEmpty(obj.CompleteDateTo))
                    completedateto = Convert.ToDateTime(obj.CompleteDateTo);
                if (!string.IsNullOrEmpty(obj.CompleteActualFrom))
                    completeactualfrom = Convert.ToDateTime(obj.CompleteActualFrom);
                if (!string.IsNullOrEmpty(obj.CompleteActualTo))
                    completeactualto = Convert.ToDateTime(obj.CompleteActualTo);
                var timestatus = Convert.ToInt32(obj.TimeStatus) == 2;

                var _unit = _uow.Repository<AuditFacility>().Find(a => a.IsActive == true).ToArray();

                var _user = _uow.Repository<Users>().Find(c => c.Id == userInfo.Id).FirstOrDefault();
                var _roles = _uow.Repository<UsersRoles>().Find(c => c.Id == userInfo.Id).FirstOrDefault();

                var _AuditRequestMonitor = (from a in _uow.Repository<AuditRequestMonitor>().Include(p => p.AuditDetect, p => p.CatAuditRequest, p => p.FacilityRequestMonitorMapping)
                                            join b in _uow.Repository<ApprovalFunction>().Find(p => p.function_code == "M_AD" && p.StatusCode == "3.1") on a.AuditDetect.id equals b.item_id.Value
                                            let checkstatus = (((a.ProcessStatus ?? 1) != 3 && (a.extend_at.HasValue ? a.extend_at < now : a.CompleteAt.HasValue ? a.CompleteAt < now : false)) || (a.ActualCompleteAt.HasValue && (a.extend_at.HasValue ? a.ActualCompleteAt > a.extend_at : a.CompleteAt.HasValue ? a.ActualCompleteAt > a.CompleteAt : false)))
                                            where (code == "" || a.Code == code)
                   && (process == 0 || (a.ProcessStatus ?? 1) == process)
                   && (type == 0 || (a.auditrequesttypeid ?? 1) == type)
                   && (obj.TimeStatus == "0" || checkstatus == timestatus)
                   && (conclusion == 0 || (a.Conclusion ?? 1) == conclusion)
                   && (a.is_deleted != true)
                   && ((!completedatefrom.HasValue || (completedatefrom.Value.Year <= a.CompleteAt.Value.Year && completedatefrom.Value.Month <= a.CompleteAt.Value.Month && completedatefrom.Value.Day <= a.CompleteAt.Value.Day)) && (!completedateto.HasValue || (a.CompleteAt.Value.Year <= completedateto.Value.Year && a.CompleteAt.Value.Month <= completedateto.Value.Month && a.CompleteAt.Value.Day <= completedateto.Value.Day)))
                   && ((!completeactualfrom.HasValue || (completeactualfrom.Value.Year <= a.ActualCompleteAt.Value.Year && completeactualfrom.Value.Month <= a.ActualCompleteAt.Value.Month && completeactualfrom.Value.Day <= a.ActualCompleteAt.Value.Day)) && (!completeactualto.HasValue || (a.ActualCompleteAt.Value.Year <= completeactualto.Value.Year && a.ActualCompleteAt.Value.Month <= completeactualto.Value.Month && a.ActualCompleteAt.Value.Day <= completeactualto.Value.Day)))
                    && (_user.UsersType != 2 || a.unitid == _user.DepartmentId)
                    && (unit == 0 || a.FacilityRequestMonitorMapping.FirstOrDefault(f => f.type == 1).audit_facility_id == unit)
                                            select a).OrderByDescending(p => p.created_at);

                //var _user = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsActive == true).ToArray();
                var _auditFacility = _uow.Repository<AuditFacility>().Find(a => a.Deleted != true).ToArray();
                var _auditWork = _uow.Repository<AuditWork>().Find(a => a.IsDeleted != true).ToArray();
                var _auditDetect = _uow.Repository<AuditDetect>().Find(a => a.IsDeleted != true).ToArray();

                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_ThongKeKienNghi.xlsx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                var template = new FileInfo(fullPath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Kien_nghi_kiem_toan"];
                    ExcelRange cellHeader = worksheet.Cells[1, 1];
                    cellHeader.Value = "BÁO CÁO TÌNH HÌNH THỰC HIỆN KIẾN NGHỊ KIỂM TOÁN NỘI BỘ";
                    cellHeader.Style.Font.Size = 11;
                    cellHeader.Style.Font.Bold = true;

                    ExcelRange cellDate = worksheet.Cells[2, 1];
                    cellDate.Value = "Ngày báo cáo " + DateTime.Now.ToString("dd/MM/yyyy");
                    cellDate.Style.Font.Size = 11;
                    cellDate.Style.Font.Bold = true;

                    var _startrow = 4;
                    var startrow = 4;
                    var startcol = 1;
                    var count = 0;

                    foreach (var a in _AuditRequestMonitor)
                    {
                        count++;
                        ExcelRange cellNo = worksheet.Cells[startrow, startcol];
                        cellNo.Value = count;

                        //var _riskitem = a.Where(x => x.Id == a.Id).ToArray();
                        //var _riskitem = a.id
                        var _auditWorkScopePlanFacilityItem = _auditFacility.Where(x => x.Id == a.unitid).ToArray();



                        ExcelRange cellrequesttype = worksheet.Cells[startrow, startcol + 1];
                        cellrequesttype.Value = string.Join(", ", a.CatAuditRequest.Name);

                        var _detectitem = _auditDetect.Where(x => x.id == a.detectid).ToArray();

                        foreach (var detect in _detectitem)
                        {
                            var _auditworkitem = _auditWork.Where(x => x.Id == detect.auditwork_id);
                            ExcelRange cellauditwork = worksheet.Cells[startrow, startcol + 2];

                            cellauditwork.Value = string.Join(", ", _auditworkitem.Select(x => x.Code).Distinct() ) +" "+ string.Join(", ", _auditworkitem.Select(x => x.Name).Distinct());

                            //var _unititem = _auditFacility.Where(x => x.Id == _auditworkitem.Select(x =>x.)).ToArray();


                        }
                        ExcelRange cellunit = worksheet.Cells[startrow, startcol + 6];
                        cellunit.Value = string.Join(", ", _unit.FirstOrDefault(u => u.Id == a.FacilityRequestMonitorMapping.FirstOrDefault(f => f.type == 1)?.audit_facility_id)?.Name);

                        ExcelRange celldetectcode = worksheet.Cells[startrow, startcol + 3];
                        celldetectcode.Value = string.Join(", ", _detectitem.Select(x => x.code).Distinct());

                        ExcelRange celldetecttext = worksheet.Cells[startrow, startcol + 4];
                        celldetecttext.Value = string.Join(", ", _detectitem.Select(x => x.description).Distinct());

                        ExcelRange cellcontent = worksheet.Cells[startrow, startcol + 5];
                        cellcontent.Value = string.Join(", ", a.Content);

                        ExcelRange celldate = worksheet.Cells[startrow, startcol + 7];
                        celldate.Value = string.Join(", ", a.CompleteAt.HasValue ? a.CompleteAt.Value.ToString("dd/MM/yyyy") : "");

                        ExcelRange cellextend = worksheet.Cells[startrow, startcol + 8];
                        cellextend.Value = string.Join(", ", a.extend_at.HasValue ? a.extend_at.Value.ToString("dd/MM/yyyy") : "");

                        var timeStatus = (((a.ProcessStatus ?? 1) != 3 && (a.extend_at.HasValue ? a.extend_at < now : a.CompleteAt.HasValue ? a.CompleteAt < now : false)) || (a.ActualCompleteAt.HasValue && (a.extend_at.HasValue ? a.ActualCompleteAt > a.extend_at : a.CompleteAt.HasValue ? a.ActualCompleteAt > a.CompleteAt : false))) ? 2 : 1;
                        ExcelRange cellstas = worksheet.Cells[startrow, startcol + 9];
                        if (timeStatus == 2)
                        {
                            cellstas.Value = string.Join(", ", "Quá hạn");
                        }
                        else
                        {
                            cellstas.Value = string.Join(", ", "Trong hạn");
                        }


                        ExcelRange yyyy = worksheet.Cells[startrow, startcol + 10];

                        if (a.ProcessStatus == 2)
                        {
                            yyyy.Value = string.Join(", ", "Hoàn thành một phần");
                        }
                        else if (a.ProcessStatus == 3)
                        {
                            yyyy.Value = string.Join(", ", "Hoàn thành");
                        }
                        else 
                        {
                            yyyy.Value = string.Join(", ", "Chưa hoàn thành");
                        }

                        //yyyy.Value = string.Join(", ", a.ProcessStatus);

                        ExcelRange cellconclusion = worksheet.Cells[startrow, startcol + 11];

                        if (a.Conclusion == 2)
                        {
                            cellconclusion.Value = string.Join(", ", "Đã đóng");
                        }
                        else
                        {
                            cellconclusion.Value = string.Join(", ", "Chưa đóng");
                        }

                       

                        ExcelRange cellactualcomplete = worksheet.Cells[startrow, startcol + 12];
                        cellactualcomplete.Value = string.Join(", ", a.ActualCompleteAt.HasValue ? a.ActualCompleteAt.Value.ToString("dd/MM/yyyy") : "");

                        ExcelRange cellincomplete = worksheet.Cells[startrow, startcol + 13];
                        cellincomplete.Value = string.Join(", ", a.incomplete_reason);

                        ExcelRange cellunitcomment = worksheet.Cells[startrow, startcol + 14];
                        cellunitcomment.Value = string.Join(", ", a.Unitcomment);

                        ExcelRange cellEvidence = worksheet.Cells[startrow, startcol + 15];
                        cellEvidence.Value = string.Join(", ", a.Evidence);


                        startrow++;
                    }
                    using ExcelRange r = worksheet.Cells[_startrow, startcol, startrow - 1, startcol + 15];
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
                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kitano_ThongKeKienNghi.xlsx");
                //return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}