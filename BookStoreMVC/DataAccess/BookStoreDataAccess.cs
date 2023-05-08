using System.Diagnostics;
using MongoDB.Driver;

namespace BookStoreMVC.DataAccess;

public class BookStoreDataAccess
{


        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string BookCollectionName { get; set; }
        public string AuthorCollectionName { get; set; }
        public string BookGenreCollectionName { get; set; }
        public string BookTypeCollectionName { get; set; }
        public string CountryCollectionName { get; set; }
        public string LanguageCollectionName { get; set; }
        public string OrderCollectionName { get; set; }
        public string ProductCollectionName { get; set; }
        public string PublisherCollectionName { get; set; }
        public string ReviewCollectionName { get; set; }

        // public IMongoCollection<T> ConnectToMongo<T>(in string collection)
        // {
        //         var client = new MongoClient(ConnectionString);
        //         var db = client.GetDatabase(DatabaseName);
        //         return db.GetCollection<T>(collection);
        // }
}
