using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data
{

    public class DCIContext
    {
        #region public properties

        public string Filepath { get; set; }
        public string Name { get; set; }

        public Span UsecaseSpan { get; set; }
        public Span ContextSpan { get; set; }

        private Dictionary<string, DCIRole> _Roles = new Dictionary<string, DCIRole>();
        public Dictionary<string, DCIRole> Roles { get { return _Roles; } }

        #endregion

        public DCIContext()
        { }

        public void AddRole(DCIRole role)
        {
            _Roles.Add(role.Name, role);
        }

        public void RemoveRole(string name)
        {
            _Roles.Remove(name);
        }
    }
}