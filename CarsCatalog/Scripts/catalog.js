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

$(function() {
        my.contentResizing = function() {
            var carImgWidth = $(".car-img-wrap").width();
            $(".car-img-wrap").height(carImgWidth * 0.8);

            self.columnsCount = $("#columns-list").val();
            var contWidth = 670 + self.columnsCount * 50;
            $(".main-container").css("min-width", contWidth + "px");

            $("#main-content").css("padding", (self.columnsCount * 2) + "px");

            $(window).resize(function() {
                var carImgWidth = $(".car-img-wrap").width();
                $(".car-img-wrap").height(carImgWidth * 0.8);
                $("#main-content").css("width", (contWidth - $("#left-sidebar").width - 20) + "px");
            });
        };
   {
// todo add to knockout

            // set numbers columns and rows
            my.setItemsCountOnPage = function() {
                var elementsOnPage = $("#elements-per-page-list").val();
                self.columnsCount = $("#columns-list").val();
                document.cookie = "elementsOnPage=" + elementsOnPage;

                var fullUrl = Url + "SetElementsCountOnPage?elementsPerPage=" + elementsOnPage + "&columns=" + self.columnsCount;
                $.get(fullUrl, function(data) {
                    if (data === "ok") {
                        $("#main-content").css("min-width", (500 + self.columnsCount * 20) + "px");

                        var url = removeFilters(["page"]);
                        getContentFromUrl(url);
                    }
                });
            };

        // change items count
        my.itemsCountOnPageHandler = function() {
            $("#elements-per-page-list").change(function() {
                setItemsCountOnPage();
            });
        };
        // change columns count
        my.columnsCountChangedHandler = function() {
            $("#columns-list").change(function() {
                self.columnsCount = $("#columns-list").val();
                var mainWidth = $("#main-car-list").width();
                document.cookie = "columns=" + self.columnsCount;
                newElementsOnPageList(); // create new dropdown list for items count on page for new columns count
                setItemsCountOnPage(); // change content
                $(".car-wrapper").width(mainWidth / self.columnsCount);
            });
        };
    }

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
                            color(null);
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
            };
       
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
            resetFilter: resetFilterHandler
        }
    }(my.parseUrlsParams().filters);


    my.pageLinkClickHandler = function () {
        $(".page-link").click(function (event) {
            var pageLink = $(this).attr("href");
            event.preventDefault();

            window.history.replaceState({}, "", pageLink);


            $.get(pageLink, function (data) {
                $("#main-car-list")[0].innerHTML = data;
                my.vm.resetFilter();
                my.pageLinkClickHandler();
            });
        });
    };


    ko.applyBindings(my.vm);
});

//var
//          newElementsOnPageList = function () {
//              var selectedContent = "";
//              var itemsPerPage = [5, 10, 20];
//              for (var itemCount in itemsPerPage) {
//                  if (itemsPerPage.hasOwnProperty(itemCount)) {
//                      if (itemCount === '10') // selected by default
//                          selectedContent += "<option selected>" + itemCount + "</option>";
//                      else
//                          selectedContent += "<option>" + itemCount + "</option>";
//                  }
//              }
//              $("#elements-per-page-list")[0].innerHTML = selectedContent;
//          };

//function ViewCars() {
//    var self = this;

//    var Url = "/Catalog/";

//    this.init = function () {
//        this.columnsCount = 4;
//        this.elementsPerPage = 10;
//        this.contentResizing();
//    };

//    // parse url parameters into object
//    var parseUrl = function (url) {
//        var result = {};
//        url.split("&").forEach(function (part) {
//            var item = part.split("=");
//            result[item[0]] = decodeURIComponent(item[1]);
//        });
//        return result;
//    }
//    // from parameters build url
//    var buildUrl = function (parameters) {
//        var res = [];
//        for (var p in parameters) {
//            if (parameters.hasOwnProperty(p)) {
//                if (p === "page")
//                    continue;
//                res.push(encodeURIComponent(p) + "=" + encodeURIComponent(parameters[p]));
//            }
//        }
//        return res.join("&");
//    };
//    // get main content from url
//    var getContentFromUrl = function (urlSearch) {
//        var queryUrl = "/Catalog/GetCars" + urlSearch;

//        $.get(queryUrl, function (data) {
//            $("#main-car-list")[0].innerHTML = data;
//            window.history.replaceState({ "html": data }, "", urlSearch);
//            self.pageLinkClickHandler();
//            self.resetFilterHandler();
//            self.contentResizing(); //fix content size
//        });
//    };

