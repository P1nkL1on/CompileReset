using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public class COMMON

    {
        public static string TypeDep(int tabs)
        {
            //while (--tabs >= 0) Console.Write( "  ");
            return "".PadLeft(tabs * 2, ' ');
        }
        public static void TraceOnDep (string S, int tabs)
        {
            TraceOnDep(S, tabs, ConsoleColor.Gray);
        }
        public static void TraceOnDep(string S, int tabs, ConsoleColor clr)
        {
            
            Console.ForegroundColor = clr;
            Console.WriteLine((TypeDep(tabs) + S).PadRight(Console.BufferWidth - 2, ' '));
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void WriteColor(string S, ConsoleColor clr)
        {
            Console.ForegroundColor = clr;
            Console.Write(S);
        }
    }
}
