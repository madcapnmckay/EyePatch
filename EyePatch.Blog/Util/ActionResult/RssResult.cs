using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml;

namespace EyePatch.Blog.Util.ActionResult
{
    public class RssResult : System.Web.Mvc.ActionResult
    {
        private readonly string description;
        private readonly List<IRss> items;
        private readonly string title;

        /// <summary>
        ///   Initialises the RssResult
        /// </summary>
        /// <param name = "items">The items to be added to the rss feed.</param>
        /// <param name = "title">The title of the rss feed.</param>
        /// <param name = "description">A short description about the rss feed.</param>
        public RssResult(IEnumerable<IRss> items, string title, string description)
        {
            this.items = new List<IRss>(items);
            this.title = title;
            this.description = description;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var settings = new XmlWriterSettings {Indent = true, NewLineHandling = NewLineHandling.Entitize};

            context.HttpContext.Response.ContentType = "text/xml";
            using (var writer = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                // Begin structure
                if (writer != null)
                {
                    writer.WriteStartElement("rss");
                    writer.WriteAttributeString("version", "2.0");
                    writer.WriteStartElement("channel");

                    writer.WriteElementString("title", title);
                    writer.WriteElementString("description", description);
                    writer.WriteElementString("link", context.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority));

                    // Individual items
                    items.ForEach(x =>
                                      {
                                          writer.WriteStartElement("item");
                                          writer.WriteElementString("title", x.Title);
                                          writer.WriteElementString("description", x.Description);
                                          writer.WriteElementString("link",
                                                                    context.HttpContext.Request.Url.GetLeftPart(
                                                                        UriPartial.Authority) + x.Link);
                                          writer.WriteEndElement();
                                      });

                    // End structure
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
        }
    }
}