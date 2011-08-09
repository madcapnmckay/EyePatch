using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EyePatch.Core.Plugins;
using EyePatch.Core.Util;
using EyePatch.Core.Widgets;
using EyePatch.Core.Entity;

namespace EyePatch.Core.Services
{
    public class WidgetService : ServiceBase, IWidgetService
    {
        protected IPageService pageService;

        public WidgetService(EyePatchDataContext context, IPageService pageService) : base(context)
        {
            this.pageService = pageService;
        }

        public IEnumerable<Widget> All()
        {
            if (Cache[EyePatchConfig.WidgetsList] == null)
                Cache[EyePatchConfig.WidgetsList] = db.Widgets.ToList();

            return Cache[EyePatchConfig.WidgetsList] as IList<Widget>;
        }

        public Widget Load(int id)
        {
            var widget = db.Widgets.SingleOrDefault(w => w.ID == id);

            if (widget == null)
                throw new ApplicationException("Widget does not exist");

            return widget;
        }

        public Widget Load(Type type)
        {
            return db.Widgets.SingleOrDefault(w => w.Type == type.AssemblyQualifiedName);
        }

        public WidgetInstance LoadInstance(int instanceId)
        {
            var instance = db.WidgetInstances.SingleOrDefault(w => w.ID == instanceId);

            if (instance == null)
                throw new ApplicationException("Page widget does not exist");

            return instance;
        }

        public WidgetInstance Add(int pageId, int widgetId, int contentAreaId, int position)
        {
            var widget = Load(widgetId);
            var page = pageService.Load(pageId);

            var contentArea = page.ContentAreas.SingleOrDefault(c => c.ID == contentAreaId);

            if (contentArea == null)
                throw new ApplicationException("The content area does not exist");

            var pageWidget = new WidgetInstance()
            {
                ContentArea = contentArea,
                Contents = string.Empty,
                Widget = widget,
                Position = position
            };

            db.WidgetInstances.InsertOnSubmit(pageWidget);
            db.WidgetInstances.Where(i => i.ContentArea.ID == pageWidget.ContentArea.ID && i.Position >= pageWidget.Position).ToList().ForEach(s => s.Position++);
            db.SubmitChanges();
            pageService.InvalidatePageCache();
            pageService.ClearOutputCacheDependency(HttpContext.Current);
            return pageWidget;
        }

        public void Delete(int id)
        {
            var instance = LoadInstance(id);

            db.WidgetInstances.DeleteOnSubmit(instance);
            // update position values
            db.WidgetInstances.Where(i => i.ContentArea.ID == instance.ContentArea.ID && i.Position >= instance.Position && i.ID != instance.ID).ToList().ForEach(s => s.Position--);
            db.SubmitChanges();
            pageService.InvalidatePageCache();
            pageService.ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Update(int id, string contents)
        {
            var instance = LoadInstance(id);
            instance.Contents = contents;
            db.SubmitChanges();
            pageService.InvalidatePageCache();
            pageService.ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Move(int id, int contentAreaId, int position)
        {
            var instance = LoadInstance(id);
            if (instance.ContentAreaID == contentAreaId)
            {
                Sort(id, position);
                return;
            }

            var contentArea = db.ContentAreas.SingleOrDefault(c => c.ID == contentAreaId);

            if (contentArea == null)
                throw new ApplicationException("The content area does not exist");

            instance.ContentArea = contentArea;
            instance.Position = position;
            db.WidgetInstances.Where(i => i.ContentArea.ID == instance.ContentArea.ID && i.Position >= instance.Position && i.ID != instance.ID).ToList().ForEach(s => s.Position++);
            db.SubmitChanges();
            pageService.InvalidatePageCache();
            pageService.ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Sort(int id, int position)
        {
            var instance = LoadInstance(id);
            if (instance.Position == position)
                return;

            instance.Position = position;
            db.WidgetInstances.Where(i => i.ContentArea.ID == instance.ContentArea.ID && i.Position >= instance.Position && i.ID != instance.ID).ToList().ForEach(s => s.Position++);
            db.SubmitChanges();
            pageService.InvalidatePageCache();
            pageService.ClearOutputCacheDependency(HttpContext.Current);
        }
    }
}