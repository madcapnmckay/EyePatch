using EyePatch.Core.Documents.Children;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class PageNode : Node
    {
        public PageNode()
        {
            CssClass = "page";
        }

        public PageNode(PageItem page) : this()
        {
            Id = page.Id;
            if (page.IsHomePage)
            {
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