using Neo4jClient;
using Neo4jClient.Cypher;
using Neo4jClient.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Meetup.Analytics.Services.Implementations
{
    /// <summary>
    /// Cypher statement containing a string Cypher query template and a list of parameters.
    /// </summary>
    public class Statement
    {
        /// <summary>
        /// The Cypher template containing parameter placeholders.
        /// </summary>
        public string statement { get; set; }

        /// <summary>
        /// A key/value list of parameters and their values to be injected into the Cypher template.
        /// </summary>
        public Dictionary<string, object> parameters { get; set; }
    }

    /// <summary>
    /// A Neo4j TransactionRequest object to be posted to the Neo4j transactional endpoint.
    /// </summary>
    public class TransactionRequest
    {
        /// <summary>
        /// A list of Neo4j Cypher statements to be sent to the Neo4j transactional endpoint.
        /// </summary>
        public List<Statement> statements { get; set; }
    }

    /// <summary>
    /// The response data for a Neo4j transactional endpoint response.
    /// </summary>
    public class Datum
    {
        /// <summary>
        /// A list of key/values returned from the transactional endpoint request.
        /// </summary>
        public List<string> row { get; set; }
    }

    /// <summary>
    /// The result wrapper for the Neo4j transactional endpoint response.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// A list of columns in the Neo4j transactional endpoint response.
        /// </summary>
        public List<string> columns { get; set; }

        /// <summary>
        /// A list of datum from the Neo4j transactional endpoint response.
        /// </summary>
        public List<Datum> data { get; set; }
    }

    /// <summary>
    /// The transaction response object containing an expiration context for the Neo4j transactional endpoint response.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// The expiration time of the current Neo4j transactional endpoint response.
        /// </summary>
        public string expires { get; set; }
    }

    /// <summary>
    /// The TransactionResponse class contains properties that bind to a Neo4j transactional endpoint response.
    /// </summary>
    public class TransactionResponse
    {
        /// <summary>
        /// Contains a URI to the Neo4j REST API to commit the current transaction within context of this class.
        /// </summary>
        public string commit { get; set; }

        /// <summary>
        /// A list of result objects containing data for each transaction statement in the context of this class.
        /// </summary>
        public List<Result> results { get; set; }

        /// <summary>
        /// The Transaction object contains the expiration time for the transactional endpoint response in the context of this class.
        /// </summary>
        public Transaction transaction { get; set; }

        /// <summary>
        /// A list of errors returned from the Neo4j transactional endpoint.
        /// </summary>
        public List<object> errors { get; set; }
    }

    public class Neo4jTransactionUtility
    {
        /// <summary>
        /// Execute a transaction on Neo4j's transactional endpoint.
        /// </summary>
        /// <param name="request">The TransactionRequest object containing statements to be transacted over the Neo4j graph.</param>
        /// <returns>Returns a TransactionResponse object containing the response from Neo4j transactional endpoint.</returns>
        public static TransactionResponse ExecuteTransaction(TransactionRequest request)
        {
            // Instantiate response object
            TransactionResponse response = default(TransactionResponse);

            // Post commit transaction to the server
            response = Post("/transaction", request);

            // Return the response to the calling class
            return response;
        }

        /// <summary>
        /// Execute a transaction on Neo4j's transactional endpoint.
        /// </summary>
        /// <param name="request">The TransactionRequest object containing statements to be transacted over the Neo4j graph.</param>
        /// <returns>Returns a TransactionResponse object containing the response from Neo4j transactional endpoint.</returns>
        public static List<T> ExecuteTransactionWithResults<T>(TransactionRequest request)
        {
            // Post commit transaction to the server
            var response = Post("/transaction", request);
            
            // Return the response to the calling class
            return PostWithResults<T>(response.commit, new TransactionRequest());
        }

        /// <summary>
        /// Commit a transaction on Neo4j's transactional endpoint with a supplied open TransactionResponse object.
        /// </summary>
        /// <param name="openTransaction">An open transaction on the Neo4j transactional endpoint.</param>
        /// <param name="request">A TransactionRequest object containing any statements that should be run before the transaction is committed.</param>
        /// <returns></returns>
        public static TransactionResponse CommitTransaction(TransactionResponse openTransaction, TransactionRequest request)
        {
            // Instantiate response object
            TransactionResponse response = default(TransactionResponse);

            // Post commit transaction to the server
            response = Post(openTransaction.commit, request);

            // Return the response to the calling class
            return response;
        }

        /// <summary>
        /// HTTP POST a transaction to the transactional endpoint via the Neo4j REST API.
        /// </summary>
        /// <param name="endpoint">The endpoint URI to HTTP POST to.</param>
        /// <param name="request">The TransactionRequest object containing statements to be executed or committed.</param>
        /// <returns>Returns a TransactionResponse object containing the results of the transaction.</returns>
        private static TransactionResponse Post(string endpoint, TransactionRequest request)
        {
            // Instantiate response object
            TransactionResponse response = default(TransactionResponse);

            // Get authenticated graph client
            GraphClient graphClient = CloudGraph.GetNeo4jGraphClient();

            // Initialize Neo4j cypher transaction context
            CypherFluentQueryCreator cypher = new CypherFluentQueryCreator(graphClient, new Uri(CloudGraph.GetDatabaseUri()));
            cypher.Connect();

            // Initialize HTTP request message
            HttpRequestMessage requestMessage = cypher.HttpPostAsJson(endpoint, request);
            requestMessage.Headers.Remove("Accept-Encoding");
            requestMessage.Headers.Add("Accept-Encoding", "UTF-8");

            // Initialize the response message from the Cypher transactional context
            HttpResponseMessage responseMessage = cypher.SendHttpRequestAsync(requestMessage, HttpStatusCode.OK).Result;

            if (responseMessage.Content.ReadAsStreamAsync().Result.Length > 0)
            {
                // Get the response string
                string responseString = Encoding.UTF8.GetString(responseMessage.Content.ReadAsByteArrayAsync().Result);

                // Deseriaize the response from the HTTP REST request into a TransactionResponse object
                response = (TransactionResponse)(new DataContractJsonSerializer(typeof(TransactionResponse))
                .ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responseString))));
            }
            return response;
        }

        /// <summary>
        /// HTTP POST a transaction to the transactional endpoint via the Neo4j REST API.
        /// </summary>
        /// <param name="endpoint">The endpoint URI to HTTP POST to.</param>
        /// <param name="request">The TransactionRequest object containing statements to be executed or committed.</param>
        /// <returns>Returns a TransactionResponse object containing the results of the transaction.</returns>
        private static List<T> PostWithResults<T>(string endpoint, TransactionRequest request)
        {
            // Get authenticated graph client
            GraphClient graphClient = CloudGraph.GetNeo4jGraphClient();

            // Initialize Neo4j cypher transaction context
            CypherFluentQueryCreator cypher = new CypherFluentQueryCreator(graphClient, new Uri(CloudGraph.GetDatabaseUri()));
            cypher.Connect();

            // Initialize HTTP request message
            HttpRequestMessage requestMessage = cypher.HttpPostAsJson(endpoint, request);
            requestMessage.Headers.Remove("Accept-Encoding");
            requestMessage.Headers.Add("Accept-Encoding", "UTF-8");

            return DeserializeResponse<T>(cypher.SendHttpRequestAsync(requestMessage, HttpStatusCode.OK), graphClient).Result;
        }

        private static async Task<List<TResult>> DeserializeResponse<TResult>(Task<HttpResponseMessage> response, IGraphClient graphClient)
        {
            var content = await response.Result.Content.ReadAsByteArrayAsync();
            string unicodeContent = Encoding.UTF8.GetString(content);

            var deserializer = new CypherJsonDeserializer<TResult>(graphClient, CypherResultMode.Projection);

            var results = deserializer
                .Deserialize(unicodeContent)
                .ToList();
            return results;
        }

        public static GraphClient GetNeo4jGraphClient()
        {
            Neo4jClient.HttpClientWrapper clientWrapper = GetNeo4jAuthenticatedClient();
            Neo4jClient.GraphClient graphClient = new Neo4jClient.GraphClient(new Uri(GetDatabaseUri()), clientWrapper);
            return graphClient;
        }

        public static Neo4jClient.HttpClientWrapper GetNeo4jAuthenticatedClient()
        {
            // Get authentication header from configuration "user:password"
            var authentication = CloudGraph.GetAuthorizationHeader();

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Convert.ToBase64String(Encoding.ASCII.GetBytes(authentication)));
            Neo4jClient.HttpClientWrapper clientWrapper = new Neo4jClient.HttpClientWrapper(httpClient);
            return clientWrapper;
        }

        public static string GetDatabaseUri()
        {
            if (Configuration.Initialized)
            {
                return Configuration.Neo4jConnectionString;
            }
            else
            {
                return CloudGraph.GetNeo4jConnectionString();
            }
        }
    }
}