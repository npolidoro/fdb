using FDB_BOT.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using todo.Models;

namespace FDB_BOT
{
    public class InsertDB
    {
        private string EndpointUrl = ConfigurationManager.AppSettings["endpoint"];
        private string PrimaryKey = ConfigurationManager.AppSettings["authKey"];
        private DocumentClient client;
        private string databaseName = ConfigurationManager.AppSettings["database"];
        private string collectionName=ConfigurationManager.AppSettings["collection"];


        public async void CreateInsert(Insert insert) {

            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            Item item = new Item();
            item.insert = insert;

            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName});
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseName),
                new DocumentCollection { Id = collectionName });

            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), item);

        }

        public List<Insert> GetInsertList() {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            // Here we find the Andersen family via its LastName
            IQueryable<Item> insertQuery = this.client.CreateDocumentQuery<Item>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                    .Where(f => f.type == "INSERT");

            List<Insert> list = new List<Insert>();
            foreach (var item in insertQuery)
            {
                list.Add(item.insert);
            }
            return list;
        }

    }
}