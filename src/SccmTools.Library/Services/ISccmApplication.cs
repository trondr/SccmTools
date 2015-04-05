using System.Security.Cryptography.X509Certificates;

namespace SccmTools.Library.Services
{
    public interface ISccmApplication
    {
        void Save(string pakageDefinitionXml);
    }
}
