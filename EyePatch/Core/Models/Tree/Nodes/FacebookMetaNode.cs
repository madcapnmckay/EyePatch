using EyePatch.Core.Documents;
using EyePatch.Core.Documents.Children;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class FacebookMetaNode : Node
    {
        public FacebookMetaNode(PageItem page)
        {
            Id = page.Id;
            CssClass = "facebook";
            Name = "Facebook";
        }

        public FacebookMetaNode(Template template)
        {
            Id = template.Id;
            CssClass = "facebook";
            Type = "templateFacebook";
            Name = "Facebook";
        }
    }
}