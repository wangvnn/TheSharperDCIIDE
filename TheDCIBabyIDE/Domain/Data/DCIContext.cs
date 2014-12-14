using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data
{
    class RegionNodes
    {
        public SyntaxTrivia RegionDirective;
        public SyntaxTrivia EndRegionDirective;

        public string RegionName
        {
            get { return RegionDirective.ToFullString(); }
        }

        public TextSpan RegionSpan
        {
            get
            {
                var start = RegionDirective.Span.Start;
                var end = EndRegionDirective.Span.Start + EndRegionDirective.Span.Length;
                if (end >= start)
                {
                    return new TextSpan(start, end - start);
                }
                else
                {
                    return new TextSpan(start, 0);
                }
            }
        }
        public List<SyntaxNode> Nodes = new List<SyntaxNode>();
        public void AddNode(SyntaxNode node)
        {
            if (RegionSpan.Contains(node.Span))
                Nodes.Add(node);
        }
    }

    public class DCIContext
    {

        #region public properties

        public string Filepath { get; private set; }
        public string Name { get; private set; }

        public TextSpan Usecase { get; private set; }
        public IList<DCIRole> Roles { get; private set; }

        #endregion

        #region private properties
        private List<RegionNodes>  RegionNodesList = new List<RegionNodes>();
        #endregion

        public DCIContext()
        { }

        public void LoadFromFile(string filePath)
        {
            Filepath = filePath;

            using (var file = File.OpenText(filePath))
            {
                var tree = CSharpSyntaxTree.ParseText(file.ReadToEnd());
                var root = tree.GetRoot();
                if (root != null)
                {
                    LoadContextInfo(root);
                }
                else
                {
                    throw new Exception("Cannot get root of syntext tree.");
                }
            }
       
        }

        private void LoadContextInfo(SyntaxNode root)
        {
            var classNode = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            Name = classNode.Identifier.ToString();

            LoadInfoFromRegions(root, classNode);  
        }

        private void LoadInfoFromRegions(SyntaxNode root, SyntaxNode classNode)
        {
            FindAllRegons(root);
            LoadUsecaseInfo(classNode);
            LoadRoleInfo(classNode);
        }

        private void FindAllRegons(SyntaxNode root)
        {
            foreach (var regionDirective in root.DescendantTrivia().Where(i => i.IsKind(SyntaxKind.RegionDirectiveTrivia)))
            {
                RegionNodesList.Add(new RegionNodes { RegionDirective = regionDirective });
            }

            var count = 0;
            foreach (var endRegionDirective in root.DescendantTrivia().Where(i => i.IsKind(SyntaxKind.EndRegionDirectiveTrivia)))
            {
                RegionNodesList[count++].EndRegionDirective = endRegionDirective;
            }
            foreach (var node in root.DescendantNodes().Where(i => i is MemberDeclarationSyntax || i is StatementSyntax))
            {
                foreach (var regionNodes in RegionNodesList)
                {
                    regionNodes.AddNode(node);
                }
            }
        }

        private void LoadUsecaseInfo(SyntaxNode classNode)
        {
            var usecaseRegion = RegionNodesList.Where(r => r.RegionName.Contains("Usecase")).FirstOrDefault();
            if (usecaseRegion != null)
            {
                Usecase = new TextSpan(usecaseRegion.RegionSpan.Start, usecaseRegion.RegionSpan.Length);
            }
        }

        private void LoadRoleInfo(SyntaxNode classNode)
        {
            Roles = new List<DCIRole>();

            var roleRegion = RegionNodesList.Where(r => r.RegionName.Contains("RolesAndInterfaces")).FirstOrDefault();
            if (roleRegion != null)
            {
                foreach (var node in roleRegion.Nodes)
                {
                    if (node.IsKind(SyntaxKind.PropertyDeclaration) &&
                        classNode == node.Parent)
                    {
                        var property = node as PropertyDeclarationSyntax;
                        var newRole = new DCIRole(property.Identifier.ToString());
                        Roles.Add(newRole);
                    }
                }
            }

        }

    }
}