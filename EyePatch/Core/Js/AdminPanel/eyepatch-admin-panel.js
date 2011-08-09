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