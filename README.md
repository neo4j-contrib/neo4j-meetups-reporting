# Graph Database Analytics Platform

A graph-based analytics platform to track and measure meetup.com groups growth over time. Identify community trends and see what people are meeting about in the world. 

The main components of the archiecture include:

* Front-end dashboard in Node.js and bootstrap
* REST API via Neo4j Swagger in Node.js
* Data import services in .NET C# to replicate group statistics from Meetup.com API via Windows Azure service bus and scheduler
* Neo4j 2.1.0-M01 data store, found in the neo4j directory of this project, should be used for testing without import services running

### Prerequisites

* An instance of Neo4j(>=2.0) running locally. ([http://www.neo4j.org/download](http://www.neo4j.org/download_thanks?edition=community&release=2.1.0-M01))
* Microsoft Visual Studio (>=2012) for data import services. (Not required for running dashboard)
* A Meetup.com API Key for data import services ([http://api.meetup.com/])

### Usage

* Unzip the store files located in /neo4j/datastore.zip into the data directory of your choice and start a Neo4j 2.1.0-M01 that is configured to use the unzipped store files. (Copy data folder into Neo4j install directory by default)
* Start the Neo4j server at `http://localhost:7474`
* Make sure `node.js` and `npm` installed on your machine
* `cd` to the `api` directory of this project and in the terminal or console and type `npm install` and enter, after `node_modules` are installed type, `node app` and enter. The REST API will be started at `http://localhost:3000/`.
* `cd` to the `dashboard` directory of this project and in the terminal or console and type `npm install` and enter, after `node_modules` are installed, type `node app` and enter. The front-end analytics dashboard will be started at `http://localhost:5000/`.

### Specifications

You can find the project specifications as a ([GraphGist project](http://gist.neo4j.org])) app at ([Neo4j for Graph Analytics Part I: Meetup Analytics](http://gist.neo4j.org/?e2e0e4469917729765fe)]

![Neo4j for Graph Analytics Part I: Meetup Analytics](https://raw.githubusercontent.com/kbastani/meetup-analytics/master/specifications/Images/meetup-analytics-graph-gist.png)
