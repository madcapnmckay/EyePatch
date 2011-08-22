using System.Collections.Generic;

namespace EyePatch.Core.Documents
{
    public class Site
    {
        public Site()
        {
            Id = "sites/"; // identity
            Templates = new List<Template>();
        }

        public string Id { get; set; }

        public string Email { get; set; }

        public IList<Template> Templates { get; protected set; }
    }
}