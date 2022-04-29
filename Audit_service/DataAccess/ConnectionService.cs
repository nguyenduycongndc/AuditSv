using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Audit_service.DataAccess
{
    public class ConnectionService
    {
        public static string connstring = "";
        public static string Set(IConfiguration config)
        {
            return connstring = config["ConnectionStrings:DefaultConnection"];
        }
    }
}
