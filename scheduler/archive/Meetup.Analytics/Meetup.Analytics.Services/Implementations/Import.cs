using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Meetup.Analytics.Services.Implementations
{
    /// <summary>
    /// This class is meant to replace Neo4j indexing for RelevantSocialCloud in CloudGraph.cs
    /// </summary>
    public class Import
    {
        public static void PopulateGraphDatabaseFromCsv(List<string> csvFiles)
        {
            GraphClient graphClient = CloudGraph.GetNeo4jGraphClient();
            graphClient.Connect();

            foreach (var csvFile in csvFiles)
            {
                // Create file HTTP request hook
                Uri blobUri = BlobService.PutBlobGetUri("csvfile", Guid.NewGuid().ToString(), new MemoryStream(Encoding.UTF8.GetBytes(csvFile)), "cache");

                // Run Cypher query for data import
                SaveToDatabase(blobUri.ToString(), graphClient);
            }
        }

        /// <summary>
        /// Populate database from cloud hosted CSV file.
        /// </summary>
        /// <param name="uri">The URI of the CSV file hosted on Windows Azure Blob Storage.</param>
        private static void SaveToDatabase(string uri, GraphClient graphClient)
        {
            var sb = new StringBuilder();
            //sb.AppendLine("USING PERIODIC COMMIT 50000");
            sb.AppendLine("LOAD CSV WITH HEADERS FROM");
            sb.AppendLine(string.Format(@"    ""{0}""", uri.Replace("https://", "http://")));
            sb.AppendLine("AS csvLine");
            sb.AppendLine("MERGE (group:Group { name: csvLine.group_name })");
            sb.AppendLine("ON CREATE SET group.created = toInt(csvLine.group_creation_date)");
            sb.AppendLine("ON CREATE SET group.year = toInt(csvLine.group_creation_date_year)");
            sb.AppendLine("ON CREATE SET group.month = toInt(csvLine.group_creation_date_month)");
            sb.AppendLine("ON CREATE SET group.day = toInt(csvLine.group_creation_date_day)");
            sb.AppendLine("MERGE (location:Location { city: csvLine.group_location, country: csvLine.group_country, state: csvLine.group_state })");
            sb.AppendLine("MERGE (tag:Tag { tag: csvLine.group_tag })");
            sb.AppendLine("MERGE (stats:Stats { group_name: csvLine.group_name, month: toInt(csvLine.last_month), day: toInt(csvLine.last_day), year: toInt(csvLine.last_year) })");
            sb.AppendLine("ON CREATE SET stats.count = toInt(csvLine.group_stats)");
            sb.AppendLine("MERGE (day:Day { month: toInt(csvLine.month), day: toInt(csvLine.day), year: toInt(csvLine.year) })");
            sb.AppendLine("ON MATCH SET day.timestamp = toInt(csvLine.day_timestamp)");
            sb.AppendLine("ON MATCH SET day.dayofweek = toInt(csvLine.day_of_week)");
            sb.AppendLine("MERGE (week:Week { year: toInt(csvLine.year), week: toInt(csvLine.week_of_year) })");
            sb.AppendLine("MERGE (week)-[:HAS_DAY]->(day)");
            sb.AppendLine("MERGE (month:Month { year: toInt(csvLine.year), month: toInt(csvLine.month) })");
            sb.AppendLine("MERGE (month)-[:HAS_DAY]->(day)");
            sb.AppendLine("MERGE (year:Year { year: toInt(csvLine.year) })");
            sb.AppendLine("MERGE (year)-[:HAS_MONTH]->(month)");
            sb.AppendLine("MERGE (lastDay:Day { month: toInt(csvLine.last_month), day: toInt(csvLine.last_day), year: toInt(csvLine.last_year) })");
            sb.AppendLine("MERGE (group)-[:LOCATED_IN]->(location)");
            sb.AppendLine("MERGE (group)-[:HAS_TAG]->(tag)");
            sb.AppendLine("MERGE (group)-[:HAS_MEMBERS]->(stats)");
            sb.AppendLine("MERGE (stats)-[:ON_DAY]->(day)");
            sb.AppendLine("MERGE (lastDay)-[:NEXT]->(day)");
            sb.AppendLine("RETURN day.day as day");

            var cypher = new CypherFluentQueryCreator(graphClient, new CypherQueryCreator(sb.ToString()), new Uri(CloudGraph.GetDatabaseUri()));
            var day = cypher.ExecuteGetCypherResults<SeriesDay>().Result;
        }
    }
}
