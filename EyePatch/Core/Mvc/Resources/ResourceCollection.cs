using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EyePatch.Core.Util;

namespace EyePatch.Core.Mvc.Resources
{
    public class ResourceCollection : IResourceCollection
    {
        private static readonly ResourceCollection empty = new ResourceCollection();
        protected List<Resource> resources = new List<Resource>();
        protected IResourcePathProvider pathProvider;
        private string unqiueId;

        public ResourceCollection() : this(new ResourcePathProvider()) {}

        public ResourceCollection(IResourcePathProvider pathProvider)
        {
            if (pathProvider == null) throw new ArgumentNullException("pathProvider");
            this.pathProvider = pathProvider;
        }

        public IList<FileInfo> Files()
        {
            return this.Select(r => r.DependentFile).Distinct().ToList();
        }

        public virtual string Mash()
        {
            return string.Join(Environment.NewLine, this.Select(r => r.FileContents() ));
        }

        public virtual string UnqiueId
        {
            get
            {
                try
                {
                    if (!this.Any())
                        return null;

                    var extension = this.First().Extension;
                    return unqiueId ?? (unqiueId = string.Format("{0}{1}",MD5.Encode(string.Join(",", this.Select(x => x.Url))), extension));
                }
                catch (Exception e)
                {
                    throw new Exception("Error encoding resource identifier" + e.Message);
                }
            }
        }

        public virtual ResourceCollection Load(Resource resource)
        {
            Add(resource);
            return this;
        }

        public virtual ResourceCollection Load(string path, MatchMode matchMode)
        {
            Add(new Resource(path, pathProvider, matchMode));
            return this;
        }

        public virtual ResourceCollection Load(string path, MatchMode matchMode, string contentType)
        {
            Add(new Resource(path, matchMode, contentType));
            return this;
        }

        public virtual bool ContainsPath(string path, MatchMode matchMode)
        {
            return resources.Any(r => string.Compare(Resource.Normalize(r.Url), Resource.Normalize(path), true) == 0);
        }

        public virtual void Add(Resource resource)
        {
            if (!ContainsPath(resource.Url, resource.MatchMode))
            {
                resources.Add(resource);
            }
        }

        public void AddRange(ResourceCollection collection)
        {
            resources.AddRange(collection);
        }

        public IEnumerator<Resource> GetEnumerator()
        {
            return resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static ResourceCollection Empty
        {
            get { return empty; }
        }
    }
}