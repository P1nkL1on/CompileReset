using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public static class TYPES
    {
        public static List<string> names = new List<string>() { "void", "int", "char", "float", "double", "bool" };
        static List<string> variableType = new List<string>() { "local", "global", "param" };
        public static string nameOfIndex(int typeIndex) { return names[typeIndex]; }
    }
    public struct TypeFull
    {
        int type; // int float char
        int pointerLevel;   // *int, ***char, *double
        public TypeFull(int type, int pointerLevel)
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
        public int getType()
        {
            return type;
        }
        public int getPointerLevel()
        {
            return pointerLevel;
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
        public Value(string S)
        {
            try
            {
                if (S.IndexOf('\'') == 0 && S.LastIndexOf('\'') == S.Length - 1)
                {
                    string inBrackets = S.Substring(1, S.Length - 2);

                    if (inBrackets.Length == 1)
                        this.date = (object)((int)(inBrackets[0]));
                    else
                        this.date = (object)(inBrackets);
                    this.type = new TypeFull(2);    // char
                    Console.WriteLine("Created value token " + this.ToString());
                    return;
                }
                if (S.IndexOf('\"') == 0 && S.LastIndexOf('\"') == S.Length - 1)
                {
                    string inBrackets = S.Substring(1, S.Length - 2);
                    this.date = (object)(inBrackets);
                    this.type = new TypeFull(2, 1);    // char
                    Console.WriteLine("Created value token " + this.ToString());
                    return;
                }

                // number section
                if (S.IndexOf('f') == S.Length - 1)
                {
                    this.date = (object)float.Parse(S.Substring(0, S.Length - 1));
                    this.type = new TypeFull(3);    // float
                    Console.WriteLine("Created value token " + this.ToString());
                    return;
                }

                if (S.IndexOf('.') >= 0)
                {
                    this.date = (object)(double.Parse(S.Replace('.', ',')));
                    this.type = new TypeFull(4);    // float
                    Console.WriteLine("Created value token " + this.ToString());
                    return;
                }

                this.date = (object)int.Parse(S);
                this.type = new TypeFull(1);    // int
                Console.WriteLine("Created value token " + this.ToString());
                return;
            }
            catch (Exception e)
            {
                throw new Exception("Can not resolve value \"" + S + "\"");
            }
        }
        public override void TraceCode()
        {
            ConsoleColor clr = ConsoleColor.Red;
            switch (type.getType())
            {
                case 1:
                case 5:
                    clr = ConsoleColor.Magenta;
                    break;
                case 2:
                    clr = ConsoleColor.DarkMagenta;
                    break;
                case 3:
                case 4:
                    clr = ConsoleColor.DarkYellow;
                    break;
                default:
                    break;
            }
            if (type.getType() == 2)
                COMMON.WriteColor("'" + (char)((int)date) + "'", clr);
            else
                COMMON.WriteColor(date.ToString(), clr);
        }
        public override void Trace(int depth)
        {
            COMMON.TraceOnDep(this.ToString(), depth, ConsoleColor.Magenta);
        }
        public override string ToString()
        {

            string dateString = date.ToString();
            if (type.getType() == 2 && type.getPointerLevel() == 1) dateString = '\"' + dateString + '\"';//'
            return type.ToString() + " " + dateString;
        }
        public virtual bool IsStatic()
        {
            return true;
        }

    }

}
