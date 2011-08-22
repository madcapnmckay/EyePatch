using EyePatch.Core.Widgets;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class WidgetNode : Node
    {
        public WidgetNode()
        {
            CssClass = "widget";
        }

        public WidgetNode(IWidget widget)
            : this()
        {
            Id = widget.GetType().GetHashCode().ToString();
            Name = widget.Name;
        }
    }
}