using System.Collections.Generic;

namespace SccmTools.Library.Module.Services
{
    public interface IProductCodeProvider2
    {
        string GetProductCodeFromText(string text);

        IEnumerable<string> GetProductCodesFromMsiFileSearch(string searchFolder);
    }
}