  $(function () {

Highcharts.theme = {
   colors: ["#DDDF0D", "#7798BF", "#55BF3B", "#DF5353", "#aaeeee", "#ff0066", "#eeaaee",
      "#55BF3B", "#DF5353", "#7798BF", "#aaeeee"],
   chart: {
      backgroundColor: {
         linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
         stops: [
            [0, 'rgb(96, 96, 96)'],
            [1, 'rgb(16, 16, 16)']
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
         color: '#FFF',
         font: '16px Lucida Grande, Lucida Sans Unicode, Verdana, Arial, Helvetica, sans-serif'
      }
   },
   subtitle: {
      style: {
         color: '#DDD',
         font: '12px Lucida Grande, Lucida Sans Unicode, Verdana, Arial, Helvetica, sans-serif'
      }
   },
   xAxis: {
      gridLineWidth: 0,
      lineColor: '#999',
      tickColor: '#999',
      labels: {
         style: {
            color: '#999',
            fontWeight: 'bold'
         }
      },
      title: {
         style: {
            color: '#AAA',
            font: 'bold 12px Lucida Grande, Lucida Sans Unicode, Verdana, Arial, Helvetica, sans-serif'
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
            color: '#999',
            fontWeight: 'bold'
         }
      },
      title: {
         style: {
            color: '#AAA',
            font: 'bold 12px Lucida Grande, Lucida Sans Unicode, Verdana, Arial, Helvetica, sans-serif'
         }
      }
   },
   legend: {
      itemStyle: {
         color: '#CCC'
      },
      itemHoverStyle: {
         color: '#FFF'
      },
      itemHiddenStyle: {
         color: '#333'
      }
   },
   labels: {
      style: {
         color: '#CCC'
      }
   },
   tooltip: {
      backgroundColor: {
         linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
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
            color: '#CCC'
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
         lineColor: 'white'
      }
   },

   toolbar: {
      itemStyle: {
         color: '#CCC'
      }
   },

   navigation: {
      buttonOptions: {
         symbolStroke: '#DDDDDD',
         hoverSymbolStroke: '#FFFFFF',
         theme: {
            fill: {
               linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
               stops: [
                  [0.4, '#606060'],
                  [0.6, '#333333']
               ]
            },
            stroke: '#000000'
         }
      }
   },

   // scroll charts
   rangeSelector: {
      buttonTheme: {
         fill: {
            linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
            stops: [
               [0.4, '#888'],
               [0.6, '#555']
            ]
         },
         stroke: '#000000',
         style: {
            color: '#CCC',
            fontWeight: 'bold'
         },
         states: {
            hover: {
               fill: {
                  linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                  stops: [
                     [0.4, '#BBB'],
                     [0.6, '#888']
                  ]
               },
               stroke: '#000000',
               style: {
                  color: 'white'
               }
            },
            select: {
               fill: {
                  linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                  stops: [
                     [0.1, '#000'],
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
            linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
            stops: [
               [0.4, '#888'],
               [0.6, '#555']
            ]
         },
      barBorderColor: '#CCC',
      buttonArrowColor: '#CCC',
      buttonBackgroundColor: {
            linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
            stops: [
               [0.4, '#888'],
               [0.6, '#555']
            ]
         },
      buttonBorderColor: '#CCC',
      rifleColor: '#FFF',
      trackBackgroundColor: {
         linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
         stops: [
            [0, '#000'],
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
   maskColor: 'rgba(255,255,255,0.3)'
};

// Apply the theme
var highchartsOptions = Highcharts.setOptions(Highcharts.theme);

$( ".group-location, .group-tags" ).keypress(function( event ) {
  if ( event.which == 13 ) {
    event.preventDefault();

    getReport($("#from").val(), $("#to").val());
  }
});

           $("#from").datepicker({
               defaultDate: "+1w",
               changeMonth: true,
               numberOfMonths: 3,
               onClose: function (selectedDate) {
                   $("#to").datepicker("option", "minDate", selectedDate);
               }
           });
           $("#to").datepicker({
               defaultDate: "+1w",
               changeMonth: true,
               numberOfMonths: 3,
               onClose: function (selectedDate) {
                   $("#from").datepicker("option", "maxDate", selectedDate);
                   // Generate report
                  getReport($("#from").val(), $("#to").val());
               }
           });

           getReport($("#from").val(), $("#to").val());
       });

Array.prototype.getUnique = function(){
       var u = {}, a = [];
       for(var i = 0, l = this.length; i < l; ++i){
          if(u.hasOwnProperty(this[i])) {
             continue;
          }
          a.push(this[i]);
          u[this[i]] = 1;
       }
       return a;
      };

    var getReport = function(from, to)
    {
      $.getJSON( "http://localhost:3000/api/v0/analytics/monthlygrowth?startDate=" + encodeURIComponent(from) + "&endDate=" + encodeURIComponent(to) + "&city=" + encodeURIComponent($(".group-location").val()) + "&topics=" + encodeURIComponent($(".group-tags").val()) + "&api_key=special-key&neo4j=true", function( data ) {
        var table = $(".table-result-view tbody");

        $(table).empty();

        

        var categories = [];
        var series = [];
        var seriesVariance = [];

        // Collect unique dates for categories
        var categoryArr = [];
        var seriesName = [];

        $.each( data, function( key, val ) {
          categoryArr.push(val.month);
          seriesName.push(val.group);
        });

        categories = categoryArr.getUnique();
        var names = seriesName.getUnique();

        $.each(names, function(i, item)
        {
          var dataPoints = [];
          $.each(data, function(key, val)
          {
            if (val.group == item) 
            { 
              dataPoints.push(val.members);
            };
          });

          series.push({ name: item, data: dataPoints })
          seriesVariance.push({name: item, data: (jStat(dataPoints).max() - jStat(dataPoints).min()) / jStat([jStat(dataPoints).min(), 1]).max() })
          
        });

        seriesVariance.sort(function(a,b){return b.data-a.data});

        $.each( seriesVariance, function( key, val ) {
          var items = [];
          items.push( "<tr><td>" + val.name + "</td>" );
          items.push( "<td>" + numeral(val.data).format('0%') + "</td>" );
          items.push( "</tr>" );
          table.append(items.join());
        });

console.log(seriesVariance);

buildChart(categories, series);                
});
};

var buildChart = function(categories, series)
            {
              $('#container').highcharts({
                  title: {
                      text: $(".group-location").val() + ' Meetup Members in ' + $(".group-location").val(),
                      x: -20 //center
                  },
                  subtitle: {
                      text: 'Source: Meetup.com',
                      x: -20
                  },
                  xAxis: {
                      categories: categories
                  },
                  yAxis: {
                      title: {
                          text: 'Membership'
                      },
                      plotLines: [{
                          value: 0,
                          width: 1,
                          color: '#808080'
                      }]
                  },
                  tooltip: {
                      valueSuffix: ' Members'
                  },
                  legend: {
                      layout: 'vertical',
                      align: 'right',
                      verticalAlign: 'middle',
                      borderWidth: 0
                  },
                  series: series
              });
            };