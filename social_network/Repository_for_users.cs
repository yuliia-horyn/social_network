/*using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

namespace social_network
{
     public class Repository_for_users
     {
        private IMongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<User> users;
        public Repository_for_users(string connectionString)
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("network");
            users = database.GetCollection<User>("users");
        }
        public void InsertUser(User user)
        {
            users.InsertOne(user);
        }
        public List<User> GetAllUsers()
        {
            return users.Find(new BsonDocument()).ToList();
        }
        public List<User> GetUsersByProp(string prop_name, string prop_value)
        {
            var filter = Builders<User>.Filter.Eq(prop_name, prop_value);
            var result = users.Find(filter).ToList();
            return result;
        }
        public List<User> GetUsers(int current_user, int counter)
        {
            var result = users.Find(new BsonDocument()).Skip(current_user).Limit(counter).ToList();
            return result;
        }
        public bool UpdateUser(ObjectId id, string update_prop_name, string update_prop_value)
        {
            var filter = Builders<User>.Filter.Eq("_id", id);
            var update = Builders<User>.Update.Set(update_prop_name, update_prop_value);
            var result = users.UpdateOne(filter, update);
            return result.ModifiedCount != 0;
        }
     }
}
*/