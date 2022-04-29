using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class DocumentUnitProvideController : BaseController
    {
        //public DocumentUnitProvideController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        //{
        //}

        private readonly IConfiguration _config;

        public DocumentUnitProvideController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<DocumentUnitProvideSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var providestatus = int.Parse(obj.providestatus);
                var unit = Convert.ToInt32(obj.unitid);
                var audit = Convert.ToInt32(obj.auditworkid);
                var year = Convert.ToInt32(obj.year);
                //(status == -1 || c.status == status) &&

                var _user = _uow.Repository<Users>().Find(c => c.Id == userInfo.Id).FirstOrDefault();
                var _roles = _uow.Repository<UsersRoles>().Find(c => c.Id == userInfo.Id).FirstOrDefault();

                Expression<Func<DocumentUnitProvide, bool>> filter = c => (unit == 0 || c.unitid == unit) && (audit == 0 || c.auditworkid == audit) && (string.IsNullOrEmpty(obj.name) || c.name.ToLower().Contains(obj.name.ToLower())) && (providestatus == -1 ? true: c.providestatus == providestatus ) && (year == 0 || c.year == year) && (_user.UsersType != 2 || c.unitid == _user.DepartmentId) && c.isDeleted != true;
                var list_depart = _uow.Repository<DocumentUnitProvide>().Find(filter).OrderByDescending(a => a.createDate);
                IEnumerable<DocumentUnitProvide> data = list_depart;
                var count = list_depart.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }

                var _unit = _uow.Repository<AuditFacility>().Find(a => a.IsActive == true).ToArray();

                var _audit = _uow.Repository<AuditWork>().Find(a => a.IsActive == true).ToArray();

                var user = data.Where(a => a.isDeleted != true).Select(a => new DocumentUnitProvideModel()
                {
                    Id = a.Id,
                    name = a.name,
                    auditworkid = a.auditworkid,
                    unitid = a.unitid,
                    status = a.status,
                    year = a.year,
                    providedate = a.providedate.HasValue ? a.providedate.Value.ToString("dd/MM/yyyy") : "",
                    expridate = a.expridate.HasValue ? a.expridate.Value.ToString("dd/MM/yyyy") : "",
                    unitname = _unit.FirstOrDefault(u => u.Id == a.unitid).Name ?? "",
                    auditcode = _audit.FirstOrDefault(u => u.Id == a.auditworkid).Code ?? "",
                    auditname = _audit.FirstOrDefault(u => u.Id == a.auditworkid).Name ?? "",
                    providestatus = a.providestatus.ToString(),


                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
            }
            catch (Exception e)
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
                var _dupInfo = new DocumentUnitProvideModel();

                var data = Request.Form["data"];
                //var pathSave = "";
                //var filetype = "";
                var Document = new DocumentUnitProvide();
                if (!string.IsNullOrEmpty(data))
                {
                    _dupInfo = JsonSerializer.Deserialize<DocumentUnitProvideModel>(data);
                }
                else
                {
                    return Ok(new { code = "0", msg = "fail" });
                }

                var _allDocument = _uow.Repository<DocumentUnitProvide>().Find(a => a.isDeleted != true && a.name.Equals(_dupInfo.name)).ToArray();

                if (_allDocument.Length > 0)
                {
                    return Ok(new { code = "-1", msg = "fail" });
                }

                Document.name = _dupInfo.name;
                Document.description = _dupInfo.description;
                Document.unitid = (int)_dupInfo.unitid;
                Document.year = (int)_dupInfo.year;
                Document.email = _dupInfo.email;
                Document.expridate = DateTime.Parse(_dupInfo.expridate);
                Document.auditworkid = (int)_dupInfo.auditworkid;
                Document.createDate = DateTime.Now;
                Document.createdBy = _userInfo.Id;
                Document.providestatus = !string.IsNullOrEmpty(_dupInfo.providestatus) ? int.Parse(_dupInfo.providestatus) : 0;
                Document.providedate = !string.IsNullOrEmpty(_dupInfo.providedate) ? DateTime.Parse(_dupInfo.providedate) : null;
                //Document.path = pathSave;
                //Document.filetype = filetype;
                Document.isDeleted = false;

                if (Document.name == "" || Document.name == null)
                {
                    return Ok(new { code = "0", msg = "fail" });
                }

                var file = Request.Form.Files;
                var _listFile = new List<DocumentUnitProvideFile>();

                if (file.Count > 0)
                {
                    foreach (var item in file)
                    {
                        var file_type = item.ContentType;
                        var pathSave = CreateUploadURL(item, "DocumentUnitProvide/" + DateTime.Now.Year + DateTime.Now.Month);
                        var document_unit_provide_file = new DocumentUnitProvideFile()
                        {
                            DocumentUnitProvide = Document,
                            IsDelete = false,
                            FileType = file_type,
                            Path = pathSave,
                        };
                        _listFile.Add(document_unit_provide_file);

                        //Document.providedate = DateTime.Now;
                        //Document.status = true;
                    }

                    foreach (var item in _listFile)
                    {
                        _uow.Repository<DocumentUnitProvideFile>().AddWithoutSave(item);
                    }
                }
                else
                {
                    Document.status = false;
                }
                _uow.Repository<DocumentUnitProvide>().Add(Document);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet("DownloadAttach")]
        public IActionResult DonwloadFile(int id)
        {
            //var userInfo = HttpContext.Items["UserInfo"] as User;            
            var self = _uow.Repository<DocumentUnitProvideFile>().FirstOrDefault(o => o.id == id);
            var fullPath = Path.Combine(_config["Upload:AuditDocsPath"], self.Path);

            string name = self.Path.ToString().Replace("\\", "");
            name = name.ToString().Replace("UploadsDocumentUnitProvide", "");
            var fs = new FileStream(fullPath, FileMode.Open);

            return File(fs, self.FileType, name);
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

                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_TaiLieuDVDKT_v0.1.xlsx");
                var _auditFacility = _uow.Repository<AuditFacility>().Find(a => a.Deleted != true && a.IsActive == true).ToList();

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
                var facilityDic = new Dictionary<int, int?>();
                _auditFacility.ForEach(o => { facilityDic.Add(o.Id, o.ParentId); });

                fullPath = fullPath.ToString().Replace("\\", "/");
                var template = new FileInfo(fullPath);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var worksheet = excelPackage.Workbook.Worksheets["Danh_sach_tai_lieu"];
                    ExcelRange headercell = worksheet.Cells[1, 1];
                    headercell.Value = "DANH SÁCH TÀI LIỆU CỦA ĐƠN VỊ ĐƯỢC KIỂM TOÁN";
                    headercell.Style.Font.Size = 11;
                    headercell.Style.Font.Name = "Times New Roman";
                    headercell.Style.Font.Bold = true;

                    var _startrow = 4;
                    var startrow = 4;
                    var startcol = 1;
                    var startrow1 = 1;
                    var startrow2 = 1;
                    var startrow3 = 1;
                    var startrow4 = 1;

                    using ExcelRange r = worksheet.Cells[_startrow, startcol, startrow +1 , startcol + 7];
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                    var worksheet1 = excelPackage.Workbook.Worksheets["Don_vi"];
                    foreach (var (value, i) in _auditFacility.Select((value, i) => (value, i)))
                    {
                        ExcelRange cell = worksheet1.Cells[(1 + i), 1];
                        cell.Value = value.Code + " - " + value.Name;
                        if (value.ParentId != null)
                        {
                            var _depth = Audit_service.Utils.Utils.DepthLengthFromRootLevel(facilityDic, (int)value.ParentId);
                            for (int j = 1; j <= _depth; j++)
                            {
                                cell.Value = ">" + cell.Value;
                            }
                        }
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
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }

                var file = Request.Form.Files[0];
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
                        var data = new List<DocumentUnitProvideModel>();

                        using (var fileStream = new MemoryStream())
                        {
                            file.CopyTo(fileStream);
                            var mappingCl = new Dictionary<string, string>() {
                                {"STT", "No"},
                                {"Tên tài liệu*", "name"},
                                {"Mô tả", "description"},
                                {"Đơn vị cung cấp*", "unitname"},
                                {"Email người phụ trách*", "email"},
                                {"Thời hạn cung cấp*", "expridate"},
                                {"Thời gian cung cấp", "providedate"},
                                {"Trạng thái cung cấp", "providestatus"},
                            };
                            data = Utils.ExcelFn.UploadToListFnc<DocumentUnitProvideModel>(fileStream, 0, true, 2, mappingCl);
                        }
                        bool hasError = false;

                        var _docNames = data.Select(t => t.name.ToLower()).Distinct().ToList();
                        var unitCodes = data.Where(o => !string.IsNullOrEmpty(o.unitname)).Select(o => o.unitname.Split(" - ")[0].Replace(">", "")).ToList();
                        
                        var _unitsDb = _uow.Repository<AuditFacility>().Find(f => f.Code != null && f.Code != "" && f.Deleted != true && unitCodes.Contains(f.Code)).Distinct().ToList();
                        var _documentsInDb = _uow.Repository<DocumentUnitProvide>().Find(f => f.isDeleted != true && _docNames.Contains(f.name)).Distinct().ToList();
                        var _s = _documentsInDb.Select(s => s.name).ToList();

                        data.ForEach(o =>
                        {
                            o.Valid = true;
                            var docs = _documentsInDb.FirstOrDefault(f => f.name.ToLower().Equals(o.name));

                            if (string.IsNullOrEmpty(o.name?.Trim()))
                            {
                                o.Valid = false;
                                o.Note += "DocumentNameEmpty,";
                            }
                            else if (docs != null)
                            {
                                o.Valid = false;
                                o.Note += "DocumentExisting,";
                            }

                            var _unitN = o?.unitname.Split(" - ")[0].Replace(">", "") ?? string.Empty;
                            var unit = _unitsDb.FirstOrDefault(f => f.Code == _unitN);
                            if (string.IsNullOrEmpty(o.unitname))
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
                                o.unitid = unit.Id;
                                o.unitname = unit.Name;
                            }

                            if (!string.IsNullOrEmpty(o.email))
                            {
                                //todo tiennv2 refactor
                                if (!Regex.IsMatch(o.email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                                {
                                    o.Valid = false;
                                    o.Note += "InvalidMail,";
                                }
                            }
                            else
                            {
                                o.Valid = false;
                                o.Note += "EmailResponNotEmpty";
                            }

                            DateTime _providedateTimeRst ;
                            //todo tiennv2 refactor
                            if (DateTime.TryParseExact(o.expridate, "dd/MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out _providedateTimeRst))
                            {
                                o.expridate = _providedateTimeRst.ToString("dd/MM/yyyy");
                                if (_providedateTimeRst > DateTime.Now)
                                {
                                    o.Valid = false;
                                    o.Note += "ProvideDateLessThanCurrentDateError,";
                                }
                            }
                            else
                            {
                                o.Valid = false;
                                o.Note += "FormatDateError,";

                                o.expridate = string.Empty;
                            }
                            //todo tiennv2 refactor
                            DateTime expirededdateTimeRst;
                            if (DateTime.TryParseExact(o.expridate, "dd/MM/yyyy", new CultureInfo("en-US"),
                                DateTimeStyles.None, out expirededdateTimeRst))
                            {
                                o.expridate = expirededdateTimeRst.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                o.expridate = string.Empty;
                            }

                            if (o.providestatus == null)
                            {
                                o.providestatus = "0";
                                //o.Valid = false;
                                //o.Note += "ProvideStatusNotEmpty";
                            }

                            if (!string.IsNullOrEmpty(o.name))
                            {
                                var countInExcel = _docNames.Count(a => a == o.name);
                                if (countInExcel > 1)
                                {
                                    o.Valid = false;
                                    o.Note += "DocNameExistingInImportFile,";
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
                                var obj = new DocumentUnitProvide()
                                {
                                    //Id = item.Id,
                                    name = item.name,
                                    year = item.year ?? 0,
                                    email = item.email,
                                    description = item.description,
                                    unitid = item.unitid ?? 0,
                                    providestatus = !string.IsNullOrEmpty(item.providestatus) ? int.Parse(item.providestatus) : 0,
                                    createDate = DateTime.Now,
                                    createdBy = _userInfo.Id,
                                    isDeleted = false,
                                    expridate = DateTime.ParseExact(item.expridate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                    providedate = DateTime.ParseExact(item.providedate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                };

                                _uow.Repository<DocumentUnitProvide>().Add(obj);
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


        protected string CreateUploadURL(IFormFile imageFile, string folder = "")
        {
            var pathSave = "";
            var pathconfig = _config["Upload:AuditDocsPath"];
            if (imageFile != null)
            {

                if (string.IsNullOrEmpty(folder)) folder = "Public";
                var pathToSave = Path.Combine(pathconfig, "Uploads");
                var extension = Path.GetExtension(imageFile.FileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName)?.Trim();
                //var fileName = Path.GetFileName(imageFile.FileName)?.Trim() /*Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(imageFile.FileName)*/;
                var fileName = fileNameWithoutExtension + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                var fullPathroot = Path.Combine(pathToSave, folder);
                if (!Directory.Exists(fullPathroot))
                {
                    Directory.CreateDirectory(fullPathroot);
                }
                pathSave = Path.Combine("Uploads", folder, fileName);
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
                var pathSave = "";
                if (!string.IsNullOrEmpty(data))
                {
                    _upfiledata = JsonSerializer.Deserialize<UploadFileModel>(data);
                    var file = Request.Form.Files;
                    pathSave = CreateUploadFile(file, "DocumentUnitProvide/" + DateTime.Now.Year + DateTime.Now.Month);
                }
                else
                {
                    return BadRequest();
                }
                var checkworkscope = _uow.Repository<DocumentUnitProvideFile>().FirstOrDefault(a => a.id == _upfiledata.id);
                if (checkworkscope == null)
                {
                    return NotFound();
                }
                checkworkscope.id = (int)_upfiledata.id;
                checkworkscope.Path = pathSave;
                _uow.Repository<DocumentUnitProvideFile>().Update(checkworkscope);
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
                var Document = _uow.Repository<DocumentUnitProvide>().FirstOrDefault(a => a.Id == id);
                if (Document == null)
                {
                    return NotFound();
                }

                Document.isDeleted = true;

                _uow.Repository<DocumentUnitProvide>().Update(Document);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("Update")]
        public IActionResult Edit()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }

                var _dupInfo = new DocumentUnitProvideModel();

                var data = Request.Form["data"];

                if (!string.IsNullOrEmpty(data))
                {
                    _dupInfo = JsonSerializer.Deserialize<DocumentUnitProvideModel>(data);
                }
                else
                {
                    return BadRequest();
                }
                var checkDocument = _uow.Repository<DocumentUnitProvide>().FirstOrDefault(a => a.Id == _dupInfo.Id);
                if (checkDocument == null) { return NotFound(); }
                if (_dupInfo.name == "" || _dupInfo.name == null) { return Ok(new { code = "0", msg = "fail" }); }
                var checkName = _uow.Repository<DocumentUnitProvide>().FirstOrDefault(a => a.name == _dupInfo.name && a.Id != _dupInfo.Id);
                if (checkDocument.Id != (checkName != null ? checkName.Id : null) && checkName != null)
                {
                    return Ok(new { code = "-1", msg = "fail" });
                }

                checkDocument.name = _dupInfo.name;
                checkDocument.year = (int)_dupInfo.year;
                checkDocument.auditworkid = (int)_dupInfo.auditworkid;
                checkDocument.description = _dupInfo.description;
                checkDocument.unitid = (int)_dupInfo.unitid;
                checkDocument.auditworkid = (int)_dupInfo.auditworkid;
                checkDocument.email = _dupInfo.email;
                checkDocument.expridate = DateTime.Parse(_dupInfo.expridate);
                if (!string.IsNullOrEmpty(_dupInfo.providedate))
                {
                    checkDocument.providedate = DateTime.Parse(_dupInfo.providedate);
                }
                
                if (!string.IsNullOrEmpty(_dupInfo.providestatus))
                {
                    checkDocument.providestatus = int.Parse(_dupInfo.providestatus);
                }


                if (_dupInfo.name == "" || _dupInfo.name == null)
                {
                    return Ok(new { code = "0", msg = "fail" });
                }

                var file = Request.Form.Files;
                var _listFile = new List<DocumentUnitProvideFile>();


                foreach (var item in file)
                {
                    var file_type = item.ContentType;
                    var pathSave = CreateUploadURL(item, "DocumentUnitProvide/" + DateTime.Now.Year + DateTime.Now.Month);
                    var document_unit_provide_file = new DocumentUnitProvideFile()
                    {
                        DocumentUnitProvide = checkDocument,
                        IsDelete = false,
                        FileType = file_type,
                        Path = pathSave,
                    };
                    _listFile.Add(document_unit_provide_file);

                    //checkDocument.providedate = DateTime.Now;
                    //checkDocument.status = true;
                }

                foreach (var item in _listFile)
                {
                    _uow.Repository<DocumentUnitProvideFile>().AddWithoutSave(item);
                }
                _uow.Repository<DocumentUnitProvide>().Update(checkDocument);
                var checklistfile = _uow.Repository<DocumentUnitProvideFile>().FirstOrDefault(a => a.documentid == _dupInfo.Id && a.IsDelete != true);

                //if(checklistfile == null)
                //{
                //    //checkDocument.status = false;
                //    //checkDocument.providedate = null;
                //    _uow.Repository<DocumentUnitProvide>().Update(checkDocument);
                //}    


                return Ok(new { code = "1", msg = "success" });


            }
            catch (Exception e)
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
                var Document = _uow.Repository<DocumentUnitProvide>().Include(a => a.DocumentUnitProvideFile).FirstOrDefault(a => a.Id == id);
                var AuditWork = _uow.Repository<AuditWorkPlan>().Include(a => a.Users, a => a.AuditWorkPlanFile, a => a.AuditWorkScopePlanFacility).FirstOrDefault(a => a.Id == id);
                if (Document != null)
                {
                    var Documentinfo = new DocumentUnitProvideModel()
                    {
                        Id = Document.Id,
                        name = Document.name,
                        year = Document.year,
                        email = Document.email,
                        auditworkid = Document.auditworkid,
                        description = Document.description,
                        unitid = Document.unitid,
                        providestatus = Document.providestatus.ToString(),
                        expridate = Document.expridate.HasValue ? Document.expridate.Value.ToString("yyyy-MM-dd") : null,
                        providedate = Document.providedate.HasValue ? Document.providedate.Value.ToString("yyyy-MM-dd") : null,
                        ListFile = Document.DocumentUnitProvideFile.Where(a => a.IsDelete != true).Select(x => new DocumentUnitProvideFileModel()
                        {
                            id = x.id,
                            path = x.Path,
                            file_type = x.FileType,
                        }).ToList(),

                    };
                    return Ok(new { code = "1", msg = "success", data = Documentinfo });
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

        [HttpGet("DeleteAttach/{id}")]
        public IActionResult DeleteFile(int id)
        {
            try
            {
                var self = _uow.Repository<DocumentUnitProvideFile>().FirstOrDefault(o => o.id == id);
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
                }
                if (check == true)
                {
                    self.IsDelete = true;
                    _uow.Repository<DocumentUnitProvideFile>().Update(self);
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