namespace EyePatch.Core.Mvc.Resources
{
    public static class Resources
    {
        public static Resource jQuery = new Resource("//ajax.googleapis.com/ajax/libs/jquery/1.6.1/jquery.min.js", MatchMode.FileName);
        public static Resource jQueryUI = new Resource("//ajax.googleapis.com/ajax/libs/jqueryui/1.8.14/jquery-ui.min.js", MatchMode.FileName);
        public static Resource jQueryValidate = new Resource("//ajax.aspnetcdn.com/ajax/jquery.validate/1.8/jquery.validate.js", MatchMode.FileName);
        public static Resource jQueryValidateUnobtrusive = new Resource("//ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.validate.unobtrusive.js", MatchMode.FileName);
        public static Resource jQueryCookie = new Resource("/core/js/jquery.cookie.js", MatchMode.FileName);
        public static Resource jQueryForm = new Resource("/core/js/jquery.form.js", MatchMode.FileName);
        public static Resource TipTip = new Resource("/core/js/jquery.tiptip.js", MatchMode.FileName);
        public static Resource jQueryTmpl = new Resource("/core/js/jquery.tmpl.js", MatchMode.FileName);
        public static Resource Knockout = new Resource("/core/js/knockout-1.2.1.js", MatchMode.FileName);
        public static Resource KnockoutMapping = new Resource("/core/js/knockout.mapping.js", MatchMode.FileName);
        public static Resource json2 = new Resource("/core/js/json2.js", MatchMode.FileName);
    }
}