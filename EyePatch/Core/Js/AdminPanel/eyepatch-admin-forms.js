(function () {
    ep.forms = {
        errorPlacement: function (error, inputElement) {
            var container = inputElement.closest('form').find("[data-valmsg-for='" + inputElement[0].name + "']"),
                replace = $.parseJSON(container.attr("data-valmsg-replace")) !== false, text = error.text();

            container.removeClass("field-validation-valid").addClass("field-validation-error");
            error.data("unobtrusiveContainer", container);

            if (replace) {
                container.empty();
                error.attr('title', text).empty().removeClass("input-validation-error").appendTo(container).tipTip({ defaultPosition: 'right' });
            }
            else {
                error.hide();
            }
        },
        parse: function (form) {
            var $form = $(form);
            $.validator.unobtrusive.parse($form);
            $form.data('validator').settings.errorPlacement = ep.forms.errorPlacement;
            return $form;
        }
    };

    ko.bindingHandlers.prepareForm = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var $form = $(element);
            if (typeof valueAccessor() != "function")
                throw new Error("The value for a submit binding must be a function to invoke on submit");

            $('.help', $form).tipTip();

            $form.ajaxForm({
                type: 'POST',
                beforeSubmit: function (arr, $form, options) {
                    return ep.forms.parse($form).valid();
                },
                success: function (responseText, statusText, xhr, form) {
                    if (responseText.success) {
                        var value = valueAccessor();
                        value.call(viewModel, element, responseText);
                    } else {
                        $.noticeAdd({ text: responseText.message || "An error occurred", stay: false, type: 'error' });
                    }
                }
            });
        }
    };
} ());