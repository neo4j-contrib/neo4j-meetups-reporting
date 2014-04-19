var express = require('express');
var path = require('path');
var favicon = require('static-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var importer = require('./import/importer');
var routes = require('./routes/index');
var users = require('./routes/users');
var url = require("url");
var _ = require('underscore');
var meetup = require('meetup-api')('4b1b7a4c4027718192d1e73723135b');

var app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

app.use(favicon());
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded());
app.use(cookieParser());
app.use(require('stylus').middleware(path.join(__dirname, 'public')));
app.use(express.static(path.join(__dirname, 'public')));

app.use('/', routes);
app.use('/users', users);

/// catch 404 and forwarding to error handler
app.use(function(req, res, next) {
    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});

/// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use(function(err, req, res, next) {
        res.status(err.status || 500);
        res.render('error', {
            message: err.message,
            error: err
        });
    });
}

// production error handler
// no stacktraces leaked to user
app.use(function(err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});

var server = app.listen(3001, function() {
    console.log('Listening on port %d', server.address().port);
});

module.exports = app;


function writeResponse (res, response, start) {
  res.header('Duration-ms', new Date() - start);
  if (response.neo4j) {
    res.header('Neo4j', JSON.stringify(response.neo4j));
  }
  res.send(JSON.stringify(response.results));
}

function parseUrl(req, key) {
  return url.parse(req.url,true).query[key];
}

function parseBool (req, key) {
  return 'true' == url.parse(req.url,true).query[key];
}

//0 5 0 * * * 
//0 0-59 * * * *

var CronJob = require('cron').CronJob;
var job = new CronJob('0 5 0 * * *', function(){

    var options = {
      neo4j: "neo4j"
    };

    getPollingCities().forEach(function(city) {

      meetup.getGroups({'topic' : 'NoSQL', 'country' : city.country, 'city' : city.city, 'state' : city.state, 'page' : '100'}, function(err,groups) {
        console.log(city);
        groups.results.forEach(function(group) {

          // Get parameters from Meetup API
          var params = getGroupImportParameters(group);
          
          // Format parameters for Cypher REST API
          params = { 'csvLine' : params };

          // Import group stats for day
          importer.importGroupStats(params, options, function (err, response) {
            console.log(response.results[0].name);
          });

        });
        
      });

    });
    
  }, function () {
    
  },
  true /* Start the job right now */,
  "America/Los_Angeles" /* Time zone of this job. */
);

function getPollingCities()
{
  var cities = [
    {
        city: "New York",
        state: "NY",
        country: "US"
    },
    {
        city: "Boston",
        state: "MA",
        country: "US"
    },
    {
        city: "Atlanta",
        state: "GA",
        country: "US"
    },
    {
        city: "Seattle",
        state: "WA",
        country: "US"
    },
    {
        city: "Los Angeles",
        state: "CA",
        country: "US"
    },
    {
        city: "San Francisco",
        state: "CA",
        country: "US"
    },
    {
        city: "Chicago",
        state: "IL",
        country: "US"
    },
    {
        city: "Austin",
        state: "TX",
        country: "US"
    },
    {
        city: "Dallas",
        state: "TX",
        country: "US"
    },
    {
        city: "Denver",
        state: "CO",
        country: "US"
    },
    {
        city: "London",
        state: "",
        country: "GB"
    },
    {
        city: "Paris",
        state: "",
        country: "FR"
    },
    {
        city: "München",
        state: "",
        country: "DE"
    },
    {
        city: "Berlin",
        state: "",
        country: "DE"
    },
    {
        city: "Copenhagen",
        state: "",
        country: "DK"
    },
    {
        city: "Stockholm",
        state: "",
        country: "SE"
    },
    {
        city: "Malmö",
        state: "",
        country: "SE"
    },
    {
        city: "Brussels",
        state: "",
        country: "BE"
    },
    {
        city: "Frankfurt",
        state: "",
        country: "DE"
    },
    {
        city: "Hamburg",
        state: "",
        country: "DE"
    },
    {
        city: "Oslo",
        state: "",
        country: "NO"
    }
  ];

  return cities;
}

function getGroupImportParameters(group)
{

  var createdDate = getDateTimeFromUnix(group.created);
  var today = new Date();
  var yesterday = new Date();
  yesterday.setDate(yesterday.getDate() - 1);

  var csvLine = {
      group_name: group.name,
      group_creation_date: group.created,
      group_creation_date_year: createdDate.year,
      group_creation_date_month: createdDate.month,
      group_creation_date_day: createdDate.day,
      group_location: group.city,
      group_country: group.country,
      group_state: group.state,
      group_tag: group.topics.map(function(d) { return d.name; }),
      last_month: yesterday.getMonth() + 1,
      last_day: yesterday.getDate(),
      last_year: yesterday.getFullYear(),
      group_stats: group.members,
      month: today.getMonth() + 1,
      day: today.getDate(),
      year: today.getFullYear(),
      day_timestamp: getTicks(today),
      day_of_week: today.getDay() + 1,
      week_of_year: getWeekOfYear(today)
  };

  console.log(csvLine);

  return csvLine;
}

function getTicks(dateTime)
{

  var ticks = ((dateTime.getTime() * 10000) + 621355968000000000);

  return ticks;
}

function getWeekOfYear(d) {
  // Attribution: https://gist.github.com/dblock/1081513

  // Create a copy of this date object
  var target = new Date(d.valueOf());
  // ISO week date weeks start on monday
  // so correct the day number
  var dayNr = (d.getDay() + 6) % 7;
   
  // Set the target to the thursday of this week so the
  // target date is in the right year
  target.setDate(target.getDate() - dayNr + 3);
   
  // ISO 8601 states that week 1 is the week
  // with january 4th in it
  var jan4 = new Date(target.getFullYear(), 0, 4);
   
  // Number of days between target date and january 4th
  var dayDiff = (target - jan4) / 86400000;
   
  // Calculate week number: Week 1 (january 4th) plus the
  // number of weeks between target date and january 4th
  var weekNr = 1 + Math.ceil(dayDiff / 7);
   
  return weekNr;
}

function getDateTimeFromUnix(unixTime)
{
  // create a new javascript Date object based on the timestamp
  // multiplied by 1000 so that the argument is in milliseconds, not seconds
  var date = new Date(unixTime);
  // hours part from the timestamp
  var day = date.getDate();
  // minutes part from the timestamp
  var month = date.getMonth() + 1;
  // seconds part from the timestamp
  var year = date.getFullYear();

  return { day: day, month: month, year: year };
}