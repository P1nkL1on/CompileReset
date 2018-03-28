using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public class monoOperation : operation
    {
        public bool isEmpty;
        public bool isPrefix;
        protected operation a;
        public override void TraceCode()
        {
            if (isEmpty)
            {
                COMMON.WriteColor("NULL", ConsoleColor.Red);
                return;
            }
            if (!isPrefix)TraceWithPossibleBrackets(a);
            COMMON.WriteColor(opName, ConsoleColor.DarkGreen);
            if (isPrefix) TraceWithPossibleBrackets(a);
        }
        public override void Trace(int depth)
        {
            COMMON.TraceOnDep((isPrefix) ? opName + "_" : "_" + opName, depth, ConsoleColor.DarkGreen);
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
            {
                operation uniq = UniqOperation.TryParse(S);
                if (uniq != null)
                    return uniq;
                return new Value(S);
            }
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
                    inner.Add(zero.Substring(opPlace + OPERATORS.monoPreNames[preOp].Length));
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

}
