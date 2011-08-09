using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class TemplateNode : Node
    {
        public TemplateNode()
        {
            CssClass = "template";
        }

        public TemplateNode(Entity.Template template)
            : this()
        {
            Id = template.ID.ToString();
            Name = template.Name;

            // meta nodes
            Children.Add(new SearchEngineMetaNode(template));
            Children.Add(new FacebookMetaNode(template));
        }
    }
}