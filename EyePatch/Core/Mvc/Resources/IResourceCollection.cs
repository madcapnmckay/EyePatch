using System.Collections.Generic;

namespace EyePatch.Core.Mvc.Resources
{
    public interface IResourceCollection : IEnumerable<Resource>
    {
        bool ContainsPath(string path, MatchMode matchMode);
    }
}