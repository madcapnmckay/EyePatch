using System.Collections.Generic;
using EyePatch.Core.Documents;
using EyePatch.Core.Models.Forms;

namespace EyePatch.Core.Services
{
    public interface ITemplateService
    {
        Template DefaultTemplate { get; }

        Template Load(string id);
        IEnumerable<Template> All();

        Template Create(string name, string path);
        Template Create(string name, string path, string controller, string action);
        void CreateTemplates(IList<string> paths);

        void Update(TemplateForm form);
        void Update(SearchForm form);
        void Update(FacebookForm form);
    }
}