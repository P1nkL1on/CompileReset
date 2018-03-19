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
    }
    public class monoOperation : operation
    {
        protected operation a;
        public override void Trace(int depth) { COMMON.TraceOnDep(opName, depth); a.Trace(depth + 1); }
    }
    public class binaryOperation : operation
    {
        protected operation a;
        protected operation b;

        public override void Trace(int depth)
        {
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
        public static operation Parse(string S)
        {
            List<string> inner;
            string zero = PARSE.ParseLevels(S, out inner);

            int currentInnerIndex = 0;
            List<operation> innerOperations = new List<operation>(), allOperations = new List<operation>();

            for (int i = 0; i < inner.Count; i++)
                innerOperations.Add(Parse(inner[i]));

            string Format = OPERATORS.MakeASync(zero, out inner);
            if (!anyOperatorsInFormat(Format))
                return new Value(zero);

            List<string> parts = zero.Split(OPERATORS.names.ToArray(), StringSplitOptions.None).ToList();
            for (int i = 0; i < parts.Count; i++)
                if (parts[i] != "@")
                    allOperations.Add(new Value(parts[i]));
                else
                    allOperations.Add(innerOperations[currentInnerIndex++]);

            return new binaryOperation(Format, allOperations);
        }

        static bool anyOperatorsInFormat(string S)
        {
            for (int i = 0; i < OPERATORS.names.Count; i++)
                if (S.IndexOf(OPERATORS.names[i]) > 0)
                    return true;
            return false;
        }

        binaryOperation(string S, List<operation> args)
        {
            for (int i = 0; i < OPERATORS.names.Count; i++)
            {
                string[] splited = S.Split(new string[] { OPERATORS.names[i] }, 2, StringSplitOptions.None);
                if (splited.Length == 2)
                {
                    opName = OPERATORS.names[i];
                    type = i;
                    if (anyOperatorsInFormat(splited[0]))
                        a = new binaryOperation(splited[0], args);
                    else
                        a = args[int.Parse(splited[0])];

                    if (anyOperatorsInFormat(splited[1]))
                        b = new binaryOperation(splited[1], args);
                    else
                        b = args[int.Parse(splited[1])];
                    return;
                }
            }
        }
    }
}
