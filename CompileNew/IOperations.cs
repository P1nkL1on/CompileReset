using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public class operation : INode
    {
        protected String opName;
        public virtual void Trace(int depth)
        {
            if (this as binaryOperation != null)
                (this as binaryOperation).Trace(depth);
            if (this as monoOperation != null)
                (this as monoOperation).Trace(depth);
        }
        public static INode Parse(string S)
        {
            return null;
        }
        public virtual void TraceCode()
        {
            // ...
        }
        protected void TraceWithPossibleBrackets(operation op)
        {
            if (op as binaryOperation != null)
            {
                binaryOperation opb = (op as binaryOperation);
                //if (OPERATORS.indexOfName(opb.opName) < OPERATORS.indexOfName(opName))
                //{
                COMMON.WriteColor("(", ConsoleColor.Gray);
                opb.TraceCode();
                COMMON.WriteColor(")", ConsoleColor.Gray);
                return;
                //}
            }
            op.TraceCode();
        }
    }
}
