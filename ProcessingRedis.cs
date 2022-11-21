using System;
using System.Collections.Generic;
using System.Linq;

namespace Redis
{
    class ProcessingRedis
    {
        public static void AddComment(Comment comment)
        {
            Processing processing = new(Helper.CnnVal());
            processing.InsertRecord("comments", comment);
        }

        public static void AddFriend(User user, User friend)
        {
            Processing processing = new(Helper.CnnVal());
            ProcessingNeo4j processing2 = new ProcessingNeo4j();
            if (processing.GetAllRecords<User>("users")
                    .FirstOrDefault(u => u.id == user.id && u.friends_id.All(el => el != friend.id)) is not null)
            {
                processing.AddFriend(user.id, friend.id);
                processing2.AddFriend(user.id, friend.id);
            }
        }

        public static void AddLike(Post post, User user)
        {
            Processing processing = new(Helper.CnnVal());
            if (processing.GetAllRecords<Post>("posts")
                    .FirstOrDefault(p => p.id == post.id && p.liked_by.All(u_id => u_id != user.id)) is not null)
                processing.AddLikeToPost(post.id, user.id);
        }

        public static void AddLike(Comment comment, User user)
        {
            Processing processing = new(Helper.CnnVal());
            if (processing.GetAllRecords<Comment>("comments")
                    .FirstOrDefault(c => c.id == comment.id && c.liked_by.All(u_id => u_id != user.id)) is not null)
                processing.AddLikeToComment(comment.id, user.id);
        }

        public static void AddPost(Post post)
        {
            Processing processing = new(Helper.CnnVal());
            processing.InsertRecord("posts", post);
        }

        public static bool CheckPassword(string login, string password)
        {
            Processing processing = new(Helper.CnnVal());
            var res = processing.GetAllRecords<User>("users").Where(u => u.login == login && u.password == password)
                .FirstOrDefault();
            return res != null;
        }

        public static List<Comment> GetAllComments()
        {
            Processing processing = new(Helper.CnnVal());
            var res = processing.GetAllRecords<Comment>("comments");
            return res;
        }

        public static List<Post> GetAllPosts()
        {
            Processing processing = new(Helper.CnnVal());
            var res = processing.GetAllRecords<Post>("posts");
            return res;
        }

        public static List<User> GetAllUsers()
        {
            string recordKey = $"Users_" + DateTime.Now.ToString("yyyyMMdd_hh");
            var res = CRUDRedis.GetRecord<List<User>>(recordKey);
            if (res == default(List<User>))
            {
                Processing processing = new(Helper.CnnVal());
                var mongoData = processing.GetAllRecords<User>("users");
                CRUDRedis.SetRecord(recordKey, mongoData);
                return mongoData;
            }
            return res;
        }

        public static User GetUserByID(Guid id)
        {
            Processing processing = new(Helper.CnnVal());
            var res = processing.GetAllRecords<User>("users").Where(u => u.id == id).FirstOrDefault();
            return res;
        }

        public static User GetUserByLogin(string login)
        {
            Processing processing = new(Helper.CnnVal());
            var res = processing.GetAllRecords<User>("users").Where(u => u.login == login).FirstOrDefault();
            return res;
        }

        public static void RemoveFriend(User user, User friend)
        {
            Processing processing = new(Helper.CnnVal());
            ProcessingNeo4j ndb = new ProcessingNeo4j();
            if (processing.GetAllRecords<User>("users")
                    .FirstOrDefault(u => u.id == user.id && u.friends_id.Any(el => el == friend.id)) is not null)
            {
                processing.RemoveFriend(user.id, friend.id);
                ndb.RemoveFriend(user.id, friend.id);
            }

        }
        public static void RemoveLike(Post post, User user)
        {
            Processing processing = new(Helper.CnnVal());
            if (processing.GetAllRecords<Post>("posts")
                    .FirstOrDefault(p => p.id == post.id && p.liked_by.Any(u_id => u_id == user.id)) is not null)
                processing.RemoveLikeFromPost(post.id, user.id);
        }

        public static void RemoveLike(Comment comment, User user)
        {
            Processing processing = new(Helper.CnnVal());
            if (processing.GetAllRecords<Comment>("comments")
                    .FirstOrDefault(c => c.id == comment.id && c.liked_by.Any(u_id => u_id == user.id)) is not null)
                processing.RemoveLikeFromComment(comment.id, user.id);
        }

        public static int GetDistanceToUser(Guid userFromId, Guid userToId)
        {
            ProcessingNeo4j ndb = new ProcessingNeo4j();
            return ndb.GetDistance(userFromId, userToId);
        }
    }
}
