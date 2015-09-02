ko.bindingHandlers.autoComplete = {
    init: function (element, valueAccessor) {
        var settings = valueAccessor();
        var distinctMethod = settings.methodForDistinct;
        var observedField = settings.observe;
        var updateElementValue = function(event, ui) {
            event.preventDefault();
            if (ui.item != null) {
                $(element).val(ui.item.value);
                observedField(ui.item.value);
            }
            else {
                $(element).val(null);
                observedField(null);
            }
        }
        $(element).autocomplete({
            source: function (request, response) {
                observedField(request.term);
                var urlParams = location.search;
                var fullUrl = distinctMethod + urlParams;
                $.getJSON(fullUrl, function (data) {
                    response(data);
                });
            },
            select: function (event, ui) {
                updateElementValue(event, ui);
            },
            change: function (event, ui) {
                updateElementValue(event, ui);
            }
        });
    }
};