/**
 *  Meetup group statistics importer
 *  these are mostly written in a functional style
 */


var _ = require('underscore');
var uuid = require('hat'); // generates uuids
var Cypher = require('../neo4j/cypher');
var async = require('async');
var randomName = require('random-name');

/**
 *  Result Functions
 *  to be combined with queries using _.partial()
 */

var _resultsImportGroupStats = function (results, callback) {
  var stats = _.map(results, function (result) {
    var thisStats = {};
    thisStats.name = result.name;
    return thisStats;
  });

  callback(null, stats);
};


/**
 *  Query Functions
 *  to be combined with result functions using _.partial()
 */

var _importGroupStats = function (params, options, callback) {

  var query = [
      'MERGE (group:Group { name: { csvLine }.group_name })',
      'ON CREATE SET group.created = toInt({ csvLine }.group_creation_date)',
      'ON CREATE SET group.year = toInt({ csvLine }.group_creation_date_year)',
      'ON CREATE SET group.month = toInt({ csvLine }.group_creation_date_month)',
      'ON CREATE SET group.day = toInt({ csvLine }.group_creation_date_day)',
      'SET group.lat = { csvLine }.lat',
      'SET group.lon = { csvLine }.lon',
      'MERGE (location:Location { city: { csvLine }.group_location, country: { csvLine }.group_country })',
      'ON CREATE SET location.countryname = { csvLine }.group_country_name',
      'ON CREATE SET location.lat = { csvLine }.group_location_lat',
      'ON CREATE SET location.lon = { csvLine }.group_location_lon',
      'ON CREATE SET location.state = { csvLine }.group_state',
      'FOREACH (name in { csvLine }.group_tag |',
      '      MERGE (tag:Tag { tag: name[0] })',
      '      SET tag.name = name[1])',
      'MERGE (stats:Stats { group_name: { csvLine }.group_name, month: toInt({ csvLine }.last_month), day: toInt({ csvLine }.last_day), year: toInt({ csvLine }.last_year) })',
      'ON CREATE SET stats.count = toInt({ csvLine }.group_stats)',
      'MERGE (day:Day { month: toInt({ csvLine }.month), day: toInt({ csvLine }.day), year: toInt({ csvLine }.year) })',
      'ON CREATE SET day.timestamp = toInt({ csvLine }.day_timestamp)',
      'ON CREATE SET day.dayofweek = toInt({ csvLine }.day_of_week)',
      'MERGE (week:Week { year: toInt({ csvLine }.year), week: toInt({ csvLine }.week_of_year) })',
      'MERGE (week)-[:HAS_DAY]->(day)',
      'MERGE (month:Month { year: toInt({ csvLine }.year), month: toInt({ csvLine }.month) })',
      'MERGE (month)-[:HAS_DAY]->(day)',
      'MERGE (year:Year { year: toInt({ csvLine }.year) })',
      'MERGE (year)-[:HAS_MONTH]->(month)',
      'MERGE (lastDay:Day { month: toInt({ csvLine }.last_month), day: toInt({ csvLine }.last_day), year: toInt({ csvLine }.last_year) })',
      'MERGE (group)-[:LOCATED_IN]->(location)',
      'MERGE (group)-[:HAS_MEMBERS]->(stats)',
      'MERGE (stats)-[:ON_DAY]->(day)',
      'MERGE (lastDay)-[:NEXT]->(day)',
      'RETURN group.name as name'].join('\n');

  callback(null, query, params);
};

/**
 *  Result Function Wrappers
 *  a wrapper function that combines both the result functions with query functions
 */

var importGroupStats = Cypher(_importGroupStats, _resultsImportGroupStats);

// export exposed functions
module.exports = {
  importGroupStats: importGroupStats
};