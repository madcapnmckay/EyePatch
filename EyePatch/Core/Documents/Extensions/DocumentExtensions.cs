using System;
using EyePatch.Core.Documents.Children;
using EyePatch.Core.Widgets;
using StructureMap;

namespace EyePatch.Core.Documents.Extensions
{
    public static class DocumentExtensions
    {
        public static IWidget GetInstance(this Widget widget)
        {
            var type = Type.GetType(widget.Type);
            return ObjectFactory.GetInstance(type) as IWidget;
        }

        public static bool IsHomePage(this Page page)
        {
            return page.Url.Trim() == "/";
        }
    }
}