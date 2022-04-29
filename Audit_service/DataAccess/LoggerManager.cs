using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Audit_service.DataAccess
{
    public interface ILoggerManager
    {
        void LogInformation(string msg);
        void LogError(string msg);
        void LogError(Exception ex);
        void Warn(string msg);
        void Debug(string msg);
    }
    public class LoggerManager : ILoggerManager
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
        public LoggerManager()
        {
            try
            {
                XmlDocument log4netConfig = new XmlDocument();

                using (var fs = File.OpenRead("log4net.config"))
                {
                    log4netConfig.Load(fs);

                    var repo = LogManager.CreateRepository(
                            Assembly.GetEntryAssembly(),
                            typeof(log4net.Repository.Hierarchy.Hierarchy));

                    XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

                    _logger.Info("Log System Initialized");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error", ex);
            }
        }
        public void LogInformation(string msg)
        {
            _logger.Info(msg);
        }
        public void Warn(string msg)
        {
            _logger.Warn(msg);
        }
        public void Debug(string msg)
        {
            _logger.Debug(msg);
        }
        public void LogError(string msg)
        {
            _logger.Error(msg);
        }
        public void LogError(Exception ex)
        {
            _logger.Error("-----------------------------------Error-----------------------------------");
            _logger.Error(JsonSerializer.Serialize(ex));
        }
    }
}
