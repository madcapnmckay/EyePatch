using System;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Security;
using EyePatch.Core.Documents;
using EyePatch.Core.Models.Forms;
using EyePatch.Core.Plugins;
using EyePatch.Core.Util;
using EyePatch.Core.Util.Exceptions;
using Raven.Client;
using StructureMap;

namespace EyePatch.Core.Services
{
    public class ApplicationService : ServiceBase, IApplicationService
    {
        protected IFolderService folderService;
        protected IFormsAuthenticationService formsService;
        protected IMembershipService membershipService;
        protected IPageService pageService;
        protected IPluginService pluginService;
        protected ITemplateService templateService;

        public ApplicationService(IDocumentSession session, IPageService pageService, ITemplateService templateService,
                                  IFolderService folderService,
                                  IPluginService pluginService, IMembershipService membershipService,
                                  IFormsAuthenticationService formsService)
            : base(session)
        {
            this.pageService = pageService;
            this.pluginService = pluginService;
            this.templateService = templateService;
            this.folderService = folderService;
            this.membershipService = membershipService;
            this.formsService = formsService;
        }

        #region IApplicationService Members

        public void Install(InstallationForm install)
        {
            var createStatus = membershipService.CreateUser(install.Username, install.Password, install.Email);

            if (createStatus != MembershipCreateStatus.Success)
                throw new InstallationException("Member creation failed", 2, createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                membershipService.CreateRole("Admin");
                membershipService.AddUserToRole("Admin", install.Username);
                formsService.SignIn(install.Username, false);

                // create site
                session.Store(new Site {Email = install.Email});

                // rename root to sitename
                folderService.RootFolder.Name = install.SiteName;
                session.SaveChanges();

                CreateDefaultPage(install.SiteName);
                EyePatchApplication.HasPages = true;
            }
        }

        public bool SignIn(string userName, string password)
        {
            if (membershipService.ValidateUser(userName, password))
            {
                formsService.SignIn(userName, false);
                return true;
            }
            throw new ApplicationException("Invalid username or password");
        }

        public void SignOut()
        {
            // any other tasks we need to perform
            formsService.SignOut();
        }

        public void Initialise()
        {
            // Check for templates
            var path = HttpContext.Current.Server.MapPath(EyePatchConfig.TemplateDir);
            var di = new DirectoryInfo(path);
            var files = di.GetFiles("*.cshtml").Where(f => !Path.GetFileNameWithoutExtension(f.FullName).StartsWith("_"));

            if (files.Count() == 0)
                throw new InstallationException("There must be at least one template file in the Templates directory", 1);

            templateService.CreateTemplates(files.Select(f => f.FullName).ToList());
            if (templateService.DefaultTemplate == null && session.Query<Template>().Any())
            {
                // there must be a default template, set to default.cshtml
                var defaultTemplate = session.Query<Template>().SingleOrDefault(
                    t => t.ViewPath.EndsWith("default.cshtml")) ?? session.Query<Template>().First();

                defaultTemplate.IsDefault = true;
                session.SaveChanges();
            }

            // create the root folder, so that plugins creating pages can use it
            if (folderService.RootFolder == null)
                folderService.Create("Site", null);

            // call startup on all plugins
            foreach (var plugin in ObjectFactory.Model.GetAllPossible<IEyePatchPlugin>())
            {
                plugin.Startup();
            }

            // check if there are any pages
            EyePatchApplication.HasPages = pageService.Count > 0;
        }

        #endregion

        protected void CreateDefaultPage(string siteName)
        {
            using (var tx = new TransactionScope())
            {
                pageService.Create("Home", "Home", "/", true);

                tx.Complete();
            }
        }
    }
}