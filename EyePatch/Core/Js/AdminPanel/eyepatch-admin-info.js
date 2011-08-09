/*global document, window, $, ko, debug, setTimeout, alert */
(function () {
    // Private function
    var templateEngine = new ko.jqueryTmplTemplateEngine();

    ko.infoPanel = {
        viewModel: function (configuration) {
            this.data = configuration.data || {};
            this.urls = configuration.urls || {};
            this.types = configuration.types;

            this.loading = ko.observable(false);
            this.defaultTemplate = ko.observable(configuration.defaultTemplate);
            this.displayType = ko.observable(configuration.displayType);

            this.templateToRender = function () {
                var type = this.displayType();
                if (type === undefined) {
                    return this.defaultTemplate();
                }
                var typeTemplate = this.types[type].template;
                return typeTemplate || type + 'InfoTemplate';
            } .bind(this);

            this.dataToDisplay = ko.dependentObservable(function () {
                var type = this.displayType();
                var result = this.types[type] || {};
                result.data = this.data;
                result.urls = this.urls;
                return result;
            }, this);

            this.mapData = function (data, type) {
                if (this.types[type].mapped) {
                    ko.mapping.updateFromJS(this.types[type], data);
                } else {
                    ko.mapping.fromJS(data, {}, this.types[type]);
                }
                var obj = this.types[type];
                this.types[type].mapped = true;
            };

            this.display = function (node) {
                var type = node.type();

                if (this.types[type].ajax) {
                    this.loading(true);
                    var that = this;
                    ep.postJson(this.urls[type].info, { id: node.id() }, function (result) {
                        that.mapData(result.data, type);
                        that.displayType(type);
                        that.loading(false);
                    });
                } else {
                    this.loading(false);
                    this.displayType(type);
                }
            };
        }
    };

    ko.addTemplateSafe("pageInfoTemplate", "<div class=\"page-info form\">\
                                <form method=\"post\" action=\"${ urls.page.update }\" data-bind=\"prepareForm: success\">\
                                    <input name=\"Id\" type=\"hidden\" data-bind=\"value : id\" />\
                                    <div class=\"field\">\
                                        <label for=\"epTitle\">Title <span class=\"help\" title=\"The page title appears in the broswer tab, search results & when added to favorites. <br/><br/>Note: This is not the same as the name of the page in the tree which is purely for your reference.\"></span></label>\
                                        <input id=\"epTitle\" name=\"Title\" type=\"text\" data-bind=\"value : title\" data-val=\"true\" data-val-required=\"A page title is required\"/>\
                                        <span data-valmsg-replace=\"true\" data-valmsg-for=\"Title\" class=\"field-validation-valid\"></span>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epUrl\">Url <span class=\"help\" title=\"The relative url at which your page can be reached. e.g. /my-new-page\"></span></label>\
                                        <input id=\"epUrl\" name=\"UrlInput\" type=\"text\" data-bind=\"value : url, disable: !urlEditable()\" data-val=\"true\" data-val-required=\"A url must be supplied\"/>\
                                        <input name=\"Url\" type=\"hidden\" data-bind=\"value : url\"/>\
                                        <span data-valmsg-replace=\"true\" data-valmsg-for=\"Url\" class=\"field-validation-valid\"></span>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTemplate\">Template <span class=\"help\" title=\"The template the page will use.\"></span></label>\
                                        <select id=\"epTemplate\" name=\"TemplateID\" data-bind=\"options: data.templates, optionsText: 'Value', optionsValue: 'Key', value: templateID\"></select>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epIsLive\">Is Page Live <span class=\"help\" title=\"Until this checkbox is checked the page will only be visible to you and will not be accessible to the outside world.\"></span></label>\
                                        <input id=\"epIsLive\" name=\"IsLive\" type=\"checkbox\" data-bind=\"checked : isLive\" value=\"true\" />\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epIsInMenu\">Show in menu <span class=\"help\" title=\"Should the page be shown in the menu.\"></span></label>\
                                        <input id=\"epIsInMenu\" name=\"IsInMenu\" type=\"checkbox\" data-bind=\"checked : isInMenu\" value=\"true\" />\
                                    </div>\
                                    <div class=\"field\" data-bind=\"visiable: isInMenu\">\
                                        <label for=\"epIsInMenu\">Menu order <span class=\"help\" title=\"If showing in the menu this field denotes the order of appearance.\"></span></label>\
                                        <input id=\"epMenuOrder\" name=\"MenuOrder\" type=\"text\" data-bind=\"value : menuOrder\" />\
                                    </div>\
                                    <br/><br/>\
                                    <div class=\"center help-text\">Double click a page in the tree to navigate to that page.</div>\
                                    <div class=\"button-container\">\
                                        <button type=\"submit\" title=\"Click here to save this page\">\
                                            Save</button>\
                                    </div>\
                                </form>\
                            </div>", templateEngine);

    ko.addTemplateSafe("templateInfoTemplate", "<div class=\"page-info form\">\
                                <div class=\"center help-text\">Templates in EyePatch provide the layout for your pages. Meta data can be set on the Search & Facebook child nodes. This provides a default value which can be overridden by each page that uses the template.</div>\
                                <form method=\"post\" action=\"${ urls.template.update }\" data-bind=\"prepareForm: success\">\
                                    <input name=\"Id\" type=\"hidden\" data-bind=\"value : id\" />\
                                    <div class=\"field\">\
                                        <label for=\"epAnalytics\">Google Analytics <span class=\"help\" title=\"The google analytics web property ID e.g. UA-XXXXX-X. This is used to track the website, visit http://www.google.com/analytics/ for more info.\"></span></label>\
                                        <input id=\"epAnalytics\" name=\"AnalyticsKey\" type=\"text\" data-bind=\"value : analyticsKey\"/>\
                                    </div>\
                                    <div class=\"button-container\">\
                                        <button type=\"submit\" title=\"Click here to save this template\">\
                                            Save</button>\
                                    </div>\
                                </form>\
                            </div>", templateEngine);

    ko.addTemplateSafe("searchInfoTemplate", "<div class=\"page-info form\">\
                                <form method=\"post\" action=\"${ urls.search.update }\" data-bind=\"prepareForm: success\">\
                                    <input name=\"Id\" type=\"hidden\" data-bind=\"value : id\" />\
                                    <div class=\"field\">\
                                        <label for=\"epDescription\">Description <span class=\"help\" title=\"The text can be used when printing a summary of the document. The text should not contain any formatting information. Used by some search engines to describe your document.\"></span></label>\
                                        <textarea id=\"epTmplDescription\" name=\"Description\" data-bind=\"value : description, attr: { placeholder: def.description() }\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epKeywords\">Keywords <span class=\"help\" title=\"Comma separated keywords are used by some search engines to index your document in addition to words from the title and document body. Typically used for synonyms and alternates of title words. Consider adding frequent misspellings. e.g. heirarchy, hierarchy.\"></span></label>\
                                        <input id=\"epTmplKeywords\" name=\"Keywords\" type=\"text\" data-bind=\"value : keywords, attr: { placeholder: def.keywords() }\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epLanguage\">Language <span class=\"help\" title=\"Declares the primary natural language(s) of the document. May be used by search engines to categorize by language.\"></span></label>\
                                        <select id=\"epLanguage\" name=\"Language\" data-bind=\"options: languages, optionsText: 'Value', optionsValue: 'Key', value: language, optionsCaption: 'Choose...'\"></select>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epCharset\">Charset <span class=\"help\" title=\"It is recommended to always use this tag and to specify the charset.\"></span></label>\
                                        <select id=\"eplCharset\" name=\"Charset\" data-bind=\"options: charsets, optionsText: 'Value', optionsValue: 'Key', value: charset\"></select>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epAuthor\">Author <span class=\"help\" title=\"The author's name.\"></span></label>\
                                        <input id=\"epAuthor\" name=\"Author\" type=\"text\" data-bind=\"value : author, attr: { placeholder: def.author() }\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epCopyright\">Copyright <span class=\"help\" title=\"The copyright owner.\"></span></label>\
                                        <input id=\"epCopyright\" name=\"Copyright\" type=\"text\" data-bind=\"value : copyright, attr: { placeholder: def.copyright() }\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epRobots\">Robots <span class=\"help\" title=\"A comma separate list of any of the following, <br/>INDEX: search engine robots should include this page. FOLLOW: robots should follow links from this page to other pages. <br/>NOINDEX: links can be explored, although the page is not indexed. <br/>NOFOLLOW: the page can be indexed, but no links are explored. <br/>NONE: robots can ignore the page. <br/>NOARCHIVE: Google uses this to prevent archiving of the page.\"></span></label>\
                                        <input id=\"epRobots\" name=\"Robots\" type=\"text\" data-bind=\"value : robots, attr: { placeholder: def.robots() }\"/>\
                                    </div>\
                                    <div class=\"button-container\">\
                                        <button type=\"submit\" title=\"Click here to save this page\">\
                                            Save</button>\
                                    </div>\
                                </form>\
                            </div>", templateEngine);

    ko.addTemplateSafe("facebookInfoTemplate", "<div class=\"page-info form\">\
                                <form method=\"post\" action=\"${ urls.facebook.update }\" data-bind=\"prepareForm: success\">\
                                    <input name=\"Id\" type=\"hidden\" data-bind=\"value : id\" />\
                                    <div class=\"field\">\
                                        <label for=\"epType\">Type <span class=\"help\" title=\"The type of your object (most common is article).\"></span></label>\
                                        <select id=\"epType\" name=\"Type\" data-bind=\"options: types, optionsText: 'Value', optionsValue: 'Key', value: type, optionsCaption: 'Choose...'\"></select>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epEmail\">Email <span class=\"help\" title=\"Contact email associated with the object.\"></span></label>\
                                        <input id=\"epEmail\" name=\"Email\" type=\"text\" data-bind=\"value : email, attr: { placeholder: def.email() }\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epPhone\">Phone <span class=\"help\" title=\"Contact phone number associated with the object.\"></span></label>\
                                        <input id=\"epPhone\" name=\"Phone\" type=\"text\" data-bind=\"value : phone, attr: { placeholder: def.phone() }\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epImage\">Image <span class=\"help\" title=\"An image URL which should represent your object within the graph. The image must be at least 50px by 50px and have a maximum aspect ratio of 3:1. We support PNG, JPEG and GIF formats. You may include multiple og:image tags to associate multiple images with your page.\"></span></label>\
                                        <input id=\"epImage\" name=\"Image\" type=\"text\" data-bind=\"value : image, attr: { placeholder: def.image() }\"/>\
                                    </div>\
                                        <div class=\"field\">\
                                        <label for=\"epAddress\">Street Address <span class=\"help\" title=\"A street address for the entity to be contacted at.\"></span></label>\
                                        <input id=\"epAddress\" name=\"StreetAddress\" type=\"text\" data-bind=\"value : streetAddress, attr: { placeholder: def.streetAddress() }\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epLocality\">Locality <span class=\"help\" title=\"The entity's locality. e.g. Palo Alto\"></span></label>\
                                        <input id=\"epLocality\" name=\"Locality\" type=\"text\" data-bind=\"value : locality, attr: { placeholder: def.locality() }\"/>\
                                    </div>\
                                        <div class=\"field\">\
                                        <label for=\"epRegion\">Region <span class=\"help\" title=\"The entity's region.\"></span></label>\
                                        <input id=\"epRegion\" name=\"Region\" type=\"text\" data-bind=\"value : region, attr: { placeholder: def.region() }\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epCountry\">Country <span class=\"help\" title=\"The entity's country.\"></span></label>\
                                        <input id=\"epCountry\" name=\"Country\" type=\"text\" data-bind=\"value : country, attr: { placeholder: def.country() }\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epPostcode\">Postcode <span class=\"help\" title=\"The entity's postal code.\"></span></label>\
                                        <input id=\"epPostcode\" name=\"Postcode\" type=\"text\" data-bind=\"value : postcode, attr: { placeholder: def.postcode() }\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epLong\">Latitude / Longitude <span class=\"help\" title=\"The entity's postal code.\"></span></label>\
                                        <input id=\"epLong\" class=\"x-small inline\" name=\"Latitude\" type=\"text\" data-bind=\"value : latitude, attr: { placeholder: def.latitude() }\"/>/<input id=\"epTmplLat\" class=\"x-small inline\" name=\"Longitude\" type=\"text\" data-bind=\"value : longitude, attr: { placeholder: def.longitude() }\" style=\"margin-left: 5px;\"/>\
                                    </div>\
                                    <div class=\"button-container\" style=\"bottom: auto;\">\
                                        <button type=\"submit\" title=\"Click here to save this page\">\
                                            Save</button>\
                                    </div>\
                                </form>\
                            </div>", templateEngine);

    ko.addTemplateSafe("templateSearchInfoTemplate", "<div class=\"page-info form\">\
                                <form method=\"post\" action=\"${ urls.templateSearch.update }\" data-bind=\"prepareForm: success\">\
                                    <input name=\"Id\" type=\"hidden\" data-bind=\"value : id\" />\
                                    <div class=\"field\">\
                                        <label for=\"epTmplDescription\">Description <span class=\"help\" title=\"The text can be used when printing a summary of the document. The text should not contain any formatting information. Used by some search engines to describe your document.\"></span></label>\
                                        <textarea id=\"epTmplDescription\" name=\"Description\" data-bind=\"value : description\" />\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplKeywords\">Keywords <span class=\"help\" title=\"Comma separated keywords are used by some search engines to index your document in addition to words from the title and document body. Typically used for synonyms and alternates of title words. Consider adding frequent misspellings. e.g. heirarchy, hierarchy.\"></span></label>\
                                        <input id=\"epTmplKeywords\" name=\"Keywords\" type=\"text\" data-bind=\"value : keywords\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplLanguage\">Language <span class=\"help\" title=\"Declares the primary natural language(s) of the document. May be used by search engines to categorize by language.\"></span></label>\
                                        <select id=\"epTmplLanguage\" name=\"Language\" data-bind=\"options: languages, optionsText: 'Value', optionsValue: 'Key', value: language, optionsCaption: 'Choose...'\"></select>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplCharset\">Charset <span class=\"help\" title=\"It is recommended to always use this tag and to specify the charset.\"></span></label>\
                                        <select id=\"epTmplCharset\" name=\"Charset\" data-bind=\"options: charsets, optionsText: 'Value', optionsValue: 'Key', value: charset\"></select>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplAuthor\">Author <span class=\"help\" title=\"The author's name.\"></span></label>\
                                        <input id=\"epTmplAuthor\" name=\"Author\" type=\"text\" data-bind=\"value : author\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplCopyright\">Copyright <span class=\"help\" title=\"The copyright owner.\"></span></label>\
                                        <input id=\"epTmplCopyright\" name=\"Copyright\" type=\"text\" data-bind=\"value : copyright\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplRobots\">Robots <span class=\"help\" title=\"A comma separate list of any of the following, <br/>INDEX: search engine robots should include this page. FOLLOW: robots should follow links from this page to other pages. <br/>NOINDEX: links can be explored, although the page is not indexed. <br/>NOFOLLOW: the page can be indexed, but no links are explored. <br/>NONE: robots can ignore the page. <br/>NOARCHIVE: Google uses this to prevent archiving of the page.\"></span></label>\
                                        <input id=\"epTmplRobots\" name=\"Robots\" type=\"text\" data-bind=\"value : robots\"/>\
                                    </div>\
                                    <div class=\"button-container\">\
                                        <button type=\"submit\" title=\"Click here to save this template\">\
                                            Save</button>\
                                    </div>\
                                </form>\
                            </div>", templateEngine);

    ko.addTemplateSafe("templateFacebookInfoTemplate", "<div class=\"page-info form\">\
                                <form method=\"post\" action=\"${ urls.templateFacebook.update }\" data-bind=\"prepareForm: success\">\
                                    <input name=\"Id\" type=\"hidden\" data-bind=\"value : id\" />\
                                    <div class=\"field\">\
                                        <label for=\"epTmplType\">Type <span class=\"help\" title=\"The type of your object (most common is article).\"></span></label>\
                                        <select id=\"epTmplType\" name=\"Type\" data-bind=\"options: types, optionsText: 'Value', optionsValue: 'Key', value: type, optionsCaption: 'Choose...'\"></select>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplEmail\">Email <span class=\"help\" title=\"Contact email associated with the object.\"></span></label>\
                                        <input id=\"epTmplEmail\" name=\"Email\" type=\"text\" data-bind=\"value : email\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplPhone\">Phone <span class=\"help\" title=\"Contact phone number associated with the object.\"></span></label>\
                                        <input id=\"epTmplPhone\" name=\"Phone\" type=\"text\" data-bind=\"value : phone\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplImage\">Image <span class=\"help\" title=\"An image URL which should represent your object within the graph. The image must be at least 50px by 50px and have a maximum aspect ratio of 3:1. We support PNG, JPEG and GIF formats. You may include multiple og:image tags to associate multiple images with your page.\"></span></label>\
                                        <input id=\"epTmplImage\" name=\"Image\" type=\"text\" data-bind=\"value : image\"/>\
                                    </div>\
                                        <div class=\"field\">\
                                        <label for=\"epTmplAddress\">Street Address <span class=\"help\" title=\"A street address for the entity to be contacted at.\"></span></label>\
                                        <input id=\"epTmplAddress\" name=\"StreetAddress\" type=\"text\" data-bind=\"value : streetAddress\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplLocality\">Locality <span class=\"help\" title=\"The entity's locality. e.g. Palo Alto\"></span></label>\
                                        <input id=\"epTmplLocality\" name=\"Locality\" type=\"text\" data-bind=\"value : locality\"/>\
                                    </div>\
                                        <div class=\"field\">\
                                        <label for=\"epTmplRegion\">Region <span class=\"help\" title=\"The entity's region.\"></span></label>\
                                        <input id=\"epTmplRegion\" name=\"Region\" type=\"text\" data-bind=\"value : region\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplCountry\">Country <span class=\"help\" title=\"The entity's country.\"></span></label>\
                                        <input id=\"epTmplCountry\" name=\"Country\" type=\"text\" data-bind=\"value : country\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplPostcode\">Postcode <span class=\"help\" title=\"The entity's postal code.\"></span></label>\
                                        <input id=\"epTmplPostcode\" name=\"Postcode\" type=\"text\" data-bind=\"value : postcode\"/>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTmplLong\">Latitude / Longitude <span class=\"help\" title=\"The entity's postal code.\"></span></label>\
                                        <input id=\"epTmplLong\" class=\"x-small inline\" name=\"Latitude\" type=\"text\" data-bind=\"value : latitude\"/>/<input id=\"epTmplLat\" class=\"x-small inline\" name=\"Longitude\" type=\"text\" data-bind=\"value : longitude\" style=\"margin-left: 5px;\"/>\
                                    </div>\
                                    <div class=\"button-container\" style=\"bottom: auto;\">\
                                        <button type=\"submit\" title=\"Click here to save this template\">\
                                            Save</button>\
                                    </div>\
                                </form>\
                            </div>", templateEngine);

    ko.addTemplateSafe("folderInfoTemplate", "<div class=\"folder-info\"><div class=\"center help-text\">Folders in eyepatch are simply places to organize your pages. They bare no relation to the urls those pages are accessed at. The root folder is always present and cannot be deleted.</div></div>", templateEngine);

    ko.addTemplateSafe("infoPanelTemplate", "\
        <div style=\"height: 100%;\" data-bind=\"template: { name: templateToRender, data: dataToDisplay }\" />\
        <div class=\"info-loading\" data-bind=\"visible : loading()\"></div>", templateEngine);

    ko.bindingHandlers.infoPanel = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor(),
                container = element.appendChild(document.createElement("DIV"));
            ko.renderTemplate("infoPanelTemplate", value, { templateEngine: templateEngine }, container, "replaceNode");
        }
    }
} ());
