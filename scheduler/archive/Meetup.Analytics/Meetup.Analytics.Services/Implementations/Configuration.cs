using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Storage;

namespace Meetup.Analytics.Services.Implementations
{
    /// <summary>
    /// Utilities class used to load and retrieve platform configurations.
    /// </summary>
    public sealed class Configuration
    {
        #region Configuration Static Methods

        #region Private Fields

        // Configuration private fields
        private CloudStorageAccount _dataConnectionString = default(CloudStorageAccount);
        private string _microsoftServiceBusConnectionString = string.Empty;
        private string _neo4jConnectionString = string.Empty;
        private string _neo4jConnectionStringAuthentication = string.Empty;
        private string _blobStorageAddress = string.Empty;
        
        // Singleton state flag
        private bool _initialized = false;

        // Instance
        private static readonly Configuration _instance = new Configuration();

        #endregion

        #region Public Fields

        /// <summary>
        /// The data connection string to Microsoft Windows Azure Blob Storage.
        /// </summary>
        public static CloudStorageAccount DataConnectionString { get { _instance.Internal_CheckEnforceIllegalAccess(); return _instance._dataConnectionString; } }

        /// <summary>
        /// The service bus connection string to Microsoft Windows Azure Service Queues.
        /// </summary>
        public static string MicrosoftServiceBusConnectionString { get { _instance.Internal_CheckEnforceIllegalAccess(); return _instance._microsoftServiceBusConnectionString; } }

        /// <summary>
        /// The Neo4j connection string to this instance's memory store.
        /// </summary>
        public static string Neo4jConnectionString { get { _instance.Internal_CheckEnforceIllegalAccess(); return _instance._neo4jConnectionString; } }

        /// <summary>
        /// The Neo4j connection string's authentication mechanism.
        /// </summary>
        public static string Neo4jConnectionStringAuthentication { get { _instance.Internal_CheckEnforceIllegalAccess(); return _instance._neo4jConnectionStringAuthentication; } }

        /// <summary>
        /// The blob storage URI for this instance.
        /// </summary>
        public static string BlobStorageAddress { get { _instance.Internal_CheckEnforceIllegalAccess(); return _instance._blobStorageAddress; } }

        /// <summary>
        /// The instance of the singleton configuration class for the Core assembly.
        /// </summary>
        public static Configuration Instance { get { _instance.Internal_CheckEnforceIllegalAccess(); return _instance; } }

        /// <summary>
        /// Flag containing the initialization status of the singleton configuration class. If this flag is set to false,
        /// access to its other properties will result in an UnauthorizedAccessException.
        /// </summary>
        public static bool Initialized { get { _instance.Internal_CheckEnforceIllegalAccess(); return _instance._initialized; } }

        #endregion

        /// <summary>
        /// Private constructor for the singleton configuration class.
        /// </summary>
        private Configuration()
        {

        }

        /// <summary>
        /// Initialization method used to instantiate configuration data embedded as a resource in the Core assembly.
        /// </summary>
        public static void Initialize()
        {
            lock (_instance)
            {
                if (!_instance._initialized)
                {
                    // Initialize configuration values
                    _instance._dataConnectionString = CloudGraph.GetDataConnectionString();
                    _instance._microsoftServiceBusConnectionString = CloudGraph._GetServiceBusConnectionString();
                    _instance._neo4jConnectionString = CloudGraph.GetNeo4jConnectionString();
                    _instance._neo4jConnectionStringAuthentication = CloudGraph.GetNeo4jConnectionStringAuthentication();

                    // Set the initialized flag to true
                    _instance._initialized = true;
                }
            }
        }

        /// <summary>
        /// Check to enforce that the singleton configuration class has been properly initialized before access.
        /// </summary>
        /// <returns>Returns true of the singleton configuration class has been initialized.</returns>
        public static bool CheckEnforceIllegalAccess()
        {
            return _instance.Internal_CheckEnforceIllegalAccess();
        }

        /// <summary>
        /// Internal reference check to enforce that the singleton configuration class has been properly initialized before access.
        /// </summary>
        /// <returns>Returns true of the singleton configuration class has been initialized.</returns>
        private bool Internal_CheckEnforceIllegalAccess()
        {
            if (_instance._initialized)
            {
                return true;
            }
            else
            {
                try
                {
                    Configuration.Initialize();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        #endregion
    }
}
