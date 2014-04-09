using Neo4jClient;
using Neo4jClient.Cypher;
using Neo4jClient.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;


namespace Meetup.Analytics.Services.Implementations
{
    [JsonObject]
    public class Extensions
    {
    }

    [JsonObject]
    public class BatchApiResponseBody
    {
        [JsonProperty]
        public Extensions extensions { get; set; }
        [JsonProperty]
        public string paged_traverse { get; set; }
        [JsonProperty]
        public string outgoing_relationships { get; set; }
        [JsonProperty]
        public string traverse { get; set; }
        [JsonProperty]
        public string all_typed_relationships { get; set; }
        [JsonProperty]
        public string all_relationships { get; set; }
        [JsonProperty]
        public string property { get; set; }
        [JsonProperty]
        public string self { get; set; }
        [JsonProperty]
        public string outgoing_typed_relationships { get; set; }
        [JsonProperty]
        public string properties { get; set; }
        [JsonProperty]
        public string incoming_relationships { get; set; }
        [JsonProperty]
        public string incoming_typed_relationships { get; set; }
        [JsonProperty]
        public string create_relationship { get; set; }
        [JsonProperty]
        public Data data { get; set; }
        public string start { get; set; }
        [JsonProperty]
        public string type { get; set; }
        [JsonProperty]
        public string end { get; set; }
        [JsonProperty]
        public string indexed { get; set; }
    }

    [JsonObject]
    public class BatchApiResponse
    {
        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public string location { get; set; }
        [JsonProperty]
        public BatchApiResponseBody body { get; set; }
        [JsonProperty]
        public string from { get; set; }
    }

    [JsonObject]
    public class Data
    {
        [JsonProperty]
        public string since { get; set; }
    }

    [JsonObject]
    public class Body
    {
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public int? age { get; set; }
        [JsonProperty]
        public string to { get; set; }
        [JsonProperty]
        public Data data { get; set; }
        [JsonProperty]
        public string type { get; set; }
        [JsonProperty(PropertyName="key")]
        public string itemkey { get; set; }
        [JsonProperty]
        public string Key { get; set; }
        [JsonProperty]
        public string Content { get; set; }
        [JsonProperty]
        public string Weight { get; set; }
        [JsonProperty]
        public string value { get; set; }
        [JsonProperty]
        public string uri { get; set; }
        [JsonProperty]
        public string phrase { get; set; }
        [JsonProperty]
        public string Expression { get; set; }
    }



    [JsonObject]
    public class ApiAction
    {
        [JsonProperty]
        public string method { get; set; }
        [JsonProperty]
        public string to { get; set; }
        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public Body body { get; set; }
    }

    [JsonObject]
    public class ApiActionLabel
    {
        [JsonProperty]
        public string method { get; set; }
        [JsonProperty]
        public string to { get; set; }
        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public string[] body { get; set; }
    }

    public class CypherQueryCreator
    {
        IDictionary<string, object> queryParameters = new Dictionary<string, object>();
        IList<object> startBits = new List<object>();
        string matchText;
        string relateText;
        string createUniqueText;
        string whereText;
        string whereMatchText;
        IList<object> createBits = new List<object>();
        string deleteText;
        string withText;
        string returnText;
        bool returnDistinct;
        CypherResultMode resultMode;
        int? limit;
        int? skip;
        string orderBy;
        string setText;
        public string queryText = null;
        

        CypherQueryCreator Clone()
        {
            return new CypherQueryCreator()
            {
                queryParameters = queryParameters,
                createBits = createBits,
                deleteText = deleteText,
                matchText = matchText,
                relateText = relateText,
                createUniqueText = createUniqueText,
                whereText = whereText,
                whereMatchText = whereMatchText,
                withText = withText,
                returnText = returnText,
                returnDistinct = returnDistinct,
                resultMode = resultMode,
                limit = limit,
                skip = skip,
                startBits = startBits,
                orderBy = orderBy,
                setText = setText,
                queryText = queryText
            };
        }

        public CypherQueryCreator()
        {
            
        }

        public CypherQueryCreator(string queryText)
        {
            this.queryText = queryText;
        }

        public CypherQueryCreator AddStartBit(string identity, string startText)
        {
            var newBuilder = Clone();
            newBuilder.startBits.Add(new RawCypherStartBit(identity, startText));
            return newBuilder;
        }

        public CypherQueryCreator AddStartBit(string identity, params NodeReference[] nodeReferences)
        {
            var newBuilder = Clone();
            newBuilder.startBits.Add(new CypherStartBit(identity, "node", nodeReferences.Select(r => r.Id).ToArray()));
            return newBuilder;
        }

        public CypherQueryCreator AddStartBit(string identity, params RelationshipReference[] relationshipReferences)
        {
            var newBuilder = Clone();
            newBuilder.startBits.Add(new CypherStartBit(identity, "relationship", relationshipReferences.Select(r => r.Id).ToArray()));
            return newBuilder;
        }

        public CypherQueryCreator AddStartBitWithNodeIndexLookup(string identity, string indexName, string parameterText)
        {
            var newBuilder = Clone();
            newBuilder.startBits.Add(new CypherStartBitWithNodeIndexLookupWithSingleParameter(identity, indexName, parameterText));
            return newBuilder;
        }

