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
        Thumbnail = function (url, image, vm) {
            this.src = url;
            var viewModel = vm;

            this.width = 101;
            this.height = 112;

            this.imageWidth = image.width || 1;
            this.imageHeight = image.height || 1;

            this.frameWidth = ko.dependentObservable(function () {
                return 0.9 * this.width;
            }, this);

            this.frameHeight = ko.dependentObservable(function () {
                return 0.6 * this.width;
            }, this);

            this.resizeRatio = function () {
                var widthRatio = this.frameWidth() / this.imageWidth,
							heightRatio = this.frameHeight() / this.imageHeight,
							ratio = widthRatio < heightRatio ? heightRatio : widthRatio;
                return ratio; // don't resize if it already fits in the frame							
            } .bind(this);

            this.imageResizedWidth = ko.dependentObservable(function () {
                return Math.ceil(this.resizeRatio() * this.imageWidth);
            }, this);

            this.imageResizedHeight = ko.dependentObservable(function () {
                return Math.ceil(this.resizeRatio() * this.imageHeight);
            }, this);

            this.isSelected = ko.dependentObservable(function () {
                return viewModel.imageSelected() && viewModel.imageSelected() === this;
            }, this);

            this.select = function () {
                viewModel.imageSelected(this);
            } .bind(this)
        };


    ko.imagePicker = {
        viewModel: function (configuration) {
            this.folderContents = ko.observableArray([]);
            this.uploadControl;
            this.upload = ko.observable(false);
            this.newFile = ko.observable('');
            this.currentFolder = ko.observable();
            this.isLoading = ko.observable(false);
            this.imageSelected = ko.observable();

            var hash = { '.tif': 1, '.jpg': 1, '.jpeg': 1, '.png': 1, '.bmp': 1, '.gif': 1 };

            var checkExtension = function (filename) {
                var re = /\..+$/;
                var ext = filename.match(re)[0].toLowerCase();
                if (hash[ext]) {
                    return true;
                } else {
                    return false;
                }
            }

            this.openDialog = function () {
                var that = this;
                this.uploadControl = $('#fileUpload');
                this.uploadControl.change(function () {
                    that.selected();
                });
                this.uploadControl.click();
            } .bind(this);

            this.uploadFile = function () {
                var that = this;
                if (!$.IsNullOrWhiteSpace(this.currentFolder())) {
                    $.ajaxFileUpload
		            (
			            {
			                url: ep.urls.mediaFolder.upload + "?parentId=" + this.currentFolder(),
			                secureuri: false,
			                fileElementId: this.uploadControl.attr('id'),
			                dataType: 'json',
			                success: function (data, status) {
			                    if (typeof (data.error) != 'undefined') {
			                        if (data.error != '') {
			                            alert(data.error);
			                        } else {
			                            var url = data.url, image = new Image();
			                            image.src = url;
			                            image.onload = function () {
			                                that.folderContents.push(new Thumbnail(url, this, that));
			                            }
			                            that.upload(false);
			                        }
			                    }
			                },
			                error: function (data, status, e) {
			                    alert(e);
			                }
			            }
		            );
                }
            } .bind(this);

            this.cancelUpload = function () {
                this.upload(false);
                this.newFile('');
            } .bind(this);

            this.validFile = ko.dependentObservable(function () {
                if ($.IsNullOrWhiteSpace(this.newFile())) {
                    return false;
                }
                return checkExtension(this.newFile());
            }, this);

            this.selected = function () {
                this.upload(true);
                this.newFile(this.uploadControl.val().replace('C:\\fakepath\\', ''));
            } .bind(this);

            this.deleteSelected = function () {
                var that = this;
                if (this.imageSelected()) {
                    ep.postJson(ep.urls.mediaFolder.removeImage, { id: this.imageSelected().src }, function (result) {
                        that.folderContents.remove(that.imageSelected());
                        that.imageSelected(undefined);
                    });
                }
            } .bind(this);

            this.display = function (node) {
                var parentId = node.id();
                this.folderContents([]);
                this.currentFolder(parentId);
                var that = this;
                // get the contents
                this.isLoading(true);
                ep.postJson(ep.urls.mediaFolder.info, { id: parentId }, function (result) {
                    var images = result.data, idx = 0,
						loadImage = function () {
						    if (idx >= images.length) {
						        that.isLoading(false);
						        return false;
						    }

						    var url = images[idx], image = new Image();
						    image.src = url;
						    image.onload = function () {
						        that.folderContents.push(new Thumbnail(url, this, that));
						        idx++;
						        loadImage();
						    }
						};
                    // start
                    loadImage();
                });
            } .bind(this);
        }
    };

    ko.addTemplateSafe("epThumbnailTemplate", "\
            <div class=\"ui-thumb\" data-bind=\"hover: 'hover', css: { selected: isSelected }, click: select\">\
				<div class=\"thumb-inner\" data-bind=\"style: { width: width + 'px', height: height + 'px' }\">\
					<div class=\"thumb-frame\" data-bind=\"style: { width: frameWidth() + 'px', height: frameHeight() + 'px' }\">\
						<img src=\"${ src }\" alt=\"${ altText }\" title=\"${ title }\" data-bind=\"attr: { width: imageResizedWidth(), height: imageResizedHeight() }\"/>\
					</div>\
				</div>\
			</div>", templateEngine);

    ko.addTemplateSafe("epImagePickerTemplate", "<div class=\"ui-imagepicker\">\
                                <div data-bind=\"visible: currentFolder\">\
                                    <div class=\"image-list\" data-bind=\"template: { name: 'epThumbnailTemplate', foreach: folderContents }\">\
                                    </div>\
                                    <div class=\"button-container\" data-bind=\"visible: !upload()\">\
                                        <div style=\"overflow:hidden;min-width: 200px;\">\
                                        <button class=\"button\" title=\"click to delete the selected image\" data-bind=\"enable: imageSelected() , click: deleteSelected\">Delete</button>\
                                        <button class=\"button\" title=\"click to add a new image\" data-bind=\"click: openDialog\">Add</button>\
                                        </div>\
                                    </div>\
                                    <div class=\"button-container\" data-bind=\"visible: upload\">\
                                        <div style=\"overflow:hidden;min-width: 220px;\">\
                                            <label class=\"upload-label\" data-bind=\"text: newFile\"></label>\
                                            <button class=\"button\" title=\"click to upload\" data-bind=\"click: uploadFile, enable: validFile\">Upload</button>\
                                            <form method=\"post\" enctype=\"multipart/form-data\">\
                                            <input id=\"fileUpload\" name=\"image\" class=\"upload\" type=\"file\" style=\"display:none;\"/>\
                                            <button class=\"button\" title=\"click to cancel\" data-bind=\"click: cancelUpload\">Cancel</button>\
                                            </form>\
                                        </div>\
                                    </div>\
                                </div>\
                            </div>", templateEngine);

    ko.bindingHandlers.imagePicker = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor(),
                container = element.appendChild(document.createElement("DIV"));
            ko.renderTemplate("epImagePickerTemplate",
                                value, { templateEngine: templateEngine }, container, "replaceNode");
        }
    }
} ());

