using System;
using System.IO;
using Microsoft.Win32;
using SccmTools.Library.Module.Common.IO;
using SccmTools.Library.Module.Services;

namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public class PackageDefinitionFileProvider : IPackageDefinitionFileProvider
    {
        private readonly IPathOperation _pathOperation;

        public PackageDefinitionFileProvider(IPathOperation pathOperation)
        {
            _pathOperation = pathOperation;
        }

        public File GetPackageDefinitionFile(string packageDefinitionFileName)
        {
            var packageDefinitionFile = GetPackageDefinitionFromParameter(packageDefinitionFileName);
            if (!packageDefinitionFile.IsNull())
            {
                packageDefinitionFile = GetPackageDefinitionFileFromUser();
            }
            if (packageDefinitionFile.IsNull())
            {
                throw new SccmToolsException("Package definition file name has not been specified.");
            }

            VerifyThatPackageDefinitionFileExists(packageDefinitionFile);

            var uncPackageDefinitionFile = ConvertToUncPath(packageDefinitionFile);

            return uncPackageDefinitionFile;
        }

        private File ConvertToUncPath(File packageDefinitionFile)
        {
            var uncPath = _pathOperation.GetUncPath(packageDefinitionFile.FileName, false);
            var packageDefinitionUri = new Uri(uncPath);
            if (!packageDefinitionUri.IsUnc)
            {
                throw new SccmToolsException(string.Format("Package definition file path '{0}' is not a UNC path or network drive path that can be resolve to an UNC path.", packageDefinitionFile));
            }
            packageDefinitionFile = new RealFile(uncPath);
            return packageDefinitionFile;
        }

        private void VerifyThatPackageDefinitionFileExists(File packageDefinitionFile)
        {
            if (!System.IO.File.Exists(packageDefinitionFile.FileName))
            {
                throw new FileNotFoundException("Package definition file not found.", packageDefinitionFile.FileName);
            }
        }

        private static File GetPackageDefinitionFromParameter(string packageDefinitionFileName)
        {
            if (!string.IsNullOrWhiteSpace(packageDefinitionFileName))
            {
                var expandedFileName = Environment.ExpandEnvironmentVariables(packageDefinitionFileName);
                var fullFileName = Path.GetFullPath(expandedFileName);
                return new RealFile(fullFileName);
            }
            return new NullFile(string.Empty);
        }

        private static File GetPackageDefinitionFileFromUser()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Package Definition (*.sms)|*.sms",
                Multiselect = false
            };
            var ok = openFileDialog.ShowDialog();
            if(ok == true)
            {
                var packageDefinitionFile = openFileDialog.FileName;
                return new RealFile(packageDefinitionFile);
            }
            else
            {
                return new NullFile(string.Empty);
            }
        }
    }
}