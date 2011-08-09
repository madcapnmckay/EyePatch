using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class FacebookMetaNode : Node
    {
        public FacebookMetaNode(Entity.Page page)
        {
            Id = page.ID.ToString();
            CssClass = "facebook";
            Name = "Facebook";
        }

        public FacebookMetaNode(Entity.Template template)
        {
            Id = template.ID.ToString();
            CssClass = "facebook";
            Type = "templateFacebook";
            Name = "Facebook";
        }
    }
}