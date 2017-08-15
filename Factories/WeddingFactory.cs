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
                    System.Console.WriteLine(users.Count());
                    wedding.WedderOne = users[0];
                    wedding.WedderTwo = users[1];
                }
                return weddings;
            }
        }
    }
}