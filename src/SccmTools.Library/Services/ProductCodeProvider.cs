using System.Text.RegularExpressions;

namespace SccmTools.Library.Services
{
    public class ProductCodeProvider : IProductCodeProvider
    {

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
    }
}