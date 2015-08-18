function ViewCars() {
    _this = this;

    var Url = "/Catalog/";
    var lastFullUrl = "";

    this.init = function () {
        this.ContentResizing();
        this.columnsCount = 4;
        this.PageNumber = 1;
        this.BrandModelFilterString = null;
        this.ShowAll();
        this.BrandClick();
        this.ModelClick();
        this.ColorFilterAutocomplete();
        this.EngineCapacityFilterAutocomplete();
        this.DatePriceRange();
        this.ElementsOnPage();
        this.ElementsOnColumns();
        this.CreatePage(1);
    };

    var getContentFromUrl = function (getElementsQuery, value) {
        var fullUrl = Url + getElementsQuery + value;
        if (_this.BrandModelFilterString != null)
            fullUrl += _this.BrandModelFilterString;
        lastFullUrl = fullUrl;
        $.get(fullUrl, function (data) {
            $("#mainCarList")[0].innerHTML = data;
            $("#paging-navigation")[0].innerHTML = "";
            _this.ContentResizing();
            _this.CreatePage(1);
        });
    };

    var autocompleteFieldFilter = function (selector, distinctQuery, getElementsQuery) {
        $(selector).autocomplete({
            source: function (request, response) {
                var value = $(selector).val();
                var fullUrl = Url + distinctQuery + value;

                if (_this.BrandModelFilterString != null)
                    fullUrl += _this.BrandModelFilterString;

                lastFullUrl = fullUrl;
                $.getJSON(fullUrl, function (data) {
                    response(data);
                });
            },
            select: function (event, ui) {
                getContentFromUrl(getElementsQuery, ui.item.value);
            }
        });
    };

    var fillAutocompletedContent = function (selector, getElementsQuery) {
        $(selector).on("change paste keyup ", function () {
            var value = $(selector).val();
            getContentFromUrl(getElementsQuery, value);
        });
    };

    var elementsOnPageChanged = function () {
        var elementsOnPage = $("#ElementsPerPageList").val();
        document.cookie = "elementsPerPage=" + elementsOnPage;
        _this.columnsCount = $("#ColumnsList").val();
        var fullUrl = Url + "SetElementsCountOnPage?elementsCount=" + elementsOnPage + "&columns=" + _this.columnsCount;
        $.get(fullUrl, function (data) {
            if (data === "ok") {
                $("#main-content").css("min-width", (500 + _this.columnsCount * 20) + "px");
                if (lastFullUrl === "")
                    lastFullUrl = Url + "GetCars?";
                $.get(lastFullUrl, function (dataCArs) {
                    $("#mainCarList")[0].innerHTML = dataCArs;
                    $("#paging-navigation")[0].innerHTML = "";
                    _this.ContentResizing();
                    _this.CreatePage(1);
                });
            }
        });
    };

    var newElementsOnPageList = function () {
        var selectedContent = "";
        for (var i = _this.columnsCount * 3; i <= _this.columnsCount * 7; i++) {
            if (i % _this.columnsCount === 0) {
                if (i === _this.columnsCount * 5)
                    selectedContent += "<option selected>" + i + "</option>";
                else
                    selectedContent += "<option>" + i + "</option>";
            }
        }
        $("#ElementsPerPageList")[0].innerHTML = selectedContent;

        document.cookie = "elementsPerPage=" + (_this.columnsCount * 5);
        elementsOnPageChanged();
    };

    var pageNextLinkClickHendler = function (pageNumber) {
        $("#page-next").click(function () {
            if (lastFullUrl === "")
                lastFullUrl = Url + "GetCars?";
            var fullUrl = lastFullUrl + "&page=" + (pageNumber + 1);
            $.get(fullUrl, function (data) {
                $(".page-content").fadeOut(300)
                 .promise().done(function () {
                     $("#mainCarList").append(data);
                     _this.ContentResizing();
                     $("#page" + (pageNumber+1)).fadeIn(300);
                 });
                $(".pageLink").addClass(" btn-default");
                $(".pageLink").removeClass("selected btn-primary");
                _this.CreatePage(pageNumber + 1);
            });
        });
    };

    var pageLinkClickHendler = function () {
        $(".pageLink").click(function () {
            var pageid = this.id;
            var id = (/\d+$/).exec(pageid);

            $(".pageLink").removeClass("selected btn-primary");
            $(".pageLink").addClass(" btn-default");
            $(this).addClass("selected btn-primary");


            if ($("#page" + id)[0] == null) {
                if (lastFullUrl === "")
                    lastFullUrl = Url + "GetCars?";
                var fullUrl = lastFullUrl + "&page=" + (id);
                $.get(fullUrl, function (data) {
                    $(".page-content").fadeOut(300)
                        .promise().done(function () {
                            $("#mainCarList").append(data);
                            _this.ContentResizing();
                         $("#page" + id).fadeIn(300);
                    });

                    $(".pageLink").removeClass("selected btn-default");
                    $(this).addClass(" btn-primary");
                });
            }
            else {
                $(".page-content").fadeOut(300)
                    .promise().done(function () { $("#page" + id).fadeIn(200) });
            }
        });
    };

    var resetFilters = function () {
        $("#engine-capacity-autocomplete").val("");
        $("#color-autocomplete").val("");
    };

    this.CreatePage = function (pageNumber) {
        if ($(".page-count")[0] == null)
            return;
        var pageCount = $(".page-count")[0].innerText;
        if (pageNumber <= 5) {
            _this.CreateFirstsPages();
            return;
        }
        var pageNumberContent;

        if (pageCount > 5) {
            var pageTag = ' <div id="page-link' + pageNumber + '" class="pageLink btn btn-primary">' + pageNumber + ' </div>';
            var pageNext = '<div id="page-next" class="nextPageLink btn btn-default">next</div>';
            $("#page-next").remove();
            if (pageNumber == pageCount) {
                pageNumberContent = pageTag;
                $("#paging-navigation").append(pageNumberContent);
                pageLinkClickHendler();
            } else {
                pageNumberContent = pageTag + pageNext;
                $("#paging-navigation").append(pageNumberContent);
                pageLinkClickHendler();
                pageNextLinkClickHendler(pageNumber);
            }
        }
    };

    this.CreateFirstsPages = function () {
        var pageCount = $(".page-count")[0].innerText;
        if (pageCount < 2)
            return;

        var pageNumberContent = ' <div id="page-link1" class="pageLink btn btn-primary">1 </div>';
        
        for (var i = 2; i <= pageCount; i++) {
            if (i > 5)
                break;
            pageNumberContent += ' <div id="page-link' + i + '" class="pageLink btn btn-default">' + i + ' </div>';
        }
        $("#paging-navigation").append(pageNumberContent);
        pageLinkClickHendler();

        if (pageCount > 5) {
            var pageNext = '<div id="page-next" class="nextPageLink btn btn-default">next</div>';
            $("#paging-navigation").append(pageNext);
            pageNextLinkClickHendler(5);
        }
    };

    this.ElementsOnPage = function () {
        $("#ElementsPerPageList").change(function () {
            elementsOnPageChanged();
        });
    };

    this.ElementsOnColumns = function () {
        $("#ColumnsList").change(function () {
            _this.columnsCount = $("#ColumnsList").val();
            var mainWidth = $("#mainCarList").width();
            document.cookie = "columns=" + _this.columnsCount;
            newElementsOnPageList(_this.columnsCount);

            $(".car-wrapper").width(mainWidth / _this.columnsCount);
        });
    };

    this.ShowAll = function () {
        $("#show-all").click(function () {
            _this.BrandModelFilterString = "";
            resetFilters();
            getContentFromUrl("GetCars?", "");
        });
    };

    this.BrandClick = function () {
        $(".brand").click(function () {
            var id = this.id;
            resetFilters();
            _this.BrandModelFilterString = "";
            getContentFromUrl("GetCarsByBrand?brandId=", id);
            _this.BrandModelFilterString = "&brandId=" + id;
        });
    };

    this.ModelClick = function () {
        $(".model").click(function () {
            var id = this.id;
            resetFilters();
            _this.BrandModelFilterString = "";
            getContentFromUrl("GetCarsByModel?modelId=", id);
            _this.BrandModelFilterString = "&modelId=" + id;
        });
    };

    this.DatePriceRange = function () {

        $("#datepicker").datepicker({
            changeMonth: true,
            changeYear: true
        });
        $("#datepicker").datepicker("option", "dateFormat", "mm/dd/yy");

        $("#date-with-price-range").click(function () {
            resetFilters();
            var date = $("#datepicker").val();
            var min = $("#min-price").val();
            var max = $("#max-price").val();
            var checkDate = Date.parse(date);
            if (date != null && max != null && min && !isNaN(checkDate) && !isNaN(min) && !isNaN(max))
                getContentFromUrl("GetCarsByPrice?date=" + date + "&min=" + min + "&max=", max);
            else alert("Please verify input data");
        });
    };

    this.ColorFilterAutocomplete = function () {
        autocompleteFieldFilter("#color-autocomplete", "GetDistinctColors?color=", "GetCarsByColor?color=");
        fillAutocompletedContent("#color-autocomplete", "GetCarsByColor?color=");
    };

    this.EngineCapacityFilterAutocomplete = function () {
        autocompleteFieldFilter("#engine-capacity-autocomplete", "GetDistinctEngineCapacity?capacity=", "GetCarsByEngineCapacity?capacity=");
        fillAutocompletedContent("#engine-capacity-autocomplete", "GetCarsByEngineCapacity?capacity=");
    };
    this.ContentResizing = function() {
        var carImgWidth = $(".car-img-wrap").width();
        $(".car-img-wrap").height(carImgWidth * 0.8);

        _this.columnsCount = $("#ColumnsList").val();
        $("#main-content").css("min-width", (500 + _this.columnsCount * 20) + "px");

        $("#main-content").css("padding", (_this.columnsCount * 2) + "px");

        $(window).resize(function () {
            var carImgWidth = $(".car-img-wrap").width();
            $(".car-img-wrap").height(carImgWidth * 0.8);
        });
    }
}

viewCars = null;
viewCars = new ViewCars();
viewCars.init();
