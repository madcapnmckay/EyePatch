ep.widgetTypes.html = {
    create: function (data, onFinish) {
        $(this).text(data.contents);
        onFinish();
    },
    saveEditable: function (contenteditable, html) {
        var id = $(contenteditable).parent().attr('data-id');

        ep.postJson('/htmlwidget/update', { pageId: ep.page.id, contents: html, id: id }, function () {
            $.noticeAdd({ text: "Page updated", stay: false, type: 'success' });
        });
    },
    getImages: function (onSuccess) {
        ep.postJson(ep.urls.mediaFolder.all, {}, function (data) {
            var images = [];
            for (var i = 0; i < data.images.length; i += 1) {
                images.push({ src: data.images[i] });
            }
            onSuccess(images);
        });
    }
};
ep.widgetTypes.html.editorSettings = new ko.editable.settings({
    handler: ep.widgetTypes.html.saveEditable,
    modalCssClass: 'eyepatch-admin-window',
    imagePickerHandler: ep.widgetTypes.html.getImages,
    onInitialized: function ($editable) {
        $editable.find('.highlighted-code').remove();
        $editable.find('.code').show();
    }
});

$('.content-area').delegate('.ep-widget-html', 'widget-initialized', function () {
    var $element = $(this), $contents = $('.widget-contents', $element);

    if ($contents.attr('data-bind') === undefined) {
        $contents.attr('data-bind', 'editable : editorSettings');
        ko.applyBindings(ep.widgetTypes.html, $contents.get(0));
    }
});