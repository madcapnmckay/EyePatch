using System.Linq;
using Raven.Client.Indexes;

namespace EyePatch.Core.Documents.Indexes
{
    public class PagesByUrl : AbstractIndexCreationTask<Page>
    {
        public PagesByUrl()
        {
            Map = pages => from page in pages
                           select new { page.Url, page.IsHidden};
        }
    }
}