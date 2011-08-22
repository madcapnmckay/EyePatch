$(function () {
    var loadedModes = [];

    var highlight = function ($pre, mime) {
        // insert element to highlight
        var highlighter = $('<pre class="highlighted-code cm-s-default" contenteditable="false"></pre>'),
            code = $pre.text();

        $pre.after(highlighter).hide();
        CodeMirror.runMode(code, mime, highlighter.get(0));
    };

    $('pre.code').each(function () {
        var $this = $(this), mode = $this.attr('data-mode'), mime = $this.attr('data-mime');

        if (loadedModes.indexOf(mode) == -1) {
            $.getScript('/core/js/codemirror/' + mode + '/' + mode + '.js', function () { highlight($this, mime); });

            loadedModes.push(mode);
        } else {
            // already loaded
            highlight($this, mime);
        }

    });
});