//    // add filter to url and load data
//    var addParamsToUrl = function (params) {
//        var isNew = location.search === "";
//        var urlParams = {};
//        if (!isNew)
//            urlParams = parseUrl(location.search.substr(1));

//        for (var p in params) {
//            if (params.hasOwnProperty(p)) {
//                urlParams[p] = params[p];
//            }
//        }
//        var urlSearch = "?" + buildUrl(urlParams);
//        getContentFromUrl(urlSearch);
//    }
//    // remove filter
//    var removeFilters = function (parameters) {
//        var isNew = location.search === "";
//        var urlParams = {};
//        if (!isNew)
//            urlParams = parseUrl(location.search.substr(1));
//        else
//            return "/";

//        for (var i = 0; i < parameters.length; i++) {
//            delete urlParams[parameters[i]];
//        }
//        return "?" + buildUrl(urlParams);
//    }

//    // create new dropdown list for items count on page when columns count changed
//    var newElementsOnPageList = function () {
//        var selectedContent = "";
//        var itemsPerPage = [5, 10, 20];
//        for (var itemCount in itemsPerPage) {
//            if (itemsPerPage.hasOwnProperty(itemCount)) {
//                if (itemCount === '10') // selected by default
//                    selectedContent += "<option selected>" + itemCount + "</option>";
//                else
//                    selectedContent += "<option>" + itemCount + "</option>";
//            }
//        }
//        $("#elements-per-page-list")[0].innerHTML = selectedContent;
//    };
//    // set numbers columns and rows
//    var setItemsCountOnPage = function () {
//        var elementsOnPage = $("#elements-per-page-list").val();
//        self.columnsCount = $("#columns-list").val();
//        document.cookie = "elementsOnPage=" + elementsOnPage;

//        var fullUrl = Url + "SetElementsCountOnPage?elementsPerPage=" + elementsOnPage + "&columns=" + self.columnsCount;
//        $.get(fullUrl, function (data) {
//            if (data === "ok") {
//                $("#main-content").css("min-width", (500 + self.columnsCount * 20) + "px");

//                var url = removeFilters(["page"]);
//                getContentFromUrl(url);
//            }
//        });
//    };

//    // change items count
//    this.itemsCountOnPageHandler = function () {
//        $("#elements-per-page-list").change(function () {
//            setItemsCountOnPage();
//        });
//    };
//    // change columns count
//    this.columnsCountChangedHandler = function () {
//        $("#columns-list").change(function () {
//            self.columnsCount = $("#columns-list").val();
//            var mainWidth = $("#main-car-list").width();
//            document.cookie = "columns=" + self.columnsCount;
//            newElementsOnPageList(); // create new dropdown list for items count on page for new columns count
//            setItemsCountOnPage(); // change content
//            $(".car-wrapper").width(mainWidth / self.columnsCount);
//        });
//    };

//    // remove concrete filter
//    this.resetFilterHandler = function () {
//        if ($(".selected-filter") != null) {
//            $(".selected-filter").click(function () {
//                var id = this.id;
//                var urlUpdate = "/";
//                if (id === "remove-brand-filter") {
//                    urlUpdate = removeFilters(["BrandId"]);
//                }
//                if (id === "remove-model-filter") {
//                    urlUpdate = removeFilters(["ModelId"]);
//                }
//                if (id === "remove-color-filter") {
//                    urlUpdate = removeFilters(["Color"]);
//                    $("#color-autocomplete").val("");
//                }
//                if (id === "remove-engine-filter") {
//                    urlUpdate = removeFilters(["Engine"]);
//                    $("#engine-capacity-autocomplete").val("");
//                }
//                if (id === "remove-price-filter") {
//                    urlUpdate = removeFilters(["MinPrice", "MaxPrice", "Date"]);
//                    $("#datepicker").val("");
//                    $("#min-price").val("");
//                    $("#max-price").val("");
//                }
//                if (id === "remove-all-filters") {
//                    $("#engine-capacity-autocomplete").val("");
//                    $("#color-autocomplete").val("");
//                    $("#datepicker").val("");
//                    $("#min-price").val("");
//                    $("#max-price").val("");
//                }
//                getContentFromUrl(urlUpdate);
//            });
//        }
//    };
//    this.contentResizing = function () {
//        var carImgWidth = $(".car-img-wrap").width();
//        $(".car-img-wrap").height(carImgWidth * 0.8);