        public CypherQueryCreator AddStartBitWithNodeIndexLookup(string identity, string indexName, string key, object value)
        {
            var newBuilder = Clone();
            newBuilder.startBits.Add(new CypherStartBitWithNodeIndexLookup(identity, indexName, key, value));
            return newBuilder;
        }

        public CypherQueryCreator SetDeleteText(string text)
        {
            var newBuilder = Clone();
            newBuilder.deleteText = text;
            return newBuilder;
        }

        public CypherQueryCreator SetMatchText(string text)
        {
            var newBuilder = Clone();
            newBuilder.matchText = text;
            return newBuilder;
        }

        public CypherQueryCreator SetRelateText(string text)
        {
            var newBuilder = Clone();
            newBuilder.relateText = text;
            return newBuilder;
        }

        public CypherQueryCreator SetWithText(string text)
        {
            var newBuilder = Clone();
            newBuilder.withText = text;
            return newBuilder;
        }

        public CypherQueryCreator SetCreateUniqueText(string text)
        {
            var newBuilder = Clone();
            newBuilder.createUniqueText = text;
            return newBuilder;
        }

        public CypherQueryCreator SetCreateText(string text)
        {
            var newBuilder = Clone();
            newBuilder.createBits.Add(new CypherCreateTextBit(text));
            return newBuilder;
        }

        public CypherQueryCreator SetWhere(string text)
        {
            var newBuilder = Clone();
            newBuilder.whereText += string.Format("({0})", text);
            return newBuilder;
        }

        public CypherQueryCreator SetWhereMatch(string text)
        {
            var newBuilder = Clone();
            newBuilder.whereMatchText += string.Format("({0})", text);
            return newBuilder;
        }

        public CypherQueryCreator SetWhere(LambdaExpression expression)
        {
            var newBuilder = Clone();
            newBuilder.whereText += whereText = CypherWhereExpressionBuilder.BuildText(expression, new Func<object,string>(str => (string)queryParameters[str.ToString()]));
            return newBuilder;
        }

        public CypherQueryCreator SetAnd()
        {
            var newBuilder = Clone();
            newBuilder.whereText += " AND ";
            return newBuilder;
        }

        public CypherQueryCreator SetOr()
        {
            var newBuilder = Clone();
            newBuilder.whereText += " OR ";
            return newBuilder;
        }

        public CypherQueryCreator SetReturnText(string returnText)
        {
            var newBuilder = Clone();
            newBuilder.returnText = returnText;
            return newBuilder;
        }

        public CypherQueryCreator SetReturn(string identity, bool distinct, CypherResultMode mode = CypherResultMode.Set)
        {
            var newBuilder = Clone();
            newBuilder.returnText = identity;
            newBuilder.returnDistinct = distinct;
            newBuilder.resultMode = mode;
            return newBuilder;
        }

        public CypherQueryCreator SetReturn(LambdaExpression expression, bool distinct)
        {
            var newBuilder = Clone();
            newBuilder.returnText = CypherReturnExpressionBuilder.BuildText(expression, new CypherCapabilities(), new List<JsonConverter>() { new TypeConverterBasedJsonConverter() }).Text;
            newBuilder.returnDistinct = distinct;
            newBuilder.resultMode = CypherResultMode.Projection;
            return newBuilder;
        }

        public CypherQueryCreator SetLimit(int? count)
        {
            var newBuilder = Clone();
            newBuilder.limit = count;
            return newBuilder;
        }

        public CypherQueryCreator SetSkip(int? count)
        {
            var newBuilder = Clone();
            newBuilder.skip = count;
            return newBuilder;
        }

        public CypherQueryCreator SetOrderBy(OrderByType orderByType, params string[] properties)
        {
            var newBuilder = Clone();
            newBuilder.orderBy = string.Join(", ", properties);

            if (orderByType == OrderByType.Descending)
                newBuilder.orderBy += " DESC";

            return newBuilder;
        }

        public CypherQueryCreator SetSetText(string text)
        {
            var newBuilder = Clone();
            newBuilder.setText = text;
            return newBuilder;
        }

        public CypherQuery ToQuery()
        {
            if (queryText != null)
            {
                return new CypherQuery(queryText, queryParameters, resultMode);
            }
            else
            {
                var queryTextBuilder = new StringBuilder();
                WriteStartClause(queryTextBuilder, queryParameters);
                WriteMatchClause(queryTextBuilder);
                if (!string.IsNullOrEmpty(whereMatchText))
                {
                    WriteWhereMatchClause(queryTextBuilder);
                }

                WriteRelateClause(queryTextBuilder);
                WriteWithClause(queryTextBuilder);
                WriteCreateUniqueClause(queryTextBuilder);
                WriteCreateClause(queryTextBuilder);
                WriteWhereClause(queryTextBuilder);
                WriteDeleteClause(queryTextBuilder);
                WriteSetClause(queryTextBuilder);
                WriteReturnClause(queryTextBuilder);
                WriteOrderByClause(queryTextBuilder);
                WriteSkipClause(queryTextBuilder, queryParameters);
                WriteLimitClause(queryTextBuilder, queryParameters);
                return new CypherQuery(queryTextBuilder.ToString(), queryParameters, resultMode);
            }
            
        }

