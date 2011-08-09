using System.Web.Mvc;

namespace EyePatch.Core.Mvc
{
    public class EyePatchViewEngine : RazorViewEngine
    {
        public EyePatchViewEngine() : this(null) { }

        public EyePatchViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator)
        {
            AreaViewLocationFormats = new string[0];
            AreaMasterLocationFormats = new string[0];
            AreaPartialViewLocationFormats = new string[0];

            ViewLocationFormats = new[]
                                      {
                                          "~/Core/Mvc/Views/{1}/{0}.cshtml", 
                                          "~/Core/Mvc/Views/{1}/{0}.vbhtml", 
                                          "~/Core/Mvc/Views/Shared/{0}.cshtml", 
                                          "~/Core/Mvc/Views/Shared/{0}.vbhtml",
                                          "~/Templates/{0}.cshtml", 
                                          "~/Templates/{0}.vbhtml",
                                          "~/Templates/Shared/{0}.cshtml",
                                          "~/Templates/Shared/{0}.vbhtml"
                                      };
            MasterLocationFormats = new[]
                                        {
                                            "~/Core/Mvc/Views/{1}/{0}.cshtml", 
                                            "~/Core/Mvc/Views/{1}/{0}.vbhtml", 
                                            "~/Core/Mvc/Views/Shared/{0}.cshtml", 
                                            "~/Core/Mvc/Views/Shared/{0}.vbhtml",
                                            "~/Templates/{0}.cshtml",
                                            "~/Templates/{0}.vbhtml",
                                            "~/Templates/Shared/{0}.cshtml",
                                            "~/Templates/Shared/{0}.vbhtml"
                                        };
            PartialViewLocationFormats = new[]
                                             {
                                                 "~/Core/Mvc/Views/{1}/{0}.cshtml", 
                                                 "~/Core/Mvc/Views/{1}/{0}.vbhtml", 
                                                 "~/Core/Mvc/Views/Shared/{0}.cshtml", 
                                                 "~/Core/Mvc/Views/Shared/{0}.vbhtml",
                                                 "~/Templates/{0}.cshtml", 
                                                 "~/Templates/{0}.vbhtml", 
                                                 "~/Templates/Shared/{0}.cshtml", 
                                                 "~/Templates/Shared/{0}.vbhtml"
                                             };
            FileExtensions = new[] { "cshtml", "vbhtml" };
        }
    }
}