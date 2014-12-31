using Microsoft.VisualStudio.Text;
using System.Collections.Generic;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo
{

    public struct DCIInterfaceSignature
    {
        public string Name { get; set; }
        public Span InterfaceSpan { get; set; }
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
            _Signatures.Add(signature.Name, signature);
        }

        public void RemoveSignature(string name)
        {
            _Signatures.Remove(name);
        }
    }
}
