/**
 *  neo4j movie functions
 *  these are mostly written in a functional style
 */


var _ = require('underscore');
var uuid = require('hat'); // generates uuids
var Cypher = require('../neo4j/cypher');
var Analytics = require('../models/neo4j/analytics');
var async = require('async');
var randomName = require('random-name');

/**
 *  Result Functions
 *  to be combined with queries using _.partial()
 */

 var _cities = function(results, callback) {
  var citiesArray = [];

  var cities = _.map(results, function (result) {
  var thisCities = {};
    thisCities.city = result.city;
    citiesArray.push(result.city);
    return thisCities;
  });

  callback(null, citiesArray);
};

 var _countries = function(results, callback) {
  var countriesArray = [];

  var countries = _.map(results, function (result) {
  var thisCountries = {};
    thisCountries.country = result.country;
    countriesArray.push(result.country);
    return thisCountries;
  });

  callback(null, countriesArray);
};

var _dailyGrowthStatistics = function (results, callback) {
  var analytics = _.map(results, function (result) {
    var thisAnalytics = {};
    thisAnalytics.day = result.day;
    thisAnalytics.group = result.group;
    thisAnalytics.members = result.members;
    return thisAnalytics;
  });

  callback(null, analytics);
};

var _weeklyGrowthStatistics = function (results, callback) {
  var analytics = _.map(results, function (result) {
    var thisAnalytics = {};
    thisAnalytics.week = result.week;
    thisAnalytics.group = result.group;
    thisAnalytics.members = result.members;
    return thisAnalytics;
  });

  callback(null, analytics);
};

var _monthlyGrowthStatistics = function (results, callback) {
  var analytics = _.map(results, function (result) {
    var thisAnalytics = {};
    thisAnalytics.month = result.month;
    thisAnalytics.group = result.group;
    thisAnalytics.members = result.members;
    return thisAnalytics;
  });

  callback(null, analytics);
};

var _dailyGrowthStatisticsByTag = function (results, callback) {
  var analytics = _.map(results, function (result) {
    var thisAnalytics = {};
    thisAnalytics.date = result.day;
    thisAnalytics.tag = result.tag;
    thisAnalytics.members = result.members;
    return thisAnalytics;
  });

  callback(null, analytics);
};

var _weeklyGrowthStatisticsByTag = function (results, callback) {
  var analytics = _.map(results, function (result) {
    var thisAnalytics = {};
    thisAnalytics.date = result.week;
    thisAnalytics.tag = result.tag;
    thisAnalytics.members = result.members;
    return thisAnalytics;
  });

  callback(null, analytics);
};

var _monthlyGrowthStatisticsByTag = function (results, callback) {
  var analytics = _.map(results, function (result) {
    var thisAnalytics = {};
    thisAnalytics.date = result.month;
    thisAnalytics.tag = result.tag;
    thisAnalytics.members = result.members;
    return thisAnalytics;
  });

  callback(null, analytics);
};

var _dailyGrowthStatisticsByLocation = function (results, callback) {
  var analytics = _.map(results, function (result) {
    var thisAnalytics = {};
    thisAnalytics.day = result.day;
    thisAnalytics.tag = result.tag;
    thisAnalytics.city = result.city;
    thisAnalytics.lat = result.lat;
    thisAnalytics.lon = result.lon;
    thisAnalytics.members = result.members;
    return thisAnalytics;
  });

  callback(null, analytics);
};

var _weeklyGrowthStatisticsByLocation = function (results, callback) {
  var analytics = _.map(results, function (result) {
    var thisAnalytics = {};
    thisAnalytics.week = result.week;
    thisAnalytics.tag = result.tag;
    thisAnalytics.city = result.city;
    thisAnalytics.lat = result.lat;
    thisAnalytics.lon = result.lon;
    thisAnalytics.members = result.members;
    return thisAnalytics;
  });

  callback(null, analytics);
};

var _monthlyGrowthStatisticsByLocation = function (results, callback) {
  var analytics = _.map(results, function (result) {
    var thisAnalytics = {};
    thisAnalytics.month = result.month;
    thisAnalytics.tag = result.tag;
    thisAnalytics.city = result.city;
    thisAnalytics.lat = result.lat;
    thisAnalytics.lon = result.lon;
    thisAnalytics.members = result.members;
    return thisAnalytics;
  });

  callback(null, analytics);
};

var _groupCountByTag = function(results, callback) {
  var tagCountArray = [];

  var tags = _.map(results, function (result) {
  var thisTags = {};
    thisTags.tag = result.tag;
    thisTags.count = result.count;
    tagCountArray.push(thisTags);
    return thisTags;
  });

  callback(null, tagCountArray);
};

/**
 *  Query Functions
 *  to be combined with result functions using _.partial()
 */

