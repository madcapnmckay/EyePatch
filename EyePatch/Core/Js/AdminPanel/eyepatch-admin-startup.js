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