using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class SearchEngineMetaNode : Node
    {
        public SearchEngineMetaNode(Entity.Page page)
        {
            Id = page.ID.ToString();
            CssClass = "search";
            Name = "Search";
        }

        public SearchEngineMetaNode(Entity.Template template)
        {
            Id = template.ID.ToString();
            CssClass = "search";
            Type = "templateSearch";
            Name = "Search";
        }
    }
}