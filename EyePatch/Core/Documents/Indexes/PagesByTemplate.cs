using System.Linq;
using Raven.Client.Indexes;

namespace EyePatch.Core.Documents.Indexes
{
    public class PagesByTemplate : AbstractIndexCreationTask<Page>
    {
        public PagesByTemplate()
        {
            Map = pages => from page in pages
                           select new {page.TemplateId};
        }
    }
}