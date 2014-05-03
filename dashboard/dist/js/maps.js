var bubble_map;
var width;
var height;

function initDataMap(bubbles)
{
  var tags = bubbles.map(function(d) { return d.tag; }).getUnique().reverse();
  var colors = ["#F25A29", "#AD62CE", "#30B6AF", "#FCC940", "#FF6C7C", "#4356C0", "#DFE1E3"];
  var colorMap = [];
  var colorMapString = "{ 'defaultFill' : '#ABDDA4', ";
  for(var i = 0; i < tags.length; i++)
  {
    colorMap.push(" '" + tags[i] + "' : " + "'" + colors[i % colors.length] + "'" + (i < (length - 1) ? ", " : ""));
  }

  colorMapString = (colorMapString + colorMap.join() +  " }").replace(/'/g, "\"");

  var fillMap = JSON.parse(colorMapString);

  $("#bubbles").empty();
  $(".map").width($(".data-map .panel-body").width());
  bubble_map = new Datamap({
    scope: "usa",
    setProjection: function(element) {

      width = element.offsetWidth;
      height = element.offsetHeight;

    projection = d3.geo.albersUsa()
      .scale(600)
      .translate([element.offsetWidth / 2, element.offsetHeight / 2]);
    var path = d3.geo.path()
      .projection(projection);
    
    return {path: path, projection: projection};
  },
     
    element: document.getElementById("bubbles"),
    geographyConfig: {
      popupOnHover: false,
      highlightOnHover: false
    },

    fills: fillMap
    
  });
  bubble_map.bubbles(bubbles, {
    borderWidth: .5,
        borderColor: '#FFFFFF',
        popupOnHover: true,
        fillOpacity: 0.30,
        highlightOnHover: true,
        highlightFillColor: '#FC8D59',
        highlightBorderColor: 'rgba(250, 15, 160, 0.2)',
        highlightBorderWidth: .5,
        highlightFillOpacity: 0.85,
    popupTemplate: function(geo, data) {
      return '<div class="hoverinfo">' + "<b>" + data.name + "</b>" + ": <br/>" + data.tag + '<br/>' + data.growth + '</div>';
    },
    done: function() {
      zoomMap();
     }
  });
}

var projection, centered, svg, path, g;

var zoomMap = function()
{


 svg = d3.select(".datamap");

projection = d3.geo.albersUsa()
      .scale(600)
      .translate([width/ 2, height / 2]);

 path = d3.geo.path()
    .projection(projection);

svg.selectAll("path")
    .on("click", clicked);

 g = svg.selectAll(".datamaps-subunits, .bubbles");



};

function clicked(d) {
  var x, y, k;

  if (d && centered !== d) {
    var centroid = path.centroid(d);
    x = centroid[0];
    y = centroid[1];
    k = 4;
    centered = d;
  } else {
    x = width / 2;
    y = height / 2;
    k = 1;
    centered = null;
  }

  g.selectAll("path")
      .classed("active", centered && function(d) { return d === centered; });

  g.transition()
      .duration(750)
      .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")scale(" + k + ")translate(" + -x + "," + -y + ")")
      .style("stroke-width", 1.5 / k + "px");
}



