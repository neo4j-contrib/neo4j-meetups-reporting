var locationTypeahead;
var countries;
var cities;

function initTypeahead(source) {
    $('.typeahead').typeahead('destroy').unbind();
    var myTypeahead = $('.typeahead').typeahead({}, {
        name: 'countries',
        displayKey: 'name',
        highlight: true,
        source: source.ttAdapter()
    }).on('typeahead:opened', onOpened)
        .on('typeahead:selected', onAutocompleted)
        .on('typeahead:autocompleted', onSelected);


    return myTypeahead;
}

$(function () {

    countries = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('name'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            // url points to a json file that contains an array of country names, see
            // https://github.com/twitter/typeahead.js/blob/gh-pages/data/countries.json
            url: 'http://localhost:3000/api/v0/analytics/countries?api_key=special-key&neo4j=true&cache=no',
            // the json file contains an array of strings, but the Bloodhound
            // suggestion engine expects JavaScript objects so this converts all of
            // those strings
            filter: function (list) {
                return $.map(list, function (country) {
                    return {
                        name: country
                    };
                });
            }
        }
    });

    // kicks off the loading/processing of `local` and `prefetch`
    countries.initialize();

    cities = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('name'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 10,
        prefetch: {
            // url points to a json file that contains an array of country names, see
            // https://github.com/twitter/typeahead.js/blob/gh-pages/data/countries.json
            url: 'http://localhost:3000/api/v0/analytics/cities?api_key=special-key&neo4j=true&cache=no',
            // the json file contains an array of strings, but the Bloodhound
            // suggestion engine expects JavaScript objects so this converts all of
            // those strings
            filter: function (list) {
                return $.map(list, function (country) {
                    return {
                        name: country
                    };
                });
            }
        }
    });

    // kicks off the loading/processing of `local` and `prefetch`
    countries.initialize();
    cities.initialize();

    // passing in `null` for the `options` arguments will result in the default
    // options being used
    locationTypeahead = initTypeahead(countries);

});

function onOpened($e) {
    console.log('opened');
}

function onAutocompleted($e, datum) {
    getReport($("#from").val(), $("#to").val());
}

function onSelected($e, datum) {
    console.log('selected');
    console.log(datum);
}

