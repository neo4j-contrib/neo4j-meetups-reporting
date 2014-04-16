using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Meetup.Analytics.Services.Implementations
{
    public class CloudGraph
    {
        public static CloudStorageAccount GetCloudStorageAccount()
        {
            if (Configuration.Initialized)
            {
                return Configuration.DataConnectionString;
            }
            else
            {
                return GetDataConnectionString();
            }
        }

        public static CloudStorageAccount GetDataConnectionString()
        {
            try
            {
                if (CloudConfigurationManager.GetSetting("DataConnectionString") != null)
                {
                    string storageId = CloudConfigurationManager.GetSetting("DataConnectionString");
                    return CloudStorageAccount.Parse(storageId);
                }
            }
            catch (Exception)
            {
            }

            string connString = ConfigurationManager.AppSettings["DataConnectionString"];
            return CloudStorageAccount.Parse(connString);

        }

        public static string GetDatabaseUri()
        {
            if (Configuration.Initialized)
            {
                return Configuration.Neo4jConnectionString;
            }
            else
            {
                return GetNeo4jConnectionString();
            }
        }

        public static string GetNeo4jConnectionString()
        {
            try
            {
                if (CloudConfigurationManager.GetSetting("Neo4j.ConnectionString") != null)
                {
                    var databaseUri = CloudConfigurationManager.GetSetting("Neo4j.ConnectionString");
                    return databaseUri;
                }
            }
            catch (Exception)
            {
            }
            // Get database URI from configuration
            return ConfigurationManager.AppSettings["Neo4j.ConnectionString"];
        }

        public static string GetReadDatabaseUri()
        {
            try
            {
                if (CloudConfigurationManager.GetSetting("Neo4j.ConnectionString") != null)
                {
                    var databaseUri = CloudConfigurationManager.GetSetting("Neo4j.ConnectionString");
                    return databaseUri;
                }
            }
            catch (Exception)
            {
            }
            // Get database URI from configuration
            return ConfigurationManager.AppSettings["Neo4j.ConnectionString"];
        }

        public static string GetAuthorizationHeader()
        {
            if (Configuration.Initialized)
            {
                return Configuration.Neo4jConnectionStringAuthentication;
            }
            else
            {
                return GetNeo4jConnectionStringAuthentication();
            }
        }

        public static string GetServiceBusConnectionString()
        {
            if (Configuration.Initialized)
            {
                return Configuration.MicrosoftServiceBusConnectionString;
            }
            else
            {
                return _GetServiceBusConnectionString();
            }
        }

        public static string _GetServiceBusConnectionString()
        {
            try
            {
                if (CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString") != null)
                {
                    var databaseUri = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
                    return databaseUri;
                }
            }
            catch (Exception)
            {
            }
            // Get database URI from configuration
            return ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        }

        public static Neo4jClient.HttpClientWrapper GetNeo4jAuthenticatedClient()
        {
            // Get authentication header from configuration "user:password"
            var authentication = GetAuthorizationHeader();

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Convert.ToBase64String(Encoding.ASCII.GetBytes(authentication)));
            Neo4jClient.HttpClientWrapper clientWrapper = new Neo4jClient.HttpClientWrapper(httpClient);
            return clientWrapper;
        }

        public static string GetNeo4jConnectionStringAuthentication()
        {
            try
            {
                if (CloudConfigurationManager.GetSetting("Neo4j.ConnectionString.Authentication") != null)
                {
                    var authentication = CloudConfigurationManager.GetSetting("Neo4j.ConnectionString.Authentication");
                    return authentication;
                }
            }
            catch (Exception)
            {
            }
            // Get database URI from configuration
            return ConfigurationManager.AppSettings["Neo4j.ConnectionString.Authentication"];
        }

        public static GraphClient GetReadNeo4jGraphClient()
        {
            Neo4jClient.HttpClientWrapper clientWrapper = GetNeo4jAuthenticatedClient();
            Neo4jClient.GraphClient graphClient = new Neo4jClient.GraphClient(new Uri(GetReadDatabaseUri()), clientWrapper);
            return graphClient;
        }

        public static GraphClient GetNeo4jGraphClient()
        {
            Neo4jClient.HttpClientWrapper clientWrapper = GetNeo4jAuthenticatedClient();
            Neo4jClient.GraphClient graphClient = new Neo4jClient.GraphClient(new Uri(GetDatabaseUri()), clientWrapper);
            return graphClient;
        }
    }
}
