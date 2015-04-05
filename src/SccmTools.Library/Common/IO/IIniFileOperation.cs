namespace SccmTools.Library.Common.IO
{
    public interface IIniFileOperation
    {
        string Read(string path, string section, string key);

        void Write(string path, string section, string key, string value);
    }
}