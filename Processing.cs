using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace social_network
{
    public class Processing
    {
        static string ConnectionString()
        {
            return "mongodb+srv://Yulia:yulia200220student@cluster0.aphvpx2.mongodb.net/test";
        }
        private User current_user;
        private IMongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<User> users;
        private IMongoCollection<Post> posts;
        public Processing()
        {
            client = new MongoClient(ConnectionString());
            database = client.GetDatabase("network");
            users = database.GetCollection<User>("users");
            posts = database.GetCollection<Post>("posts");
        }
        public bool Log_in(string username, string password)
        {
            var filterBuilder = Builders<User>.Filter;
            var filter = Builders<User>.Filter.Eq("username", username) & filterBuilder.Eq("password", password);
            var found = users.Find(filter).ToList();
            if (found.Count == 0)
            {
                return false;
            }
            current_user = found[0];
            return true;
        }
        public List<Post> Scroll_Posts()
        {
            var filter = Builders<Post>.Filter.In("username", current_user.Subscribed);
            var result_posts = posts.Find(filter).Sort("{date : -1}").ToList();
            return result_posts;
        }
        public List<Post> Scroll_Posts(string username)
        {
            var filter = Builders<Post>.Filter.Eq("username", username);
            var result_posts = posts.Find(filter).Sort("{date : -1}").ToList();
            return result_posts;
        }
        public List<User> GetSubscribed()
        {
            var filter = Builders<User>.Filter.In("username", current_user.Subscribed);
            var subscribed = users.Find(filter).ToList();
            return subscribed;
        }
        public void LikePost(Post post)
        {
            if (!post.Likes.Contains(current_user.UserName))
            {
                post.Likes.Add(current_user.UserName);
                Console.WriteLine("You liked this post");
                posts.ReplaceOne(p => p.Id == post.Id, post);
            }
            else
            {
                post.Likes.Remove(current_user.UserName);
                Console.WriteLine("You remove your like");
                posts.ReplaceOne(p => p.Id == post.Id, post);
            }
        }
        public void WriteComment(Post post, string comment)
        {
            post.Comments.Add(new Comment { UserName = current_user.UserName, CommentText = comment, CreationDate = DateTime.Now });
            posts.ReplaceOne(p => p.Id == post.Id, post);
        }
        public bool UnSubscribe(string username)
        {
            bool result = current_user.Subscribed.Remove(username);
            if (result)
            {
                users.ReplaceOne(u => u.Id == current_user.Id, current_user);
            }
            return result;
        }
        public bool GetSubscribers(string username)
        {
            return current_user.Subscribed.Contains(username);
        }
        public User FindUser(string username)
        {
            var filter = Builders<User>.Filter.Eq("username", username);
            var user_s = users.Find(filter).ToList();
            if (user_s.Count == 1)
            {
                return user_s[0];
            }
            return null;
        }
        public bool IsSubscribed(User user)
        {
            return current_user.Subscribed.Contains(user.UserName);
        }
        public void Subscribe(string username)
        {
            current_user.Subscribed.Add(username);
            users.ReplaceOne(u => u.Id == current_user.Id, current_user);
        }
        public void CreateUser(string userName, string firstName, string lastName, string password, List<string> follows)
        {
            var newUser = new User
            {
                UserName = userName,
                FirstName = firstName,
                Surname = lastName,
                Password = password,
                Subscribed = follows
            };

            users.InsertOne(newUser);
        }

        public void DeleteUser(string userName)
        {
            users.DeleteOne(p => p.UserName == userName);
        }
        public string DrawInConsoleBox( string s)
        {
            string ulCorner = "╔";
            string llCorner = "╚";
            string urCorner = "╗";
            string lrCorner = "╝";
            string vertical = "║";
            string horizontal = "═";
            string[] lines = s.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int longest = 0;
            foreach (string line in lines)
            {
                if (line.Length > longest)
                    longest = line.Length;
            }
            int width = longest + 2; 
            string h = string.Empty;
            for (int i = 0; i < width; i++)
                h += horizontal;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ulCorner + h + urCorner);

            foreach (string line in lines)
            {
                double dblSpaces = (((double)width - (double)line.Length) / (double)2);
                int iSpaces = Convert.ToInt32(dblSpaces);

                if (dblSpaces > iSpaces) 
                {
                    iSpaces += 1; 
                }

                string beginSpacing = "";
                string endSpacing = "";
                for (int i = 0; i < iSpaces; i++)
                {
                    beginSpacing += " ";

                    if (!(iSpaces > dblSpaces && i == iSpaces - 1)) 
                    {
                        endSpacing += " ";
                    }
                }
                sb.AppendLine(vertical + beginSpacing + line + endSpacing + vertical);
            }
            sb.AppendLine(llCorner + h + lrCorner);
            return sb.ToString();
        }
   
        public string Title()
        {
            return @"
 _____            _       _              _                      _      _ _  ___       _       _ _ _ 
/  ___|          (_)     | |            | |                    | |    ( | )|_  |     (_)     | ( | )
\ `--.  ___   ___ _  __ _| |  _ __   ___| |___      _____  _ __| | __  V V   | | ___  _ _ __ | |V V 
 `--. \/ _ \ / __| |/ _` | | | '_ \ / _ \ __\ \ /\ / / _ \| '__| |/ /        | |/ _ \| | '_ \| |    
/\__/ / (_) | (__| | (_| | | | | | |  __/ |_ \ V  V / (_) | |  |   <     /\__/ / (_) | | | | |_|    
\____/ \___/ \___|_|\__,_|_| |_| |_|\___|\__| \_/\_/ \___/|_|  |_|\_\    \____/ \___/|_|_| |_(_)    
                                                                                                    
                                                                                                    
";
        }
    }
}
