using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace EyePatch.Core.Util.Extensions
{
    public static class FormExtensions
    {
        public static MvcHtmlString HelpfulLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            if (string.IsNullOrWhiteSpace(metaData.Description))
                return html.LabelFor(expression);

            var label = html.LabelFor(expression);
            return new MvcHtmlString(label.ToString().Replace("</label>",
                           string.Format(" <span class=\"help\" title=\"{0}\"></span></label>", metaData.Description)));
        }
    }
}