var _getDailyGrowth = function (params, options, callback) {
  var cypher_params = {
    startDate: getTicks(params.startDate),
    endDate: getTicks(params.endDate),
    city: params.city,
    country: params.country,
    topics: params.topics,
    groups: params.groups
  };
  var query = [
    'MATCH (day:Day)',
    'WHERE day.timestamp >= { startDate } AND day.timestamp <= { endDate }',
    'MATCH (tag:Tag), (location:Location' + ((params.country || params.city) ? '{ ' : '') + (params.city ? 'city: { city }' : '') + ((params.country && params.city) ? ', ' : '') + (params.country ? 'country: { country } }' : (params.city ? ' }' : '')) + ')',
    'WHERE tag.tag in { topics }',
    'WITH tag, location, day',
    'MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day',
    'MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)' + (params.groups.length > 0 ? ' WHERE group.name in { groups }' : ''),
    'WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as day, group.name as group, stats.count as members, day as time',
    'ORDER BY time.timestamp',
    'RETURN day, group, members'
  ].join('\n');

  callback(null, query, cypher_params);
};

var _getWeeklyGrowth = function (params, options, callback) {
  var cypher_params = {
    startDate: getTicks(params.startDate),
    endDate: getTicks(params.endDate),
    city: params.city,
    country: params.country,
    topics: params.topics,
    groups: params.groups
  };
  var query = [
    'MATCH (day:Day { dayofweek: 1 })',
    'WHERE day.timestamp >= { startDate } AND day.timestamp <= { endDate }',
    'MATCH (tag:Tag), (location:Location' + ((params.country || params.city) ? '{ ' : '') + (params.city ? 'city: { city }' : '') + ((params.country && params.city) ? ', ' : '') + (params.country ? 'country: { country } }' : (params.city ? ' }' : '')) + ')',
    'WHERE tag.tag in { topics }',
    'WITH tag, location, day',
    'MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day',
    'MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)' + (params.groups.length > 0 ? ' WHERE group.name in { groups }' : ''),
    'WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as week, group.name as group, stats.count as members, day',
    'ORDER BY day.timestamp',
    'RETURN week, group, members'
  ].join('\n');

  callback(null, query, cypher_params);
};

var _getMonthlyGrowth = function (params, options, callback) {
  var cypher_params = {
    startDate: getTicks(params.startDate),
    endDate: getTicks(params.endDate),
    city: params.city,
    country: params.country,
    topics: params.topics,
    groups: params.groups
  };
  var query = [
    'MATCH (d:Day)<-[:HAS_DAY]-(month:Month)',
    'WHERE d.timestamp >= { startDate } AND d.timestamp <= { endDate }',
    'WITH DISTINCT month',
    'MATCH (month:Month)-[:HAS_DAY]->(day:Day { day: 1 })',
    'MATCH (tag:Tag), (location:Location' + ((params.country || params.city) ? '{ ' : '') + (params.city ? 'city: { city }' : '') + ((params.country && params.city) ? ', ' : '') + (params.country ? 'country: { country } }' : (params.city ? ' }' : '')) + ')',
    'WHERE tag.tag in { topics }',
    'WITH tag, location, day',
    'MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day',
    'MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)' + (params.groups.length > 0 ? ' WHERE group.name in { groups }' : ''),
    'WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as month, group.name as group, stats.count as members, day',
    'ORDER BY day.timestamp',
    'RETURN month, group, members'
  ].join('\n');

  callback(null, query, cypher_params);
};

var _getDailyGrowthByTag = function (params, options, callback) {
  var cypher_params = {
    startDate: getTicks(params.startDate),
    endDate: getTicks(params.endDate),
    city: params.city,
    country: params.country,
    topics: params.topics,
    groups: params.groups
  };
  var query = [
    'MATCH (day:Day)',
    'WHERE day.timestamp >= { startDate } AND day.timestamp <= { endDate }',
    'MATCH (tag:Tag), (location:Location' + ((params.country || params.city) ? '{ ' : '') + (params.city ? 'city: { city }' : '') + ((params.country && params.city) ? ', ' : '') + (params.country ? 'country: { country } }' : (params.city ? ' }' : '')) + ')',
    'WHERE tag.tag in { topics }',
    'WITH tag, location, day',
    'MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day, tag',
    'MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)' + (params.groups.length > 0 ? ' WHERE group.name in { groups }' : ''),
    'WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as day, tag.name as tag, sum(stats.count) as members, day as time',
    'ORDER BY time.timestamp',
    'RETURN day, tag, members'
  ].join('\n');

  callback(null, query, cypher_params);
};

