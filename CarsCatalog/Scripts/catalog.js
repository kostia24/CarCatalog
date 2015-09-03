var my = my || {};
my.parseUrlsParams = function () {
    var result = {};
    var isNew = location.search === "";
    if (isNew)
        return result;

    var urlToParse = location.search.substr(1);

    urlToParse.split("&").forEach(function (part) {
        var item = part.split("=");
        result[item[0]] = decodeURIComponent(item[1]);
    });
    return { filters: result };
};
my.buildUrl = function (parameters) {
    var res = [];
    for (var p in parameters) {
        if (parameters.hasOwnProperty(p)) {
            if (p === "page")
                continue;
            res.push(encodeURIComponent(p) + "=" + encodeURIComponent(parameters[p]));
        }
    }
    return res.join("&");
};

$(function () {
    my.vm = function (filters) {

        if (filters == null)
            filters = {};

        var
            modelId = ko.observable(filters.ModelId),
            brandId = ko.observable(filters.BrandId),
            engine = ko.observable(filters.Engine),
            color = ko.observable(filters.Color),
            minPrice = ko.observable(filters.MinPrice),
            maxPrice = ko.observable(filters.MaxPrice),
            date = ko.observable(filters.Date),
            elementsOnPage = ko.observable(12),
            columnsCount = ko.observable(4),
            changed = ko.observable(false),
            contentResizing = function () {
                var carImgWidth = $(".car-img-wrap").width();
                $(".car-img-wrap").height(carImgWidth * 0.8);

                var columns = columnsCount();
                var contWidth = 670 + columns * 50;
                $(".main-container").css("min-width", contWidth + "px");

                $("#main-content").css("padding", (columns * 2) + "px");

                $(window).resize(function () {
                    var carImgWidth = $(".car-img-wrap").width();
                    $(".car-img-wrap").height(carImgWidth * 0.8);
                    $("#main-content").css("width", (contWidth - $("#left-sidebar").width - 20) + "px");
                });
            },
            resetFilterHandler = function () {
                if ($(".selected-filter") != null) {
                    $(".selected-filter").click(function () {
                        var id = this.id;
                        if (id === "remove-brand-filter") {
                            brandId(null);
                        }
                        if (id === "remove-model-filter") {
                            modelId(null);
                        }
                        if (id === "remove-color-filter") {
                            color();
                        }
                        if (id === "remove-engine-filter") {
                            engine(null);
                        }
                        if (id === "remove-price-filter") {
                            minPrice(null);
                            maxPrice(null);
                            date(null);
                        }
                        if (id === "remove-all-filters") {
                            if (modelId() != null)
                                modelId(null);
                            if (brandId() != null)
                                brandId(null);
                            if (engine() != null)
                                engine(null);
                            if (color() != null)
                                color(null);
                            if (minPrice() != null)
                                minPrice(null);
                            if (maxPrice() != null)
                                maxPrice(null);
                            if (date() != null)
                                date(null);
                        }
                    });
                }
            },
            getFilteredContent = ko.computed(function () {
                var urlParams = {};
                changed();

                if (brandId() != null)
                    urlParams["BrandId"] = brandId();

                if (modelId() != null)
                    urlParams["ModelId"] = modelId();

                if (engine() != null)
                    urlParams["Engine"] = engine();

                if (color() != null)
                    urlParams["Color"] = color();

                if (date() != null && date() !== "")
                    urlParams["Date"] = date();

                if (minPrice() != null && minPrice() !== "")
                    urlParams["MinPrice"] = minPrice();

                if (maxPrice() != null && maxPrice() !== "")
                    urlParams["MaxPrice"] = maxPrice();

                var urlSearch = my.buildUrl(urlParams);
                if (urlSearch !== "")
                    urlSearch = "?" + urlSearch;
                else
                    urlSearch = "/";

                var queryUrl = "/Catalog/GetCars" + urlSearch;
                window.history.replaceState({}, "", urlSearch);


                $.get(queryUrl, function (data) {
                    $("#main-car-list")[0].innerHTML = data;
                    resetFilterHandler();
                    my.pageLinkClickHandler();
                    contentResizing();
                });
            }),
            choseBrand = function (brand, event) {
                if (modelId() != null)
                    modelId(null);

                var id = event.target.id;
                brandId(id);
            },
            choseModel = function (model, event) {
                var id = event.toElement.parentNode.id;
                modelId(id);
            },
            setItemsCountOnPage = ko.computed(function () {
                var itemsOnPage = elementsOnPage(),
                columns = columnsCount();
                document.cookie = "elementsPerPage=" + itemsOnPage;
                document.cookie = "columns=" + columns;
                
                var fullUrl = "/Catalog/SetElementsCountOnPage?elementsPerPage=" + itemsOnPage + "&columns=" + columns;
                $.get(fullUrl, function (data) {
                    if (data === "ok") {
                        $("#main-content").css("min-width", (500 + columnsCount * 20) + "px");
                        changed(!changed());
                        var mainWidth = $("#main-car-list").width();
                        $(".car-wrapper").width(mainWidth / self.columnsCount);
                    }
                });
            });

        return {
            BrandId: brandId,
            ModelId: modelId,
            Engine: engine,
            Color: color,
            MinPrice: minPrice,
            MaxPrice: maxPrice,
            Date: date,
            choseBrand: choseBrand,
            choseModel: choseModel,
            resetFilter: resetFilterHandler,
            columnsCount: columnsCount,
            elementsOnPage: elementsOnPage,
            setItemsCountOnPage: setItemsCountOnPage,
            getContent: getFilteredContent,
            resizeContent: contentResizing
        }
    }(my.parseUrlsParams().filters);

    my.pageLinkClickHandler = function () {
        $(".page-link").click(function (event) {
            var pageLink = $(this).attr("href"),
                fullUrl = "/Catalog/GetCars" + pageLink;
            
            event.preventDefault();

            window.history.replaceState({}, "", pageLink);

            $.get(fullUrl, function (data) {
                $("#main-car-list")[0].innerHTML = data;
                my.vm.resetFilter();
                my.pageLinkClickHandler();
                my.vm.resizeContent();
            });
        });
    };

    ko.applyBindings(my.vm);
    my.vm.resizeContent();
});