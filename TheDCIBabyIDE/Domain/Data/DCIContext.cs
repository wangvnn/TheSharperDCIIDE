using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data
{
    public class DCIContext
    {
        public IList<DCIRole> Roles { get; private set; }

        public void LoadFromFile(string filePath)
        {
            using (var file = File.OpenText(filePath))
            {
                var tree = CSharpSyntaxTree.ParseText(file.ReadToEnd());

                var syntaxRoot = tree.GetRoot();
                var MyClass = syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
                var MyMethod = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().First();

                Console.WriteLine(MyClass.Identifier.ToString());
                Console.WriteLine(MyMethod.Identifier.ToString());               
            }
       
        }

    }
}
