using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using SccmTools.Library.Module.Common.Msi;

namespace SccmTools.Library.Module.Services
{
    public class ProductCodeProvider2 : IProductCodeProvider2
    {
        private readonly IProductCodeProvider _productCodeProvider;
        private readonly IMsiHelper _msiHelper;

        public ProductCodeProvider2(IProductCodeProvider productCodeProvider, IMsiHelper msiHelper)
        {
            _productCodeProvider = productCodeProvider;
            _msiHelper = msiHelper;
        }

        public string GetProductCodeFromText(string text)
        {
            return _productCodeProvider.GetProductCodeFromText(text);
        }

        public IEnumerable<string> GetProductCodesFromMsiFileSearch(string searchFolder)
        {
            if (searchFolder == null) throw new ArgumentNullException("searchFolder");
            if (!Directory.Exists(searchFolder)) throw new DirectoryNotFoundException("Folder to search for msi file does not exist: " + searchFolder);
            var msiFiles = Directory.GetFiles(searchFolder,"*.msi", SearchOption.AllDirectories);
            foreach (var msiFile in msiFiles)
            {
                using (var database = new Database(msiFile, DatabaseOpenMode.ReadOnly))
                {
                    var productCode = _msiHelper.GetProperty(database, "ProductCode");
                    yield return productCode;
                }
            }
        }
    }
}