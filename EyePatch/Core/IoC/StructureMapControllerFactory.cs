using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using StructureMap;

namespace EyePatch.Core.IoC
{
    public class StructureMapControllerFactory : DefaultControllerFactory
    {
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            return base.GetControllerType(requestContext, controllerName) ??
                   ObjectFactory.Model.GetAllPossible<IController>().Select(c => c.GetType()).FirstOrDefault(
                       c => c.Name.EndsWith(controllerName + "controller", true, CultureInfo.InvariantCulture));
        }
    }
}