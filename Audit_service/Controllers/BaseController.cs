using Audit_service.Attributes;
using Audit_service.DataAccess;
using Audit_service.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Audit_service.Controllers
{

    [BaseAuthorize]
    public class BaseController : ControllerBase
    {
        protected readonly ILoggerManager _logger;
        protected readonly IUnitOfWork _uow;
        protected readonly IMapper _mapper;

        protected string[] contentType = new string[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel" };
        protected string[] formats = new string[] { ".xlsx", ".xls" };
        
        public BaseController(ILoggerManager logger,IUnitOfWork uow) : base()
        {
            _uow = uow;
            _logger = logger;
        }
    }
}
