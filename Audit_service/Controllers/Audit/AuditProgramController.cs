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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class AuditProgramController : BaseController
    {
        protected readonly IConfiguration _config;
        public AuditProgramController(ILoggerManager logger, IUnitOfWork uow, IConfiguration config) : base(logger, uow)
        {
            _config = config;
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditProgramModelSearch>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                //var year = !string.IsNullOrEmpty(obj.Year) ? Convert.ToInt32(obj.Year) : -1;
                var AuditProcess = obj.AuditProcess.HasValue ? obj.AuditProcess : -1;
                var AuditWork = obj.AuditWork.HasValue ? obj.AuditWork : -1;
                var Facility = obj.Facility.HasValue ? obj.Facility : -1;
                var Activity = obj.Activity.HasValue ? obj.Activity : -1;
                var status = obj.Status;
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_APRG" && (string.IsNullOrEmpty(status) || a.StatusCode == status)).ToArray();
                var list_appoval_id = approval_status.Select(a => a.item_id).ToList();
                var auditprogram = _uow.Repository<AuditProgram>().Include(a => a.AuditWork).Where(a => (AuditWork == -1 || a.auditwork_id == AuditWork) && (!obj.Year.HasValue || a.Year == obj.Year) && (AuditProcess == -1 || a.auditprocess_id == AuditProcess) && (Facility == -1 || a.auditfacilities_id == Facility) && (Activity == -1 || a.bussinessactivities_id == Activity) && (string.IsNullOrEmpty(status) || list_appoval_id.Contains(a.Id) || (status == "1.0" && !list_appoval_id.Contains(a.Id))) && a.IsDeleted != true).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditProgram> data = auditprogram;
                var count = data.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var result = data.Select(a => new AuditProgramListModel()
                {
                    Id = a.Id,
                    Year = a.Year + "",
                    AuditWork = a.AuditWork?.Name,
                    AuditWorkID = a.AuditWork?.Id,
                    AuditActivityID = a.bussinessactivities_id,
                    AuditActivity = a.bussinessactivities_name,
                    AuditProcess = a.auditprocess_name,
                    AuditProcessID = a.auditprocess_id,
                    AuditFacility = a.auditfacilities_name,
                    AuditFacilityID = a.auditfacilities_id,
                    Status = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.StatusCode ?? "1.0",
                    ApprovalUser = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.approver,
                    ApprovalUserLast = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.approver_last
                });
                //var r
                return Ok(new { code = "1", msg = "success", data = result, total = count });

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("SearchRisk")]
        public IActionResult SearchRisk(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditProgramRiskModelSearch>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var model = _uow.Repository<ProcessLevelRiskScoring>().Include(a => a.CatRisk).Where(a => a.auditprogram_id == obj.AuditWorkScopeId && a.IsDeleted != true
                && (string.IsNullOrEmpty(obj.RiskName) || (a.catrisk_id.HasValue && a.CatRisk.Name.ToLower().Contains(obj.RiskName.ToLower().Trim())))
                && (string.IsNullOrEmpty(obj.RiskCode) || (a.catrisk_id.HasValue && a.CatRisk.Code.ToLower().Contains(obj.RiskCode.ToLower().Trim())))
                && (obj.AuditProposal == -1 || (obj.AuditProposal == 1 ? a.AuditProposal == true : a.AuditProposal != true))).ToArray();
                IEnumerable<ProcessLevelRiskScoring> data = model;
                var count = data.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var riskid = data.Select(a => a.catrisk_id.ToString()).ToList();
                var workingpaperList = _uow.Repository<WorkingPaper>().Find(a => a.isdelete != true).ToArray();
                var workingpaper = workingpaperList.Where(a => riskid.Any(p => a.risk_str.Contains(p))).ToArray();
                var result = data.Select(a => new AuditRiskModel()
                {
                    Id = a.Id,
                    CatRiskCode = a.catrisk_id.HasValue ? a.CatRisk.Code : "",
                    CatRiskName = a.catrisk_id.HasValue ? a.CatRisk.Name : "",
                    Potential_Possibility = a.Potential_Possibility,
                    Potential_InfulenceLevel = a.Potential_InfulenceLevel,
                    ScorePotentialRiskRating = (a.Potential_Possibility.HasValue ? a.Potential_Possibility : 0) * (a.Potential_InfulenceLevel.HasValue ? a.Potential_InfulenceLevel : 0),
                    Potential_RiskRating_Name = a.Potential_RiskRating_Name,
                    ScoreRemainingRisk = (a.Remaining_Possibility.HasValue ? a.Remaining_Possibility : 0) * (a.Remaining_InfulenceLevel.HasValue ? a.Remaining_InfulenceLevel : 0),
                    AuditProposal = a.AuditProposal == true ? 1 : 0,
                    WorkingPaper = string.Join(", ", workingpaper.Where(x => x.risk_str.Contains(a.catrisk_id.ToString())).Select(x => x.code).Distinct()),
                });
                //var r
                return Ok(new { code = "1", msg = "success", data = result, total = count });

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
                var auditProgram = _uow.Repository<AuditProgram>().FirstOrDefault(a => a.Id == id);
                if (auditProgram == null)
                {
                    return NotFound();
                }

                auditProgram.IsDeleted = true;
                auditProgram.DeletedBy = userInfo.Id;
                auditProgram.DeletedAt = DateTime.Now;
                _uow.Repository<AuditProgram>().Update(auditProgram);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpDelete("DeleteRisk/{id}")]
        public IActionResult DeleteRiskConfirmed(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var ProcessLevelRisk = _uow.Repository<ProcessLevelRiskScoring>().FirstOrDefault(a => a.Id == id);
                if (ProcessLevelRisk == null)
                {
                    return NotFound();
                }

                ProcessLevelRisk.IsDeleted = true;
                ProcessLevelRisk.DeletedAt = DateTime.Now;
                ProcessLevelRisk.DeletedBy = userInfo.Id;
                _uow.Repository<ProcessLevelRiskScoring>().Update(ProcessLevelRisk);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpDelete("DeleteProcedures/{id}")]
        public IActionResult DeleteProceduresConfirmed(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var RiskScoringProcedures = _uow.Repository<RiskScoringProcedures>().FirstOrDefault(a => a.Id == id);
                if (RiskScoringProcedures == null)
                {
                    return NotFound();
                }

                RiskScoringProcedures.IsDeleted = true;
                RiskScoringProcedures.DeletedAt = DateTime.Now;
                RiskScoringProcedures.DeletedBy = userInfo.Id;
                _uow.Repository<RiskScoringProcedures>().Update(RiskScoringProcedures);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public IActionResult Create([FromBody] AuditProgramModifyModel audiprograminfo)
        {
            try
            {

                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkAuditprogram = _uow.Repository<AuditProgram>().Find(a => a.auditwork_id == audiprograminfo.auditwork_id && a.auditprocess_id == audiprograminfo.auditprocess_id && a.auditfacilities_id == audiprograminfo.auditfacilities_id && a.IsDeleted != true).ToArray();
                if (checkAuditprogram.Length > 0)
                {
                    return Ok(new { code = "0", msg = "fail" });
                }
                var _auditprogram = new AuditProgram
                {
                    auditwork_id = audiprograminfo.auditwork_id,
                    auditprocess_name = audiprograminfo.auditprocess_name,
                    auditprocess_id = audiprograminfo.auditprocess_id,
                    auditfacilities_name = audiprograminfo.auditfacilities_name,
                    auditfacilities_id = audiprograminfo.auditfacilities_id,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userInfo.Id,
                    IsActive = true,
                    Year = audiprograminfo.Year,
                    Status = 1,
                };
                var process = _uow.Repository<AuditProcess>().FirstOrDefault(a => a.Id == audiprograminfo.auditprocess_id);
                if (process != null)
                {
                    _auditprogram.bussinessactivities_id = process.ActivityId;
                    _auditprogram.bussinessactivities_name = process.ActivityName;
                }
                _uow.Repository<AuditProgram>().Add(_auditprogram);

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
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

                var audit_program = _uow.Repository<AuditProgram>().FirstOrDefault(a => a.Id == id);

                if (audit_program != null)
                {
                    var approval_status = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.item_id == id && a.function_code == "M_APRG");
                    var auditprogram_file = _uow.Repository<ApprovalFunctionFile>().Find(a => a.item_id == id && a.function_code == "M_APRG" && a.IsDeleted != true).ToArray();
                    var auditwork = _uow.Repository<AuditWork>().Include(a => a.Users).FirstOrDefault(a => a.Id == audit_program.auditwork_id);
                    var ProcessLevelRiskScoring = _uow.Repository<ProcessLevelRiskScoring>().Include(a => a.CatRisk).Where(a => a.auditprogram_id == audit_program.Id && a.IsDeleted != true).ToArray();
                    var riskid = ProcessLevelRiskScoring.Select(a => a.catrisk_id.ToString()).ToList();
                    var workingpaperList = _uow.Repository<WorkingPaper>().Find(a => a.isdelete != true && auditwork != null && a.auditworkid == auditwork.Id).ToArray();
                    var workingpaper = workingpaperList.Where(a => riskid.Any(p => a.risk_str.Contains(p)) ).ToArray();
                    var result = new AuditProgramDetailModel()
                    {
                        Id = audit_program.Id,
                        Year = audit_program.Year + "",
                        AuditWork = auditwork?.Name,
                        AuditWorkId = auditwork?.Id,
                        AuditActivity = audit_program.bussinessactivities_name,
                        AuditActivityId = audit_program.bussinessactivities_id,
                        AuditProcess = audit_program.auditprocess_name,
                        AuditProcessId = audit_program.auditprocess_id,
                        AuditFacility = audit_program.auditfacilities_name,
                        AuditFacilityId = audit_program.auditfacilities_id,
                        PersonInCharge = auditwork.person_in_charge.HasValue ? auditwork.Users.FullName : "",
                        Status = approval_status?.StatusCode ?? "1.0",
                        AuditRiskList = ProcessLevelRiskScoring.Select(a => new AuditRiskModel()
                        {
                            Id = a.Id,
                            CatRiskCode = a.catrisk_id.HasValue ? a.CatRisk.Code : "",
                            CatRiskName = a.catrisk_id.HasValue ? a.CatRisk.Name : "",
                            CatRiskID = a.catrisk_id,
                            Potential_Possibility = a.Potential_Possibility,
                            Potential_InfulenceLevel = a.Potential_InfulenceLevel,
                            ScorePotentialRiskRating = (a.Potential_Possibility.HasValue ? a.Potential_Possibility : 0) * (a.Potential_InfulenceLevel.HasValue ? a.Potential_InfulenceLevel : 0),
                            //Potential_RiskRating = a.Potential_RiskRating,
                            Potential_RiskRating_Name = a.Potential_RiskRating_Name,
                            ScoreRemainingRisk = (a.Remaining_Possibility.HasValue ? a.Remaining_Possibility : 0) * (a.Remaining_InfulenceLevel.HasValue ? a.Remaining_InfulenceLevel : 0),
                            AuditProposal = a.AuditProposal == true ? 1 : 0,
                            WorkingPaper = string.Join(", ", workingpaper.Where(x => x.risk_str.Contains(a.catrisk_id.ToString())).Select(x => x.code).Distinct()),
                        }).ToList(),
                        ListFile = auditprogram_file.Select(x => new AuditProgramFileModel()
                        {
                            id = x.id,
                            Path = x.Path,
                            FileType = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = result });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
        }
        public class RiskDetailModel
        {
            public int? id_audit_scope { get; set; }
            public int? id_process_risk { get; set; }
        }
        [HttpGet("RiskDetail/{id}")]
        public IActionResult RiskDetails(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var record = _uow.Repository<ProcessLevelRiskScoring>().Include(a => a.CatRisk).FirstOrDefault(a => a.Id == id);

                if (record != null)
                {
                    var catcontrol = _uow.Repository<RiskControl>().Include(a => a.CatControl).Where(a => a.riskid == record.catrisk_id && a.isdeleted != true).ToArray();
                    var result = new AuditRiskDetailModel()
                    {
                        Id = record.Id,
                        AuditScopeId = record.auditprogram_id,
                        CatRiskID = record.catrisk_id,
                        CatRiskCode = record.catrisk_id.HasValue ? record.CatRisk.Code : "",
                        CatRiskName = record.catrisk_id.HasValue ? record.CatRisk.Name : "",
                        CatControlList = catcontrol.Select(x => new CatControlDetailModel()
                        {
                            Id = x.CatControl.Id,
                            Code = x.CatControl?.Code,
                            Description = x.CatControl?.Description,
                            Controlformat = x.CatControl?.Controlformat,
                            Controltype = x.CatControl?.Controltype,
                            Controlfrequency = x.CatControl?.Controlfrequency,
                        }).ToList(),
                        DescriptionCatRisk = record.catrisk_id.HasValue ? record.CatRisk.Description : "",
                        Potential_Possibility = record.Potential_Possibility,
                        Potential_InfulenceLevel = record.Potential_InfulenceLevel,
                        Potential_RiskRating = record.Potential_RiskRating,
                        Potential_ReasonRating = record.Potential_ReasonRating,
                        AuditProposal = record.AuditProposal == true ? 1 : 0,
                        Remaining_Possibility = record.Remaining_Possibility,
                        Remaining_InfulenceLevel = record.Remaining_InfulenceLevel,
                        Remaining_RiskRating = record.Remaining_RiskRating,
                        Remaining_ReasonRating = record.Remaining_ReasonRating,
                    };

                    var RiskProcedures = _uow.Repository<RiskScoringProcedures>().Include(a => a.CatAuditProcedures).Where(a => a.risk_scoring_id == id && a.IsDeleted != true).Select(a => new ProgramProceduresDetailModel()
                    {
                        Id = a.Id,
                        Code = a.catprocedures_id.HasValue ? a.CatAuditProcedures.Code : "",
                        Name = a.catprocedures_id.HasValue ? a.CatAuditProcedures.Name : "",
                        Description = a.catprocedures_id.HasValue ? a.CatAuditProcedures.Description : "",
                        ControlID = a.catprocedures_id.HasValue ? a.CatAuditProcedures.cat_control_id : -1,
                        lstAuditor = a.user_id.HasValue ? a.Users.Id + ":" + a.Users.FullName : "",
                    }).ToList();
                    //var lstusers = _uow.Repository<Users>().Find(a => a.IsActive == true && a.IsDeleted != true).ToArray();
                    var cat_Control = _uow.Repository<CatControl>().Find(a => a.Status == 1).ToArray();
                    RiskProcedures.ForEach(o =>
                    {
                        o.ControlCode = cat_Control.FirstOrDefault(a => a.Id == o.ControlID)?.Code;
                    });
                    result.listProgramProcedures = RiskProcedures.OrderBy(a => a.ControlID).ToList();
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
        [HttpPut("Update")]
        public IActionResult RiskEdit([FromBody] AuditRiskModifyModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var record = _uow.Repository<ProcessLevelRiskScoring>().Include(a => a.CatRisk).FirstOrDefault(a => a.Id == model.Id);
                if (record == null)
                {
                    return NotFound();
                }
                record.Potential_Possibility = model.Potential_Possibility;
                record.Potential_InfulenceLevel = model.Potential_InfulenceLevel;
                record.Potential_RiskRating = model.Potential_RiskRating;
                record.Potential_RiskRating_Name = model.Potential_RiskRating.HasValue && model.Potential_RiskRating > 0 ? model.PotentialRiskRatingName : "";
                record.Potential_ReasonRating = model.Potential_ReasonRating;
                record.AuditProposal = model.AuditProposal == 1;
                record.Remaining_Possibility = model.Remaining_Possibility;
                record.Remaining_InfulenceLevel = model.Remaining_InfulenceLevel;
                record.Remaining_RiskRating = model.Remaining_RiskRating;
                record.Remaining_RiskRating_Name = model.Remaining_RiskRating.HasValue && model.Remaining_RiskRating > 0 ? model.RemainingRiskRatingName : "";
                record.Remaining_ReasonRating = model.Remaining_ReasonRating;
                record.ModifiedAt = DateTime.Now;
                record.ModifiedBy = _userInfo.Id;
                var listRiskScoringProcedures = new List<RiskScoringProcedures>();
                if (model.listControlProcedures.Count > 0)
                {
                    var RiskProcedures = _uow.Repository<RiskScoringProcedures>().Find(a => a.risk_scoring_id == model.Id && a.IsDeleted != true).ToArray();
                    foreach (var item in model.listControlProcedures)
                    {
                        var RiskProcedures_item = RiskProcedures.FirstOrDefault(a => a.Id == item.id);
                        if (RiskProcedures_item != null)
                        {
                            RiskProcedures_item.user_id = item.AuditorId;
                            listRiskScoringProcedures.Add(RiskProcedures_item);
                        }
                    }
                }
                _uow.Repository<RiskScoringProcedures>().UpdateWithoutSave(listRiskScoringProcedures);
                _uow.Repository<ProcessLevelRiskScoring>().UpdateWithoutSave(record);
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("AddRisk")]
        public IActionResult CreateRisk([FromBody] ProgramCatRiskCreateModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var _allCatRisk = _uow.Repository<CatRisk>().Find(a => a.IsDeleted != true && a.Status == 1 && a.Code.ToLower() == model.Code.ToLower().Trim()).ToArray();
                if (_allCatRisk.Length > 0)
                {
                    return Ok(new { code = "0", msg = "success" });
                }
                var catriskinfo = new CatRisk()
                {
                    Code = model.Code,
                    Name = string.IsNullOrEmpty(model.Name) ? model.Name : model.Name.Trim(),
                    Description = string.IsNullOrEmpty(model.Description) ? model.Description : model.Description.Trim(),
                    Status = model.Status,
                    Unitid = model.Unitid,
                    Activationid = model.Activationid,
                    Processid = model.Processid,
                    RelateStep = string.IsNullOrEmpty(model.RelateStep) ? model.RelateStep : model.RelateStep.Trim(),
                    IsDeleted = false,
                    Createdate = DateTime.Now,
                    CreatedBy = userInfo.Id,
                };

                var process_scope = new ProcessLevelRiskScoring()
                {
                    CatRisk = catriskinfo,
                    auditprogram_id = model.AuditWorkScopeId,
                    IsActive = true,
                    IsDeleted = false,
                };
                _uow.Repository<CatRisk>().AddWithoutSave(catriskinfo);
                _uow.Repository<ProcessLevelRiskScoring>().AddWithoutSave(process_scope);

                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("AddProcedures")]
        public IActionResult CreateProcedures([FromBody] ProgramProceduresModifyModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var _allCatProcedures = _uow.Repository<CatAuditProcedures>().Find(a => a.IsDeleted != true && a.Status == 1 && a.Code.ToLower() == model.Code.ToLower().Trim()).ToArray();
                if (_allCatProcedures.Length > 0)
                {
                    return Ok(new { code = "0", msg = "success" });
                }
                var catproceduresinfo = new CatAuditProcedures()
                {
                    Code = string.IsNullOrEmpty(model.Code) ? model.Code : model.Code.Trim(),

                    Name = string.IsNullOrEmpty(model.Name) ? model.Name : model.Name.Trim(),
                    Description = string.IsNullOrEmpty(model.Description) ? model.Description : model.Description.Trim(),
                    Status = model.Status,
                    Unitid = model.Unitid,
                    Activationid = model.Activationid,
                    Processid = model.Processid,
                    cat_control_id = model.ControlID,
                    IsDeleted = false,
                    Createdate = DateTime.Now,
                    Createby = userInfo.Id
                };

                var risk_scoring_procedures = new RiskScoringProcedures()
                {
                    CatAuditProcedures = catproceduresinfo,
                    risk_scoring_id = model.RiskScoringId,
                    IsDeleted = false,
                };
                _uow.Repository<CatAuditProcedures>().AddWithoutSave(catproceduresinfo);
                _uow.Repository<RiskScoringProcedures>().AddWithoutSave(risk_scoring_procedures);

                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("AddRiskLib")]
        public IActionResult AddRiskLib([FromBody] AddRiskLibModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var audit_work_scope = _uow.Repository<AuditProgram>().FirstOrDefault(a => a.Id == model.Id);
                if (audit_work_scope == null)
                {
                    return NotFound();
                }
                var ProcessLevelRiskScoring_old = _uow.Repository<ProcessLevelRiskScoring>().Find(a => a.auditprogram_id == audit_work_scope.Id && a.IsDeleted != true).Select(a => a.catrisk_id).ToArray();
                var _allCatRisk = _uow.Repository<CatRisk>().Find(a => a.IsDeleted != true && a.Status == 1 && a.Unitid == model.Unitid && a.Processid == model.Processid).ToArray();
                for (int i = 0; i < _allCatRisk.Length; i++)
                {
                    var catriskinfo = _allCatRisk[i];
                    if (!ProcessLevelRiskScoring_old.Contains(catriskinfo.Id))
                    {
                        var process_scope = new ProcessLevelRiskScoring()
                        {
                            CatRisk = catriskinfo,
                            auditprogram_id = model.Id,
                            IsActive = true,
                            IsDeleted = false,
                        };
                        _uow.Repository<ProcessLevelRiskScoring>().AddWithoutSave(process_scope);

                        var riskConrol = _uow.Repository<RiskControl>().Find(a => a.isdeleted != true && a.riskid == catriskinfo.Id).Select(a => a.controlid).ToList();
                        var _allCatProcedures = _uow.Repository<CatAuditProcedures>().Find(a => a.IsDeleted != true && a.Status == 1 && a.cat_control_id.HasValue && riskConrol.Contains(a.cat_control_id.Value)).ToArray();
                        for (int j = 0; j < _allCatProcedures.Length; j++)
                        {
                            var catproceduresinfo = _allCatProcedures[j];
                            var risk_scoring_procedures = new RiskScoringProcedures()
                            {
                                CatAuditProcedures = catproceduresinfo,
                                ProcessLevelRiskScoring = process_scope,
                                IsDeleted = false,
                            };
                            _uow.Repository<RiskScoringProcedures>().AddWithoutSave(risk_scoring_procedures);
                        }
                    }
                }
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("AddProceduresLib")]
        public IActionResult AddProceduresLib([FromBody] AddProceduresLibModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var risk_scoring_procedures_old = _uow.Repository<RiskScoringProcedures>().Find(a => a.IsDeleted != true && a.risk_scoring_id == model.ProcessRiskID).Select(a => a.catprocedures_id).ToArray();
                var riskConrol = _uow.Repository<RiskControl>().Find(a => a.isdeleted != true && a.riskid == model.CatRiskId).Select(a => a.controlid).ToList();
                var _allCatProcedures = _uow.Repository<CatAuditProcedures>().Find(a => a.IsDeleted != true && a.Status == 1 && a.cat_control_id.HasValue && riskConrol.Contains(a.cat_control_id.Value)).ToArray();
                for (int j = 0; j < _allCatProcedures.Length; j++)
                {
                    var catproceduresinfo = _allCatProcedures[j];
                    if (!risk_scoring_procedures_old.Contains(catproceduresinfo.Id))
                    {
                        var risk_scoring_procedures = new RiskScoringProcedures()
                        {
                            CatAuditProcedures = catproceduresinfo,
                            risk_scoring_id = model.ProcessRiskID,
                            IsDeleted = false,
                        };
                        _uow.Repository<RiskScoringProcedures>().AddWithoutSave(risk_scoring_procedures);
                    }

                }
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        //Chức năng copy chương trình kiểm toán
        [HttpPost("Copy")]
        public IActionResult Copy([FromBody] AuditProgramCopyModel audiprograminfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var auditprogram = _uow.Repository<AuditProgram>().FirstOrDefault(a => a.Id == audiprograminfo.id);
                if (auditprogram == null)
                {
                    return NotFound();
                }
                var checkAuditprogram = _uow.Repository<AuditProgram>().Find(a => a.auditwork_id == audiprograminfo.auditwork_id && a.auditprocess_id == audiprograminfo.auditprocess_id && a.auditfacilities_id == audiprograminfo.auditfacilities_id && a.IsDeleted != true).ToArray();
                if (checkAuditprogram.Length > 0)
                {
                    return Ok(new { code = "0", msg = "fail" });
                }
               
                var _auditprogram = new AuditProgram
                {
                    auditwork_id = audiprograminfo.auditwork_id,
                    auditprocess_name = audiprograminfo.auditprocess_name,
                    auditprocess_id = audiprograminfo.auditprocess_id,
                    auditfacilities_name = audiprograminfo.auditfacilities_name,
                    auditfacilities_id = audiprograminfo.auditfacilities_id,
                    bussinessactivities_id = auditprogram.bussinessactivities_id,
                    bussinessactivities_name = auditprogram.bussinessactivities_name,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userInfo.Id,
                    IsActive = true,
                    Year = audiprograminfo.Year,
                    Status = 1,
                };
                var ProcessLevelRiskScoring = _uow.Repository<ProcessLevelRiskScoring>().Find(a => a.auditprogram_id == audiprograminfo.id && a.IsDeleted != true).ToArray();
                var RiskScoringId = ProcessLevelRiskScoring.Select(a => a.Id).ToList();
                var risk_scoring_procedures_old = _uow.Repository<RiskScoringProcedures>().Find(a => a.IsDeleted != true && a.risk_scoring_id.HasValue && RiskScoringId.Contains(a.risk_scoring_id.Value)).ToArray();
                var listProcessLevelRiskScoring = new List<ProcessLevelRiskScoring>();
                var listRiskScoringProcedures = new List<RiskScoringProcedures>();
                foreach (var item in ProcessLevelRiskScoring)
                {
                    var process_scope = new ProcessLevelRiskScoring()
                    {
                        catrisk_id = item.catrisk_id,
                        AuditProgram = _auditprogram,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    process_scope.Potential_Possibility = item.Potential_Possibility;
                    process_scope.Potential_InfulenceLevel = item.Potential_InfulenceLevel;
                    process_scope.Potential_RiskRating = item.Potential_RiskRating;
                    process_scope.Potential_RiskRating_Name = item.Potential_RiskRating_Name;
                    process_scope.Potential_ReasonRating = item.Potential_ReasonRating;
                    process_scope.AuditProposal = item.AuditProposal;
                    process_scope.Remaining_Possibility = item.Remaining_Possibility;
                    process_scope.Remaining_InfulenceLevel = item.Remaining_InfulenceLevel;
                    process_scope.Remaining_RiskRating = item.Remaining_RiskRating;
                    process_scope.Remaining_RiskRating_Name = item.Remaining_RiskRating_Name;
                    process_scope.Remaining_ReasonRating = item.Remaining_ReasonRating;
                    var risk_scoring_procedures_item = risk_scoring_procedures_old.Where(x => x.risk_scoring_id == item.Id).ToArray();
                    foreach (var item_procedures in risk_scoring_procedures_item)
                    {
                        var risk_scoring_procedures = new RiskScoringProcedures()
                        {
                            catprocedures_id = item_procedures.catprocedures_id,
                            ProcessLevelRiskScoring = process_scope,                            
                            IsDeleted = false,
                        };
                        listRiskScoringProcedures.Add(risk_scoring_procedures);
                    }
                    listProcessLevelRiskScoring.Add(process_scope);
                }
                _uow.Repository<AuditProgram>().AddWithoutSave(_auditprogram);
                foreach (var item in listProcessLevelRiskScoring)
                {
                    _uow.Repository<ProcessLevelRiskScoring>().AddWithoutSave(item);
                }
                foreach (var item in listRiskScoringProcedures)
                {
                    _uow.Repository<RiskScoringProcedures>().AddWithoutSave(item);
                }
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
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
        [HttpPost("ExportExcel/{id}")]
        public IActionResult Export(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var audit_program = _uow.Repository<AuditProgram>().Include(a => a.AuditWork).FirstOrDefault(a => a.Id == id && a.IsDeleted != true);
                if (audit_program == null)
                {
                    return NotFound();
                }
                var user = _uow.Repository<Users>().FirstOrDefault(a => audit_program.CreatedBy == a.Id && a.IsDeleted != true);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage;
                byte[] Bytes = null;
                var memoryStream = new MemoryStream();
                var fullPath = Path.Combine(_config["Template:AuditDocsTemplate"], "Kitano_ChuongTrinhKiemToan.xlsx");
                var template = new FileInfo(fullPath);
                using (excelPackage = new ExcelPackage(template, false))
                {
                    var _paramInfoPrefix = "Param.SystemInfo";
                    var worksheet = excelPackage.Workbook.Worksheets["Sheet1"];
                    var iCache = (IConnectionMultiplexer)HttpContext.RequestServices
                     .GetService(typeof(IConnectionMultiplexer));
                    if (!iCache.IsConnected)
                    {
                        return BadRequest();
                    }
                    var redisDb = iCache.GetDatabase();
                    var value_get = redisDb.StringGet(_paramInfoPrefix);
                    var company = "";
                    if (value_get.HasValue)
                    {
                        var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
                        company = list_param.FirstOrDefault(a => a.name == "COMPANY_NAME")?.value;
                    }
                    ExcelRange cellCompany = worksheet.Cells[1, 2];
                    cellCompany.Value = company;

                    ExcelRange cellAuditWork = worksheet.Cells[4, 3];
                    cellAuditWork.Value = audit_program?.AuditWork?.Name;
                    cellAuditWork.Style.Font.Size = 11;
                    //cellAuditWork.Style.Font.Bold = true;

                    ExcelRange cellUnit = worksheet.Cells[5, 3];
                    cellUnit.Value = audit_program?.auditfacilities_name;
                    cellUnit.Style.Font.Size = 11;
                    //cellUnit.Style.Font.Bold = true;

                    ExcelRange cellProcess = worksheet.Cells[6, 3];
                    cellProcess.Value = audit_program?.auditprocess_name;
                    cellProcess.Style.Font.Size = 11;
                    //cellUnit.Style.Font.Bold = true;

                    ExcelRange cellCreater = worksheet.Cells[5, 9];
                    cellCreater.Value = user?.FullName;
                    cellCreater.Style.Font.Size = 11;

                    ExcelRange cellCreateAt = worksheet.Cells[5, 10];
                    cellCreateAt.Value = audit_program?.CreatedAt?.ToString("dd/MM/yyyy");
                    cellCreateAt.Style.Font.Size = 11;

                    var approval_status = _uow.Repository<ApprovalFunction>().Include(a => a.Users, a => a.Users_Last).FirstOrDefault(a => a.item_id == id && a.function_code == "M_APRG");
                    ExcelRange cellApprover = worksheet.Cells[6, 9];
                    cellApprover.Value = approval_status != null ? (approval_status.approver_last.HasValue ? approval_status?.Users_Last?.FullName : approval_status?.Users?.FullName) : "";
                    cellApprover.Style.Font.Size = 11;

                    ExcelRange cellApproverAt = worksheet.Cells[6, 10];
                    cellApproverAt.Value = approval_status != null ? approval_status.ApprovalDate?.ToString("dd/MM/yyyy") : "";
                    cellApproverAt.Style.Font.Size = 11;

                    var record = _uow.Repository<ProcessLevelRiskScoring>().Include(a => a.CatRisk).Where(a => a.auditprogram_id == id && a.IsDeleted != true).ToArray();
                    var catrisk_id = record.Select(a => a.catrisk_id.ToString()).ToList();
                    var record_id = record.Select(a => a.Id).ToList();
                    var catrisklevel_id = record.Select(a => a.Potential_RiskRating).ToList();
                    var catcontrolTotal = _uow.Repository<RiskControl>().Include(a => a.CatControl).Where(a => catrisk_id.Contains(a.riskid.ToString()) && a.isdeleted != true).ToArray();
                    var catrisklevels = _uow.Repository<CatRiskLevel>().Find(a => catrisklevel_id.Contains(a.Id) && a.IsDeleted != true).ToArray();
                    var RiskProcedures = _uow.Repository<RiskScoringProcedures>().Include(a => a.CatAuditProcedures).Where(a => record_id.Contains(a.risk_scoring_id.Value) && a.IsDeleted != true).ToArray();
                    var workingpaperList = _uow.Repository<WorkingPaper>().Find(a => a.isdelete != true).ToArray();
                    var workingpaper = workingpaperList.Where(a => catrisk_id.Any(p=>a.risk_str.Contains(p))).ToArray();
                    var riskType = _uow.Repository<SystemCategory>().Find(a => a.ParentGroup == "LoaiRuiRoQuyTrinh" && a.Deleted != true).ToArray();
                    var _startrow = 10;
                    var startrow = 10;
                    var startcol = 1;
                    var count = 0;
                    foreach (var a in record)
                    {
                        count++;
                        ExcelRange cellNo = worksheet.Cells[startrow, startcol];
                        cellNo.Value = count;

                        ExcelRange cellRelateStep = worksheet.Cells[startrow, startcol + 1];
                        cellRelateStep.Value = a.CatRisk?.RelateStep;

                        ExcelRange cellDescription = worksheet.Cells[startrow, startcol + 2];
                        cellDescription.Value = a.CatRisk?.Description;

                        ExcelRange cellType = worksheet.Cells[startrow, startcol + 3];
                        cellType.Value = riskType.FirstOrDefault(x => x.Code == a.CatRisk.RiskType.ToString())?.Name;

                        ExcelRange cellPossibility = worksheet.Cells[startrow, startcol + 4];
                        cellPossibility.Value = a.Potential_Possibility ?? 0;

                        ExcelRange cellInfulenceLevel = worksheet.Cells[startrow, startcol + 5];
                        cellInfulenceLevel.Value = a.Potential_InfulenceLevel ?? 0;

                        ExcelRange cellTotal = worksheet.Cells[startrow, startcol + 6];
                        cellTotal.Value = (a.Potential_Possibility ?? 0) * (a.Potential_InfulenceLevel ?? 0);
                        cellTotal.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        cellTotal.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        var catrisklevel = catrisklevels.FirstOrDefault(x => x.Id == a.Potential_RiskRating);
                        ExcelRange cellRiskLevel = worksheet.Cells[startrow, startcol + 7];
                        cellRiskLevel.Value = catrisklevel?.Name;

                        ExcelRange cellReason = worksheet.Cells[startrow, startcol + 8];
                        cellReason.Value = a.Potential_ReasonRating;

                        var catcontrol = catcontrolTotal.Where(x => x.riskid == a.catrisk_id).ToArray();
                        var mota = "";
                        var tansuat = "";
                        var loai = "";
                        var hinhthuc = "";
                        var index = 0;
                        foreach (var item in catcontrol)
                        {
                            index++;
                            mota += index + ". " + item.CatControl?.Description + Environment.NewLine;
                            var _tansuat = "";
                            var _loai = "";
                            var _hinhthuc = "";
                            var controlfrequency = item.CatControl?.Controlfrequency ?? 0;
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
                            var controltype = item.CatControl?.Controltype ?? 0;
                            switch (controltype)
                            {

                                case 1:
                                    _loai = "Phòng ngừa";
                                    break;
                                case 2:
                                    _loai = "Phát hiện";
                                    break;

                            }
                            var controlformat = item.CatControl?.Controlformat ?? 0;
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
                            tansuat += index + ". " + _tansuat + Environment.NewLine;
                            loai += index + ". " + _loai + Environment.NewLine;
                            hinhthuc += index + ". " + _hinhthuc + Environment.NewLine;
                        }

                        ExcelRange cellDescriptionControl = worksheet.Cells[startrow, startcol + 9];
                        cellDescriptionControl.Value = mota;
                        ExcelRange cellcontrolfrequency = worksheet.Cells[startrow, startcol + 10];
                        cellcontrolfrequency.Value = tansuat;
                        ExcelRange cellcontroltype = worksheet.Cells[startrow, startcol + 11];
                        cellcontroltype.Value = loai;
                        ExcelRange cellcontrolformat = worksheet.Cells[startrow, startcol + 12];
                        cellcontrolformat.Value = hinhthuc;
                        cellcontrolformat.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        cellcontrolformat.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        var indexx = 0;
                        var motathutuc = "";
                        var RiskScoringProcedures = RiskProcedures.Where(x => x.risk_scoring_id == a.Id).ToArray();
                        foreach (var x in RiskScoringProcedures)
                        {
                            indexx++;
                            if (!string.IsNullOrEmpty(x.CatAuditProcedures?.Description))
                            {
                                motathutuc += indexx + ". " + x.CatAuditProcedures?.Description + Environment.NewLine;
                            }
                        }

                        ExcelRange cellCatAuditProcedures = worksheet.Cells[startrow, startcol + 13];
                        cellCatAuditProcedures.Value = motathutuc;
                        ExcelRange cellPersonInCharge = worksheet.Cells[startrow, startcol + 14];
                        var person_in_charge = string.Join(", ", workingpaper.Where(x => x.risk_str.Contains(a.catrisk_id.ToString())).Select(x => x.code).Distinct());
                        cellPersonInCharge.Value = person_in_charge;
                        startrow++;
                    }
                    using ExcelRange r = worksheet.Cells[_startrow, startcol, _startrow == startrow ? startrow : startrow - 1, startcol + 14];
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
                return File(Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kitano_ChuongTrinhKiemToan.xlsx");
                //return Ok(new { code = "1", data = Bytes, file_name = "Ke_hoach_kiem_toan_nam.xlsx",   msg = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("SearchFromHome")]
        public IActionResult SearchFromHome(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AuditProgramModelSearch>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                //var year = !string.IsNullOrEmpty(obj.Year) ? Convert.ToInt32(obj.Year) : -1;
                var AuditProcess = obj.AuditProcess.HasValue ? obj.AuditProcess : -1;
                var AuditWork = obj.AuditWork.HasValue ? obj.AuditWork : -1;
                var Facility = obj.Facility.HasValue ? obj.Facility : -1;
                var Activity = obj.Activity.HasValue ? obj.Activity : -1;
                var status = obj.Status;
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_APRG" && (string.IsNullOrEmpty(status) || a.StatusCode == status)).ToArray();
                var list_appoval_id = approval_status.Select(a => a.item_id).ToList();
                var auditprogram = _uow.Repository<AuditProgram>().Include(a => a.AuditWork).Where(a => (AuditWork == -1 || a.auditwork_id == AuditWork) && (!obj.Year.HasValue || a.Year == obj.Year) && (AuditProcess == -1 || a.auditprocess_id == AuditProcess) && (Facility == -1 || a.auditfacilities_id == Facility) && (Activity == -1 || a.bussinessactivities_id == Activity) && (string.IsNullOrEmpty(status) || list_appoval_id.Contains(a.Id) || (status == "1.0" && !list_appoval_id.Contains(a.Id))) && a.IsDeleted != true).OrderByDescending(a => a.CreatedAt);
                IEnumerable<AuditProgram> data = auditprogram;
                var count = data.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var result = data.Select(a => new AuditProgramListModel()
                {
                    Id = a.Id,
                    Year = a.Year + "",
                    AuditWork = a.AuditWork?.Name,
                    AuditWorkID = a.AuditWork?.Id,
                    AuditActivityID = a.bussinessactivities_id,
                    AuditActivity = a.bussinessactivities_name,
                    AuditProcess = a.auditprocess_name,
                    AuditProcessID = a.auditprocess_id,
                    AuditFacility = a.auditfacilities_name,
                    AuditFacilityID = a.auditfacilities_id,
                    Status = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.StatusCode ?? "1.0",
                    ApprovalUser = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.approver,
                    ApprovalUserLast = approval_status.FirstOrDefault(x => x.item_id == a.Id)?.approver_last
                });
                //var r
                return Ok(new { code = "1", msg = "success", data = result, total = count });

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}