using EyePatch.Core.Models.Tree.Nodes;
using EyePatch.Core.Util;
using NKnockoutUI.Tree;

namespace EyePatch.Core.Models.Tree
{
    public class TemplateTree : NKnockoutUI.Tree.Tree
    {
        public TemplateTree()
        {
            Id = EyePatchApplication.SiteID + "EyePatchTemplateTree";
            Remember = true;

            Defaults = new TemplateTreeDefaults();
        }

        public void AddTemplate(Entity.Template template)
        {
            Children.Add(new TemplateNode(template));
        }

        private class TemplateTreeDefaults
        {
            public Behavior Template
            {
                get
                {
                    return new Behavior { IsDraggable = false, IsDropTarget = false, CanAddChildren = false };
                }
            }
        }
    }
}