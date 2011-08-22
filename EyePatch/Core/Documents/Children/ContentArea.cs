using System.Collections.Generic;
using EyePatch.Core.Documents.Children;

namespace EyePatch.Core.Documents
{
    public class ContentArea
    {
        public string Name { get; set; }

        public IList<Widget> Widgets { get; set; }

        public ContentArea()
        {
            Widgets = new List<Widget>();
        }
    }
}