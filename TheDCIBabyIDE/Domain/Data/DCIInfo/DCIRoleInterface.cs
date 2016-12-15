using Microsoft.VisualStudio.Text;
using System.Collections.Generic;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo
{

    public class DCIInterfaceSignature : SpanObject
    {
        public string Name { get; set; }
    }

    public class DCIRoleInterface
    {
        public string Name { get; set; }

        public Dictionary<string, DCIInterfaceSignature> Signatures { get { return _Signatures; } }
        private Dictionary<string, DCIInterfaceSignature> _Signatures = new Dictionary<string, DCIInterfaceSignature>();

        public DCIRoleInterface()
        {
        }

        public void AddSignature(DCIInterfaceSignature signature)
        {
            if (!_Signatures.ContainsKey(signature.Name))
                _Signatures.Add(signature.Name, signature);
        }

        public void RemoveSignature(string name)
        {
            if (!_Signatures.ContainsKey(name))
                _Signatures.Remove(name);
        }
    }
}