        public static string CreateParameter(IDictionary<string, object> parameters, object paramValue)
        {
            var paramName = string.Format("p{0}", parameters.Count);
            parameters.Add(paramName, paramValue);
            return "{" + paramName + "}";
        }

        void WriteStartClause(StringBuilder target, IDictionary<string, object> paramsDictionary)
        {
            if (startBits.Any()) {
                target.Append("START ");

                var formattedStartBits = startBits.Select(bit => {
                    var standardStartBit = bit as CypherStartBit;
                    if (standardStartBit != null) {
                        var lookupIdParameterNames = standardStartBit
                            .LookupIds
                            .Select(i => CreateParameter(paramsDictionary, i))
                            .ToArray();
                        
                        var lookupContent = string.Join(", ", lookupIdParameterNames);
                        return string.Format("{0}={1}({2})",
                            standardStartBit.Identifier,
                            standardStartBit.LookupType,
                            lookupContent);
                    }

                    var rawStartBit = bit as RawCypherStartBit;
                    if (rawStartBit != null)
                    {
                        return string.Format("{0}={1}", rawStartBit.Identifier, rawStartBit.StartText);
                    }
                    
                    var startBithWithNodeIndexLookup = bit as CypherStartBitWithNodeIndexLookup;
                    if (startBithWithNodeIndexLookup != null) {
                        var valueParameter = CreateParameter(paramsDictionary, startBithWithNodeIndexLookup.Value);
                        return string.Format("{0}=node:{1}({2} = {3})",
                            startBithWithNodeIndexLookup.Identifier,
                            startBithWithNodeIndexLookup.IndexName,
                            startBithWithNodeIndexLookup.Key,
                            valueParameter);
                    }
                    
                    var startBithWithNodeIndexLookupSingleParameter = bit as CypherStartBitWithNodeIndexLookupWithSingleParameter;
                    if (startBithWithNodeIndexLookupSingleParameter != null) {
                        var valueParameter = CreateParameter(paramsDictionary, startBithWithNodeIndexLookupSingleParameter.Parameter);
                        return string.Format("{0}=node:{1}({2})",
                            startBithWithNodeIndexLookupSingleParameter.Identifier,
                            startBithWithNodeIndexLookupSingleParameter.IndexName,
                            valueParameter);
                    }
                    
                    throw new NotSupportedException(string.Format("Start bit of type {0} is not supported.", bit.GetType().FullName));
                });

                target.Append(string.Join(", ", formattedStartBits));
            } 
        }

        void WriteMatchClause(StringBuilder target)
        {
            if (matchText == null) return;
            target.AppendFormat("\r\nMATCH {0}", matchText);

            //if (whereText != null)
            //{
            //    WriteWhereClause(target);
            //}
        }

        void WriteDeleteClause(StringBuilder target)
        {
            if (deleteText == null) return;
            target.AppendFormat("\r\nDELETE {0}", deleteText);
        }

        void WriteRelateClause(StringBuilder target)
        {
            if (relateText == null) return;
            target.AppendFormat("\r\nRELATE {0}", relateText);
        }

        void WriteWithClause(StringBuilder target)
        {
            if (withText == null) return;
            target.AppendFormat("\r\nWITH {0}", withText);
        }

        void WriteCreateUniqueClause(StringBuilder target)
        {
            if (createUniqueText == null) return;
            target.AppendFormat("\r\nCREATE UNIQUE {0}", createUniqueText);
        }

        void WriteCreateClause(StringBuilder target)
        {
            if (createBits.Any())
            {
                target.Append("\r\nCREATE ");
                var formattedCreateBits = createBits.Select(bit =>
                {
                    var createTextbit = bit as CypherCreateTextBit;
                    if (createTextbit != null)
                    {
                        return createTextbit.CreateText;
                    }

                    throw new NotSupportedException(string.Format("Create bit of type {0} is not supported.", bit.GetType().FullName));
                });

                target.Append(string.Join("", formattedCreateBits));
            }
        }

        void WriteWhereClause(StringBuilder target)
        {
            if (whereText == null)
                return;

            target.Append("\r\nWHERE ");
            target.Append(whereText);
        }

        void WriteWhereMatchClause(StringBuilder target)
        {
            if (whereMatchText == null)
                return;

            target.Append("\r\nWHERE ");
            target.Append(whereMatchText);
        }

        void WriteReturnClause(StringBuilder target)
        {
            if (returnText == null) return;
            target.Append("\r\nRETURN ");
            if (returnDistinct) target.Append("distinct ");
            target.Append(returnText);
        }

        void WriteLimitClause(StringBuilder target, IDictionary<string, object> paramsDictionary)
        {
            if (limit == null) return;
            target.AppendFormat("\r\nLIMIT {0}", CreateParameter(paramsDictionary, limit));
        }

        void WriteSkipClause(StringBuilder target, IDictionary<string, object> paramsDictionary)
        {
            if (skip == null) return;
            target.AppendFormat("\r\nSKIP {0}", CreateParameter(paramsDictionary, skip));
        }

        void WriteOrderByClause(StringBuilder target )
        {
            if (string.IsNullOrEmpty(orderBy)) return;
            target.AppendFormat("\r\nORDER BY {0}",  orderBy);
        }