var _getWeeklyGrowthByTag = function (params, options, callback) {
  var cypher_params = {
    startDate: getTicks(params.startDate),
    endDate: getTicks(params.endDate),
    city: params.city,
    country: params.country,
    topics: params.topics,
    groups: params.groups
  };
  var query = [
    'MATCH (day:Day { dayofweek: 1 })',
    'WHERE day.timestamp >= { startDate } AND day.timestamp <= { endDate }',
    'MATCH (tag:Tag), (location:Location' + ((params.country || params.city) ? '{ ' : '') + (params.city ? 'city: { city }' : '') + ((params.country && params.city) ? ', ' : '') + (params.country ? 'country: { country } }' : (params.city ? ' }' : '')) + ')',
    'WHERE tag.tag in { topics }',
    'WITH tag, location, day',
    'MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day, tag',
    'MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)' + (params.groups.length > 0 ? ' WHERE group.name in { groups }' : ''),
    'WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as week, tag.name as tag, sum(stats.count) as members, day',
    'ORDER BY day.timestamp',
    'RETURN week, tag, members'
  ].join('\n');

  callback(null, query, cypher_params);
};

var _getMonthlyGrowthByTag = function (params, options, callback) {
  var cypher_params = {
    startDate: getTicks(params.startDate),
    endDate: getTicks(params.endDate),
    city: params.city,
    country: params.country,
    topics: params.topics,
    groups: params.groups
  };
  var query = [
    'MATCH (day:Day { day: 1 })',
    'WHERE day.timestamp >= { startDate } AND day.timestamp <= { endDate }',
    'MATCH (tag:Tag), (location:Location' + ((params.country || params.city) ? '{ ' : '') + (params.city ? 'city: { city }' : '') + ((params.country && params.city) ? ', ' : '') + (params.country ? 'country: { country } }' : (params.city ? ' }' : '')) + ')',
    'WHERE tag.tag in { topics }',
    'WITH tag, location, day',
    'MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day, tag',
    'MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)' + (params.groups.length > 0 ? ' WHERE group.name in { groups }' : ''),
    'WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as month, tag.name as tag, sum(stats.count) as members, day',
    'ORDER BY day.timestamp',
    'RETURN month, tag, members'
  ].join('\n');

  callback(null, query, cypher_params);
};

var _getDailyGrowthByLocation = function (params, options, callback) {
  var cypher_params = {
    startDate: getTicks(params.startDate),
    endDate: getTicks(params.endDate),
    city: params.city,
    country: params.country,
    topics: params.topics,
    groups: params.groups
  };
  var query = [
    'MATCH (day:Day)',
    'WHERE day.timestamp >= { startDate } AND day.timestamp <= { endDate }',
    'MATCH (tag:Tag), (location:Location' + ((params.country || params.city) ? '{ ' : '') + (params.city ? 'city: { city }' : '') + ((params.country && params.city) ? ', ' : '') + (params.country ? 'country: { country } }' : (params.city ? ' }' : '')) + ')',
    'WHERE tag.tag in { topics }',
    'WITH tag, location, day',
    'MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day, tag, location',
    'MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)' + (params.groups.length > 0 ? ' WHERE group.name in { groups }' : ''),
    'WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as day, location.city as city, tag.name as tag, sum(stats.count) as members, day as time, location.lat as lat, location.lon as lon',
    'ORDER BY time.timestamp',
    'RETURN day, tag, members, city, coalesce(lat, "") as lat, coalesce(lon, "") as lon'
  ].join('\n');

  callback(null, query, cypher_params);
};

var _getWeeklyGrowthByLocation = function (params, options, callback) {
  var cypher_params = {
    startDate: getTicks(params.startDate),
    endDate: getTicks(params.endDate),
    city: params.city,
    country: params.country,
    topics: params.topics,
    groups: params.groups
  };
  var query = [
    'MATCH (day:Day { dayofweek: 1 })',
    'WHERE day.timestamp >= { startDate } AND day.timestamp <= { endDate }',
    'MATCH (tag:Tag), (location:Location' + ((params.country || params.city) ? '{ ' : '') + (params.city ? 'city: { city }' : '') + ((params.country && params.city) ? ', ' : '') + (params.country ? 'country: { country } }' : (params.city ? ' }' : '')) + ')',
    'WHERE tag.tag in { topics }',
    'WITH tag, location, day',
    'MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day, tag, location',
    'MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)' + (params.groups.length > 0 ? ' WHERE group.name in { groups }' : ''),
    'WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as week, location.city as city, tag.name as tag, sum(stats.count) as members, day, location.lat as lat, location.lon as lon',
    'ORDER BY day.timestamp',
    'RETURN week, tag, members, city, coalesce(lat, "") as lat, coalesce(lon, "") as lon'
  ].join('\n');

  callback(null, query, cypher_params);
};

