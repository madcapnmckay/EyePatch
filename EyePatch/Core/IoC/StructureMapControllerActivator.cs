using System;
using System.Web.Mvc;
using System.Web.Routing;
using StructureMap;

namespace EyePatch.Core.IoC
{
    public class StructureMapControllerActivator : IControllerActivator
    {
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            return ObjectFactory.GetInstance(controllerType) as IController;
        }
    }
}