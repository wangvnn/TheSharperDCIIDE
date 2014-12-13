using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data
{
    public class DCIContext
    {
        public IList<DCIRole> Roles { get; private set; }
        public string Filepath { get; private set; }
        public DCIContext()
        { }

        public void LoadFromFile(string filePath)
        {
            Filepath = filePath;

            using (var file = File.OpenText(filePath))
            {
                var tree = CSharpSyntaxTree.ParseText(file.ReadToEnd());

                LoadContextInfo(tree);
                LoadRoleInfo(tree);
            }
       
        }

        private void LoadContextInfo(SyntaxTree tree)
        {
            var root = tree.GetRoot();
            if (root != null)
            {
                var classNode = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
                Name = classNode.Identifier.ToString();
            }
        }

        private void LoadRoleInfo(SyntaxTree tree)
        {
            Roles = new List<DCIRole>();
            var root = tree.GetRoot();

            if (root != null)
            {
                var properties = root.DescendantNodes().OfType<PropertyDeclarationSyntax>();
                foreach (var p in properties)
                {
                    if (p.Identifier.ToString().EndsWith("Role")){
                        var newRole = new DCIRole(p.Identifier.ToString());

                        Roles.Add(newRole);
                    }
                }

            }
        }


        public string Name { get; private set; }
    }
}





