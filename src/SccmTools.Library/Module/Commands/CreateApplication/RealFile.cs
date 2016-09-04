namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public class RealFile : File
    {
        public override bool IsNull()
        {
            return false;
        }

        public RealFile(string fileName) : base(fileName)
        {
        }
    }
}