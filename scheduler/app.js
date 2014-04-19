var importer = require('./import/importer');
var url = require("url");
var _ = require('underscore');
var meetup = require('meetup-api')('4b1b7a4c4027718192d1e73723135b');
var express = require('express');
var app = express();

var env = process.env.NODE_ENV || 'development';
if ('development' == env) {
    app.use('/dist/assets', express.static(__dirname + '/dist/assets'));
    app.use(express.static(__dirname + '/dist'));
}

var port = process.env.PORT || 3001;
app.listen(port, function () {
    console.log("Listening on " + port);
});


function writeResponse(res, response, start) {
    res.header('Duration-ms', new Date() - start);
    if (response.neo4j) {
        res.header('Neo4j', JSON.stringify(response.neo4j));
    }
    res.send(JSON.stringify(response.results));
}

function parseUrl(req, key) {
    return url.parse(req.url, true).query[key];
}

function parseBool(req, key) {
    return 'true' == url.parse(req.url, true).query[key];
}

//0 5 0 * * * 
//0 0-59 * * * *

var CronJob = require('cron').CronJob;
var job = new CronJob('0 5 0 * * *', function () {
      var cities = getPollingCities();
      iteratorCityCallback(0, cities.length, cities);
    }, function () {

    },
    true /* Start the job right now */ ,
    "America/Los_Angeles" /* Time zone of this job. */
);

function iteratorCityCallback(count, length, cities) {
    if (count < length) {
        // Get the city for this iteration
        var city = cities[count];

        // Import group stats for day
        meetup.getGroups({
            'topic': 'NoSQL',
            'country': city.country,
            'city': city.city,
            'state': city.state,
            'page': '100'
        }, function (err, groups) {
            //console.log(city);
            iteratorGroupCallback(0, groups.results.length, groups.results, count, length, cities);
        });
    }
}

function iteratorGroupCallback(count, length, groups, city_count, city_length, cities) {
    if (count < length) {
        // Get parameters from Meetup API
        var params = getGroupImportParameters(groups[count]);

        // Format parameters for Cypher REST API
        params = {
            'csvLine': params
        };

        var options = {
            neo4j: "neo4j"
        };

        // Import group stats for day
        importer.importGroupStats(params, options, function (err, response) {
            // Write the group name to the console
            console.log(response.results[0].name);

            // Call the iterator callback to iterate the data import process sequentially
            iteratorGroupCallback(count + 1, length, groups, city_count, city_length, cities);
        });
    } else {
        iteratorCityCallback(city_count + 1, city_length, cities);
    }
}

function getPollingCities() {
    var cities = [{
        city: "New York",
        state: "NY",
        country: "US"
    }, {
        city: "Boston",
        state: "MA",
        country: "US"
    }, {
        city: "Atlanta",
        state: "GA",
        country: "US"
    }, {
        city: "Seattle",
        state: "WA",
        country: "US"
    }, {
        city: "Los Angeles",
        state: "CA",
        country: "US"
    }, {
        city: "San Francisco",
        state: "CA",
        country: "US"
    }, {
        city: "Chicago",
        state: "IL",
        country: "US"
    }, {
        city: "Austin",
        state: "TX",
        country: "US"
    }, {
        city: "Dallas",
        state: "TX",
        country: "US"
    }, {
        city: "Denver",
        state: "CO",
        country: "US"
    }, {
        city: "Washington",
        state: "DC",
        country: "US"
    }, {
        city: "London",
        state: "",
        country: "GB"
    }, {
        city: "Paris",
        state: "",
        country: "FR"
    }, {
        city: "München",
        state: "",
        country: "DE"
    }, {
        city: "Berlin",
        state: "",
        country: "DE"
    }, {
        city: "Copenhagen",
        state: "",
        country: "DK"
    }, {
        city: "Stockholm",
        state: "",
        country: "SE"
    }, {
        city: "Malmö",
        state: "",
        country: "SE"
    }, {
        city: "Brussels",
        state: "",
        country: "BE"
    }, {
        city: "Frankfurt",
        state: "",
        country: "DE"
    }, {
        city: "Hamburg",
        state: "",
        country: "DE"
    }, {
        city: "Oslo",
        state: "",
        country: "NO"
    }];

    return cities;
}

function getGroupImportParameters(group) {

    var createdDate = getDateTimeFromUnix(group.created);
    var today = new Date();
    today = new Date((today.getMonth() + 1) + "/" + today.getDate() + "/" + today.getFullYear());
    var yesterday = new Date();
    yesterday.setDate(today.getDate() - 1);

    var csvLine = {
        group_name: group.name,
        group_creation_date: group.created,
        group_creation_date_year: createdDate.year,
        group_creation_date_month: createdDate.month,
        group_creation_date_day: createdDate.day,
        group_location: group.city,
        group_country: group.country,
        group_state: group.state,
        group_tag: group.topics.map(function (d) {
            return d.name;
        }),
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
    return csvLine;
}

function getTicks(dateTime) {

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

function getDateTimeFromUnix(unixTime) {
    // create a new javascript Date object based on the timestamp
    // multiplied by 1000 so that the argument is in milliseconds, not seconds
    var date = new Date(unixTime);
    // hours part from the timestamp
    var day = date.getDate();
    // minutes part from the timestamp
    var month = date.getMonth() + 1;
    // seconds part from the timestamp
    var year = date.getFullYear();

    return {
        day: day,
        month: month,
        year: year
    };
}