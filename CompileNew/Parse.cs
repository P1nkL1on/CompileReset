using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public static class PARSE
    {
        public static void SeparateLineOnIndex(string S, int index, out string left, out string right)
        {
            int i = 0;
            string l = "", r = "";
            while (i < S.Length)
            {
                if (i < index)
                    l += S[i];
                if (i > index)
                    r += S[i];
                i++;
            }
            left = l;
            right = r;
        }

        public static int CheckBracket(char symbol)
        {
            int brTypeO = OPERATORS.bracketOpen.IndexOf(symbol),
                brTypeC = OPERATORS.bracketClose.IndexOf(symbol);
            if (brTypeO >= 0 || brTypeC >= 0)
            {
                if (brTypeO >= 0 && brTypeC < 0)    // ( [ {
                    OPERATORS.bracketStack.Push(symbol);
                if (brTypeC >= 0 && brTypeO < 0)
                {    // ) ] }
                    if (OPERATORS.bracketOpen[brTypeC] == OPERATORS.bracketStack.First())
                        OPERATORS.bracketStack.Pop();
                    else
                        throw new Exception("Bracket error!");
                }
                //Console.Write(symbol);
            }

            return OPERATORS.bracketStack.Count;
        }

        public static string ParseLevels(string S, out List<string> innerParse)
        {
            int nowIn = 0;
            string noBracketsParse = "", currentInnerParse = "";
            List<string> inBracketsPrase = new List<string>();


            while (nowIn < S.Length)
            {
                int empty = PARSE.CheckBracket(S[nowIn]);
                if (empty == 0)
                {
                    if (currentInnerParse.Length > 0)   // put a inner bracket to a list for far going parsing
                    {
                        inBracketsPrase.Add(currentInnerParse + S[nowIn]);
                        currentInnerParse = "";
                    }
                    // add to a main zero level parsing
                    char addC = ((OPERATORS.bracketClose.IndexOf(S[nowIn]) < 0) ? S[nowIn] : '@');
                    if (addC != ' ')
                        noBracketsParse += addC;
                }
                else
                    // adding to current non zero level
                    currentInnerParse += S[nowIn];// ((OPERATORS.bracketOpen.IndexOf(S[nowIn]) < 0) ? S[nowIn] + "" : "");
                nowIn++;
            }
            Console.WriteLine(": " + noBracketsParse);
            for (int i = 0; i < inBracketsPrase.Count; i++)
            {
                inBracketsPrase[i] = inBracketsPrase[i].Substring(1, inBracketsPrase[i].Length - 2);
                Console.WriteLine("    \"" + inBracketsPrase[i]+"\"");
            }

            innerParse = inBracketsPrase;

            while
            (noBracketsParse == "@")
            {
                string innerP = innerParse[0]; innerParse = new List<string>();
                noBracketsParse = ParseLevels(innerP, out innerParse);
            }
            return noBracketsParse;
        }
    }
}
