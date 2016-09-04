namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public abstract class File
    {
        protected File(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }

        public abstract bool IsNull();

    }
}