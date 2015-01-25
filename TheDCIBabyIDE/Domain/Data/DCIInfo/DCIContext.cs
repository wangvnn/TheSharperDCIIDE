using System.Collections.Generic;
using Microsoft.VisualStudio.Text;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo
{
    public class SpanObject
    {
        public Span CodeSpan { get; set; }
    }

    public class DCIContext : SpanObject
    {
        #region public properties

        public string Filepath { get; set; }
        public string Name { get; set; }

        public Span UsecaseSpan { get; set; }

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