using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SccmTools.Library.Module.Commands.CreateApplication
{
    public class NullFile: File
    {
        public override bool IsNull()
        {
            return true;
        }

        public NullFile(string fileName) : base(fileName)
        {
        }
    }
}
