using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using EyePatch.Core.Util;

namespace EyePatch.Blog.Models.Widgets
{
    public class PostList : List<PostSummary>
    {
        protected int currentPage;
        protected int pageSize;
        protected int totalRecords;

        public bool PagerVisible
        {
            get { return totalRecords > pageSize; }
        }

        public bool OlderVisible
        {
            get
            {
                var last = Math.Ceiling(totalRecords / (double)pageSize);
                return currentPage < last;
            }
        }

        public bool NewerVisible
        {
            get
            {
                return currentPage > 1;
            }
        }

        public string OlderLink
        {
            get
            {
                if (!OlderVisible)
                    return string.Empty;

                var query = new NameValueCollection(HttpContext.Current.Request.QueryString);
                var newPage = (currentPage + 1).ToString();

                if (query["page"] == null)
                    query.Add("page", newPage);
                else
                    query["page"] = newPage;

                return HttpContext.Current.Request.Path.WithQueryString(query); 
            }
        }

        public string NewerLink
        {
            get {
                if (!NewerVisible)
                    return string.Empty;

                var query = new NameValueCollection(HttpContext.Current.Request.QueryString);
                var newPage = (currentPage - 1).ToString();
                
                if (query["page"] == null)
                    query.Add("page", newPage);
                else
                    query["page"] = newPage;

                return HttpContext.Current.Request.Path.WithQueryString(query); 
            }
        }

        public PostList(int currentPage, int pageSize, int totalRecords, IEnumerable<PostSummary> posts)
        {
            this.currentPage = currentPage;
            this.pageSize = pageSize;
            this.totalRecords = totalRecords;
            this.AddRange(posts);
        }
    }
}