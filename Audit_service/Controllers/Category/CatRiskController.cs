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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
//using AutoMapper;


namespace Audit_service.Controllers.Category
{
    [Route("[controller]")]
    [ApiController]
    public class CatRiskController : BaseController
    {
        protected readonly IConfiguration _config;
        ///protected readonly IMapper _mapper;

        //public CatRiskController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config ,  IMapper mapper) : base(logger, uow)
        //{
        //    _config = config;
        //    _mapper = mapper;
        //}

        public CatRiskController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<CatRiskSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.status);
                var activation = Convert.ToInt32(obj.activationid);
                var unit = Convert.ToInt32(obj.unitid);
                var process = Convert.ToInt32(obj.processid);

                Expression<Func<CatRisk, bool>> filter = c => (status == -1 || c.Status == status) && (activation == 0 || c.Activationid == activation) && (unit == 0 || c.Unitid == unit) && (process == 0 || c.Processid == process) && (string.IsNullOrEmpty(obj.code) || c.Code.ToLower().Contains(obj.code.ToLower())) && (string.IsNullOrEmpty(obj.name) || c.Name.ToLower().Contains(obj.name.ToLower())) && c.IsDeleted != true;
                var list_depart = _uow.Repository<CatRisk>().Find(filter).OrderByDescending(a => a.Createdate);
                IEnumerable<CatRisk> data = list_depart;
                var count = list_depart.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.IsDeleted != true).Select(a => new CatRiskModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Status = a.Status.ToString(),
                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
            }
            catch (Exception)
            {
                return Ok(new { code = "1", msg = "success", data = "", total = 0 });
            }
        }
        //test
        [HttpGet("SearchTest")]
        public IActionResult SearchTest(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<CatRiskSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.status);
                var activation = Convert.ToInt32(obj.activationid);
                var unit = Convert.ToInt32(obj.unitid);
                var process = Convert.ToInt32(obj.processid);
                var controlId = Convert.ToInt32(obj.controlid);

                var approval_status = _uow.Repository<RiskControl>().Find(a => a.controlid == controlId && a.isdeleted != true).ToArray();
                var _riskid = approval_status.Select(a => a.riskid).ToList();

                Expression<Func<CatRisk, bool>> filter = c => (status == -1 || c.Status == status) && (activation == 0 || c.Activationid == activation) && (unit == 0 || c.Unitid == unit) && (process == 0 || c.Processid == process) && (string.IsNullOrEmpty(obj.code) || c.Code.ToLower().Contains(obj.code.ToLower())) && (string.IsNullOrEmpty(obj.name) || c.Name.ToLower().Contains(obj.name.ToLower())) && c.IsDeleted != true && !_riskid.Contains(c.Id);
                var list_depart = _uow.Repository<CatRisk>().Find(filter).OrderByDescending(a => a.Createdate);

                //khi chọn check box lưu thì flag mới từ false sang true 
                IEnumerable<CatRisk> data = list_depart;
                var count = list_depart.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.IsDeleted != true).Select(a => new CatRiskModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Status = a.Status.ToString(),
                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
            }
            catch (Exception e)
            {
                return Ok(new { code = "1", msg = "success", data = "", total = 0 });
            }
        }

        [HttpGet("{id}")]
        public IActionResult Details(int? id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var CatRiskdetail = _uow.Repository<CatRisk>().FirstOrDefault(a => a.Id == id);
                if (CatRiskdetail != null)
                {
                    var getunit = _uow.Repository<AuditFacility>().FirstOrDefault(a => a.Id == CatRiskdetail.Unitid && a.Deleted != true && a.IsActive == true);
                    var checkCreatedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatRiskdetail.CreatedBy);
                    var checkModifiedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatRiskdetail.Editby);
                    var catriskinfo = new CatRiskDetailModel()
                    {
                        Id = CatRiskdetail.Id,
                        Code = CatRiskdetail.Code,
                        Name = CatRiskdetail.Name,
                        Description = CatRiskdetail.Description,
                        Status = CatRiskdetail.Status,
                        Unitid = CatRiskdetail.Unitid,
                        RiskType = CatRiskdetail.RiskType,
                        Activationid = CatRiskdetail.Activationid,
                        Processid = CatRiskdetail.Processid,
                        RelateStep = CatRiskdetail.RelateStep,
                        Unitname = getunit.Name ?? "",
                        IsDeleted = CatRiskdetail.IsDeleted,
                        CreatedBy = CatRiskdetail.CreatedBy,
                        Createdate = CatRiskdetail.Createdate != null ? CatRiskdetail.Createdate.Value.ToString("dd/MM/yyyy") : null,
                        Editby = CatRiskdetail.Editby,
                        Editdate = CatRiskdetail.Editdate != null ? CatRiskdetail.Editdate.Value.ToString("dd/MM/yyyy") : null,
                        createname = checkCreatedBy != null ? checkCreatedBy.UserName : "",
                        editname = checkModifiedBy != null ? checkModifiedBy.UserName : "",
                    };

                    return Ok(new { code = "1", msg = "success", data = catriskinfo });
                }
                else
                {
                    return Ok(new { code = "1", msg = "success", data = "" });
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("SearchList")]
        public IActionResult DetailsStrId(string strid)
        {
            try
            {
                var listid = string.IsNullOrEmpty(strid) ? new List<int>() : strid.Split(",").Select(x => Convert.ToInt32(x)).ToList();
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var catriskinfo = new List<CatRiskDetailModel>();
                var CatRiskdetail = _uow.Repository<CatRisk>().GetAll(a => listid.Contains(a.Id));
                if (CatRiskdetail != null)
                {

                    var getunit = _uow.Repository<AuditFacility>().GetAll(a => CatRiskdetail.Any(p => p.Unitid == a.Id) && a.Deleted != true && a.IsActive == true);
                    var checkCreatedBy = _uow.Repository<Users>().GetAll(a => CatRiskdetail.Any(p => p.CreatedBy == a.Id));
                    var checkModifiedBy = _uow.Repository<Users>().GetAll(a => CatRiskdetail.Any(p => p.Editby == a.Id));

                    foreach (var item in CatRiskdetail.ToList())
                    {
                        catriskinfo.Add(new CatRiskDetailModel()
                        {
                            Id = item.Id,
                            Code = item.Code,
                            Name = item.Name,
                            Description = item.Description,
                            Status = item.Status,
                            Unitid = item.Unitid,
                            RiskType = item.RiskType,
                            Activationid = item.Activationid,
                            Processid = item.Processid,
                            RelateStep = item.RelateStep,
                            Unitname = getunit.FirstOrDefault(p => p.Id == item.Unitid).Name ?? "",
                            IsDeleted = item.IsDeleted,
                            CreatedBy = item.CreatedBy,
                            Createdate = item.Createdate != null ? item.Createdate.Value.ToString("dd/MM/yyyy") : null,
                            Editby = item.Editby,
                            Editdate = item.Editdate != null ? item.Editdate.Value.ToString("dd/MM/yyyy") : null,
                            createname = checkCreatedBy.FirstOrDefault(p => p.Id == item.CreatedBy) != null ? checkCreatedBy.FirstOrDefault(p => p.Id == item.CreatedBy).UserName : "",
                            editname = checkModifiedBy.FirstOrDefault(p => p.Id == item.Editby) != null ? checkModifiedBy.FirstOrDefault(p => p.Id == item.Editby).UserName : "",
                        });
                    }
                    return Ok(new { code = "1", msg = "success", data = catriskinfo });
                }
                else
                {
                    return Ok(new { code = "1", msg = "success", data = "" });
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CatRiskModel catriskinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _allCatRisk = _uow.Repository<CatRisk>().Find(a => a.IsDeleted != true && a.Name.Equals(catriskinfo.Name)).ToArray();
                var checkcode = _uow.Repository<CatRisk>().Find(a => a.IsDeleted != true && a.Code.Equals(catriskinfo.Code)).ToArray();

                if (_allCatRisk.Length > 0)
                {
                    return Ok(new { code = "2", msg = "error" });
                }
                else if (checkcode.Length > 0)
                {
                    return Ok(new { code = "3", msg = "error" });
                }
                else
                {
                    var _CatRisk = new CatRisk();
                    _CatRisk.Name = catriskinfo.Name;
                    _CatRisk.Processid = catriskinfo.Processid;
                    _CatRisk.RelateStep = catriskinfo.RelateStep;
                    _CatRisk.Unitid = catriskinfo.Unitid;
                    _CatRisk.Activationid = catriskinfo.Activationid;
                    _CatRisk.Status = Convert.ToInt32(catriskinfo.Status);
                    _CatRisk.Code = catriskinfo.Code;
                    _CatRisk.Description = catriskinfo.Description;
                    _CatRisk.Createdate = DateTime.Now;
                    _CatRisk.CreatedBy = _userInfo.Id;
                    _CatRisk.RiskType = catriskinfo.risktype;
                    _uow.Repository<CatRisk>().Add(_CatRisk);
                    return Ok(new { code = "1", msg = "success" });
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Edit([FromBody] CatRiskModifyModel CatRiskinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var list_all = _uow.Repository<CatRisk>().GetAll().ToArray();

                var _CatRiskInfo = list_all.FirstOrDefault(a => a.Id == CatRiskinfo.Id);
                if (_CatRiskInfo == null)
                {
                    return NotFound();
                }

                var checkcode = _uow.Repository<CatRisk>().Find(a => a.IsDeleted != true && a.Code.Equals(CatRiskinfo.Code) && (a.Id != CatRiskinfo.Id)).ToArray();
                

                if (checkcode.Length >= 1)
                {
                    return Ok(new { code = "2", msg = "error" });
                }                
                else
                {
                    _CatRiskInfo.Id = CatRiskinfo.Id;
                    _CatRiskInfo.Unitid = CatRiskinfo.Unitid;
                    _CatRiskInfo.Code = CatRiskinfo.Code;
                    _CatRiskInfo.Activationid = CatRiskinfo.Activationid;
                    _CatRiskInfo.Processid = CatRiskinfo.Processid;
                    _CatRiskInfo.RelateStep = CatRiskinfo.RelateStep;
                    _CatRiskInfo.Name = CatRiskinfo.Name;
                    _CatRiskInfo.Status = CatRiskinfo.Status;
                    _CatRiskInfo.Description = CatRiskinfo.Description;
                    _CatRiskInfo.Editdate = DateTime.Now;
                    _CatRiskInfo.Editby = _userInfo.Id;
                    _CatRiskInfo.RiskType = CatRiskinfo.RiskType;
                    _uow.Repository<CatRisk>().Update(_CatRiskInfo);
                    return Ok(new { code = "1", msg = "success" });
                }
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
                var CatDetectType = _uow.Repository<CatRisk>().FirstOrDefault(a => a.Id == id);
                if (CatDetectType == null)
                {
                    return NotFound();
                }

                var _RiskControl = _uow.Repository<RiskControl>().Find(a => a.isdeleted != true).FirstOrDefault(a => a.riskid == id);
                var _ProcessLevelRiskScoring = _uow.Repository<ProcessLevelRiskScoring>().Find(a => a.IsDeleted != true).FirstOrDefault(a => a.catrisk_id == id);

                if (_ProcessLevelRiskScoring == null)
                {
                    if (_RiskControl == null) // neu khong co rang buoc thi cho xoa
                    {
                        CatDetectType.IsDeleted = true;
                        _uow.Repository<CatRisk>().Update(CatDetectType);
                        return Ok(new { code = "1", msg = "success" });
                    }
                    else // neu co bat ki rang buoc nao thi ko cho xoa
                    {
                        return Ok(new { code = "2", msg = "error" });
                    }
                }
                else
                {
                    return Ok(new { code = "3", msg = "error" });
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("ExportExcel")]
        public IActionResult Export()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var _CatRisk = _uow.Repository<CatRisk>().Find(a => a.IsDeleted != true).ToArray();
                var _user = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsActive == true).ToArray();
                var _auditFacility = _uow.Repository<AuditFacility>().Find(a => a.Deleted != true).ToArray();
                var _auditActivity = _uow.Repository<BussinessActivity>().Find(a => a.Deleted != true).ToArray();
                var _auditProcess = _uow.Repository<AuditProcess>().Find(a => a.Deleted != true).ToArray();

                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_Danh_Muc_Rui_Ro.xlsx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                var template = new FileInfo(fullPath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Danh_muc_RR"];
                    ExcelRange cellHeader = worksheet.Cells[1, 1];
                    cellHeader.Value = "DANH MỤC RỦI RO";
                    cellHeader.Style.Font.Size = 11;
                    cellHeader.Style.Font.Bold = true;
                    var _startrow = 4;
                    var startrow = 4;
                    var startcol = 1;
                    var count = 0;

                    foreach (var a in _CatRisk)
                    {
                        count++;
                        ExcelRange cellNo = worksheet.Cells[startrow, startcol];
                        cellNo.Value = count;

                        var _auditWorkScopePlanFacilityItem = _auditFacility.Where(x => x.Id == a.Unitid).ToArray();
                        var _activeitem = _auditActivity.Where(x => x.ID == a.Activationid).ToArray();
                        var _processitem = _auditProcess.Where(x => x.Id == a.Processid).ToArray();
                        var _riskitem = _CatRisk.Where(x => x.Id == a.Id).ToArray();

                        ExcelRange cellAuditFacility = worksheet.Cells[startrow, startcol + 1];
                        cellAuditFacility.Value = string.Join(", ", _auditWorkScopePlanFacilityItem.Select(x => x.Name).Distinct());

                        ExcelRange cellActivity = worksheet.Cells[startrow, startcol + 2];
                        cellActivity.Value = string.Join(", ", _activeitem.Select(x => x.Name).Distinct());

                        ExcelRange cellProcess = worksheet.Cells[startrow, startcol + 3];
                        cellProcess.Value = string.Join(", ", _processitem.Select(x => x.Name).Distinct());

                        ExcelRange cellstep = worksheet.Cells[startrow, startcol + 4];
                        cellstep.Value = string.Join(", ", _riskitem.Select(x => x.RelateStep).Distinct());

                        ExcelRange cellcode = worksheet.Cells[startrow, startcol + 5];
                        cellcode.Value = string.Join(", ", _riskitem.Select(x => x.Code).Distinct());

                        ExcelRange cellname = worksheet.Cells[startrow, startcol + 6];
                        cellname.Value = string.Join(", ", _riskitem.Select(x => x.Name).Distinct());

                        ExcelRange celldes = worksheet.Cells[startrow, startcol + 7];
                        celldes.Value = string.Join(", ", _riskitem.Select(x => x.Description).Distinct());

                        ExcelRange cellstas = worksheet.Cells[startrow, startcol + 8];

                        var status = string.Join(", ", _riskitem.Select(x => x.Status).Distinct());

                        if (Convert.ToInt32(status) == 1)
                        {
                            cellstas.Value = "Sử dụng";
                        }
                        else
                        {
                            cellstas.Value = "Không sử dụng";
                        }

                        ExcelRange celltypes = worksheet.Cells[startrow, startcol + 9];
                        var riskType = string.Join(", ", _riskitem.Select(x => x.RiskType).Distinct());
                        if (riskType !=null && riskType != "" )
                        {
                            celltypes.Value = _uow.Repository<SystemCategory>().FirstOrDefault(a => a.Code == riskType && a.ParentGroup == "LoaiRuiRoQuyTrinh").Name;                            
                        }    
                        else
                        {
                            celltypes.Value = "";
                        }    

                        startrow++;
                    }
                    using ExcelRange r = worksheet.Cells[_startrow, startcol, startrow - 1, startcol + 9];
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
                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kitano_Danh_Muc_Rui_Ro.xlsx");
                //return Ok(new { code = "1", data = Bytes, file_name = "Ke_hoach_kiem_toan_nam.xlsx",   msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost("Upload"), DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];

                string[] contentType = new string[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel" };
                var formats = new string[] { ".xlsx", ".xls" };

                if (!contentType.Any(o => o == file.ContentType) || !formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest("File type is not allow!");
                }

                var sizeConfig = string.IsNullOrEmpty(_config["Upload:MaxLength"]) ? "10" : _config["Upload:MaxLength"];

                if (!int.TryParse(sizeConfig, out int allowSize))
                {
                    allowSize = 10;
                }

                if (file.Length > 0 && file.Length < allowSize * 1024 * 1024)
                {
                    if (file.Length > 0)
                    {
                        var data = new List<CatRiskModel>();

                        using (var fileStream = new MemoryStream())
                        {
                            file.CopyTo(fileStream);
                            var mappingCl = new Dictionary<string, string>() {
                                {"STT","No"},
                                {"Đơn vị liên quan","unitname"},
                                {"Hoạt động liên quan","activename"},
                                {"Quy trình","processname"},
                                {"Bước liên quan","relatestep"},
                                {"Mã rủi ro","code"},
                                {"Tên rủi ro","name"},
                                {"Mô tả rủi ro","description"},
                                {"Trạng thái","statusname"},
                                {"Loại rủi ro","risktypename"}
                            };
                            data = Utils.ExcelFn.UploadToListFnc<CatRiskModel>(fileStream, 0, true, 2, mappingCl);
                        }

                        bool hasError = false;
                        //var trans = _uow.BeginTransaction();
                        var userInfo = HttpContext.Items["UserInfo"] as CurrentUserModel;

                        var allCode = data.Where(o => o.Code != null && o.Code != "").Select(o => o.Code).Distinct().ToList();

                        var codeInDb = _uow.Repository<CatRisk>().Find(o => o.Code != null && o.Code != "" && o.IsDeleted != true).Select(o => o.Code).Distinct().ToList();

                        var s = codeInDb.Where(o => allCode.Contains(o)).Select(o => o).Distinct().ToList();

                        var allParentCode = codeInDb.Select(o => o).ToList();

                        var unitlist = _uow.Repository<AuditFacility>().Find(o => o.Code != null && o.Code != "" && o.Deleted != true).ToList();
                        var activelist = _uow.Repository<BussinessActivity>().Find(o => o.Code != null && o.Code != "" && o.Deleted != true).ToList();
                        var processlist = _uow.Repository<AuditProcess>().Find(o => o.Code != null && o.Code != "" && o.Deleted != true).ToList();
                        var risktypelist = _uow.Repository<SystemCategory>().Find(o => o.Code != null && o.Code != "" && o.Deleted != true && o.ParentGroup == "LoaiRuiRoQuyTrinh").ToList();
                        allParentCode.AddRange(allCode);

                        data.ForEach(o =>
                        {
                            o.Valid = true;
                            if (string.IsNullOrEmpty(o.Name))
                            {
                                o.Valid = false;
                                o.Note += "803,";
                            }
                            var unit = unitlist.FirstOrDefault(a => a.Name == o.unitname);
                            if (unit == null)
                            {
                                o.Valid = false;
                                o.Note += "805,";
                            }
                            else
                            {
                                o.Unitid = unit.Id;
                                o.unitname = unit.Name;
                            }

                            var active = activelist.FirstOrDefault(a => a.Name == o.activename);
                            if (active == null)
                            {
                                o.Activationid = null;
                                o.activename = "";
                            }
                            else
                            {
                                o.Activationid = active.ID;
                                o.activename = active.Name;
                            }

                            var process = processlist.FirstOrDefault(a => a.Name == o.processname && a.ActivityId == o.Activationid && a.FacilityId == o.Unitid);
                            if (process == null)
                            {
                                o.Valid = false;
                                o.Note += "805,";
                            }
                            else
                            {
                                o.Processid = process.Id;
                                o.processname = process.Name;
                            }

                            if (o.statusname == "Sử dụng")
                            {
                                o.Status = "1";
                            }
                            else
                            {
                                o.Status = "0";
                            }

                            var risktype = risktypelist.FirstOrDefault(a => a.Name == o.risktypename.ToString());
                            if (risktype == null)
                            {
                                o.Valid = false;
                                o.Note += "805,";
                            }
                            else
                            {
                                o.risktype = Convert.ToInt32(risktype.Code);
                                //o.activename = risktype.Name;
                            }

                            if (!string.IsNullOrEmpty(o.Code) && s.Contains(o.Code))
                            {
                                if (s.Contains(o.Code))
                                {
                                    o.Valid = false;
                                    o.Note += "801,";
                                }
                                var countInExcel = allCode.Count(a => a == o.Code);
                                if (countInExcel > 1)
                                {
                                    o.Valid = false;
                                    o.Note += "807,";
                                }
                            }

                            if (o.Valid)
                                o.Note = "000";
                        });

                        hasError = data.Any(o => !o.Valid);                        
                        if (!hasError)                        
                        {
                            var batch1 = data.Where(o => o.Valid);
                            foreach (var item in batch1)
                            {
                                var obj = new CatRisk();

                                obj.Name = item.Name;
                                obj.Processid = item.Processid;
                                obj.RelateStep = item.RelateStep;
                                obj.Unitid = item.Unitid;
                                obj.Activationid = item.Activationid;
                                obj.Status = Convert.ToInt32(item.Status);
                                obj.Code = item.Code;
                                obj.Description = item.Description.ToString().TrimEnd(Environment.NewLine.ToCharArray());
                                obj.CreatedBy = userInfo.Id;
                                obj.Createdate = DateTime.Now;
                                obj.RiskType = item.risktype;
                                _uow.Repository<CatRisk>().Add(obj);
                            }
                        }
                        return Ok(new { code = "001", data = data, total = data.Count });
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest("File too large!");
                }
            }
            catch (Exception ex)
            {
                return Ok(new { code = "800" });
            }
        }

        [HttpPost("Download")]
        public IActionResult Download()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_Danh_Muc_Rui_Ro.xlsx");

                var _auditFacility = _uow.Repository<AuditFacility>().Find(a => a.Deleted != true && a.IsActive == true).ToArray();
                var _auditActivity = _uow.Repository<BussinessActivity>().Find(a => a.Deleted != true && a.Status == true).ToArray();
                var _auditProcess = _uow.Repository<AuditProcess>().Find(a => a.Deleted != true && a.Status == true).ToArray();
                var _riskType = _uow.Repository<SystemCategory>().Find(a => a.Deleted != true && a.Status == true && a.ParentGroup == "LoaiRuiRoQuyTrinh").ToArray();

                fullPath = fullPath.ToString().Replace("\\", "/");
                var template = new FileInfo(fullPath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Danh_muc_RR"];
                    ExcelRange cellHeader = worksheet.Cells[1, 1];
                    cellHeader.Value = "DANH MỤC RỦI RO";
                    cellHeader.Style.Font.Size = 11;
                    cellHeader.Style.Font.Bold = true;
                    var _startrow = 4;
                    var startrow = 4;
                    var startcol = 1;
                    var startrow1 = 1;
                    var startrow2 = 1;
                    var startrow3 = 1;
                    var startrow4 = 1;
                    using ExcelRange r = worksheet.Cells[_startrow, startcol, startrow + 1, startcol + 9];
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                    var worksheet2 = excelPackage.Workbook.Worksheets["Danh_sach_hoat_dong"];

                    foreach (var a in _auditActivity)
                    {
                        var _activeitem = _auditActivity.Where(x => x.ID == a.ID).ToArray();
                        ExcelRange cellActivity = worksheet2.Cells[startrow1, 1];
                        cellActivity.Value = string.Join(", ", _activeitem.Select(x => x.Name).Distinct());

                        startrow1++;
                    }

                    var worksheet3 = excelPackage.Workbook.Worksheets["Danh_sach_don_vi"];
                    foreach (var a in _auditFacility)
                    {
                        var _activeitem = _auditFacility.Where(x => x.Id == a.Id).ToArray();
                        ExcelRange cellActivity = worksheet3.Cells[startrow2, 1];
                        cellActivity.Value = string.Join(", ", _activeitem.Select(x => x.Name).Distinct());

                        startrow2++;
                    }

                    var worksheet4 = excelPackage.Workbook.Worksheets["Danh_sach_quy_trinh"];
                    foreach (var a in _auditProcess)
                    {
                        var _activeitem = _auditProcess.Where(x => x.Id == a.Id).ToArray();
                        ExcelRange cellActivity = worksheet4.Cells[startrow3, 1];
                        cellActivity.Value = string.Join(", ", _activeitem.Select(x => x.Name).Distinct());

                        startrow3++;
                    }

                    var worksheet5 = excelPackage.Workbook.Worksheets["Danh_sach_loai_rui_ro"];
                    foreach (var a in _riskType)
                    {
                        var _activeitem = _riskType.Where(x => x.Id == a.Id).ToArray();
                        ExcelRange cellRiskType = worksheet5.Cells[startrow4, 1];
                        cellRiskType.Value = string.Join(", ", _activeitem.Select(x => x.Name).Distinct());

                        startrow4++;
                    }

                    Bytes = excelPackage.GetAsByteArray();
                }

                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kitano_Danh_Muc_Rui_Ro.xlsx");
                //return Ok(new { code = "1", data = Bytes, file_name = "Ke_hoach_kiem_toan_nam.xlsx",   msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }//[HttpPost("Upload"), DisableRequestSizeLimit]        
    }
}