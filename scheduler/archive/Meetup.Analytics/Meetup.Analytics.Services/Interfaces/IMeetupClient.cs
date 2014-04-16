using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meetup.Analytics.Services.Interfaces
{
    /// <summary>
    /// The IMeetupClient interface defines a contract for a REST client for the Meetup.com API
    /// </summary>
    public interface IMeetupClient
    {
        /// <summary>
        /// The ApiKey property contains a reference to the API key for accessing the Meetup.com API
        /// </summary>
        string ApiKey { get; set; }
    }
}
