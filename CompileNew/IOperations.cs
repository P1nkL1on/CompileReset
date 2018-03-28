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
        public bool isPrefix;
        protected operation a;
        public override void Trace(int depth)
        {
            COMMON.TraceOnDep((isPrefix)? opName+"_" : "_"+opName, depth, ConsoleColor.DarkGreen);
            if (!isEmpty) a.Trace(depth + 1);
        }

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
            S = S.Trim();
            if (S.Length == 0)
                return new monoOperation();
            List<string> inner;
            string zero = PARSE.ParseLevels(S, out inner);
            int preOp = FoundPreMonoOpIndexInFormat(zero),
                postOp = FoundPostMonoOpIndexInFormat(zero),
                nonZeroStartIndex = 0;
            while (S[nonZeroStartIndex] == ' ')
                nonZeroStartIndex++;
            // check for first non ' ' symbol
            if ((preOp < 0 && postOp < 0))
                return new Value(S);
            if (inner.Count == 0 || inner.Count > 1)
            {
                int opPlace = -1;
                if (postOp >= 0 && zero.LastIndexOf(OPERATORS.monoPostNames[postOp]) > nonZeroStartIndex)                // prefix operation
                {
                    opPlace = zero.IndexOf(OPERATORS.monoPostNames[postOp]);
                    inner.Add(zero.Remove(opPlace));
                }
                else               // postfix operation
                {
                    opPlace = zero.IndexOf(OPERATORS.monoPreNames[preOp]);
                    inner.Add(zero.Substring(opPlace + OPERATORS.monoPreNames[preOp].Length) );
                }
            }
            if (inner.Count > 1)
            {
                //throw new Exception("Strange unar operation! \"" + S + "\"");
                string res = inner[inner.Count - 1], inner0 = "";
                for (int i = 0, curInd = 0; i < res.Length; i++)
                    if (res[i] == '@')
                        inner0 += inner[curInd++];
                    else
                        inner0 += res[i];
                inner = new List<string>();
                inner.Add(inner0);
            }
            operation inOp = binaryOperation.Parse(inner[0]);
            if (postOp >= 0 && zero.LastIndexOf(OPERATORS.monoPostNames[postOp]) > nonZeroStartIndex)
                return new monoOperation(false, postOp, inOp);
            if (preOp >= 0)
                return new monoOperation(true, preOp, inOp);
            throw new Exception("Can not parse a operation \"" + S + "\" with format mask \"" + zero + "\"");
        }
        public monoOperation(bool prefix, int operatorIndex, operation inner)
        {
            isPrefix = prefix;
            a = inner;
            opName = (prefix) ? OPERATORS.monoPreNames[operatorIndex] : OPERATORS.monoPostNames[operatorIndex];
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
            {
                if (Format.IndexOf(OPERATORS.monoPostNames[i]) >= 0)
                    return i;
                if (Format.IndexOf(OPERATORS.monoPostNames[i]) >= 0 && Format.IndexOf(OPERATORS.monoPostNames[i]) == Format.Length - OPERATORS.monoPostNames[i].Length)
                    if (maxLength < OPERATORS.monoPostNames[i].Length)
                    { foundIndex = i; maxLength = OPERATORS.monoPostNames[foundIndex].Length; }
            }
            return foundIndex;
        }
    }
    public class binaryOperation : operation
    {
        protected operation a;
        protected operation b;

        public override void Trace(int depth)
        {
            COMMON.TraceOnDep(opName, depth, ConsoleColor.DarkGray);
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
            Console.WriteLine("Trying to parse: \"" + S + "\"\n");
            List<string> inner, innerMono;

            string zero = PARSE.ParseLevels(S, out inner);
            zero = PARSE.ParseMono(zero, out innerMono, ref inner);
            COMMON.TraceOnDep(zero, 1, ConsoleColor.Red);

            if (zero.Trim(' ') == "@")
                return monoOperation.Parse(inner[0]);
            if (zero.Trim(' ') == "#")
                return monoOperation.Parse(innerMono[0]);

            List<string> formatInner = new List<string>();
            string Format = OPERATORS.MakeASync(zero, out formatInner);

            int currentInnerIndex = 0, currentMonoIndex = 0;
            List<operation> innerOperations = new List<operation>(), allOperations = new List<operation>();

            Console.WriteLine("Mono:");
            for (int i = 0; i < innerMono.Count; i++)
            {
                COMMON.TraceOnDep("# " + innerMono[i], 3, ConsoleColor.Red);
                innerOperations.Add(monoOperation.Parse(innerMono[i]));
            }
            Console.WriteLine("Inner:");
            for (int i = 0; i < inner.Count; i++)
            {
                COMMON.TraceOnDep("@ " + inner[i], 3, ConsoleColor.Red);
                innerOperations.Add(Parse(inner[i]));
            }
            ///!!!
            //
            if (zero != "#" && zero != "@")
                if (!anyOperatorsInFormat(Format))
                    return monoOperation.Parse(zero);

            List<string> parts = zero.Split(OPERATORS.names.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < parts.Count; i++)
            {
                // trimming problems
                parts[i] = parts[i].Trim(' ');
                if (parts[i] != "@" && parts[i] != "#")
                    allOperations.Add(monoOperation.Parse(parts[i]));
                else
                    allOperations.Add(innerOperations[currentInnerIndex++]);
            }
            if (allOperations.Count > 1)
                return new binaryOperation(Format, allOperations);
            else
                ///@!!!!!!!!!!!!
                return allOperations[0];
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
            //args.Add(new monoOperation());
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
            throw new Exception("Invalid format of binary operation: \"" + S + "\"");

        }
    }
}
