var debug = false;

ep.body = $('body');
ep.tabs  = {};
ep.pages  = {};
ep.widgets = {};
ep.urls = {};
ep.dom = {};
ep.forms = {};
ep.widgetTypes = {};

ep.dragHolder = ko.observable(undefined);

ep.templateList = [];

var logger = function (log) {
    if (typeof debug !== 'undefined') {
        $('<div></div>').appendTo('#log').text(new Date().toGMTString() + ' : eyepatch-admin-panel.js - ' + log);
    }
};

var evalProperties = function(object) {
    for (var propertyName in object) {
        var value = object[propertyName];
        if (propertyName.indexOf('~') === 0) {
            var newName = propertyName.substring(1);
            var propValue = eval(value);
            object[newName] = propValue;
        }
        if (typeof value === "object") {
            evalProperties(value);
        }
    }
    return object;
};

ep.postJson = function (url, data, onSuccess, onError) {
    $.ajax({
        url: url,
        data: JSON.stringify(data),
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        success: function (data) {
            if (data.success) {
                onSuccess(data);
            }
        },
        error: function () {
            if (onError) {
                onError();
            }
        }
    });
};

String.prototype.format = function () {
    var formatted = this;
    for (var i = 0; i < arguments.length; i++) {
        var regexp = new RegExp('\\{' + i + '\\}', 'gi');
        formatted = formatted.replace(regexp, arguments[i]);
    }
    return formatted;
};

(function () {
    ep.forms = {
        errorPlacement: function (error, inputElement) {
            var container = inputElement.closest('form').find("[data-valmsg-for='" + inputElement[0].name + "']"),
                replace = $.parseJSON(container.attr("data-valmsg-replace")) !== false, text = error.text();

            container.removeClass("field-validation-valid").addClass("field-validation-error");
            error.data("unobtrusiveContainer", container);

            if (replace) {
                container.empty();
                error.attr('title', text).empty().removeClass("input-validation-error").appendTo(container).tipTip({ defaultPosition: 'right' });
            }
            else {
                error.hide();
            }
        },
        parse: function (form) {
            var $form = $(form);
            $.validator.unobtrusive.parse($form);
            $form.data('validator').settings.errorPlacement = ep.forms.errorPlacement;
            return $form;
        }
    };

    ko.bindingHandlers.prepareForm = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var $form = $(element);
            if (typeof valueAccessor() != "function")
                throw new Error("The value for a submit binding must be a function to invoke on submit");

            $('.help', $form).tipTip();

            $form.ajaxForm({
                type: 'POST',
                beforeSubmit: function (arr, $form, options) {
                    return ep.forms.parse($form).valid();
                },
                success: function (responseText, statusText, xhr, form) {
                    if (responseText.success) {
                        var value = valueAccessor();
                        value.call(viewModel, element, responseText);
                    } else {
                        $.noticeAdd({ text: responseText.message || "An error occurred", stay: false, type: 'error' });
                    }
                }
            });
        }
    };
} ());
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

