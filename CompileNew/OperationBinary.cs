using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public class binaryOperation : operation
    {
        protected operation a;
        protected operation b;

        public override void TraceCode()
        {
            TraceWithPossibleBrackets(a);//a.TraceCode();
            COMMON.WriteColor(" " + opName + " ", ConsoleColor.DarkGray);
            TraceWithPossibleBrackets(b);//b.TraceCode();
        }
        
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
