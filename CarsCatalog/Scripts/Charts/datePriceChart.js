var chart = function chartPrice(chartData) {
    var chart = AmCharts.makeChart("chartdiv", {
        "type": "serial",
        "theme": "light",
        "marginRight": 80,
        "autoMarginOffset": 20,
        "marginTop": 7,
        "dataProvider": chartData,
        "valueAxes": [{
            "axisAlpha": 0.2,
            "dashLength": 1,
            "position": "left"
        }],
        "mouseWheelZoomEnabled": true,
        "graphs": [{
            "id": "g1",
            "balloonText": "[[category]]<br/><b><span style='font-size:14px;'>price: $[[value]]</span></b>",
            "bullet": "round",
            "bulletBorderAlpha": 1,
            "bulletColor": "#FFFFFF",
            "hideBulletsCount": 50,
            "title": "red line",
            "valueField": "price",
            "useLineColorForBulletBorder": true
        }],
        "chartScrollbar": {
            "graph": "g1",
            "oppositeAxis": false,
            "offset": 30,
            "scrollbarHeight": 40,
            "backgroundAlpha": 0,
            "selectedBackgroundAlpha": 0.1,
            "selectedBackgroundColor": "#888888",
            "graphFillAlpha": 0,
            "graphLineAlpha": 0.5,
            "selectedGraphFillAlpha": 0,
            "selectedGraphLineAlpha": 1,
            "autoGridCount": true,
            "color": "#AAAAAA"
        },
        "chartCursor": {

        },
        "categoryField": "date",
        "categoryAxis": {
            "parseDates": true,
            "axisColor": "#DADADA",
            "dashLength": 1,
            "minorGridEnabled": true
        },
        "export": {
            "enabled": true
        }
    });

    function zoomChart() {
        chart.zoomToIndexes(0, chartData.length - 1);
    }

    chart.addListener("rendered", zoomChart);
    zoomChart();

}

init = function () {
    if (chartData.length !== 0) {
        $("#chartdiv").addClass("chartdiv");

        chart(chartData);
    }
}

init();
