﻿function ViewCars() {
    _this = this;

    var Url = "/Catalog/";

    this.init = function () {

        this.columnsCount = 4;
        this.rowsCount = 5;
        this.contentResizing();
    };

    // parse url parameters into object
    var parseUrl = function (url) {
        var result = {};
        url.split("&").forEach(function (part) {
            var item = part.split("=");
            result[item[0]] = decodeURIComponent(item[1]);
        });
        return result;
    }
    // from parameters build url
    var buildUrl = function (parameters) {
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
    // get main content from url
    var getContentFromUrl = function (urlSearch) {

        var queryUrl = "/Catalog/GetCars" + urlSearch;

        $.get(queryUrl, function (data) {
            $("#main-car-list")[0].innerHTML = data;
            window.history.replaceState({ "html": data }, "", urlSearch);
            _this.pageLinkClickHandler();
            _this.resetFilterHandler();
            _this.contentResizing(); //fix content size
        });
    };

    // add filter to url and load data
    var addParamsToUrl = function (params) {
        var isNew = location.search === "";
        var urlParams = {};
        if (!isNew)
            urlParams = parseUrl(location.search.substr(1));

        for (var p in params) {
            if (params.hasOwnProperty(p)) {
                urlParams[p] = params[p];
            }
        }
        var urlSearch = "?" + buildUrl(urlParams);
        getContentFromUrl(urlSearch);
    }
    // remove filter
    var removeFilters = function (parameters) {
        var isNew = location.search === "";
        var urlParams = {};
        if (!isNew)
            urlParams = parseUrl(location.search.substr(1));
        else
            return "/";

        for (var i = 0; i < parameters.length; i++) {
            delete urlParams[parameters[i]];
        }
        return "?" + buildUrl(urlParams);
    }

    // create new dropdown list for items count on page when columns count changed
    var newElementsOnPageList = function () {
        var selectedContent = "";
        for (var row = 3; row < 10; row++) {
            if (row === 5) // selected by default
                selectedContent += "<option selected>" + row * _this.columnsCount + "</option>";
            else
                selectedContent += "<option>" + row * _this.columnsCount + "</option>";
        }
        $("#elements-per-page-list")[0].innerHTML = selectedContent;
    };
    // set numbers columns and rows
    var setItemsCountOnPage = function () {
        var elementsOnPage = $("#elements-per-page-list").val();
        _this.columnsCount = $("#columns-list").val();
        _this.rowsCount = elementsOnPage / _this.columnsCount;
        document.cookie = "rows=" + _this.rowsCount;

        var fullUrl = Url + "SetElementsCountOnPage?rows=" + _this.rowsCount + "&columns=" + _this.columnsCount;
        $.get(fullUrl, function (data) {
            if (data === "ok") {
                $("#main-content").css("min-width", (500 + _this.columnsCount * 20) + "px");

                var url = removeFilters(["page"]);
                getContentFromUrl(url);
            }
        });
    };
    
    // change items count
    this.itemsCountOnPageHandler = function () {
        $("#elements-per-page-list").change(function () {
            setItemsCountOnPage();
        });
    };
    // change columns count
    this.columnsCountChangedHandler = function () {
        $("#columns-list").change(function () {
            _this.columnsCount = $("#columns-list").val();
            var mainWidth = $("#main-car-list").width();
            document.cookie = "columns=" + _this.columnsCount;
            newElementsOnPageList(); // create new dropdown list for items count on page for new columns count
            setItemsCountOnPage(); // change content
            $(".car-wrapper").width(mainWidth / _this.columnsCount);
        });
    };

    // remove concrete filter
    this.resetFilterHandler = function () {
        if ($(".selected-filter") != null) {
            $(".selected-filter").click(function () {
                var id = this.id;
                var urlUpdate = "/";
                if (id === "remove-brand-filter") {
                    urlUpdate = removeFilters(["BrandId"]);
                }
                if (id === "remove-model-filter") {
                    urlUpdate = removeFilters(["ModelId"]);
                }
                if (id === "remove-color-filter") {
                    urlUpdate = removeFilters(["Color"]);
                    $("#color-autocomplete").val("");
                }
                if (id === "remove-engine-filter") {
                    urlUpdate = removeFilters(["Engine"]);
                    $("#engine-capacity-autocomplete").val("");
                }
                if (id === "remove-price-filter") {
                    urlUpdate = removeFilters(["MinPrice", "MaxPrice", "Date"]);
                    $("#datepicker").val("");
                    $("#min-price").val("");
                    $("#max-price").val("");
                }
                if (id === "remove-all-filters") {
                    $("#engine-capacity-autocomplete").val("");
                    $("#color-autocomplete").val("");
                    $("#datepicker").val("");
                    $("#min-price").val("");
                    $("#max-price").val("");
                }
                getContentFromUrl(urlUpdate);
            });
        }
    };
    this.contentResizing = function () {
        var carImgWidth = $(".car-img-wrap").width();
        $(".car-img-wrap").height(carImgWidth * 0.8);

        _this.columnsCount = $("#columns-list").val();
        var contWidth = 670 + _this.columnsCount * 50;
        $(".main-container").css("min-width", contWidth + "px");


        $("#main-content").css("padding", (_this.columnsCount * 2) + "px");

        $(window).resize(function () {
            var carImgWidth = $(".car-img-wrap").width();
            $(".car-img-wrap").height(carImgWidth * 0.8);
            $("#main-content").css("width", (contWidth - $("#left-sidebar").width - 20) + "px");
        });
    };

    this.brandClick = function () {
        $(".brand").click(function () {
            var id = this.id;
            var urlSearch = removeFilters(["ModelId"]);
            window.history.replaceState({}, "", urlSearch);

            addParamsToUrl({ "BrandId": id });
        });
    };

    this.modelClick = function () {
        $(".model").click(function () {
            var id = this.id;
            addParamsToUrl({ "ModelId": id });
        });
    };

    this.datePriceRange = function () {
        $("#date-with-price-range").click(function () {

            var params = {};

            var date = $("#datepicker").val();
            var min = $("#min-price").val();
            var max = $("#max-price").val();

            if (!isNaN(min))
                params["MinPrice"] = min;

            if (!isNaN(max))
                params["MaxPrice"] = max;

            var checkDate = Date.parse(date);
            if (!isNaN(checkDate) || date === "")
                params["Date"] = date;
            else
                alert("wrong date format");

            if (params[0] !== null)
                addParamsToUrl(params);
        });
    };
    // navigate on page
    this.pageLinkClickHandler = function () {
        $(".page-link").click(function (event) {
            var pageLink = $(this).attr("href");
            event.preventDefault();
            getContentFromUrl(pageLink);
        });
    };

    //get distinct data source and add filter
    this.autocompleteFieldFilter = function (selector, distinctMethod, filterName) {
        $(selector).autocomplete({
            source: function (request, response) {
                var urlParams = location.search;
                var fullUrl = Url + distinctMethod + urlParams;
                $.getJSON(fullUrl, function (data) {
                    response(data);
                });
            },
            select: function (event, ui) {
                var params = {};
                params[filterName] = ui.item.value;
                addParamsToUrl(params);
            }
        });
    };
    // add filter for autocompleting fields when typing
    this.fillAutocompletedContent = function (selector, getElementsQuery) {
        $(selector).on("change paste keyup ", function () {
            var value = $(selector).val();
            var params = {};
            params[getElementsQuery] = value;
            addParamsToUrl(params);
        });
    };

    // restore filers state from url
    this.restoreFiltersState = function () {
        var urlParams = parseUrl(location.search.substr(1));
        if (urlParams["Engine"] != null)
            $("#engine-capacity-autocomplete").val(urlParams["Engine"]);

        if (urlParams["Color"] != null)
            $("#color-autocomplete").val(urlParams["Color"]);

        if (urlParams["Date"] != null)
            $("#datepicker").val(urlParams["Date"]);

        if (urlParams["MinPrice"] !== "")
            $("#min-price").val(urlParams["MinPrice"]);

        if (urlParams["MaxPrice"] != null)
            $("#max-price").val(urlParams["MaxPrice"]);
    }

}

viewCars = null;
viewCars = new ViewCars();
viewCars.init();

viewCars.brandClick();
viewCars.modelClick();
viewCars.datePriceRange();

//get distinct colors source and filter selected value
viewCars.autocompleteFieldFilter("#color-autocomplete", "GetDistinctColors", "Color");
// add filter color when typing
viewCars.fillAutocompletedContent("#color-autocomplete", "Color");

//get distinct engine source and filter selected value
viewCars.autocompleteFieldFilter("#engine-capacity-autocomplete", "GetDistinctEngineCapacity", "Engine");
// add filter engine when typing
viewCars.fillAutocompletedContent("#engine-capacity-autocomplete", "Engine");

viewCars.pageLinkClickHandler();
// restore filers state from url
viewCars.restoreFiltersState();

// handle removing filter
viewCars.resetFilterHandler();
// change items count on page
viewCars.itemsCountOnPageHandler();
// change columns count
viewCars.columnsCountChangedHandler();