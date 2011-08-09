using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using StructureMap;

namespace EyePatch.Core.IoC
{
    public class StructureMapControllerFactory : DefaultControllerFactory
    {
        protected override Type GetControllerType(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            return base.GetControllerType(requestContext, controllerName) ??
                   ObjectFactory.Model.GetAllPossible<IController>().Select(c => c.GetType()).FirstOrDefault(c => c.Name.EndsWith(controllerName + "controller", true, CultureInfo.InvariantCulture));
        }
    }
}