using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    class Program
    {
        static void Main(string[] args)
        {

            //Value v1 = new Value((object)4, new TypeFull(1, 0)),
            //      v2 = new Value((object)5, new TypeFull(1,0));
            //{ "=" , "==", "!=", ">", "<", ">=", "<=", "||", "|", "or","&&", "&", "and",  "^", "<<", ">>", "+", "-", "*", "/", "%"};
            while (true)
            {
                try
                {
                    Console.Clear(); Console.Write("Type a operation: ");
                    operation bo = binaryOperation.Parse(Console.ReadLine());//(" *(2 + 7)+ 10 & 1237680 + &(3) + 6*6 - !823187 + 7++  ");//(" 2 +  \"     a     d   3 4   \"  +  \"   ___ ...____ ..\" -\'a\'  *   \'b\' + 10 -(2 -    11+2+3+4)");
                    Console.BackgroundColor = ConsoleColor.White;
                    bo.Trace(1);
                }
                catch (Exception e)
                {
                    COMMON.TraceOnDep(e.Message, 0, ConsoleColor.Red);
                }
                finally
                {
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }
            Console.ReadLine();
        }
    }
}
