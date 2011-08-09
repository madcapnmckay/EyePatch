using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class PageNode : Node
    {
        public PageNode()
        {
            CssClass = "page";
        }

        public PageNode(Entity.Page page) : this()
        {
            Id = page.ID.ToString();
            if (page.Url == "/") {
                CssClass = "homepage";
                Type = "page";
            }
            Name = page.Name;
            
            // meta nodes
            Children.Add(new SearchEngineMetaNode(page));
            Children.Add(new FacebookMetaNode(page));
        }
    }
}