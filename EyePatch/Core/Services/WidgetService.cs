using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EyePatch.Core.Documents;
using EyePatch.Core.Documents.Children;
using EyePatch.Core.Plugins;
using EyePatch.Core.Widgets;
using Raven.Client;
using StructureMap;

namespace EyePatch.Core.Services
{
    public class WidgetService : ServiceBase, IWidgetService
    {
        protected IPageService pageService;

        public WidgetService(IDocumentSession session, IPageService pageService)
            : base(session)
        {
            this.pageService = pageService;
        }

        #region IWidgetService Members

        protected IEnumerable<IWidget> All()
        {
            return ObjectFactory.GetAllInstances<IEyePatchPlugin>().SelectMany(p => p.Widgets).Select(w => ObjectFactory.GetInstance(w) as IWidget);
        }

        public Widget Add(string pageId, string widgetTypeId, string newContentArea, int position)
        {
            var page = pageService.Load(pageId);

            var contentArea = page.ContentAreas.SingleOrDefault(c => c.Name == newContentArea);
            if (contentArea == null)
                throw new ApplicationException("The content area does not exist");

            // find the widget
            var widget = All().SingleOrDefault(w => w.GetType().GetHashCode().ToString() == widgetTypeId);
            if (widget == null)
                throw new ApplicationException("Widget type cannot be found");

            var instance = new Widget(Guid.NewGuid().ToString(), widget.GetType().AssemblyQualifiedName) {Contents = string.Empty};
            contentArea.Widgets.Insert(position, instance);
            session.SaveChanges();
            pageService.ClearOutputCacheDependency(HttpContext.Current);
            return instance;
        }

        public void Delete(string pageId, string widgetId)
        {
            var page = pageService.Load(pageId);
            ContentArea contentArea = null;
            Widget widget = null;
            FindWidget(page, widgetId, out widget, out contentArea);

            if (widget == null)
                throw new ApplicationException("Widget cannot be found");

            contentArea.Widgets.Remove(widget);
            session.SaveChanges();

            pageService.ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Update(string pageId, string widgetId, string contents)
        {
            var page = pageService.Load(pageId);

            ContentArea contentArea;
            Widget widget;
            FindWidget(page, widgetId, out widget, out contentArea);

            if (widget == null)
                throw new ApplicationException("Widget cannot be found");

            widget.Contents = contents;
            session.SaveChanges();

            pageService.ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Move(string pageId, string widgetId, string newContentArea, int position)
        {
            var page = pageService.Load(pageId);

            ContentArea contentArea;
            Widget widget;
            FindWidget(page, widgetId, out widget, out contentArea);

            if (widget == null || contentArea == null)
                throw new ApplicationException("Widget cannot be found");

            var destination = page.ContentAreas.SingleOrDefault(c => string.Compare(newContentArea, c.Name, true) == 0);

            if (destination == null)
                throw new ApplicationException("Destination content area cannot be found");

            contentArea.Widgets.Remove(widget);
            destination.Widgets.Insert(position, widget);

            session.SaveChanges();

            pageService.ClearOutputCacheDependency(HttpContext.Current);
        }

        public void Sort(string pageId, string widgetId, int position)
        {
            var page = pageService.Load(pageId);

            ContentArea contentArea;
            Widget widget;
            FindWidget(page, widgetId, out widget, out contentArea);

            if (widget == null || contentArea == null)
                throw new ApplicationException("Widget cannot be found");

            contentArea.Widgets.Remove(widget);
            contentArea.Widgets.Insert(position, widget);

            session.SaveChanges();

            pageService.ClearOutputCacheDependency(HttpContext.Current);
        }

        #endregion

        protected void FindWidget(Page page, string widgetId, out Widget widget, out ContentArea contentArea)
        {
            widget = null;
            contentArea = null;
            foreach (var area in page.ContentAreas)
            {
                foreach (var w in area.Widgets.Where(w => w.Id == widgetId))
                {
                    contentArea = area;
                    widget = w;
                    break;
                }
            }
        }
    }
}