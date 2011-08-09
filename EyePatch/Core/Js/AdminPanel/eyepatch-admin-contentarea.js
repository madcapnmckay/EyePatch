(function () {
    // Private function
    var logger = function (log) {
        if (typeof debug !== 'undefined') {
            $('<div></div>').appendTo('#log').text(new Date().toGMTString() + ' : eyepatch-contentarea.js - ' + log);
        }

        if (console !== undefined) {
            console.debug(new Date().toGMTString() + ' : ' + log);
        }
    },
		templateEngine = new ko.jqueryTmplTemplateEngine(),
    // options
		sortOptions = {
		    helper: 'clone',
		    opacity: 0.6,
		    addClasses: false,
		    tolerance: 'pointer',
		    connectWith: '.content-area',
		    forcePlaceholderSize: true,
		    forceHelperSize: true,
		    handle: '.widget-handle .icon.move',
		    refreshPositions: true
		},
    // classes
		ContentArea = function (id, name, element) {
		    this.id = id;
		    this.name = name;
		    this.element = element;
		},
		Widget = function (id, element, contentArea) {
		    this.id = id;
		    this.element = element;
		    this.contentArea = contentArea;
		    this.disableHover = ko.observable(false);

		    this.deleteSelf = function () {
		        $(element).trigger('delete', this);
		    } .bind(this);
		};

    ko.addTemplateSafe("widgetHandleTemplate", "<div class=\"widget-handle\"><div class=\"inner-handle\"><div class=\"button\"><div class=\"inner-button\"><div class=\"icon move\"></div></div></div><div class=\"button\"><div class=\"inner-button\" data-bind=\"click : deleteSelf\"><div class=\"icon delete\"></div></div></div></div></div>", templateEngine);

    ko.epContentArea = {
        viewModel: function (configuration) {
            this.dragHolder = configuration.dragHolder;
            this.areas = ko.observableArray([]);
            this.widgets = ko.observableArray([]);
            this.findWidget = function (widgetId) {
                for (var w = 0; w < this.widgets().length; w++) {
                    var widget = this.widgets()[w];
                    if (widget.id === widgetId) {
                        return widget;
                    }
                }
                return false;
            };
            this.handlers = {
                onMove: function (contentArea, widget, position) {
                    ep.postJson(ep.urls.widget.move, { instanceId: widget.id, position: position, contentAreaId: contentArea.id });
                },
                onSort: function (widget, position) {
                    ep.postJson(ep.urls.widget.sort, { instanceId: widget.id, position: position });
                },
                onDelete: function (widget, onSuccess) {
                    ep.postJson(ep.urls.widget.remove, { instanceId: widget.id }, onSuccess);
                },
                onAdd: function (contentArea, widgetNode, position, element, onSuccess) {
                    var data = {
                        pageId: ep.page.id,
                        widgetId: widgetNode.id(),
                        contentAreaId: contentArea.id,
                        position: position
                    };

                    ep.postJson(ep.urls.widget.add, data, function (data) {
                        ep.utils.loadCss(data.widget.css);
                        ep.utils.loadScripts(data.widget.js, function () {
                            var widget = evalProperties(data.widget);
                            onSuccess(widget);
                        });
                    });
                }
            };
        }
    }

    ko.bindingHandlers.epContentArea = {
        'init': function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var $element = $(element),
				config = valueAccessor(),
                name = $element.attr('data-name'),
                contentAreaId = $element.attr('data-id'),
				self = new ContentArea(contentAreaId, name, element),
				deleteWidget = function (event, widget) {
				    var domElement = $(this);
				    if (confirm('Are you sure you wish to delete?')) {
				        config.handlers.onDelete(widget, function () {
				            // the handler returned true so we can remove the element
				            domElement.remove();
				            // and the widget
				            config.widgets.remove(widget);
				        });
				    }
				};

            // add self to collection
            config.areas.push(self);


            // identify widgets and add to collection
            $('.ep-widget', $element).each(function () {
                var $widget = $(this),
					id = $widget.attr('data-id');

                if (id !== undefined) {
                    var widget = new Widget(id, this, self);
                    // render the handle
                    $widget.attr('data-bind', 'epWidget : {}');
                    ko.applyBindings(widget, this);
                    config.widgets.push(widget);

                    // wire up delete event
                    $widget.bind('delete', deleteWidget);
                }
            });

            // wire up events
            // a widget is reordered
            sortOptions.update = function (event, ui) {
                // also gets fired when item is dragged out
                // check if item comes from this contentArea
                var $item = $(ui.item).addClass('ep-widget'),
                    widgetId = $(ui.item).attr('data-id'),
                    widget = config.findWidget(widgetId),
                    position = $('.ep-widget', $element).index(ui.item);

                if (widget) {
                    if (widget.contentArea.id === contentAreaId && position >= 0) {
                        config.handlers.onSort(widget, position);
                    }
                    widget.contentArea = self;
                }
                else {
                    // a new widget has been received
                    var node = ko.utils.unwrapObservable(config.dragHolder);
                    $item
                        .removeClass() // remove all previous classes from node
                        .addClass('ep-widget')
                        .addClass('loading')
                        .removeAttr('aria-disabled')
                        .removeAttr('style').html('<div class="loading-widget"><div class="loading-icon"></div></div>');

                    config.handlers.onAdd(self, node, position, $item, function (data) {

                        var newWidget = new Widget(data.id, $item, self), $contents;
                        config.widgets.push(newWidget);
                        // render the handle
                        $item.attr('data-bind', 'epWidget : {}').attr('data-id', data.id);

                        $item.addClass(data.cssClass).bind('delete', deleteWidget);
                        // widget contents
                        $contents = $('<div class="widget-contents"></div>').hide().appendTo($item);

                        // initialize widget
                        data.initializeFunction.call($contents, data, function () {
                            $('.loading-widget', $item).fadeOut('fast', function () {
                                var $this = $(this);
                                $this.parent().removeClass('loading');
                                $this.remove();

                                $contents.fadeIn('fast');

                                ko.applyBindings(newWidget, $item.get(0));

                                // trigger initialized event
                                $item.trigger('');
                            });
                        });
                    },
                        function () {
                            // something went wrong, cleanup
                            //$item.remove();
                        });
                }
            };
            // a widget is dragged into a new content area
            // or a new widget is dragged from the admin panel
            sortOptions.receive = function (event, ui) {
                var $item = $(ui.item).addClass('ep-widget'),
                    $helper = $(ui.helper),
                    widgetId = $item.attr('data-id'),
                    position = $('.ep-widget', $element).index(ui.item);

                if ($helper.hasClass('new-widget')) {
                    // tidy up the dragged node
                } else {
                    widget = config.findWidget(widgetId);
                    config.handlers.onMove(self, widget, position);
                }
            };

            sortOptions.start = function (event, ui) {
                //$(ui.item).style('position', 'relative');
            };

            $element.sortable(sortOptions);
        }
    };

    ko.bindingHandlers.epWidget = {
        'init': function (element, valueAccessor, allBindingsAccessor, widget) {
            var $widget = $(element);
            origZ = $widget.css('zIndex');

            $widget.hover(
				function () {
				    var $this = $(this),
						$handle = $('.widget-handle', $this);
				    $this.addClass('hover');

				    var z = $this.css('zIndex');
				    if (z === 'auto') {
				        $this.css('zIndex', '50000');
				    } else {
				        $this.css('zIndex', '');
				    }
				    $handle.position({ my: 'right bottom', at: 'right top', offset: '-1 0', of: $this });
				},
				function () {
				    $(this).removeClass('hover').css('zIndex', origZ);
				}
			);

            ko.renderTemplate("widgetHandleTemplate", widget, { templateEngine: templateEngine }, $('<div></div>').appendTo($widget), "replaceNode");
            // when complete notify any scripts wanting to do further work
            $widget.trigger('widget-initialized');
        }
    };

})();
