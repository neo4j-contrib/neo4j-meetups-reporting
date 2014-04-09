using Meetup.Analytics.Services.Interfaces;
using Meetup.Analytics.Services.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meetup.Analytics.Services.Implementations
{
    public class MeetupClient : IMeetupClient
    {
        private const string FIND_GROUPS_REQUEST_TEMPLATE = "https://api.meetup.com/2/groups?&sign=true&topic={0}&country={1}&city={2}&page={3}&key={4}";
        private string _ApiKey = string.Empty;

        /// <summary>
        /// The ApiKey property contains a reference to the API key for accessing the Meetup.com API
        /// </summary>
        public string ApiKey
        {
            get
            {
                return _ApiKey;
            }
            set
            {
                _ApiKey = value;
            }
        }

        /// <summary>
        /// Instantiate a new MeetupClient for communicating with the Meetup.com REST API.
        /// </summary>
        /// <param name="apiKey">The API key provided by Meetup.com for authenticated access to the REST API.</param>
        public MeetupClient(string apiKey)
        {
            _ApiKey = apiKey;
        }

        /// <summary>
        /// Get meetup groups from the Meetup.com API with a topic, country code, city, and result size limit (default 20).
        /// </summary>
        /// <param name="topic">The topic of the meetup groups to retrieve.</param>
        /// <param name="country">The country the meetup group resides in.</param>
        /// <param name="city">The city the meetup group resides in.</param>
        /// <param name="limit">The number of results to return.</param>
        /// <returns></returns>
        public MeetupGroups GetMeetupGroups(string topic, string country, string city, int limit)
        {
            MeetupGroups meetupGroups = default(MeetupGroups);

            string requestUri = string.Format(FIND_GROUPS_REQUEST_TEMPLATE, topic, country, city, limit, _ApiKey);

            var request = System.Net.WebRequest.Create(requestUri) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.Accept = "application/json";
            request.ContentLength = 0;
            string responseContent;
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    responseContent = reader.ReadToEnd();
                }
            }

            meetupGroups = Newtonsoft.Json.JsonConvert.DeserializeObject<MeetupGroups>(responseContent);

            return meetupGroups;
        }

        /// <summary>
        /// Get meetup groups from the Meetup.com API with a topic, country code, city, and result size limit (default 20).
        /// </summary>
        /// <param name="topic">The topic of the meetup groups to retrieve.</param>
        /// <param name="country">The country the meetup group resides in.</param>
        /// <param name="city">The city the meetup group resides in.</param>
        /// <param name="limit">The number of results to return.</param>
        /// <param name="state">The state code is required if the country is the United States.</param>
        /// <returns></returns>
        public MeetupGroups GetMeetupGroups(string topic, string country, string city, int limit, string state)
        {
            MeetupGroups meetupGroups = default(MeetupGroups);

            string requestUri = string.Format(FIND_GROUPS_REQUEST_TEMPLATE + "&state={5}", topic, country, city, limit, _ApiKey, state);

            var request = System.Net.WebRequest.Create(requestUri) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.Accept = "application/json";
            request.ContentLength = 0;
            string responseContent;
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    responseContent = reader.ReadToEnd();
                }
            }

            meetupGroups = Newtonsoft.Json.JsonConvert.DeserializeObject<MeetupGroups>(responseContent);

            return meetupGroups;
        }

        /// <summary>
        /// Gets a normalized CSV file representation for data importing into Neo4j using LOAD CSV Cypher.
        /// </summary>
        /// <param name="meetupCity">The meetup city to search for on the Meetup.com API.</param>
        /// <param name="tag">The Meetup.com topic to limit results to.</param>
        /// <returns>Returns a CSV string containing the normalized Neo4j graph data model.</returns>
        public string GetMeetupCityCsvByTag(List<MeetupCity> meetupCity, string tag)
        {
            
            // Generate CSV file with headers
            string header = "group_name,group_location,group_country,group_state,group_tag,group_stats,month,day,year,last_month,last_day,last_year,group_creation_date,group_creation_date_year,group_creation_date_month,group_creation_date_day";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(header);

            DateTime today = DateTime.Now;
            DateTime yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            foreach (var city in meetupCity)
            {
                MeetupGroups meetupGroups = this.GetMeetupGroups(tag, city.country, city.city, 100, city.state);

                meetupGroups.results.ForEach(mg =>
                {
                    // Back date data from the group creation date using linear scaling
                    DateTime createdDate = FromUnixTime(mg.created);
                    foreach (var topic in mg.topics)
                    {
                        sb.AppendLine(new string[] { 
                            mg.name, 
                            mg.city.Replace("München", "Munich").Replace("Malmö", "Malmo"),
                            mg.country,
                            mg.state ?? "",
                            topic.name, 
                            mg.members.ToString(),
                            today.Month.ToString(), 
                            today.Day.ToString(), 
                            today.Year.ToString(),
                            yesterday.Month.ToString(),
                            yesterday.Day.ToString(), 
                            yesterday.Year.ToString(),
                            mg.created.ToString(),
                            createdDate.Year.ToString(),
                            createdDate.Month.ToString(),
                            createdDate.Day.ToString()
                        }.Select(s => StringToCSVCell(s))
                            .ToList()
                            .Aggregate((a, b) => string.Format("{0},{1}", a, b)));
                    }
                });
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a normalized CSV file representation for data importing into Neo4j using LOAD CSV Cypher.
        /// </summary>
        /// <param name="meetupCity">The meetup city to search for on the Meetup.com API.</param>
        /// <param name="tag">The Meetup.com topic to limit results to.</param>
        /// <returns>Returns a CSV string containing the normalized Neo4j graph data model.</returns>
        public string GetMeetupCityCsvByTagAndInterpolate(MeetupCity meetupCity, string tag)
        {

            // Generate CSV file with headers
            string header = "group_name,group_location,group_country,group_state,group_tag,group_stats,month,day,year,day_timestamp,day_of_week,week_of_year,last_month,last_day,last_year,group_creation_date,group_creation_date_year,group_creation_date_month,group_creation_date_day";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(header);

            DateTime today = DateTime.Now;
            DateTime yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            //foreach (var city in meetupCity)
            //{
            var city= meetupCity; 
                MeetupGroups meetupGroups = this.GetMeetupGroups(tag, city.country, city.city, 100, city.state);

                meetupGroups.results.ForEach(mg =>
                {
                    // Back date data from the group creation date using linear scaling
                    DateTime createdDate = FromUnixTime(mg.created);

                    // Create a list of dates to use as data points
                    // Count the amount of days since the creation date
                    int dayCount = DateTime.Now.Subtract(createdDate).Days;

                    // Create an index of data points and calculate linear interpolation for a given point
                    int[] membershipPoints = new int[dayCount];

                    for (int i = 0; i < membershipPoints.Length; i++)
                    {
                        // Calculate membership count using linear interpolation
                        membershipPoints[i] =  (int)(((float)mg.members / ((float)membershipPoints.Length)) * (float)i);
                    }

                    // Get week of year
                    DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                    Calendar cal = dfi.Calendar;

                    foreach (var topic in mg.topics)
                    {
                        sb.AppendLine(new string[] { 
                            mg.name, 
                            mg.city.Replace("München", "Munich").Replace("Malmö", "Malmo"),
                            mg.country,
                            mg.state ?? "",
                            topic.name, 
                            mg.members.ToString(),
                            today.Month.ToString(), 
                            today.Day.ToString(), 
                            today.Year.ToString(),
                            today.Ticks.ToString(),
                            ((int)today.DayOfWeek).ToString(),
                            cal.GetWeekOfYear(today, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString(),
                            yesterday.Month.ToString(),
                            yesterday.Day.ToString(), 
                            yesterday.Year.ToString(),
                            mg.created.ToString(),
                            createdDate.Year.ToString(),
                            createdDate.Month.ToString(),
                            createdDate.Day.ToString()
                        }.Select(s => StringToCSVCell(s))
                            .ToList()
                            .Aggregate((a, b) => string.Format("{0},{1}", a, b)));
                    }

                    for (int i = (membershipPoints.Length > 365) ? membershipPoints.Length - 365 : 0; i < membershipPoints.Length; i++)
                    {

                        DateTime thisDay = DateTime.Now.Subtract(TimeSpan.FromDays(membershipPoints.Length - i));
                        DateTime dayBeforeThisDay = thisDay.Subtract(TimeSpan.FromDays(1));

                        sb.AppendLine(new string[] { 
                            mg.name, 
                            mg.city.Replace("München", "Munich").Replace("Malmö", "Malmo"),
                            mg.country,
                            mg.state ?? "",
                            "NoSQL", 
                            membershipPoints[i].ToString(),
                            thisDay.Month.ToString(), 
                            thisDay.Day.ToString(), 
                            thisDay.Year.ToString(),
                            thisDay.Ticks.ToString(),
                            ((int)thisDay.DayOfWeek).ToString(),
                            cal.GetWeekOfYear(thisDay, dfi.CalendarWeekRule, dfi.FirstDayOfWeek).ToString(),
                            dayBeforeThisDay.Month.ToString(),
                            dayBeforeThisDay.Day.ToString(), 
                            dayBeforeThisDay.Year.ToString(),
                            mg.created.ToString(),
                            createdDate.Year.ToString(),
                            createdDate.Month.ToString(),
                            createdDate.Day.ToString()
                        }.Select(s => StringToCSVCell(s))
                            .ToList()
                            .Aggregate((a, b) => string.Format("{0},{1}", a, b)));
                    }
                });
            //}

            return sb.ToString();
        }

        public DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

        /// <summary>
        /// Turn a string into a CSV cell output
        /// </summary>
        /// <param name="str">String to output</param>
        /// <returns>The CSV cell formatted string</returns>
        public static string StringToCSVCell(string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }
    }
}
