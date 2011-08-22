using EyePatch.Core.Models.Tree.Nodes;
using EyePatch.Core.Plugins;
using EyePatch.Core.Util;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree
{
    public class WidgetTree : NKnockoutUI.Tree.Tree
    {
        public WidgetTree()
        {
            Id = EyePatchApplication.SiteID + "EyePatchWidgetTree";
            Remember = true;

            Defaults = new WidgetTreeDefaults();
        }

        public void AddWidgetGroup(IEyePatchPlugin plugin)
        {
            Children.Add(new WidgetGroupNode(plugin));
        }

        #region Nested type: WidgetTreeDefaults

        private class WidgetTreeDefaults
        {
            public Behavior Group
            {
                get { return new Behavior {IsDraggable = false, IsDropTarget = false, CanAddChildren = false}; }
            }

            public Behavior Widget
            {
                get
                {
                    return new Behavior
                               {
                                   IsDraggable = true,
                                   IsDropTarget = false,
                                   CanAddChildren = false,
                                   ConnectToSortable = ".content-area",
                                   DragCursor = "move",
                                   DragCursorAt = new {top = -12, left = -20},
                                   DragHelper = "ep.tree.widget.dragHelper"
                               };
                }
            }
        }

        #endregion
    }
}