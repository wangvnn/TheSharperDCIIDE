using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data
{
    internal class DCIContext
    {
        public IEnumerable<DCIRole> Roles { get; private set; }
    }
}
