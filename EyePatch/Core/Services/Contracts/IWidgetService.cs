using System;
using System.Collections.Generic;
using EyePatch.Core.Widgets;
using EyePatch.Core.Entity;

namespace EyePatch.Core.Services
{
    public interface IWidgetService
    {
        IEnumerable<Widget> All();

        Widget Load(int id);
        Widget Load(Type type);
        WidgetInstance LoadInstance(int instanceId);

        WidgetInstance Add(int pageId, int widgetId, int contentAreaId, int position);
        void Delete(int id);
        void Update(int id, string contents);
        void Move(int instanceId, int contentAreaId, int position);
        void Sort(int id, int position);
    }
}