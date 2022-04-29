using Audit_service.DataAccess;
using Audit_service.Models.ExecuteModels;
using Audit_service.Models.MigrationsModels;
using Audit_service.Repositories;
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
using Audit_service.Utils;
using NPOI.OpenXmlFormats.Dml.Chart;

namespace Audit_service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CatControlController : BaseController
    {
        protected readonly IConfiguration _config;
        public CatControlController(ILoggerManager logger, IUnitOfWork uow , IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<CatControlSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.Status);
                var activation = Convert.ToInt32(obj.Activationid);
                var unit = Convert.ToInt32(obj.Unitid);
                var process = Convert.ToInt32(obj.Processid);

                Expression<Func<CatControl, bool>> filter = c => (status == -1 || c.Status == status) && (activation == 0 || c.Activationid == activation) && (unit == 0 || c.Unitid == unit) && (process == 0 || c.Processid == process) && (string.IsNullOrEmpty(obj.Code) || c.Code.ToLower().Contains(obj.Code.ToLower())) && (string.IsNullOrEmpty(obj.Name) || c.Name.ToLower().Contains(obj.Name.ToLower())) && c.IsDeleted != true;
                var list_depart = _uow.Repository<CatControl>().Find(filter).OrderByDescending(a => a.Createdate);
                IEnumerable<CatControl> data = list_depart;
                var count = list_depart.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.IsDeleted != true).Select(a => new CatControlModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Status = a.Status,
                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
            }
            catch (Exception e)
            {
                return Ok(new { code = "1", msg = "success", data = "", total = 0 });
            }
        }

        [HttpGet("SearchDocument")]
        public IActionResult SearchDocument(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<DocumentSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.Status);
                var unitid = Convert.ToInt32(obj.unit_id);
                var controlId = Convert.ToInt32(obj.controlid);

                var approval_status = _uow.Repository<ControlDocument>().Find(a => a.controlid == controlId && a.isdeleted != true).ToArray();
                var _docid = approval_status.Select(a => a.documentid).ToList();

                Expression<Func<Document, bool>> filter = c => (string.IsNullOrEmpty(obj.name) || c.name.ToLower().Contains(obj.name.ToLower())) && (status == -1 || (status == 1 ? c.status == true : c.status != true)) && (string.IsNullOrEmpty(obj.unit_id.ToString()) || unitid == 0 || c.unit_id == unitid) && (string.IsNullOrEmpty(obj.code.ToString()) || c.code == obj.code) && c.isdeleted != true && !_docid.Contains(c.id);
                var list_depart = _uow.Repository<Document>().Find(filter).OrderByDescending(a => a.CreatedAt);
                var _unit = _uow.Repository<AuditFacility>().Find(a => a.IsActive == true).ToArray();
                IEnumerable<Document> data = list_depart;
                var count = list_depart.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.isdeleted != true).Select(a => new DocumentModel()
                {
                    Id = a.id,
                    name = a.name,
                    Code = a.code,                    
                    description = a.description,                                        
                    unitname = a.unit_id.HasValue ? _unit.FirstOrDefault(u => u.Id == a.unit_id)?.Name : "",
                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
            }
            catch (Exception e)
            {
                return Ok(new { code = "0", msg = "fail", data = new DocumentModel(), total = 0 });
            }
        }

        [HttpGet("GetRiskControl/{id}")]
        public IActionResult GetCatControl(int? id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var list_cat_control = _uow.Repository<RiskControl>().Include(c => c.CatControl).Where(c => c.isdeleted != true & c.riskid == id).ToArray();
                IEnumerable<RiskControl> data = list_cat_control;

                var result = data.Select(a => new CatControlModel()
                {
                    Id = a.CatControl?.Id,
                    Name = a.CatControl?.Name,
                    Code = a.CatControl?.Code,
                });
                return Ok(new { code = "1", msg = "success", data = result });
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
                var CatControldetail = _uow.Repository<CatControl>().FirstOrDefault(a => a.Id == id);
                var getunit = _uow.Repository<AuditFacility>().FirstOrDefault(a => a.Id == CatControldetail.Unitid && a.Deleted != true && a.IsActive == true);
                var checkCreatedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatControldetail.CreatedBy);
                var checkModifiedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatControldetail.Editby);
                if (CatControldetail != null)
                {
                    var CatControlinfo = new CatControlDetailModel()
                    {
                        Id = CatControldetail.Id,
                        Code = CatControldetail.Code,
                        Name = CatControldetail.Name,
                        Description = CatControldetail.Description,
                        Status = CatControldetail.Status,
                        Unitid = CatControldetail.Unitid,
                        Activationid = CatControldetail.Activationid,
                        Processid = CatControldetail.Processid,
                        RelateStep = CatControldetail.RelateStep,
                        Unitname = getunit.Name ?? "",
                        ActualControl = CatControldetail.ActualControl,
                        IsDeleted = CatControldetail.IsDeleted,
                        Controlformat = CatControldetail.Controlformat,
                        Controltype = CatControldetail.Controltype,
                        Controlfrequency = CatControldetail.Controlfrequency,
                        CreatedBy = CatControldetail.CreatedBy,
                        Createdate = CatControldetail.Createdate != null ? CatControldetail.Createdate.Value.ToString("dd/MM/yyyy") : null,
                        Editby = CatControldetail.Editby,
                        Editdate = CatControldetail.Editdate != null ? CatControldetail.Editdate.Value.ToString("dd/MM/yyyy") : null,
                        createname = checkCreatedBy != null ? checkCreatedBy.UserName : "",
                        editname = checkModifiedBy != null ? checkModifiedBy.UserName : "",
                    };
                    return Ok(new { code = "1", msg = "success", data = CatControlinfo });
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

        [HttpPost]
        public IActionResult Create([FromBody] CatControlModel catcontrolinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                //var _allCatControl = _uow.Repository<CatControl>().Find(a => a.IsDeleted != true && a.Name.Equals(catcontrolinfo.Name)).ToArray();
                var checkcode = _uow.Repository<CatControl>().Find(a => a.IsDeleted != true && a.Code.Equals(catcontrolinfo.Code)).ToArray();

                if (checkcode.Length > 0)
                {
                    return Ok(new { code = "3", msg = "error" });
                }
                else
                {
                    var _CatControl = new CatControl();
                    _CatControl.Name = catcontrolinfo.Name;
                    _CatControl.Processid = catcontrolinfo.Processid;
                    _CatControl.RelateStep = catcontrolinfo.RelateStep;
                    _CatControl.Unitid = catcontrolinfo.Unitid;
                    _CatControl.Activationid = catcontrolinfo.Activationid;
                    _CatControl.Status = catcontrolinfo.Status;
                    _CatControl.Code = catcontrolinfo.Code;
                    _CatControl.Description = catcontrolinfo.Description;
                    _CatControl.ActualControl = catcontrolinfo.ActualControl;
                    _CatControl.Createdate = DateTime.Now;
                    _CatControl.CreatedBy = _userInfo.Id;
                    _CatControl.Controlformat = Convert.ToInt32(catcontrolinfo.controlformat);
                    _CatControl.Controlfrequency = Convert.ToInt32(catcontrolinfo.controlfrequency);
                    _CatControl.Controltype = Convert.ToInt32(catcontrolinfo.controltype);
                    _uow.Repository<CatControl>().Add(_CatControl);

                    var list_control = _uow.Repository<CatControl>().Find(c => c.Name == catcontrolinfo.Name && c.Code == catcontrolinfo.Code).FirstOrDefault();
                    var risk = catcontrolinfo.ListRisk.ToList();
                    if (risk != null)
                    {
                        for (int i = 0; i < risk.Count(); i++)
                        {
                            var list_riskcontrol = _uow.Repository<RiskControl>().GetAll().OrderByDescending(a => a.Id).FirstOrDefault();
                            var _riskcontrol = new RiskControl();
                            if (list_riskcontrol != null )
                            {
                                _riskcontrol.Id = list_riskcontrol.Id + 1;
                            }
                            else
                            {
                                _riskcontrol.Id = 1;
                            }                           

                            _riskcontrol.riskid = Convert.ToInt32(risk[i].riskid);
                            _riskcontrol.controlid = list_control.Id;

                            _uow.Repository<RiskControl>().Add(_riskcontrol);
                            _uow.SaveChanges();
                        }
                    }
                    var document = catcontrolinfo.ListDocument.ToList();
                    if (document != null)
                    {
                        for (int i = 0; i < document.Count(); i++)
                        {
                            var _controldocument = new ControlDocument();
                            // Any random integer   
                            _controldocument.Id = Guid.NewGuid();
                            _controldocument.documentid = document[i].document_id;
                            _controldocument.controlid = list_control.Id;

                            _uow.Repository<ControlDocument>().Add(_controldocument);
                            _uow.SaveChanges();
                        }
                    }
                    return Ok(new { code = "1", msg = "success" });
                }
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Edit([FromBody] CatControlModifyModel CatControlinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var list_all = _uow.Repository<CatControl>().GetAll().ToArray();
                var _CatControlInfo = list_all.FirstOrDefault(a => a.Id == CatControlinfo.Id);
                if (_CatControlInfo == null)
                {
                    return NotFound();
                }

                var checkcode = _uow.Repository<CatControl>().Find(a => a.IsDeleted != true && a.Code.Equals(CatControlinfo.Code) && (a.Id != CatControlinfo.Id)).ToArray();
                
                if (checkcode.Length >= 1)
                {
                    return Ok(new { code = "3", msg = "error" });
                }
                else
                {
                    _CatControlInfo.Id = CatControlinfo.Id;
                    _CatControlInfo.Unitid = CatControlinfo.Unitid;
                    _CatControlInfo.Activationid = CatControlinfo.Activationid;
                    _CatControlInfo.Processid = CatControlinfo.Processid;
                    _CatControlInfo.RelateStep = CatControlinfo.RelateStep;
                    _CatControlInfo.Name = CatControlinfo.Name;
                    _CatControlInfo.Code = CatControlinfo.Code;
                    _CatControlInfo.Status = CatControlinfo.Status;
                    _CatControlInfo.Description = CatControlinfo.Description;
                    _CatControlInfo.ActualControl = CatControlinfo.ActualControl;
                    _CatControlInfo.Controlformat = CatControlinfo.Controlformat;
                    _CatControlInfo.Controlfrequency = CatControlinfo.Controlfrequency;
                    _CatControlInfo.Controltype = CatControlinfo.Controltype;
                    _CatControlInfo.Editdate = DateTime.Now;
                    _CatControlInfo.Editby = _userInfo.Id;
                }

                _uow.Repository<CatControl>().Update(_CatControlInfo);

                var currenrisk = _uow.Repository<RiskControl>().Find(a => (a.controlid == CatControlinfo.Id) && (a.isdeleted != true)).OrderByDescending(a => a.Id).ToList();
                foreach (var riskdelete in currenrisk)
                {
                    if (riskdelete.flag == 1)
                    {
                        riskdelete.isdeleted = true;
                        _uow.Repository<RiskControl>().Update(riskdelete);
                        _uow.SaveChanges();
                    }
                }

                var currentdoc = _uow.Repository<ControlDocument>().Find(a => (a.controlid == CatControlinfo.Id) && (a.isdeleted != true)).OrderByDescending(a => a.Id).ToList();
                foreach (var docdelete in currentdoc)
                {
                    if (docdelete.flag == 1)
                    {
                        docdelete.isdeleted = true;
                        _uow.Repository<ControlDocument>().Update(docdelete);
                        _uow.SaveChanges();
                    }
                }

                var risk = CatControlinfo.ListRiskEdit.ToList();
                if (risk != null)
                {
                    for (int i = 0; i < risk.Count(); i++)
                    {
                        if (risk[i].riskid != null)
                        {
                            var _riskcontrol = new RiskControl();
                            var list_riskcontrol = _uow.Repository<RiskControl>().GetAll().OrderByDescending(a => a.Id).FirstOrDefault();
                            if (list_riskcontrol != null)
                            {
                                _riskcontrol.Id = list_riskcontrol.Id + 1;
                            }
                            else
                            {
                                _riskcontrol.Id = 1;
                            }
                            _riskcontrol.riskid = Convert.ToInt32(risk[i].riskid);
                            _riskcontrol.controlid = CatControlinfo.Id;

                            var list_control = _uow.Repository<RiskControl>().Find(c => (c.riskid == _riskcontrol.riskid) && (c.controlid == _riskcontrol.controlid) && (c.isdeleted !=true) ).FirstOrDefault();

                            if (list_control == null)
                            {
                                _uow.Repository<RiskControl>().Add(_riskcontrol);
                                _uow.SaveChanges();
                            }
                        }
                    }
                }                

                var document = CatControlinfo.ListDocumentEdit.ToList();

                if (document != null)
                {
                    for (int i = 0; i < document.Count(); i++)
                    {
                        var _controldocument = new ControlDocument();
                        // Any random integer   
                        _controldocument.Id = Guid.NewGuid();
                        _controldocument.documentid = document[i].document_id;
                        _controldocument.controlid = CatControlinfo.Id;
                        var list_document = _uow.Repository<ControlDocument>().Find(c => (c.documentid == _controldocument.documentid) && (c.controlid == _controldocument.controlid) && (c.isdeleted != true)).FirstOrDefault();
                        if (list_document == null)
                        {
                            _uow.Repository<ControlDocument>().Add(_controldocument);
                            _uow.SaveChanges();
                        }
                    }
                }
                return Ok(new { code = "1", msg = "success" });
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
                var CatDetectType = _uow.Repository<CatControl>().FirstOrDefault(a => a.Id == id);
                if (CatDetectType == null)
                {
                    return NotFound();
                }

                var _catap = _uow.Repository<CatAuditProcedures>().Find(a => a.IsDeleted != true).FirstOrDefault(a => a.cat_control_id == id);
                if (_catap == null)
                {
                    CatDetectType.IsDeleted = true;
                    _uow.Repository<CatControl>().Update(CatDetectType);
                    var riskcontrol = _uow.Repository<RiskControl>().GetAll(a => a.controlid == id).ToList();
                    foreach (var risk in riskcontrol)
                    {
                        risk.isdeleted = true;
                        _uow.Repository<RiskControl>().Update(risk);
                    }
                    return Ok(new { code = "1", msg = "success" });
                }
                else
                {
                    return Ok(new { code = "2", msg = "error" });
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
                var _CatControl = _uow.Repository<CatControl>().Find(a => a.IsDeleted != true).ToArray();
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_Danh_Muc_Kiem_Soat.xlsx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                var template = new FileInfo(fullPath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Danh_muc_kiem_soat"];
                    ExcelRange cellHeader = worksheet.Cells[1, 1];
                    cellHeader.Value = "DANH MỤC KIỂM SOÁT";
                    cellHeader.Style.Font.Size = 11;
                    cellHeader.Style.Font.Bold = true;
                    var _startrow = 4;
                    var startrow = 4;
                    var startcol = 1;
                    var count = 0;

                    foreach (var a in _CatControl)
                    {
                        count++;
                        ExcelRange cellNo = worksheet.Cells[startrow, startcol];
                        cellNo.Value = count;

                        var _auditWorkScopePlanFacilityItem = _auditFacility.Where(x => x.Id == a.Unitid).ToArray();
                        var _activeitem = _auditActivity.Where(x => x.ID == a.Activationid).ToArray();
                        var _processitem = _auditProcess.Where(x => x.Id == a.Processid).ToArray();
                        var _controlitem = _CatControl.Where(x => x.Id == a.Id).ToArray();

                        var _list_risk = _uow.Repository<RiskControl>().Include(a => a.CatRisk).Where(c => (c.controlid == a.Id)).OrderByDescending(a => a.Id).Select(a => new CatRiskSearchModel()
                        {
                            Riskcontrolid = a.Id,
                            Id = a.CatRisk.Id,
                            name = a.CatRisk.Code + ":" + a.CatRisk.Name,
                            Description = a.CatRisk.Description,
                            code = a.CatRisk.Code

                        });

                        var _list_document = _uow.Repository<ControlDocument>().Include(a => a.Document).Where(c => (c.controlid == a.Id)).Select(a => new DocumentModel()
                        {
                            controldocumentid = a.Id,
                            Id = a.Document.id,
                            name = a.Document.name,
                            Code = a.Document.code
                        });

                        ExcelRange cellAuditFacility = worksheet.Cells[startrow, startcol + 1];
                        cellAuditFacility.Value = string.Join(", ", _auditWorkScopePlanFacilityItem.Select(x => x.Name).Distinct());

                        ExcelRange cellActivity = worksheet.Cells[startrow, startcol + 2];
                        cellActivity.Value = string.Join(", ", _activeitem.Select(x => x.Name).Distinct());

                        ExcelRange cellProcess = worksheet.Cells[startrow, startcol + 3];
                        cellProcess.Value = string.Join(", ", _processitem.Select(x => x.Name).Distinct());

                        ExcelRange cellstep = worksheet.Cells[startrow, startcol + 4];
                        cellstep.Value = string.Join(", ", _controlitem.Select(x => x.Code).Distinct());

                        ExcelRange cellcode = worksheet.Cells[startrow, startcol + 5];
                        cellcode.Value = string.Join(", ", _controlitem.Select(x => x.Name).Distinct());

                        ExcelRange cellname = worksheet.Cells[startrow, startcol + 6];
                        cellname.Value = string.Join(", ", _controlitem.Select(x => x.Description).Distinct());

                        ExcelRange celldes = worksheet.Cells[startrow, startcol + 7];
                        celldes.Value = string.Join(", ", _controlitem.Select(x => x.ActualControl).Distinct());

                        ExcelRange cellFrequency = worksheet.Cells[startrow, startcol + 8];

                        var frequnecy = string.Join(", ", _controlitem.Select(x => x.Controlfrequency).Distinct());

                        if (Convert.ToInt32(frequnecy) == 1)
                        {
                            cellFrequency.Value = "Mỗi khi phát sinh";
                        }    
                         else if (Convert.ToInt32(frequnecy) == 2)
                        {
                            cellFrequency.Value = "Nhiều lần trong ngày";
                        }
                        else if (Convert.ToInt32(frequnecy) == 3)
                        {
                            cellFrequency.Value = "Hàng ngày";
                        }
                        else if (Convert.ToInt32(frequnecy) == 4)                            
                        {
                            cellFrequency.Value = "Hàng tuần";
                        }
                        else if (Convert.ToInt32(frequnecy) == 5)
                        {
                            cellFrequency.Value = "Hàng tháng";
                        }
                        else if (Convert.ToInt32(frequnecy) == 6)
                        {
                            cellFrequency.Value = "Hàng quý";
                        }
                        else if (Convert.ToInt32(frequnecy) == 7)
                        {
                            cellFrequency.Value = "Hàng năm";
                        }
                        else 
                        {
                            cellFrequency.Value = "";
                        }

                        ExcelRange celltype = worksheet.Cells[startrow, startcol + 9];
                        var type = string.Join(", ", _controlitem.Select(x => x.Controltype).Distinct());

                        if (Convert.ToInt32(type) == 1)
                        {
                            celltype.Value = "phòng ngừa";
                        }
                        else if (Convert.ToInt32(type) == 2)
                        {
                            celltype.Value = "phát hiện";
                        }
                        else 
                        {
                            celltype.Value = "";
                        }
                         
                        ExcelRange cellformat = worksheet.Cells[startrow, startcol + 10];
                        var format = string.Join(", ", _controlitem.Select(x => x.Controlformat).Distinct()); 
                        if (Convert.ToInt32(format) == 1)
                        {
                            cellformat.Value = "Tự động";
                        }
                        else if (Convert.ToInt32(format) == 2)
                        {
                            cellformat.Value = "Bán tự động";
                        }
                        else if (Convert.ToInt32(format) == 3)
                        {
                            cellformat.Value = "Thủ công";
                        }
                        else
                        {
                            cellformat.Value = "";
                        }

                        ExcelRange cellrisk = worksheet.Cells[startrow, startcol + 11];
                        cellrisk.Value = string.Join(", ", _list_risk.Select(x => x.name).Distinct());

                        ExcelRange cellstas = worksheet.Cells[startrow, startcol + 12];
                        var status = string.Join(", ", _controlitem.Select(x => x.Status).Distinct());

                        if (Convert.ToInt32(status) == 1)
                        {
                            cellstas.Value = "Sử dụng";
                        }
                        else
                        {
                            cellstas.Value = "Không Sử dụng";
                        }

                        ExcelRange cellDoc = worksheet.Cells[startrow, startcol + 13];
                        cellDoc.Value = string.Join(", ", _list_document.Select(x => x.Code).Distinct());

                        startrow++;
                    }
                    using ExcelRange r = worksheet.Cells[_startrow, startcol, startrow - 1, startcol + 13];
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
                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kitano_Danh_Muc_Kiem_Soat.xlsx");
                //return Ok(new { code = "1", data = Bytes, file_name = "Ke_hoach_kiem_toan_nam.xlsx",   msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
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
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_DanhMucKiemSoat_v0.1.xlsx");
                var _auditFacility = _uow.Repository<AuditFacility>().Find(a => a.Deleted != true && a.IsActive == true).ToList();
                var _businessActivity = _uow.Repository<BussinessActivity>().Find(a => a.Deleted != true && a.Status == true).ToList();
                var _auditProcess = _uow.Repository<AuditProcess>().Find(a => a.Deleted != true && a.Status == true).ToArray();
                var _frequencyControl = _uow.Repository<SystemCategory>().Find(a => a.Deleted != true && a.Status == true && a.ParentGroup == "TanSuatKiemSoat").ToList();
                var _controlType = _uow.Repository<SystemCategory>().Find(a => a.Deleted != true && a.Status == true && a.ParentGroup == "LoaiKiemSoat").ToList();
                var _formalControl = _uow.Repository<SystemCategory>().Find(a => a.Deleted != true && a.Status == true && a.ParentGroup == "HinhThucKiemSoat").ToList();

                var lookup = _auditFacility.ToLookup(x => x.ParentId);
                IEnumerable<AuditFacility> FlattenAuditFacility(int? parentId)
                {
                    foreach (var node in lookup[parentId])
                    {
                        yield return node;
                        foreach (var child in FlattenAuditFacility(node.Id))
                        {
                            yield return child;
                        }
                    }
                }
                _auditFacility = FlattenAuditFacility(null).ToList();
                var facilityDic = new Dictionary< int, int?>();
                _auditFacility.ForEach(o => {facilityDic.Add(o.Id, o.ParentId);});

                var lookupBA = _businessActivity.ToLookup(x => x.ParentId);
                IEnumerable<BussinessActivity> FlattenAuditBa(int? parentId)
                {
                    foreach (var node in lookupBA[parentId])
                    {
                        yield return node;
                        foreach (var child in FlattenAuditBa(node.ID))
                        {
                            yield return child;
                        }
                    }
                }
                _businessActivity = FlattenAuditBa(null).ToList();

                var sBaDic = new Dictionary<int, int?>();
                _businessActivity.ForEach(o => { sBaDic.Add(o.ID, o.ParentId); });

                fullPath = fullPath.ToString().Replace("\\", "/");
                var template = new FileInfo(fullPath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Danh_muc_kiem_soat"];
                    ExcelRange headercell = worksheet.Cells[1, 1];
                    headercell.Value = "Danh mục kiểm soát";
                    headercell.Style.Font.Size = 12;
                    headercell.Style.Font.Bold = true;

                    var notecell = worksheet.Cells[2,2];
                    notecell.Value = "Ghi chú:\r\n(*): các trường bắt buộc nhập\r\nKhông được xóa hoặc chèn thêm cột, Không được thay đổi vị trí các cột, Không nhập trùng mã kiểm soát với kiểm soát đã có trên phần mềm và trên file excel";
                    notecell.Style.Font.Size = 12;

                    var _startrow = 4;
                    var startrow = 4;
                    var startcol = 1;
                    var startrow1 = 1;
                    var startrow2 = 1;
                    var startrow3 = 1;
                    var startrow4 = 1;

                    using ExcelRange r = worksheet.Cells[_startrow, startcol, startrow + 2, startcol + 11];
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                    var worksheet2 = excelPackage.Workbook.Worksheets["Don_vi_lien_quan"];

                    foreach (var (value, i) in _auditFacility.Select((value, i) => (value, i)))
                    {
                        ExcelRange cell = worksheet2.Cells[(1 + i), 1];
                        cell.Value = value.Code + " - " +  value.Name;
                        if (value.ParentId != null)
                        {
                            var _depth = Audit_service.Utils.Utils.DepthLengthFromRootLevel(facilityDic , (int)value.ParentId);
                            for (int j = 1; j <= _depth; j++)
                            {
                                cell.Value = ">" + cell.Value;
                            }
                        }
                    }

                    var worksheet3 = excelPackage.Workbook.Worksheets["Hoat_dong_lien_quan"];
                    foreach (var (value, i) in _businessActivity.Select((value, i) => (value, i)))
                    {
                        ExcelRange cell = worksheet3.Cells[(1 + i), 1];
                        cell.Value = value.Code + " - " + value.Name;
                        if (value.ParentId != null)
                        {
                            var _depth = Audit_service.Utils.Utils.DepthLengthFromRootLevel(sBaDic, (int)value.ParentId);
                            for (int j = 1; j <= _depth; j++)
                            {
                                cell.Value = ">" + cell.Value;
                            }
                        }
                    }

                    var worksheet4 = excelPackage.Workbook.Worksheets["Quy_trinh_lien_quan"];
                    foreach (var (value, i) in _auditProcess.Select((value, i) => (value, i)))
                    {
                        ExcelRange cell = worksheet4.Cells[(1 + i), 1];
                        cell.Value = value.Code + " - " + value.Name;
                    }

                    var worksheet5 = excelPackage.Workbook.Worksheets["Tan_suat_kiem_soat"];

                    foreach (var a in _frequencyControl)
                    {
                        var _activeitem = _frequencyControl.Where(x => x.Id == a.Id).ToArray();
                        ExcelRange cellActivity = worksheet5.Cells[startrow2, 1];
                        cellActivity.Value = string.Join(", ", _activeitem.Select(x => x.Name).Distinct());
                        startrow2++;
                    }

                    var worksheet6 = excelPackage.Workbook.Worksheets["Loai_kiem_soat"];

                    foreach (var a in _controlType)
                    {
                        var _activeitem = _controlType.Where(x => x.Id == a.Id).ToArray();
                        ExcelRange cellActivity = worksheet6.Cells[startrow3, 1];
                        cellActivity.Value = string.Join(", ", _activeitem.Select(x => x.Name).Distinct());
                        startrow3++;
                    }

                    var worksheet7 = excelPackage.Workbook.Worksheets["Hinh_thuc_kiem_soat"];
                    foreach (var a in _formalControl)
                    {
                        var _activeitem = _formalControl.Where(x => x.Id == a.Id).ToArray();
                        ExcelRange cellActivity = worksheet7.Cells[startrow4, 1];
                        cellActivity.Value = string.Join(", ", _activeitem.Select(x => x.Name).Distinct());
                        startrow4++;
                    }

                    Bytes = excelPackage.GetAsByteArray();
                }

                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kitano_DanhMucKiemSoat_v0.1.xlsx");
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
                        var data = new List<CatControlModel>();

                        using (var fileStream = new MemoryStream())
                        {
                            file.CopyTo(fileStream);
                            var mappingCl = new Dictionary<string, string>() {
                                {"STT", "No"},
                                {"Đơn vị liên quan*", "unitname"},
                                {"Hoạt động liên quan*", "activename"},
                                {"Quy trình*", "processname"},
                                {"Mã kiểm soát*", "code"},
                                {"Tên kiểm soát*", "name"},
                                {"Kiểm soát theo quy định*", "description"},
                                {"Tần suất kiểm soát*", "controlfrequency"},
                                {"Loại kiểm soát*", "controltype"},
                                {"Hình thức kiểm soát*", "controlformat"},
                                {"Trạng thái", "statusname"},
                                {"Kiểm soát thực tế", "actualcontrol"}
                            };
                            data = Utils.ExcelFn.UploadToListFnc<CatControlModel>(fileStream, 0, true, 2, mappingCl);
                        }

                        bool hasError = false;
                        //var trans = _uow.BeginTransaction();
                        var userInfo = HttpContext.Items["UserInfo"] as CurrentUserModel;

                        var allCode = data.Where(o => o.Code != null && o.Code != "").Select(o => o.Code).ToList();
                        var controlInDb = _uow.Repository<CatControl>().Find(o => o.Code != null && o.Code != "" && o.IsDeleted != true).Distinct().ToList();
                        var codeInDb = _uow.Repository<CatControl>().Find(o => o.Code != null && o.Code != "" && o.IsDeleted != true).Select(o => o.Code).Distinct().ToList();
                        var s = codeInDb.Where(o => allCode.Contains(o)).Select(o => o).Distinct().ToList();
                        //var allParentCode = codeInDb.Select(o => o).ToList();

                        var allFrequencyControl = data.Where(o => o.Code != null && o.Code != "").Select(o => o.Code).Distinct().ToList();
                        var nameControl = data.Where(o => o.Code != null && o.Code != "").Select(o => o.Name).Distinct().ToList();
                        var codeControl = data.Where(o => o.Code != null && o.Code != "").Select(o => o.Code).Distinct().ToList();
                        var nameControlInDb = _uow.Repository<CatControl>().Find(o => o.Code != null && o.Code != "" && o.IsDeleted != true).Select(o => o.Name).Distinct().ToList();
                        var name = nameControlInDb.Where(o => nameControl.Contains(o)).Select(o => o).Distinct().ToList();

                        var unitCodes = data.Where(o => !string.IsNullOrEmpty(o.unitname)).Select(o => o.unitname.Split(" - ")[0].Replace(">", "")).ToList();
                        var activityCodes = data.Where(o => !string.IsNullOrEmpty(o.activename)).Select(o => o.activename.Split(" - ")[0].Replace(">", "")).ToList();
                        var processCodes = data.Where(o => !string.IsNullOrEmpty(o.processname)).Select(o => o.processname.Split(" - ")[0]).ToList();

                        var unitlist = _uow.Repository<AuditFacility>().Find(o => o.Code != null && o.Code != "" && o.Deleted != true && unitCodes.Contains(o.Code)).Distinct().ToList();
                        var activelist = _uow.Repository<BussinessActivity>().Find(o => o.Code != null && o.Code != "" && o.Deleted != true && activityCodes.Contains(o.Code)).Distinct().ToList();
                        var processlist = _uow.Repository<AuditProcess>().Find(o => o.Code != null && o.Code != "" && o.Deleted != true && processCodes.Contains(o.Code)).Distinct().ToList();

                        var frequencyControlLst = _uow.Repository<SystemCategory>().Find(a => a.Deleted != true && a.Status == true && a.ParentGroup == "TanSuatKiemSoat").Distinct().ToList();
                        var controlTypeLst = _uow.Repository<SystemCategory>().Find(a => a.Deleted != true && a.Status == true && a.ParentGroup == "LoaiKiemSoat").Distinct().ToList();
                        var formalControlLst = _uow.Repository<SystemCategory>().Find(a => a.Deleted != true && a.Status == true && a.ParentGroup == "HinhThucKiemSoat").Distinct().ToList();

                        //allParentCode.AddRange(allCode);
                        data.ForEach(o =>
                        {
                            o.Valid = true;
                            o.unitname = o.unitname?.Split('-')[0]?.Trim() ?? string.Empty;
                            var unit = unitlist.FirstOrDefault(t => t.Code == o.unitname);
                            if (string.IsNullOrEmpty(o.unitname?.Trim()))
                            {
                                o.Valid = false;
                                o.Note += "FacilityEmpty,";
                            }
                            else if (unit == null)
                            {
                                o.Valid = false;
                                o.Note += "FacilityNotFound,";
                            }
                            else
                            {
                                o.Unitid = unit.Id;
                                o.unitname = unit.Name;
                            }

                            o.activename = o.activename?.Split('-')[0]?.Trim() ?? string.Empty;
                            var active = activelist.FirstOrDefault(t => t.Code == o.activename);
                            if (string.IsNullOrEmpty(o.activename))
                            {
                                o.Valid = false;
                                o.Note += "ActivityEmpty,";
                            }
                            else if (active == null)
                            {
                                o.Activationid = null;
                                o.activename = string.Empty;
                                o.Note += "ActivityNotFound,";
                            }
                            else
                            {
                                o.Activationid = active.ID;
                                o.activename = active.Name;
                            }

                            o.processname = o.processname?.Split('-')[1]?.Trim() ?? string.Empty;
                            var process = processlist.FirstOrDefault(a=> a.Name == o.processname && a.ActivityId == o.Activationid && a.FacilityId == o.Unitid);
                            if (string.IsNullOrEmpty(o.processname))
                            {
                                o.Valid = false;
                                o.Note += "ProcessNameEmpty,";
                            }
                            else if (process == null)
                            {
                                o.Valid = false;
                                o.Note += "ProcessNameNotFound,";
                            }
                            else
                            {
                                o.Processid = process.Id;
                                o.processname = process.Name;
                            }

                            if (string.IsNullOrEmpty(o.Code))
                            {
                                o.Valid = false;
                                o.Note += "CodeControlEmpty,";
                            }

                            if (string.IsNullOrEmpty(o.Description))
                            {
                                o.Valid = false;
                                o.Note += "DescriptionEmpty,";
                            }

                            var frequency = frequencyControlLst.FirstOrDefault(a => a.Name == o.controlfrequency);
                            if (string.IsNullOrEmpty(o.controlfrequency))
                            {
                                o.Valid = false;
                                o.Note += "ControlFrequencyEmpty,";
                            }else if (frequency == null)
                            {
                                o.Valid = false;
                                o.Note += "ControlFrequencyNotFound,";
                            }

                            var controlT = controlTypeLst.FirstOrDefault(a => a.Name == o.controltype);
                            if (string.IsNullOrEmpty(o.controltype))
                            {
                                o.Valid = false;
                                o.Note += "ControlTypeEmpty,";
                            } else if (controlT == null)
                            {
                                o.Valid = false;
                                o.Note += "ControlTypeNotFound,";
                            }

                            var controlF = formalControlLst.FirstOrDefault(a => a.Name == o.controlformat);
                            if (string.IsNullOrEmpty(o.controlformat))
                            {
                                o.Valid = false;
                                o.Note += "ControlFormalEmpty,";
                            } else if (controlF == null)
                            {
                                o.Valid = false;
                                o.Note += "ControlFormalNotFound,";
                            }

                            if ("Sử dụng".Equals(o.statusname))
                            {
                                o.Status = 1;
                            }
                            else
                            {
                                o.Status = 0;
                            }

                            if (!string.IsNullOrEmpty(o.Code) && s.Contains(o.Code))
                            {
                                if (s.Contains(o.Code))
                                {
                                    o.Valid = false;
                                    o.Note += "CodeControlExisting,";
                                }

                                var countInExcel = allCode.Count(a => a == o.Code);
                                if (countInExcel > 1)
                                {
                                    o.Valid = false;
                                    o.Note += "CodeControlExistingInImportFile,";
                                }
                            }
                            else if (!string.IsNullOrEmpty(o.Code))
                            {
                                var countInExcel = allCode.Count(a => a == o.Code);
                                if (countInExcel > 1)
                                {
                                    o.Valid = false;
                                    o.Note += "CodeControlExistingInImportFile,";
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
                                var obj = new CatControl();

                                obj.Name = item.Name;
                                obj.Processid = item.Processid;
                                obj.RelateStep = item.RelateStep;
                                obj.Unitid = item.Unitid;
                                obj.Activationid = item.Activationid;
                                obj.Status = Convert.ToInt32(item.Status);
                                obj.Code = item.Code;
                                obj.Description = item.Description;
                                obj.CreatedBy = userInfo.Id;
                                obj.Createdate = DateTime.Now;
                                obj.ActualControl = item.ActualControl;

                                obj.Controltype = Convert.ToInt32(controlTypeLst.FirstOrDefault(t=> t.Name.Equals(item.controltype.Trim()))?.Code);
                                obj.Controlfrequency = Convert.ToInt32(frequencyControlLst.FirstOrDefault(t => t.Name.Equals(item.controlfrequency.Trim()))?.Code);
                                obj.Controlformat = Convert.ToInt32(formalControlLst.FirstOrDefault(t => t.Name.Equals(item.controlformat.Trim()))?.Code);

                                _uow.Repository<CatControl>().Add(obj);
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
    }
}