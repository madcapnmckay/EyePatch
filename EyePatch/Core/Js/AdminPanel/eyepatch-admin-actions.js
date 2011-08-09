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
