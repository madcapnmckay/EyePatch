using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace EyePatch.Core.Util
{
    public class CacheProvider : ICacheProvider
    {
        #region ICacheProvider Members

        public void Add(string key, object value)
        {
            HttpContext.Current.Cache.Add(key, value, null,
                                          Cache.NoAbsoluteExpiration,
                                          Cache.NoSlidingExpiration, CacheItemPriority.Normal,
                                          null);
        }

        public void Add(string key, object value, FileInfo dependentFile)
        {
            Add(key, value, new List<FileInfo> {dependentFile});
        }

        public void Add(string key, object value, IEnumerable<FileInfo> dependentFiles)
        {
            var dependencies = dependentFiles != null
                                   ? new CacheDependency(dependentFiles.Select(f => f.FullName).ToArray())
                                   : null;

            HttpContext.Current.Cache.Add(key, value, dependencies,
                                          Cache.NoAbsoluteExpiration,
                                          Cache.NoSlidingExpiration, CacheItemPriority.Normal,
                                          null);
        }

        public T Get<T>(string key)
        {
            return (T) HttpContext.Current.Cache.Get(key);
        }

        public void Remove(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        #endregion
    }
}