using System.Collections.Generic;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data
{
    public class DCIRole
    {
        public string Name { get; set; }
        public DCIRoleInterface Interface { get; set; }
        private Dictionary<string, DCIRoleMethod> _Methods = new Dictionary<string, DCIRoleMethod>();
        public Dictionary<string, DCIRoleMethod> Methods { get { return _Methods; } }

        public DCIRole()
        {
        }

        public void AddMethod(DCIRoleMethod method)
        {
            _Methods.Add(method.Name, method);
        }

        public void Remove(string name)
        {
            _Methods.Remove(name);
        }
    }
}
