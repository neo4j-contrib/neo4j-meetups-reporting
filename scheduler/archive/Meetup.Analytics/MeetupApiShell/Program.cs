using Meetup.Analytics.Services.Implementations;
using Meetup.Analytics.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetupApiShell
{
    class Program
    {
        static void Main(string[] args)
        {
            Import.PopulateGraphDatabaseFromCsv(new MeetupClient("4b1b7a4c4027718192d1e73723135b").GetMeetupCityCsvByTag(new MeetupCity() { city = "San Francisco", state = "CA", country = "US" }, "NoSQL"));
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
