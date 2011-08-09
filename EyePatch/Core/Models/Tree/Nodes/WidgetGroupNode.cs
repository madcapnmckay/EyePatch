using System.Linq;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree.Nodes
{
    public class WidgetGroupNode : Node
    {
        public WidgetGroupNode()
        {
            CssClass = "group";
        }


        public WidgetGroupNode(Entity.Plugin plugin)
            : this()
        {
            var entity = plugin;
            Id = entity.ID.ToString();
            Name = entity.Name;

            entity.Widgets.ToList().ForEach(AddWidget);
        }

        private void AddGroup(Entity.Plugin plugin)
        {
            Children.Add(new WidgetGroupNode(plugin));
        }

        public void AddWidget(Entity.Widget widget)
        {
            Children.Add(new WidgetNode(widget));
        }
    }
}