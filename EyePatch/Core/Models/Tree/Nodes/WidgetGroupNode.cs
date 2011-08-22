using EyePatch.Core.Plugins;
using EyePatch.Core.Widgets;
using NKnockoutUI.Tree;
using StructureMap;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class WidgetGroupNode : Node
    {
        public WidgetGroupNode()
        {
            CssClass = "group";
        }


        public WidgetGroupNode(IEyePatchPlugin plugin)
            : this()
        {
            Id = plugin.Name;
            Name = plugin.Name;

            foreach (var widget in plugin.Widgets)
            {
                AddWidget(ObjectFactory.GetInstance(widget) as IWidget);
            }
        }

        public void AddWidget(IWidget widget)
        {
            Children.Add(new WidgetNode(widget));
        }
    }
}