using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data.Settings
{
    public class DCIBabyIDESettings
    {
        public enum ContextFiletype
        {
            ContextFiletype_Unknown,
            ContextFiletype_Injectionless,
            ContextFiletype_Marvin
        }

        public ContextFiletype ContextFileTypeSettings { get; set; }
    }
}
