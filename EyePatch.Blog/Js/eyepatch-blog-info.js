/*global document, window, $, ko, debug, setTimeout, alert */
(function () {
    // Private function
    var templateEngine = new ko.jqueryTmplTemplateEngine();

    ko.blogInfoPanel = {
        viewModel: function (configuration) {
            this.data = {
                urls: configuration.urls,
                templates: ko.observableArray(configuration.templates || [])
            };

            this.post = {
                mapped: false,
                data: this.data,
                title: ko.observable(),
                url: ko.observable(),
                tags: ko.observableArray(),
                success: function (element, data) {
                    if (data.published) {
                        ep.blog.publish();
                        $.noticeAdd({ text: 'Post published', stay: false, type: 'success' });
                    } else {
                        $.noticeAdd({ text: 'Post saved successfully', stay: false, type: 'success' });
                    }
                },
                guessUrl: function (e) {
                    if (this.url().length === 0) {
                        var guess = '/' + this.title().toLowerCase().replace(/[^\w ]+/g, '').replace(/ +/g, '-');
                        this.url(guess);
                    }
                }

            };
            var currentPost = this.post;

            this.post.title.subscribe(function (title) {
                var guess = '/' + title.toLowerCase().replace(/[^\w ]+/g, '').replace(/ +/g, '-');
                currentPost.url(guess);
            });

            this.loading = ko.observable(false);
            this.defaultTemplate = ko.observable(configuration.defaultTemplate || 'blankInfoTemplate');
            this.displayType = ko.observable();

            this.templateToRender = function () {
                var type = ko.utils.unwrapObservable(this.displayType);
                var template = type === undefined ? this.defaultTemplate() : type + 'InfoTemplate';
                return template;
            } .bind(this);

            this.dataToDisplay = ko.dependentObservable(function () {
                var result = this[this.displayType()];
                return result || {};
            }, this);

            this.mapData = function (data, type) {
                if (this[type].mapped) {
                    ko.mapping.updateFromJS(this[type], data);
                } else {
                    ko.mapping.fromJS(data, {}, this[type]);
                }
                this[type].mapped = true;
            };

            this.display = function (node, ajax) {
                if (node === undefined) {
                    // display default
                    this.loading(false);
                    this.displayType(undefined);
                } else {

                    var type = node.type();
                    if (ajax) {
                        this.loading(true);
                        var that = this;
                        ep.postJson(this.data.urls.info, { id: node.id() }, function(result) {
                            that.mapData(result.data, type);
                            that.displayType(type);
                            that.loading(false);
                        });
                    } else {
                        this.loading(false);
                        this.displayType(type);
                    }
                }
            };
        }
    };

    ko.addTemplateSafe("postInfoTemplate", "<div class=\"form\">\
                                <form method=\"post\" action=\"${ data.urls.update }\" data-bind=\"prepareForm: success\">\
                                    <input name=\"Id\" type=\"hidden\" data-bind=\"value : id\" />\
                                    <div class=\"field\">\
                                        <label for=\"epTitle\">Title <span class=\"help\" title=\"The page title appears in the broswer tab, search results & when added to favorites. <br/><br/>Note: This is not the same as the name of the post in the tree which is purely for your reference.\"></span></label>\
                                        <input id=\"epTitle\" name=\"Title\" type=\"text\" data-bind=\"value : title\" data-val=\"true\" data-val-required=\"A post title is required\" placeholder=\"enter your post title here\"/>\
                                        <span data-valmsg-replace=\"true\" data-valmsg-for=\"Title\" class=\"field-validation-valid\"></span>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epUrl\">Url <span class=\"help\" title=\"The relative url at which your post can be reached. e.g. /some-pics-of-cats\"></span></label>\
                                        <input id=\"epUrl\" name=\"Url\" type=\"text\" data-bind=\"value : url\" data-val=\"true\" data-val-required=\"A url must be supplied\" placeholder=\"enter your post url here\"/>\
                                        <span data-valmsg-replace=\"true\" data-valmsg-for=\"Url\" class=\"field-validation-valid\"></span>\
                                    </div>\
                                    <div class=\"field\">\
                                        <label for=\"epTags\">Tags <span class=\"help\" title=\"Tags allow you to organise your post and display a tag cloud widget of your post archive. Enter comma separated tags e.g. tag1, tag2\"></span></label>\
                                        <input id=\"epTags\" name=\"Tags\" type=\"text\" data-bind=\"value : tags\" placeholder=\"tag your post\"/>\
                                    </div>\
                                    <br/><br/>\
                                    <div class=\"center help-text\">Double click a post in the tree to navigate to it and allow you to edit the post body.</div>\
                                    <div class=\"button-container\">\
                                        <button type=\"submit\" title=\"Click here to publish this post\" name=\"published\" value=\"true\" data-bind=\"visible: !published()\">\
                                            Publish</button>\
                                        <button type=\"submit\" title=\"Click here to save this post\" name=\"save\" value=\"true\">\
                                            Save</button>\
                                    </div>\
                                </form>\
                            </div>", templateEngine);

    ko.addTemplateSafe("noDraftsSelected", "<div class=\"form\"><div class=\"center help-text\">Blog posts currently saved as drafts. These are not published and will not be visible to the outside world. Click on a draft to edit it's details. Double click a draft to navigate to that page.</div></div>", templateEngine);
    ko.addTemplateSafe("noPublishedSelected", "<div class=\"form\"><div class=\"center help-text\">Blog posts already published. Click on a post to view info and edit some details. Double click on a post to navigate to that page.</div></div>", templateEngine);

    ko.addTemplateSafe("blankInfoTemplate", "<div class=\"form\">Blank</div>", templateEngine);

    ko.addTemplateSafe("blogInfoPanelTemplate", "\
        <div style=\"height: 100%;\" data-bind=\"template: { name: templateToRender, data: dataToDisplay }\" />\
        <div class=\"info-loading\" data-bind=\"visible : loading()\"></div>", templateEngine);

    ko.bindingHandlers.blogInfoPanel = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor(),
                container = element.appendChild(document.createElement("DIV"));
            ko.renderTemplate("blogInfoPanelTemplate", value, { templateEngine: templateEngine }, container, "replaceNode");
        }
    };
} ());
