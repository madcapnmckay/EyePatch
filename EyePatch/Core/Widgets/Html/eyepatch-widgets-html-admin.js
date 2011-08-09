ep.widgetTypes.html = {
    create: function (data, onFinish) {
        $(this).text(data.contents);
        onFinish();
    },
    saveEditable: function (contenteditable, html) {
        var id = $(contenteditable).parent().attr('data-id');

        ep.postJson('/htmlwidget/update', { contents: html, id: id }, function () {
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
    imagePickerHandler: ep.widgetTypes.html.getImages /*function(onSuccess) { 
				// we could call a service here, for now lets fake an ajax call
				var results = [
					{ src: "http://lol.ianloic.com/image-cache/2ff/2ff0236b5a3b9f45414457db219803a9.jpg", title: "some title 1", altText: "some alt text 1"  },
					{ src: "http://lol.ianloic.com/image-cache/4e1/4e104c20507d9ac354193e66bdc32124.jpg", title: "some title 2", altText: "some alt text 2"  },
					{ src: "http://lol.ianloic.com/image-cache/de0/de0fc43880d525e72d013638dbbe5e6b.jpg", title: "some title 3", altText: "some alt text 3"  },
					{ src: "http://lol.ianloic.com/image-cache/b0b/b0bd4b663f7dbf0f70a7400ab97b2ce1.jpg", title: "some title 4", altText: "some alt text 4"  },
					{ src: "http://lol.ianloic.com/image-cache/c21/c21de1baa6245232699d9d29d2dcbb6b.jpg", title: "some title 5", altText: "some alt text 5"  },
					{ src: "http://lol.ianloic.com/image-cache/3c8/3c84a83ad0844644bab70fe24b525a3d.jpg", title: "some title 6", altText: "some alt text 6"  },
					{ src: "http://lol.ianloic.com/image-cache/1a1/1a1b26c4def09bd9e09666a51ca4cdfe.jpg", title: "some title 7", altText: "some alt text 7"  },
					{ src: "http://lol.ianloic.com/image-cache/407/40786684125659748cc1c92fb5bfc15f.jpg", title: "some title 8", altText: "some alt text 8"  },
					{ src: "http://lol.ianloic.com/image-cache/05f/05ff574023935efded87d574234af4eb.jpg", title: "some title 9", altText: "some alt text 9"  },
					{ src: "http://lol.ianloic.com/image-cache/af7/af7dbe51db361ff667188fc42bd54d13.jpg", title: "some title 10", altText: "some alt text 10"  },
					{ src: "http://lol.ianloic.com/image-cache/4e4/4e46239f5ca78b9fed5f040a5f94b922.jpg", title: "some title 11", altText: "some alt text 11"  },
					{ src: "http://lol.ianloic.com/image-cache/7d3/7d387414ff627bd6289129073e9b83aa.jpg", title: "some title 12", altText: "some alt text 12"  },
					{ src: "http://lol.ianloic.com/image-cache/2b0/2b0dbaeba12f5abcc4158719411dc41d.jpg", title: "some title 13", altText: "some alt text 13"  },
					{ src: "http://lol.ianloic.com/image-cache/edf/edfb72bf470555b200b6306676ef8765.jpg", title: "some title 14", altText: "some alt text 14"  }
				];
				onSuccess(results);
	}*/
});

$('.content-area').delegate('.ep-widget-html', 'widget-initialized', function () {
    var $element = $(this), $contents = $('.widget-contents', $element);

    if ($contents.attr('data-bind') === undefined) {
        $contents.attr('data-bind', 'editable : editorSettings');
        ko.applyBindings(ep.widgetTypes.html, $contents.get(0));
    }
});