using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EyePatch.Core.Documents;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Util;
using Raven.Abstractions.Data;
using Raven.Client;

namespace EyePatch.Core.Services
{
    public class TemplateService : ServiceBase, ITemplateService
    {
        public TemplateService(IDocumentSession session) : base(session)
        {
        }

        #region ITemplateService Members

        public Template DefaultTemplate
        {
            get { return session.Query<Template>().SingleOrDefault(t => t.IsDefault); }
        }

        public Template Load(string id)
        {
            var template = session.Load<Template>(id);

            if (template == null)
                throw new ApplicationException("Template does not exist");

            return template;
        }

        public IEnumerable<Template> All()
        {
            return session.Query<Template>().Take(1024);
        }

        public Template Create(string name, string path)
        {
            return Create(name, path, "Content", "Service");
        }

        public Template Create(string name, string path, string controller, string action)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path");

            var template = new Template
                               {
                                   Name = name,
                                   ViewPath = path,
                                   Controller = controller ?? "Content",
                                   Action = action ?? "Service"
                               };
            session.Store(template);
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
            session.SaveChanges();
        }

        public void Update(TemplateForm form)
        {
            var template = Load(form.Id);

            template.AnalyticsKey = form.AnalyticsKey;

            // update all pages with this analytics key
            session.Advanced.DatabaseCommands.UpdateByIndex("PagesByTemplate",
                                                            new IndexQuery
                                                                {
                                                                    Query = "TemplateId:" + template.Id
                                                                }, new[]
                                                                       {
                                                                           new PatchRequest
                                                                               {
                                                                                   Type = PatchCommandType.Set,
                                                                                   Name = "AnalyticsKey",
                                                                                   Value = form.AnalyticsKey
                                                                               }
                                                                       }, false);
            session.SaveChanges();
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

            session.SaveChanges();
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

            session.SaveChanges();
        }

        #endregion

        public bool Exists(string path)
        {
            return session.Query<Template>().Any(t => path.Trim() == t.ViewPath);
        }
    }
}