        void WriteSetClause(StringBuilder target)
        {
            if (setText == null) return;
            target.AppendFormat("\r\nSET {0}", setText);
        }
    }

    internal class CypherCreateTextBit
    {
        readonly string createText;

        public CypherCreateTextBit(string createText)
        {
            this.createText = createText;
        }

        public string CreateText
        {
            get { return createText; }
        }
    }

    internal class CypherStartBitWithNodeIndexLookupWithSingleParameter
    {
        readonly string identifier;
        readonly string indexName;
        readonly string parameter;

        public CypherStartBitWithNodeIndexLookupWithSingleParameter(string identifier, string indexName, string parameter)
        {
            this.identifier = identifier;
            this.indexName = indexName;
            this.parameter = parameter;
        }

        public string Identifier { get { return identifier; } }
        public string IndexName { get { return indexName; } }
        public string Parameter { get { return parameter; } }
    }

    internal class CypherStartBitWithNodeIndexLookup
    {
        readonly string identifier;
        readonly string indexName;
        readonly string key;
        readonly object value;

        public CypherStartBitWithNodeIndexLookup(string identifier, string indexName, string key, object value)
        {
            this.identifier = identifier;
            this.indexName = indexName;
            this.key = key;
            this.value = value;
        }

        public string Identifier { get { return identifier; } }
        public string IndexName { get { return indexName; } }
        public string Key { get { return key; } }
        public object Value { get { return value; } }
    }


    internal class CypherStartBit
    {
        readonly string identifier;
        readonly string lookupType;
        readonly IEnumerable<long> lookupIds;

        public CypherStartBit(string identifier, string lookupType, IEnumerable<long> lookupIds)
        {
            this.identifier = identifier;
            this.lookupType = lookupType;
            this.lookupIds = lookupIds;
        }

        public string Identifier
        {
            get { return identifier; }
        }

        public string LookupType
        {
            get { return lookupType; }
        }

        public IEnumerable<long> LookupIds
        {
            get { return lookupIds; }
        }
    }

    internal class RawCypherStartBit
    {
        readonly string identifier;
        readonly string startText;

        public RawCypherStartBit(string identifier, string startText)
        {
            this.identifier = identifier;
            this.startText = startText;
        }

        public string Identifier
        {
            get { return identifier; }
        }

        public string StartText
        {
            get { return startText; }
        }
    }



    [DebuggerDisplay("{Query.DebugQueryText}")]
    public class CypherFluentQueryCreator :
        IAttachedReference
    {
        protected readonly IRawGraphClient Client;
        protected readonly CypherQueryCreator Builder;
        readonly IHttpClient httpClient;
        public Uri CypherQueryDatabaseUri { get; set; }

        public CypherFluentQueryCreator(IGraphClient client, Uri databaseUri)
            : this(client, new CypherQueryCreator(), databaseUri)
        {
            this.CypherQueryDatabaseUri = databaseUri;
            httpClient = GetNeo4jAuthenticatedClient(httpClient as HttpClient, this.CypherQueryDatabaseUri);
            
        }

        private static HttpClientWrapper GetNeo4jAuthenticatedClient(HttpClient httpClient, Uri databaseUri)
        {
            
            if (httpClient == null)
            {
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Convert.ToBase64String(Encoding.ASCII.GetBytes(CloudGraph.GetAuthorizationHeader())));
            }

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Convert.ToBase64String(Encoding.ASCII.GetBytes(CloudGraph.GetAuthorizationHeader())));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Remove(new System.Net.Http.Headers.StringWithQualityHeaderValue("UTF-8"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("UTF-8"));
            HttpClientWrapper clientWrapper = new HttpClientWrapper(httpClient);
            clientWrapper.Uri = databaseUri.ToString();
            
            return clientWrapper;
        }

        public CypherFluentQueryCreator(IGraphClient client, CypherQueryCreator builder, Uri databaseUri)
        {
            this.CypherQueryDatabaseUri = databaseUri;
            if (!(client is IRawGraphClient))
                throw new ArgumentException("The supplied graph client also needs to implement IRawGraphClient", "client");
            httpClient = new HttpClientWrapper();
            Client = (IRawGraphClient)client;
            Builder = builder;
        }

