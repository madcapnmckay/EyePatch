/*global document, window, $, ko, debug, setTimeout, alert */
(function () {
    // Private function
    var templateEngine = new ko.jqueryTmplTemplateEngine(),
        Thumbnail = function (url, image, vm) {
            this.src = url;
            var viewModel = vm;

            this.width = 101;
            this.height = 112;

            this.imageWidth = image.width || 1;
            this.imageHeight = image.height || 1;

            this.frameWidth = ko.dependentObservable(function () {
                return 0.9 * this.width;
            }, this);

            this.frameHeight = ko.dependentObservable(function () {
                return 0.6 * this.width;
            }, this);

            this.resizeRatio = function () {
                var widthRatio = this.frameWidth() / this.imageWidth,
							heightRatio = this.frameHeight() / this.imageHeight,
							ratio = widthRatio < heightRatio ? heightRatio : widthRatio;
                return ratio; // don't resize if it already fits in the frame							
            } .bind(this);

            this.imageResizedWidth = ko.dependentObservable(function () {
                return Math.ceil(this.resizeRatio() * this.imageWidth);
            }, this);

            this.imageResizedHeight = ko.dependentObservable(function () {
                return Math.ceil(this.resizeRatio() * this.imageHeight);
            }, this);

            this.isSelected = ko.dependentObservable(function () {
                return viewModel.imageSelected() && viewModel.imageSelected() === this;
            }, this);

            this.select = function () {
                viewModel.imageSelected(this);
            } .bind(this)
        };


    ko.imagePicker = {
        viewModel: function (configuration) {
            this.folderContents = ko.observableArray([]);
            this.uploadControl;
            this.upload = ko.observable(false);
            this.newFile = ko.observable('');
            this.currentFolder = ko.observable();
            this.isLoading = ko.observable(false);
            this.imageSelected = ko.observable();

            var hash = { '.tif': 1, '.jpg': 1, '.jpeg': 1, '.png': 1, '.bmp': 1, '.gif': 1 };

            var checkExtension = function (filename) {
                var re = /\..+$/;
                var ext = filename.match(re)[0].toLowerCase();
                if (hash[ext]) {
                    return true;
                } else {
                    return false;
                }
            }

            this.openDialog = function () {
                var that = this;
                this.uploadControl = $('#fileUpload');
                this.uploadControl.change(function () {
                    that.selected();
                });
                this.uploadControl.click();
            } .bind(this);

            this.uploadFile = function () {
                var that = this;
                if (!$.IsNullOrWhiteSpace(this.currentFolder())) {
                    $.ajaxFileUpload
		            (
			            {
			                url: ep.urls.mediaFolder.upload + "?parentId=" + this.currentFolder(),
			                secureuri: false,
			                fileElementId: this.uploadControl.attr('id'),
			                dataType: 'json',
			                success: function (data, status) {
			                    if (typeof (data.error) != 'undefined') {
			                        if (data.error != '') {
			                            alert(data.error);
			                        } else {
			                            var url = data.url, image = new Image();
			                            image.src = url;
			                            image.onload = function () {
			                                that.folderContents.push(new Thumbnail(url, this, that));
			                            }
			                            that.upload(false);
			                        }
			                    }
			                },
			                error: function (data, status, e) {
			                    alert(e);
			                }
			            }
		            );
                }
            } .bind(this);

            this.cancelUpload = function () {
                this.upload(false);
                this.newFile('');
            } .bind(this);

            this.validFile = ko.dependentObservable(function () {
                if ($.IsNullOrWhiteSpace(this.newFile())) {
                    return false;
                }
                return checkExtension(this.newFile());
            }, this);

            this.selected = function () {
                this.upload(true);
                this.newFile(this.uploadControl.val().replace('C:\\fakepath\\', ''));
            } .bind(this);

            this.deleteSelected = function () {
                var that = this;
                if (this.imageSelected()) {
                    ep.postJson(ep.urls.mediaFolder.removeImage, { id: this.imageSelected().src }, function (result) {
                        that.folderContents.remove(that.imageSelected());
                        that.imageSelected(undefined);
                    });
                }
            } .bind(this);

            this.display = function (node) {
                var parentId = node.id();
                this.folderContents([]);
                this.currentFolder(parentId);
                var that = this;
                // get the contents
                this.isLoading(true);
                ep.postJson(ep.urls.mediaFolder.info, { id: parentId }, function (result) {
                    var images = result.data, idx = 0,
						loadImage = function () {
						    if (idx >= images.length) {
						        that.isLoading(false);
						        return false;
						    }

						    var url = images[idx], image = new Image();
						    image.src = url;
						    image.onload = function () {
						        that.folderContents.push(new Thumbnail(url, this, that));
						        idx++;
						        loadImage();
						    }
						};
                    // start
                    loadImage();
                });
            } .bind(this);
        }
    };

    ko.addTemplateSafe("epThumbnailTemplate", "\
            <div class=\"ui-thumb\" data-bind=\"hover: 'hover', css: { selected: isSelected }, click: select\">\
				<div class=\"thumb-inner\" data-bind=\"style: { width: width + 'px', height: height + 'px' }\">\
					<div class=\"thumb-frame\" data-bind=\"style: { width: frameWidth() + 'px', height: frameHeight() + 'px' }\">\
						<img src=\"${ src }\" alt=\"${ altText }\" title=\"${ title }\" data-bind=\"attr: { width: imageResizedWidth(), height: imageResizedHeight() }\"/>\
					</div>\
				</div>\
			</div>", templateEngine);

    ko.addTemplateSafe("epImagePickerTemplate", "<div class=\"ui-imagepicker\">\
                                <div data-bind=\"visible: currentFolder\">\
                                    <div class=\"image-list\" data-bind=\"template: { name: 'epThumbnailTemplate', foreach: folderContents }\">\
                                    </div>\
                                    <div class=\"button-container\" data-bind=\"visible: !upload()\">\
                                        <div style=\"overflow:hidden;min-width: 200px;\">\
                                        <button class=\"button\" title=\"click to delete the selected image\" data-bind=\"enable: imageSelected() , click: deleteSelected\">Delete</button>\
                                        <button class=\"button\" title=\"click to add a new image\" data-bind=\"click: openDialog\">Add</button>\
                                        </div>\
                                    </div>\
                                    <div class=\"button-container\" data-bind=\"visible: upload\">\
                                        <div style=\"overflow:hidden;min-width: 220px;\">\
                                            <label class=\"upload-label\" data-bind=\"text: newFile\"></label>\
                                            <button class=\"button\" title=\"click to upload\" data-bind=\"click: uploadFile, enable: validFile\">Upload</button>\
                                            <form method=\"post\" enctype=\"multipart/form-data\">\
                                            <input id=\"fileUpload\" name=\"image\" class=\"upload\" type=\"file\" style=\"display:none;\"/>\
                                            <button class=\"button\" title=\"click to cancel\" data-bind=\"click: cancelUpload\">Cancel</button>\
                                            </form>\
                                        </div>\
                                    </div>\
                                </div>\
                            </div>", templateEngine);

    ko.bindingHandlers.imagePicker = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = valueAccessor(),
                container = element.appendChild(document.createElement("DIV"));
            ko.renderTemplate("epImagePickerTemplate",
                                value, { templateEngine: templateEngine }, container, "replaceNode");
        }
    }
} ());
