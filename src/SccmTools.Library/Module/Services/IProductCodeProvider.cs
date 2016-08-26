namespace SccmTools.Library.Module.Services
{
    public interface IProductCodeProvider
    {
        string GetProductCodeFromText(string text);

        string GetProductCodeFromMsiFileSearch(string searchFolder);
    }
}
