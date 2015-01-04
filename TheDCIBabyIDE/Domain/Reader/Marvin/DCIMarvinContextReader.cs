using System;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using KimHaiQuang.TheDCIBabyIDE.Domain.Operation;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Reader.Marvin
{
    public class DCIMarvinContextReader : 
        ContextFileParsingContext.IDCIContextReader
    {
        public DCIMarvinContextReader(DCIContext contextFileModel)
        {
            throw new Exception("No implementation");
        }

        public DCIContext Read(string filePath)
        {
            throw new Exception("No implementation");
        }
    }
}
