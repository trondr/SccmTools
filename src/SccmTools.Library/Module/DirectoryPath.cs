using System;
using System.IO;
using LanguageExt;

namespace SccmTools.Library.Module
{
    public class DirectoryPath
    {
        public string Value { get; }

        private DirectoryPath(string path)
        {
            Value = path;
        }
        
        public static Result<DirectoryPath> Get(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return new Result<DirectoryPath>(new ArgumentException("Value cannot be null or whitespace.", nameof(path)));
                var expandedPath = Environment.ExpandEnvironmentVariables(path);
                var fullPath = Path.GetFullPath(expandedPath);
                var directoryInfo = new DirectoryInfo(fullPath);
                return new DirectoryPath(directoryInfo.FullName);
            }
            catch (Exception e)
            {
                return new Result<DirectoryPath>(e);
            }
        }
    }
}