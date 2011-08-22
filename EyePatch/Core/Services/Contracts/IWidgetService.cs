using EyePatch.Core.Documents.Children;

namespace EyePatch.Core.Services
{
    public interface IWidgetService
    {
        Widget Add(string pageId, string widgetTypeId, string newContentArea, int position);
        void Delete(string pageId, string widgetId);
        void Update(string pageId, string widgetId, string contents);
        void Move(string pageId, string widgetId, string newContentArea, int position);
        void Sort(string pageId, string widgetId, int position);
    }
}