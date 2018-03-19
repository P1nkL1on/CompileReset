using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public static class TYPES{
        static List<string> names = new List<string>() { "void", "int", "float", "double", "char", "bool" };
        public static string nameOfIndex (int typeIndex) { return names[typeIndex]; }
    }


    public struct TypeFull
    {
        int type; // int float char
        int pointerLevel;   // *int, ***char, *double
        public TypeFull (int type, int pointerLevel)
        {
            if (pointerLevel < 0)
                throw new Exception("POINTER should be 0 or more");
            this.type = type;
            this.pointerLevel = pointerLevel;
        }
        public TypeFull(int type)
        {
            this.type = type;
            this.pointerLevel = 0;
        }
        public override string ToString()
        {
            return TYPES.nameOfIndex(type) + "".PadLeft(pointerLevel, '*');
        }
    }

    class Value : operation
    {
        public Object date;
        TypeFull type;

        public Value(Object date, TypeFull type)
        {
            this.date = date;
            this.type = type;
        }
        public override void Trace(int depth)
        {
            COMMON.TraceOnDep(this.ToString(), depth, ConsoleColor.Yellow);
        }
        public override string ToString()
        {
            return type.ToString() + " " + date.ToString();
        }
        public virtual bool IsStatic()
        {
            return true;
        }
    }
    
}
