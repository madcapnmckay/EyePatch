(function () {
    var templateEngine = new ko.jqueryTmplTemplateEngine();

    ko.addTemplateSafe("blogInterface", "\
                <div class=\"folder-info\">\
                    <form method=\"post\" action=\"${ urls.settings }\" data-bind=\"prepareForm: saved\">\
                        <h3>Configuration</h3>\
                        <div class=\"field\">\
                            <label for=\"epListPage\">Post list page <span class=\"help\" title=\"The page where your list of blog posts will display (defaults to homepage). This page is used by widgets such as tag cloud to redirect the user to a filtered list of posts.\"></span></label>\
                            <select id=\"epListPage\" name=\"listPage\" data-bind=\"options: state.pages, optionsText: 'Value', optionsValue: 'Key', value: state.listPage\"></select>\
                        </div>\
                        <div class=\"field\">\
                            <label for=\"epTemplate\">Post template <span class=\"help\" title=\"The template used by every blog post\"></span></label>\
                            <select id=\"epTemplate\" name=\"template\" data-bind=\"options: state.templates, optionsText: 'Value', optionsValue: 'Key', value: state.template \"></select>\
                        </div>\
                        <div class=\"field\">\
                            <label for=\"epListPage\">Disqus ID <span class=\"help\" title=\"Used to generate the Disqus comments on each post. Note if this is omitted no comments will be displayed\"></span></label>\
                            <input type=\"text\" name=\"disqus\" data-bind=\"value: state.disqus\" />\
                        </div>\
                        <div class=\"button-container\">\
                            <button type=\"button\" name title=\"Click here to create a new post\" data-bind=\"click: newDraft,  clickBubble: false\">\
                                New Post</button>\
                            <button type=\"submit\" name title=\"Click here to apply your changes\">\
                                Apply</button>\
                        </div>\
                    </form>\
                </div>", templateEngine);

    ep.blog = {
        state: {
            listPage: ko.observable(),
            template: ko.observable(),
            disqus: ko.observable()
        },
        saved: function () {
            $.noticeAdd({ text: 'Blog settings saved successfully', stay: false, type: 'success' });
        },
        drafts: {},
        draftsInfo: {},
        published: {},
        publishedInfo: {},
        tabs: {},
        createPanelBody: function (element, viewModel, parentViewModel) {
            $(element).append('<div class="ko-tabcontainer" data-bind="tabs : tabs"></div>');

            ep.blog.urls = viewModel.contents.urls;
            ep.blog.state.pages = viewModel.contents.pages;
            ep.blog.state.templates = ep.templateList;
            ep.blog.state.listPage(viewModel.contents.listPage);
            ep.blog.state.template(viewModel.contents.template);
            ep.blog.state.disqus(viewModel.contents.disqus);
            ep.blog.tabs = new ko.tabs.viewModel(viewModel.contents.tabs);

            // connect handlers
            viewModel.contents.drafts.handlers = ep.blog.actions.draft.handlers;
            viewModel.contents.published.handlers = ep.blog.actions.published.handlers;
            ep.blog.draftsInfo = new ko.blogInfoPanel.viewModel({ templates: ep.templateList, urls: viewModel.contents.urls, defaultTemplate: 'noDraftsSelected' });
            ep.blog.drafts = new ko.tree.viewModel(evalProperties(viewModel.contents.drafts));
            ep.blog.publishedInfo = new ko.blogInfoPanel.viewModel({ templates: ep.templateList, urls: viewModel.contents.urls, defaultTemplate: 'noPublishedSelected' });
            ep.blog.published = new ko.tree.viewModel(evalProperties(viewModel.contents.published));

            ko.applyBindings(ep.blog, element);
        },
        createBlogTab: function (element, viewModel, parentViewModel) {
            var container = element.appendChild(document.createElement("DIV"));
            ko.renderTemplate("blogInterface", ep.blog, { templateEngine: templateEngine }, container, "replaceNode");
        },
        createDraftsTab: function (element, viewModel, parentViewModel) {
            var info = $('<div data-bind="blogInfoPanel: draftsInfo"></div>'),
                tree = $('<div data-bind="tree : drafts"></div>');

            // it appears that sometime the css doesn't load in time for the splitter to do it job so must set these explicitly
            $(element)
                    .height(423).width(496)
                    .append(tree)
				    .append(info)
                    .splitter({
                        type: 'v',
                        minLeft: 203, sizeLeft: 203, minRight: 150,
                        cookie: 'EyePatchDraftsVSplitter',
                        accessKey: 'I'
                    });

            ko.applyBindings(ep.blog, element);
        },
        createPublishedTab: function (element, viewModel, parentViewModel) {
            var info = $('<div data-bind="blogInfoPanel: publishedInfo"></div>'),
                tree = $('<div data-bind="tree : published"></div>');

            // it appears that sometime the css doesn't load in time for the splitter to do it job so must set these explicitly
            $(element)
                    .height(423).width(496)
                    .append(tree)
				    .append(info)
                    .splitter({
                        type: 'v',
                        minLeft: 203, sizeLeft: 203, minRight: 150,
                        cookie: 'EyePatchPublishedVSplitter',
                        accessKey: 'I'
                    });

            ko.applyBindings(ep.blog, element);
        },
        buildPostTreeContext: function (event, dataItem) {
            dataItem.selectNode();
            return { name: 'post' };
        },
        postDelete: function (dataItem) {
            dataItem.deleteSelf();
        },
        postRename: function (dataItem) {
            dataItem.isRenaming(true);
        },
        newDraft: function () {
            // show the drafts tab
            ep.blog.tabs.tabs()[1].show();

            // create new draft
            ep.blog.drafts.addNode({ parent: undefined, type: 'post', name: 'New Post' });
        },
        publish: function () {
            // remove the node in question
            var node = ep.blog.drafts.selectedNode();
            // publish
            ep.blog.drafts.deleteNode(true);
            ep.blog.published.addNode(node);

            // show the published tab
            ep.blog.tabs.tabs()[2].show();
            ep.blog.draftsInfo.display();
        },
        actions: {
            draft: {
                handlers: {
                    selectNode: function (node, onSuccess) {
                        onSuccess();
                        ep.blog.draftsInfo.display(node, true);
                    },
                    addNode: function (parent, type, name, onSuccess) {
                        ep.postJson(ep.blog.urls.create, { name: name }, function (result) { onSuccess(result.data); });
                    },
                    renameNode: function (node, from, to, onSuccess) {
                        ep.postJson(ep.blog.urls.rename, { id: node.id(), name: to }, onSuccess);
                    },
                    deleteNode: function (node, noAjax, onSuccess) {
                        if (!noAjax) {
                            ep.postJson(ep.blog.urls.remove, { id: node.id() }, onSuccess);
                        } else {
                            onSuccess();
                        }
                    },
                    doubleClick: function (node) {
                        if (node.type() === 'post') {
                            ep.postJson(ep.blog.urls.navigate, { id: node.id() }, function (data) {
                                window.location = data.url;
                            });
                        }
                    }
                }
            },
            published: {
                handlers: {
                    selectNode: function (node, onSuccess) {
                        onSuccess();
                        ep.blog.publishedInfo.display(node, true);
                    },
                    renameNode: function (node, from, to, onSuccess) {
                        ep.postJson(ep.blog.urls.rename, { id: node.id(), name: to }, onSuccess);
                    },
                    deleteNode: function (node, noAjax, onSuccess) {
                        ep.postJson(ep.blog.urls.remove, { id: node.id() }, onSuccess);
                    },
                    doubleClick: function (node) {
                        if (node.type() === 'post') {
                            ep.postJson(ep.blog.urls.navigate, { id: node.id() }, function (data) {
                                window.location = data.url;
                            });
                        }
                    }
                }
            }
        },
        tagcloud: {
            create: function (data, onFinish) {
                $(this).html(data.contents);
                onFinish();
            }
        },
        list: {
            create: function (data, onFinish) {
                $(this).html(data.contents);
                onFinish();
            }
        },
        post: {
            create: function (data, onFinish) {
                $(this).html(data.contents);
                onFinish();
            },
            saveEditable: function (contenteditable, html) {
                if (blogPostId) {
                    ep.postJson('/blogadmin/body', { html: html, postId: blogPostId }, function () {
                        $.noticeAdd({ text: "Page updated", stay: false, type: 'success' });
                    });
                }
            }
        }
    };

    $('.content-area').delegate('.ep-widget-blog-body', 'widget-initialized', function () {
        var $element = $(this), $contents = $('.post-body', $element), isInvalid = $('.blog-post-invalid', $element).length > 0;

        if (!isInvalid) {

            if (ep.blog.post.editorSettings === undefined) {
                ep.blog.post.editorSettings = new ko.editable.settings({
                    handler: ep.blog.post.saveEditable,
                    modalCssClass: 'eyepatch-admin-window',
                    imagePickerHandler: function (onSuccess) {
                        ep.postJson(ep.urls.mediaFolder.all, {}, function (data) {
                            var images = [];
                            for (var i = 0; i < data.images.length; i += 1) {
                                images.push({ src: data.images[i] });
                            }
                            onSuccess(images);
                        });
                    },
                    onInitialized: function ($editable) {
                        $editable.find('.highlighted-code').remove();
                        $editable.find('.code').show();
                    }
                });
            }

            if ($contents.attr('data-bind') === undefined) {
                $contents.attr('data-bind', 'editable : editorSettings');
                ko.applyBindings(ep.blog.post, $contents.get(0));
            }
        }
    });
} ());