using NKnockoutUI.Window;

namespace EyePatch.Core.Models
{
    internal class AdminWindow : Window
    {
        public AdminWindow()
        {
            Id = "1";
            Name = "EyePatch CMS";
            CssClass = "eyepatch-admin-window";
            TaskbarCssClass = "tb-eyepatch";
            CreateFunction = "ep.createPanelBody";
            IsPinned = true;
            Width = 500;
            Height = 700;

            Buttons.Add(new Button
                            {
                                Title = "click here to log out of EyePatch",
                                IconCssClass = "close",
                                OnClick = "ep.actions.logout",
                                CssClass = "title-button red"
                            });
            Buttons.Add(new Button {Title = "click here to minimize", IconCssClass = "minimize", OnClick = "minimize"});
        }
    }
}