//        self.columnsCount = $("#columns-list").val();
//        var contWidth = 670 + self.columnsCount * 50;
//        $(".main-container").css("min-width", contWidth + "px");

//        $("#main-content").css("padding", (self.columnsCount * 2) + "px");

//        $(window).resize(function () {
//            var carImgWidth = $(".car-img-wrap").width();
//            $(".car-img-wrap").height(carImgWidth * 0.8);
//            $("#main-content").css("width", (contWidth - $("#left-sidebar").width - 20) + "px");
//        });
//    };

//    this.brandClick = function () {
//        $(".brand").click(function () {
//            var id = this.id;
//            var urlSearch = removeFilters(["ModelId"]);
//            window.history.replaceState({}, "", urlSearch);

//            addParamsToUrl({ "BrandId": id });
//        });
//    };

//    this.modelClick = function () {
//        $(".model").click(function () {
//            var id = this.id;
//            addParamsToUrl({ "ModelId": id });
//        });
//    };

//    this.datePriceRange = function () {
//        $("#date-with-price-range").click(function () {
//            var params = {};

//            var date = $("#datepicker").val();
//            var min = $("#min-price").val();
//            var max = $("#max-price").val();

//            if (!isNaN(min))
//                params["MinPrice"] = min;

//            if (!isNaN(max))
//                params["MaxPrice"] = max;

//            var checkDate = Date.parse(date);
//            if (!isNaN(checkDate) || date === "")
//                params["Date"] = date;
//            else
//                alert("wrong date format");

//            if (params[0] !== null)
//                addParamsToUrl(params);
//        });
//    };
//    // navigate on page
//    this.pageLinkClickHandler = function () {
//        $(".page-link").click(function (event) {
//            var pageLink = $(this).attr("href");
//            event.preventDefault();
//            getContentFromUrl(pageLink);
//        });
//    };

//    //get distinct data source and add filter
//    this.autocompleteFieldFilter = function (selector, distinctMethod, filterName) {
//        $(selector).autocomplete({
//            source: function (request, response) {
//                var urlParams = location.search;
//                var fullUrl = Url + distinctMethod + urlParams;
//                $.getJSON(fullUrl, function (data) {
//                    response(data);
//                });
//            },
//            select: function (event, ui) {
//                var params = {};
//                params[filterName] = ui.item.value;
//                addParamsToUrl(params);
//            }
//        });
//    };
//    // add filter for autocompleting fields when typing
//    this.fillAutocompletedContent = function (selector, getElementsQuery) {
//        $(selector).on("change paste keyup ", function () {
//            var value = $(selector).val();
//            var params = {};
//            params[getElementsQuery] = value;
//            addParamsToUrl(params);
//        });
//    };

//    // restore filers state from url
//    this.restoreFiltersState = function () {
//        var urlParams = parseUrl(location.search.substr(1));
//        if (urlParams["Engine"] != null)
//            $("#engine-capacity-autocomplete").val(urlParams["Engine"]);

//        if (urlParams["Color"] != null)
//            $("#color-autocomplete").val(urlParams["Color"]);

//        if (urlParams["Date"] != null)
//            $("#datepicker").val(urlParams["Date"]);

//        if (urlParams["MinPrice"] !== "")
//            $("#min-price").val(urlParams["MinPrice"]);

//        if (urlParams["MaxPrice"] != null)
//            $("#max-price").val(urlParams["MaxPrice"]);
//    }

//}

//viewCars = null;
//viewCars = new ViewCars();
//viewCars.init();

//viewCars.brandClick();
//viewCars.modelClick();
//viewCars.datePriceRange();

////get distinct colors source and filter selected value
//viewCars.autocompleteFieldFilter("#color-autocomplete", "GetDistinctColors", "Color");
//// add filter color when typing
//viewCars.fillAutocompletedContent("#color-autocomplete", "Color");

////get distinct engine source and filter selected value
//viewCars.autocompleteFieldFilter("#engine-capacity-autocomplete", "GetDistinctEngineCapacity", "Engine");
//// add filter engine when typing
//viewCars.fillAutocompletedContent("#engine-capacity-autocomplete", "Engine");

//viewCars.pageLinkClickHandler();
//// restore filers state from url
//viewCars.restoreFiltersState();

//// handle removing filter
//viewCars.resetFilterHandler();
//// change items count on page
//viewCars.itemsCountOnPageHandler();
//// change columns count
//viewCars.columnsCountChangedHandler();