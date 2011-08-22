using EyePatch.Core.Mvc.Resources;

namespace EyePatch.Core.Widgets
{
    public interface IWidget
    {
        /// <summary>
        ///   The name of the widget as it appears in the widget tree
        /// </summary>
        string Name { get; }

        /// <summary>
        ///   An custom object that will be passed to page on widget creation
        /// </summary>
        object InitialContents { get; }

        /// <summary>
        ///   The name of the function called when a new instance is added
        /// </summary>
        string CreateFunction { get; }

        /// <summary>
        ///   The css Class applied to the outer widget div, this should be used as the base for
        ///   all styles
        /// </summary>
        string CssClass { get; }

        /// <summary>
        ///   The javascript files needed for correct operation of the widget when rendered normally
        /// </summary>
        ResourceCollection Js { get; }

        /// <summary>
        ///   The css files needed for correct opertion of the widget when rendered normally
        /// </summary>
        ResourceCollection Css { get; }

        /// <summary>
        ///   The javascript files needed for correct operation of the widget when in admin mode
        /// </summary>
        ResourceCollection AdminJs { get; }

        /// <summary>
        ///   The css files needed for correct operation of the widget when in admin mode
        /// </summary>
        ResourceCollection AdminCss { get; }

        /// <summary>
        ///   Produces the markup required by the widget, this should not include any admin interfaces or markup
        ///   This must be added client side via Js
        /// </summary>
        /// <param name = "context"></param>
        void Render(WidgetContext context);
    }
}