$(function () {

    Highcharts.theme = {
        colors: ["#338529", "#f68709", "#37c89a", "#6c7093", "#2dd2a6", "#ff0066", "#5ba483",
            "#f60940", "#dc6faf", "#c7c043", "#b6f7a6"
        ],
        chart: {
            backgroundColor: {
                linearGradient: {
                    x1: 0,
                    y1: 0,
                    x2: 0,
                    y2: 1
                },
                stops: [
                    [0, 'rgb(255, 255, 255)'],
                    [1, 'rgb(255, 255, 255)']
                ]
            },
            borderWidth: 0,
            borderRadius: 0,
            plotBackgroundColor: null,
            plotShadow: false,
            plotBorderWidth: 0
        },
        title: {
            style: {
                color: '#333',
                font: '16px "Open Sans", sans-serif'
            }
        },
        subtitle: {
            style: {
                color: '#333',
                font: '12px "Open Sans", sans-serif'
            }
        },
        xAxis: {
            gridLineWidth: 0,
            lineColor: '#333',
            tickColor: '#333',
            labels: {
                style: {
                    color: '#333',
                    fontWeight: 'normal'
                }
            },
            title: {
                style: {
                    color: '#333',
                    font: 'bold 12px "Open Sans", sans-serif'
                }
            }
        },
        yAxis: {
            alternateGridColor: null,
            minorTickInterval: null,
            gridLineColor: 'rgba(255, 255, 255, .1)',
            minorGridLineColor: 'rgba(255,255,255,0.07)',
            lineWidth: 0,
            tickWidth: 0,
            labels: {
                style: {
                    color: '#333',
                    fontWeight: 'normal'
                }
            },
            title: {
                style: {
                    color: '#333',
                    font: 'normal 12px "Open Sans", sans-serif'
                }
            }
        },
        legend: {
            itemStyle: {
                color: '#333'
            },
            itemHoverStyle: {
                color: '#428BCA'
            },
            itemHiddenStyle: {
                color: '#CCC'
            }
        },
        labels: {
            style: {
                color: '#333'
            }
        },
        tooltip: {
            backgroundColor: {
                linearGradient: {
                    x1: 0,
                    y1: 0,
                    x2: 0,
                    y2: 1
                },
                stops: [
                    [0, 'rgba(96, 96, 96, .8)'],
                    [1, 'rgba(16, 16, 16, .8)']
                ]
            },
            borderWidth: 0,
            style: {
                color: '#FFF'
            }
        },


        plotOptions: {
            series: {
                nullColor: '#444444'
            },
            line: {
                dataLabels: {
                    color: '#333'
                },
                marker: {
                    lineColor: '#333'
                }
            },
            spline: {
                marker: {
                    lineColor: '#333'
                }
            },
            scatter: {
                marker: {
                    lineColor: '#333'
                }
            },
            candlestick: {
                lineColor: 'black'
            }
        },

        toolbar: {
            itemStyle: {
                color: '#333'
            }
        },

        navigation: {
            buttonOptions: {
                symbolStroke: '#DDDDDD',
                hoverSymbolStroke: '#FFFFFF',
                theme: {
                    fill: {
                        linearGradient: {
                            x1: 0,
                            y1: 0,
                            x2: 0,
                            y2: 1
                        },
                        stops: [
                            [0.4, '#FFF'],
                            [0.6, '#FFF']
                        ]
                    },
                    stroke: '#CCC'
                }
            }
        },

        // scroll charts
        rangeSelector: {
            buttonTheme: {
                fill: {
                    linearGradient: {
                        x1: 0,
                        y1: 0,
                        x2: 0,
                        y2: 1
                    },
                    stops: [
                        [0.4, '#888'],
                        [0.6, '#555']
                    ]
                },
                stroke: '#000000',
                style: {
                    color: '#CCC',
                    fontWeight: 'normal'
                },
                states: {
                    hover: {
                        fill: {
                            linearGradient: {
                                x1: 0,
                                y1: 0,
                                x2: 0,
                                y2: 1
                            },
                            stops: [
                                [0.4, '#BBB'],
                                [0.6, '#888']
                            ]
                        },
                        stroke: '#000000',
                        style: {
                            color: 'black'
                        }
                    },
                    select: {
                        fill: {
                            linearGradient: {
                                x1: 0,
                                y1: 0,
                                x2: 0,
                                y2: 1
                            },
                            stops: [
                                [0.1, '#333'],
                                [0.3, '#333']
                            ]
                        },
                        stroke: '#000000',
                        style: {
                            color: 'yellow'
                        }
                    }
                }
            },
            inputStyle: {
                backgroundColor: '#333',
                color: 'silver'
            },
            labelStyle: {
                color: 'silver'
            }
        },

        navigator: {
            handles: {
                backgroundColor: '#666',
                borderColor: '#AAA'
            },
            outlineColor: '#CCC',
            maskFill: 'rgba(16, 16, 16, 0.5)',
            series: {
                color: '#7798BF',
                lineColor: '#A6C7ED'
            }
        },

        scrollbar: {
            barBackgroundColor: {
                linearGradient: {
                    x1: 0,
                    y1: 0,
                    x2: 0,
                    y2: 1
                },
                stops: [
                    [0.4, '#888'],
                    [0.6, '#555']
                ]
            },
            barBorderColor: '#CCC',
            buttonArrowColor: '#CCC',
            buttonBackgroundColor: {
                linearGradient: {
                    x1: 0,
                    y1: 0,
                    x2: 0,
                    y2: 1
                },
                stops: [
                    [0.4, '#888'],
                    [0.6, '#555']
                ]
            },
            buttonBorderColor: '#CCC',
            rifleColor: '#FFF',
            trackBackgroundColor: {
                linearGradient: {
                    x1: 0,
                    y1: 0,
                    x2: 0,
                    y2: 1
                },
                stops: [
                    [0, '#333'],
                    [1, '#333']
                ]
            },
            trackBorderColor: '#666'
        },

        // special colors for some of the demo examples
        legendBackgroundColor: 'rgba(48, 48, 48, 0.8)',
        legendBackgroundColorSolid: 'rgb(70, 70, 70)',
        dataLabelsColor: '#444',
        textColor: '#E0E0E0',
        maskColor: 'rgba(0,0,0,0.3)'
    };




    // Apply the theme
    var highchartsOptions = Highcharts.setOptions(Highcharts.theme);

    $(".meetup-location-btn").click(function () {
        $("#meetup-location").val($(this).text().toLowerCase());
        $(".meetup-location-btn").removeClass("selected");
        $(this).addClass("selected");
        $(".group-location").val("");
        $(".group-location").attr("placeholder", $(this).text());

        initTypeahead($(this).text() == "City" ? cities : countries);
    });

    $(".group-location, .group-tags").keypress(function (event) {
        if (event.which == 13) {
            event.preventDefault();

            getReport($("#from").val(), $("#to").val());
        }
    });

    var today = new Date();

    var todayMinusYear = new Date();

    todayMinusYear.setDate(today.getDate() - 365); // minus the date

    $("#from").val(todayMinusYear.getMonth() + 1 + "/" + todayMinusYear.getDate() + "/" + todayMinusYear.getFullYear());

    $("#to").val(today.getMonth() + 1 + "/" + today.getDate() + "/" + today.getFullYear());

    $("#from").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        numberOfMonths: 1,
        onClose: function (selectedDate) {
            $("#to").datepicker("option", "minDate", selectedDate);
        }
    });
    $("#to").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        numberOfMonths: 1,
        onClose: function (selectedDate) {
            $("#from").datepicker("option", "maxDate", selectedDate);
            // Generate report
            getReport($("#from").val(), $("#to").val());
        }
    });

    getReport($("#from").val(), $("#to").val());
});

