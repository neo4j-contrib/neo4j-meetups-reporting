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

####### getWeeklyGrowthPercent

Get weekly growth percent of meetup groups as a time series.

####### getMonthlyGrowthPercent

Get monthly growth percent of meetup groups as a time series.

####### getMonthlyGrowthPercentByTag

Get monthly growth percent of meetup group tags as a time series.

####### getMonthlyGrowthPercentByLocation

Get monthly growth percent of meetup group locations and tags as a time series.

####### getCities

Get a list of cities that meetup groups reside in.

####### getCountries

Get a list of countries that meetup groups reside in.

####### getGroupCountByTag

Get a count of groups by tag.

##### Views

##### Routes

###### Analytics.js

##### Endpoints

## Dashboard

This section covers the graph-based analytics web dashboard.

### Getting started

## Neo4j

This section covers storage options and deployment with Neo4j.

### Getting started

## Scheduler

This section covers scheduled data import services from a desired external REST API.

### Getting started

