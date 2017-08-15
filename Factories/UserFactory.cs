using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Options;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
 
namespace WeddingPlanner.Factory
{
    public class UserFactory : IFactory<User>
    {
        private readonly IOptions<MySqlOptions> MySqlConfig;
        public UserFactory(IOptions<MySqlOptions> config)
        {
            MySqlConfig = config;

        }
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(MySqlConfig.Value.ConnectionString);
            }
        }
        public void AddUser(User user){
            using (IDbConnection dbConnection = Connection){
                string query = "INSERT INTO users (FirstName, LastName, Email, Password)" +
                                @"VALUES (@FirstName, @LastName, @Email, @Password)";

                dbConnection.Open();
                dbConnection.Execute(query, user);
            }
        }
        public bool ValidateUser(string email, string password){
            using(IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM users WHERE (Email = '{email}')";
                dbConnection.Open();
                User user = dbConnection.Query<User>(query).SingleOrDefault();
                if(user != null && password != null){
                    var Hasher = new PasswordHasher<User>();
                    if(Hasher.VerifyHashedPassword(user, user.Password, password) != 0){
                        return true;
                    }
                }
                return false;
            }
        }
        public User GetUserByEmail(string email){
            using(IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM users WHERE (Email = '{email}')";
                dbConnection.Open();
                User user = dbConnection.Query<User>(query).SingleOrDefault();
                return user;
            }
        }
        public User GetUserById(int id){
            using(IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM users WHERE (Id = {id})";
                dbConnection.Open();
                User user = dbConnection.Query<User>(query).SingleOrDefault();
                return user;
            }
        }
        public IEnumerable<User> GetUnwedUsers(int id){
            using (IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM users WHERE (users.Id != {id} && users.WeddingId IS NULL)";
                dbConnection.Open();
                var users = dbConnection.Query<User>(query).ToList();
                return users;
            }
        }
        public IEnumerable<int> GetRSVPsForId(int id){
            using(IDbConnection dbConnection = Connection){
                string query = $"SELECT WeddingId FROM guestlists WHERE (UserId = {id})";
                dbConnection.Open();
                List<int> ids = dbConnection.Query<int>(query).ToList();
                foreach (int eyedee in ids){
                    System.Console.WriteLine(eyedee);
                }
                return ids;

            }
        }
    }
}