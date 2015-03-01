using System;
using System.Collections.Generic;
using System.Linq;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.DCIInfo;
using KimHaiQuang.TheDCIBabyIDE.Domain.Operation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;
using KimHaiQuang.TheDCIBabyIDE.Domain.Data.Code;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Reader.Injectionless
{
    public class DCIInjectionlessContextReader :
        ContextFileParsingContext.IDCIContextReader
    {
        #region Usecase

        /* USE CASE 2: Read INJECTIONLESS CONTEXT FILE
        # Primary Actor: SYSTEM
        # Precondition: CONTEXT FILE is valid
        # Postcondition: DCI CONTEXT READER will return CONTEX INFO
        # Trigger: SYSTEM asks the CONTEXT READER to read CONTEXT FILE
        # Main Success Scenario:
        # 1. REGION READER reads all the REGIONS
        # 2. CONTEXT READER reads CONTEXT INFO
        # 3. USECASE READER reads USECASE INFO
        # 4. ROLE READER reads ROLE INFO
        # 5. INTERACTION READER reads INTERACTIONS
        */

        #endregion

        #region Roles

         private DCIContext ContextFileModel { get; set; }

        private List<RegionNodes> RegionReader { get; set; }

        private DCIContext ContextReader { get; set; }
        private DCIContext UsecaseReader { get; set; }
        private DCIContext RoleReader { get; set; }
        private DCIContext InteractionReader { get; set; }

        #endregion

        #region Context

        public DCIInjectionlessContextReader(DCIContext contextFileModel)
        {
            ContextFileModel = contextFileModel;

            RegionReader =  new List<RegionNodes>();

            ContextReader = contextFileModel;
            UsecaseReader = contextFileModel;
            RoleReader = contextFileModel;
            InteractionReader = contextFileModel;
        }

        public DCIContext Read(string filePath)
        {
            RegionReader_Read(filePath);

            return ContextFileModel;
        }

        #endregion

        #region RegionReader_Methods

        private void RegionReader_Read(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }
            using (var file = File.OpenText(filePath))
            {
                var tree = CSharpSyntaxTree.ParseText(file.ReadToEnd());
                var root = tree.GetRoot();
                if (root != null)
                {
                    var classNode = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

                    RegionReader_FindAllRegions(root);

                    if (RegionReader.Count > 0)
                    {
                        ContextReader_Read(classNode);
                        UsecaseReader_Read();
                        RoleReader_Read(classNode);
                        InteractionReader_ReadInteractions(classNode);
                    }
                }
                else
                {
                    throw new Exception("Cannot get root of syntext tree.");
                }
            }


        }

        private void RegionReader_FindAllRegions(SyntaxNode root)
        {
            foreach (var regionDirective in root.DescendantTrivia().Where(i => i.IsKind(SyntaxKind.RegionDirectiveTrivia)))
            {
                RegionReader.Add(new RegionNodes { RegionDirective = regionDirective });
            }

            var count = 0;
            foreach (var endRegionDirective in root.DescendantTrivia().Where(i => i.IsKind(SyntaxKind.EndRegionDirectiveTrivia)))
            {
                RegionReader[count++].EndRegionDirective = endRegionDirective;
            }
            foreach (var node in root.DescendantNodes().Where(i => i is MemberDeclarationSyntax || i is StatementSyntax))
            {
                foreach (var regionNodes in RegionReader)
                {
                    regionNodes.AddNode(node);
                }
            }
        }

        #endregion

        #region ContextReader_Methods

        private void ContextReader_Read(ClassDeclarationSyntax parentNode)
        {
            ContextFileModel.Name = parentNode.Identifier.ToString();
            ContextFileModel.CodeSpan = new Span(parentNode.Span.Start, parentNode.Span.Length);

            var contextRegion = RegionReader.Where(r => r.RegionName.Contains("Context")).FirstOrDefault();
            if (contextRegion != null)
            {
                DCIRole contextRole = null;

                if (contextRole == null)
                {
                    contextRole = new DCIRole();
                    contextRole.Name = parentNode.Identifier.ToString();
                    ContextFileModel.AddRole(contextRole);
                }

                foreach (var node in contextRegion.Nodes)
                {
                    if (node.IsKind(SyntaxKind.MethodDeclaration) &&
                        parentNode == node.Parent)
                    {

                        var methodNode = node as MethodDeclarationSyntax;
                        var roleMethod = new DCIRoleMethod();
                        var underScoreIndex = methodNode.Identifier.ToString().IndexOf("_");

                        roleMethod.Name = methodNode.Identifier.ToString();

                        roleMethod.CodeSpan = new Span(methodNode.Span.Start, methodNode.Span.Length);
                        contextRole.AddMethod(roleMethod);
                    }
                    else if (node.IsKind(SyntaxKind.ConstructorDeclaration) &&
                            parentNode == node.Parent)
                    {
                        contextRole.CodeSpan = new Span(node.Span.Start, node.Span.Length);
                    }
                }
            }
        }

        #endregion

        #region UsecaseReader_Methods

        private void UsecaseReader_Read()
        {
            var usecaseRegion = RegionReader.Where(r => r.RegionName.Contains("Usecase")).FirstOrDefault();
            if (usecaseRegion != null)
            {
                ContextFileModel.UsecaseSpan = new Span(usecaseRegion.RegionSpan.Start, usecaseRegion.RegionSpan.Length);
            }
        }

        #endregion

        #region RoleReader_Methods

        private void RoleReader_Read(ClassDeclarationSyntax parentNode)
        {
            var roleRegion = RegionReader.Where(r => r.RegionName.Contains("Roles")).FirstOrDefault();
            if (roleRegion != null)
            {
                foreach (var node in roleRegion.Nodes)
                {
                    if (node.IsKind(SyntaxKind.PropertyDeclaration) &&
                        parentNode == node.Parent)
                    {
                        var roleNode = node as PropertyDeclarationSyntax;
                        var roleTypeNode = (roleNode.Type as IdentifierNameSyntax);
                        var roleNodeTypeName = roleTypeNode != null ? roleTypeNode.Identifier.ToString() : "";

                        var newRole = new DCIRole();
                        newRole.Name = roleNode.Identifier.ToString();
                        newRole.CodeSpan = new Span(roleNode.Span.Start, roleNode.Span.Length);
                        ContextFileModel.AddRole(newRole);

                        RoleReader_ReadInterface(newRole, roleRegion, roleNodeTypeName, parentNode);
                        RoleReader_ReadMethods(newRole, parentNode);
                    }
                }
            }

        }

        private void RoleReader_ReadMethods(DCIRole role, ClassDeclarationSyntax parentNode)
        {
            foreach (var roleMethodRegion in RegionReader.Where(r => r.RegionName.Contains("_Methods") &&
                                                                        r.RegionName.Contains(role.Name+"_")))
            {
                foreach (var node in roleMethodRegion.Nodes)
                {
                    if (node.IsKind(SyntaxKind.MethodDeclaration) &&
                        parentNode == node.Parent)
                    {
                        var methodNode = node as MethodDeclarationSyntax;
                        var roleMethod = new DCIRoleMethod();

                        var underScoreIndex = methodNode.Identifier.ToString().IndexOf("_");

                        if (underScoreIndex >= 0 && underScoreIndex < methodNode.Identifier.ToString().Length - 1)
                        {
                            roleMethod.Name = methodNode.Identifier.ToString().Substring(underScoreIndex + 1);
                        }
                        else
                        {
                            roleMethod.Name = methodNode.Identifier.ToString();
                        }
                        roleMethod.CodeSpan = new Span(methodNode.Span.Start, methodNode.Span.Length);
                        role.AddMethod(roleMethod);
                    }
                }
            }
        }

        private void RoleReader_ReadInterface(DCIRole role,  RegionNodes roleRegion, string roleName, ClassDeclarationSyntax parentNode)
        {
            foreach (var node in roleRegion.Nodes)
            {
                if (node.IsKind(SyntaxKind.InterfaceDeclaration) &&
                    parentNode == node.Parent)
                {
                    var interfaceNode = node as InterfaceDeclarationSyntax;

                    if (roleName.Equals(interfaceNode.Identifier.ToString()))
                    {
                        var newInterface = new DCIRoleInterface();
                        newInterface.Name = interfaceNode.Identifier.ToString();
                        role.Interface = newInterface;
                        foreach (var member in interfaceNode.Members)
                        {
                            var property = member as PropertyDeclarationSyntax;
                            var method = member as MethodDeclarationSyntax;
                            var newInterfaceSignature = new DCIInterfaceSignature();

                            var start = property != null ? property.Span.Start : method.Span.Start;
                            var length = property != null ? property.Span.Length : method.Span.Length;

                            newInterfaceSignature.Name = property != null ? property.Identifier.ToString() : method.Identifier.ToString();
                            newInterfaceSignature.CodeSpan = new Span(start, length);

                            newInterface.AddSignature(newInterfaceSignature);
                        }
                    }
                }
            }
        }

        #endregion

        #region InteractionReader_Methods

        private void InteractionReader_ReadContextInteractions(ClassDeclarationSyntax parentNode)
        {
            var role1 = ContextFileModel.Roles.Values.ElementAt(0);

            var roleMethodRegion = RegionReader.FirstOrDefault(r => r.RegionName.Contains("Context"));

            foreach (var node in roleMethodRegion.Nodes)
            {
                if (node.HasParentIsKindOf(SyntaxKind.MethodDeclaration) ||
                    node.HasParentIsKindOf(SyntaxKind.ConstructorDeclaration))
                {
                    InteractionReader_FindInteraction(role1, node);
                }
            }
        }

        private void InteractionReader_ReadRoleInteractions(ClassDeclarationSyntax parentNode)
        {
            for (int i = 0; i < ContextFileModel.Roles.Values.Count; ++i)
            {
                var role1 = ContextFileModel.Roles.Values.ElementAt(i);

                foreach (var roleMethodRegion in RegionReader.Where(r => r.RegionName.Contains("_Methods") &&
                                                                        r.RegionName.Contains(role1.Name)))
                {
                    foreach (var node in roleMethodRegion.Nodes)
                    {
                        InteractionReader_FindInteraction(role1, node);
                    }
                }
            }

        }
        private void InteractionReader_FindInteraction(DCIRole role1, SyntaxNode node)
        {
            var role2 = InteractionReader_FindTargetRole(role1, node.ToString());

            if (role2 == null && node.IsKind(SyntaxKind.ExpressionStatement))
            {
                var assignement = (node as ExpressionStatementSyntax).Expression as AssignmentExpressionSyntax;
                if (assignement != null)
                    role2 = InteractionReader_FindTargetRole(role1, assignement.Right.ToString(), true);
            }

            if (role2 != null)
            {
                var interaction = new DCIInteraction();
                interaction.Source = role1;
                interaction.Target = role2;
                interaction.Name = node.ToString();
                ContextFileModel.AddInteraction(interaction);
            }
        }

        private DCIRole InteractionReader_FindTargetRole(DCIRole role1, string expression, bool checkSameName=false)
        {
            for (int j = 0; j < ContextFileModel.Roles.Values.Count; ++j)
            {
                var role2 = ContextFileModel.Roles.Values.ElementAt(j);

                if (role1 != role2)
                {
                    if (expression.Contains(role2.Name + "_") ||
                        expression.Contains(role2.Name + ".") ||
                        (expression.Contains(role2.Name) && checkSameName))
                    {
                        return role2;
                    }
                }
            }
            return null;
        }


        private void InteractionReader_ReadInteractions(ClassDeclarationSyntax parentNode)
        {
            InteractionReader_ReadContextInteractions(parentNode);
            InteractionReader_ReadRoleInteractions(parentNode);
        }
        #endregion
    }

    public static class SyntaxNodeExtension
    {
        public static bool HasParentIsKindOf(this SyntaxNode node, SyntaxKind kind)
        {
            var result = false;

            var parent = node.Parent;
            if (parent != null)
            {
                result = parent.IsKind(kind);

                if (!result)
                {
                    result = parent.HasParentIsKindOf(kind);
                }
            }

            return result;
        }
    }
}