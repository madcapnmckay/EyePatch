using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;

namespace EyePatch.Core.IoC
{
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        #region IDependencyResolver Members

        public object GetService(Type serviceType)
        {
            return serviceType.IsClass ? GetConcreteService(serviceType) : GetInterfaceService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ObjectFactory.GetAllInstances(serviceType).Cast<object>();
        }

        #endregion

        private static object GetConcreteService(Type serviceType)
        {
            try
            {
                return ObjectFactory.GetInstance(serviceType);
            }
            catch (StructureMapException)
            {
                return null;
            }
        }

        private static object GetInterfaceService(Type serviceType)
        {
            return ObjectFactory.TryGetInstance(serviceType);
        }
    }
}