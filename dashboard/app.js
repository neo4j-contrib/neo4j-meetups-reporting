var express = require('express');
var app = express();

var env = process.env.NODE_ENV || 'development';
if ('development' == env) {
  app.use('/dist/assets', express.static(__dirname + '/dist/assets'));
  app.use(express.static(__dirname + '/dist'));
}

var port = process.env.PORT || 5000;
app.listen(port, function() {
  console.log("Listening on " + port);
});