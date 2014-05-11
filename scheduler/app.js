var importer = require('./import/importer');
var url = require("url");
var _ = require('underscore');
var meetup = require('meetup-api')('4b1b7a4c4027718192d1e73723135b');
var geocoder = require('geocoder');
var geoLocationCache = [];
 
function parseUrl(req, key) {
    return url.parse(req.url, true).query[key];
}

function parseBool(req, key) {
    return 'true' == url.parse(req.url, true).query[key];
}

scheduledImport();
//trackOnNewGroupsByCity("Neo4j");

function scheduledImport()
{
    var options = {
        neo4j: "neo4j"
    };

    var count = 0;
    var length;

    // Get tracked groups
    var groups = importer.getTrackedGroups({}, options, function (err, groups) {
        // Write the group name to the console

        length = groups.results[0].groups.length;

        var chunkSize = 200;

        // Slice groups into groups of 200
        var groupSlices = length / chunkSize;
        var groupBatches = [];

        for(var i = 0; i < groupSlices; i++)
        {
            var block = (i * chunkSize) + chunkSize;
            block = block > length ? (i * chunkSize) + (length - (i * chunkSize)) : block;
            // Slice group of 200
            groupBatches.push(groups.results[0].groups.slice(i * chunkSize, block));
        }

        // Call the iterator callback to iterate the data import process sequentially
        iteratorTrackGroupBatchCallback(0, groupBatches.length, groupBatches);

        // Slice each group call into batches of 100
        //console.log(groups.results[0].groups.join(', '));
    });
}

// Begin tracking on new groups
function trackOnNewGroupsByCity(topic)
{
    var cities = getPollingCities();
    iteratorCityCallback(0, cities.length, cities, topic);
}

function iteratorTrackGroupBatchCallback(batchCount, batchLength, batchGroups) {
    console.log(batchCount + " " + batchLength + " " + batchGroups.length)
    if (batchCount < batchLength) {
        // Import group stats for day
        meetup.getGroups({
            'group_id': batchGroups[batchCount].join(', ')
        }, function (err, groups) {
            // Call the iterator callback to iterate the data import process sequentially
            iteratorTrackGroupCallback(0, groups.results.length, groups.results, batchCount, batchLength, batchGroups);
        });
    }
    else
    {
      console.log("Import complete");
      process.exit();
    }
}

function iteratorTrackGroupCallback(count, length, groups, batchCount, batchLength, batchGroups) {
    if (count < length) {
        var group = groups[count];
        var key = getGeoLocationKey(group);

        //Check geo location cache
        if (!geoLocationCache[key]) {
            var callback = geoCodeTrackedGroupCallback(count, length, groups, batchCount, batchLength, batchGroups);
            console.log(key);
            getCityGeocode(groups[count], callback);
        }
        else
        {
            importTrackedGroupStats(count, length, groups, batchCount, batchLength, batchGroups);   
        }
    } else {
        iteratorTrackGroupBatchCallback(batchCount + 1, batchLength, batchGroups);
    }
}


function iteratorCityCallback(count, length, cities, topic) {
    if (count < length) {
        // Get the city for this iteration
        var city = cities[count];

        // Import group stats for day
        meetup.getGroups({
            'topic': topic,
            'country': city.countryCode,
            'city': city.city,
            'state': city.state,
            'page': '100'
        }, function (err, groups) {
            // Call the iterator callback to iterate the data import process sequentially
            iteratorGroupCallback(0, groups.results.length, groups.results, count, length, cities, topic);
        });
    }
    else
    {
      console.log("Import complete");
      process.exit();
    }
}

function geoCodeTrackedGroupCallback(count, length, groups, batchCount, batchLength, batchGroups) {
    var key = getGeoLocationKey(groups[count]);

    return function(err, data) {
        if (!err) {
            var coordinates = data.results[0].geometry.location;
            geoLocationCache[key] = { lat: coordinates.lat, lon: coordinates.lng };
        }
        else
        {
            return { lat: '', lon: '' };
        }

        importTrackedGroupStats(count, length, groups, batchCount, batchLength, batchGroups);       
    };
};

function geoCodeCallback(count, length, groups, city_count, city_length, cities, topic) {
    var key = getGeoLocationKey(groups[count]);

    return function(err, data) {
        if (!err) {
            var coordinates = data.results[0].geometry.location;
            geoLocationCache[key] = { lat: coordinates.lat, lon: coordinates.lng };
        }
        else
        {
            return { lat: '', lon: '' };
        }

        importGroupStats(count, length, groups, city_count, city_length, cities, topic);       
    };
};

