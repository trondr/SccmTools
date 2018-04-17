using System;
using System.IO;
using LanguageExt;

namespace SccmTools.Library.Module
{
    public class ExistingDirectoryPath
    {
        public string Value { get; }

        private ExistingDirectoryPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            if(!Directory.Exists(path)) throw new DirectoryNotFoundException($"Directory path not found: '{path}'.");
            Value = path;
        }

        public static Result<ExistingDirectoryPath> Get(Result<DirectoryPath> path)
        {
            return path.Match(directoryPath => Get(directoryPath),
                exception => new Result<ExistingDirectoryPath>(exception));
        }

        public static Result<ExistingDirectoryPath> Get(DirectoryPath path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            try
            {
                return Directory.Exists(path.Value) ?
                    new Result<ExistingDirectoryPath>(new ExistingDirectoryPath(path.Value)) :
                    new Result<ExistingDirectoryPath>(new DirectoryNotFoundException($"Directory path not found: '{path.Value}'."));
            }
            catch (Exception e)
            {
                return new Result<ExistingDirectoryPath>(e);
            }
        }

        public static Result<ExistingDirectoryPath> Get(string path)
        {
            var directoryPath = DirectoryPath.Get(path);
            return Get(directoryPath);            
        }

    }
}