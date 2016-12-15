using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo
{
    public class DCIRole : SpanObject
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
            if (!Methods.ContainsKey(method.Name))
                _Methods.Add(method.Name, method);
        }

        public void Remove(string name)
        {
            if (Methods.ContainsKey(name))
                _Methods.Remove(name);
        }
    }
}
