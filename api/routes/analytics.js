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
    var topics = _.invoke(parseUrl(req, 'topics').toLowerCase().split(','), 'trim');
    var groups = parseUrl(req, 'groups') ? _.invoke(parseUrl(req, 'groups').toLowerCase().split(','), 'trim') : [];
    
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

exports.getMonthlyGrowthPercentByTag = {
  'spec': {
    "description" : "Get monthly growth percent of meetup group tags as a time series.",
    "path" : "/analytics/monthlygrowthbytag",
    "notes" : "Returns a set of data points containing the month of the year, the meetup group tag name, and membership count.",
    "summary" : "Get the time series that models the growth percent of meetup group tags month over month.",
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
    "nickname" : "getMonthlyGrowthPercentByTag"
  },
  'action': function (req, res) {
    var options = {
      neo4j: parseBool(req, 'neo4j')
    };

    var startDate = parseUrl(req, 'startDate');
    var endDate = parseUrl(req, 'endDate');
    var location = parseUrl(req, 'city');
    var country = parseUrl(req, 'country');
    var topics = _.invoke(parseUrl(req, 'topics').toLowerCase().split(','), 'trim');
    var groups = parseUrl(req, 'groups') ? _.invoke(parseUrl(req, 'groups').toLowerCase().split(','), 'trim') : [];
    
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
    Analytics.getMonthlyGrowthPercentByTag(params, options, function (err, response) {
      if (err || !response.results) throw swe.notFound('analytics');
      writeResponse(res, response, start);
    });
  }
};

exports.getGroupCountByTag = {
  'spec': {
    "description" : "Get a count of groups by tag.",
    "path" : "/analytics/groupsbytag",
    "notes" : "Returns a list of tags and the number of groups per tag.",
    "summary" : "Gets list of tags and the number of groups per tag.",
    "method": "GET",
    "params" :  [param.query("tags", "A list of tags that a meetup group must have to be returned in the result set. Multiple tag names should be delimited by a comma.", "string", true, true),
    param.query("city", "The city name where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
      param.query("country", "The country code where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true)],
    "responseClass" : "List[Tag]",
    "errorResponses" : [],
    "nickname" : "getGroupCountByTag"
  },
  'action': function (req, res) {
    var options = {
      neo4j: parseBool(req, 'neo4j')
    };

    var tags = _.invoke(parseUrl(req, 'tags').toLowerCase().split(','), 'trim');
    var location = parseUrl(req, 'city');
    var country = parseUrl(req, 'country');

    var params = {
      tags: tags,
      city: location,
      country: country
    };

    var start = new Date();
    Analytics.getGroupCountByTag(params, options, function (err, response) {
      if (err || !response.results) throw swe.notFound('city');
      writeResponse(res, response, start);
    });
  }
};


exports.getCities = {
  'spec': {
    "description" : "Get a list of cities that meetup groups reside in.",
    "path" : "/analytics/cities",
    "notes" : "Returns a distinct list of cities for typeahead.",
    "summary" : "Gets a distinct list of cities that a meetup group resides in.",
    "method": "GET",
    "params" :  [],
    "responseClass" : "List[City]",
    "errorResponses" : [],
    "nickname" : "getCities"
  },
  'action': function (req, res) {
    var options = {
      neo4j: parseBool(req, 'neo4j')
    };

    var start = new Date();
    Analytics.getCities({}, options, function (err, response) {
      if (err || !response.results) throw swe.notFound('city');
      writeResponse(res, response, start);
    });
  }
};

exports.getCountries = {
  'spec': {
    "description" : "Get a list of countries that meetup groups reside in.",
    "path" : "/analytics/countries",
    "notes" : "Returns a distinct list of countries for typeahead.",
    "summary" : "Gets a distinct list of countries that a meetup group resides in.",
    "method": "GET",
    "params" :  [],
    "responseClass" : "List[City]",
    "errorResponses" : [],
    "nickname" : "getCountries"
  },
  'action': function (req, res) {
    var options = {
      neo4j: parseBool(req, 'neo4j')
    };

    var start = new Date();
    Analytics.getCountries({}, options, function (err, response) {
      if (err || !response.results) throw swe.notFound('city');
      writeResponse(res, response, start);
    });
  }
};