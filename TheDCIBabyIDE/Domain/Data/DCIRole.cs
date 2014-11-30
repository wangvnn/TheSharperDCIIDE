using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data
{
    internal class DCIRole
    {
        public IEnumerable<DCIRoleInterface> Interfaces { get; private set; }
        public IEnumerable<DCIRoleMethod> Methods { get; private set;  }
    }
}
