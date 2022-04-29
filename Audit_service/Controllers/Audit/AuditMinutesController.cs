using Audit_service.DataAccess;
using Audit_service.Models.ExecuteModels;
using Audit_service.Models.ExecuteModels.Audit;
using Audit_service.Models.MigrationsModels;
using Audit_service.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Spire.Doc;
using Spire.Pdf;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Formatting;
using System.Drawing;
using Document = Spire.Doc.Document;

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class AuditMinutesController : BaseController
    {
        protected readonly IConfiguration _config;
        public AuditMinutesController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
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
                var obj = JsonSerializer.Deserialize<AuditMinutesSearchModel>(jsonData);
                var status = Convert.ToInt32(obj.status);
                Expression<Func<AuditMinutes, bool>> filter = c => (obj.year == null || c.year.Equals(obj.year))
                                                && (obj.auditwork_id == null || c.auditwork_id.Equals(obj.auditwork_id))
                                                && (obj.auditfacilities_id == null || c.auditfacilities_id.Equals(obj.auditfacilities_id))
                                                && (status == -1 || (status != 1 ? c.status == 2 : c.status == 1))
                                                && c.IsDeleted != true;
                var list_auditminutes = _uow.Repository<AuditMinutes>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditMinutes> data = list_auditminutes;
                var count = list_auditminutes.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var auditminutes = data.Select(a => new AuditMinutesModel()
                {
                    id = a.id,
                    year = a.year,
                    auditwork_id = a.auditwork_id,
                    auditwork_name = a.auditwork_name,
                    auditwork_code = a.auditwork_code,
                    audit_work_taget = a.audit_work_taget,
                    audit_work_person = a.audit_work_person,
                    audit_work_classify = a.audit_work_classify,
                    auditfacilities_name = a.auditfacilities_name,
                    status = a.status,
                    IsDeleted = a.IsDeleted,
                    CreatedAt = a.CreatedAt,
                    ModifiedAt = a.ModifiedAt,
                    DeletedAt = a.DeletedAt,
                    CreatedBy = a.CreatedBy,
                    ModifiedBy = a.ModifiedBy,
                    DeletedBy = a.DeletedBy,
                });
                return Ok(new { code = "1", msg = "success", data = auditminutes, total = count, record_number = obj.StartNumber });
            }
            catch (Exception e)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels(), total = 0 });
            }
        }
        [HttpPost("CreateAuditMinutes")]// thêm mới báo cáo
        public IActionResult CreateAuditMinutes([FromBody] AuditMinutesCreateModel auditMinutesCreateModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditMinutes = _uow.Repository<AuditMinutes>().Find(a => a.IsDeleted != true
                && a.year.Equals(auditMinutesCreateModel.year)
                && a.auditwork_id.Equals(auditMinutesCreateModel.auditwork_id)
                && a.auditfacilities_id.Equals(auditMinutesCreateModel.auditfacilities_id)).ToArray();
                if (checkAuditMinutes.Length > 0)
                {
                    return Ok(new { code = "416", msg = "fail" });
                }

                var checkAuditWork = _uow.Repository<AuditWork>().FirstOrDefault(a => a.Id == auditMinutesCreateModel.auditwork_id);

                var checkAuditFacility = _uow.Repository<AuditFacility>().FirstOrDefault(a => a.Id == auditMinutesCreateModel.auditfacilities_id);

                var auditminutes = new AuditMinutes
                {
                    year = auditMinutesCreateModel.year,
                    auditwork_id = auditMinutesCreateModel.auditwork_id,
                    auditwork_name = (checkAuditWork != null ? checkAuditWork.Name : ""),
                    auditwork_code = (checkAuditWork != null ? checkAuditWork.Code : ""),
                    auditfacilities_id = auditMinutesCreateModel.auditfacilities_id,
                    auditfacilities_name = (checkAuditFacility != null ? checkAuditFacility.Name : ""),
                    audit_work_person = (int)checkAuditWork.person_in_charge,
                    audit_work_taget = checkAuditWork.Target,
                    status = auditMinutesCreateModel.status,
                    audit_work_classify = (int)checkAuditWork.Classify,
                    IsDeleted = false,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userInfo.Id,
                    ModifiedAt = DateTime.Now,
                    ModifiedBy = _userInfo.Id,
                    OtherContent = auditMinutesCreateModel.other_content,
                };
                _uow.Repository<AuditMinutes>().Add(auditminutes);
                var checkAuditMinutesCreate = _uow.Repository<AuditMinutes>().FirstOrDefault(a => a.year == auditminutes.year
                && a.auditwork_id == auditminutes.auditwork_id && a.auditfacilities_id == auditminutes.auditfacilities_id && a.IsDeleted == false);
                return Ok(new { code = "1", msg = "success", auditminutes_id = checkAuditMinutesCreate.id });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("{id}")]
        public IActionResult Detail(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditMinutes = _uow.Repository<AuditMinutes>().Include(a => a.AuditMinutesFile).FirstOrDefault(a => a.id == id && a.IsDeleted.Equals(false));

                var checkSystemCategory = _uow.Repository<SystemCategory>().FirstOrDefault(a => a.Id == checkAuditMinutes.rating_level_total && a.Deleted.Equals(false));

                var checkperson = _uow.Repository<AuditMinutes>().Include(a => a.Users).FirstOrDefault(a => a.id == id
                && a.Users.Id == checkAuditMinutes.audit_work_person);
                var checkAuditWork = _uow.Repository<AuditMinutes>().Include(a => a.AuditWork).FirstOrDefault(a => a.id == id
                && a.AuditWork.Id == checkAuditMinutes.auditwork_id);

                var checkAuditWorkScope = _uow.Repository<AuditWorkScope>().Include(x => x.AuditWork).FirstOrDefault(x => x.auditwork_id == checkAuditMinutes.auditwork_id);
                var auditWorkScope = _uow.Repository<AuditWorkScope>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var auditDetect = _uow.Repository<AuditDetect>().Include(x => x.CatDetectType).Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countRiskRatingHight = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.rating_risk == 1
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countRiskRatingMedium = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.rating_risk == 2
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countRiskRatingLow = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.rating_risk == 3
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countOpinionAuditTrue = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.opinion_audit == true
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countOpinionAuditFalse = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.opinion_audit == false
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                //for (int i = 0; i < auditWorkScope.Length; i++)
                //{
                //    var checkAuditWorkScopeUserMapping = _uow.Repository<AuditWorkScopeUserMapping>().Include(a => a.Users).Where(a => a.auditwork_scope_id == auditWorkScope[i].Id).ToArray();
                //}
                //var indexPath = checkAuditMinutes.path != null ? checkAuditMinutes.path.ToString().LastIndexOf("\\") : 0;
                //string name = checkAuditMinutes.path != null ? checkAuditMinutes.path.ToString().Remove(0, indexPath + 1) : "";

                if (checkAuditMinutes != null)
                {
                    var auditminutes = new AuditMinutesModel()
                    {
                        id = checkAuditMinutes.id,
                        year = checkAuditMinutes.year,
                        auditwork_id = checkAuditMinutes.auditwork_id,
                        auditwork_name = checkAuditMinutes.auditwork_name,
                        auditwork_code = checkAuditMinutes.auditwork_code,
                        str_auditwork_name = checkAuditMinutes != null ? checkAuditMinutes.auditwork_id + ":" + checkAuditMinutes.auditwork_name : "",
                        audit_work_taget = checkAuditMinutes.audit_work_taget,
                        audit_work_person = checkAuditMinutes.audit_work_person,
                        audit_work_person_name = checkperson != null ? checkperson.Users.FullName : "",
                        str_audit_work_person = checkperson != null ? checkperson.Users.Id + ":" + checkperson.Users.FullName : "",
                        audit_work_classify = checkAuditMinutes.audit_work_classify,
                        auditfacilities_id = checkAuditMinutes.auditfacilities_id,
                        auditfacilities_name = checkAuditMinutes.auditfacilities_name,
                        str_auditfacilities_name = checkAuditMinutes != null ? checkAuditMinutes.auditfacilities_id + ":" + checkAuditMinutes.auditfacilities_name : "",
                        status = checkAuditMinutes.status,
                        OtherContent = checkAuditMinutes.OtherContent,

                        IsActive = checkAuditMinutes.IsActive,
                        IsDeleted = checkAuditMinutes.IsDeleted,
                        CreatedAt = checkAuditMinutes.CreatedAt,
                        CreatedBy = checkAuditMinutes.CreatedBy,
                        ModifiedAt = checkAuditMinutes.ModifiedAt,
                        ModifiedBy = checkAuditMinutes.ModifiedBy,
                        DeletedAt = checkAuditMinutes.DeletedAt,
                        DeletedBy = checkAuditMinutes.DeletedBy,
                        //rating = checkAuditMinutes.rating,
                        str_rating = checkSystemCategory != null ? checkSystemCategory.Id + ":" + checkSystemCategory.Name : "",

                        problem = checkAuditMinutes.problem,
                        idea = checkAuditMinutes.idea,
                        //risk_rating_hight = countRiskRatingHight.Count(),
                        //risk_rating_medium = countRiskRatingMedium.Count(),
                        //risk_rating_low = countRiskRatingLow.Count(),
                        AuditScopeOutside = checkAuditMinutes.AuditWork.AuditScopeOutside,
                        audit_scope = checkAuditMinutes.AuditWork.AuditScope,
                        from_date = checkAuditMinutes.AuditWork.from_date.HasValue ? checkAuditMinutes.AuditWork.from_date.Value.ToString("yyyy-MM-dd") : null,
                        to_date = checkAuditMinutes.AuditWork.to_date.HasValue ? checkAuditMinutes.AuditWork.to_date.Value.ToString("yyyy-MM-dd") : null,

                        //ListAuditWorkScopeAuditMinutes = auditWorkScope.Select(a => new ListAuditWorkScopeAuditMinutes
                        //{
                        //    id = a.Id,
                        //    auditwork_id = a.auditwork_id,
                        //    auditprocess_id = a.auditprocess_id,
                        //    bussinessactivities_id = a.bussinessactivities_id,
                        //    auditfacilities_id = a.auditfacilities_id,
                        //    auditfacilities_name = a.auditfacilities_name,//đơn vị kiểm toán
                        //    auditprocess_name = a.auditprocess_name,//Quy trình kiểm toán
                        //    bussinessactivities_name = a.bussinessactivities_name,//Hoạt động được kiểm toán
                        //    year = a.Year,
                        //    risk_rating = a.RiskRating,
                        //}).ToList(),
                        ListStatisticsOfDetections = auditDetect.Select(a => new ListStatisticsOfDetections
                        {
                            id = a.id,
                            audit_detect_code = a.code,
                            auditwork_id = a.auditwork_id,
                            auditfacilities_id = a.auditfacilities_id,
                            auditfacilities_name = a.auditfacilities_name,//đơn vị kiểm toán
                            title = a.title,//Tiêu đề phát hiện
                            year = a.year,
                            risk_rating = a.rating_risk,
                            str_classify_audit_detect = a.CatDetectType.Name,
                            reason = a.reason,
                            opinion_audit_true = countOpinionAuditTrue.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                            opinion_audit_false = countOpinionAuditFalse.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                            risk_rating_hight = countRiskRatingHight.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                            risk_rating_medium = countRiskRatingMedium.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                            risk_rating_low = countRiskRatingLow.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                        }).ToList(),
                        ListAuditDetectAuditMinutes = auditDetect.Select(a => new ListAuditDetectAuditMinutes
                        {
                            id = a.id,
                            audit_detect_code = a.code,
                            auditwork_id = a.auditwork_id,
                            auditfacilities_id = a.auditfacilities_id,
                            auditfacilities_name = a.auditfacilities_name,//đơn vị kiểm toán
                            title = a.title,//Tiêu đề phát hiện
                            year = a.year,
                            risk_rating = a.rating_risk,
                            str_classify_audit_detect = a.CatDetectType.Name,
                            reason = a.reason,
                            opinion_audit = a.opinion_audit,
                        }).ToList(),
                        ListFile = checkAuditMinutes.AuditMinutesFile.Where(a => a.IsDelete != true).Select(x => new AuditMinutesFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    var lst = auditminutes.ListStatisticsOfDetections.GroupBy(a => a.str_classify_audit_detect).Select(z => z.FirstOrDefault()).ToList();
                    return Ok(new { code = "1", msg = "success", data = auditminutes, datanew = lst });
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
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditMinutes = _uow.Repository<AuditMinutes>().FirstOrDefault(a => a.id == id);
                if (checkAuditMinutes == null)
                {
                    return NotFound();
                }
                checkAuditMinutes.DeletedAt = DateTime.Now;
                checkAuditMinutes.IsDeleted = true;
                checkAuditMinutes.DeletedBy = userInfo.Id;
                _uow.Repository<AuditMinutes>().Update(checkAuditMinutes);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("EditAuditMinutes")]
        public IActionResult Edit()
        {
            try
            {

                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }

                var auditMinutesEditModel = new AuditMinutesEditModel();
                var data = Request.Form["data"];
                //var pathSave = "";
                //var file_type = "";
                if (!string.IsNullOrEmpty(data))
                {
                    auditMinutesEditModel = JsonSerializer.Deserialize<AuditMinutesEditModel>(data);
                    //var file = Request.Form.Files;
                    //file_type = file.FirstOrDefault()?.ContentType;
                    //pathSave = CreateUploadFile(file, "AuditMinutes");
                }
                else
                {
                    return BadRequest();
                }
                var checkAuditMinutes = _uow.Repository<AuditMinutes>().FirstOrDefault(a => a.id == auditMinutesEditModel.id && a.IsDeleted.Equals(false));
                if (checkAuditMinutes == null) { return NotFound(); }

                //checkAuditMinutes.path = pathSave != "" ? pathSave : checkAuditMinutes.path;
                //checkAuditMinutes.FileType = file_type != null ? file_type : checkAuditMinutes.FileType;
                checkAuditMinutes.ModifiedAt = DateTime.Now;
                checkAuditMinutes.ModifiedBy = _userInfo.Id;
                checkAuditMinutes.status = Int32.Parse(auditMinutesEditModel.status);
                checkAuditMinutes.rating_level_total = Int32.Parse(auditMinutesEditModel.rating);
                checkAuditMinutes.problem = auditMinutesEditModel.problem;
                checkAuditMinutes.idea = auditMinutesEditModel.idea;
                checkAuditMinutes.OtherContent = auditMinutesEditModel.other_content;
                _uow.Repository<AuditMinutes>().Update(checkAuditMinutes);

                var file = Request.Form.Files;
                var _listFile = new List<AuditMinutesFile>();
                foreach (var item in file)
                {
                    var file_type = item.ContentType;
                    var pathSave = CreateUploadURL(item, "AuditMinutes");
                    var audit_minutes_file = new AuditMinutesFile()
                    {
                        AuditMinutes = checkAuditMinutes,
                        IsDelete = false,
                        FileType = file_type,
                        Path = pathSave,
                    };
                    _listFile.Add(audit_minutes_file);
                }
                foreach (var item in _listFile)
                {
                    _uow.Repository<AuditMinutesFile>().AddWithoutSave(item);
                }
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception e)
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
        //download file
        [AllowAnonymous]
        [HttpGet("DownloadAuditMinutes")]
        public IActionResult DonwloadFileAuditMinutes(int id)
        {
            try
            {
                var self = _uow.Repository<AuditMinutesFile>().FirstOrDefault(o => o.id == id);
                if (self == null)
                {
                    return NotFound();
                }
                var fullPath = Path.Combine(_config["Upload:AuditDocsPath"], self.Path);
                var name = "DownLoadFile";
                if (!string.IsNullOrEmpty(self.Path))
                {
                    //var _array = self.path.Split("\\");
                    var _array = self.Path.Replace("/", "\\").Split("\\");
                    name = _array[_array.Length - 1];
                }
                var fs = new FileStream(fullPath, FileMode.Open);

                return File(fs, self.FileType, name);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
        [HttpGet("DeleteAuditMinutes/{id}")]
        public IActionResult DeleteFileAuditMinutes(int id)
        {
            try
            {
                var self = _uow.Repository<AuditMinutesFile>().FirstOrDefault(o => o.id == id);
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
                    _uow.Repository<AuditMinutesFile>().Update(self);
                }

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        //xuất word MIC
        [HttpGet("ExportFileWordMIC/{id}")]
        public IActionResult ExportFileWordMIC(int id)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditMinutes = _uow.Repository<AuditMinutes>().Include(a => a.AuditMinutesFile, a => a.SystemCategory).FirstOrDefault(a => a.id == id && a.IsDeleted.Equals(false));

                var checkperson = _uow.Repository<AuditMinutes>().Include(a => a.Users).FirstOrDefault(a => a.id == id
                && a.Users.Id == checkAuditMinutes.audit_work_person);

                var checkAuditWork = _uow.Repository<AuditMinutes>().Include(a => a.AuditWork).FirstOrDefault(a => a.id == id
                && a.AuditWork.Id == checkAuditMinutes.auditwork_id);

                var checkAuditWorkScope = _uow.Repository<AuditWorkScope>().Include(x => x.AuditWork).FirstOrDefault(x => x.auditwork_id == checkAuditMinutes.auditwork_id);

                var auditWorkScope = _uow.Repository<AuditWorkScope>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var auditDetect = _uow.Repository<AuditDetect>().Include(x => x.CatDetectType, x => x.AuditRequestMonitor).Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                 && a.IsDeleted != true
                 && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var auditRequestMonitor = _uow.Repository<AuditRequestMonitor>().Include(x => x.AuditDetect, x => x.Users).Where(a => a.is_deleted != true).ToArray();

                var day = DateTime.Now.ToString("dd");
                var month = DateTime.Now.ToString("MM");
                var year = DateTime.Now.ToString("yyyy");

                var countRiskRatingHight = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.rating_risk == 1
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countRiskRatingMedium = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.rating_risk == 2
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countRiskRatingLow = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.rating_risk == 3
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countOpinionAuditTrue = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.opinion_audit == true
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countOpinionAuditFalse = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.opinion_audit == false
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var ListStatisticsOfDetections = auditDetect.Select(a => new ListStatisticsOfDetections
                {
                    id = a.id,
                    audit_detect_code = a.code,
                    auditwork_id = a.auditwork_id,
                    auditfacilities_id = a.auditfacilities_id,
                    auditfacilities_name = a.auditfacilities_name,//đơn vị kiểm toán
                    title = a.title,//Tiêu đề phát hiện
                    year = a.year,
                    risk_rating = a.rating_risk,
                    str_classify_audit_detect = a.CatDetectType.Name,
                    reason = a.reason,
                    opinion_audit_true = countOpinionAuditTrue.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                    opinion_audit_false = countOpinionAuditFalse.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                    risk_rating_hight = countRiskRatingHight.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                    risk_rating_medium = countRiskRatingMedium.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                    risk_rating_low = countRiskRatingLow.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                }).GroupBy(a => a.str_classify_audit_detect).Select(z => z.FirstOrDefault()).ToList();
                //data

                var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "REPORT_HEADER");
                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");

                //ẩn hiện
                var checkHiden = _uow.Repository<ConfigDocument>().GetAll().Where(a => a.item_name == "Biên bản kiểm toán"
                && a.isShow == true).ToArray();
                var MTKT = checkHiden.Where(a => a.item_code == "MTKT" && a.status == true).Count();
                var PVKT = checkHiden.Where(a => a.item_code == "PVKT" && a.status == true).Count();
                var GHKT = checkHiden.Where(a => a.item_code == "GHKT" && a.status == true).Count();
                var THKT = checkHiden.Where(a => a.item_code == "THKT" && a.status == true).Count();
                var KSHQ = checkHiden.Where(a => a.item_code == "KSHQ" && a.status == true).Count();
                var KSCT = checkHiden.Where(a => a.item_code == "KSCT" && a.status == true).Count();
                var YKKT = checkHiden.Where(a => a.item_code == "YKKT" && a.status == true).Count();
                var DSPHKT = checkHiden.Where(a => a.item_code == "DSPHKT" && a.status == true).Count();
                //ẩn hiện

                //Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_BienBanKiemToan_v0.2.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\Kitano_BienBanKiemToan_v0.2.docx"))
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
                    if (MTKT != 1 && PVKT != 1 && GHKT != 1 && THKT != 1)
                    {
                        doc.Replace("Cuộc kiểm toán nhằm đảm bảo các mục tiêu sau:", "", false, true);
                        doc.Replace("Các nội dung nằm ngoài phạm vi kiểm toán:", "", false, true);
                        doc.Replace("Thời hiệu kiểm toán từ ngày", "", false, true);
                        doc.Replace("đến ngày", "", false, true);
                    }
                    if (MTKT != 1)
                    {
                        doc.Replace("Cuộc kiểm toán nhằm đảm bảo các mục tiêu sau:", "", false, true);
                    }
                    if (GHKT != 1)
                    {
                        doc.Replace("Các nội dung nằm ngoài phạm vi kiểm toán:", "", false, true);
                    }
                    if (THKT != 1)
                    {
                        doc.Replace("Thời hiệu kiểm toán từ ngày", "", false, true);
                        doc.Replace("đến ngày", "", false, true);
                    }
                    if (KSCT != 1)
                    {
                        doc.Replace("Mức xếp hạng kiểm toán:", "", false, true);
                        doc.Replace("Bảng tổng hợp số lượng phát hiện", "", false, true);
                    }
                    if ((KSHQ != 1 && KSCT != 1 && YKKT != 1))
                    {
                        doc.Replace("Dựa trên kết quả kiểm toán, đoàn Kiểm toán đưa ra đánh giá tổng quan và kết luận về:", "", false, true);
                    }
                    doc.MailMerge.Execute(new string[] { "ngay_1", "thang_1", "nam_1", "ngay_hien_tai_1", "Ten_cong_ty_1", "nguoi_phu_trach_1",
                        "td_tong_quan_1",//7
                        "td_ket_luan_1",//7
                        "td_muc_tieu_kt_1",//8
                        "td_pham_vi_kt_1",//9
                        "td_gioi_han_pham_vi_kt_1",//10
                        "td_thoi_hieu_kt_1",//11
                        "td_ks_hq_1",//12
                        "td_ks_ct_1",//13
                        "td_y_kien_dvkt_1",//14
                        "td_danh_sach_phkt_1",//15
                        "muc_dich_kt_1",//16
                        "pham_vi_kt_new_1",//17
                        "ngoai_pham_vi_kt_1",//18
                        "thoi_hieu_kt_tu_1",//19
                        "thoi_hieu_kt_den_1",//20
                        "cac_van_de_1" ,//21
                        "muc_do_xep_hang_1",//22
                        "y_kien_1",//23
                    },
                    //new string[] { day, month, year, DateTime.Now.ToString("dd/MM/yyyy"), dataCt.Value, checkAuditMinutes.Users.FullName,
                    //checkAuditMinutes.audit_work_taget,
                    //    checkAuditMinutes.AuditWork.AuditScope,
                    //    checkAuditMinutes.AuditWork.AuditScopeOutside,
                    //    checkAuditMinutes.AuditWork.from_date != null ? checkAuditMinutes.AuditWork.from_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                    //    checkAuditMinutes.AuditWork.to_date != null ? checkAuditMinutes.AuditWork.to_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy",
                    //    checkAuditMinutes.problem,
                    //    checkAuditMinutes.rating,
                    //    checkAuditMinutes.idea,
                    //});

                    //ẩn hiện
                    new string[] { day, month, year, DateTime.Now.ToString("dd/MM/yyyy"), dataCt.Value, checkAuditMinutes.Users.FullName,
                    (MTKT != 1 && PVKT != 1 && GHKT != 1 && THKT != 1) ? "" : "TỔNG QUAN",//7
                    (KSHQ != 1 && KSCT != 1 && YKKT != 1) ? "" : "KẾT LUẬN",//7
                    (MTKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "MTKT")?.content),//8
                    (PVKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "PVKT")?.content),//9
                    (GHKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "GHKT")?.content),//10
                    (THKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "THKT")?.content),//11
                    (KSHQ != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "KSHQ")?.content),//12
                    (KSCT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "KSCT")?.content),//13
                    (YKKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "YKKT")?.content),//14
                    (DSPHKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "DSPHKT")?.content),//15
                    MTKT == 1 ? checkAuditMinutes.audit_work_taget: "",//16
                    PVKT == 1 ? checkAuditMinutes.AuditWork.AuditScope: "",//17
                    GHKT == 1 ? checkAuditMinutes.AuditWork.AuditScopeOutside: "",//18
                    (THKT == 1 ? (checkAuditMinutes.AuditWork.from_date != null ? checkAuditMinutes.AuditWork.from_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy"): ""),//19
                    (THKT == 1 ? (checkAuditMinutes.AuditWork.to_date != null ? checkAuditMinutes.AuditWork.to_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy"): ""),//20
                    KSHQ == 1 ? checkAuditMinutes.problem: "",//21
                    KSCT == 1 ? (checkAuditMinutes.SystemCategory != null ? checkAuditMinutes.SystemCategory.Name: "") : "",//22
                    YKKT == 1 ? checkAuditMinutes.idea: "",//23
                    });
                    //ẩn hiện



                    ////Table table = doc.Sections[0].Tables[1] as Table;
                    ////table.ResetCells(auditWorkScope.Count() + 1, 3);
                    ////TextRange txtTable = table[0, 0].AddParagraph().AppendText("Quy trình được KT");
                    ////txtTable.CharacterFormat.FontName = "Times New Roman";
                    ////txtTable.CharacterFormat.FontSize = 12;
                    ////txtTable.CharacterFormat.Bold = true;

                    ////txtTable = table[0, 1].AddParagraph().AppendText("Đơn vị được KT");
                    ////txtTable.CharacterFormat.FontName = "Times New Roman";
                    ////txtTable.CharacterFormat.FontSize = 12;
                    ////txtTable.CharacterFormat.Bold = true;

                    ////txtTable = table[0, 2].AddParagraph().AppendText("Hoạt động được KT");
                    ////txtTable.CharacterFormat.FontName = "Times New Roman";
                    ////txtTable.CharacterFormat.FontSize = 12;
                    ////txtTable.CharacterFormat.Bold = true;
                    ////for (int x = 0; x < auditWorkScope.Length; x++)
                    ////{
                    ////    txtTable = table[x + 1, 0].AddParagraph().AppendText(auditWorkScope[x].auditprocess_name);
                    ////    txtTable.CharacterFormat.FontName = "Times New Roman";
                    ////    txtTable.CharacterFormat.FontSize = 12;

                    ////    txtTable = table[x + 1, 1].AddParagraph().AppendText(auditWorkScope[x].auditfacilities_name);
                    ////    txtTable.CharacterFormat.FontName = "Times New Roman";
                    ////    txtTable.CharacterFormat.FontSize = 12;

                    ////    txtTable = table[x + 1, 2].AddParagraph().AppendText(auditWorkScope[x].bussinessactivities_name);
                    ////    txtTable.CharacterFormat.FontName = "Times New Roman";
                    ////    txtTable.CharacterFormat.FontSize = 12;
                    ////}
                    if (KSCT == 1)
                    {
                        Table table1 = doc.Sections[0].Tables[1] as Table;
                        table1.ResetCells(ListStatisticsOfDetections.Count() + 2, 7);
                        table1.ApplyVerticalMerge(0, 0, 1);
                        table1.ApplyVerticalMerge(1, 0, 1);
                        table1.ApplyHorizontalMerge(0, 2, 4);
                        table1.ApplyHorizontalMerge(0, 5, 6);
                        TextRange txtTable1 = table1[0, 0].AddParagraph().AppendText("Loại phát hiện");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        txtTable1.CharacterFormat.Bold = true;

                        txtTable1 = table1[0, 1].AddParagraph().AppendText("Tổng cộng số lượng");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        txtTable1.CharacterFormat.Bold = true;

                        txtTable1 = table1[0, 2].AddParagraph().AppendText("Số lượng phát hiện");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        txtTable1.CharacterFormat.Bold = true;

                        txtTable1 = table1[0, 5].AddParagraph().AppendText("Ý kiến của đơn vị được kiểm toán");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        txtTable1.CharacterFormat.Bold = true;

                        txtTable1 = table1[1, 2].AddParagraph().AppendText("Cao/Quan trọng");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        txtTable1.CharacterFormat.Bold = true;

                        txtTable1 = table1[1, 3].AddParagraph().AppendText("Trung bình");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        txtTable1.CharacterFormat.Bold = true;

                        txtTable1 = table1[1, 4].AddParagraph().AppendText("Thấp/Ít quan trọng");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        txtTable1.CharacterFormat.Bold = true;

                        txtTable1 = table1[1, 5].AddParagraph().AppendText("Đồng ý");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        txtTable1.CharacterFormat.Bold = true;

                        txtTable1 = table1[1, 6].AddParagraph().AppendText("Không đồng ý");
                        txtTable1.CharacterFormat.FontName = "Times New Roman";
                        txtTable1.CharacterFormat.FontSize = 12;
                        txtTable1.CharacterFormat.Bold = true;
                        for (int z = 0; z < ListStatisticsOfDetections.Count(); z++)
                        {
                            txtTable1 = table1[z + 2, 0].AddParagraph().AppendText(ListStatisticsOfDetections[z].str_classify_audit_detect);
                            txtTable1.CharacterFormat.FontName = "Times New Roman";
                            txtTable1.CharacterFormat.FontSize = 12;

                            txtTable1 = table1[z + 2, 1].AddParagraph().AppendText((ListStatisticsOfDetections[z].risk_rating_hight + ListStatisticsOfDetections[z].risk_rating_medium + ListStatisticsOfDetections[z].risk_rating_low).ToString());
                            txtTable1.CharacterFormat.FontName = "Times New Roman";
                            txtTable1.CharacterFormat.FontSize = 12;

                            txtTable1 = table1[z + 2, 2].AddParagraph().AppendText(ListStatisticsOfDetections[z].risk_rating_hight.ToString());
                            table1[z + 2, 2].CellFormat.BackColor = ColorTranslator.FromHtml("#FF0000");
                            txtTable1.CharacterFormat.FontName = "Times New Roman";
                            txtTable1.CharacterFormat.FontSize = 12;

                            txtTable1 = table1[z + 2, 3].AddParagraph().AppendText(ListStatisticsOfDetections[z].risk_rating_medium.ToString());
                            table1[z + 2, 3].CellFormat.BackColor = ColorTranslator.FromHtml("#FFC000");
                            txtTable1.CharacterFormat.FontName = "Times New Roman";
                            txtTable1.CharacterFormat.FontSize = 12;

                            txtTable1 = table1[z + 2, 4].AddParagraph().AppendText(ListStatisticsOfDetections[z].risk_rating_low.ToString());
                            table1[z + 2, 4].CellFormat.BackColor = ColorTranslator.FromHtml("#92D050");
                            txtTable1.CharacterFormat.FontName = "Times New Roman";
                            txtTable1.CharacterFormat.FontSize = 12;

                            txtTable1 = table1[z + 2, 5].AddParagraph().AppendText(ListStatisticsOfDetections[z].opinion_audit_true.ToString());
                            txtTable1.CharacterFormat.FontName = "Times New Roman";
                            txtTable1.CharacterFormat.FontSize = 12;

                            txtTable1 = table1[z + 2, 6].AddParagraph().AppendText(ListStatisticsOfDetections[z].opinion_audit_false.ToString());
                            txtTable1.CharacterFormat.FontName = "Times New Roman";
                            txtTable1.CharacterFormat.FontSize = 12;
                        }
                    }
                    else
                    {
                        doc.Sections[0].Tables.Remove(doc.Sections[0].Tables[1]);
                    }
                    if (DSPHKT == 1)
                    {
                        String[] _Title = { "", " Tiêu đề:", " Phân loại phát hiện:", " Vấn đề:", " Ảnh hưởng:", " Nguyên nhân:",/* " Dẫn chiếu:",*/ " Mức độ rủi ro:", " Ý kiến của đơn vị:", " " };
                        for (int c = 0; c < auditDetect.Length; c++)
                        {
                            Section section = doc.Sections[0];
                            Paragraph paraInserted = section.AddParagraph();


                            TextRange textNew = paraInserted.AppendText(_Title[0] + " " + (c + 1) + ". " + "Phát hiện:" + auditDetect[c].code);
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;
                            textNew.CharacterFormat.Bold = true;

                            textNew = paraInserted.AppendText("\n" + _Title[1] + " " + auditDetect[c].title);
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[2] + " " + auditDetect[c].CatDetectType.Name);
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[3] + " " + auditDetect[c].description);
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[4] + " " + auditDetect[c].affect);
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[5] + " " + auditDetect[c].cause);
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            //textNew = paraInserted.AppendText("\n" + _Title[6] + " " + auditDetect[c].evidence);
                            //textNew.CharacterFormat.FontName = "Times New Roman";
                            //textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[6] + " " + (auditDetect[c].rating_risk == 1 ? "Cao/Quan trọng" : auditDetect[c].rating_risk == 2 ? "Trung bình" : auditDetect[c].rating_risk == 3 ? "Thấp/Ít quan trọng" : ""));
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[7] + " " + (auditDetect[c].opinion_audit == true ? "Đồng ý" : "Không đồng ý"));
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[8] + " " + (auditDetect[c].reason));
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            var _auditRequestMonitor = _uow.Repository<AuditRequestMonitor>().Include(x => x.AuditDetect, x => x.Users, x => x.FacilityRequestMonitorMapping).Where(a => a.is_deleted != true
                            && a.detectid == auditDetect[c].id).ToArray();

                            Table tablenew = section.AddTable(true);

                            String[] Header = { "Nội dung kiến nghị", "Đơn vị đầu mối", "Đơn vị phối hợp", "Thời hạn hoàn thành" };
                            //Add Cells
                            tablenew.ResetCells(_auditRequestMonitor.Length, Header.Length);
                            //Header Row
                            TableRow FRow = tablenew.Rows[0];
                            FRow.IsHeader = true;
                            FRow.Height = 23;
                            //Row Height
                            for (int i = 0; i < Header.Length; i++)
                            {
                                //Cell Alignment
                                Paragraph p = FRow.Cells[i].AddParagraph();
                                FRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                p.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                //Data Format
                                TextRange TR = p.AppendText(Header[i]);
                                TR.CharacterFormat.FontName = "Times New Roman";
                                TR.CharacterFormat.FontSize = 12;
                                TR.CharacterFormat.Bold = true;
                            }
                            for (int d = 0; d < _auditRequestMonitor.Length; d++)
                            {
                                string unit_name = "";
                                string cooperateunit_name = "";
                                for (int zx = 0; zx < _auditRequestMonitor[d].FacilityRequestMonitorMapping.Count(); zx++)
                                {
                                    unit_name = (_auditRequestMonitor[d].FacilityRequestMonitorMapping != null ? string.Join(", ", _auditRequestMonitor[d].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 1 && c.audit_request_monitor_id.Equals(_auditRequestMonitor[d].Id)).Select(x => x.audit_facility_name).Distinct()) : "");
                                    cooperateunit_name = (_auditRequestMonitor[d].FacilityRequestMonitorMapping != null ? string.Join(", ", _auditRequestMonitor[d].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 2 && c.audit_request_monitor_id.Equals(_auditRequestMonitor[d].Id)).Select(x => x.audit_facility_name).Distinct()) : "");
                                }
                                ////Add Cells
                                //tablenew.ResetCells(data.Length + 1, Header.Length);



                                //Data Row

                                String[][] data = {
                            new String[]{ _auditRequestMonitor[d].Content, unit_name, cooperateunit_name,
                                _auditRequestMonitor[d].CompleteAt.Value.ToString("dd/MM/yyyy") },};
                                //for (int r = 0; r < _auditRequestMonitor.Length; r++)
                                //{
                                TableRow DataRow = tablenew.Rows[d];
                                //Row Height
                                DataRow.Height = 20;


                                //C Represents Column.
                                for (int xc = 0; xc < data[0].Length; xc++)
                                //for (int xc = 0; xc < _auditRequestMonitor.Length; xc++)
                                {
                                    //Cell Alignment
                                    DataRow.Cells[xc].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                    //Fill Data in Rows
                                    Paragraph p2 = DataRow.Cells[xc].AddParagraph();
                                    TextRange TR2 = p2.AppendText(data[0][xc]);
                                    //Format Cells
                                    p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                    TR2.CharacterFormat.FontName = "Times New Roman";
                                    TR2.CharacterFormat.FontSize = 12;
                                }
                                //}
                                doc.SaveToFile("Kitano_BienBanKiemToan_v0.2.docx");
                            }
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
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_BienBanKiemToan_v0.2.docx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("ExportFileWordAMC/{id}")]
        public IActionResult ExportFileWordAMC(int id)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditMinutes = _uow.Repository<AuditMinutes>().Include(a => a.SystemCategory, a => a.AuditWork, a => a.Users, a => a.AuditFacility).FirstOrDefault(a => a.id == id
                   && a.IsDeleted.Equals(false));

                var catDetectType = _uow.Repository<SystemCategory>().Find(x => x.Deleted != true
                && x.ParentGroup == "MucXepHangKiemToan"
                && x.Id == checkAuditMinutes.rating_level_total).FirstOrDefault();

                var facilityType = _uow.Repository<UnitType>().Find(x => x.IsDeleted != true
                    && x.Id == checkAuditMinutes.AuditFacility.ObjectClassId).FirstOrDefault();

                var approvalStatus = _uow.Repository<ApprovalFunction>().Include(x => x.Users_Last).Where(a => a.function_code == "M_PAP"
                 && a.StatusCode == "3.1"
                 && a.item_id == checkAuditMinutes.auditwork_id).FirstOrDefault();

                var auditDetect = _uow.Repository<AuditDetect>().Include(x => x.CatDetectType, x => x.AuditRequestMonitor).Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                 && a.IsDeleted != true
                 && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var allAuditDetectId = auditDetect.Select(x => x.id.ToString());

                var facilityRequestMonitorMapping = _uow.Repository<FacilityRequestMonitorMapping>().Find(x => allAuditDetectId.Contains(x.AuditRequestMonitor.AuditDetect.id.ToString()) && x.type == 1);

                var auditDetectGrp = auditDetect.GroupBy(x => x.CatDetectType).Select(grp => grp.ToArray()).ToArray();

                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "AMC_Kitano_BienBanKiemToan_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\Kitano_BienBanKiemToan_v0.2.docx"))
                {
                    Section sectionOther = doc.Sections[2];
                    Section sectionDetect = doc.Sections[2];
                    var font = "Times New Roman";
                    var now = DateTime.Now;

                    ListStyle listStyle = new ListStyle(doc, ListType.Numbered);
                    listStyle.Name = "levelstyle";
                    listStyle.Levels[0].PatternType = ListPatternType.Arabic;
                    listStyle.Levels[0].TextPosition = 18f;//28.08 = 0.39 inch trong word
                    listStyle.Levels[0].NumberPosition = 0;
                    listStyle.Levels[0].CharacterFormat.Bold = true;
                    listStyle.Levels[0].CharacterFormat.Italic = true;
                    listStyle.Levels[0].CharacterFormat.FontName = font;
                    listStyle.Levels[0].CharacterFormat.FontSize = 12;
                    doc.ListStyles.Add(listStyle);

                    ListStyle bulletStyle = new ListStyle(doc, ListType.Bulleted);
                    bulletStyle.Name = "bulletstyle";
                    bulletStyle.Levels[0].BulletCharacter = "-";
                    bulletStyle.Levels[0].CharacterFormat.FontName = font;
                    bulletStyle.Levels[0].CharacterFormat.FontSize = 12;
                    bulletStyle.Levels[0].TextPosition = 28.08f;//28.08 = 0.39 inch trong word
                    bulletStyle.Levels[0].NumberPosition = 0;
                    doc.ListStyles.Add(bulletStyle);

                    ParagraphStyle styleTitle = new ParagraphStyle(doc);
                    styleTitle.Name = "titlestyle";
                    styleTitle.CharacterFormat.FontName = font;
                    styleTitle.CharacterFormat.FontSize = 12;
                    styleTitle.CharacterFormat.Bold = true;
                    styleTitle.CharacterFormat.Italic = true;
                    styleTitle.ParagraphFormat.BeforeSpacing = 0;
                    styleTitle.ParagraphFormat.AfterSpacing = 0;
                    styleTitle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    styleTitle.ParagraphFormat.LineSpacing = 14.4360902256f;// line height 1.2
                    doc.Styles.Add(styleTitle);

                    ParagraphStyle contentStyle = new ParagraphStyle(doc);
                    contentStyle.Name = "contentstyle";
                    contentStyle.CharacterFormat.FontName = font;
                    contentStyle.CharacterFormat.FontSize = 12;
                    contentStyle.ParagraphFormat.BeforeSpacing = 0;
                    contentStyle.ParagraphFormat.AfterSpacing = 0;
                    contentStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                    styleTitle.ParagraphFormat.LineSpacing = 14.4360902256f;//line height 1.2 trong word
                    doc.Styles.Add(contentStyle);


                    //remove empty paragraphs
                    doc.MailMerge.HideEmptyParagraphs = true;
                    //remove empty group
                    doc.MailMerge.HideEmptyGroup = true;
                    doc.MailMerge.Execute(new string[] { "ngay_bc", "thang_bc", "nam_bc",
                        "ten_cuoc_kt",
                        "don_vi_duoc_kt",
                        "muc_tieu_kt",
                        "thoi_hieu_kt_tu",
                        "thoi_hieu_kt_den",
                        "gioi_han_kt",
                        "cac_van_de_kiem_soat_hieu_qua",
                        "muc_xep_hang_kt",
                        "nguoi_phu_trach",
                        "nguoi_phe_duyet",
                        "loai_don_vi",
                    },
                    new string[] { now.Day.ToString(), now.Month.ToString(), now.Year.ToString(),
                        checkAuditMinutes.auditwork_name,
                        checkAuditMinutes.auditfacilities_name,
                        checkAuditMinutes.audit_work_taget,
                        checkAuditMinutes.AuditWork.from_date.HasValue ?  checkAuditMinutes.AuditWork.from_date.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditMinutes.AuditWork.to_date.HasValue ?  checkAuditMinutes.AuditWork.to_date.Value.ToString("dd/MM/yyyy") : "dd/MM/yyyy",
                        checkAuditMinutes.AuditWork.AuditScopeOutside,
                        checkAuditMinutes.problem,
                        catDetectType!= null ? catDetectType.Name : "",
                        checkAuditMinutes.Users.FullName,
                        approvalStatus?.Users_Last?.FullName ?? "",
                        facilityType?.Name ?? "",
                    });
                    //ẩn hiện
                    for (int i = 0; i < sectionOther.Body.ChildObjects.Count; i++)
                    {
                        if (sectionOther.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
                        {
                            if (String.IsNullOrEmpty((sectionOther.Body.ChildObjects[i] as Paragraph).Text.Trim()))
                            {
                                sectionOther.Body.ChildObjects.Remove(sectionOther.Body.ChildObjects[i]);
                                i--;
                            }
                        }

                    }
                    sectionOther.AddParagraph().AppendHTML(!string.IsNullOrEmpty(checkAuditMinutes.OtherContent) ? checkAuditMinutes.OtherContent : "");


                    foreach (var (grp, i) in auditDetectGrp.Select((grp, i) => (grp, i)))
                    {
                        var paragraphTitle = sectionDetect.AddParagraph();
                        var txtSectionTitle = paragraphTitle.AppendText(String.Format("V.{0} Tổng hợp {1}", i + 2, grp.FirstOrDefault().CatDetectType.Name));
                        paragraphTitle.ApplyStyle("titlestyle");
                        if (i > 0)
                        {
                            paragraphTitle.Format.BeforeSpacing = 6;
                            paragraphTitle.Format.AfterSpacing = 6;
                        }


                        foreach (var (detect, j) in grp.Select((detect, j) => (detect, j)))
                        {
                            var risk_level = "";

                            switch (detect.rating_risk)
                            {
                                case 1:
                                    risk_level = "Cao/Quan trọng";
                                    break;
                                case 2:
                                    risk_level = "Trung bình";
                                    break;
                                case 3:
                                    risk_level = "Thấp/Ít quan trọng";
                                    break;
                                default:
                                    risk_level = "";
                                    break;
                            }

                            var paragraphDetectDetail = sectionDetect.AddParagraph();
                            var txtParagraphDetectDetail = paragraphDetectDetail.AppendText(string.Format("Phát hiện số {0}: {1}", j + 1, detect.short_title));
                            paragraphDetectDetail.ListFormat.ApplyStyle("levelstyle");
                            paragraphDetectDetail.Format.FirstLineIndent = -18f;
                            paragraphDetectDetail.ApplyStyle("titlestyle");

                            paragraphDetectDetail = sectionDetect.AddParagraph();
                            txtParagraphDetectDetail = paragraphDetectDetail.AppendText(string.Format("Chi tiết phát hiện và ảnh hưởng:"));
                            paragraphDetectDetail.ListFormat.ApplyStyle("bulletstyle");
                            paragraphDetectDetail.Format.FirstLineIndent = -28.08f;
                            paragraphDetectDetail.ApplyStyle("titlestyle");

                            paragraphDetectDetail = sectionDetect.AddParagraph();
                            txtParagraphDetectDetail = paragraphDetectDetail.AppendText(string.Format(detect.description));
                            paragraphDetectDetail.ApplyStyle("contentstyle");

                            paragraphDetectDetail = sectionDetect.AddParagraph();
                            txtParagraphDetectDetail = paragraphDetectDetail.AppendText(string.Format("Mức độ rủi ro: {0}", risk_level));
                            paragraphDetectDetail.ListFormat.ApplyStyle("bulletstyle");
                            paragraphDetectDetail.Format.FirstLineIndent = -28.08f;
                            paragraphDetectDetail.ApplyStyle("titlestyle");

                            paragraphDetectDetail = sectionDetect.AddParagraph();
                            txtParagraphDetectDetail = paragraphDetectDetail.AppendText(string.Format("Kiến nghị:"));
                            paragraphDetectDetail.ListFormat.ApplyStyle("bulletstyle");
                            paragraphDetectDetail.Format.FirstLineIndent = -28.08f;
                            paragraphDetectDetail.ApplyStyle("titlestyle");

                            Table table = sectionDetect.AddTable();
                            table.TableFormat.Borders.BorderType = BorderStyle.Single;
                            table.ResetCells(detect.AuditRequestMonitor.Count() + 1, 3);


                            var row = table.Rows[0];
                            row.Cells[0].SetCellWidth(60, CellWidthType.Percentage);
                            var paraRowTable = row.Cells[0].AddParagraph();
                            var txtParaRowTable = paraRowTable.AppendText("Nội dung kiến nghị");
                            paraRowTable.ApplyStyle("titlestyle");
                            txtParaRowTable.CharacterFormat.Italic = false;
                            paraRowTable.Format.HorizontalAlignment = HorizontalAlignment.Center;

                            row.Cells[1].SetCellWidth(22, CellWidthType.Percentage);
                            paraRowTable = row.Cells[1].AddParagraph();
                            txtParaRowTable = paraRowTable.AppendText("Đơn vị đầu mối");
                            paraRowTable.ApplyStyle("titlestyle");
                            txtParaRowTable.CharacterFormat.Italic = false;
                            paraRowTable.Format.HorizontalAlignment = HorizontalAlignment.Center;

                            row.Cells[2].SetCellWidth(18, CellWidthType.Percentage);
                            paraRowTable = row.Cells[2].AddParagraph();
                            txtParaRowTable = paraRowTable.AppendText("Thời hạn chỉnh sửa");
                            paraRowTable.ApplyStyle("titlestyle");
                            txtParaRowTable.CharacterFormat.Italic = false;
                            paraRowTable.Format.HorizontalAlignment = HorizontalAlignment.Center;

                            foreach (var (requestMonitor, k) in detect.AuditRequestMonitor.Select((requestMonitor, k) => (requestMonitor, k)))
                            {
                                var mapping = facilityRequestMonitorMapping.Where(x => requestMonitor.Id == x.audit_request_monitor_id).ToArray();

                                row = table.Rows[k + 1];
                                paraRowTable = row.Cells[0].AddParagraph();
                                txtParaRowTable = paraRowTable.AppendText(requestMonitor.Content);
                                paraRowTable.ApplyStyle("contentstyle");
                                paraRowTable.Format.HorizontalAlignment = HorizontalAlignment.Left;

                                paraRowTable = row.Cells[1].AddParagraph();
                                txtParaRowTable = paraRowTable.AppendText(mapping != null ? string.Join(", ", mapping.Select(x => x.audit_facility_name).Distinct()) : "");
                                paraRowTable.ApplyStyle("contentstyle");
                                paraRowTable.Format.HorizontalAlignment = HorizontalAlignment.Left;

                                paraRowTable = row.Cells[2].AddParagraph();
                                txtParaRowTable = paraRowTable.AppendText(requestMonitor.CompleteAt.HasValue ? requestMonitor.CompleteAt.Value.ToString("dd/MM/yyyy") : "");
                                paraRowTable.ApplyStyle("contentstyle");
                                paraRowTable.Format.HorizontalAlignment = HorizontalAlignment.Left;
                            }

                            paragraphDetectDetail = sectionDetect.AddParagraph();
                            txtParagraphDetectDetail = paragraphDetectDetail.AppendText(string.Format("Ý kiến của đơn vị được kiểm toán: {0}", detect.opinion_audit ? "Đồng ý" : "Không đồng ý"));
                            paragraphDetectDetail.ListFormat.ApplyStyle("bulletstyle");
                            paragraphDetectDetail.Format.FirstLineIndent = -28.08f;
                            paragraphDetectDetail.ApplyStyle("titlestyle");
                            paragraphDetectDetail.Format.BeforeSpacing = 6;

                            paragraphDetectDetail = sectionDetect.AddParagraph();
                            txtParagraphDetectDetail = paragraphDetectDetail.AppendText(string.Format("Nguyên nhân/Giải trình (nếu có)"));
                            paragraphDetectDetail.ApplyStyle("titlestyle");
                            txtParagraphDetectDetail.CharacterFormat.Italic = false;

                            paragraphDetectDetail = sectionDetect.AddParagraph();
                            txtParagraphDetectDetail = paragraphDetectDetail.AppendText(detect.reason);
                            paragraphDetectDetail.ApplyStyle("contentstyle");
                        }
                    }

                    MemoryStream stream = new MemoryStream();
                    stream.Position = 0;
                    doc.SaveToFile(stream, Spire.Doc.FileFormat.Docx);
                    Bytes = stream.ToArray();
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_BienBanKiemToan_v0.2.docx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        //select mức xếp hạng kt
        [HttpGet("RattingType")]
        public IActionResult RattingType(string q)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                string KeyWord = q;
                Expression<Func<SystemCategory, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Name.ToLower().Contains(q.ToLower()))
                && c.Deleted != true && c.ParentGroup == "MucXepHangKiemToan";
                var listcatdetecttype = _uow.Repository<SystemCategory>().Find(filter).OrderByDescending(a => a.Code);
                IEnumerable<SystemCategory> data = listcatdetecttype;
                var res = data.Select(a => new DropListRattingTypeModel()
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
        //xuất word MBS
        [HttpGet("ExportFileWordMBS/{id}")]
        public IActionResult ExportFileWordMBS(int id)
        {
            byte[] Bytes = null;
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditMinutes = _uow.Repository<AuditMinutes>().Include(a => a.AuditMinutesFile, a => a.SystemCategory).FirstOrDefault(a => a.id == id && a.IsDeleted.Equals(false));

                var checkperson = _uow.Repository<AuditMinutes>().Include(a => a.Users).FirstOrDefault(a => a.id == id
                && a.Users.Id == checkAuditMinutes.audit_work_person);

                var checkAuditWork = _uow.Repository<AuditMinutes>().Include(a => a.AuditWork).FirstOrDefault(a => a.id == id
                && a.AuditWork.Id == checkAuditMinutes.auditwork_id);

                var checkAuditWorkScope = _uow.Repository<AuditWorkScope>().Include(x => x.AuditWork).FirstOrDefault(x => x.auditwork_id == checkAuditMinutes.auditwork_id);

                var auditWorkScope = _uow.Repository<AuditWorkScope>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var auditDetect = _uow.Repository<AuditDetect>().Include(x => x.CatDetectType, x => x.AuditRequestMonitor).Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                 && a.IsDeleted != true
                 && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var auditRequestMonitor = _uow.Repository<AuditRequestMonitor>().Include(x => x.AuditDetect, x => x.Users).Where(a => a.is_deleted != true).ToArray();

                var day = DateTime.Now.ToString("dd");
                var month = DateTime.Now.ToString("MM");
                var year = DateTime.Now.ToString("yyyy");

                var countRiskRatingHight = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.rating_risk == 1
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countRiskRatingMedium = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.rating_risk == 2
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countRiskRatingLow = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.rating_risk == 3
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countOpinionAuditTrue = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.opinion_audit == true
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var countOpinionAuditFalse = _uow.Repository<AuditDetect>().GetAll().Where(a => a.auditwork_id == checkAuditMinutes.auditwork_id
                && a.IsDeleted != true
                && a.opinion_audit == false
                && a.auditfacilities_id == checkAuditMinutes.auditfacilities_id).ToArray();

                var ListStatisticsOfDetections = auditDetect.Select(a => new ListStatisticsOfDetections
                {
                    id = a.id,
                    audit_detect_code = a.code,
                    auditwork_id = a.auditwork_id,
                    auditfacilities_id = a.auditfacilities_id,
                    auditfacilities_name = a.auditfacilities_name,//đơn vị kiểm toán
                    title = a.title,//Tiêu đề phát hiện
                    year = a.year,
                    risk_rating = a.rating_risk,
                    str_classify_audit_detect = a.CatDetectType.Name,
                    reason = a.reason,
                    opinion_audit_true = countOpinionAuditTrue.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                    opinion_audit_false = countOpinionAuditFalse.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                    risk_rating_hight = countRiskRatingHight.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                    risk_rating_medium = countRiskRatingMedium.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                    risk_rating_low = countRiskRatingLow.Where(x => x.IsDeleted != true && x.CatDetectType.Name == a.CatDetectType.Name).Count(),
                }).GroupBy(a => a.str_classify_audit_detect).Select(z => z.FirstOrDefault()).ToList();
                //data

                var hearderSystem = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "REPORT_HEADER");
                var dataCt = _uow.Repository<SystemParameter>().FirstOrDefault(x => x.Parameter_Name == "COMPANY_NAME");

                //ẩn hiện
                var checkHiden = _uow.Repository<ConfigDocument>().GetAll().Where(a => a.item_name == "Biên bản kiểm toán"
                && a.isShow == true).ToArray();
                var MTKT = checkHiden.Where(a => a.item_code == "MTKT" && a.status == true).Count();
                var PVKT = checkHiden.Where(a => a.item_code == "PVKT" && a.status == true).Count();
                var GHKT = checkHiden.Where(a => a.item_code == "GHKT" && a.status == true).Count();
                var THKT = checkHiden.Where(a => a.item_code == "THKT" && a.status == true).Count();
                var KSHQ = checkHiden.Where(a => a.item_code == "KSHQ" && a.status == true).Count();
                var KSCT = checkHiden.Where(a => a.item_code == "KSCT" && a.status == true).Count();
                var YKKT = checkHiden.Where(a => a.item_code == "YKKT" && a.status == true).Count();
                var DSPHKT = checkHiden.Where(a => a.item_code == "DSPHKT" && a.status == true).Count();
                //ẩn hiện

                // Export word
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "MBS_Kitano_BienBanKiemToan_v0.1.docx");
                fullPath = fullPath.ToString().Replace("\\", "/");
                using (Document doc = new Document(fullPath))
                //using (Document doc = new Document(@"D:\test\MBS_Kitano_BienBanKiemToan_v0.1.docx"))
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
                    //if (MTKT != 1 && PVKT != 1 && GHKT != 1 && THKT != 1)
                    //{
                    //    doc.Replace("Cuộc kiểm toán nhằm đảm bảo các mục tiêu sau:", "", false, true);
                    //    doc.Replace("Các nội dung nằm ngoài phạm vi kiểm toán:", "", false, true);
                    //    doc.Replace("Thời hiệu kiểm toán từ ngày", "", false, true);
                    //    doc.Replace("đến ngày", "", false, true);
                    //}
                    //if (MTKT != 1)
                    //{
                    //    doc.Replace("Cuộc kiểm toán nhằm đảm bảo các mục tiêu sau:", "", false, true);
                    //}
                    //if (GHKT != 1)
                    //{
                    //    doc.Replace("Các nội dung nằm ngoài phạm vi kiểm toán:", "", false, true);
                    //}
                    //if (THKT != 1)
                    //{
                    //    doc.Replace("Thời hiệu kiểm toán từ ngày", "", false, true);
                    //    doc.Replace("đến ngày", "", false, true);
                    //}
                    //if (KSCT != 1)
                    //{
                    //    doc.Replace("Mức xếp hạng kiểm toán:", "", false, true);
                    //    doc.Replace("Bảng tổng hợp số lượng phát hiện", "", false, true);
                    //}
                    //if ((KSHQ != 1 && KSCT != 1 && YKKT != 1))
                    //{
                    //    doc.Replace("Dựa trên kết quả kiểm toán, đoàn Kiểm toán đưa ra đánh giá tổng quan và kết luận về:", "", false, true);
                    //}
                    doc.MailMerge.Execute(new string[] { "ten_cuoc_kt_2", "ngay_1", "thang_1", "nam_1", "nguoi_phu_trach_1",
                        //"td_tong_quan_1",//7
                        //"td_ket_luan_1",//7
                        //"td_muc_tieu_kt_1",//8
                        //"td_pham_vi_kt_1",//9
                        //"td_gioi_han_pham_vi_kt_1",//10
                        //"td_thoi_hieu_kt_1",//11
                        //"td_ks_hq_1",//12
                        //"td_ks_ct_1",//13
                        //"td_y_kien_dvkt_1",//14
                        //"td_danh_sach_phkt_1",//15
                        "muc_dich_kt_1",//16
                        "pham_vi_kt_new_1",//17
                        "ngoai_pham_vi_kt_1",//18
                        //"thoi_hieu_kt_tu_1",//19
                        //"thoi_hieu_kt_den_1",//20
                        //"cac_van_de_1" ,//21
                        //"muc_do_xep_hang_1",//22
                        //"y_kien_1",//23
                    },
                    //ẩn hiện
                    new string[] { checkAuditMinutes.auditwork_name, day, month, year, checkAuditMinutes.Users.FullName,
                    //(MTKT != 1 && PVKT != 1 && GHKT != 1 && THKT != 1) ? "" : "TỔNG QUAN",//7
                    //(KSHQ != 1 && KSCT != 1 && YKKT != 1) ? "" : "KẾT LUẬN",//7
                    //(MTKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "MTKT")?.content),//8
                    //(PVKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "PVKT")?.content),//9
                    //(GHKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "GHKT")?.content),//10
                    //(THKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "THKT")?.content),//11
                    //(KSHQ != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "KSHQ")?.content),//12
                    //(KSCT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "KSCT")?.content),//13
                    //(YKKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "YKKT")?.content),//14
                    //(DSPHKT != 1) ? "" : (checkHiden.FirstOrDefault(a=>a.item_code == "DSPHKT")?.content),//15
                    MTKT == 1 ? checkAuditMinutes.audit_work_taget: "",//16
                    PVKT == 1 ? checkAuditMinutes.AuditWork.AuditScope: "",//17
                    GHKT == 1 ? checkAuditMinutes.AuditWork.AuditScopeOutside: "",//18
                    //(THKT == 1 ? (checkAuditMinutes.AuditWork.from_date != null ? checkAuditMinutes.AuditWork.from_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy"): ""),//19
                    //(THKT == 1 ? (checkAuditMinutes.AuditWork.to_date != null ? checkAuditMinutes.AuditWork.to_date.Value.ToString("dd/MM/yyyy"): "dd/MM/yyyy"): ""),//20
                    //KSHQ == 1 ? checkAuditMinutes.problem: "",//21
                    //KSCT == 1 ? (checkAuditMinutes.SystemCategory != null ? checkAuditMinutes.SystemCategory.Name: "") : "",//22
                    //YKKT == 1 ? checkAuditMinutes.idea: "",//23
                    });
                    //ẩn hiện

                    //if (KSCT == 1)
                    //{
                    //    Table table1 = doc.Sections[0].Tables[1] as Table;
                    //    table1.ResetCells(ListStatisticsOfDetections.Count() + 2, 7);
                    //    table1.ApplyVerticalMerge(0, 0, 1);
                    //    table1.ApplyVerticalMerge(1, 0, 1);
                    //    table1.ApplyHorizontalMerge(0, 2, 4);
                    //    table1.ApplyHorizontalMerge(0, 5, 6);
                    //    TextRange txtTable1 = table1[0, 0].AddParagraph().AppendText("Loại phát hiện");
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 12;
                    //    txtTable1.CharacterFormat.Bold = true;

                    //    txtTable1 = table1[0, 1].AddParagraph().AppendText("Tổng cộng số lượng");
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 12;
                    //    txtTable1.CharacterFormat.Bold = true;

                    //    txtTable1 = table1[0, 2].AddParagraph().AppendText("Số lượng phát hiện");
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 12;
                    //    txtTable1.CharacterFormat.Bold = true;

                    //    txtTable1 = table1[0, 5].AddParagraph().AppendText("Ý kiến của đơn vị được kiểm toán");
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 12;
                    //    txtTable1.CharacterFormat.Bold = true;

                    //    txtTable1 = table1[1, 2].AddParagraph().AppendText("Cao/Quan trọng");
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 12;
                    //    txtTable1.CharacterFormat.Bold = true;

                    //    txtTable1 = table1[1, 3].AddParagraph().AppendText("Trung bình");
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 12;
                    //    txtTable1.CharacterFormat.Bold = true;

                    //    txtTable1 = table1[1, 4].AddParagraph().AppendText("Thấp/Ít quan trọng");
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 12;
                    //    txtTable1.CharacterFormat.Bold = true;

                    //    txtTable1 = table1[1, 5].AddParagraph().AppendText("Đồng ý");
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 12;
                    //    txtTable1.CharacterFormat.Bold = true;

                    //    txtTable1 = table1[1, 6].AddParagraph().AppendText("Không đồng ý");
                    //    txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //    txtTable1.CharacterFormat.FontSize = 12;
                    //    txtTable1.CharacterFormat.Bold = true;
                    //    for (int z = 0; z < ListStatisticsOfDetections.Count(); z++)
                    //    {
                    //        txtTable1 = table1[z + 2, 0].AddParagraph().AppendText(ListStatisticsOfDetections[z].str_classify_audit_detect);
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 12;

                    //        txtTable1 = table1[z + 2, 1].AddParagraph().AppendText((ListStatisticsOfDetections[z].risk_rating_hight + ListStatisticsOfDetections[z].risk_rating_medium + ListStatisticsOfDetections[z].risk_rating_low).ToString());
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 12;

                    //        txtTable1 = table1[z + 2, 2].AddParagraph().AppendText(ListStatisticsOfDetections[z].risk_rating_hight.ToString());
                    //        table1[z + 2, 2].CellFormat.BackColor = ColorTranslator.FromHtml("#FF0000");
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 12;

                    //        txtTable1 = table1[z + 2, 3].AddParagraph().AppendText(ListStatisticsOfDetections[z].risk_rating_medium.ToString());
                    //        table1[z + 2, 3].CellFormat.BackColor = ColorTranslator.FromHtml("#FFC000");
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 12;

                    //        txtTable1 = table1[z + 2, 4].AddParagraph().AppendText(ListStatisticsOfDetections[z].risk_rating_low.ToString());
                    //        table1[z + 2, 4].CellFormat.BackColor = ColorTranslator.FromHtml("#92D050");
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 12;

                    //        txtTable1 = table1[z + 2, 5].AddParagraph().AppendText(ListStatisticsOfDetections[z].opinion_audit_true.ToString());
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 12;

                    //        txtTable1 = table1[z + 2, 6].AddParagraph().AppendText(ListStatisticsOfDetections[z].opinion_audit_false.ToString());
                    //        txtTable1.CharacterFormat.FontName = "Times New Roman";
                    //        txtTable1.CharacterFormat.FontSize = 12;
                    //    }
                    //}
                    //else
                    //{
                    //    doc.Sections[0].Tables.Remove(doc.Sections[0].Tables[1]);
                    //}
                    if (DSPHKT == 1)
                    {
                        String[] _Title = { "", " Chi tiết nội dung và ảnh hưởng:", " Nguyên nhân:", " Mức độ rủi ro:", " Ý kiến của đơn vị:", " " };
                        for (int c = 0; c < auditDetect.Length; c++)
                        {
                            Section section = doc.Sections[0];
                            Paragraph paraInserted = section.AddParagraph();


                            TextRange textNew = paraInserted.AppendText(_Title[0] + " " + (c + 1) + ". " + "Phát hiện:" + auditDetect[c].title);
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;
                            textNew.CharacterFormat.Bold = true;

                            textNew = paraInserted.AppendText("\n" + _Title[1] + " " + auditDetect[c].description);
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[2] + " " + auditDetect[c].cause);
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[3] + " " + (auditDetect[c].rating_risk == 1 ? "Cao/Quan trọng" : auditDetect[c].rating_risk == 2 ? "Trung bình" : auditDetect[c].rating_risk == 3 ? "Thấp/Ít quan trọng" : ""));
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[4] + " " + (auditDetect[c].opinion_audit == true ? "Đồng ý" : "Không đồng ý"));
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            textNew = paraInserted.AppendText("\n" + _Title[5] + " " + (auditDetect[c].reason));
                            textNew.CharacterFormat.FontName = "Times New Roman";
                            textNew.CharacterFormat.FontSize = 12;

                            var _auditRequestMonitor = _uow.Repository<AuditRequestMonitor>().Include(x => x.AuditDetect, x => x.Users, x => x.FacilityRequestMonitorMapping).Where(a => a.is_deleted != true
                            && a.detectid == auditDetect[c].id).ToArray();

                            Table tablenew = section.AddTable(true);

                            String[] Header = { "Nội dung kiến nghị", "Đơn vị đầu mối", "Thời hạn hoàn thành" };
                            //Add Cells
                            tablenew.ResetCells(_auditRequestMonitor.Length, Header.Length);
                            //Header Row
                            TableRow FRow = tablenew.Rows[0];
                            FRow.IsHeader = true;
                            FRow.Height = 23;
                            //Row Height
                            for (int i = 0; i < Header.Length; i++)
                            {
                                //Cell Alignment
                                Paragraph p = FRow.Cells[i].AddParagraph();
                                FRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                p.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                //Data Format
                                TextRange TR = p.AppendText(Header[i]);
                                TR.CharacterFormat.FontName = "Times New Roman";
                                TR.CharacterFormat.FontSize = 12;
                                TR.CharacterFormat.Bold = true;
                            }
                            for (int dx = 0; dx < _auditRequestMonitor.Length; dx++)
                            {
                                string unit_name = "";
                                for (int zx = 0; zx < _auditRequestMonitor[dx].FacilityRequestMonitorMapping.Count(); zx++)
                                {
                                    unit_name = (_auditRequestMonitor[dx].FacilityRequestMonitorMapping != null ? string.Join(", ", _auditRequestMonitor[dx].FacilityRequestMonitorMapping.ToList().Where(c => c.type == 1 && c.audit_request_monitor_id.Equals(_auditRequestMonitor[dx].Id)).Select(x => x.audit_facility_name).Distinct()) : "");
                                }
                                    String[][] data = {
                            new String[]{ _auditRequestMonitor[dx].Content, unit_name,
                                _auditRequestMonitor[dx].CompleteAt.Value.ToString("dd/MM/yyyy") },};
                                    //for (int r = 0; r < _auditRequestMonitor.Length; r++)
                                    //{
                                    TableRow DataRow = tablenew.Rows[dx];
                                    //Row Height
                                    DataRow.Height = 20;


                                    //C Represents Column.
                                    for (int xc = 0; xc < data[0].Length; xc++)
                                    //for (int xc = 0; xc < _auditRequestMonitor.Length; xc++)
                                    {
                                        //Cell Alignment
                                        DataRow.Cells[xc].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                        //Fill Data in Rows
                                        Paragraph p2 = DataRow.Cells[xc].AddParagraph();
                                        TextRange TR2 = p2.AppendText(data[0][xc]);
                                        //Format Cells
                                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                        TR2.CharacterFormat.FontName = "Times New Roman";
                                        TR2.CharacterFormat.FontSize = 12;
                                    }
                                    //}
                            }
                            doc.SaveToFile("Kitano_BienBanKiemToan_v0.2.docx");
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
                    return File(Bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Kitano_BienBanKiemToan_v0.2.docx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
