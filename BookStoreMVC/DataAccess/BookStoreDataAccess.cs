using System.Diagnostics;
using MongoDB.Driver;

namespace BookStoreMVC.DataAccess;

public class BookStoreDataAccess
{
        // private const string ConnectionString =  "mongodb+srv://admin:090327298@cluster0.j8vge.mongodb.net/?retryWrites=true&w=majority";
        // private const string DatabaseName = "BookStore";
        // private const string BookCollectionName = "Books";
        // private const string AuthorCollectionName = "Authors";
        // private const string BookGenreCollectionName = "BookGenres";
        // private const string BookTypeCollectionName = "BookTypes";
        // private const string CountryCollectionName = "Countries";
        // private const string LanguageCollectionName = "Languages";
        // private const string OrderCollectionName = "Orders";
        // private const string ProductCollectionName = "Products";
        // private const string PublisherCollectionName = "Publishers";
        // private const string ReviewCollectionName = "Reviews";

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