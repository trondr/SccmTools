using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace SccmTools.Library.Infrastructure
{
    public class LoggingConfiguration : ILoggingConfiguration
    {
        private string _logDirectoryPath;
        private string _logFileName;
        private readonly string _sectionName;

        public LoggingConfiguration()
        {            
            _sectionName = "SccmTools";
        }

        public string LogDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(_logDirectoryPath))
                {
                    var section = (NameValueCollection)ConfigurationManager.GetSection(_sectionName);
                    if (section == null)
                    {
                        throw new ConfigurationErrorsException("Missing section in application configuration file: " + _sectionName);
                    }
                    _logDirectoryPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(section["LogDirectoryPath"]));
                }
                return _logDirectoryPath;
            }
            set { _logDirectoryPath = value; }
        }

        public string LogFileName
        {
            get
            {
                if (string.IsNullOrEmpty(_logFileName))
                {
                    var section = (NameValueCollection)ConfigurationManager.GetSection(_sectionName);
                    if (section == null)
                    {
                        throw new ConfigurationErrorsException("Missing section in application configuration file: " + _sectionName);
                    }
                    _logFileName = Environment.ExpandEnvironmentVariables(section["LogFileName"]);
                }
                return _logFileName;
            }
            set { _logFileName = value; }
        }
    }
}