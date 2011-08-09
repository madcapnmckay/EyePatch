using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Util.Extensions;
using EyePatch.Core.Entity;
using EyePatch.Core.Util;

namespace EyePatch.Core.Services
{
    public class TemplateService : ServiceBase, ITemplateService
    {
        public TemplateService(EyePatchDataContext context) : base(context) {}

        public Template DefaultTemplate
        {
            get { return db.Templates.SingleOrDefault(t => t.IsDefault); }
        }

        public Template Load(int id)
        {
            var template = db.Templates.SingleOrDefault(t => t.ID == id);

            if (template == null)
                throw new ApplicationException("Template does not exist");

            return template;
        }

        public IEnumerable<Template> All()
        {
            return db.Templates.ListFromCache();
        }

        public bool Exists(string path)
        {
            return db.Templates.Where(t => string.Compare(path.Trim(), t.ViewPath, false) == 0).Any();
        }

        public Template Create(string name, string path)
        {
            return Create(name, path, "Content", "Service");
        }

        public Template Create(string name, string path, string controller, string action)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (path == null) throw new ArgumentNullException("path");

            var template = new Template
            {
                Name = name,
                ViewPath = path,
                Controller = controller ?? "CMS",
                Action = action ?? "Service"
            };
            db.Templates.InsertOnSubmit(template);
            db.SubmitChanges();
            return template;
        }

        public void CreateTemplates(IList<string> paths)
        {
            foreach (var path in paths.Where(f => !f.StartsWith("_")))
            {
                var viewPath = PathHelper.PhysicalToUrl(path);
                if (!Exists(viewPath))
                {
                    var name = Path.GetFileNameWithoutExtension(path);
                    name = char.ToUpper(name[0]) + name.Substring(1);
                    Create(name, viewPath);
                }
            }
        }

        public void Update(TemplateForm form)
        {
            var template = Load(form.Id);

            template.AnalyticsKey = form.AnalyticsKey;

            db.SubmitChanges();
        }

        public void Update(SearchForm form)
        {
            var template = Load(form.Id);

            template.Description = form.Description;
            template.Keywords = form.Keywords;
            template.Language = form.Language;
            template.Charset = form.Charset;
            template.Author = form.Author;
            template.Copyright = form.Copyright;
            template.Robots = form.Robots;

            db.SubmitChanges();
        }

        public void Update(FacebookForm form)
        {
            var template = Load(form.Id);

            template.OgType = form.Type;
            template.OgEmail = form.Email;
            template.OgPhone = form.Phone;
            template.OgImage = form.Image;
            template.OgStreetAddress = form.StreetAddress;
            template.OgLocality = form.Locality;
            template.OgRegion = form.Region;
            template.OgCountry = form.Country;
            template.OgPostcode = form.Postcode;
            template.OgLongitude = form.Longitude;
            template.OgLatitude = form.Latitude;

            db.SubmitChanges();
        }
    }
}