(function () {
    // Private function
    var logger = function (log) {
        if (typeof debug !== 'undefined') {
            $('<div></div>').appendTo('#log').text(new Date().toGMTString() + ' : eyepatch-contentarea.js - ' + log);
        }

        if (console !== undefined) {
            console.debug(new Date().toGMTString() + ' : ' + log);
        }
    },
		templateEngine = new ko.jqueryTmplTemplateEngine(),
    // options
		sortOptions = {
		    helper: 'clone',
		    opacity: 0.6,
		    addClasses: false,
		    tolerance: 'pointer',
		    connectWith: '.content-area',
		    forcePlaceholderSize: true,
		    forceHelperSize: true,
		    handle: '.widget-handle .icon.move',
		    refreshPositions: true
		},
    // classes
		ContentArea = function (id, name, element) {
		    this.id = id;
		    this.name = name;
		    this.element = element;
		},
		Widget = function (id, element, contentArea) {
		    this.id = id;
		    this.element = element;
		    this.contentArea = contentArea;
		    this.disableHover = ko.observable(false);

		    this.deleteSelf = function () {
		        $(element).trigger('delete', this);
		    } .bind(this);
		};

    ko.addTemplateSafe("widgetHandleTemplate", "<div class=\"widget-handle\"><div class=\"inner-handle\"><div class=\"button\"><div class=\"inner-button\"><div class=\"icon move\"></div></div></div><div class=\"button\"><div class=\"inner-button\" data-bind=\"click : deleteSelf\"><div class=\"icon delete\"></div></div></div></div></div>", templateEngine);

    ko.epContentArea = {
        viewModel: function (configuration) {
            this.dragHolder = configuration.dragHolder;
            this.areas = ko.observableArray([]);
            this.widgets = ko.observableArray([]);
            this.findWidget = function (widgetId) {
                for (var w = 0; w < this.widgets().length; w++) {
                    var widget = this.widgets()[w];
                    if (widget.id === widgetId) {
                        return widget;
                    }
                }
                return false;
            };
            this.handlers = {
                onMove: function (contentArea, widget, position) {
                    ep.postJson(ep.urls.widget.move, { pageId: ep.page.id, widgetId: widget.id, position: position, contentAreaId: contentArea.id });
                },
                onSort: function (widget, position) {
                    ep.postJson(ep.urls.widget.sort, { pageId: ep.page.id, widgetId: widget.id, position: position });
                },
                onDelete: function (widget, onSuccess) {
                    ep.postJson(ep.urls.widget.remove, { pageId: ep.page.id, widgetId: widget.id }, onSuccess);
                },
                onAdd: function (contentArea, widgetNode, position, element, onSuccess) {
                    var data = {
                        pageId: ep.page.id,
                        widgetId: widgetNode.id(),
                        contentAreaId: contentArea.id,
                        position: position,
                        sourceUrl: document.location.pathname
                    };

                    ep.postJson(ep.urls.widget.add, data, function (data) {
                        ep.utils.loadCss(data.widget.css);
                        ep.utils.loadScripts(data.widget.js, function () {
                            var widget = evalProperties(data.widget);
                            onSuccess(widget);
                        });
                    });
                }
            };
        }
    }

    ko.bindingHandlers.epContentArea = {
        'init': function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var $element = $(element),
				config = valueAccessor(),
                name = $element.attr('data-name'),
                contentAreaId = $element.attr('data-id'),
				self = new ContentArea(contentAreaId, name, element),
				deleteWidget = function (event, widget) {
				    var domElement = $(this);
				    if (confirm('Are you sure you wish to delete?')) {
				        config.handlers.onDelete(widget, function () {
				            // the handler returned true so we can remove the element
				            domElement.remove();
				            // and the widget
				            config.widgets.remove(widget);
				        });
				    }
				};

            // add self to collection
            config.areas.push(self);


            // identify widgets and add to collection
            $('.ep-widget', $element).each(function () {
                var $widget = $(this),
					id = $widget.attr('data-id');

                if (id !== undefined) {
                    var widget = new Widget(id, this, self);
                    // render the handle
                    $widget.attr('data-bind', 'epWidget : {}');
                    ko.applyBindings(widget, this);
                    config.widgets.push(widget);

                    // wire up delete event
                    $widget.bind('delete', deleteWidget);
                }
            });

            // wire up events
            // a widget is reordered
            sortOptions.update = function (event, ui) {
                // also gets fired when item is dragged out
                // check if item comes from this contentArea
                var $item = $(ui.item).addClass('ep-widget'),
                    widgetId = $(ui.item).attr('data-id'),
                    widget = config.findWidget(widgetId),
                    position = $('.ep-widget', $element).index(ui.item);

                if (widget) {
                    if (widget.contentArea.id === contentAreaId && position >= 0) {
                        config.handlers.onSort(widget, position);
                    }
                    widget.contentArea = self;
                }
                else {
                    // a new widget has been received
                    var node = ko.utils.unwrapObservable(config.dragHolder);
                    $item
                        .removeClass() // remove all previous classes from node
                        .addClass('ep-widget')
                        .addClass('loading')
                        .removeAttr('aria-disabled')
                        .removeAttr('style').html('<div class="loading-widget"><div class="loading-icon"></div></div>');

                    config.handlers.onAdd(self, node, position, $item, function (data) {

                        var newWidget = new Widget(data.id, $item, self), $contents;
                        config.widgets.push(newWidget);
                        // render the handle
                        $item.attr('data-bind', 'epWidget : {}').attr('data-id', data.id);

                        $item.addClass(data.cssClass).bind('delete', deleteWidget);
                        // widget contents
                        $contents = $('<div class="widget-contents"></div>').hide().appendTo($item);

                        // initialize widget
                        data.initializeFunction.call($contents, data, function () {
                            $('.loading-widget', $item).fadeOut('fast', function () {
                                var $this = $(this);
                                $this.parent().removeClass('loading');
                                $this.remove();

                                $contents.fadeIn('fast');

                                ko.applyBindings(newWidget, $item.get(0));

                                // trigger initialized event
                                $item.trigger('');
                            });
                        });
                    },
                        function () {
                            // something went wrong, cleanup
                            //$item.remove();
                        });
                }
            };
            // a widget is dragged into a new content area
            // or a new widget is dragged from the admin panel
            sortOptions.receive = function (event, ui) {
                var $item = $(ui.item).addClass('ep-widget'),
                    $helper = $(ui.helper),
                    widgetId = $item.attr('data-id'),
                    position = $('.ep-widget', $element).index(ui.item);

                if ($helper.hasClass('new-widget')) {
                    // tidy up the dragged node
                } else {
                    widget = config.findWidget(widgetId);
                    config.handlers.onMove(self, widget, position);
                }
            };

            sortOptions.start = function (event, ui) {
                //$(ui.item).style('position', 'relative');
            };

            $element.sortable(sortOptions);
        }
    };

    ko.bindingHandlers.epWidget = {
        'init': function (element, valueAccessor, allBindingsAccessor, widget) {
            var $widget = $(element);
            origZ = $widget.css('zIndex');

            $widget.hover(
				function () {
				    var $this = $(this),
						$handle = $('.widget-handle', $this);
				    $this.addClass('hover');

				    var z = $this.css('zIndex');
				    if (z === 'auto') {
				        $this.css('zIndex', '50000');
				    } else {
				        $this.css('zIndex', '');
				    }
				    $handle.position({ my: 'right bottom', at: 'right top', offset: '-1 0', of: $this });
				},
				function () {
				    $(this).removeClass('hover').css('zIndex', origZ);
				}
			);

            ko.renderTemplate("widgetHandleTemplate", widget, { templateEngine: templateEngine }, $('<div></div>').appendTo($widget), "replaceNode");
            // when complete notify any scripts wanting to do further work
            $widget.trigger('widget-initialized');
        }
    };

})();

