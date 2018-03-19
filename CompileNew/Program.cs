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
            binaryOperation bo = new binaryOperation("1+(2+3)*(4+5)/(12+11)-(6*(16-13)*(2-12/10)*3*(2*(3*(1+((0))))))"); //(OPERATORS.indexOfName("+"), v1, v2);
            //
            //bo.Trace(1);
            Console.ReadLine();
        }
    }
}
