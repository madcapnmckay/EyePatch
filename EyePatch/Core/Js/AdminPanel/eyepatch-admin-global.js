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