ep.infoPanel = {};
ep.templateInfoPanel = { };

/* application actions */
ep.actions = {
    logout: function () {
        $('div.window-container').fadeOut(1000, function () { window.location = '/signout'; });
    },
    contentArea: {
        addHighlight: function () {
            $('.content-area').css({ 'boxShadow': '0 0 5px #ffcc00', 'minHeight': '30px' });
        },
        removeHighlight: function () {
            $('.content-area').css({ 'boxShadow': 'none', 'minHeight': '0' });
        }
    },
    widget: {
        addHighlight: function () {
            $('.content-area').css('boxShadow', '0 0 5px #ffcc00');
        },
        removeHighlight: function () {
            $('.content-area').css('boxShadow', 'none');
        }
    },
    file: {
        
    }
};

/* tree events */
ep.tree = {
    page: {
        context: {
            newPage: function (dataItem) {
                dataItem.addChild({ type: 'page' });
            },
            newFolder: function (dataItem) {
                dataItem.addChild({ type: 'folder' });
            },
            nodeDelete: function (dataItem) {
                dataItem.deleteSelf();
            },
            nodeRename: function (dataItem) {
                dataItem.isRenaming(true);
            }
        },
        handlers: {
            selectNode: function (node, onSuccess) {
                onSuccess();
                ep.infoPanel.display(node);
            },
            addNode: function (parent, type, name, onSuccess) {
                ep.postJson(ep.urls[type].add, { parentId: parent.id(), name: name }, function (result) { onSuccess(result.data); });
            },
            renameNode: function (node, from, to, onSuccess) {
                ep.postJson(ep.urls[node.type()].rename, { id: node.id(), name: to }, onSuccess);
            },
            deleteNode: function (node, action, onSuccess) {
                ep.postJson(ep.urls[node.type()].remove, { id: node.id() }, onSuccess);
            },
            moveNode: function (node, newParent, onSuccess) {
                ep.postJson(ep.urls[node.type()].move, { id: node.id(), parentId: newParent.id() }, onSuccess);
            },
            doubleClick: function (node) {
                if (node.type() === 'page') {
                    ep.postJson(ep.urls.page.navigate, { id: node.id() }, function (data) {
                        window.location = data.url;
                    });
                }
            }
        }
    },
    widget: {
        dragHelper: function (event) {
            return $("<div class='new-widget ui-widget-header'></div>");
        },
        handlers: {
            doubleClick: function (node) {
                if (node.type() === 'widget') {

                }
            },
            startDrag: function (node) {
                ep.actions.contentArea.addHighlight();
            },
            endDrag: function (node) {
                ep.actions.contentArea.removeHighlight();
            }
        }
    },
    template: {
        handlers: {
            selectNode: function (node, onSuccess) {
                onSuccess();
                ep.templateInfoPanel.display(node);
            }
        }
    },
    mediaFolder: {
        context: {
            newFolder: function (dataItem) {
                dataItem.addChild({ type: 'mediaFolder' });
            },
            nodeDelete: function (dataItem) {
                dataItem.deleteSelf();
            },
            nodeRename: function (dataItem) {
                dataItem.isRenaming(true);
            }
        },
        handlers: {
            selectNode: function (node, onSuccess) {
                onSuccess();
                ep.mediaFolderContents.display(node);
            },
            addNode: function (parent, type, name, onSuccess) {
                ep.postJson(ep.urls[type].add, { parentId: parent.id(), name: name }, function (result) { onSuccess(result.data); });
            },
            renameNode: function (node, from, to, onSuccess) {
                ep.postJson(ep.urls[node.type()].rename, { id: node.id(), name: to }, onSuccess);
            },
            deleteNode: function (node, action, onSuccess) {
                ep.postJson(ep.urls[node.type()].remove, { id: node.id() }, onSuccess);
            }
        }
    }
};

