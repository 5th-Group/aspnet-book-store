using MongoDB.Driver;

namespace BookStoreMVC.Infastructure;

public static class MongoDbExtension 
{
    public static async Task<TDocument> FetchDBRefAsAsync<TDocument>(this MongoDBRef dbRef, IMongoDatabase database)
    {
        var collection = database.GetCollection<TDocument>(dbRef.CollectionName);

        var query = Builders<TDocument>.Filter.Eq("_id", dbRef.Id);
        return await (await collection.FindAsync(query)).FirstOrDefaultAsync();
    }
}