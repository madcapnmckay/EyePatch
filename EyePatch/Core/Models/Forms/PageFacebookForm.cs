using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EyePatch.Core.Models.Forms
{
    public class PageFacebookForm : FacebookForm
    {
        public IFacebookForm Def { get; set; }
    }
}