using NKnockoutUI.Window;

namespace EyePatch.Blog.Models
{
    public class BlogWindow : Window
    {
        public BlogWindow()
        {
            Id = "EPG-1";
            Name = "EyePatch Blog";
            CssClass = "eyepatch-admin-window blog-window";
            TaskbarCssClass = "tb-blog";
            CreateFunction = "ep.blog.createPanelBody";
            IsPinned = true;
            Width = 500;
            Height = 500;
            IsMinimized = true;

            Buttons.Add(new Button { Title = "click here to minimize", IconCssClass = "minimize", OnClick = "minimize" });
        }
    }
}