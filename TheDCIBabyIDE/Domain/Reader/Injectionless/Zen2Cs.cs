using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Reader.Injectionless
{
    public class Zen2Cs
    {
        public static void Transform(ref string sourceCode)
        {
            sourceCode = sourceCode.Replace("* ", " ");
            sourceCode = sourceCode.Replace("implements ", ": ");
            sourceCode = sourceCode.Replace("*)", ")");
        }
    }
}
