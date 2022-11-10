using MongoDB.Driver;

namespace BookStoreMVC.DataAccess;

public class BookStoreDataAccess
{
        private const string ConnectionString =  "mongodb+srv://admin:090327298@cluster0.j8vge.mongodb.net/?retryWrites=true&w=majority";
        private const string DatabaseName = "BookStore";
        private const string BookCollectionName = "Books";
        private const string AuthorCollectionName = "Authors";
        private const string BookGenreCollectionName = "BookGenres";
        private const string BookTypeCollectionName = "BookTypes";
        private const string CountryCollectionName = "Countries";
        private const string LanguageCollectionName = "Languages";
        private const string OrderCollectionName = "Orders";
        private const string ProductCollectionName = "Products";
        private const string PublisherCollectionName = "Publishers";
        private const string ReviewCollectionName = "Reviews";

        public IMongoCollection<T> ConnectToMongo<T>(in string collection)
        {
                var client = new MongoClient(connectionString: ConnectionString);
                var db = client.GetDatabase(DatabaseName);
                return db.GetCollection<T>(collection);
        }
}