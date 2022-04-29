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

namespace Audit_service.Controllers.Category
{
    [Route("[controller]")]
    [ApiController]
    public class RiskControlController : BaseController
    {
        public RiskControlController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }

        [HttpGet("GetControl/{id}")]
        public IActionResult SearchControl(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                Expression<Func<RiskControl, bool>> _filter = c => (id == 0 || c.riskid == id) && (c.isdeleted != true);
                var _list_risk = _uow.Repository<RiskControl>().Include(a => a.CatControl).Where(_filter).OrderByDescending(a => a.Id).Select(a => new CatControlSearchModel()
                {
                    Riskcontrolid = a.Id,
                    Id = a.CatControl.Id,
                    Name = a.CatControl.Name,
                    Description = a.CatControl.Description,
                    Code = a.CatControl.Code,
                    Controlfrequency = a.CatControl.Controlfrequency,
                    Controltype = a.CatControl.Controltype,
                    Controlformat = a.CatControl.Controlformat
                });
                return Ok(new { code = "1", msg = "success", data = _list_risk });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpGet("GetRisk/{id}")]
        public IActionResult SearchRisk(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                Expression<Func<RiskControl, bool>> _filter = c => (id == 0 || c.controlid == id) && (c.isdeleted != true);
                var _list_control = _uow.Repository<RiskControl>().Include(a => a.CatRisk).Where(_filter).OrderByDescending(a => a.Id).Select(a => new CatRiskSearchModel()
                {
                    Riskcontrolid = a.Id,
                    Id = a.CatRisk.Id,
                    name = a.CatRisk.Name,
                    Description = a.CatRisk.Description,
                    code = a.CatRisk.Code

                });

                return Ok(new { code = "1", msg = "success", data = _list_control });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] RiskControlModel catcontrolinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _allCatControl = _uow.Repository<RiskControl>().Find(a => a.controlid == catcontrolinfo.ControlId && a.riskid == catcontrolinfo.RiskId).ToArray();
                if (_allCatControl.Length > 0)
                {
                    return Ok(new { code = "1", msg = "success" });
                }
                var _RiskControl = new RiskControl();
                _RiskControl.controlid = catcontrolinfo.ControlId;
                _RiskControl.riskid = catcontrolinfo.RiskId;

                _uow.Repository<RiskControl>().Add(_RiskControl);
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
                var _risk = _uow.Repository<RiskControl>().FirstOrDefault(a => a.Id == id);
                _risk.flag = 1;
                _uow.Repository<RiskControl>().Update(_risk);
                return Ok(new { code = "1", msg = "success" });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