        public CypherFluentQueryCreator Start(string identity, string startText)
        {
            var newBuilder = new CypherQueryCreator();
            newBuilder.AddStartBit(identity, startText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Start(string identity, params NodeReference[] nodeReferences)
        {
            var newBuilder = new CypherQueryCreator();
            newBuilder.AddStartBit(identity, nodeReferences);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Start(string identity, params RelationshipReference[] relationshipReferences)
        {
            var newBuilder = new CypherQueryCreator();
            newBuilder.AddStartBit(identity, relationshipReferences);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator StartWithNodeIndexLookup(string identity, string indexName, string key, object value)
        {
            var newBuilder = new CypherQueryCreator();
            newBuilder.AddStartBitWithNodeIndexLookup(identity, indexName, key, value);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator StartWithNodeIndexLookup(string identity, string indexName, string parameter)
        {
            var newBuilder = new CypherQueryCreator();
            newBuilder.AddStartBitWithNodeIndexLookup(identity, indexName, parameter);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator AddStartPoint(string identity, string startText)
        {
            var newBuilder = Builder.AddStartBit(identity, startText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator AddStartPointWithNodeIndexLookup(string identity, string indexName, string key, object value)
        {
            var newBuilder = Builder.AddStartBitWithNodeIndexLookup(identity, indexName, key, value);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator AddStartPoint(string identity, params NodeReference[] nodeReferences)
        {
            var newBuilder = Builder.AddStartBit(identity, nodeReferences);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator AddStartPoint(string identity, params RelationshipReference[] relationshipReferences)
        {
            var newBuilder = Builder.AddStartBit(identity, relationshipReferences);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Match(params string[] matchText)
        {
            var newBuilder = Builder.SetMatchText(string.Join(", ", matchText));
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator With(string withText)
        {
            var newBuilder = Builder.SetWithText(withText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Where(string whereText)
        {
            var newBuilder = Builder.SetWhere(whereText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator WhereMatch(string whereMatchText)
        {
            var newBuilder = Builder.SetWhereMatch(whereMatchText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Limit(int? count)
        {
            var newBuilder = Builder.SetLimit(count);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator OrderBy(OrderByType orderByType, string orderByText)
        {
            var newBuilder = Builder.SetOrderBy(orderByType, orderByText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator And()
        {
            var newBuilder = Builder.SetWhere("and");
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Relate(string relateText)
        {
            if (Client.ServerVersion == new Version(1, 8) ||
                Client.ServerVersion >= new Version(1, 8, 0, 7))
                throw new NotSupportedException("You're trying to use the RELATE keyword against a Neo4j instance ≥ 1.8M07. In Neo4j 1.8M07, it was renamed from RELATE to CREATE UNIQUE. You need to update your code to use our new CreateUnique method. (We didn't want to just plumb the Relate method to CREATE UNIQUE, because that would introduce a deviation between the .NET wrapper and the Cypher language.)\r\n\r\nSee https://github.com/systay/community/commit/c7dbbb929abfef600266a20f065d760e7a1fff2e for detail.");

            var newBuilder = Builder.SetRelateText(relateText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator CreateUnique(string createUniqueText)
        {
            if (Client.ServerVersion < new Version(1, 8) ||
                (Client.ServerVersion >= new Version(1, 8, 0, 1) && Client.ServerVersion <= new Version(1, 8, 0, 6)))
                throw new NotSupportedException("The CREATE UNIQUE clause was only introduced in Neo4j 1.8M07, but you're querying against an older version of Neo4j. You'll want to upgrade Neo4j, or use the RELATE keyword instead. See https://github.com/systay/community/commit/c7dbbb929abfef600266a20f065d760e7a1fff2e for detail.");

            var newBuilder = Builder.SetCreateUniqueText(createUniqueText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Create(string createText)
        {
            var newBuilder = Builder.SetCreateText(createText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Return(string returnText)
        {
            var newBuilder = Builder.SetReturnText(returnText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Create<TNode>(string identity, TNode node)
            where TNode : class
        {
            if (typeof(TNode).IsGenericType &&
                 typeof(TNode).GetGenericTypeDefinition() == typeof(Node<>))
            {
                throw new ArgumentException(string.Format(
                    "You're trying to pass in a Node<{0}> instance. Just pass the {0} instance instead.",
                    typeof(TNode).GetGenericArguments()[0].Name),
                    "node");
            }

            if (node == null)
                throw new ArgumentNullException("node");

            var validationContext = new ValidationContext(node, null, null);
            Validator.ValidateObject(node, validationContext);

            var serializer = new CustomJsonSerializer { NullHandling = NullValueHandling.Ignore, QuoteName = false };
            var newBuilder = Builder.SetCreateText(string.Format("({0} {1})", identity, serializer.Serialize(node)));
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Delete(string identities)
        {
            var newBuilder = Builder.SetDeleteText(identities);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherFluentQueryCreator Set(string setText)
        {
            var newBuilder = Builder.SetSetText(setText);
            return new CypherFluentQueryCreator(Client, newBuilder, this.CypherQueryDatabaseUri);
        }

        public CypherQuery Query
        {
            get
            {
                if (!string.IsNullOrEmpty(Builder.queryText))
                {
                    return new CypherQuery(Builder.queryText, null, CypherResultMode.Projection);
                }
                else
                {
                    return Builder.ToQuery();
                }

            }
        }

        public void ExecuteWithoutResults()
        {
            Client.ExecuteCypher(Query);
        }

        IGraphClient IAttachedReference.Client
        {
            get { return Client; }
        }

        public async Task<IEnumerable<TResult>> ExecuteGetCypherResults<TResult>()
        {
            var task = await ExecuteGetCypherResultsAsync<TResult>(Query);
            return task;
        }

        public async Task<IEnumerable<TResult>> ExecuteGetCypherResults<TResult>(Dictionary<string, object> parameters)
        {
            CypherQuery query = new CypherQuery(Query.QueryText, parameters, CypherResultMode.Projection);
            var task = await ExecuteGetCypherResultsAsync<TResult>(query);
            return task;
        }

        internal RootApiResponse RootApiResponse = new RootApiResponse();

        public void ExecuteCustomQueryWithoutResults()
        {
            if (RootApiResponse.neo4j_version == null)
            {
                Connect();
            }

            var response = SendHttpRequestAsync(
                HttpPostAsJson(RootApiResponse.Cypher, new CypherApiQuery(Query)),
                string.Format("The query was: {0}", Query.QueryText),
                HttpStatusCode.OK);
        }

        public async Task<IEnumerable<TResult>> ExecuteGetCypherResultsAsync<TResult>(CypherQuery query)
        {
            //CheckRoot();

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            //await Connect();
            if (RootApiResponse.neo4j_version == null)
            {
                Connect();
            }

            var response = SendHttpRequestAsync(
                HttpPostAsJson(RootApiResponse.Cypher, new CypherApiQuery(query)),
                string.Format("The query was: {0}", query.QueryText),
                HttpStatusCode.OK);


            var results = await DeserializeNeo4jResponse<TResult>(response);

            //stopwatch.Stop();
            //OnOperationCompleted(new OperationCompletedEventArgs
            //{
            //    QueryText = query.QueryText,
            //    ResourcesReturned = results.Count(),
            //    TimeTaken = stopwatch.Elapsed
            //});

            return (IEnumerable<TResult>)results;
        }

        public async Task<List<TResult>> DeserializeNeo4jResponse<TResult>(Task<HttpResponseMessage> response)
        {
            var content = await response.Result.Content.ReadAsByteArrayAsync();
            string unicodeContent = Encoding.UTF8.GetString(content);

            var deserializer = new CypherJsonDeserializer<TResult>(Client, CypherResultMode.Projection);

            var results = deserializer
                .Deserialize(unicodeContent)
                .ToList();
            return results;
        }

        Uri RootUri;

        Uri BuildUri(string relativeUri)
        {
            RootUri = CypherQueryDatabaseUri;
            var baseUri = CypherQueryDatabaseUri;
            if (!RootUri.AbsoluteUri.EndsWith("/"))
                baseUri = new Uri(RootUri.AbsoluteUri + "/");

            if (relativeUri == null)
            {
                relativeUri = "";
            }

            if (relativeUri.StartsWith("/"))
                relativeUri = relativeUri.Substring(1);

            return new Uri(baseUri, relativeUri);
        }

        static CustomJsonSerializer BuildSerializer()
        {
            return new CustomJsonSerializer();
        }

        public HttpRequestMessage HttpPostAsJson(string relativeUri, object postBody)
        {
            var absoluteUri = BuildUri(relativeUri);
            var postBodyJson = BuildSerializer().Serialize(postBody);
            var request = new HttpRequestMessage(HttpMethod.Post, absoluteUri);
            request.Content = new StringContent(postBodyJson, Encoding.UTF8, "application/json");
            return request;
        }

        public  Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, params HttpStatusCode[] expectedStatusCodes)
        {
            return SendHttpRequestAsync(request, null, expectedStatusCodes);
        }

        public Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, string commandDescription, params HttpStatusCode[] expectedStatusCodes)
        {
            if (jsonStreamingAvailable)
            {
                request.Headers.Accept.Clear();
                request.Headers.Remove("Accept");
                request.Headers.Add("Accept", "application/json;stream=true");
                request.Headers.Remove("Accept-Encoding");
                request.Headers.Add("Accept-Encoding", "UTF-8");
                //
            }

            var assemblyVersion = GetType().Assembly.GetName().Version;
            var userAgent = string.Format("Neo4jClient/{0}", "1.0.0.498");
            request.Headers.Add("User-Agent", userAgent);

            //var httpRequest = HttpWebRequest.Create(request.RequestUri);
            //httpRequest.Headers.Remove("Accept");
            //httpRequest.Headers.Add("Accept", "application/json;stream=true");


            //request.Headers.Add("Authorization", Convert.ToBase64String(Encoding.ASCII.GetBytes("06533ff48:45733a7f7")));
            //httpClient.SendAsync(request).

            return httpClient.SendAsync(request);

            //return httpClient.SendAsync(request).ContinueWith(requestTask =>
            //{
            //    try
            //    {
            //        var response = requestTask.r;
            //        //response.EnsureSuccessStatusCode();
            //        if (response.StatusCode != HttpStatusCode.OK)
            //        {
            //            throw new Exception(response.ReasonPhrase + " " + " Payload: " + request.Content.ReadAsString().Result);
            //        }
            //        return response;
            //    }
            //    catch (Exception exr)
            //    {
            //        throw exr;
            //    }
            //});

            //.SendAsync(request);



            //var continuationTask = baseTask.ContinueWith(requestTask =>
            //{
            //var response = requestTask.Result;
            //response.EnsureSuccessStatusCode(); //EnsureExpectedStatusCode(commandDescription, expectedStatusCodes);
            //return response;
            //});
            //return continuationTask;
        }
        
        public event OperationCompletedEventHandler OperationCompleted;

        protected void OnOperationCompleted(OperationCompletedEventArgs args)
        {
            var eventInstance = OperationCompleted;
            if (eventInstance != null)
                eventInstance(this, args);
        }

        public async Task<T> SendHttpRequestAndParseResultAs<T>(HttpRequestMessage request, params HttpStatusCode[] expectedStatusCodes) where T : new()
        {
            return await SendHttpRequestAndParseResultAs<T>(request, null, expectedStatusCodes);
        }

        async Task<HttpResponseMessage> SendHttpRequest(HttpRequestMessage request, params HttpStatusCode[] expectedStatusCodes)
        {
            return await SendHttpRequest(request, null, expectedStatusCodes);
        }

        //Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, params HttpStatusCode[] expectedStatusCodes)
        //{
        //    return SendHttpRequestAsync(request, null, expectedStatusCodes);
        //}

        Task<HttpResponseMessage> SendHttpRequest(HttpRequestMessage request, string commandDescription, params HttpStatusCode[] expectedStatusCodes)
        {
            var task = SendHttpRequestAsync(request, commandDescription, expectedStatusCodes);
            return task;
        }

        //T SendHttpRequestAndParseResultAs<T>(HttpRequestMessage request, params HttpStatusCode[] expectedStatusCodes) where T : new()
        //{
        //    return SendHttpRequestAndParseResultAs<T>(request, null, expectedStatusCodes);
        //}

        async Task<T> SendHttpRequestAndParseResultAs<T>(HttpRequestMessage request, string commandDescription, params HttpStatusCode[] expectedStatusCodes) where T : new()
        {
            request.Headers.Remove("Accept-Encoding");
            request.Headers.Add("Accept-Encoding", "UTF-8");

            var response = SendHttpRequest(request, commandDescription, expectedStatusCodes).Result;

            return response.Content == null ? default(T) : await response.Content.ReadAsJson<T>();
        }

        //private void CheckRoot()
        //{
        //    Client.
        //}

        HttpRequestMessage HttpGet(string relativeUri)
        {
            var absoluteUri = BuildUri(relativeUri);
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, absoluteUri))
            {
                return new HttpRequestMessage(HttpMethod.Get, absoluteUri);
            }
            
        }

        bool jsonStreamingAvailable;

        public void Connect()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            HttpRequestMessage request = HttpGet("");

            RootApiResponse = SendHttpRequestAndParseResultAs<RootApiResponse>(request, HttpStatusCode.OK).Result;
            RootApiResponse.Batch = RootApiResponse.Batch.Substring(RootUri.AbsoluteUri.Length);
            RootApiResponse.Node = RootApiResponse.Node.Substring(RootUri.AbsoluteUri.Length);
            RootApiResponse.NodeIndex = RootApiResponse.NodeIndex.Substring(RootUri.AbsoluteUri.Length);
            RootApiResponse.RelationshipIndex = RootApiResponse.RelationshipIndex.Substring(RootUri.AbsoluteUri.Length);
            RootApiResponse.ExtensionsInfo = RootApiResponse.ExtensionsInfo.Substring(RootUri.AbsoluteUri.Length);
            if (RootApiResponse.Extensions != null && RootApiResponse.Extensions.GremlinPlugin != null)
            {
                RootApiResponse.Extensions.GremlinPlugin.ExecuteScript =
                    RootApiResponse.Extensions.GremlinPlugin.ExecuteScript.Substring(RootUri.AbsoluteUri.Length);
            }

            if (RootApiResponse.Cypher != null)
            {
                RootApiResponse.Cypher =
                    RootApiResponse.Cypher.Substring(RootUri.AbsoluteUri.Length);
            }

            //rootNode = string.IsNullOrEmpty(RootApiResponse.ReferenceNode)
            //    ? null
            //    : new RootNode(long.Parse(GetLastPathSegment(RootApiResponse.ReferenceNode)), this);

            // http://blog.neo4j.org/2012/04/streaming-rest-api-interview-with.html
            jsonStreamingAvailable = RootApiResponse.Version >= new Version(1, 8);

            stopwatch.Stop();
            OnOperationCompleted(new OperationCompletedEventArgs
            {
                QueryText = "Connect",
                ResourcesReturned = 0,
                TimeTaken = stopwatch.Elapsed
            });
        }

        public CypherFluentQueryCreator Or()
        {
            throw new NotImplementedException();
        }

        
    }

    public class HttpClientWrapper : IHttpClient
    {
        readonly HttpClient client;

        public string Uri { get { return client.BaseAddress.ToString(); } set { client.BaseAddress = new Uri(value); } }

        public HttpClientWrapper() : this(new HttpClient()) { }

        public HttpClientWrapper(HttpClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            if (request.RequestUri.ToString().Contains(CloudGraph.GetDatabaseUri()))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Convert.ToBase64String(Encoding.ASCII.GetBytes(CloudGraph.GetAuthorizationHeader())));
            }
            else
            {
                client.DefaultRequestHeaders.Authorization = null;
            }

            try
            {
                if (request.Method == HttpMethod.Post)
                {
                    return client.PostAsync(request.RequestUri, request.Content).ContinueWith((requestTask) =>
                        {
                            HttpResponseMessage response = requestTask.Result;
                            response.EnsureSuccessStatusCode();
                            return response;
                        });
                }
                else
                {
                    return client.SendAsync(request).ContinueWith((requestTask) =>
                    {
                        HttpResponseMessage response = requestTask.Result;
                        response.EnsureSuccessStatusCode();

                        return response;
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    //public class HttpClientWrapper : IHttpClient
    //{
    //    readonly HttpClient client;

    //    public HttpClientWrapper() : this(new HttpClient()) { }

    //    public HttpClientWrapper(HttpClient client)
    //    {
    //        this.client = client;
    //    }

    //    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    //    {
    //        try
    //        {
    //            //client.MaxResponseContentBufferSize = 256000;
    //            //client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
    //            //client.Timeout = new TimeSpan(0, 0, 30);
    //            client.CancelPendingRequests();
    //            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", Convert.ToBase64String(Encoding.ASCII.GetBytes("06533ff48:45733a7f7")));
                
    //            return client.SendAsync(request); //(request, HttpCompletionOption.ResponseHeadersRead);
    //        }
    //        catch (Exception)
    //        {

    //            throw;
    //        }
    //    }
    //}

    public class CypherApiQuery
    {
        readonly string queryText;
        readonly IDictionary<string, object> queryParameters;

        public CypherApiQuery(CypherQuery query)
        {
            queryParameters = new Dictionary<string, object>();
            queryText = query.QueryText;
            queryParameters = query.QueryParameters ?? new Dictionary<string, object>();
        }

        [JsonProperty("query")]
        public string Query
        {
            get { return queryText; }
        }

        [JsonProperty("params")]
        public IDictionary<string, object> Parameters
        {
            get { return queryParameters; }
        }
    }

    public static class HttpContentExtensions
    {
        public static async Task<string> ReadAsString(this HttpContent content)
        {
            var readTask = await content.ReadAsStringAsync();
            return readTask;
        }

        public static async Task<T> ReadAsJson<T>(this HttpContent content) where T : new()
        {
            var stringContent = await content.ReadAsString();
            return JsonConvert.DeserializeObject<T>(stringContent);
        }
    }

    public class RootApiResponse
    {
        [JsonProperty("cypher")]
        public string Cypher { get; set; }

        [JsonProperty("batch")]
        public string Batch { get; set; }

        [JsonProperty("node")]
        public string Node { get; set; }

        [JsonProperty("node_index")]
        public string NodeIndex { get; set; }

        [JsonProperty("relationship_index")]
        public string RelationshipIndex { get; set; }

        [JsonProperty("reference_node")]
        public string ReferenceNode { get; set; }

        [JsonProperty("extensions_info")]
        public string ExtensionsInfo { get; set; }

        [JsonProperty("extensions")]
        public ExtensionsApiResponse Extensions { get; set; }

        public string neo4j_version { get; set; }

        [JsonIgnore]
        public Version Version
        {
            get
            {
                if (string.IsNullOrEmpty(neo4j_version))
                    return new Version();

                switch (neo4j_version)
                {
                    case "1.8.RC1": return new Version(1, 8, 0, 8);
                }

                var numericalVersionString = Regex.Replace(
                    neo4j_version,
                    @"(?<major>\d*)[.](?<minor>\d*)[.]?M(?<build>\d*).*",
                    "${major}.${minor}.0.${build}");

                numericalVersionString = Regex.Replace(
                    numericalVersionString,
                    @"(?<major>\d*)[.](?<minor>\d*)-.*",
                    "${major}.${minor}");

                Version result;
                var parsed = Version.TryParse(numericalVersionString, out result);

                return parsed ? result : new Version(0, 0);
            }
        }

        public class ExtensionsApiResponse
        {
            public GremlinPluginApiResponse GremlinPlugin { get; set; }
        }

        public class GremlinPluginApiResponse
        {
            [JsonProperty("execute_script")]
            public string ExecuteScript { get; set; }
        }

        [JsonObject]
        public class BatchResponseExtensions
        {
        }

        [JsonObject]
        public class BatchResponseData
        {
            [JsonProperty]
            public string Key { get; set; }
            [JsonProperty]
            public string Content { get; set; }
            [JsonProperty]
            public string Weight { get; set; }
            [JsonProperty]
            public string EndPoint { get; set; }
        }

        [JsonObject]
        public class BatchResponseBody
        {
            [JsonProperty]
            public BatchResponseExtensions extensions { get; set; }
            [JsonProperty]
            public string paged_traverse { get; set; }
            [JsonProperty]
            public string outgoing_relationships { get; set; }
            [JsonProperty]
            public string all_typed_relationships { get; set; }
            [JsonProperty]
            public string traverse { get; set; }
            [JsonProperty]
            public string all_relationships { get; set; }
            [JsonProperty]
            public string property { get; set; }
            [JsonProperty]
            public string self { get; set; }
            [JsonProperty]
            public string outgoing_typed_relationships { get; set; }
            [JsonProperty]
            public string properties { get; set; }
            [JsonProperty]
            public string incoming_relationships { get; set; }
            [JsonProperty]
            public string incoming_typed_relationships { get; set; }
            [JsonProperty]
            public string create_relationship { get; set; }
            [JsonProperty]
            public BatchResponseData data { get; set; }
            [JsonProperty]
            public string start { get; set; }
            [JsonProperty]
            public string type { get; set; }
            [JsonProperty]
            public string end { get; set; }
        }

        [JsonObject]
        public class BatchResponseObject
        {
            [JsonProperty]
            public int id { get; set; }
            [JsonProperty]
            public string from { get; set; }
            [JsonProperty]
            public BatchResponseBody body { get; set; }
            [JsonProperty]
            public string location { get; set; }
            [JsonProperty]
            public int status { get; set; }
            [JsonProperty]
            public string message { get; set; }
        }

        
    }
}