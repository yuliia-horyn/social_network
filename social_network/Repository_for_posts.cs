using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

namespace social_network
{
    public class Repository_for_posts
    {
        private IMongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<Post> posts;
        public Repository_for_posts(string connectionString)
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("network");
            posts = database.GetCollection<Post>("posts");
        }
        public void InsertPost(Post post)
        {
            posts.InsertOne(post);
        }
        public List<Post> GetAllPosts()
        {
            return posts.Find(new BsonDocument()).ToList();
        }
        public List<Post> GetPostsByProp(string prop_name, string prop_value)
        {
            var filter = Builders<Post>.Filter.Eq(prop_name, prop_value);
            var result = posts.Find(filter).ToList();
            return result;
        }
        public List<Post> GetPosts(int current_post, int counter)
        {
            var result = posts.Find(new BsonDocument()).Skip(current_post) .Limit(counter).ToList();
            return result;
        }
        public bool UpdatePost(ObjectId id, string update_prop_name, string update_prop_value)
        {
            var filter = Builders<Post>.Filter.Eq("_id", id);
            var update = Builders<Post>.Update.Set(update_prop_name, update_prop_value);
            var result = posts.UpdateOne(filter, update);
            return result.ModifiedCount != 0;
        }
    }
}