/*global document, window, $, ko, debug, setTimeout, alert */
(function () {
    // Private function
    var templateEngine = new ko.jqueryTmplTemplateEngine(),
        Thumbnail = function (url, image,hú©Köª™àÈté1²dPn›-Ypç”{¨/m&9ßesË#-8-ŠäûòÿÅîÇo*ƒú_;ƒCt{·XÜúC sKœ¿qùL˜‹'›{ÂYºLşhïé–Ö”½8U	ôIÒ ó µïä%ÈH)\n˜–³¹İ+#¸(ídK–Í‰²ßõEB$¬K$:A½Ó[´©†fÑ.„@ëõ¸(×dİzş6úµŠÆ§¨".dâŠ¢ú?’ÃILNÑµZsÎwoõk‰ôgÅŞÊÙo;¸óâYFL?®ZâXı£WÑü«9’Ø†‹-<ÈÔ”šDÁ£˜:¹‰[„JÑ§ÍÂ6‚uöÒ`ŞqSêÁ"Éáî¹@=Ò\Fªác«j9µT‰b?Öñš€û?…®Àğ$æ
‹¶Ö+šF˜åÃ£Åê*¾õIXÆuÚpÛÅQ/kõš%óËšb]œ¯c™¬èê›ÑLáó{‰}\[øœWzE8æ	u©7>¶)²ÃañÃ-C cfY}ÎlÀDìôàö5°ô°ğü´Y˜­üb›!WÔ1‰ÕªåÂİ4ÙŒö”±Ñ^½è4K0Q 3½WòT…ÈTÄ<_÷K1oíRƒ8+j×ãÁõÁ.
q±¦?çí¦C+ıPrdJ ÀLZ+²û_ªüEÕ²’?v°íµ±Péi"ıBnÊ]ûc–s–6¶'P}$úfuÄ&áyF(ßÒ0zO1ÿ%SØ<Ã'—‡Ùc}Ÿæ¸Û;×åøš1rN>‹sÖÚ¾ôÖ¬'üäATÆ‹ë “qŠş T¶<{‹®Gj8oHCç-Ê2¯RóÉÒìÔ¼ÔXÖL5!¦"¡ÌS%Æ½ S9Ud(%¥#¹ktÙàl_ez‹ÂGDŸ°A¬ô¿¤úÔO4vsÃ7Xô°t$òø±îˆufÙNEcƒc¦F$ıäÄ&Ljƒ‹ı,>îÂv|‡é\šã­Kdn{'„<Á®áy“Qeš.Æd­’…8>Âî÷šÕvğ½4åA‘zRlÌTo3óòòC{KÓ£99ç5ÇÚ_¥G-Ö×pÍL¦R}GTÿfÑ•†æ¤İmF€·ø•ÀUÂô-›+ò¥T…FÑïh­¥ÍLôØ·eÊÏ?ébDŠ-#NOÙ,ğÈwECyh˜¹ùÛñQ„èZD-×é+1¦m"_ào“ Ï­
¾KÄUJBõqY2ë2Ğ*	Ğó•\¤üÕ2§ˆØGbMùY«ó„Cİf£v`i qöğxl¢\ÉXÊ‘SºÓµùİ[É<ü¹ñİsc9bO¾’ªĞù¶'”¸Qö ¥ğ›ñ±TÑéps4è%Ÿ^‡Å/¤iDş¯ya&ÙzcH'ùdh#“ô×–í^+XÂÌÈâVøŞÿGZ:ÍLMn¥Áí$*üà›,Ï*E7êÊƒlzÂS|1Íõåë!ÇË'à¹‰ ú¾fßõu1]ü?Vqzãæÿo\¨¸z•¯º_B:ŞycSÿg“YpÕÓ¹è/Â°v¶’XQ¬–]-’áï=f'óÔueÛj08màÈ=ÿĞö_Uãïr4oş);jı[Î±|È†mmÎZû­“ ·â-|Oo’Ÿæ‚,Ïy#)'Š÷7ÁT4rÏ#h¯Ñ‘´GJ•ê€…@PÙ¤d¦½zZÿ“GX¸†/:sµ\!Ræ ŸÚÎ*}o%ÑC1Å+æ“ù‘±õÄÃ˜p 	®Zó‰Ë¹Ï™pÃˆêèKê¿nwã»Œñ‚Ú Ûİä°{ñ!­›Ã0Nk—š½9·¤Ì„pZõ¸õ™%¼:¹F§x‚ŸPÈYU%Ş
9WÎ¯¥óOåA})øD@¿QlLËc­ç¾ÍbÌkzÒGUwxTHğ<u”L£µtµóûb×<}ÎÊ÷jŞqÇí‰m!i“_ ª’ lgìGC{^T@ÙŸæÓîğÛ_œÖĞf+Nöã=Vş °sò"sÚ²ˆ—½õìëÖnC Cš¿ 	ÅÂ÷ømÍQE EË•ákü›­?…ıÿ:ÿrèC1õ¾P¯ş¹²ıò;Ò{ÃaåÅ{cºÉL B¥Æú-+ÉEÎ¹‡á>iwr ~µICp:±-°N"6U²Œh]~xü•2Z½—ó%8Œ ÑJW§—PÊMıu[ót;adÆŒ³4IŸË9³ËäLGölòôõ“=ß¥Hbx<½åÙÃ¥œ·4ÏíÿH¦ÖÇAç…‡§#Í‚V>›ÎèšÈèšs¦h¼§ÂæuW–³#]%È:E=-”>ˆÄbØ+zV6…†O_êu¤ó¿{ dUÜ¸yù|¨:*ÖªMM²¹I(Ë¸îşœ0$P0îÙX¶N/qY¿sãœ;²ß.’§(ë°î¨.Ãğ+ìÈ@n›ç´ª\tÎTaª~*ÄaOQÎ»*“r&@CÈp
ª"§ÈÚŒ^¹‘‘›RrÛ¹¾
«y¡cÑŒcS˜”F|c¶J&9—xúBÏa¿«™_ØêÉÇUÛ4É±D¢$â±‡©-§’{»‹òEAÆØ%Ó–‹	Óƒ+üsDsºÌ#œUoÙ‰ÀäÉÈ˜ªæ‘NìijÈİ‡­^Œë F]î×·îúºŠEÁI8óubA½Tÿ&¯¨ºk¿.’At˜qÅ!¦¤¶o.dHåmô1kìØÆ»“¡G½ïÃGÅSëştéŠo¬ŒØwØIk´½Ì.ÍS©x2¿R4&@æÚ=}øø¬Š”³„¾-‰«nâ§µeÜ‰jqÈI(–ÎÏJ’œÀIŸÙA`è²:3é¡YÖ½º¯Å»œk•E!üéÓ]l:	¹4%TœÆP…\_p
“è±âÇûª±Ë^ì	›™'½U5WIºA¶Z]à¼Ä_$» |°2…ÍÀŠë•¿§‰şØƒrFP¥¯nez¢9·öB¾o`	OYáx^ÍÓ|²Vq-ĞXİqTÜÁC¸“Ënıèğáá^Ü“)Z†Hä]p¹XšjBå‚©6, µlèªNÉVÁÎ¾5™ºÎéå Ö&4T=ä©qğ™ÀjÅÑh‹!¡a‚€“}9¹vÈdØÂjëÛyô‚ØÌ¹‚idTêˆüÁû"vYËûh^ôIlGÊçExñ	sojÍoÂ]Œä÷ß=à”×}{WáFû•LîĞ£$Á>¶{QFeYÁÏ†#2´¢ö@”p"½sZ¹#8Ä7£K›W…ÒùláÛ=–¿@Î'÷ğ¾ì“Š>¹¾kcîÿV[Ú*éèNw)!ªÏ­®ä¡—€(’œµ{~E¡¸üO¼!-çjéP+*.{pÀøÕ@Æ%¤Ÿ§%|Ym™âQé–Ë•f^XB¶‡£ŠÆ%]ê4İ_¡aè€ÑÆ˜ºÇñu“K;ã8¾­«cšş,úY¥öpƒ£we@ñŒu&H;O¸ =ÚáLw_Ù7tUOÉ¿ç²¨ÙUï  ë˜€½óM®ŒOB…†{9ö¥`ö©ÏÌÜş±AİO#Ct,ùo\y€\…S)˜hbAú@¥0#‚ ü¢Hb7«=“Æû}ÌƒcoX³yÒºı¬ÒZ(Ëçı™,V‡²Bì‡“ôˆø‰9WÛË´œº<ÇÏ¤'éu	ÕÛY ¼¹ï;¨ ÄëşÀÃ‡Ís]Š€k¢=øæ?oë†´ zlrAQ`|dp–N9›ğt¼Ù‰áøkÔuòŸÒÖì½Îø£ˆKÕ±ƒ¹×äNG†“ÖïòyÉö8—‰ÈBO6…§°„çÔ¨!4öDy˜Ìvx{k€Y¹jt£8¦ÛÔ‡(›Ï­Mï=Õ×q€®š<Šw¨óîíò|ÏKÖÉ
¶ÉÄHE32Øçşt™j§É4Íº‚üj½ÊAó:bã+{7ìÛùŒ\¥5>
à8Ç´7Ö¦Ó	Ú:
*ÁŞÿOÒÁ|‰ÈJ]X!€ÜípÕ÷©o¿„–[õ?¡Y1‚³/<fÍğxWCÊ9­Ù\¢¯„t”\NºÂhzRé˜&sPÓfNÄ’’j”7o-VokÈß5ö²¼tÚ[øâ>vÆÑwÕ«/OÛÉlğÆ‡íz¢jìpeçö ù*KƒvÎ”VV ¯7
¿|ñc	Ù°*\j‡]q©ïÿúVğèrf¸û‘WA¸ö¾-&ÃúvV!rµOôX‘
f„‰Í•™©Oân‰ºCi¡¸ÄX†‘QI'm-G‚4â8‚÷.q‡Rew®ï‹ƒ¦$%ç÷şËš{U|V1ë6?Ëm[“ªë„å!
Iãú5„PØav*fğ'Z}e’A6.gG—4wg•;˜+tf¦»±äüîügH£!„»,íğÄYìÒ [ÅØ~Õå­¨£ü·ûÓÖÄ«`6ò*µ"Í´ÈòW®GÛt©ûÎÑšc³Ó±³uQ4=9R®x^¦É+Uùgd¢qdvs^-—;!ÀµÏÛoÁ6Ğ8*äsy‰ûç2`ú!RñæòĞhH~ËTW’Ùï_ÒÊ{DÓD•ÎòÁ¨Ğ™´lš{ŠRÔéç«Œ:ı´—\º˜¢K%8äôAÏ›Èrÿtj¯€š3½²ÌÆ5'?àƒN– ¸æ¸Åı?ìÇ 9›Éú²NlH>æ kÈÆ=¶|ğßØÂĞNoC©Ê®˜«U€Q ¥ñ"i7D&c/j3€„)¿z‹ÀA?Í%Fş,Æ6Ñc%ü'¢üQ—Ş‰:VÜ]g±ü-–“AÃ

‰ìÀÈıÖ,#~—#´OkJLaš)AÒËá7mùiı’tW´}ß’’¡”&('~]F>â™Öç©1µÓ·ŞâáubÃY¹Ì+0¤ËèC=$LŠ¡@9$­S
d”€ÆÄ£kíşø£ìä){Òpíí2èx(D·×QfÔf£hÍPõmø-ÎÉ².)X3¡»î­„Un/Æ5_G€t7D	˜uä9†êÉ+Ù‚W?“m7a¸èĞâ,–=}ªQ€Z¦×‹XByT«™¸á‘xÊ}Äú0ä`GbŒcíc€ü<¾•C¢¡æW˜,¼@€1ncìV'}hxªöIN[¾9ğ[µíƒ_¬Ö#fâb‹Gá¼Éò™¦Ûxª¦üœ{A=¦æR%'SæŞ
¸ÒôãÅ«Á½˜OŒ…À¯´eûÿE{¸²1¢„ä´ªC.Y”ìj ’[\ë&x³àÑÒ[Òûï«Rö(˜‡ /œgğâáÏ$×­î§xşƒL³¦ô
QËtçù#§¦Å«ÇHÉ°)ï¡)›ÏäŸ§±;k>½ÜÆ³ÊÉ¦Ÿ‹%ë"®ÖÆêoìVúêÁŠƒµ?×ûº´Á7ÍÚÏ°ËPE•Ü¿Çˆ8mäÖYZÀ&+˜O·×x}#­šë›Fä~«·¡Cäe´Ì.Î7m#F2nVàóÂ4ê"øøÙXa@íNY·v«ÜPêÖâ †„|´èoşŸócÙy0@XU@¢5Cx®Î?åU†[´EË øÁÃ²ÚJ¬^ş”Ó¤(¾ŠFé rØŠ\óX±ûıÊ
j:èı=ƒòZós*.	ş‹È¾BØ¢`‹àêpÚÒÁØ	º^a;›ÉG´E»"n]ÕĞÌa#îÃb)õµhßîår§Zü+û¤Ëy “Ñ}Ì˜m"?ĞÙk{2|·BÑ±­}¶9J §0?ı0ıyèÅÕûgMõF½]lË_õHØEKŠÄ|5Û¶c|»švkôóÊ Rğ²ænça©1úÿø	¡¶PÄÃ}§O	 ù&mØk~¿ş»ŒJCEÑê×L=ƒ3H øè3zÒQ2À'[¨‡QFÙµæ¸²RYûjìl¹\
Ã©vk;Š0±^`¿­…‘€Mõ:UyÙ4	nØ]¹"QÛÛ˜#™ëI‘Tªj¼W¥*3zÂòn3°HñŠ‰•’¤şoÇª£•“	½ª˜;Ç-ıWQ@YÃ¹Ì QîÅ	ŒBÄœb Æù]ğf•˜˜™-NÒº?Ëëø5ö-û-1JÉbf±¬v#.â³+¡7ßuLdfeı\/µ“Ùğçà¶åBÿğ—VÖğ`ZkÛÌ»Îg¬ëğ‚¤Y<¡ùÚ…Ï@4)8n_º›v–èJ</›t‡Öc"NåZ¾;sµì³	+Ÿ‚LÖy›Ïï±D¬ñªÑ‰ùãmBˆ2‹ï·ÓYµ¦_ÜZÉ[°*¨‚ÈÁİ¶ÓìÛL@ó¦İqÏy€-‹¿ÿÆò^ìd½hUL|†÷Y–Cfû‘®¦ªX7æ˜nH7ñcÿf{UˆßvÖİpÁue`&ˆ
’‚Ø_Ê1Ü<3[»¯¶W~O[Q¶×êû,ŠVÀ3:¹,/ì¾¹'ÙïŸ~´€3±«=¿ÏX—ÕÍ†Ç¤»mµNßçrBÈõˆŒ©úJ‘Ï“/×xµ°f¯eíê¥ÁLï+ÕgãürQ‹³™bêY>´ÛÎêèwĞê³˜°N“-š'ûÑjÓ‡”9…•M)‘Ó×—	¬Æ	4 G“6B.T§gÉ+55- ¯ZfÒ¡Œ#{R	|üÌ§” $™Yî€—úeùs÷ı–‘–˜¿Ó…ÉÛ¨«‡éõ°INŞÁ½4 :ƒÀ<	ÒRÇ	ºhC”>µ«r¢©Ğ ò¸Î…”7“!¥Ë^1š^&`TÇ©´5Œ÷·‰ÌHK}…Âiµ”–O—­úF	Í´\é†Ã(İ
CŞÊ®*€ˆf²úeë–•¾—>ì£·©ÑÙÊ$&Œ«ñáàpĞ<œÚÕÇ5Ìƒğ!V‘byl2u¤f½cˆÁ…Å»õ,rÔ«Xˆs²)®[ söNKw)Âëy˜m7/Jí§E3wóP ‰ÇÒM„%DŸİô7¬¾qrëh´‹5¤‰ê>)½ª|­D‘‘¿Î±|à°xTŒÈ¼Ş‹ˆ?åÉVbÈ¨‡2«Óf7{Ó÷ï‰…‡p+¼8·u­xH†f‘ì³1BÊ;¼QÈÃprÑ¨bÂ!Y4”A³Êì¸¶…æ`§”Ÿœ,(şŒ}KdH’ÚÌ¨ ‰ßÃ‚±P4ÓQ3ÁÃhIäO¢u ó§„T’ö,k<û®+ıÉ˜ı¹°ˆ€R¹[Ü¸šáÕ-'¤ÖÙ)’>·úÿÜö°v¢Y½<ÇÓc äÛ‰ÆØf#¿–­CÚ2wc¾—jqÃ´_ñ*3íÓ8ÁĞkåxßE5S"ò¤Ü*`w@û“Kê„`ìó3 ZOQÊxŒtbWÈS3Mü†z bŸ¶’ï²€˜¾š—]H¼ÍÏWi“$-^$…]¢úKháyáÂ Îç˜ç¬×n€i¨Ô5Su¶ƒÿIZ?õªˆ|?“›Ñ+İÒ …= ¨¾ØÈp~T\È£³2’˜²Ã&ÌIœ{kc.¦Bz-u†2›Ip6ê·(³Š'Öv;úe9˜Hğ-…|Ö›°ÔlO;|¿I Gj/,İT	ø‰/‡›ÎoXhh¹èvmI.D—æÁt¦A¢Ä \eñ˜(Z>¨¥ÑU7Ÿ‡cÉÊÓ;²L­ë…¥_ˆÉaSî8Pj¸˜¸ûc–‘¾$´¿`P#6Ñ.®”„BÑmv!;×19@ê/\Ÿâqğ¨‡¼eş"íÃ›äàâ°ŠN“KĞûÅû¤Tâ	–Õ’Q-ƒVqz<^Ë9˜
½=³q^åÅ!~L¹¦¶ƒ+xĞÚïröUØš½O¿?mWµ‚íctş·wö%Gğla
MûvK8ÈÅ»Ñ…gJí°BÕw5²¸‚ïO
ÄÆ\ğ.Q!©·hÎ×4ş3iÔ¼'{“I­ŞTjæDìªw	&,ï4Y«¤TxöqEs‘wö{ª[&ÃˆÃù ´!!ämM¨ug±³N%aJ¢ĞÏi¹/|ô'*š’¥à©d?æ†õöÅÚ>CÒÅLtÙ|Gñ§·í!ÍÅ¶‰öú…Ééb/V@ÓßãtÑçëÁ²ŞšMØ3ë< æˆ˜@!Ù˜·åLàËX«	|xbÊN¿ag¾ìà²bX"–lŒvQ Ø;0ÒŠŠæ3ıÂË÷S >¢¶ÛWP'uÅ‚±"QXT¥2ê<?:9X!øS¿3<ùhA2+½Â6lâPXÈí¿N„áv{ŠÀN_zŒG
p½Ót¹üä¶c8‘–vZuet¶Ë#Gø¸7©õğ¹³Wšõ`TK èáÃÏet*ŒïE\]ãm8¬ËCz$xÉ¯;&–Ì ÔŒ}íä˜ŞmAAS×Ÿ Eü¹[GëyâY@'¶dÂX{úUcş½JR±f7ÚF6‹¡Ê¹¶µ` Fî…´údÿnš„kq»T‰ÌE4¸Ûıß¬Å}3£jqö;¹­%{vpaûù*¾ù´L+)9€€‰Å	²‹¨èu–À*‡Ğç9åE–¢O (‚¿±«ƒF“G=p+°À'qT”(ÿ·Jú±á€yÁK—¨‡˜ŞıûÇe“¹ÊğeÊ÷¼{Òá½‹
WH9uÃAJdckÒ$Æ’%ÊEZBğt®åù&ŸpåÉ†2‡Ôb2vÜ(£ÔTtŠÔu„Møö(f­±TC—çùç·%>ØÉ+IØµ‘ºù|]¦YÇyp’ë†g€|ÏA	­Õe+ïä*5ˆ‹ñ´Ê5JU—d
Ò”—€ßµ‡+.¢ÙatQâƒ¤Óê^^ 	™ÔÅŸv\nü· `’£`WÉèü+#'ç×¬Ëc2z/9#ms2ÃšİM‘@HÛÉ˜ÕÛa^öîûO¸ö½àvr\½YØÉÖº‹¿nŞ"ÿŒ(GÜ'»oiğ	şé' èqÌ•÷Hw@¾_QÊ‹í©a×%+S­&Ÿ†;Dx *:Û¶tw^5C––¬7Â°Y,ŞlVşùâˆv±«¿\MÑa•¤,t/²Pàı«~D›ı,ÁU.¬@c3ïN¯ß|Ø(úKBé™A)Ú–®€<XwÈ}HWéMªlĞ±Kˆ^¯ Q4hÎ}¹k}Á6Ç7]í¬^<©+ãÍĞºÄEÓd´ 7•	’á1S–h—MWgŒEÜ» =%h˜€õ˜{ªÅ7¦¨Õ³™T
‚Ï€k0Ì;ÙİÔÌ¡Ä#{IŸ¾@˜Ôï¸%a3…—m:ÖTG2'¢d@±U/·¢8|ş¾ÌQl¾è^ïÍñ·øsc¶LÄ?F§}M©óu¨àà¿5b_Şp	îê-¡<Ş½Õ"Ë&ºŞÛyÎÀvQÀA¼¶j	ÚvÄmõ`ot*&Ğˆ\v [|D,ÜC”nİßtzRù«'…Ë¦
Z¯Í\Ûôä­‘0]Cu¡C™É, Åé•7¯úÔUH‚‘ú2Ô6ÌšRiş‰ó†¬M$£¾W¤øP˜Â,apFÔùe:f¦ş¾©I‡µ‘¬‡Ë¥ê•<Œqâ¿E'X“H<Ö÷¶æ¿@¼èÁ”´g´õWÆä„V+2|ŸHÀ¤OJ
g+F'-ÖÁ>'UŸo#×ZVÏ>>y†N÷¶µiôÂ­@ÆkNhr¥O)y¹A·T7VUIc…–3T”ô‡+"r—%×P?n.BĞô¨r¬~EÃ§6¤#¬®WİÌu$¥49x7©ù¡	Ûe¼‹Xùt÷wÁÜ]^Ìï[Şœ¡­‡Õ¤^[ç"2Âğó0²qkÃ´F«ÆÇ?¡¨~»„3S ÄÅÒªü~0`â@•JOEå-–¢³Sì¿|ñäÚì¥X„Í—Q$
ø D( RÃ[ïœ5òAyªiŞŸï)–D¢ÊÂß5Ìz÷[	=â–‘t÷#¹Ióç÷ÈôRÚ;å	yI:W¯OFÍÍglŞ3yo(¶ß¬YFu‚jwìİ1Ü­S¡•Ê~Â=Ô#ÔhŠïæ
•éldôëJmlÛ›¤°3§ÓhÜZê†ºƒA¯8ßÍ§gñì¶üWYÖlÄØ•İËß	j„DiŠî!Oîe—êg:LcµÈNBûÿ¯uDRòŞµ‡±W´ÕUİÛ †íÄ.$âŞÃº[Ëf)Àå~Hğ¯…]eA0Ûùñ¤LÖ'±úŞfõ£Z¦’ƒbáçıKCK»¯Ï„ô¦Lç¬¤Ë97üI° #À"&Ò›=o%fİaØG4©~U­	[~EŞÄµåÊ|J÷Dä1Lhíàm@dL<Õ+&İÈgËQÜöìWÙ<å´5¥púô¹¾ÕOÕ««e¹wcÊuVø˜7Ä¶şÄ!B4Eâ=º]å¤ƒ‚‘ÉØ#†ÔÙ<|GW6ÒçKËˆy7Ågùª<Ö_óĞŸÓy×“4ËPCnæ{m°‰)Œ*öQGfÿµ±³\’»ŞõXG{ÂwAPñ~á¾¯}w}Âæ‚4’¹ï’djFUÓT7Ì/˜×T—RÍ-:HÛ	Ø¯¹¶®½¤¼˜å3@1Hx^Oåé.Ö&”3–6÷UÖûËG!ÊØCf¦'<ë]>İG‡¸›óî@qvõ.zÎzw(Ewï2ºÚıöÂ=Âpšqq+¼h/ÜÉÑ±Á’aŒm—ã»°2‚«ç¶G«ak‚ß•ÌÊ3…9†ó$Ê(åË6ªX84~Ó‚‡.ş×´j9÷‘àPÅX‘ÅDàã¨FO4òÇ¤0¨áä>¿¢?ÿJ…Äæ¿UwÛîÚ.·ÿuƒºuîIõ‘!‘a6İOúoÉ ŞZ379‘-÷T»‘œÖäE0\£:.$
Y_qtjk#†eÎ`L½-&8ûK¬ÉR;L”«¿=CQ/Õa_5eRı7:‘rYVÁ1|!"¶Ë(¹Å–{œ|æÀ7Sì•¾ªYCŸÕ}ãv£¥ºüß*&%À>¯†…*¦{ÕÕe+FœBÄİ\,üã ºŠW*ÓTe-;=ú(UÓJ¢t½Zé”4¦¦5k/çÉ€)EêŠãøÊÖŞ¦N$Y»6˜Æb4í×0å'&,§pc.l­ÿ&Ë5Ğ-Bh.=K![{w|n÷0ı,¶U\¾û¨µÈU}ºó´pMö{êœôœ Â­zv˜&Àc€¼W:JIRJ‹*<IyÃ8ªí¢/Xö(oÏ4íi=}o&&‡•¿Î
Q»l†+ñ“ÅFñÂ‘o¡¿SÚ×KysâO´û1Âtpwhâ!ûÇtX°Ç-¬ÊP³ÌW(-¯Ğö}r1*¸î§Ø‘¥õ¤uè’Iò5ı…†½1zŒqÏä”6&GKşP›¬ÏÆê"ßé5×+ëÈH~Â¤Èˆ¶_Ç[©-Õ«¾ı'ÃØÿ-<7M©şr…‚’×°‡š£ÆŞ5òy{€®Ğ	VÒ2¥2†ëğŸòƒ6á_şÒ›­ oÚÓòZº¤ÏÍåó&5²Œäy»@pˆYô…!s>V`MFèÈ‹åçF‘Hg£'ğ‘6(VèD…Sú·°}%ÿc-7à=À­°FH¸Sm¯9ì3e®0ë[2¾ÜB.?‰Ò2²p°ëmğu$âZÃŞP?3n5ÛíÍ9rÒ¾}N±&RHm\ş«`æjÑ½Ë•+¾e©Ñ»|íÛ¥9 Ö ÉÚO¡Wœ=¤ğ o >ŸVù)ûìÀ4úZ´7h‚Pz;ğj)eEõÈî…©Oì=Ü¢ÜvccäÚ–´fjYl-ÖV¸K’(v4dkTDı=	U-÷sÈ[JR-xZÍİºÑˆcúÖ_h„ìo™ø±®Çd­M¯©Ä<Œ„ÇÈ7vö[^Ã*,h‘!N^|o¯^'ÇogdöÌQ¨"êøçªòG1hæ^„¥-Î}	‘&²‡¦MwŞaÇ n©-a•Q~Z"
lG•‘-²rxwaoå@O`z!1ØÎêF³©àA‡.Rù
ˆ'øÀY’‰‰>|jJibg•P3(½(×ÉA(n2ÜaÜ|9éÈÿÑá5šÁVìë&ÊE±ÏFÀÄd•üB6_É gòy})nwM’ŠpußXÛ©¼îxÎ¢w§'\‚a™OÒ{lI!RÍ¹º„2F·´ÿõS“v?Ã=Åì‰ªü[À>9fÖ3"©¿&©]Õ^Jã„1ñ_A&AfôÚÇ’Q<á¤Jw@!ª¿ ôˆKOKâğ!Ì$ÌvÊ¥¶4ZÎ„¶ÛÉ¹@8P” eD&µKØ,/ñ”ˆ¶zëf6ªEõv°œƒMÁ?fU›
“FĞ®BöêfŸ¯)H‘QAâÆéø©2±ZÓÁ”-.Kï’ıÔ:Á˜ŠÎÓêæÊÒÈí`øÜÆ7l—¡'E?fjF›‰äÌ€ÃIuÇÖ±-Ï(Í1¶›-9'sQTÛ›‰§@˜"EÏ(9ü.&'=‰úİX@ê|>â¤Ö?nb9»
Ûâ ¦vB¬ }%ê”î ò½u²lE:Ûğ‡qFˆ¡ïwİà=¤Ü^iü"Ÿ¥7šC±u¢Â(
MT]ÉË´0&2b©k)aõA  }	]°¢F—Ğ=+hçsZD¯ˆ)f|fò#vù¾^¢.ù‚x–Ïûmß’6â­_mñ”†œvuµ/Sö‹Gõ8-ËÏØuYi K8uû¼ËÇ°ğÜÊÎpâB=n¥rJqÄÁè(*FVvÒP‡oµ]Äğ`á&IÂñıoCU¨-![û†](×=Úéºä¯èO.‹SA¬«Z$‰  Ä‡Õj.›sÆÌïã+PÉ­ÄNÎÅ…&	1Ä€AhEˆj8ÖA×Çp™<…¬tp|+¾{lñ pµFÆÜúpÊ>Í-šW2KÁ{]ì­KD¿ï…‡R@Õ4IËÁµ†Xn€u¡AøÀ¹äÍc*dŠôk•Fz7§le2ÌÜàÖ¬·Î‡öe Aênü¥qGO®>äCÙ/"ÜjyÈîı$¨–yYk æ‘AŸ~H†¤AÀÍ©½æG(å¶ô›k—ˆÇÙ´VòªÂq°. qSãUÉjÿÀL$5ÈÎÉãÍı
¹„‚ÓÛÎÛ’ßÑ1?1¹K”ßJphj€PÙ$êÉÊcNiì§¿è¢Jèßùg
Jméº:àk  œN,ê¼¤®ì7‰9&¼>O*;‘BMŠ¸&F¸Y–şæ¨`â‚©çç/NİUgüj¦rp&Ä¾4á§î-á±plê“†`ƒd³DÑïÓ§¦ÃÂsZşê¯÷Û·ı5¡gÔëwï¡zx`~ÖBÛ„"\6B²ÿ÷œƒAà€P³Àg\Æıøı¾ô—Ùa”9_çr8&· ¶x²¦ÃŠ‡4;ßÀ¦ôdm=w©(i¯43Çò •jZ+£ºé{ae˜çcœñFâ“<÷Íy„Å‡Ş*OhWzÎ€ô¥¥wÿ{©—Õ­'ø¦ÍºU{»@­mÚà<Üw³+ÔzçÔqN{~Õ7Ì¨­ä·Õî ÈõŒùÛå:F?!ÙW›«Ö€ß›ø®IW‘…ÿÌ éÇÏM@¸Šî®ÖÅ2¤ò²b"n©‚v1ËKŒ®]}ŒeÈ¹?ä?.{b×÷yF‡ˆ¬Úº:ÿ¸–ŸÇ.‘æmZõ2^…4Ğ¢2Ù­õX=õåöWteN†x5fá!Ï]X`0/ÿïg
Ñ*Œº¹LmE6ÿˆÎbËB¨‹iÂ DÇÏ{K¸qxÈûÒ"ğS½<%ĞE?ácø®iäÿ3—˜æHy fde?Æ+z}sÇİ#6ÅGNCÒQBÔ÷Q+ `ej˜iLù0Z³m¶g.RNñÒa¥:¨Ğb«ÂĞ:§Ãt;ãhœ4lÍqëÄhvygÎ±éÃ"R€Û¨ÊŸõİ|‹åàÎä¤‹ÅW>í¨/¹W~ê§óÄ5~Mn;­òñ!¯8/º´!õÒµÊ‡"tQƒ£)èÎ.bAªVŞ’g	šNùrmIoR”õàÊ’¸/ µ4{l“ §Ë÷F²ªèR>•¢ÉPí‘Ğ´Š3^ªÌÊ"µEİÅ•¿ØZµNGÄ2¶¬U_è3Nrr‘ãÎ$a{SûÉßÌ	säVØ“èı£S4#ÿÉá¨Ÿºrdl øBlVg’ªã:'m[Up?!yîÍ•[WµYR]{ˆN‡âƒ#ó&©$ØÇ;®Ä¿m$}KEÆ$à¼#îi³'5“&‚ËÂ´w¢¢t/+”@ª
#»Y¾û½ğ¦‚ÿcv²é °µÀHn©.ı[èŒ÷ã¿|‡dÊTıFêšú ¥ñ$oÛH•qËÏ7œÖSY¨î°ºÜ¼0ıH© O.§[İòş1¡gß?põ(~»+Ìå”ÒäI¬a¹±—?³vƒ@w¬g‰ğa	èõä‘"Åßvè$D¾jlé€“˜ÿñä¯ëâ²E&Ù”™•Z÷ş³ôVùŒ_o(Wvkª±¢‰IİAş.öÏ¡sÅB2Hˆ1Ù#µlOw3úfmsV†{fj£…b]“ä#á!ª>a/I¡qá>½0KßÎ¿ş»µyì¬ô\ tY×lÕÁ!yF±šÔÏsÁXÍ¡t¯ãÑ:­:@? ŠÜèQ¬Và	ëÍFÄbnbE
}X7	5I›n¨2/ñJYõ”¾ñw”dF+ åÔ¼öÏR]o¤¥Š¶}b°{”»µLQnXµ¡^_,«·g€»¦Ş½½ªİë9Bª ‡¼–	öq^òUI¢çjÁò—°ı£Ÿd£Üü5S¬Ô.­FêÈ…øu^­El6x§—æ¡EÌŸeÊ³÷3›²Øm|İÓ[2.pºSÁŞ"Æ7Ú#7Æh½C	!š´YbÁmKUGO¥ ¡X‚ş•&Kdxuñ²(Rb=ùûyT¸ø(ñ 7NrŞ'–¼ÁÚù S­æ˜ Œ©ıåÏÉR·a|¾òÌöØ£Rª•³ñäN£!]]^Soo«]7ƒv=}@ÙtF_ç!—4\õ]×½°Uu¯“H.ö	Ö=¾0³jNı#BZY½Ÿai*m`Ÿ¥Xzïù÷¢±
ê°é\KüIñâkX’!zÙ{šˆÌÏïß™å­ÜföÒD*ÃİøáéîÛydzõÇ¸ÁğJ¾½†|«•_‰ŸëU0¨;JÒ¥BØ¢@ı,EA{pî•Ëƒ¡‘àXT0·jÉX¿74w(Ø<ƒ‚±èŠ#ó,HöAÄV¢gÍŞ+î7Á`šë†¶GëÌ¢Œ·‘ï½x‰°Èe¤ÙÊõuŸe(7xˆÉO
¦î´ªõÃ	FªsÚ›Ë˜ƒÕ;?0ÍÉ4™¼ û*ØÂzÚ¢‰cBõÂ‚gÆí!CÜy¨êÍô£å„?œµHN
ôïº_¨­_„W|““`Ğot‰‡¹än‚™=«Šeõ öŞ“T„¹?.ÁfB:Ëj’ID*ü°¬8úšßø‡´˜¥<Ğ ‡ô±ëd˜ú3“ö4˜ºq Šèw&É_W+oÊ¢Àp€ÏBçSïKõ”·§÷æäy,··s÷†É‡©‚}¼¨"M‘‰r@”+4;úøØ*eÇBÍaq“¢’{£ßĞ…¼Ğn»+3Kªçh0]ÂsCÑ}@ùxÅ´¯zng€¶4[©Ï"Î;ş©ab?õªo‡³ÎÚ.¡r¸áPÔÛrÀS9º½Ç:/Å«IIR€±nÖ ğƒn™‰Ü †=Ä4Èl_A}`âx•îˆpíyÚıÑî<t½ß3™<CÚØx®Iñ¿íÎ	4½,x6hXã¯8T®6‡vÎÍâø*î>À'ªQ˜A¿-õJj¹úŒr|Ûˆ4£Ô‹&“Ûu^¶imæ‘4$éB ½e>?Óñî¶µÏ‡½ó¸G0G{å»X ­_[vrcuß.Ş³¥á,¬¬â*\Ø‰zÀFŒT-³‘wÇ“ÿbUáBgm³İ+··ç¤ê½áù3T·Ûq&<|¥i.@(ÜoY±+3‚@5$§–³/P8†!'—rfnDšÚ‹] måşU&¦Æñ»Í8r#Ò™'4¦È“é(ÖÑÍ€¢è€o@f_Âîµ%rÈ?h›°ñØG“0‘Wõ'ÿ.´üA'Ö*»Ïv·#nT‡- Y±gı³@.6™ÃCÅ’¬D›
‰ª	XğåKÚo-xŒM~Äà`2ÿs¶µ=.IoÆ¤œüæ¢èB7…SöÊ¢ˆZø?=¸}ôf®X–ùüGÀN8vWˆüÒN>8!Ûx¤‚7rT®cOŸz3¢ï Ù¡¸ì¸,ò`'É¨CÉ¢'7Sú­¿…sáCØfVú™¼<#×\…k~X~Ò'_e±Á(‚go¸¤¦&UFÕõÅ´ûªßU—bŒuä™yÂœz&eFi&Œ‡ZTföÊˆdäxÆ••¸Û÷‡TXª+'}Ï!QMM¥WÖÉš÷-&øk”réúœmGğšè
Bóàôè¥¥6ûZiìV™ .ÎÄa>½’’`Y[™iGİ:c…Ù×ûr«ÖV}‡<”æ[fïEÑ;æ¥ªî–-„XÀ!áÍ:©÷Ä½øö¥Cà?fZçÛuøØõU!™4… â#©²Õ»°æIjŸ››	š;îùUÍò4n›Âß¾ÿĞ`Qp›?@!©·ƒ§ ‹Í]Ëè›€1H…‰š4Ìÿ–§|ÃFÆß³×ÎnA‚hêÊÇ
ÆÃÑµš›cúßÆÖ¦ÈîjZSoDê=Ä„ıë-Ûi4„†˜Ù‹ ‹¶¥HhW6îmùxš
clü Ÿ7<‰³Öv¢ÙÛDIß!İıËß²cÂ¥Q¥0~òêİ.0!ÂÄr„â;@e0Î74ÔîàÍ$ö˜„í”2bpÉ¥÷‘µE05b rúQ›qL%¾ÀG®³‰¥e¡M‘µŒL-º©õB¥_{ğ«}i™ïÿaEßÿXƒ•GƒöRKµ‹ÖÅa¡ô¦¥Éî^4U8§¿ÉN=ùÙâœ¥L"™sÃò&'*N´•€v[É,l„B5ŞV¸‹2·*|t¯uíå½2¥ĞÇEê¡=(:'´³t‚&‹)€1¿1˜x?Zƒ¬E: U´>+kp£2<d)Ïäñærtš|òO»Gä>ñZ?l-¢{ğ)òLÅäÇp
µ†ö³»–C­ıB8ËYÏŞâ’³±DÄ¼JäPªıÓp¯ş½`ù3*g©%aÖ¥·8ÚYVŒ¨Dz¾\rÃ@:$…hCã3mjsÒ´aè¶®êÙ¯î¯ëmëDğÑ˜Ÿj¤±Ö
Zº2á":2sOCÅ‹ÌÆo±\JdÉ>8Wú®ó¡ó>b8Pò[Q+[ôÁjm}ZœÔò!A s­Å86³ƒ~¶^Ú»³pÚ¹Äª©å¢U-à”mÃLtÙ´vî:–¨¥[×váòcÛ"|Gi•^Âyçói¥ß™aÙúÏ³jUæ¡øÁv®;væ^ßG½ld¶İTˆFù77CSƒœÇ–îÜ¡ÇğåJñEL¨1wÌæ¶÷ş8Aê…„@ œğ•EŸ€ì|ÅÉ§Ì(&”)¦´c×êÒ[¯ ¾W
ã³zLO'œØ•s,uëÀ<N¥Ït¤ºç[j¡”ê|ê[…EQãít‘­§ç“â‚#|Ó( I¹1{‡€r\ê¸Oš3â”¹\­¾Vâğš½C9…waÏvÎmêø…•ó“%Êëıø×j4ÕA¸ïe‘jT
#Œ±aÛz”J©.§æm‡æÒÃ¶Vf¯ım“×1gDå`ĞÚòhÕÎB¬gü=ë.Œv˜Èô3W1çPÉF+`¢é"Ğ›tš¿7®r\Ëô>ëû|Ò·Í¸ƒ¼¶))Šæv6XºNrtK–Rˆƒ~=m‹”A«XëT^”EÈ2Î­ÌŸ™b$ƒç
*' (i¦†[›Ûñv“9„ L¾—<ñpŠ«2£ò¾Ú¾ê½õÅ^oŒsHrıW‰7¸³
¡üÖª(´Üñ?´ó˜Ş¤õxH¬–‚°õ§’—š³×ÜÔÂ3DÛÌ‹®;Åôí”~G+ıòŠºAïœ‡Ğ7ßfÌ3ßı¸m6Rí°€ÒÅoµØĞÒvó¡÷”œy½	³‹§N§V'»z“y”Ë¦ôÄgÃ›6H¨ÖWÀ´â ìùæâ7M²9$S Æ±åîØŸŸõPÀõç'ĞÒr¦¯yôNÒË¾ÿD$dj„
¡Çä,{ºä¶ípåÊÕEtƒa^LÁëÂªı­!½9S r†Á2Ú8,óqâ•É^œ»¹îãºåËÙô¼³«_ÈCbé¢†+lØùÄ‚õ`†¼¶½ÏKo\¥èíóØNÊÏ/Ç¦x&‚f€÷º‹g8®:b€Yx¨F1^’r ›uÈ~´¤^üM7Øã Vóáy<aï<Ö Ô½Ş¤Ú ZÉ.™Lxš.ÿâ„u]„½­:°×·¹ê›I"dôcé‰ ÁGÙ=.Ü{Ä\'ër0•’³D7ªÚìû¸¡…Úª®Epqºg·1ƒ
+MÍörTi;›~|Î©Æ"ï´¸©ßäR³ğä<S"îÚnNœrPÏ¾³¨ø·`-sIoá¶¨0U_:vù9'[À÷Í€b%Ù	ÛWÄ½sÎ‹u{Ïı¾&!T7geq‰¼ ´ÊRÓÆ?¤0.çÓQŒPÛ?İ&MŞ‰ÊlL~úIäçÓø0öù»UÄ–ÒŸÜõ²y;^åoæ³¤õUK¢%(î"|sîâ¿=u,¬ZÈlèÿÉìµ¥Ç¬?JàAËBÔÚl|ÍÃH+5—SƒÏLïõ ˜ì½®ºZİ©Ş. IÌü3èÏğ¯Ï
ŞrbõÆùıaõßC€Œ»ól£—7Æ:'¼ğÃcJÒaÔ¹i^ aÿŸ}«rcPÎ6oº*e’L·kåjFÕ^-ñ6Q½*[_6TS™fM«å¾DàšZ,OÚŠ}ÖÀ‰g‹/bØ ¤…bDI<nÁ…T4gC|íÛòŒßg®_­¿ dİ/<Ø—OÆ¢)ƒÂÆs± õ¨2/3§Æ†/ëìüVœÍŒã$m°}ús-ÂÈ€§àß€•èÈ´à0À5#ûøÍ¹ûtF±ï.XÈf‰·#ÕµŞœ¯b­k?eQüÄÒÆZœ ‘NÙ â²ÑOJ7c®µ`úêˆŸ0hsÊ¡V¯'¢W¹Ğ3÷5àgLTÚH»(1JhĞ«mt„ `yixÚÉBÿi÷òÆcÎ¯!±nÏø•¾øÍP‘È„e”H@¯Ğ9<Ÿöâ”+p³uuÇP=ÅºQoùëÃ[È	Ãµ›’ƒ;ÎX	– ²n‰ª¦„~Ãn"AuÄ5YäîU˜/£Ôï–µşt$‚ÉjVR.•' 3¤DFklÙ(ìäwz  ú0ÍŠkÌÙ‚õÊyü|¹¡ÊöŠ>5yU$HÌ¦¼='ju"”~3;Ûà±0„ƒ2hÓ² §¹v—CÔ“U\™¼Äõ´¶òÌH‡·<9£Î)=bÚ–J¥¤Iuâhñš&Ù=:ZPqzBÎ¸ºøá½šøfØ¾ç¨rÙÎ¤+ĞğT$37ÊöÙn¤4Vc”oêÇá±`rŞk+ºå1	ÆZ¦ó<»öôõ$pû¸!‚°vÄèÈÑX’¹ø4ëcè76C“p¶xçqn{©h×To>M¿íf=-ÜRüMà'e•Wõª>’
‰‚­»¼hg¯1ÁÙiªıŞ´~Œ´-Ø€+ä ¢B—bØ.“ŠÆ~k„¼SÓOs8*5Ó^AğöéÜo"±7µğWøwÕhÿ aŞÔÇ\€C§†9—Ö‡7¶ÆBô­]%t´só@üKã¸3CŒk³]i|íÓ°èJ…\~!MQ2Ç¢yš†ŒµZêşèµÚÍwˆ>ª¸[ğ›Ş†Ò›W‰I…jò¢ª¥Ï’ãùjÀŒ´]}jéœÖòøûãmÄ·ş¤ş AW±2üm½=ïB-O›¤I:b>;±Äàƒ  ›îW
Üâ_mmöüí‡eë‘%Ì’2Eóá]6’­®g™Ö1Â ‘I•%ráÂóÉ[ŠbG;NÀ”ª-.ÙÖ{GÚ€÷„4X*ñÒÌ¤\(·©NÖBı8TšI—g[À§Vv½Å˜·¶,ğk›|¬ã,AWé"3ÅQî¤ñTÿRh ¿‡Œ¾…¥±b½NVµSÏ~×Ë<X%®-Ò·˜A"½FğßN)o²ç÷²¹^Çä¤R)Œº‡a¿*V	¯ÊFhØ(ê>ÄàjòI¤º–·³ê•-n!ş^Û‚È= ÈeY½µR@^~É‚úëcz"–ŸïD1šQã’õN[ÎÊ{ÄğëuÓ–¯†~fz±A3 ³aor¼ïÒG1º›ŒCx²jU}a"Â´æ½˜`¥¨’P**ë©ñ“JIùoüviJÏ°COxàk5«X‰#sC´°Q—İÿU(Á%=îë¡'Š1x·5i¼wùi'\`‡ì ^MÚN.ô¹Ãs¹Õı‰-_~æìÓ$™ÕhX„£­ı)zpÛóM|£hh&RJlÂ˜®ğÕ¸dûÅ£—ûÉÉSàx¦@ïZj}GQåİ‘œèo‚ëšïE«oª#5rÓ°$B#L—h<jC«–Å~le_:2Uşä/_Nª; ı¡-,¡Ö5#ê:®öåŞBÇR •Ï<Xiû9¡²IÙ[^0À‘$!ˆA¶&£ym>2î'¤Í…á {ÊH+j\*Ç«O…Fê!¨÷'Œ¿­íÊ{ğóuOª¦x¬é(ê¿¨×ä‚Îe|\[~l?ˆ"»²M~é¤'ÙË	f á5­h•4-©³ŸEMs5$p@%†út–P{¼Æ5Åmù­ÅÎ_O7ïŒPHPï V–¢öİ¶•*4‹U9RS:eÜÑõ1¤^ù¼…?	¯	•‰*úqZÊŠU2‰éf¥nïÍ¬ûOu™İ¯Ï Â(¶oW(qÈ$:6á4…Ê9QFuÄı§™WŒÃ«Ç7½,š´šÔ=q²qTÔıQp­0sq¾§İröİèï×²¶O‹v¦İ‹|Æ,kê4ÀıPÉŒı²u‰#ãJ«ÃÅ‚«¿½ÂÊ;Nè	«|t¸©ód%y€
^ÕàrñN‹õ·l.7æĞRÎ-ŞOV¬‡‘ZY§Ó%"£`Ğ 7-J&â1c“›Ó«æ[†÷Í³*}óË2ŠnÚéûqÂI±eŞ€«2vş.6şûø‚‰ØÓ=åÃp FÎr]ˆÀÈ.oÖ²òÃ|†C© 2N5Ú#DxË-6t¾)ÉäNjmÑD—nš™:wcVïeÒ¢š}C“ONÙ€1ˆÆ0-©€Á'_4>Ú…TE©Z{§áÇp³×¤yØV¹"MíTÈƒ8u#r¶½e‘é²îlÉàTx»3¼6HôlÆòüizŞ•(ï#ØñCÅjö(–pĞANš¸ŸqIùˆ£ f†êvÏ¾Ì+ú¦W	ºw0¿	bFSUš@;Á½é+ÔW µ ê¹†ÊO¨N… ·^á½ïXˆä‹ù¥7Â
‡Á›Úu˜pıpQŞ˜×Êe`FÑëŠ·…º
fÅ(Ó_*.¼q6›°ĞÄ+'ô"Äœ¢À=æû™5¥L`2'"…v©x¥ÿi¶Ï} ;Å”,}J6Å‰^´ı`Æ¤&Ïr0úG÷¬0 j•ÏBIiGÆ&SZ~Á»6¿ª§¿ÒŸ(5w®ôµjúEàbóÈu¬·UiŒ-x¥ …1gé¡Œöz¦4CJ¾tu'fˆY“¯#/{¢7¶Q¬wÃ2»HfèÆ2üÆq–HÜ6¿ü¤7wì¨å]šBs¬×D½ŠpşŒ•qvõBçÇÖtÛ¿Ø×i‰<@x¼>¥®_Ï‰öd{³·Å,5•Ø'ş6(R¿!òõÁ÷¸}u»Au+-ı\n€²5•NÊdböÚßËëµdŞR¬IÀå™²ætMÏ³–ÑÎD}—µı0‡áÚ'Á™,-	”ñO¢aaVxÉë(œŸ…?b(ïÜ{3§Ej%ÍÏ[y_ò™J7¨@WR±#ÊÓõ£¸|Íù{I}ˆ³râ*t»'6µõ5jŸêç@oyäª¼[·¸Spıı¼‚Kø¾çåË-ï¸÷â^‰%‰X:grœœ¤&á¸æ	¾k\Å¶£¼òiŒQ/Ì ZÂlÓ0“Ta,ÒıxÍh°k€{ÜBõ6)ñDì6ØˆïíÛ|GzHíj¯¿ùé¾šNFË-ºAáÚ/;v/õûZN’(K¯†Š‡ùİ÷ËÁÛÿe%x¼¯A˜=ÿ@Îy…<¹mìœ–’$™-$ô[şçÂôá…b<fiĞ3ffëKŒ÷|–»…;8(ÙÕR+–8ZwÓİãcê„çjœîÛÖrt
a£¾©Ryî¥•›â6I³«OİsøEïPõ
;zÚ'sP¢]Ïs6Á0“ü“&õUäáóÓ<ˆ 1Xõ`WÄŒºPÒËªe„'ºNŠÂF´­°±OTÑ½T8É~ÆbN£Úõ–jlwyËWe•`R#ÅÁP—,L¦µ–ã(yÏ¯ê}"áM Ks6Ae7áÒ“O½ZI±pZp•5¿ÄFšªm[P<¶{ò;Ú!Âİ÷÷}¢üš×ñÑ¡Zÿ™N·©øÖD_M?kã}üŸ\÷:lu%éâÿu¿ÿ‡ŒÅFN“åœœÑ_çG2EhO8}qˆ¿’Y&{—´	Ÿ*Hİã;˜ÁâšË_&{‡¤ãšÛ4¶Äšœ™±øÉñ3•gIbTÈògé3ê  âd¸BQÆˆø™fàÏ^LPpı…ÛGËåö†Œ„ƒóG¦ªª(ÿ:{‰hñßQ!H]²İ¤Ìfq\ğı\gsÇĞe2İ'peõÉt¦8Ê•öÿ)>
ãl%}ÍŞ*iuP¢Ü¬ÑıùúD-æíwCú!²um'ZÖé;c±&¹Åé›ÜC[ıÇy±ä@=Á®Z|êè€øNhô‚—ë\n^:¯>€õP’‰±ÀşòBÊÔf3‘^"œÑ]±²\R42M!ı'u˜ŒH0y^¼g”¶©¥Î×1Ç¹™	”;[4\Ø´Å”‚­İ·©3{³VjÌî-‚4+êŒLİ=AO‰æ©ÕST¤ñëaoå§{FJÁ}à—*Xiï7ï‹ĞÈåœ#b-GeÌ~‰W¤Æn£œcz&L!‚^¶ QÚ-›šœÙÀv.’ÎO\øïg¤[ƒÓDd*CIäí¥G–ğàô"/¹gQñÓ%‡İ9<|ÂÒ&ãÖæ¨ëGlŠñSœÌÃÏ¶§o} âlª§VÂ_¼t£Õ@cÉ?ĞMkP*• 5¤•ÊŸòUÚódòä²Ú‹Å¿6PaĞ¡´ƒ™şfd€s›×W·0"UuKëV}zèor-Nê Ğ3ÓVÓ¤‹xu$whY¾*ÏÉŒ¾•‡ÁRÄ ½ ¤'o; ÓÉ”\F~‹ÖÓ  m)‹ÕXÁ‚Øœğ=c	Izêœ"W—jtD¤Áe—¨[<ÎzV–®MÄXîX8ÎE±¸Ô¼´‡°(Z‹¹ù	­{ŸÅœQá–ÃFùÈğ 0út°ï=“šëú)G¯Ê,asB›yI½·BDW<èTTGˆÓì	ğâ	".|Õó÷RúÕC²·ï€Ï¡æl]Ìæ­©Mw½¨åõáã‹}Œ¦‰éK¥d÷/Ï"O”b:EA•PGD¼…fòD “FšEÃ——/øĞ¿¼4`áÑ¸…yy µÖ<–g6cÎíˆx]fÿFOÎ7Š`N¿–ŞJCÔÆGmæEù/W*£_†±åŠ¥4ûÂzW˜tıY(r‰¡R §¨ï´3–Â¶ZCØ ÿt(bï–îZğP3.$ËãUr‚š¢ã¹œwË†<WUBÌ³Ö¨ˆ!”ŸfÃÔÇ<KšÄWĞí­„3§ç]2;(\ÍÍˆ^jôôõ¯@»gÛ3°›¨:ÙÕö&µ°IƒBãögùPÁ¾]U{"õ¾;~ÿGrb™©Ï×«©æèxn^)W–bv‰¢ßJšsmÀô¿?Rw^ê5óœ²Ê¢ôÔaÙ4 –zgeå òJ>OŸm™G’V{ü8ĞVíERrâæ®æ^9`O¼éhg|ä(HÂ0ÔĞ{¢`>2úÀRıŞñ?DVb¡Õ=Ö·“ãüifGYÕdå"”ıÂ	 8Ô|Å”ªãõÙÓKƒ•‘÷pÕhiz^eV¥ä*¬çb^ÜYUì„ŞaOïT£)äOU×{ZÛHh„”ÇBÕö¨,á0kD¦3úFô1$ÃÙ\;
'(%ü?è¨[?›ûEsİ;íö{õÈ3»ß…	ÑõˆªÌ rå¬ícZB¬ÈÄìPß9÷‘5ÿ?TœVVÀÔœ	D£’['í4ÏøÇj½E×Âæ@ïÈ¥dıë¶öD;Ò‘b`†û¤64ôğM¢
,–aÒ6@îì‚£¶Äë>B>|§ÙÌ{«:2V|ÒÒ3qÊíU9ôÅ;:ìƒ—È¿ôFĞdzóŞœúªª |,1°ë†ª®Y Ë\WX´¶›ü-häK¢4¤Q‹Š¨Ó?œĞ7Æ2(—p^ó€”6h©È°¡#]ëxV‹lÂß¸ıö®Ì×Ë'Æ$“t”Âdkªä%Ã–&Â‘Ï.&ÖCœUçE6ÄšƒqõŞXZÊy\÷y‡†sù
Æ÷à
v<°–”>Äú²Y‘â+1·£×´[vd›ÖúÀ0çoNí=¹÷öœĞ„İ÷6•´¢¥l‡_8JÇ.ÜĞ""µ½ßÜ?ºÛ¬ ¾RÖc‰á÷ø¶.?nÒ7¬“”ËÜ	§Õ
ÑÀÖ/TjlÑVZÍËRè—me–Î\èÉÉ«Ìâ…15Öš¥h–Âw´Œ?ÿ
.Üµ8EãÏ
^o+AÒ'—ãÉHmp@XU¼C< dœ#CôäÈe\ô?Ë¡H¸_F"=z$ëãB*ÑlƒÃ´¬D?¹·åS¾«õ$”zàCHº â”PòcgD*ÃêM¡Zöáuªê-6•ÚT/ëÃoü«A£DÊ‰äbõÙ„a“•Öƒ)Â‚í6<r/=íKC0¾®¼6æ,ZR&¯äûò9²2Ğq2õ%U<t¤:¥°AÉå^±Ê­¬‘åæ¹D¯²ıÊÙêrÙ1Õ­‰’ş QÌN¤d2„áÆĞQí[0C|,9°švÀLŠ’ ¾[6¬&‰,gîÄ!haw/æmè8oM´™ç³.ğkgOšiHÜÆŸÔ”&ª§+ B<8;œZô+Yıÿ4ÍöÈ¦>ÃÄ½wÎ-”¿?|Ä²!MÑÏàÏ-ø­=,†YVÛRE†n7ßoB‘$›÷½Ó(då£}@Kt6-¹,€ÅÀKo€ÜÜw„å¬†æ-¼ß4›øQøŒuWóû¶MâÜv*¥ïÓ›7AŒ>?#)Å¸‹+—åÖèÿ¤M¾ŒLç:%^vt‚¯;§^à¹[ÙkÅPIç­9=TĞF-õvÎ±ÿÀO#Ls!ÕYó‡¤Vä‚j!à"¾5¾±êŸÖµ^†¡¬Fá ˜LÃJ-òB;ı˜¡JU›ZìvÚ5‹à/w`¬n°.İ–á5®ºiş¬²ZÖÃ3b fÓIj9kb*³÷
\75z”.ób°2Ü»MNõ^%Q¡µ¤4¿ùRªƒ?òû4>»ç“ŞÁx¸@N”îõëü‚ Ìfß\bÒTï?]…°UÚ";¢ªR›®ı¬QLßtdpí\L†IŸn'©0&JfŒ«˜)ßÂºãİ.N!×Eîª{ª¤.êóz×ÅWG»Xè|tBˆFWúiÆ´ğqµ›[Rw{ö=ÕT¿yßziâ3Ğ:Û/}8—åıÆü!†‘ÿ -ŸoŞÌğOÀ3ÒÇëO5y/’æE_$/ILq(G´C{Rã³EÀäN)š­›h1ôÂ™&“e­Q•—h7£ç½bLœ˜Š¬À÷ÑÌÊIÅ3 KÌY=ÖÃòlÕ’Ìê‘œÁİËlW"kc×‡ª‡=åÅ·m^³MgOñÕ[}3Àj4hR^“Ìµ"QjzYúë|æ8²)¾”ßÆM…R§=úÓKÌBKQ„ª¡÷ÊÊs ={õOÇ›T?
ü=‡}­Boøc—· IÃ›êø¾¼(…V´r¿R*nÛãëRÿÁ½ú1T|¶2*8â”g,ªÖÚGràÛùn^R£b±”cPğ\Âíp~§ìl÷8°
pn€ü(å~€€±ÎWf£–=ºĞ×ÑìØïy¡±Íqñ¿ øâÎ=}Şè³+LxÔÇÉÙG¦@Ÿa]mc$n…Ä1VàoQM<w¬D²†ë™SzÙ&W†·¹}ndiJ[2è;(SJ9LXVÅç:^îıåSUGıU²§DOä9ÔJÍ¡”‚ìÙ¹ÉNÚ%†ÓXÈ2%KÇnê¾±ŞéÖO9±²ÿ³ÊÃ§úe4Ç/‚¾¬Cáv&(ÌÍÀÛ!dÂ†EàÔº?<Ûçİ9Õ@n}ë¦ë­Š|Ğ¯âáî)­±¦·æí’Erh3î¡%áë!03]k¡qœúê)ëCv¬iÜÛ½Ûß’ƒ~Ù‹e·ÖşstB$xìÿV4õâ=ÙW€4å4Víøu•Ğ­W# ”á‰÷$şhS¿»ãú‹»7dF$XœÅh1÷½ÚŸƒÏ¤VËÆ‹êÕú_”h{[®V<’¸KÇ6«¸´àAÙ‰&±(,7h	ÃA³¹€26•UÚhˆ©¹rg4ÄÒÛ¹²gêjÑ8C2’÷×ÿıRGRøVt|¶g5w`ÈsfZ8i~RIgQ–1¥Š«
\j—×**”¹¸G13^1ƒ¢ï6—x“>ı–Û„uta?*7 ¹â&<ˆ3¢¥ş^ïw–í¦“éœ ‰EanBÔßêò;¼Œrë·İÅ„_XÓ†¯ü‚"1ÏtÔ‡÷ÔÈİ¶{ÍÎvX²=AıŠLŒzpª•™$-Ú#\VšK˜BV•½­¥ñ`”ùÂä·%ÊÃşÕ~;ãÎ[–ñƒ«K¬×Lõ«‰ÙSøÙ¯°U>/Må„|C¬üJ0F¯l²o¡ĞšÜwêÔï"-•µu¬éå0fÚË½Å.ûâ7õ„t¢S)ãO€­O«tÄL©RÔ>(¹èçÙ¯Ç@ã†$rØšêçìİaÌ8±úg
,æ½¨ïñ¡_î-ü
‚I*;2*Àpi™¹Fì ¢3èù*ábvKæ„m´ñÅõÊWcÔ¸¼¥çÑû,òœI¯@JÊ¨‡ìÕÍ±›'ª3e¦Ÿ='ö®Ä)Œ¡	9âXlZE›2õÌ? ‡@UÁ·Ç¹ßŒoÚn®Z
d›„=9BNÖF-LĞæòWL÷µ±:\['òHa¡]:”^ø4<ÿ•ha”ĞQ\QøAAànÊÙ,ÙÄCc¨:àŒ>,¹ ~]]O‹oÆ•BqY­‰á¶æñÕ)–[+²ÙaZv+õ·;,\c }"ƒ]—Ô: ô
ëKhÿOk¦ï÷z§÷°¸øe8]U«±ßÓƒ¼Â¾ Ì­„{bYÛm;Gc/SòÛrÒÜœ$–,‡¦(§tAí—Ç¶¬O(ĞIKW85I`ÿnË]-ØbaÀıÒ­Ôóa-CÃFQwCnÛ¥ØÚlÙ!NšŠ‹Ÿyè§æVÑ·¯ñÄnÉ sµ˜#Pñn>Rºñ*ÖÈâá<Wš_ñ¢Ğä7ìåtÕì°ø±8GÇ,}
_?kW²©€W‰ÚúºtXæË•V§°ÁWë•°ûK*S¦‘ïk09…µÄO @oŞX°y¥PŠ¢ş÷~&y/~loì0¨v4ó¾İR×NÇÇh‚Š,b˜"–|Ê×ŸÊ9ñë€E](ö™ŒRX²?^øpRÂã¾ŸAš(È¢ôN¬+]ì£–oš`Ãûjäìê'F¡UºJı'á¼±«Ğ;ç}À•æÙèLšGàràÈë¿±=Ù§'ál0•Ú™ú¹æräíÚèÑÈt,Cò
‹z4väêŞTÌşm-ÏÃ•JFÈF(E^bZn!Ôúù£]cïˆì5¦ÍbÅëãıá°ŸYx¸a™ÿõ‹ÑÇA«pà €f½>-5…EaµsŠJÅF×—Ïß-x¯ä{<C!~X™ãh‘J²Ò”¤
!@‡_ˆ\7M~X´úÇMË—9™Oş8§|ª¹¢<õo~# VX‡`ÌŒ—Åè¼õ¼ÙøRª™ñò~#Ü¥Àr^â4›|U$.m"¤È
W·ë&.N¾Ä©·nNVÌ³IdéÖ¯ ˜¼çm,9úrBÎj°«;“Ú£H¿pË"m'!‹Ñ
­ª‚R<¹!‘0Lÿ}ã€·4§-nä¥+Ñ’õÚ§òy!³Âë†ä	£³--$~-!Å/qŞ¾	¤ú\Â‘jŠ{M¦¥
*h¬+G%
’laöl©İÁ›ö·y>ìl€§Ù©‘G?|LU0Ò`ì<ƒ²’;k5ÂRn.¨9äìMÊ£0o¹|´ª¦¼ZdïÇZyuï-Y»Ô„Üô®%GûšA¶IFêCÊÁc¯êËD6œßm›$­–iU•$×ÿ6ƒ¯îÖ8[ÖVï4¥
¶û{(Şe6ˆ)Wò}¼×™¢˜Z¿yšjµq«2éN%Í_ÙMª2wŠÃò#\U\zÉ÷¸ñ÷ØÎN,·å€ò¼xcäx=E6±‹É¾ ¾˜R’©‰†…vı=´ÍU¬zĞ4¡8!Äö5zÄ€B=¢¢x}&UkÌÌ†•^ç>Ê±åÆ½ËRXøáàÿÂˆÌ¿¬çmN(–†õ“u ‡ÁıB7a¨=N2jîü;Æ|»ˆö¨ØIïìJ‘¹(­h¨ÖüŞh¨±¬SëĞãÛÍW÷+lyìï—¶ô‡bF=8&=$¹Í¤6Œ¨p›‰†*‹bRnÊî1£fbì¾f!_À´ÌƒÚÇ®#Ê†éè ¯!î±UVÅ<Iæ¡´³²%ÊÕ÷É‚6­ûÒ›âøœÙ,dÊ¦fÎŸr;º(+\$´<gĞ"^‰‡Ì±‹+iê¸ŒSU¡y	Kz*´'¢Ù­wâ~vÒ=%ğ”ƒ´º'¶UŞÌÓ{[£
ªË±8Xâì§f<¾İp…6ë½ÂOü?MG%‚ømı`o¯¤9ºƒ|5ËÁÃXiÉ>“ŞJ¬t2,µhøqjşaø·†¹ëfgq"2|WÒjƒ‹ŸMi%±ZO?èd;ñX£—ÙhèÏõOmL‘ô¡^’û]‚}¬p¹l?v–r+/°æ~Í{tİ/Ò‹†Å½D,Wò¦¼%‰“vâ!U¼O_B!?[l«&"I’3¾ÆjHB¼u´–ö©üåpí„ö(¶WóÖmHnQÃ¬¥7«‹µTtè¯9­µ†í7$"h5‘;£« t+¾æ¢óÑ7«‰ «.&¸gŸùgİIĞÂŒäV‡qwû+¨
,ÜğÇÄİµó!Ö“>(N'½JDmT÷"á/0Ø>«7ø‡ÔúØ§hÔŠ^NVÒùø´ã9ô—ŸÓiö¯SH3Ù.³‡WY¢Ğ¾›²|»Ï›ú‹>CÜÃÑ’Ö³Ü6åG=jóÜªìZ9Ş.ãÂö1Æ=ÁÉ•Ÿ`™}=Jü–½"* J¥w>µy3±8PÆdÕ~áA0v4tWvä,S@™]_”£	µb¼á7;ÜÄ‚¶èaRQxä$¢6Ãéß¼lj ¢´şòêĞ¦€ m’8“6?™‚nîY08ÄUÖNJxÀ-Cû ÁÉÚ5ÊoqgÆTJY)‘°•’U,¶:DÃ_yì VTá7üËs«v%ä<T‘â'öª;t´zğ\Ûv5™¼,Óİ\Â£ıÍ%uîy~¥s•5Î·)ğôHC{$æŒl.â`÷IËı#¼Ñd ŞÁ—¯¶’°A„0ÄHÎé^ì@kı×ÊLóÊ(Õmÿwã›ÈúzbÎß»k¾aƒšáÌ½KğUqTÂ‹ç	¦·Ò,ÆéìùÅY:8Ê¦Šø›yª`_‡«ï%$èª[	—~L]Íüç¬âv:‚”ÃÔrlÔ(í9x—ŞàİşşæèˆÒ<O‹»Ş—?Òe¹,¢`¥Ï›¿B!¸êìéŸÅp[‹ä]>Oaô§NjLÆI²åH«f® iõ°°6Ô™¬4—OwÊ.¾Š÷©Úç<bv­/å+}•Œ}ƒQËV6ü„„L˜7³nÙáIšOª_ş{ˆ!¨ã¼ºü=@t–PM`éÖ4q©æŞk½ğøöÌEú›ò‚8‰û%EõFèW×Ù¬$0W›Áq1F;¬ãn+°D2†œôJ+²Q¹Ñyp€9”ëú¬5w¡Îç9Jk5Ñ•e´¯íÏJw¡GOêkÈråŒóZòM¯'À…£àxA¹öH¾që.6şbnW¡Y{ì#C×~½5¬Ğ¹[Ç„é½—PaÓy'U[Ø7ß˜¡ó.5~‡_® ®8œªaäG~¾†£B½Ÿ1'rµ\­©îç»µc,Ide.\úç7Ãqp‰
 deÉ OK|58¹yÖ¾Ónq_!ÎˆŠ+´·Ñ#»">îA­	k¯s½8Nÿ}5“ã˜dÌë?*ˆj¿>=W€Ê¤´Ròù-¬Voö_m¡8šßì~Ò›Ñ×VóIš>]"ä€ÊK—[É@Z„l¤¤Ï“¶3.¶²×Õ¾3°q#Š*kœ'€âŒƒrÓªå‚Øu}½uÓˆÙæXü½Š8r&Zîºg«,¾/ğ{4O
Ú)ìş©NgÖì^‚{i ?_Û#«ï¾<Ne×š³
"×>¸sÜõBú&×ïö/‹L|òu9[ª˜«¬"'k*œ%áfºü4‘œ!‚.—yŞ¢¨^Àm~ÁŠœ´i]¶úü{s7Z]‡k†ùÔ†>Ãã‚Wµë/ÅmÏ–+åç/`\ê»1
4×oO.ó3‹õ/ñÊd•Ùùmñá2)_ó„f¼ÔS¥È½N€²Y+Æ
Cu$şqİ¤¿êpv!?ôIè:îÙ„v1L’C}ÈšNIŒìmgÄ<†t9‘†~¸1d*œó²…è¢KDùb‚¿€ù pJã`ğğ9¹Ü€ AÒy­gNªó»ã¶,gİÓq¬%³ÁÆÂ.Ã“¢9ßÁänk¢gxÄ}n÷ig}WÑ\ZUUY 0DöW‡"v[&§şÿ¶Pó”™íÚ)9RK)Ê Am™ûût”Qo3–<9?ïÀôZB&Ày&#éƒwiŞƒH<> 657¼UÌTSxª×*yÀâØ%3o~L£TÎiûÕæ…¦ûO,êĞ.;a_×ù	DƒƒÄ4¹QÍ"+av-­åbw0HÒ­0ºş»Õ>}J•<OÃæà*ÂÒ¥C$D/ÿUo.x(˜A°÷‘…:Û×ö»ğ·vFˆ,â~e-ì/µƒH¤ô¯ä%”ó€ä¹á&š#šB¬ÀúeÚ2¬bÉyÖÉÆ±
„Ò8-­ÖŠKõ!ÿ^»Ô[ÅF6ô2½Ç—¸åfZ6ùát†ÛCŸI¥+Âºóã=á¼ó@´¬Ç'yO‡;ÇöÑ¯µx—
F6<c½ÏLÂ]pt}´y7Ê]-W…õvã¾š2ûv.ËX5UÅŸ„ OÀÏÌ+½ÈÌb\Ãw‚o™e§?3¥¬`Ä–½—ï£ƒ8^OPÊ=¸óì¿¾í7ì3_G2@qãƒi7¬.ÕĞ
6(¶´;-ÚW_yKq‰Ó•“J4t-ÄÁÄŸ‡{øV]m—Íò»r„¨²r§Ë ‘aaÏ<%Î|dõ·]gÒ*"fU·Ğ^ñ:Ds9ÂqpÕaIŸøşk§”¾I´ôWç÷Í¢‹FåbsºË*Gw;Í Ô¤•¤Tg+/­$·ßˆlp_²Aª/J·®>jã×Qœé„.Ò)ÄN5œ}¦¢ßèÅöç:Â»´&^ü(ÈâÄßß«º^Q«ríUtF œÈuÿ>nØëN/ÙÌKØ9b]P?İBÄ=Û1Ò°QHa¢äk¢B½HM-¾ÃúâçüÂ*Ê£beÃMÌn©®Câô&‚PX³ÎB¨È0Šş.óS}¤‹(5óNVÎzÁùŸ)OJ‰!â¹€şcÿÔ{«q™sœ@¨¶øØ»ÑîÌî°T°.¾á‘ñmÅKŸ)0vB„›^ô—ÍÙı,¤Ï8ÓìñíRx‡6'\èNâ@_İLrÍ	ñE}	¶	õí,M«2:¯èÍpşuİúYÅÌ¢j%rÀØ¦PcZ,³P[kºÅ@QÃ#Õt)Jd:­	rdâÔÉ‹eã­ˆqw±¶s-ÛûH³1cË Ç´É”ZC’„÷m¢È~ô’¾¬AJîÍıŠ<6#ştÏ„MæÊnÚÇh¿(ƒ$öwµWLí_±™µ>ÛùŒí¾Zc8”È·UÄ{´yŸ#lVÎ¼>ü¤íCx£ÖpBò„Ì ~…ÿ„+GÁ›R<Â"’ˆH©0%XN„„2–,ı¡Mõ˜ßyÚG{u-Å˜iv,7ú;ím*ˆº6aì…¤h!/¤Âw;€á”úJ€ˆ4pdïTôò›‡.$!_½m½|¡¾î{õI86ÏÖ¬w‘—İ¥â~%` ‹9ë¦›É‹ºšãü^ôßJ×béş£µŠec¬zkÀv²J*Ôì‹îgë^~6é	oc™ª1ú©ÉĞ8ëÖAĞÙ7^……<J!99™ ¡ı'­=dÄ–]$†´Ëöö.-&a$LTji¯pBh¢ö–Rnv»CNÚ^,ò†K2>ÜÇí5Vv	‚´å:|Û=|¹ÿiq!©¦ã)€´Sög49{5çOZ·*l¼«ˆà½VÜÔ¶§cæÕ8^˜‰Dä,øúÉÀŠT£~À»8¼hT)È>t>XHŒ¿ßş>æ½½È@"ø<ïÙÉµà¯r”ÄW]:—İ²?øâÓGÁ˜ ©Ñ‡LÛ“sÃZ£ÍRg7#ñëH_38[èùµJYpgŠ#s0ÿáÓwQİÑJUêÜRšÇGÚ/l³–¾ßƒÿí<ÜİS£ĞTÙ?+»¢%Ğ—,üƒy#©ğHÌjkˆÍía¡¹•Aek77ã.[^±—™ãì
•Ï…Í—E_Bş~ƒöşü(¹ÉMî?¿’{°zb÷tT"H	¦ÏÅn‰Æ…®'Á,×¨ôÍ÷Î¶Y¦ ”œ«WšùİÎ2‹5˜®zE3ßŠS³0Æ³…}ßNL ®®ÜÁK	¾XCCmãGÂÆ,¹¤Ê«„¢}fÛÌºÑMIv¹Eõşÿlà6ß÷½<—œüèÜÅìN+
ÓÆ2V‡Z¡;/«EoÀ×c¨Éê˜ój²à3qwíæÂëÎ8uS„ü‘…æ;¥ƒĞ°[Ÿ¢—”CmØvÍSÒ8’.Ç|·	>Û/½ıÅ,e’>‰ÅD‡ì_ß··¾¥ˆ¿ª2~‚xŞ/ƒo{JÅ¶ccáñ¥tcm£[óò<Stîæ_ı£úä[qr­½í¸qÍšLÂÚ«ÈW:%€9ZD¨y6B­ŞQ¡BHğû¾×©uÙW’şøh<‡í\r§\Ù¢p^ zºßaĞİ1£®Í 3Ñåò-O³y×¯¼5XHÕB$Ydö*“fĞ“êf§ñi´B Æcö9¢0¶l²µ­à”}m4UıTóŠRe€7°é¡öO\†D`Å u>J,rï/Æ K`Zúqc1‰³×íL÷0úá·áÔWôu0¸ÖñÜ0ãş!è64Í†Ü3j]jG	¸Ql¨²¢7ÑT`Ö½~K—s+öÂsà>o…”K´ÕÄU’ÍÓ6éÊ· ˜aüßálhEâbşáq¶avˆ*bóÆˆõ-¥ªQ"İ	çç’Ëºµ5É¼¦±E0	ÎSnú÷¥ŒIÁOBŞ%±ƒºq®Vgx[ËÚx²A^è
§Õ„¼|ïAÈ>’â~î_c lçµö*&êû´”İ.|K‘gOèF+‡øÄÙÿF¢ˆÉ#“40¿®ïÏİšÆÎü}á{E‰v2ÁXÿ;za´¢l<Áºú îSAŸHqßFó[Ò×K`Î8Ñ[5Yú&©Ö†Ä
TA9{Ìk4T°R-i¥‘À†1Q*­ƒ¬|?{Dm
n©ıÔÑ€êsxÎ*ÿ£ÿH½&Jtc}kêÚ`İ3h~újÔşT
¦{şŒ6ÿÊqÎ‚ï6zK¦ô…Œ(tV(lÁÄ–ƒI„Ÿ¡QG¯Ş0ø·÷ş^¯$‡	 ]%ñŸğF9ñ,Oä]EÅÇˆå§ÙÆ§•® ÜsNåc‚L1ài«ÿf"LÁ1æJ¼d;$V%ÑT{ğN>;Ñ—w¡£k×o¢ÀÛRqÁ×Ì>HsÇÊ#{ÔÑ¯uˆt&KÛ-núD?´9ãûTäı±©ÿ{©&_†æ ëßÀ¦™Ç{£ëÅƒUíÖ¨ÃÕ~%¯ç(ŒùùxŞÄÓ:oKÜÆ5oŞË.´æß<Õ‚„%œrƒµ%‚ìMv%Š0% b´R½XDSøRäÒìKTIeÀå9€1ÅÚ³dù‘MlãFÏãuÓÇİW¥V¼ŠCê(vÿ%X¼£Z6*àÂ70Í÷ÚİRw+@)=õËš<%úXQDÂ€Em…
»ágÎ½‹¥Œ¯xÇŒPşaQ+Èí×‡Ş "R¤ç²l}ä$ƒÕ»AB•dH™4š²²˜Ü<Ğêe®†æ'°#y/.qÜ´•ì:n%«Uş~eærwD(ú$·ágÿH—DË|CÚeo£}²ËÎÏŒ\]¦:¥‰`Å.Î ¡È¥›0Õ-/vBƒ.aó”`f»‹nG¥)Á4Nş-j+ÕHÙU“<›è†Â˜ŞaöÊ·&pÖ-tülû”İ~Itÿ«¢Ï)†³³<VÈî¦ÌŠyo[@Ç1e…AŠšé,î¨§æIŸF¾Yùº«]2äJÕò~,hÀ0A‰å¾ÙòlÄlğüö9ú-Ÿ^ı6£‘‡hñsè!»´ÆEƒs+Mt0¾îÅB$ÖØ˜X´tS-*àJªf+ód{µaÅç®ÿ®²éƒëhË4r£{í]¸öÒzÎ×R£x¯6‡3±¯Ù$Ú˜*ÛÄ†¾W¹ôÁı”œH¢,äT2à÷³²c<R)|D°$§”i™‘`blÜ! gv[»-¿ÎÁG.‡w,¹úrE)è@\­zìÃífsf ì¹½·W/ä&t§ÇaÇìüq §v‚bN€µ5fá½ZË·¼ûÈÜ…;!•‚¢”i€RiÆ£¥Û~ìv%Øç¥/oúçaJ5àq¥ÿ€Å%µğ®¬·/íœ‹ƒ=êÎ{ôIÜÌ‰Ğ¾š!†7²¤©=ëA†	*SVëno[†Éùf·R5é©}î¶'FoF¶è¨>Cqu/P?ñf|ÏeˆÒûÌBlëF+[¦°-6³T&ŒMrÖ•<óhV_z±gsö—Õ¬sô=»;ëSsî˜ç: 6¢eLm˜Ê"^çš®…AıÔº3ÑÄ8Æ{ò¥îş¾‰7~^Ô{’'D\=ùbñgÂÁ¼„¤~#‚I"Rx¶ÈèA‡1òİXŠò4/ù !µ"ÍU„pÅR¦›.½ºŸ ¡Iµl]~ !Ş!¨UşŞ0ì&ní¢µ–ÂvŠ*Ä»w]àªÊV¨ÀŞÇOÜj)ïîuÜù›1b, Ğ—*Ô qÇ>™Ÿ+íJ3ºCÔ¹¥—«‹œÄÀ€ÓbBÑ’>¿»#T$¤G­ÃØ2ázÒ½‰3û	st³É²>¸´‰õøÃe
ÒHwÎ3û7Ãk¥ä‚,ŒÜÉ’#ni“Ù5ËäÈŸˆiæáù}¾í&á¸D§Zthnœ|ù/ı#œ¤³ş5`óÈ,6µ$ÏŠ(§ŸB¼IHª™ÃÑæØŠrı J:]”’ß 2¢ªµê™]Xœ¡ÒqÉ4O=bôÉ qFQãcÖüÅ°¿`¿	ì© ì•äİÛŒhêÎDuÏ/z„1qPçË,4FÄDÙxºTzÛëj¼`:İĞ+{k%~â°:£}ïå=_ ìC~^…
_z—İbqUoĞè²¸„Pì€C±XŠ9CpóMÓRÙrâ	_Ó®œœÈ™¾ACÇ4¹WT
ø Sm_Ó-hH©
¨t$3ËaVì|tÊÄ<¥?ÏgA[¢ J!3:[ê	 ßÑ:Ñtq#NsèÑÄS·Ø_}¨jMºw‘W9DÃÉxFPyjyne@¦[nö‰Ê>Ãï°{©±imûOÄS¬øõÿ-·ıe™ç>˜çŠ1Úˆ$3|”?¿Ç¬m-ó×¢ÈÑîjÇ:^
ãèà\ICc„ï~6	Ñp˜<¾¬çn	~@É1/\¦—>a.'VU•Ã¶|ÖìÇûâ•„2±TáïVaªyP®ªĞ™ §m¨_Ò+:D],&°ñ×á6{÷Úòã0íÊÚ>	ıŸl &àVdÆKN©ÆÏ¨DGvÛê[»ô;"®"ê-’ÇqrÅÜ¤€²µ¯œäYb”LZÃ-Å_$Øâ94&‡×Ê‡xEp*ü%-7-´ÿHFe·¦}Q%NkQ7|!Şw¥´-7baÚÖÖóx—b<qOY}0úü†¹x'ùgÄÿå }}Cƒ8ü˜|Ç“7Äf‘]²»òŸw
×È5ûõ÷«\æyÙT±ğn×(Ù®$ì%]§lSSˆ,[:ÎóF§@˜f<3wD–ı´Rh,ë”_ÂR§ctò{+/Çğ 1;n¹êÔ«}ŸÑ‡¨{ÕÆ}—`: Õ±šÉ‚éQIÀW‰Å½µ¥P_1ˆ!Fñpqò=eÃB­ß y²V9i‡Ä‚|¤ïJu}$Ç»CŠÃy:¼„Ä¿û³’Ti)-|	Æ M5¯oV­ÓÜUM©Xûå)¼²uã`šw©j.shı” Ùvİ(ó¡0ºÆŒ5Ót
ÿ XÒ#ïÇ«À;ªAĞ7æ¹Pãâ·ø(¹k˜òNQòQ]—§z•‹ÁÆ˜WzJ9åòdË|~•zIA1lr,*Ş´ÔÉÍé\|$¤»ÄF´
9ÚJ¨I¬ubcÌı¦.-ÃFfA²x6ÍÿF7F|ä&¢ù×b¿îğ˜÷4ÿ»¿AXq}úÚg[m(®½ù½•>©XN@½ùKü	ŒEàDj%Í #!^Ñ’ŠHÆókfG…2|°`G'gØ,Á†¼KZUÜ¶@rÍ”>õNaƒÁóÒü¹e¤†Ü¶à“'ÜÂ–pÅ‰F’HmÌoí©ıb3‡Ì-Dª8’ééGu¿UÙVû‰›ëMC õºTÓ	~ö 2²Qr™nªİÊ¡D½ƒİUëEµ·ˆÓ÷kÁŞíÏ=oK¨s%ìş8enˆ¿‡¶­[H«#ösíò]jòƒÖ—ÏäKEZ5wáòˆ¦J˜úç7ØÜ	erkİó	7*Îi8Eb=°çEÃËkQ¶ Ÿf;cHL‹IWF	Ö<¤CCZÆW‚‡#—±hÇrDpxšav`TÏ_«dá©ÎÕ¦D‘ÒZşÁğ9Qv²{VÖèø„×´jF˜|šÃôíAt¥4Dk–êİV4bìA× ÛÈé½^ª¾Û…b¼Òİb}N&9«"µ½$2lêÄSò÷ŸÃÕÀ’ÉF^PZ†#[2ZşëïkLÍ­ Ñ .D®Á§£¹ıİ¡2[ƒİg¸ü9ü#hÖ‡cE³<§Ö¤3à?ºW;Q¡ûÖV%, Tö.Æë.@¬5Ìéî+_¾¼¸Døöæ^T˜êœ£¥ô>Æ‚n%åø³,>ÿŒ%ÊãBÿütŠ¦rØ³ÆM"htÊ„*¹ØtJâÆ‡5ÖĞ-äSş\r¢Sº[r"zñÿäeùWV`å®ñø’5.ö¦`ÊØö?ë#nÃZÁİÿå÷Ùø‘BMèAÂ(Ÿ2ªĞ|·î­ ÏË¿rdSÍr)¦ÓRŞŠüxSf\ 
ˆ²¿±ëBsÛ¥=Á Ğ gôóÏ£>àÖ#&w+­èõ½ßTì­:'Ü™[çùT“ÿ%3%ZÊM~¿Z‡Ñ¹@— t0O§,_†ª]#–Iå–¾ÑîÂ4Uæ¢¥1G…Ç2bÚ‚ØT’Eçıœ¢ÛMz4¿>X±²°§Š•KÄ±©¦ù«rµ'Ñ7f¶š›şú¿VLl¼&™u"«ä€èbã£ç¥‡]ù6S©0ò|d`qê}W$´§®'õ{ß·ç$“«è5kˆ‹çÙ`†øs§ªm>îdNtÅ‘ # -	“{PØWêRLÖÚóøÀšpœ°€oqBrÔäAÓçijg\ÎÔúPY€õ ŠË¯ç»e¯ùÚ>t½-<P 2+ò_ó…ªZŠg
%Şı°Ş›²wZ0	µçÜIhZ‘_<…0|	½?`á%1Ù@¥}4
Ÿ¦ …WI[`]êFbMpw€…øé×’$şµ_Õ_e E§—Šp ö¥ÃmŒ0şGÆßÕƒÎÔğ´·Ç‡—OºËşc3¡§eòE,Ø0ğêU¶'»E”·ÿ\©Ópó­æÕ…œµ6Ü¸Ü³u(<=ÕÛ¤­³´9tùC†Dç]OÜ©™ÓWôl í/½öLµdÒANØB™K¾AuzâvCî¶,ë7<îO ØCÑÁ-+ÿøO5á%Óó­„o5½z2ÊÄ'Ö)˜@ø§Œ!ÓJ	¢H+@»ÀØYs—<]oÑjöq+ïz: &«e[ÎcÖwÎï¢×µnxV˜ìF±.bRÎv1Bm’ÀÌâ¼fØÇ=©\'ÆŒ?íñL–Ïfğ<æP¶øjè&Å{§åºNsIÂ^-Ø7ï·òá9*®õ#2I.l¼**SË’Æ×!´¨=Ôd¨&¢ˆ_©ÒwÒ °Ü«ã2gÒßm²‚ÍÂZ`wå±(‘U¾ï5nòõ9,ámb¬–Ç<‘iC©¥±>ˆßÇ©à|Ê‡¿z©DšW¨=!äN	ñuç]E¨ˆÕÁH6ùŠå…èŠ9ü…)áwŒ¼â±˜}“MLÇH{ï=#§Î~J2Õ