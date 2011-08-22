using System.Collections.Generic;
using EyePatch.Core.Mvc.Resources;

namespace EyePatch.Core.Services
{
    public interface IResourceService
    {
        IEnumerable<ResourcePath> GetResourcePaths(ResourceCollection resources);
        IEnumerable<ResourcePath> GetResourcePaths(ResourceCollection resources, bool mash, bool minify, bool cache);
        ResourcePath GetContentsFor(string identifer);

        byte[] EmbeddedResource(string resourceName);
        string MimeType(string filename);
    }
}