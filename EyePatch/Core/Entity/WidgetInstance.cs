using System;
using System.Linq;
using EyePatch.Core.Widgets;
using StructureMap;

namespace EyePatch.Core.Entity
{
    partial class WidgetInstance
    {
        public IWidget GetInstance()
        {
            var all = ObjectFactory.Model.GetAllPossible<IWidget>().Select(t => t.GetType());
            var type = Type.GetType(Widget.Type);

            return ObjectFactory.GetInstance(Type.GetType(Widget.Type)) as IWidget;
        }
    }
}