using EyePatch.Core.Documents;
using EyePatch.Core.Documents.Children;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class SearchEngineMetaNode : Node
    {
        public SearchEngineMetaNode(PageItem page)
        {
            Id = page.Id;
            CssClass = "search";
            Name = "Search";
        }

        public SearchEngineMetaNode(Template template)
        {
            Id = template.Id;
            CssClass = "search";
            Type = "templateSearch";
            Name = "Search";
        }
    }
}