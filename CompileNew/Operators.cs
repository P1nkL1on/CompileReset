using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public static class OPERATORS
    {
        public static List<string> names = new List<string>() { "=", "||", "|", "or", "&&", "&", "and", "==", "!=", ">", "<", ">=", "<=", "<<", ">>", "+", "-", "^", "*", "/", "%" };
        public static List<char> bracketOpen = new List<char>() { '(', '{', '[', '\'', '\"' };
        public static List<char> bracketClose = new List<char>() { ')', '}', ']', '\'', '\"' };


        public static Stack<char> bracketStack = new Stack<char>();

        public static string nameOfIndex(int typeIndex) { if (typeIndex >= 0 && typeIndex < names.Count)return names[typeIndex]; return "?"; }

        public static int indexOfName(string name) { for (int i = 0; i < names.Count; i++) if (names[i] == name) return i; throw new Exception("Finding index for unknown operation \"" + name + "\""); }

        public static string MakeASync(string S, out List<string> splitedS)
        {
            int nowIn = 0, betInd = 0;
            string currentOp = "", res = "";
            bool bet = false;

            while (nowIn < S.Length)
            {
                char newC = S[nowIn];
                bool anyMatch = false;

                for (int i = 0; i < names.Count; i++)
                    if (!anyMatch && names[i].IndexOf(currentOp + newC) == 0)
                    {
                        if (currentOp != newC + "")
                            currentOp += newC;
                        anyMatch = true;
                    }
                if (!anyMatch)
                {
                    int ind = names.IndexOf(currentOp);
                    if (ind >= 0)
                    {
                        res += currentOp;
                        bet = false;
                    }
                    currentOp = "";
                }
                if (!bet)
                {
                    res += betInd;
                    bet = true; betInd++;
                }
                nowIn++;
            }
            splitedS = S.Split(names.ToArray(), StringSplitOptions.None).ToList();
            return res;
        }
    }
}
