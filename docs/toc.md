# Table of contents

## API

This section covers the graph-based analytics REST API. This REST API communicates with the Neo4j graph database and exposes the database layer to the front-end web dashboard.

### Getting started

### Usage

### Setup

### Components

#### Neo4j Swagger

#### Project Files

This section covers an explanation of project files in the analytics REST API node.js project.

##### app.js

The `app.js` file is the entry-point to starting the REST API.

##### Models

The `models` folder contains files that manage Cypher queries that map to REST API endpoints. This is the data access layer that builds Cypher queries and executes transactions on a Neo4j database instance.

###### analytics.js

The `models/analytics.js` file contains a set of functions that are meant to manage parameterized transactions to the Neo4j graph database.

The Neo4j Cypher queries mapped to the `models/analytics.js` file are defined below.

* [getWeeklyGrowthPercent](#-getweeklygrowthpercent)
* [getMonthlyGrowthPercent](#-getmonthlygrowthpercent)
* [getMonthlyGrowthPercentByTag](#-getmonthlygrowthpercentbytag)
* [getMonthlyGrowthPercentByLocation](#-getmonthlygrowthpercentbytag)
* [getCities](#-getcities)
* [getCountries](#-getcountries)
* [getGroupCountByTag](#-getgroupcountbytag)

####### getWeeklyGrowthPercent

Get weekly growth percent of meetup groups as a time series.

######## Cypher query

```cypher
MATCH (d:Day)<-[:HAS_DAY]-(month:Month)
WHERE d.timestamp > { startDate } AND d.timestamp < { endDate }
WITH DISTINCT month
MATCH (month:Month)-[:HAS_DAY]->(day:Day { dayofweek: 1 })
MATCH (tag:Tag), (location:Location{ country: { country } })
WHERE tag.tag in { topics }
WITH tag, location, day
MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day
MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)
WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as week, group.name as group, stats.count as members, day
ORDER BY day.timestamp
RETURN week, group, members
```

######## Parameters

* `startDate`
... A date to retrieve results from. Results will be returned for the entire week that the start date occurs within.
* `endDate`
... A date to retrieve results until. Results will be returned for the entire week that the start date occurs within.
* `city`
... The city name where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.
* `country`
... The country code where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.
* `topics`
... A list of topics that a meetup group must have to be returned in the result set. Multiple topic names should be delimited by a comma.
* `groups`
... A list of names to match on meetup groups, only groups with the name that are specified in the list are returned. Multiple topic names should be delimited by a comma. Leave blank to ignore this field.

####### getMonthlyGrowthPercent

Get monthly growth percent of meetup groups as a time series.

######## Cypher query

```cypher
MATCH (d:Day)<-[:HAS_DAY]-(month:Month)
WHERE d.timestamp > { startDate } AND d.timestamp < { endDate }
WITH DISTINCT month
MATCH (month:Month)-[:HAS_DAY]->(day:Day { day: 1 })
MATCH (tag:Tag), (location:Location{ city: { city }, country: { country } })
WHERE tag.tag in { topics }
WITH tag, location, day
MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day
MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)
WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as month, group.name as group, stats.count as members, day
ORDER BY day.timestamp
RETURN month, group, members
```

######## Parameters

* `startDate`
* `endDate`
* `city`
* `country`
* `topics`
* `groups`

####### getMonthlyGrowthPercentByTag

Get monthly growth percent of meetup group tags as a time series.

######## Cypher query

```cypher
MATCH (d:Day)<-[:HAS_DAY]-(month:Month)
WHERE d.timestamp > { startDate } AND d.timestamp < { endDate }
WITH DISTINCT month
MATCH (month:Month)-[:HAS_DAY]->(day:Day { day: 1 })
MATCH (tag:Tag), (location:Location{ city: { city }, country: { country } })
WHERE tag.tag in { topics }
WITH tag, location, day
MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day, tag
MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)
WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as month, tag.tag as tag, sum(stats.count) as members, day
ORDER BY day.timestamp
RETURN month, tag, members
```

######## Parameters

* `startDate`
* `endDate`
* `city`
* `country`
* `topics`
* `groups`

####### getMonthlyGrowthPercentByLocation

Get monthly growth percent of meetup group locations and tags as a time series.

######## Cypher query

```cypher
MATCH (d:Day)<-[:HAS_DAY]-(month:Month)
WHERE d.timestamp > { startDate } AND d.timestamp < { endDate }
WITH DISTINCT month
MATCH (month:Month)-[:HAS_DAY]->(day:Day { day: 1 })
MATCH (tag:Tag), (location:Location{ city: { city }, country: { country } })
WHERE tag.tag in { topics }
WITH tag, location, day
MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location) WITH DISTINCT group, day, tag, location
MATCH (group)-[:HAS_MEMBERS]->(stats:Stats)-[:ON_DAY]->(day)
WITH DISTINCT (day.month + "/" + day.day + "/" + day.year) as month, location.city as city, tag.tag as tag, sum(stats.count) as members, day
ORDER BY day.timestamp
RETURN month, tag, members, city
```

######## Parameters

* `startDate`
* `endDate`
* `city`
* `country`
* `topics`
* `groups`

####### getCities

Get a list of cities that meetup groups reside in.

######## Cypher query

```cypher
MATCH (location:Location)
RETURN DISTINCT location.city as city
```

######## Parameters

This query does not use parameters.

####### getCountries

Get a list of countries that meetup groups reside in.

######## Cypher query

```cypher
MATCH (location:Location)
RETURN DISTINCT location.country as country
```

######## Parameters

This query does not use parameters.

####### getGroupCountByTag

Get a count of groups by tag.

######## Cypher query

```cypher
MATCH (tag:Tag), (location:Location{ city: { city }, country: { country } })
WHERE tag.tag in { tags }
MATCH (tag)<-[:HAS_TAG]-(group:Group)-[:LOCATED_IN]->(location)
RETURN tag.tag as tag, count(group) as count
```

######## Parameters

* `topics`

##### Views

##### Routes

The `routes` directory contains files that define a group of endpoints with a set of specifications for each REST API call. These specifications contain a `description`, `path`, `notes`, `summary`, `method`, `params`, `responseClass`, `errorResponses`, and `nickname`.

###### analytics.js

The `analytics.js` file contains a set of REST API call definitions and specifications that query the Neo4j graph database with a set of parameters that return analytical results as JSON objects. See below for a set of available endpoints and notes about their return results.

![Neo4j Swagger REST API calls for Analytics routes.](https://raw.githubusercontent.com/kbastani/meetup-analytics/master/docs/images/Swagger_UI_analytics.png "Analytics REST API calls.")

The analytics API calls are as follows:

* [/analytics/weeklygrowth](#-analyticsweeklygrowth)
* [/analytics/monthlygrowth](#-analyticsmonthlygrowth)
* [/analytics/monthlygrowthbytag](#-analyticsmonthlygrowthbytag)
* [/analytics/monthlygrowthbylocation](#-analyticsmonthlygrowthbylocation)
* [/analytics/groupsbytag](#-analyticsgroupsbytag)
* [/analytics/cities](#-analyticscities)
* [/analytics/countries](#-analyticscountries)

####### /analytics/weeklygrowth

Returns a set of data points containing the week of the year, the meetup group name, and membership count.

```javascript
'spec': {
    "description": "Get weekly growth percent of meetup groups as a time series.",
    "path": "/analytics/weeklygrowth",
    "notes": "Returns a set of data points containing the week of the year, the meetup group name, and membership count.",
    "summary": "Get the time series that models the growth percent of meetup groups week over week.",
    "method": "GET",
    "params": [
        param.query("startDate", "A date to retrieve results from. Results will be returned for the entire week that the start date occurs within.", "string", true, true),
        param.query("endDate", "A date to retrieve results until. Results will be returned for the entire week that the start date occurs within.", "string", true, true),
        param.query("city", "The city name where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
        param.query("topics", "A list of topics that a meetup group must have to be returned in the result set. Multiple topic names should be delimited by a comma.", "string", true, true),
        param.query("groups", "A list of names to match on meetup groups, only groups with the name that are specified in the list are returned. Multiple topic names should be delimited by a comma. Leave blank to ignore this field.", "string", false, false)
    ],
    "responseClass": "List[Analytics]",
    "errorResponses": [],
    "nickname": "getWeeklyGrowthPercent"
}
```

####### /analytics/monthlygrowth

Returns a set of data points containing the month of the year, the meetup group name, and membership count.

```javascript
'spec': {
    "description": "Get monthly growth percent of meetup groups as a time series.",
    "path": "/analytics/monthlygrowth",
    "notes": "Returns a set of data points containing the month of the year, the meetup group name, and membership count.",
    "summary": "Get the time series that models the growth percent of meetup groups month over month.",
    "method": "GET",
    "params": [
        param.query("startDate", "A date to retrieve results from. Results will be returned for the entire month that the start date occurs within.", "string", true, true),
        param.query("endDate", "A date to retrieve results until. Results will be returned for the entire month that the start date occurs within.", "string", true, true),
        param.query("city", "The city name where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
        param.query("country", "The country code where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
        param.query("topics", "A list of topics that a meetup group must have to be returned in the result set. Multiple topic names should be delimited by a comma.", "string", true, true),
        param.query("groups", "A list of names to match on meetup groups, only groups with the name that are specified in the list are returned. Multiple topic names should be delimited by a comma. Leave blank to ignore this field.", "string", false, false)
    ],
    "responseClass": "List[Analytics]",
    "errorResponses": [],
    "nickname": "getMonthlyGrowthPercent"
}
```

####### /analytics/monthlygrowthbytag

Returns a set of data points containing the month of the year, the meetup group tag name, and membership count.

```javascript
'spec': {
    "description": "Get monthly growth percent of meetup group locations and tags as a time series.",
    "path": "/analytics/monthlygrowthbylocation",
    "notes": "Returns a set of data points containing the month of the year, the meetup group tag name, the city, and membership count.",
    "summary": "Get the time series that models the growth percent of meetup group tags month over month, by city.",
    "method": "GET",
    "params": [
        param.query("startDate", "A date to retrieve results from. Results will be returned for the entire month that the start date occurs within.", "string", true, true),
        param.query("endDate", "A date to retrieve results until. Results will be returned for the entire month that the start date occurs within.", "string", true, true),
        param.query("city", "The city name where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
        param.query("country", "The country code where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
        param.query("topics", "A list of topics that a meetup group must have to be returned in the result set. Multiple topic names should be delimited by a comma.", "string", true, true),
        param.query("groups", "A list of names to match on meetup groups, only groups with the name that are specified in the list are returned. Multiple topic names should be delimited by a comma. Leave blank to ignore this field.", "string", false, false)
    ],
    "responseClass": "List[Analytics]",
    "errorResponses": [],
    "nickname": "getMonthlyGrowthPercentByTag"
}
```

####### /analytics/monthlygrowthbylocation

Returns a set of data points containing the month of the year, the meetup group tag name, the city, and membership count.

```javascript
'spec': {
    "description": "Get monthly growth percent of meetup group locations and tags as a time series.",
    "path": "/analytics/monthlygrowthbylocation",
    "notes": "Returns a set of data points containing the month of the year, the meetup group tag name, the city, and membership count.",
    "summary": "Get the time series that models the growth percent of meetup group tags month over month, by city.",
    "method": "GET",
    "params": [
        param.query("startDate", "A date to retrieve results from. Results will be returned for the entire month that the start date occurs within.", "string", true, true),
        param.query("endDate", "A date to retrieve results until. Results will be returned for the entire month that the start date occurs within.", "string", true, true),
        param.query("city", "The city name where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
        param.query("country", "The country code where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
        param.query("topics", "A list of topics that a meetup group must have to be returned in the result set. Multiple topic names should be delimited by a comma.", "string", true, true),
        param.query("groups", "A list of names to match on meetup groups, only groups with the name that are specified in the list are returned. Multiple topic names should be delimited by a comma. Leave blank to ignore this field.", "string", false, false)
    ],
    "responseClass": "List[Analytics]",
    "errorResponses": [],
    "nickname": "getMonthlyGrowthPercentByTag"
}
```

####### /analytics/groupsbytag

Returns a list of tags and the number of groups per tag.

```javascript
'spec': {
	"description": "Get a count of groups by tag.",
    "path": "/analytics/groupsbytag",
    "notes": "Returns a list of tags and the number of groups per tag.",
    "summary": "Gets list of tags and the number of groups per tag.",
    "method": "GET",
    "params": [
        param.query("tags", "A list of tags that a meetup group must have to be returned in the result set. Multiple tag names should be delimited by a comma.", "string", true, true),
        param.query("city", "The city name where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true),
        param.query("country", "The country code where a meetup group resides. This field is case sensitive. Leave blank to query on world-wide meetup groups.", "string", false, true)
    ],
    "responseClass": "List[Tag]",
    "errorResponses": [],
    "nickname": "getGroupCountByTag"
}
```

####### /analytics/cities

Returns a distinct list of cities for typeahead.

```javascript
'spec': {
    "description": "Get a list of cities that meetup groups reside in.",
    "path": "/analytics/cities",
    "notes": "Returns a distinct list of cities for typeahead.",
    "summary": "Gets a distinct list of cities that a meetup group resides in.",
    "method": "GET",
    "params": [],
    "responseClass": "List[City]",
    "errorResponses": [],
    "nickname": "getCities"
}
```

####### /analytics/countries

Returns a distinct list of countries for typeahead.

```javascript
'spec': {
    "description": "Get a list of cities that meetup groups reside in.",
    "path": "/analytics/cities",
    "notes": "Returns a distinct list of cities for typeahead.",
    "summary": "Gets a distinct list of cities that a meetup group resides in.",
    "method": "GET",
    "params": [],
    "responseClass": "List[City]",
    "errorResponses": [],
    "nickname": "getCities"
}
```

## Dashboard

This section covers the graph-based analytics web dashboard.

### Getting started

## Neo4j

This section covers storage options and deployment with Neo4j.

### Getting started

## Scheduler

This section covers scheduled data import services from a desired external REST API.

### Getting started

