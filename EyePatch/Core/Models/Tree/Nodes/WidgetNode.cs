using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class WidgetNode : Node
    {
        public WidgetNode()
        {
            CssClass = "widget";
        }

        public WidgetNode(Entity.Widget widget)
            : this()
        {
            Id = widget.ID.ToString();
            Name = widget.Name;
        }
    }
}