// movies.js
var Analytics = require('../models/analytics');
var sw = require("swagger-node-express");
var param = sw.params;
var url = require("url");
var swe = sw.errors;
var _ = require('underscore');


/*
 *  Util Functions
 */

function writeResponse (res, response, start) {
  sw.setHeaders(res);
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


/*
 * API Specs and Functions
 */

exports.getWeeklyGrowthPercent = {
  'spec': {
    "description" : "Get weekly growth percent of meetup groups as a time series.",
    "path" : "/analytics/weeklygrowth",
    "notes" : "Returns a set of data points containing the week of the year, the meetup group name, and membership count.",
    "summary" : "Get the time series that models the growth percent of meetup groups week over week.",
    "method": "GET",
    "params" :  [
      param.query("startDate", "A date to retrieve results from. Results will be returned for the entire week that the start date occurs within.", "string", true, true),
      param.query("endDate", "A date to retrieve results until. Results will be returned for the entire week that the start date occurs within.", "string", true, true),
      param.query("city", "The city name where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
      param.query("topics", "A list of topics that a meetup group must have to be returned in the result set. Multiple topic names should be delimited by a comma.", "string", true, true),
      param.query("groups", "A list of names to match on meetup groups, only groups with the name that are specified in the list are returned. Multiple topic names should be delimited by a comma. Leave blank to ignore this field.", "string", false, false)
    ],
    "responseClass" : "List[Analytics]",
    "errorResponses" : [],
    "nickname" : "getWeeklyGrowthPercent"
  },
  'action': function (req, res) {
    var options = {
      neo4j: parseBool(req, 'neo4j')
    };

    var startDate = parseUrl(req, 'startDate');
    var endDate = parseUrl(req, 'endDate');
    var location = parseUrl(req, 'city');
    var topics = _.invoke(parseUrl(req, 'topics').split(','), 'trim');
    var groups = parseUrl(req, 'groups') ? _.invoke(parseUrl(req, 'groups').split(','), 'trim') : [];
    
    var dateFrom = new Date(startDate);
    var dateTo = new Date(endDate);

    var params = {
      startDate: { day: dateFrom.getDate(), month: dateFrom.getMonth(), year: dateFrom.getFullYear() },
      endDate: { day: dateTo.getDate(), month: dateTo.getMonth(), year: dateTo.getFullYear() },
      city: location,
      topics: topics,
      groups: groups
    };

    var start = new Date();
    Analytics.getWeeklyGrowthPercent(params, options, function (err, response) {
      if (err || !response.results) throw swe.notFound('analytics');
      writeResponse(res, response, start);
    });
  }
};

exports.getMonthlyGrowthPercent = {
  'spec': {
    "description" : "Get monthly growth percent of meetup groups as a time series.",
    "path" : "/analytics/monthlygrowth",
    "notes" : "Returns a set of data points containing the month of the year, the meetup group name, and membership count.",
    "summary" : "Get the time series that models the growth percent of meetup groups month over month.",
    "method": "GET",
    "params" :  [
      param.query("startDate", "A date to retrieve results from. Results will be returned for the entire month that the start date occurs within.", "string", true, true),
      param.query("endDate", "A date to retrieve results until. Results will be returned for the entire month that the start date occurs within.", "string", true, true),
      param.query("city", "The city name where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
      param.query("country", "The country code where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
      param.query("topics", "A list of topics that a meetup group must have to be returned in the result set. Multiple topic names should be delimited by a comma.", "string", true, true),
      param.query("groups", "A list of names to match on meetup groups, only groups with the name that are specified in the list are returned. Multiple topic names should be delimited by a comma. Leave blank to ignore this field.", "string", false, false)
    ],
    "responseClass" : "List[Analytics]",
    "errorResponses" : [],
    "nickname" : "getMonthlyGrowthPercent"
  },
  'action': function (req, res) {
    var options = {
      neo4j: parseBool(req, 'neo4j')
    };

    var startDate = parseUrl(req, 'startDate');
    var endDate = parseUrl(req, 'endDate');
    var location = parseUrl(req, 'city');
    var country = parseUrl(req, 'country');
    var topics = _.invoke(parseUrl(req, 'topics').split(','), 'trim');
    var groups = parseUrl(req, 'groups') ? _.invoke(parseUrl(req, 'groups').split(','), 'trim') : [];
    
    var dateFrom = new Date(startDate);
    var dateTo = new Date(endDate);

    var params = {
      startDate: dateFrom,
      endDate: dateTo,
      city: location,
      country: country,
      topics: topics,
      groups: groups
    };

    var start = new Date();
    Analytics.getMonthlyGrowthPercent(params, options, function (err, response) {
      if (err || !response.results) throw swe.notFound('analytics');
      writeResponse(res, response, start);
    });
  }
};