using Microsoft.Data.SqlClient;
using YemekSepeti.Models;

namespace YemekSepeti.Data
{
    public class RestaurantRepository
    {
        private readonly string _connectionString;

        public RestaurantRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new Exception("Connection string missing");
        }

        // ---------------- GET ALL ACTIVE RESTAURANTS ----------------
        public List<Restaurant> GetAll()
        {
            var list = new List<Restaurant>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"SELECT RestaurantID, Name, Description, Address, ImageUrl, IsActive
                           FROM Restaurants
                           WHERE IsActive = 1
                           ORDER BY Name ASC";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Restaurant
                {
                    RestaurantID = (int)reader["RestaurantID"],
                    Name = reader["Name"].ToString() ?? "",
                    Description = reader["Description"].ToString() ?? "",
                    Address = reader["Address"].ToString() ?? "",
                    ImageUrl = reader["ImageUrl"].ToString() ?? "",
                    IsActive = (bool)reader["IsActive"]
                });
            }

            return list;
        }

        // ---------------- GET RESTAURANT BY ID ----------------
        public Restaurant? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"SELECT RestaurantID, Name, Description, Address, ImageUrl, IsActive
                           FROM Restaurants
                           WHERE RestaurantID = @ID";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ID", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Restaurant
                {
                    RestaurantID = (int)reader["RestaurantID"],
                    Name = reader["Name"].ToString() ?? "",
                    Description = reader["Description"].ToString() ?? "",
                    Address = reader["Address"].ToString() ?? "",
                    ImageUrl = reader["ImageUrl"].ToString() ?? "",
                    IsActive = (bool)reader["IsActive"]
                };
            }

            return null;
        }

        // ---------------- SEARCH RESTAURANTS ----------------
        public List<Restaurant> Search(string q)
        {
            var list = new List<Restaurant>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                SELECT RestaurantID, Name, Description, Address, ImageUrl, IsActive
                FROM Restaurants
                WHERE IsActive = 1
                AND (Name LIKE @q OR Description LIKE @q)
                ORDER BY Name ASC";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@q", "%" + q + "%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Restaurant
                {
                    RestaurantID = (int)reader["RestaurantID"],
                    Name = reader["Name"].ToString() ?? "",
                    Description = reader["Description"].ToString() ?? "",
                    Address = reader["Address"].ToString() ?? "",
                    ImageUrl = reader["ImageUrl"].ToString() ?? "",
                    IsActive = (bool)reader["IsActive"]
                });
            }

            return list;
        }

        // ---------------- ADD RESTAURANT ----------------
        public void Add(Restaurant r)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"INSERT INTO Restaurants (Name, Description, Address, ImageUrl, IsActive)
                           VALUES (@Name, @Desc, @Addr, @Img, 1)";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", r.Name);
            cmd.Parameters.AddWithValue("@Desc", r.Description);
            cmd.Parameters.AddWithValue("@Addr", r.Address);
            cmd.Parameters.AddWithValue("@Img", r.ImageUrl ?? "");

            cmd.ExecuteNonQuery();
        }

        // ---------------- UPDATE RESTAURANT ----------------
        public void Update(Restaurant r)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"UPDATE Restaurants
                           SET Name = @Name,
                               Description = @Desc,
                               Address = @Addr,
                               ImageUrl = @Img
                           WHERE RestaurantID = @ID";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ID", r.RestaurantID);
            cmd.Parameters.AddWithValue("@Name", r.Name);
            cmd.Parameters.AddWithValue("@Desc", r.Description);
            cmd.Parameters.AddWithValue("@Addr", r.Address);
            cmd.Parameters.AddWithValue("@Img", r.ImageUrl ?? "");

            cmd.ExecuteNonQuery();
        }

        // ---------------- SOFT DELETE RESTAURANT ----------------
        public void Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"UPDATE Restaurants
                           SET IsActive = 0
                           WHERE RestaurantID = @ID";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ID", id);

            cmd.ExecuteNonQuery();
        }
    }
}
