using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo4j
{
    public class ProcessingNeo4j
    {
        static BoltGraphClient Client
        {
            get
            {
                BoltGraphClient client = new BoltGraphClient("neo4j+s://28831b13.databases.neo4j.io:7687", "neo4j", "_gObS56EP_e23-Qr-5gSDOx6JRKXyr6_aQ98eUspc-4");
                client.ConnectAsync().Wait();
                return client;
            }
        }
        private User currentUser;
        public void Authtentificate(string username, string pass)
        {
            var user = Client.Cypher
                .Match("(u:User { username: $un})")
                .WithParam("un", username)
                .Where("u.password= $pass")
                .WithParam("pass", pass)
                .Return(u => u.As<User>())
                .ResultsAsync.Result;
            currentUser = user.ElementAt(0);
        }
        public void CreateUser(string userName, string firstName, string surname, string password)
        {
            var newUser = new User
                (
                userName,
                firstName,
                surname,
                password
                );
            Client.Cypher
                .Create("(u:User $newUser)")
                .WithParam("newUser", newUser)
                .ExecuteWithoutResultsAsync().Wait();

        }
        public void CreateRelationshipUserSubscribed(string SubscribedName)
        {
            Client.Cypher
                .Match("(u:User{username:$un})", "(f:User{username: $fn})")
                .WithParam("un", currentUser.UserName)
                .WithParam("fn", SubscribedName)
                .Create("(u)-[:Subscribed]->(f)")
                .ExecuteWithoutResultsAsync().Wait();
        }
        public void DeleteRelationshipUserSubscribed(string SubscribedName)
        {
            Client.Cypher
                .Match("(u:User{username:$un})-[r:Subscribed]->(f:User{username: $fn})")
                .WithParam("un", currentUser.UserName)
                .WithParam("fn", SubscribedName)
                .Delete("r")
                .ExecuteWithoutResultsAsync().Wait();
        }
        public void DeleteUser(string userName)
        {
            DeleteAllRelationshipWithUser(userName);
            Client.Cypher
                .Match("(u:User {username: $deleteUser})")
                .WithParam("deleteUser", userName)
                .Delete("u")
                .ExecuteWithoutResultsAsync().Wait();

        }
        public void DeleteAllRelationshipWithUser(string userName)
        {
            Client.Cypher
                .Match("(u:User{username:$un})-[r]-(f:User)")
                .WithParam("un", currentUser.UserName)
                .Delete("r")
                .ExecuteWithoutResultsAsync().Wait();
        }
        public IEnumerable<Object> SearchRelationshipOfUser(string searchedUser)
        {
            var userWithSubscribed = Client.Cypher
                .Match("(u:User {username: $un})-[r]-> (f: User {username: $fn})")
                .WithParam("un", currentUser.UserName)
                .WithParam("fn", searchedUser)
                .Return((u, f) => new
                {
                    User = u.As<User>(),
                    Follower = f.As<User>()
                })
                .ResultsAsync.Result;
            return userWithSubscribed;
        }
        public double ShortestPathToSearchedUser(string searchedUserName)
        {
            var userWithSubscribed = Client.Cypher
                .Match("sp = shortestPath((:User {username: $un})-[*]-(:User {username: $fn}))")
                .WithParam("un", currentUser.UserName)
                .WithParam("fn", searchedUserName)
                .Return(sp => sp.Length())
                .ResultsAsync.Result;
            return userWithSubscribed.First();
        }
    }
}
    