function importTrackedGroupStats(count, length, groups, batchCount, batchLength, batchGroups)
{
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

        iteratorTrackGroupCallback(count + 1, length, groups, batchCount, batchLength, batchGroups);
    });
}

function importGroupStats(count, length, groups, city_count, city_length, cities, topic)
{
    // Get parameters from Meetup API
    var params = getGroupImportParameters(groups[count], (cities ? cities[city_count] : undefined ));

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

        iteratorGroupCallback(count + 1, length, groups, city_count, city_length, cities, topic);
    });
}

function iteratorGroupCallback(count, length, groups, city_count, city_length, cities, topic) {
    if (count < length) {
        var group = groups[count];
        var key = getGeoLocationKey(group);

        //Check geo location cache
        if (!geoLocationCache[key]) {
            var callback = geoCodeCallback(count, length, groups, city_count, city_length, cities, topic);
            console.log(key);
            getCityGeocode(groups[count], callback);
        }
        else
        {
            importGroupStats(count, length, groups, city_count, city_length, cities, topic);   
        }

    } else {
        iteratorCityCallback(city_count + 1, city_length, cities, topic);
    }
}

function getGeoLocationKey(group)
{
    return group.city + ", " + (group.state ? group.state : group.country);
}

function getCityGeocode(group, callback)
{
    var key = getGeoLocationKey(group);
    return geocoder.geocode(key, callback);
}

function getPollingCities() {
    var cities = [{
        city: "New York",
        state: "NY",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "Boston",
        state: "MA",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "Atlanta",
        state: "GA",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "Seattle",
        state: "WA",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "Los Angeles",
        state: "CA",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "San Francisco",
        state: "CA",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "Chicago",
        state: "IL",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "Austin",
        state: "TX",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "Dallas",
        state: "TX",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "Denver",
        state: "CO",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "Washington",
        state: "DC",
        countryCode: "US",
        countryName: "United States"
    }, {
        city: "London",
        state: "",
        countryCode: "GB",
        countryName: "Great Britain"
    }, {
        city: "Paris",
        state: "",
        countryCode: "FR",
        countryName: "France"
    }, {
        city: "München",
        state: "",
        countryCode: "DE",
        countryName: "Germany"
    }, {
        city: "Berlin",
        state: "",
        countryCode: "DE",
        countryName: "Germany"
    }, {
        city: "Copenhagen",
        state: "",
        countryCode: "DK",
        countryName: "Denmark"
    }, {
        city: "Stockholm",
        state: "",
        countryCode: "SE",
        countryName: "Sweden"
    }, {
        city: "Malmö",
        state: "",
        countryCode: "SE",
        countryName: "Sweden"
    }, {
        city: "Brussels",
        state: "",
        countryCode: "BE",
        countryName: "Belgium"
    }, {
        city: "Frankfurt",
        state: "",
        countryCode: "DE",
        countryName: "Germany"
    }, {
        city: "Hamburg",
        state: "",
        countryCode: "DE",
        countryName: "Germany"
    }, {
        city: "Oslo",
        state: "",
        countryCode: "NO",
        countryName: "Norway"
    }];

    return cities;
}

function getGroupImportParameters(group, city) {

    var createdDate = getDateTimeFromUnix(group.created);
    var today = new Date();
    today = new Date((today.getMonth() + 1) + "/" + today.getDate() + "/" + today.getFullYear());
    var yesterday = new Date();
    yesterday.setDate(today.getDate() - 1);
    var key = getGeoLocationKey(group);
    var countryCodeDictionary = [];

    if (!city) {
        getPollingCities().map(function(d)
        {
            countryCodeDictionary[d.countryCode] = d.countryName;
            return d.countryName;
        });

        city = { countryName: countryCodeDictionary[group.country] };
    };

    var csvLine = {
        group_name: group.name,
        group_creation_date: group.created,
        group_creation_date_year: createdDate.year,
        group_creation_date_month: createdDate.month,
        group_creation_date_day: createdDate.day,
        group_location: group.city,
        group_country: group.country,
        group_country_name: city.countryName,
        group_location_lat: geoLocationCache[key].lat,
        group_location_lon: geoLocationCache[key].lon,
        group_state: group.state,
        group_tag: group.topics.map(function (d) {
            return [d.name.toLowerCase(), d.name];
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
        week_of_year: getWeekOfYear(today),
        lat: group.lat,
        lon: group.lon,
        group_id: group.id
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