(function () {
    ep.buildPageTreeContext = function (event, dataItem) {
        dataItem.selectNode();
        var id = dataItem.id();
        var type = dataItem.type();
        if (dataItem.type() === 'page') {
            if (dataItem.cssClass() === 'homepage') {
                return { name: 'page', disable: ['Delete'] };
            }
            return { name: 'page' };
        }
        return id === '1' ? { name: 'folder', disable: ['Delete']} : { name: 'folder' };
    },
    ep.buildMediaTreeContext = function (event, dataItem) {
        dataItem.selectNode();
        return dataItem.id() === '/Media/' ? { name: 'folder', disable: ['Delete', 'Rename']} : { name: 'folder' };
    },
    ep.createPagesTab = function (element, viewModel, parentViewModel) {
        ep.dom.info = $('<div class="info-panel" data-bind="infoPanel : infoPanel"></div>');
        ep.dom.tree = $('<div data-bind="tree : pages"></div>');
        /* it appears that sometime the css doesn't load in time for the splitter to do it job so must set these explicitly */
        $(element)
                .height(623).width(496)
                .append(ep.dom.tree)
				.append(ep.dom.info)
                .splitter({
                    type: 'v',
                    minLeft: 203, sizeLeft: 203, minRight: 150,
                    cookie: 'EyePatchPageVSplitter',
                    accessKey: 'I'
                });

        ko.applyBindings(ep, element);
    };
    ep.createWidgetsTab = function (element, viewModel, parentViewModel) {
        $(element).append('<div data-bind="tree : widgets"></div>');
        ko.applyBindings(ep, element);
    };
    ep.createTemplatesTab = function (element, viewModel, parentViewModel) {
        var info = $('<div class="info-panel" data-bind="infoPanel : templateInfoPanel"></div>');
        var tree = $('<div data-bind="tree : templates"></div>');

        $(element)
                .height(623).width(496)
                .append(tree)
				.append(info)
                .splitter({
                    type: 'v',
                    minLeft: 203, sizeLeft: 203, minRight: 150,
                    cookie: 'EyePatchTemplateVSplitter',
                    accessKey: 'I'
                });

        ko.applyBindings(ep, element);
    };
    ep.createImagesTab = function (element, viewModel, parentViewModel) {
        var info = $('<div class="info-panel" data-bind="imagePicker : mediaFolderContents"></div>');
        var tree = $('<div data-bind="tree : mediaFolders"></div>');

        $(element)
                .height(623).width(496)
                .append(tree)
				.append(info)
                .splitter({
                    type: 'v',
                    minLeft: 203, sizeLeft: 203, minRight: 150,
                    cookie: 'EyePatchMediaVSplitter',
                    accessKey: 'I'
                });

        ko.applyBindings(ep, element);
    };
    ep.createPanelBody = function (element, viewModel, parentViewModel) {
        $(element).append('<div class="ko-tabcontainer" data-bind="tabs : tabs"></div>');
        ko.applyBindings(ep, element);
    };

})();
(function () {
    ep.initialize = function () {
        ep.body.attr('data-bind', 'windowManager: interfaces');
        $('div.content-area').attr('data-bind', 'epContentArea : contentAreas');

        ko.applyBindings(ep);
    };
})();
ep.onLoad = function () {
    var data = ep.data;
    // assign globals
    debug = data.debug;
    ep.urls = data.urls;
    // current page data
    ep.page = data.page;
    // templates list for drop downs etc
    ep.templateList = data.templateList;
    // page info panel
    ep.infoPanel = new ko.infoPanel.viewModel({
        urls: ep.urls,
        data: {
            templates: ko.observableArray(ep.templateList)
        },
        types: {
            page: {
                ajax: true,
                success: function () {
                    $.noticeAdd({ text: 'Page saved successfully', stay: false, type: 'success' });
                }
            },
            search: {
                ajax: true,
                success: function () {
                    $.noticeAdd({ text: 'Page saved successfully', stay: false, type: 'success' });
                }
            },
            facebook: {
                latitude: ko.observable(),
                longitude: ko.observable(),
                ajax: true,
                success: function () {
                    $.noticeAdd({ text: 'Page saved successfully', stay: false, type: 'success' });
                }
            },
            folder: {
                ajax: false
            }
        },
        defaultTemplate: 'folderInfoTemplate'
    });

    ep.templateInfoPanel = new ko.infoPanel.viewModel({
        urls: ep.urls,
        data: {
            templates: ko.observableArray()
        },
        types: {
            template: {
                ajax: true,
                success: function () {
                    $.noticeAdd({ text: 'Template saved successfully', stay: false, type: 'success' });
                }
            },
            templateSearch: {
                ajax: true,
                success: function () {
                    $.noticeAdd({ text: 'Template saved successfully', stay: false, type: 'success' });
                }
            },
            templateFacebook: {
                latitude: ko.observable(),
                longitude: ko.observable(),
                ajax: true,
                success: function () {
                    $.noticeAdd({ text: 'Template saved successfully', stay: false, type: 'success' });
                }
            }
        },
        defaultTemplate: 'folderInfoTemplate'
    });

    // connect the handlers object
    data.pages.handlers = ep.tree.page.handlers;
    data.widgets.handlers = ep.tree.widget.handlers;
    data.templates.handlers = ep.tree.template.handlers;
    data.mediaFolders.handlers = ep.tree.mediaFolder.handlers;
    // page tree
    ep.pages = new ko.tree.viewModel(data.pages);
    // templates tree
    ep.templates = new ko.tree.viewModel(data.templates);
    // widgets tree
    data.widgets.dragHolder = ep.dragHolder;
    ep.widgets = new ko.tree.viewModel(evalProperties(data.widgets));
    // tabs
    ep.tabs = new ko.tabs.viewModel(data.tabs);
    // content areas
    ep.contentAreas = new ko.epContentArea.viewModel({ dragHolder: ep.dragHolder });

    ep.mediaFolderContents = new ko.imagePicker.viewModel({ images: data.images, handlers: ep.actions.file });
    ep.mediaFolders = new ko.tree.viewModel(data.mediaFolders);

    // center the window initially
    data.windows[0].position = Math.floor((ep.body.width() / 2) - (data.windows[0].width / 2)) + ',100';
    ep.interfaces = new ko.windowManager.viewModel({ windows: data.windows, cssClass: "eyepatch-admin" });

    ep.initialize();
    ep.hideLoader();
};
