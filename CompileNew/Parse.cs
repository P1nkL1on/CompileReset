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
                }
                catch (Exception e) { }
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
                    //if (addC != ' ')
                    noBracketsParse += addC;
                    if (addC == '*')
                        noBracketsParse += ' ';
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
            //Console.WriteLine(": " + noBracketsParse);
            for (int i = 0; i < inBracketsPrase.Count; i++)
            {
                if (OPERATORS.bracketOpen.IndexOf(inBracketsPrase[i][0]) < 3)
                    inBracketsPrase[i] = inBracketsPrase[i].Substring(1, inBracketsPrase[i].Length - 2);
                //Console.WriteLine("    {" + inBracketsPrase[i] + "}");
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

        public static string ParseMono(string S, out List<string> monoInner, ref List<string> innerDogs)
        {

            //" * @ + &@ - !8 + 7++"

            int nowIn = 0, currentPrevOperatorIndex = -1, currentPostOperatorIndex = -1, wasPrev = -1, wasPost = -1;
            string lastValuePuted = "", currentMayBeOperator = "", res = "", prevLastValuePuted = "";
            int nextOprIsPrev = 0;
            monoInner = new List<string>();


            while (nowIn < S.Length)
            {
                currentMayBeOperator += S[nowIn];

                currentPrevOperatorIndex = currentPostOperatorIndex = -1;
                for (int i = 0; i < OPERATORS.monoPreNames.Count; i++)
                    if (OPERATORS.monoPreNames[i].IndexOf(currentMayBeOperator) == 0)
                        currentPrevOperatorIndex = i;
                for (int i = 0; i < OPERATORS.monoPostNames.Count; i++)
                    if (OPERATORS.monoPostNames[i].IndexOf(currentMayBeOperator) == 0)
                        currentPostOperatorIndex = i;

                if (currentPrevOperatorIndex == -1 && wasPrev >= 0)
                {
                    string oper = currentMayBeOperator.Substring(0, currentMayBeOperator.Length - 1);//OPERATORS.monoPreNames[wasPrev];
                    int operatorIndex = OPERATORS.monoPreNames.IndexOf(oper);
                    // to prevent + in case of ++ real
                    if (operatorIndex >= 0 && prevLastValuePuted == "")
                    {
                        //string checkForExtra = oper + S[nowIn + 1];
                        //int index = OPERATORS.names.IndexOf(checkForExtra);
                        nextOprIsPrev = 1;
                        monoInner.Add(oper);
                        res += "#";
                    }
                    else
                    {
                        res += currentMayBeOperator.Remove(currentMayBeOperator.Length - 1);
                    }
                }
                wasPrev = currentPrevOperatorIndex;

                if (currentPostOperatorIndex == -1 && wasPost >= 0)
                {
                    string oper = currentMayBeOperator.Substring(0, currentMayBeOperator.Length - 1);//OPERATORS.monoPreNames[wasPrev];
                    int operatorIndex = OPERATORS.monoPostNames.IndexOf(oper);
                    if (operatorIndex >= 0 && prevLastValuePuted != "")
                    {
                        monoInner.Add(prevLastValuePuted + oper);
                        res = res.TrimEnd(' ');
                        res = res.Substring(0, res.Length - oper.Length).TrimEnd(' ');
                        //res = res
                        res = res.Substring(0, res.Length - prevLastValuePuted.Length).TrimEnd(' ') + "#";
                    }
                }
                wasPost = currentPostOperatorIndex;

                if (currentPostOperatorIndex < 0 && currentPrevOperatorIndex < 0)
                {
                    currentMayBeOperator = "";
                    if (nextOprIsPrev == 0)
                        lastValuePuted += S[nowIn];
                    else
                    {
                        if (OPERATORS.nonConfirmedSymbols.IndexOf(S[nowIn]) >= 0 && nextOprIsPrev == 2)
                            nextOprIsPrev = 0;
                        else
                        {
                            monoInner[monoInner.Count - 1] += S[nowIn];
                            if (S[nowIn] != ' ')
                            {
                                nextOprIsPrev = 2;
                                ///!!!
                                //lastValuePuted = lastValuePuted.Substring(0, lastValuePuted.Length - 1);
                            }
                        }
                    }
                }
                else
                {
                    if (lastValuePuted.Length > 0)
                    {
                        prevLastValuePuted = "";
                        int niv = 0;
                        while (niv < lastValuePuted.Length) { if (lastValuePuted[niv] != ' ') prevLastValuePuted += lastValuePuted[niv]; niv++; }
                    }
                    res += lastValuePuted;
                    lastValuePuted = "";
                }

                nowIn++;
            }
            if (currentPostOperatorIndex >= 0)
            {
                string oper = currentMayBeOperator.Substring(0, currentMayBeOperator.Length);//OPERATORS.monoPreNames[wasPrev];
                int operatorIndex = OPERATORS.monoPostNames.IndexOf(oper);
                if (operatorIndex >= 0 && prevLastValuePuted != "")
                {
                    monoInner.Add(prevLastValuePuted + oper);
                    res = res.TrimEnd(' ');
                    res = res.Substring(0, res.Length - oper.Length).TrimEnd(' ');
                    // res = res
                    res = res.Substring(0, res.Length - prevLastValuePuted.Length).TrimEnd(' ') + "#";
                }
            }
            res = res + lastValuePuted; int dogIndex = 0;
            //  1 + #* # + 15 + @ -@
            //  !@
            //  
            for (int i = 0; i < monoInner.Count; i++)
                monoInner[i] = monoInner[i].Trim(' ');
            int currentDogInd = 0;
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] == '@')
                    currentDogInd++;
                // false # like # && == meaned a incorrect struct
                if (dogIndex < monoInner.Count && res[i] == '#')
                {
                    if (OPERATORS.names.IndexOf(monoInner[dogIndex]) >= 0)
                    {
                        res = res.Substring(0, i) + monoInner[dogIndex] + res.Substring(i + 1);
                        monoInner.RemoveAt(dogIndex);
                    }
                    else
                    {
                        //Console.WriteLine(monoInner[dogIndex]);
                        if (monoInner[dogIndex].IndexOf("@") >= 0)
                            while (monoInner[dogIndex].IndexOf("@") >= 0)
                            {
                                monoInner[dogIndex] = monoInner[dogIndex].Substring(0, monoInner[dogIndex].IndexOf("@")) 
                                    + "(" + innerDogs[currentDogInd] + ")"
                                    + monoInner[dogIndex].Substring(monoInner[dogIndex].IndexOf("@") + 1);
                                innerDogs.RemoveAt(currentDogInd);
                                //currentDogInd++;
                            }
                        dogIndex++;
                    }
                }
            }
            return res;
        }

    }

    //public static string ParseMonoOps(string S, out List<string> innerParse)
    //{
    //    // **++@ + 1233 - *@--
    //}
}
