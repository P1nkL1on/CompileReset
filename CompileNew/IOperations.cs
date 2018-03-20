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
        public bool isEmpty;
        protected operation a;
        public override void Trace(int depth) { COMMON.TraceOnDep(opName, depth, ConsoleColor.Green); if (!isEmpty) a.Trace(depth + 1); }

        public monoOperation()
        {
            isEmpty = true;
            opName = "NONE";
        }
        public monoOperation(bool prefix, operation a, string opName)
        {
            isEmpty = false;
            this.opName = opName;
            this.a = a;
        }

        public static operation Parse(string S)
        {
            if (S.Length == 0)
                return new monoOperation();
            //List<string> inner;
            //string zero = PARSE.ParseLevels(S, out inner).Trim(' ');
            //while (zero == "@")
            //    zero = PARSE.ParseLevels(inner[0], out inner).Trim(' ');
            //// lets find any non mono operations

            ////
            //// check for any prefix uno operations
            //int indPre = FoundPreMonoOpIndexInFormat(zero),
            //    indPost = FoundPostMonoOpIndexInFormat(zero);
            return new Value(S);
        }
        

        static int FoundPreMonoOpIndexInFormat(string Format)
        {
            int maxLength = 0, foundIndex = -1;
            for (int i = 0; i < OPERATORS.monoPreNames.Count; i++)
                if (Format.IndexOf(OPERATORS.monoPreNames[i]) == 0)
                    if (maxLength < OPERATORS.monoPreNames[i].Length)
                    { foundIndex = i; maxLength = OPERATORS.monoPreNames[foundIndex].Length; }
            return foundIndex;
        }
        static int FoundPostMonoOpIndexInFormat(string Format)
        {
            int maxLength = 0, foundIndex = -1;
            for (int i = 0; i < OPERATORS.monoPostNames.Count; i++)
                if (Format.IndexOf(OPERATORS.monoPostNames[i]) == Format.Length - OPERATORS.monoPostNames[i].Length)
                    if (maxLength < OPERATORS.monoPostNames[i].Length)
                    { foundIndex = i; maxLength = OPERATORS.monoPostNames[foundIndex].Length; }
            return foundIndex;
        }
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
            if (S == "")
                return monoOperation.Parse(S);
            Console.WriteLine(S + "\n");
            List<string> inner;
            string zero = PARSE.ParseLevels(S, out inner);

            int currentInnerIndex = 0;
            List<operation> innerOperations = new List<operation>(), allOperations = new List<operation>();

            for (int i = 0; i < inner.Count; i++)
            {
                if (inner.Count == 1 && S == inner[0] && S.Length >= 2 && S[0] == S[S.Length - 1] && OPERATORS.bracketOpen.IndexOf(S[0]) >= 3)
                    return monoOperation.Parse(S);
                innerOperations.Add(Parse(inner[i]));
            }

            string Format = OPERATORS.MakeASync(zero, out inner);

            COMMON.TraceOnDep(zero, 5, ConsoleColor.Cyan); COMMON.TraceOnDep(Format, 5, ConsoleColor.DarkCyan);

            if (!anyOperatorsInFormat(Format))
                return monoOperation.Parse(zero);

            List<string> parts = zero.Split(OPERATORS.names.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < parts.Count; i++)
            {
                // trimming problems
                parts[i] = parts[i].Trim(' ');
                if (parts[i] != "@")
                    allOperations.Add(monoOperation.Parse(parts[i]));
                else
                    allOperations.Add(innerOperations[currentInnerIndex++]);
            }

            binaryOperation resOp = new binaryOperation(Format, allOperations);

            // case of monooperation checking of NONE
            if ((resOp.a as monoOperation) != null && (resOp.a as monoOperation).isEmpty)
                return new monoOperation(true, resOp.b, resOp.opName);
            if ((resOp.b as monoOperation) != null && (resOp.b as monoOperation).isEmpty)
                return new monoOperation(true, resOp.a, resOp.opName);
            //
            return resOp;
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
            args.Add(new monoOperation());
            for (int i = 0; i < OPERATORS.names.Count; i++)
            {
                string[] splited = PARSE.SeparateFormatOnOperator(S, i);
                //splited = S.Split(new string[] { OPERATORS.names[i]}, 2, StringSplitOptions.None);

                if (splited.Length == 2)
                {
                    COMMON.TraceOnDep(OPERATORS.names[i] + "   : " + splited[0] + "   |   " + splited[1], 5, ConsoleColor.DarkYellow);

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
