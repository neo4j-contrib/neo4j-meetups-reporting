using Neo4jClient;
using Neo4jClient.Cypher;
using Neo4jClient.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Meetup.Analytics.Services.Implementations
{
    public class Analytics
    {

        public static TimeSeries GetMeetupAnalytics(string tag, string city, string from, string to)
        {
            //var dateRange = GetDateRange();
            var dayFrom = DateTime.Parse(from);
            var dayTo = DateTime.Parse(to);

            DateRange dateRange = new DateRange() { firstDay = new SeriesDay() { day = dayFrom.Day, month = dayFrom.Month, year = dayFrom.Year }, lastDay = new SeriesDay() { day = dayTo.Day, month = dayTo.Month, year = dayTo.Year } };

            GraphClient graphClient = CloudGraph.GetNeo4jGraphClient();
            graphClient.Connect();

            var sb = new StringBuilder();
            sb.AppendLine("MATCH (dayStart:Day { day: {from}.day, month: {from}.month, year: {from}.year }), (dayEnd:Day { day: {to}.day, month: {to}.month, year: {to}.year })");
            sb.AppendLine("MATCH (dayStart)-[:NEXT*0..]->(day:Day)-[:NEXT*0..]->(dayEnd)");
            sb.AppendLine("MATCH (day)<-[:ON_DAY]-(members:Stats),");
            sb.AppendLine("(members)<-[:HAS_MEMBERS]-(group:Group),");
            sb.AppendLine("(group)-[:HAS_TAG]->(tag:Tag { tag: {tag} }),");
            sb.AppendLine("(group)-[:LOCATED_IN]->(city { city: {city} })");
            sb.AppendLine("WITH (day.month + \"/\" + day.day + \"/\" + day.year) AS date, group.name AS name, members.count AS membership_count");
            sb.AppendLine("WITH collect(\"'\" + date + \"'\") as categories, name, collect(membership_count) as series");
            sb.AppendLine("RETURN categories, collect({ name:name,data: series }) AS series");

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("from", new { day = dateRange.firstDay.day, month = dateRange.firstDay.month, year = dateRange.firstDay.year });
            parameters.Add("to", new { day = dateRange.lastDay.day, month = dateRange.lastDay.month, year = dateRange.lastDay.year });
            parameters.Add("tag", tag);
            parameters.Add("city", city);

            var cypher = new CypherFluentQueryCreator(graphClient, new CypherQueryCreator(sb.ToString()), new Uri(CloudGraph.GetDatabaseUri()));
            var series = cypher.ExecuteGetCypherResults<TimeSeries>(parameters).Result.FirstOrDefault();
            return series;
        }

        public static TimeSeries GetMeetupAnalytics(string tag, string city)
        {
            DateRange dateRange = GetDateRange();

            GraphClient graphClient = CloudGraph.GetNeo4jGraphClient();
            graphClient.Connect();

            var sb = new StringBuilder();
            sb.AppendLine("MATCH (dayStart:Day { day: {from}.day, month: {from}.month, year: {from}.year }), (dayEnd:Day { day: {to}.day, month: {to}.month, year: {to}.year })");
            sb.AppendLine("MATCH (dayStart)-[:NEXT*0..]->(day:Day)-[:NEXT*0..]->(dayEnd)");
            sb.AppendLine("MATCH (day)<-[:ON_DAY]-(members:Stats),");
                  sb.AppendLine("(members)<-[:HAS_MEMBERS]-(group:Group),");
                  sb.AppendLine("(group)-[:HAS_TAG]->(tag:Tag { tag: {tag} }),");
                  sb.AppendLine("(group)-[:LOCATED_IN]->(city { city: {city} })");
            sb.AppendLine("WITH (day.month + \"/\" + day.day + \"/\" + day.year) AS date, group.name AS name, members.count AS membership_count");
            sb.AppendLine("WITH collect(\"'\" + date + \"'\") as categories, name, collect(membership_count) as series");
            sb.AppendLine("RETURN categories, collect({ name:name,data: series }) AS series");

            Dictionary<string, object> parameters = new Dictionary<string,object>();
            parameters.Add("from", new { day = dateRange.firstDay.day, month = dateRange.firstDay.month, year = dateRange.firstDay.year });
            parameters.Add("to", new { day = dateRange.lastDay.day, month = dateRange.lastDay.month, year = dateRange.lastDay.year });
            parameters.Add("tag", tag);
            parameters.Add("city", city);

            var cypher = new CypherFluentQueryCreator(graphClient, new CypherQueryCreator(sb.ToString()), new Uri(CloudGraph.GetDatabaseUri()));
            var series = cypher.ExecuteGetCypherResults<TimeSeries>(parameters).Result.FirstOrDefault();
            return series;
        }

        public static TimeSeries GetMeetupAnalyticsByWeek(string tag, string city, string from, string to)
        {
            var dayFrom = DateTime.Parse(from);
            var dayTo = DateTime.Parse(to);

            DateRange dateRange = new DateRange() { firstDay = new SeriesDay() { day = dayFrom.Day, month = dayFrom.Month, year = dayFrom.Year }, lastDay = new SeriesDay() { day = dayTo.Day, month = dayTo.Month, year = dayTo.Year } };

            GraphClient graphClient = CloudGraph.GetNeo4jGraphClient();
            graphClient.Connect();

            var sb = new StringBuilder();
            sb.AppendLine("MATCH (dayStart:Day { day: {from}.day, month: {from}.month, year: {from}.year }), (dayEnd:Day { day: {to}.day, month: {to}.month, year: {to}.year })");
            sb.AppendLine("MATCH (dayStart)-[:NEXT*0..]->(day:Day)-[:NEXT*0..]->(dayEnd)");
            sb.AppendLine("MATCH (day)<-[:ON_DAY]-(members:Stats),");
            sb.AppendLine("(members)<-[:HAS_MEMBERS]-(group:Group),");
            sb.AppendLine("(group)-[:HAS_TAG]->(tag:Tag { tag: {tag} }),");
            sb.AppendLine("(group)-[:LOCATED_IN]->(city { city: {city} }),"); 
            sb.AppendLine("(week:Week)-[:HAS_DAY]->(day)"); 
            sb.AppendLine("WITH week.week + \"-\" + week.year as week, group.name AS name, sum(members.count) AS membership_count");
            sb.AppendLine("ORDER BY week");
            sb.AppendLine("WITH collect(week) as categories, name, collect(membership_count) as series");
            sb.AppendLine("RETURN categories, collect({ name:name,data: series }) AS series");

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("from", new { day = dateRange.firstDay.day, month = dateRange.firstDay.month, year = dateRange.firstDay.year });
            parameters.Add("to", new { day = dateRange.lastDay.day, month = dateRange.lastDay.month, year = dateRange.lastDay.year });
            parameters.Add("tag", tag);
            parameters.Add("city", city);

            var cypher = new CypherFluentQueryCreator(graphClient, new CypherQueryCreator(sb.ToString()), new Uri(CloudGraph.GetDatabaseUri()));
            var series = cypher.ExecuteGetCypherResults<TimeSeries>(parameters).Result.FirstOrDefault();
            return series;
        }

        public static GrowthTimeSeries GetMeetupAnalyticsByGroupAndGrowthPercent(string tag, string city, string from, string to)
        {
            var dayFrom = DateTime.Parse(from);
            var dayTo = DateTime.Parse(to);

            DateRange dateRange = new DateRange() { firstDay = new SeriesDay() { day = dayFrom.Day, month = dayFrom.Month, year = dayFrom.Year }, lastDay = new SeriesDay() { day = dayTo.Day, month = dayTo.Month, year = dayTo.Year } };

            GraphClient graphClient = CloudGraph.GetNeo4jGraphClient();
            graphClient.Connect();

            var sb = new StringBuilder();
            sb.AppendLine("MATCH (dayStart:Day { day: {from}.day, month: {from}.month, year: {from}.year }), (dayEnd:Day { day: {to}.day, month: {to}.month, year: {to}.year })");
            sb.AppendLine("MATCH (dayStart)-[:NEXT*0..]->(day:Day)-[:NEXT*0..]->(dayEnd)");
            sb.AppendLine("MATCH (day)<-[:ON_DAY]-(members:Stats),");
            sb.AppendLine("(members)<-[:HAS_MEMBERS]-(group:Group),");
            sb.AppendLine("(group)-[:HAS_TAG]->(tag:Tag { tag: {tag} }),");
            sb.AppendLine("(group)-[:LOCATED_IN]->(city { city: {city} })");
            sb.AppendLine("OPTIONAL MATCH (group)-[:HAS_TAG]->(tags:Tag)");
            sb.AppendLine("WITH (day.month + \"/\" + day.day + \"/\" + day.year) AS date, tags.tag as name, sum(members.count) AS membership_count");
            sb.AppendLine("ORDER BY date");
            sb.AppendLine("WITH collect(\"'\" + date + \"'\") as categories, name, collect(membership_count) as series, sum(membership_count) as sum_total");
            sb.AppendLine("ORDER BY sum_total DESC");
            sb.AppendLine("LIMIT 5");
            sb.AppendLine("RETURN categories, collect({ name:name,data: series }) AS series");

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("from", new { day = dateRange.firstDay.day, month = dateRange.firstDay.month, year = dateRange.firstDay.year });
            parameters.Add("to", new { day = dateRange.lastDay.day, month = dateRange.lastDay.month, year = dateRange.lastDay.year });
            parameters.Add("tag", tag);
            parameters.Add("city", city);

            var cypher = new CypherFluentQueryCreator(graphClient, new CypherQueryCreator(sb.ToString()), new Uri(CloudGraph.GetDatabaseUri()));
            var series = cypher.ExecuteGetCypherResults<GrowthTimeSeries>(parameters).Result.FirstOrDefault();

            if (series != null)
            {
                GrowthTimeSeries growthSeries = new GrowthTimeSeries() { categories = series.categories, series = new List<GrowthTimeSeriesMap>() };

                // Get growth percent
                foreach (var stats in series.series)
                {
                    List<float> growthPercentSeries = new List<float>();

                    // Create growth array from day over day
                    for (int i = 0; i < stats.data.Count(); i++)
                    {
                        if (i > 0)
                        {
                            float growthPercent = (float)Math.Round((double)((((float)stats.data[i] - (float)stats.data[i - 1]) / (float)stats.data[i - 1]) * 100), 2);
                            growthPercentSeries.Add(growthPercent);
                        }
                        else
                        {
                            growthPercentSeries.Add(0);
                        }
                    }

                    growthSeries.series.Add(new GrowthTimeSeriesMap() { name = stats.name, data = growthPercentSeries });
                }

                return growthSeries;
            }
            else
            {
                return null;
            }
        }

        public static DateRange GetDateRange()
        {
            GraphClient graphClient = CloudGraph.GetNeo4jGraphClient();
            graphClient.Connect();

            var sb = new StringBuilder();
            sb.AppendLine("MATCH p=(day:Day)-[:NEXT*]->()");
            sb.AppendLine("WITH nodes(p) as nodeCol");
            sb.AppendLine("ORDER BY length(nodeCol) DESC");
            sb.AppendLine("WITH nodeCol");
            sb.AppendLine("LIMIT 1");
            sb.AppendLine("WITH head(nodeCol) as firstDay, last(nodeCol) as lastDay");
            sb.AppendLine("RETURN firstDay, lastDay");

            var cypher = new CypherFluentQueryCreator(graphClient, new CypherQueryCreator(sb.ToString()), new Uri(CloudGraph.GetDatabaseUri()));
            
            DateRange dateRange = cypher.ExecuteGetCypherResults<DateRange>().Result.FirstOrDefault();

            return dateRange;
        }
    }

    public class GrowthTimeSeriesMap
    {
        public string name { get; set; }
        public List<float> data { get; set; }
    }

    public class GrowthTimeSeries
    {
        public List<string> categories { get; set; }
        public List<GrowthTimeSeriesMap> series { get; set; }
    }

    public class TimeSeriesMap
    {
        public string name { get; set; }
        public List<int> data { get; set; }
    }

    public class TimeSeries
    {
        public List<string> categories { get; set; }
        public List<TimeSeriesMap> series { get; set; }
    }

    public class DateRange
    {
        public SeriesDay firstDay { get; set; }
        public SeriesDay lastDay { get; set; }
    }

    public class SeriesDay
    {
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
    }

}