var _getMonthlyGrowthByLocation = function (params, options, callback) {
  var cypher_params = {
    startDate: getTicks(params.startDate),
    endDate: getTicks(params.endDate),
    city: params.city,
    country: params.country,
    topics: params.topics,
    groups: params.groups
  };
  var query = [
    'MATCH (d:Day)<-[:HAS_DAY]-(month:Month)',
    'WHERE d.timestamp >= { startDate } AND d.timestamp <= { endDate }',
    'WITH DISTINCT month',
    'MATCH (month:Month)-[:HAS_DAY]->(day:Day { day: 1 })',
    'MATCH (tag:Tag), (location:Location' + ((params.country || params.city) ? '{ ' : '') + (params.city ? 'city: { city }' : '') + ((params.country && params.city) ? ', ' : '') + (params.country ? 'country: { country } }' : (params.city ? ' }' : '')) + ')',
    'WHERE tag.tag in { topics }',
    'WITH tag, location, day',
    'MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day, tag, location',
    'MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)' + (params.groups.length > 0 ? ' WHERE group.name in { groups }' : ''),
    'WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as month, location.city as city, tag.name as tag, sum(stats.count) as members, day, location.lat as lat, location.lon as lon',
    'ORDER BY day.timestamp',
    'RETURN month, tag, members, city, coalesce(lat, "") as lat, coalesce(lon, "") as lon'
  ].join('\n');

  callback(null, query, cypher_params);
};

var _getGroupsByTag = function (params, options, callback) {

  var query = [
    'MATCH (tag:Tag), (location:Location' + ((params.country || params.city) ? '{ ' : '') + (params.city ? 'city: { city }' : '') + ((params.country && params.city) ? ', ' : '') + (params.country ? 'country: { country } }' : (params.city ? ' }' : '')) + ')',
    'WHERE tag.tag in { tags }',
    'MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location)',
    'RETURN tag.name as tag, count(group) as count'
  ].join('\n');

  callback(null, query, params);
};

var _getCities = function (params, options, callback) {

  var query = [
    'MATCH (location:Location)',
    'RETURN DISTINCT location.city as city'
  ].join('\n');

  callback(null, query, params);
};

var _getCountries = function (params, options, callback) {

  var query = [
    'MATCH (location:Location)',
    'RETURN DISTINCT location.country as country'
  ].join('\n');

  callback(null, query, params);
};


/**
 *  Result Function Wrappers
 *  a wrapper function that combines both the result functions with query functions
 */

var getDailyGrowth = Cypher(_getDailyGrowth, _dailyGrowthStatistics);
var getWeeklyGrowth = Cypher(_getWeeklyGrowth, _weeklyGrowthStatistics);
var getMonthlyGrowth = Cypher(_getMonthlyGrowth, _monthlyGrowthStatistics);
var getDailyGrowthByTag = Cypher(_getDailyGrowthByTag, _dailyGrowthStatisticsByTag);
var getWeeklyGrowthByTag = Cypher(_getWeeklyGrowthByTag, _weeklyGrowthStatisticsByTag);
var getMonthlyGrowthByTag = Cypher(_getMonthlyGrowthByTag, _monthlyGrowthStatisticsByTag);
var getDailyGrowthByLocation = Cypher(_getDailyGrowthByLocation, _dailyGrowthStatisticsByLocation);
var getWeeklyGrowthByLocation = Cypher(_getWeeklyGrowthByLocation, _weeklyGrowthStatisticsByLocation);
var getMonthlyGrowthByLocation = Cypher(_getMonthlyGrowthByLocation, _monthlyGrowthStatisticsByLocation);
var getCities = Cypher(_getCities, _cities);
var getCountries = Cypher(_getCountries, _countries);
var getGroupCountByTag = Cypher(_getGroupsByTag, _groupCountByTag);

// export exposed functions
module.exports = {
  getDailyGrowth: getDailyGrowth,
  getWeeklyGrowth: getWeeklyGrowth,
  getMonthlyGrowth: getMonthlyGrowth,
  getDailyGrowthByTag: getDailyGrowthByTag,
  getWeeklyGrowthByTag: getWeeklyGrowthByTag,
  getMonthlyGrowthByTag: getMonthlyGrowthByTag,
  getDailyGrowthByLocation: getDailyGrowthByLocation,
  getWeeklyGrowthByLocation: getWeeklyGrowthByLocation,
  getMonthlyGrowthByLocation: getMonthlyGrowthByLocation,
  getCities: getCities,
  getCountries: getCountries,
  getGroupCountByTag: getGroupCountByTag
};

function getTicks(dateTime)
{

  var ticks = ((dateTime.getTime() * 10000) + 621355968000000000);

  return ticks;
}