using System;
using System.Threading;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public interface IPackageDefinitionProvider
    {
        PackageDefinition2 ReadPackageDefinition(string packageDefinitionFileName);

        void WritePackageDefinition(string packageDefinitionFileName, PackageDefinition2 packageDefinition);
    }
}