using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public class COMMON

    {
        public static void TypeDep(int tabs)
        {
            while (--tabs >= 0) Console.Write( "  ");
        }
        public static void TraceOnDep (string S, int tabs)
        {
            TraceOnDep(S, tabs, ConsoleColor.Gray);
        }
        public static void TraceOnDep(string S, int tabs, ConsoleColor clr)
        {
            TypeDep(tabs);
            Console.ForegroundColor = clr;
            Console.WriteLine(S);
            Console.ResetColor();
        }
    }
}
