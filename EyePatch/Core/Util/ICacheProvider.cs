using System.Collections.Generic;
using System.IO;

namespace EyePatch.Core.Util
{
    public interface ICacheProvider
    {
        void Add(string key, object value);
        void Add(string key, object value, System.IO.FileInfo dependentFile);
        void Add(string key, object value, IEnumerable<FileInfo> dependencies);
        T Get<T>(string key);
        void Remove(string pageListKey);
    }
}