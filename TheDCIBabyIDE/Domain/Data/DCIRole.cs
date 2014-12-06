using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data
{
    public class DCIRole
    {
        public IEnumerable<DCIRoleInterface> Interfaces { get; private set; }
        public IEnumerable<DCIRoleMethod> Methods { get; private set;  }
        public string Name { get; private set; }

        public DCIRole(string name){
            Name = name;
        }
    }
}
