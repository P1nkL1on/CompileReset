using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileNew
{
    public static class UniqOperation
    {
        public static operation TryParse(String S)
        {
            // check for define struct
            string defineS = S.Trim(' ');
            for (int i = 0; i < TYPES.names.Count; i++)
                if (defineS.IndexOf(TYPES.names[i]) == 0)
                {
                    int typeInd = i, pointerCount = 0;
                    defineS = defineS.Substring( TYPES.names[i].Length );
                    while (defineS.Length > 0 && defineS[0] == '*')
                    { pointerCount++; defineS = defineS.Substring(1); }
                    if (defineS.Length > 1 && defineS[0] == ' ')
                    {
                        defineS = defineS.Trim(' ');
                        if (defineS.Length > 0) // any of name detected (not allow "" variables with zero name)
                        {

                        }
                    }
                }
            return null;
        }
    }
    class Define : monoOperation
    {
        public Define()
        {

        }
    }
}
