using System;

namespace EyePatch.Core.Mvc.Sitemap
{
    public interface ISiteMapItem
    {
        /// <summary>
        /// This url can be fully qualified or relative, the system will convert relative urls
        /// to fully qualified urls using the current application root
        /// </summary>
        string Url { get; }
        DateTime? LastModified { get; }
        ChangeFrequency? ChangeFrequency { get; }
        double? Priority { get; }
    }
}
