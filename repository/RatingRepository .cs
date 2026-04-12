using Entities;
using Microsoft.Data.SqlClient; 
using Microsoft.Extensions.Configuration;
using Repository.Models;
using System.Data;

namespace Repository
{
    public class RatingRepository : IRatingRepository
    {
        private readonly IConfiguration _configuration;
        public RatingRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Rating> AddRating(Rating newRating)
        {
            string connectionString = _configuration.GetConnectionString("Tehila");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
        
                string query = @"INSERT INTO RATING (HOST, METHOD, [PATH], REFERER, USER_AGENT, Record_Date) 
                                VALUES (@Host, @Method, @Path, @Referer, @UserAgent, @Date)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Host", newRating.Host ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Method", newRating.Method ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Path", newRating.Path ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Referer", newRating.Referer ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserAgent", newRating.UserAgent ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", newRating.RecordDate);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return newRating;
        }
    }
}