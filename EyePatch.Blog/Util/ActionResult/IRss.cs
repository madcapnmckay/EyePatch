namespace EyePatch.Blog.Util.ActionResult
{
    public interface IRss
    {
        string Title { get; }
        string Description { get; }
        string Link { get; }
    }
}