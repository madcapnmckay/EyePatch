ep.widgetTypes.menu = {
    create: function (data, onFinish) {
        $(this).html(data.contents);
        onFinish();
    }
};