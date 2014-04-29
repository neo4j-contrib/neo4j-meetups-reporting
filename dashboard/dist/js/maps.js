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

  var bubble_map = new Datamap({
    scope: "usa",
    setProjection: function(element) {
    var projection = d3.geo.albersUsa()
      .scale(500)
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
        fillOpacity: 0.75,
        highlightOnHover: true,
        highlightFillColor: '#FC8D59',
        highlightBorderColor: 'rgba(250, 15, 160, 0.2)',
        highlightBorderWidth: .5,
        highlightFillOpacity: 0.85,
    popupTemplate: function(geo, data) {
      return '<div class="hoverinfo">' + "<b>" + data.name + "</b>" + ": <br/>" + data.tag + '<br/>' + data.growth + '</div>';
    }
  });
}
