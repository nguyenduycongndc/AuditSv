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

namespace Audit_service.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CatAuditProceduresController : BaseController
    {
        protected readonly IConfiguration _config;

        public CatAuditProceduresController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<CatAuditProceduresSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.Status);
                var activation = Convert.ToInt32(obj.Activationid);
                var unit = Convert.ToInt32(obj.Unitid);
                var process = Convert.ToInt32(obj.Processid);

                Expression<Func<CatAuditProcedures, bool>> filter = c => (status == -1 || c.Status == status) && (activation == 0 || c.Activationid == activation) && (unit == 0 || c.Unitid == unit) && (process == 0 || c.Processid == process) && (string.IsNullOrEmpty(obj.Code) || c.Code.ToLower().Contains(obj.Code.ToLower())) && (string.IsNullOrEmpty(obj.Name) || c.Name.ToLower().Contains(obj.Name.ToLower())) && c.IsDeleted != true;
                var list_depart = _uow.Repository<CatAuditProcedures>().Find(filter).OrderByDescending(a => a.Createdate);
                IEnumerable<CatAuditProcedures> data = list_depart;
                var count = list_depart.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.IsDeleted != true).Select(a => new CatAuditProceduresModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Status = a.Status,
                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
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
                var CatAuditProceduresdetail = _uow.Repository<CatAuditProcedures>().FirstOrDefault(a => a.Id == id);

                if (CatAuditProceduresdetail != null)
                {
                    var getunit = _uow.Repository<AuditFacility>().FirstOrDefault(a => a.Id == CatAuditProceduresdetail.Unitid && a.Deleted !=true && a.IsActive == true);
                    var getcontrol = _uow.Repository<CatControl>().FirstOrDefault(a => a.Id == CatAuditProceduresdetail.cat_control_id && a.IsDeleted !=true);
                    var checkCreatedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatAuditProceduresdetail.Createby);
                    var checkModifiedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatAuditProceduresdetail.Editby);
                    var CatAuditProceduresinfo = new CatAuditProceduresDetailModel()
                    {
                        Id = CatAuditProceduresdetail.Id,
                        Code = CatAuditProceduresdetail.Code,

                        Name = CatAuditProceduresdetail.Name,
                        Description = CatAuditProceduresdetail.Description,
                        Status = CatAuditProceduresdetail.Status,
                        Unitid = CatAuditProceduresdetail.Unitid,
                        Activationid = CatAuditProceduresdetail.Activationid,
                        Processid = CatAuditProceduresdetail.Processid,
                        RelateStep = CatAuditProceduresdetail.RelateStep,
                        Unitname = getunit.Name ?? "",
                        IsDeleted = CatAuditProceduresdetail.IsDeleted,
                        ControlName = getcontrol.Name,
                        cat_control_id = CatAuditProceduresdetail.cat_control_id,
                        CreatedBy = CatAuditProceduresdetail.Createby,
                        Createdate = CatAuditProceduresdetail.Createdate != null ? CatAuditProceduresdetail.Createdate.Value.ToString("dd/MM/yyyy") : null,
                        Editby = CatAuditProceduresdetail.Editby,
                        Editdate = CatAuditProceduresdetail.Editdate != null ? CatAuditProceduresdetail.Editdate.Value.ToString("dd/MM/yyyy") : null,
                        createname = checkCreatedBy != null ? checkCreatedBy.UserName : "",
                        editname = checkModifiedBy != null ? checkModifiedBy.UserName : "",

                    };
                    return Ok(new { code = "1", msg = "success", data = CatAuditProceduresinfo });
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
        public IActionResult Create([FromBody] CatAuditProceduresModel catriskinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                //var _allCatAuditProcedures = _uow.Repository<CatAuditProcedures>().Find(a => a.IsDeleted != true && (a.Name.Equals(catriskinfo.Name))).ToArray();
                var checkCode = _uow.Repository<CatAuditProcedures>().Find(a => a.IsDeleted != true && (a.Code.Equals(catriskinfo.Code))).ToArray();
                
                if (checkCode.Length > 0)
                {
                    return Ok(new { code = "3", msg = "error" });
                }
                else
                {
                    var _CatAuditProcedures = new CatAuditProcedures();
                    _CatAuditProcedures.Name = catriskinfo.Name;
                    _CatAuditProcedures.Processid = catriskinfo.Processid;
                    _CatAuditProcedures.RelateStep = catriskinfo.RelateStep;
                    _CatAuditProcedures.Unitid = catriskinfo.Unitid;
                    _CatAuditProcedures.Activationid = catriskinfo.Activationid;
                    _CatAuditProcedures.Status = catriskinfo.Status;
                    _CatAuditProcedures.Code = catriskinfo.Code;
                    _CatAuditProcedures.Description = catriskinfo.Description;
                    _CatAuditProcedures.Createdate = DateTime.Now;
                    _CatAuditProcedures.cat_control_id = catriskinfo.cat_control_id;
                    _CatAuditProcedures.Createby = _userInfo.Id;

                    _CatAuditProcedures.IsDeleted = false;
                    _uow.Repository<CatAuditProcedures>().Add(_CatAuditProcedures);

                    return Ok(new { code = "1", msg = "success" });
                }


            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Edit([FromBody] CatAuditProceduresModifyModel CatAuditProceduresinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var list_all = _uow.Repository<CatAuditProcedures>().GetAll().ToArray();

                var _CatAuditProceduresInfo = list_all.FirstOrDefault(a => a.Id == CatAuditProceduresinfo.Id);
                if (_CatAuditProceduresInfo == null)
                {
                    return NotFound();
                }

                var checkcode = _uow.Repository<CatAuditProcedures>().Find(a => a.IsDeleted != true && a.Code.Equals(CatAuditProceduresinfo.Code) && (a.Id != CatAuditProceduresinfo.Id)).ToArray();                

                if (checkcode.Length >= 1)
                {
                    return Ok(new { code = "2", msg = "error" });
                }                
                else
                {
                    _CatAuditProceduresInfo.Id = CatAuditProceduresinfo.Id;
                    _CatAuditProceduresInfo.Name = CatAuditProceduresinfo.Name;
                    _CatAuditProceduresInfo.Unitid = CatAuditProceduresinfo.Unitid;
                    _CatAuditProceduresInfo.Status = CatAuditProceduresinfo.Status;
                    _CatAuditProceduresInfo.Processid = CatAuditProceduresinfo.Processid;
                    _CatAuditProceduresInfo.Description = CatAuditProceduresinfo.Description;
                    _CatAuditProceduresInfo.Activationid = CatAuditProceduresinfo.Activationid;
                    _CatAuditProceduresInfo.cat_control_id = CatAuditProceduresinfo.cat_control_id;
                    _CatAuditProceduresInfo.Editdate = DateTime.Now;
                    _CatAuditProceduresInfo.Editby = _userInfo.Id;

                    _uow.Repository<CatAuditProcedures>().Update(_CatAuditProceduresInfo);
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
                var CatDetectType = _uow.Repository<CatAuditProcedures>().FirstOrDefault(a => a.Id == id);
                if (CatDetectType == null)
                {
                    return NotFound();
                }
                var _auditprogram = _uow.Repository<RiskScoringProcedures>().FirstOrDefault(a => a.catprocedures_id == id);
                if (_auditprogram == null)
                {
                    CatDetectType.IsDeleted = true;
                    _uow.Repository<CatAuditProcedures>().Update(CatDetectType);
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

        [HttpGet("GetProcedures/{id}")]
        public IActionResult SearchProcedure(int id,bool? disabled)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                Expression<Func<CatAuditProcedures, bool>> _filter = c => (id == 0 || c.cat_control_id == id) && c.IsDeleted != true;

                var _list_control = _uow.Repository<CatAuditProcedures>().Find(_filter).OrderByDescending(a => a.Createdate);
                IEnumerable<CatAuditProcedures> data = _list_control;

                var user = data.Where(a => a.IsDeleted != true).Select(a => new CatAuditProceduresModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Status = a.Status,
                });

                return Ok(new { code = "1", msg = "success", data = _list_control,disabled = disabled });
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

                var _CatRisk = _uow.Repository<CatAuditProcedures>().Find(a => a.IsDeleted != true).ToArray();
                var _user = _uow.Repository<Users>().Find(a => a.UsersType == 1 && a.IsActive == true).ToArray();
                var _auditFacility = _uow.Repository<AuditFacility>().Find(a => a.Deleted != true).ToArray();
                var _auditActivity = _uow.Repository<BussinessActivity>().Find(a => a.Deleted != true).ToArray();
                var _auditProcess = _uow.Repository<AuditProcess>().Find(a => a.Deleted != true).ToArray();
                var _CatControl = _uow.Repository<CatControl>().Find(a => a.IsDeleted != true).ToArray();

                

                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_Danh_Muc_Thu_Tuc_Kiem_Toan.xlsx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                var template = new FileInfo(fullPath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Danh_muc_thu_tuc_kiem_toan"];
                    ExcelRange cellHeader = worksheet.Cells[1, 1];
                    cellHeader.Value = "DANH MỤC THỦ TỤC KIỂM TOÁN";
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
                        var _controlitem = _CatControl.Where(x => x.Id == a.cat_control_id).ToArray();

                        var _list_risk = _uow.Repository<RiskControl>().Include(a => a.CatRisk).Where(c => (c.controlid == a.cat_control_id)).OrderByDescending(a => a.Id).Select(a => new CatRiskSearchModel()
                        {
                            Riskcontrolid = a.Id,
                            Id = a.CatRisk.Id,
                            name = a.CatRisk.Code +":"+ a.CatRisk.Name,
                            Description = a.CatRisk.Description,
                            code = a.CatRisk.Code

                        });

                        ExcelRange cellAuditFacility = worksheet.Cells[startrow, startcol + 1];
                        cellAuditFacility.Value = string.Join(", ", _auditWorkScopePlanFacilityItem.Select(x => x.Name).Distinct());

                        ExcelRange cellActivity = worksheet.Cells[startrow, startcol + 2];
                        cellActivity.Value = string.Join(", ", _activeitem.Select(x => x.Name).Distinct());

                        ExcelRange cellProcess = worksheet.Cells[startrow, startcol + 3];
                        cellProcess.Value = string.Join(", ", _processitem.Select(x => x.Name).Distinct());

                        ExcelRange cellstep = worksheet.Cells[startrow, startcol + 4];
                        cellstep.Value = string.Join(", ", _controlitem.Select(x => x.Name).Distinct());

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
                            cellstas.Value = "Không Sử dụng";
                        }

                        ExcelRange cellrisk = worksheet.Cells[startrow, startcol + 9];
                        cellrisk.Value = string.Join(", ", _list_risk.Select(x => x.name).Distinct());

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
                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Ke_hoach_kiem_toan_nam.xlsx");
                //return Ok(new { code = "1", data = Bytes, file_name = "Ke_hoach_kiem_toan_nam.xlsx",   msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}