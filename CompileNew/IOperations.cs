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
        public virtual void Trace(int depth) { /* ... */ }
        
        public static INode Parse (string S)
        {
            return null;
        }
    }
    public class monoOperation : operation
    {
        protected operation a;
        public virtual void Trace(int depth) { COMMON.TraceOnDep(opName, depth); a.Trace(depth + 1); }
    }
    public class binaryOperation : monoOperation
    {
        protected operation b;
        public virtual void Trace(int depth) {
            COMMON.TraceOnDep(opName, depth);
            a.Trace(depth + 1);
            b.Trace(depth + 1);
        }

        int type;
        public binaryOperation(int type, operation a, operation b)
        {
            this.a = a;
            this.b = b;
            this.type = type;
            opName = OPERATORS.nameOfIndex(type);
        }
        public binaryOperation (string S)
        {
            List<string> inner;
            string zero = PARSE.ParseLevels(S, out inner);

            List<string> parts = zero.Split(OPERATORS.names.ToArray(), StringSplitOptions.None).ToList();
            int X = 0;
        }
    }


}
