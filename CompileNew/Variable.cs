using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    class Variable : Value
    {
        int placeTag;   // local global param
        string varName;

        public Variable(TypeFull type, string varName, int placeTag) : base(null, type)
        {
            this.varName = varName;
            this.placeTag = placeTag;
        }
    }
}
