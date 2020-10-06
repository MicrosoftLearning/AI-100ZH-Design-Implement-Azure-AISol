using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageStorageLibrary
{
    /// <summary>
    /// Helper for accessing CosmosDB. Set the endpoint URI and Access Key (find these in the portal), your DB and Collection names, and then build your instance.
    /// </summary>
    public class CosmosDBHelper<T> where T : class, new()
    {

        private static CosmosDBHelper<T> singletone { get; set; }

        private CosmosDBHelper()
        {
        }

        public static CosmosDBHelper<T> GetInstance
        {
            get
            {
                if (singletone == null)
                {
                    if (String.IsNullOrEmpty(EndpointUri)) throw new ArgumentException("You must init 'EndpointUri' first");
                    if (String.IsNullOrEmpty(AccessKey)) throw new ArgumentException("You must init 'AccessKey' first");
                    if (String.IsNullOrEmpty(DatabaseName)) throw new ArgumentException("You must init 'DatabaseName' first");
                    if (String.IsNullOrEmpty(CollectionName)) throw new ArgumentException("You must init 'CollectionName' first");

                    singletone = new CosmosDBHelper<T>();
                    singletone.Client = BuildAsync().Result;
                }

                return singletone;
            }
        }

        public static string EndpointUri { get; set; }
        public static string AccessKey { get; set; }
        public static string DatabaseName { get; set; }
        public static string CollectionName { get; set; }

        private static async Task<CosmosDbService<T>> BuildAsync()
        {
            if (string.IsNullOrWhiteSpace(EndpointUri))
                throw new ArgumentNullException("EndpointUri");
            if (string.IsNullOrWhiteSpace(AccessKey))
                throw new ArgumentNullException("AccessKey");
            if (string.IsNullOrWhiteSpace(DatabaseName))
                throw new ArgumentNullException("DatabaseName");
            if (string.IsNullOrWhiteSpace(CollectionName))
                throw new ArgumentNullException("CollectionName");

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(EndpointUri, AccessKey);
            CosmosClient client = clientBuilder
                                .WithConnectionModeDirect()
                                .Build();


            CosmosDbService<T> cosmosDbService = new CosmosDbService<T>(client, DatabaseName, CollectionName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(DatabaseName);
            await database.Database.CreateContainerIfNotExistsAsync(CollectionName, "/id");

            return cosmosDbService;
        }



        private CosmosDbService<T> Client { get; set; }
        //private Database Database { get; set; }
        //private DocumentCollection Collection { get; set; }

        /// <summary>
        /// Create a document with the given ID in the DB/Collection, unless it already exists. If it exists, return the existing version.
        /// </summary>
        /// <typeparam name="T">Type of the document.</typeparam>
        /// <param name="document">Document to create.</param>
        /// <param name="id">ID for the created document, used in Document URI so must be valid DocuemntDB ID.</param>
        /// <returns>Tuple with whether document was created or not, and either created or existing document.</returns>
        public async Task<Tuple<bool, T>> CreateDocumentIfNotExistsAsync<T>(T document, string id)
            where T : new()
        {
                await this.Client.AddItemAsync(document);
                return Tuple.Create(false, document);           
        }

        /// <summary>
        /// Update/replace the existing document with the given ID.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="update">Document to update</param>
        /// <param name="id">ID for the updated document, used in Cosmos URI so must be valid Cosmos DB ID.</param>
        /// <returns>Updated document.</returns>
        public async Task<T> UpdateDocumentAsync<T>(T update, string id)
            where T : new()
        {
            await this.Client.UpdateItemAsync(id, update);
            return update;
        }

        /// <summary>
        /// Find all documents in the collection.
        /// </summary>
        /// <typeparam name="T">Type of documents to find.</typeparam>
        /// <returns>Queryable capable of returning all documents.</returns>
        public IEnumerable<T> FindAllDocuments<T>()
            where T : new()
        {
            return null;
           // return Client.GetItemsAsync("SELECT * FROM c", "MaxItemCount", "-1").Result;
        }

        /// <summary>
        /// Find all documents matching the given query in the collection.
        /// </summary>
        /// <typeparam name="T">Type of documents to find.</typeparam>
        /// <param name="query">Query against the document store.</param>
        /// <returns>Queryable capable of returning all matching documents.</returns>
        public IEnumerable<T> FindMatchingDocuments<T>(string query)
            where T : new()
        {
            return (IEnumerable<T>)Client.GetItemsAsync(query).Result;
        }

        /// <summary>
        /// Simple "find by ID" query.
        /// </summary>
        /// <typeparam name="T">Type of document to find.</typeparam>
        /// <param name="id">ID of the document, will be used to look up by CosmosDB URI.</param>
        /// <returns>Found document, or null (assuming T is nullable).</returns>
        public async Task<T> FindDocumentByIdAsync<T>(string id)
            where T : new()
        {
            try
            {                
                return await Client.GetItemAsync<T>(id);
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound) return default(T);
                throw;
            }
        }


    }
}
