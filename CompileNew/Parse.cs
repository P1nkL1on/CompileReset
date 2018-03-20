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

        public static string[] SeparateFormatOnOperator(string S, int opIndex)
        {
            string op = OPERATORS.names[opIndex], left = "", right = "";
            int foundIndex = -1; bool noConflicts = true;
            do
            {
                try
                {
                    foundIndex = S.IndexOf(op, foundIndex + op.Length);
                    for (int i = 0; i < OPERATORS.names.Count; i++)
                        if (i != opIndex && OPERATORS.names[i].IndexOf(op) >= 0)
                        {
                            int curOpIndex = (S.IndexOf(OPERATORS.names[i], foundIndex - OPERATORS.names[i].IndexOf(op))),
                                requiredIndex = foundIndex - OPERATORS.names[i].IndexOf(op);
                            if (curOpIndex == requiredIndex)
                                // then conflict with another operator
                                if (noConflicts)
                                    noConflicts = false;
                        }
                }catch(Exception e) { }
            } while (!noConflicts && foundIndex >= 0);
            
            if (!noConflicts)
                foundIndex = -1;

            if (foundIndex < 0)
                return new string[1] { S };
    
            return new string[] { S.Substring(0, foundIndex), S.Substring(foundIndex + op.Length, S.Length - foundIndex - op.Length) };
        }

        public static int CheckBracket(char symbol)
        {
            //if (OPERATORS.bracketStack.Count > 0 && OPERATORS.bracketStack.First() == '\"' && symbol != '\"')
            //    return OPERATORS.bracketStack.Count - 1;
            int brTypeO = OPERATORS.bracketOpen.IndexOf(symbol),
                brTypeC = OPERATORS.bracketClose.IndexOf(symbol);
            if (brTypeO >= 0 || brTypeC >= 0)
            {
                if (brTypeO >= 0 && brTypeC < 0)    // ( [ {
                    OPERATORS.bracketStack.Push(symbol);
                if (brTypeC >= 0 && brTypeO < 0)
                {    // ) ] }
                    if (OPERATORS.bracketOpen[brTypeC] == OPERATORS.getLastBracket())
                        OPERATORS.bracketStack.Pop();
                    else
                        throw new Exception("Bracket error!");
                }
                if (brTypeO >= 0 && brTypeC >= 0)
                {
                    if (OPERATORS.getLastBracket() != symbol)
                        OPERATORS.bracketStack.Push(symbol);
                    else
                        OPERATORS.bracketStack.Pop();
                }
                //Console.Write(symbol);
            }
            //if(symbol == '\"')
            //    Console.WriteLine("open \"  " + OPERATORS.bracketStack.Count);
            //if (OPERATORS.bracketOpen.IndexOf(OPERATORS.getLastBracket()) >= 3 && (OPERATORS.bracketOpen.IndexOf(symbol) >= 3))
            //    return -OPERATORS.bracketStack.Count;
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
                if (empty == 0 /*|| (empty == 1 && OPERATORS.bracketStack.First() == '\"')*/)
                {
                    if (currentInnerParse.Length > 0)   // put a inner bracket to a list for far going parsing
                    {
                        currentInnerParse += S[nowIn];
                        char first = (currentInnerParse[0]);
                        //if (OPERATORS.bracketOpen.IndexOf(first) >= 3)
                        //    currentInnerParse += currentInnerParse[0];

                        inBracketsPrase.Add(currentInnerParse);

                        COMMON.TraceOnDep("|" + currentInnerParse + "|", 5, ConsoleColor.Green);
                        currentInnerParse = "";
                    }
                    // add to a main zero level parsing
                    char addC = ((OPERATORS.bracketClose.IndexOf(S[nowIn]) < 0) ? S[nowIn] : '@');
                    if (addC != ' ')
                        noBracketsParse += addC;
                    //else
                    //    noBracketsParse += OPERATORS.bracketStack.Count;
                }
                else
                {
                    if (empty < 0)
                        // adding a " " and ' ' symbols to inner strings and chars
                        currentInnerParse += S[nowIn - 1];

                    // adding to current non zero level
                    currentInnerParse += S[nowIn];// ((OPERATORS.bracketOpen.IndexOf(S[nowIn]) < 0) ? S[nowIn] + "" : "");
                }
                nowIn++;
            }
            Console.WriteLine(": " + noBracketsParse);
            for (int i = 0; i < inBracketsPrase.Count; i++)
            {
                if (OPERATORS.bracketOpen.IndexOf(inBracketsPrase[i][0]) < 3)
                    inBracketsPrase[i] = inBracketsPrase[i].Substring(1, inBracketsPrase[i].Length - 2);
                Console.WriteLine("    {" + inBracketsPrase[i] + "}");
            }

            innerParse = inBracketsPrase;

            while
            (noBracketsParse == "@" && innerParse[0][0] == '(')
            {
                string innerP = innerParse[0]; innerParse = new List<string>();
                noBracketsParse = ParseLevels(innerP, out innerParse);
            }
            return noBracketsParse;
        }
    }
}
