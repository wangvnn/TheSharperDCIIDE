using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace KimHaiQuang.TheDCIBabyIDE.Domain.Data.Code
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

}
