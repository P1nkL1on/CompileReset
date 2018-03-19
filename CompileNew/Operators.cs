using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public static class OPERATORS
    {
        public static List<string> names = new List<string>() { "+", "-", "*", "/" };
        public static List<char> bracketOpen = new List<char>() { '(', '{', '[', '\'', '\"' };
        public static List<char> bracketClose = new List<char>() { ')', '}', ']', '\'', '\"' };
        

        public static Stack<char> bracketStack = new Stack<char>();

        public static string nameOfIndex(int typeIndex) { if (typeIndex >=0 && typeIndex < names.Count)return names[typeIndex]; return "?"; }

        public static int indexOfName(string name) { for (int i = 0; i < names.Count; i++) if (names[i] == name) return i; throw new Exception("Finding index for unknown operation \"" + name + "\""); }
    }
}
