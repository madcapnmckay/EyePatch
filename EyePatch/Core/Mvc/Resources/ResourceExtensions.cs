using System.Collections.Generic;

namespace EyePatch.Core.Mvc.Resources
{
    public static class ResourceExtensions
    {
        public static ResourceCollection ToResourceCollection(this IEnumerable<Resource> collection)
        {
            var result = new ResourceCollection();
            foreach (var resource in collection)
            {
                result.Add(resource);
            }
            return result;
        }
    }
}