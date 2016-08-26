namespace SccmTools.Library.Common.IO
{
    public interface IPathOperation
    {
        string GetUncPath(string path, bool useAdminShareForLocalDrive);

        bool IsUncPath(string path);
    }
}
