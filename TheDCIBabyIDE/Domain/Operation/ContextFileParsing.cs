using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Operation
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

    public class ContextFileParsingContext
    {
        #region Usecase
        /* USE CASE 2: Parse CONTEXT FILE
        # Primary Actor: SYSTEM
        # Precondition: CONTEXT FILE is valid
        # Postcondition: DCI CONTEXT PARSER will return CONTEX INFO
        # Trigger: SYSTEM asks the PARSER to parse the FILE
        # Main Success Scenario:
        # 1. SYSTEM asks the PARSER to parse the FILE
        #   2. PARSER parses the FILE
        #   3. PARSER return INFO
        */
        #endregion

        #region Context

        public DCIContext Parse(string filename)
        {
            var dciContext = new DCIContext();
            return dciContext;
        }

        #endregion
        #region private properties
        private List<RegionNodes> RegionNodesList = new List<RegionNodes>();
        #endregion
        //#region TODO parser algo
        //public void LoadFromFile(string filePath)
        //{
        //    Filepath = filePath;

        //    using (var file = File.OpenText(filePath))
        //    {
        //        var tree = CSharpSyntaxTree.ParseText(file.ReadToEnd());
        //        var root = tree.GetRoot();
        //        if (root != null)
        //        {
        //            LoadContextInfo(root);
        //        }
        //        else
        //        {
        //            throw new Exception("Cannot get root of syntext tree.");
        //        }
        //    }

        //}

        //private void LoadContextInfo(SyntaxNode root)
        //{
        //    var classNode = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        //    Name = classNode.Identifier.ToString();

        //    LoadInfoFromRegions(root, classNode);
        //}

        //private void LoadInfoFromRegions(SyntaxNode root, SyntaxNode classNode)
        //{
        //    FindAllRegons(root);
        //    LoadUsecaseInfo(classNode);
        //    LoadRoleInfo(classNode);
        //}

        //private void FindAllRegons(SyntaxNode root)
        //{
        //    foreach (var regionDirective in root.DescendantTrivia().Where(i => i.IsKind(SyntaxKind.RegionDirectiveTrivia)))
        //    {
        //        RegionNodesList.Add(new RegionNodes { RegionDirective = regionDirective });
        //    }

        //    var count = 0;
        //    foreach (var endRegionDirective in root.DescendantTrivia().Where(i => i.IsKind(SyntaxKind.EndRegionDirectiveTrivia)))
        //    {
        //        RegionNodesList[count++].EndRegionDirective = endRegionDirective;
        //    }
        //    foreach (var node in root.DescendantNodes().Where(i => i is MemberDeclarationSyntax || i is StatementSyntax))
        //    {
        //        foreach (var regionNodes in RegionNodesList)
        //        {
        //            regionNodes.AddNode(node);
        //        }
        //    }
        //}

        //private void LoadUsecaseInfo(SyntaxNode classNode)
        //{
        //    var usecaseRegion = RegionNodesList.Where(r => r.RegionName.Contains("Usecase")).FirstOrDefault();
        //    if (usecaseRegion != null)
        //    {
        //        Usecase = new TextSpan(usecaseRegion.RegionSpan.Start, usecaseRegion.RegionSpan.Length);
        //    }
        //}

        //private void LoadRoleInfo(SyntaxNode classNode)
        //{
        //    Roles = new List<DCIRole>();

        //    var roleRegion = RegionNodesList.Where(r => r.RegionName.Contains("RolesAndInterfaces")).FirstOrDefault();
        //    if (roleRegion != null)
        //    {
        //        foreach (var node in roleRegion.Nodes)
        //        {
        //            if (node.IsKind(SyntaxKind.PropertyDeclaration) &&
        //                classNode == node.Parent)
        //            {
        //                var property = node as PropertyDeclarationSyntax;
        //                var newRole = new DCIRole(property.Identifier.ToString());
        //                Roles.Add(newRole);
        //                LoadRoleInterface(classNode, roleRegion, property, newRole);
        //            }
        //        }
        //    }

        //}

        //private void LoadRoleInterface(SyntaxNode classNode, RegionNodes roleRegion, PropertyDeclarationSyntax roleNode, DCIRole role)
        //{
        //    foreach (var node in roleRegion.Nodes)
        //    {
        //        if (node.IsKind(SyntaxKind.InterfaceDeclaration) &&
        //            classNode == node.Parent)
        //        {
        //            var interfaceNode = node as InterfaceDeclarationSyntax;
        //            var type = roleNode.Type as IdentifierNameSyntax;

        //            if (type != null && type.Identifier.ToString().Equals(interfaceNode.Identifier.ToString()))
        //            {
        //                var newInterface = new DCIRoleInterface(interfaceNode.Identifier.ToString());
        //                role.Interface = newInterface;
        //                foreach (var member in interfaceNode.Members)
        //                {
        //                    var property = member as PropertyDeclarationSyntax;
        //                    var method = member as MethodDeclarationSyntax;
        //                    var newInterfaceSignature = new InterfaceSignature();

        //                    var start = property != null ? property.Span.Start : method.Span.Start;
        //                    var length = property != null ? property.Span.Length : method.Span.Length;

        //                    newInterfaceSignature.Identifier = property != null ? property.Identifier.ToString() : method.Identifier.ToString();
        //                    newInterfaceSignature.Span = new TextSpan(start, length);

        //                    newInterface.AddSignature(newInterfaceSignature);
        //                }
        //            }


        //        }
        //    }
        //}
        //#endregion

    }
}
