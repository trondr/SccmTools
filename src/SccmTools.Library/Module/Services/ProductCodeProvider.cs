using System;
using System.IO;
using System.Text.RegularExpressions;
using Common.Logging;
using Microsoft.Deployment.WindowsInstaller;
using SccmTools.Library.Module.Common.Msi;

namespace SccmTools.Library.Module.Services
{
    public class ProductCodeProvider : IProductCodeProvider
    {
        private readonly IMsiHelper _msiHelper;
        private readonly ILog _logger;

        public ProductCodeProvider(IMsiHelper msiHelper, ILog logger)
        {
            _msiHelper = msiHelper;
            _logger = logger;
        }

        private readonly Regex _guidRegex = new Regex(@"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b");

        public string GetProductCodeFromText(string text)
        {
            var match = _guidRegex.Match(text);
            if(match.Success)
            {
                return string.Format("{{{0}}}", match.Value);
            }
            return null;
        }

        public string GetProductCodeFromMsiFileSearch(string searchFolder)
        {
            if (searchFolder == null) throw new ArgumentNullException("searchFolder");
            if (!Directory.Exists(searchFolder)) throw new DirectoryNotFoundException("Folder to search for msi file does not exist: " + searchFolder);
            var msiFiles = Directory.GetFiles(searchFolder,"*.msi", SearchOption.AllDirectories);
            if(msiFiles.Length == 1)
            {
                using (var database = new Database(msiFiles[0], DatabaseOpenMode.ReadOnly))
                {
                    return _msiHelper.GetProperty(database,"ProductCode");
                }
            }
            if(msiFiles.Length > 1)
            {
                _logger.Warn("More than one msi file found. Unable to automatically decide product code for use in application detection method.");
                return null;
            }
            return null;
        }
    }
}