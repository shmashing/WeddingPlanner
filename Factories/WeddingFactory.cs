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
    public class WeddingFactory : IFactory<Wedding>
    {
        private readonly IOptions<MySqlOptions> MySqlConfig;
        public WeddingFactory(IOptions<MySqlOptions> config)
        {
            MySqlConfig = config;
        }
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(MySqlConfig.Value.ConnectionString);
            }
        }
        public void AddWedding(Wedding wedding){
            using (IDbConnection dbConnection = Connection){
                string query = "INSERT INTO weddings (WedderOneId, WedderTwoId, Date, Address)" +
                                "VALUES (@WedderOneId, @WedderTwoId, @Date, @Address)";

                dbConnection.Open();
                dbConnection.Execute(query, wedding);

                var updated_wed = dbConnection.Query<Wedding>("SELECT * FROM weddings ORDER BY Id DESC LIMIT 1").SingleOrDefault();
                string query2 = $"UPDATE users SET WeddingId = {updated_wed.Id} WHERE Id = {wedding.WedderOneId}";
                dbConnection.Execute(query2);

                string query3 = $"UPDATE users SET WeddingId = {updated_wed.Id} WHERE Id = {wedding.WedderTwoId}";
                dbConnection.Execute(query3);
            }
        }
        public IEnumerable<Wedding> GetAllWeddings(){
            using (IDbConnection dbConnection = Connection){
                string query = "SELECT * FROM weddings"; 
                dbConnection.Open();
                var weddings = dbConnection.Query<Wedding>(query);

                foreach (var wedding in weddings){
                    query = $"SELECT * FROM users WHERE users.WeddingId = {wedding.Id}";
                    var users = dbConnection.Query<User>(query).ToList();
                    wedding.WedderOne = users[0];
                    wedding.WedderTwo = users[1];

                    query = "SELECT * FROM guestlists JOIN users ON guestlists.UserId = users.Id " +
                            $"WHERE guestlists.WeddingId = {wedding.Id}";

                    wedding.Guests = dbConnection.Query<User>(query).ToList();
                }
                return weddings;
            }
        }
        public Wedding GetWeddingById(int id){
            using (IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM weddings WHERE (Id = {id})";
                dbConnection.Open();
                Wedding wedding = dbConnection.Query<Wedding>(query).SingleOrDefault();

                query = $"SELECT * FROM users WHERE (WeddingId = {id})";
                var users = dbConnection.Query<User>(query).ToList();
                wedding.WedderOne = users[0];
                wedding.WedderTwo = users[1];

                query = "SELECT * FROM guestlists JOIN users ON guestlists.UserId = users.Id " +
                       $"WHERE guestlists.WeddingId = {wedding.Id}";

                wedding.Guests = dbConnection.Query<User>(query).ToList();
                return wedding;
            }
        }
        public void DeleteWedding(Wedding wedding){
            using (IDbConnection dbConnection = Connection){
                string query1 = $"UPDATE users SET WeddingId = NULL WHERE (Id = {wedding.WedderOneId} || Id = {wedding.WedderTwoId})";
                string query2 = $"DELETE FROM weddings WHERE (Id = {wedding.Id})";
                dbConnection.Open();
                dbConnection.Execute(query1);
                dbConnection.Execute(query2);
            }
        }
        public void AddGuestToWedding(int weddingId, int guestId){
            using (IDbConnection dbConnection = Connection){
                string query = $"INSERT INTO guestlists (UserId, WeddingId) VALUES ({guestId},{weddingId})";
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        }
        public void RemoveGuestFromWedding(int weddingId, int guestId){
            using (IDbConnection dbConnection = Connection){
                string query = $"DELETE FROM guestlists WHERE (WeddingId = {weddingId} && UserId = {guestId})";
                dbConnection.Open();
                dbConnection.Execute(query);
            }
        }
    }
}