# Graph-based Analytics Platform

A graph-based analytics platform to track and measure meetup.com groups growth over time. Identify community trends and see what people are meeting about in the world. 

![Meetup Analytics Dashboard](https://raw.githubusercontent.com/kbastani/meetup-analytics/master/specifications/Images/Meetup%20Analytics%20Dashboard%20-%20Screen.png)

## Architecutre

The main components of the archiecture include:

* Front-end dashboard in Node.js and bootstrap
* REST API via Neo4j Swagger in Node.js
* Data import services in .NET C# to replicate group statistics from Meetup.com API via Windows Azure service bus and scheduler
* Neo4j 2.1.0-M01 data store, found in the neo4j directory of this project, should be used for testing without import services running

### Prerequisites

* An instance of Neo4j (`>=2.0`) running locally - [http://www.neo4j.org/download](http://www.neo4j.org/download_thanks?edition=community&release=2.1.0-M01)
* Microsoft Visual Studio (`>=2012`) for running data import services. (Not required for testing the dashboard)
* A Meetup.com API Key for running data import services - [http://www.meetup.com/meetup_api/](http://www.meetup.com/meetup_api/)
* Installed `node.js` and `npm` on your machine

### Usage

* Unzip the Neo4j store files located in `/neo4j/datastore.zip` to your Neo4j directory `neo4j/data`
* Start the Neo4j server at `http://localhost:7474`
* From the terminal, go to the `api` directory of this project and type `npm install`, after `node_modules` are installed, type `node app`. The analytics REST API will be started at `http://localhost:3000`
* From the terminal, go to the `dashboard` directory of this project and type `npm install`, after `node_modules` are installed, type `node app`. The analytics REST API will be started at `http://localhost:5000`

### Specifications

You can find the project specifications as a [GraphGist project](http://gist.neo4j.org]) app at [Neo4j for Graph Analytics Part I: Meetup Analytics](http://gist.neo4j.org/?e2e0e4469917729765fe)


