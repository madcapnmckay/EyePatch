namespace EyePatch.Core.Models.Menu
{
    public class MenuItem
    {
        public MenuItem(string title, string url, string description, bool isActive)
        {
            Title = title;
            Url = url;
            Description = description;
            IsActive = isActive;
        }

        public string Title { get; protected set; }
        public string Description { get; protected set; }
        public string Url { get; protected set; }

        public bool IsActive { get; protected set; }

        public string CssClass
        {
            get { return IsActive ? "active" : "inactive"; }
        }
    }
}