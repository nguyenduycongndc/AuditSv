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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Audit_service.Controllers.Audit
{
    [Route("[controller]")]
    [ApiController]
    public class ControlAssessmentController : BaseController
    {
        public ControlAssessmentController(ILoggerManager logger, IUnitOfWork uow) : base(logger, uow)
        {
        }

        [HttpPost]
        public IActionResult Create([FromBody] ControlAssessmentModel ControlAssessmentinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                //var _allControlAssessment = _uow.Repository<ControlAssessment>().Find(a => a.isdelete != true && a.code.Equals(ControlAssessmentinfo.code)).ToArray();
                //if (_allControlAssessment.Length > 0)
                //{
                //    return BadRequest();
                //}
                var ControlAssessmentCreate = new ControlAssessment();

                ControlAssessmentCreate.controlid = Convert.ToInt32(ControlAssessmentinfo.controlid);
                ControlAssessmentCreate.designassessment = ControlAssessmentinfo.designassessment;
                ControlAssessmentCreate.designconclusion = ControlAssessmentinfo.designconclusion;
                ControlAssessmentCreate.effectiveassessment = ControlAssessmentinfo.effectiveassessment;
                ControlAssessmentCreate.effectiveconclusion = ControlAssessmentinfo.effectiveconclusion;
                
                _uow.Repository<ControlAssessment>().Add(ControlAssessmentCreate);

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}