Array.prototype.getUnique = function () {
    var u = {}, a = [];
    for (var i = 0, l = this.length; i < l; ++i) {
        if (u.hasOwnProperty(this[i])) {
            continue;
        }
        a.push(this[i]);
        u[this[i]] = 1;
    }
    return a;
};

function monthDiff(d1, d2) {
    var months;
    months = (d2.getFullYear() - d1.getFullYear()) * 12;
    months -= d1.getMonth() + 1;
    months += d2.getMonth();
    return months <= 0 ? 0 : months;
}

var getReport = function (from, to) {
    $.getJSON("http://localhost:3000/api/v0/analytics/monthlygrowth?startDate=" + encodeURIComponent(from) + "&endDate=" + encodeURIComponent(to) + "&" + $("#meetup-location").val() + "=" + encodeURIComponent(document.getElementById("location").value) + "&topics=" + encodeURIComponent($(".group-tags").val()) + "&api_key=special-key&neo4j=true", function (data) {
        var table = $(".table-result-view tbody");

        $(table).empty();

        // Get months between the dates
        var monthCount = monthDiff(new Date(from), new Date(to));
        console.log(monthCount);

        var categories = [];
        var series = [];
        var seriesVariance = [];

        // Collect unique dates for categories
        var categoryArr = [];
        var seriesName = [];

        $.each(data, function (key, val) {
            categoryArr.push(val.month);
            seriesName.push(val.group);
        });

        categories = categoryArr.getUnique();
        var names = seriesName.getUnique();

        $.each(names, function (i, item) {
            var dataPoints = [];
            var dataPointValues = [];


            $.each(data, function (key, val) {
                if (val.group == item) {
                    dataPointsArr = [];
                    var thisDateTime = new Date(val.month);
                    var utcDateTime = Date.UTC(thisDateTime.getUTCFullYear(), thisDateTime.getUTCMonth(), thisDateTime.getUTCDate());
                    dataPointsArr.push(utcDateTime);
                    dataPointsArr.push(val.members);
                    dataPoints.push(dataPointsArr);
                    dataPointValues.push(val.members);
                };
            });

            series.push({
                name: item,
                data: dataPoints
            })

            // Only measure annual percent growth for groups older than a year
            if (dataPointValues.length > monthCount) {
                seriesVariance.push({
                    name: item,
                    data: (jStat(dataPointValues).max() - jStat(dataPointValues).min()) / jStat([jStat(dataPointValues).min(), 1]).max()
                });
            }
        });

        seriesVariance.sort(function (a, b) {
            return b.data - a.data
        });

        $.each(seriesVariance, function (key, val) {
            var items = [];
            items.push("<tr><td>" + val.name + "</td>");
            items.push("<td>" + numeral(val.data).format('0%') + "</td>");
            items.push("</tr>");
            table.append(items.join());
        });

        buildChart(categories, series);
    });
};

var buildChart = function (categories, series) {
    $(series).each(function (i, item) {
        item.fillColor = {
            linearGradient: [0, 0, 0, 300],
            stops: [
                [0, "#4572A7"],
                [1, "rgba(2,0,0,0)"]
            ]
        };
    });

    console.log(series);
    var chartOptions = {
        chart: {
            renderTo: 'container',
            type: 'line'
        },
        title: {
            text: $(".group-tags").val() + ' Meetup Members in ' + document.getElementById("location").value,
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'top',
            x: 0,
            y: 100
        },
        subtitle: {
            text: 'Meetup.com data'
        },
        plotOptions: {
            series: {
                lineWidth: 2
            }
        },
        xAxis: {
            type: 'datetime',
            dateTimeLabelFormats: { // don't display the dummy year
                month: '%b %y',
                year: '%y',
                day: '%d'
            }
        },
        yAxis: {
            title: {
                text: 'Group Members'
            },
            min: 0,
            gridLineColor: '#DDDDDD'
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.series.name + '</b><br/>' +
                    Highcharts.dateFormat("%b '%y", this.x) + ' - ' + this.y + ' members';
            }
        },

        series: series
    };

    window.chart = new Highcharts.Chart(chartOptions);


};