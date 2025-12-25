using Microsoft.Data.SqlClient;
using YemekSepeti.Models;

namespace YemekSepeti.Data
{
    public class MenuItemRepository
    {
        private readonly string? _connectionString;

        public MenuItemRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

public List<MenuItem> GetByRestaurant(int restaurantId)
{
    var list = new List<MenuItem>();

    using var conn = new SqlConnection(_connectionString);
    conn.Open();

    string sql = @"SELECT MenuItemID, RestaurantID, Name, Description, Price, ImageUrl, Category, Rating, IsActive
                   FROM MenuItems
                   WHERE RestaurantID = @r AND IsActive = 1";

    using var cmd = new SqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@r", restaurantId);

    using var r = cmd.ExecuteReader();
    while (r.Read())
    {
        list.Add(new MenuItem
        {
            MenuItemID = (int)r["MenuItemID"],
            RestaurantID = (int)r["RestaurantID"],
            Name = r["Name"].ToString() ?? "",
            Description = r["Description"] == DBNull.Value ? "" : r["Description"].ToString(),
            Price = Convert.ToDecimal(r["Price"]),
            ImageUrl = r["ImageUrl"] == DBNull.Value ? "/images/no-food.png" : r["ImageUrl"].ToString(),
            Category = r["Category"] == DBNull.Value ? "General" : r["Category"].ToString(),
            Rating = r["Rating"] == DBNull.Value ? 5 : Convert.ToInt32(r["Rating"]),
            IsActive = (bool)r["IsActive"]
        });
    }

    return list;
}



        public void Add(MenuItem item)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"INSERT INTO MenuItems (RestaurantID, Name, Description, Price, ImageUrl, IsActive)
                           VALUES (@r, @n, @d, @p, @i, 1)";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@r", item.RestaurantID);
            cmd.Parameters.AddWithValue("@n", item.Name);
            cmd.Parameters.AddWithValue("@d", item.Description);
            cmd.Parameters.AddWithValue("@p", item.Price);
            cmd.Parameters.AddWithValue("@i", item.ImageUrl);

            cmd.ExecuteNonQuery();
        }

        public MenuItem? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = "SELECT * FROM MenuItems WHERE MenuItemID = @id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return new MenuItem
                {
                    MenuItemID = (int)r["MenuItemID"],
                    RestaurantID = (int)r["RestaurantID"],
                    Name = r["Name"].ToString() ?? "",
                    Description = r["Description"].ToString() ?? "",
                    Price = Convert.ToDecimal(r["Price"]),
                    ImageUrl = r["ImageUrl"].ToString() ?? "",
                    IsActive = (bool)r["IsActive"]
                };
            }

            return null;
        }

        public void Update(MenuItem item)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"UPDATE MenuItems
                           SET Name=@n, Description=@d, Price=@p, ImageUrl=@i
                           WHERE MenuItemID=@id";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", item.MenuItemID);
            cmd.Parameters.AddWithValue("@n", item.Name);
            cmd.Parameters.AddWithValue("@d", item.Description);
            cmd.Parameters.AddWithValue("@p", item.Price);
            cmd.Parameters.AddWithValue("@i", item.ImageUrl);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"UPDATE MenuItems SET IsActive = 0 WHERE MenuItemID = @id";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }
        public List<MenuItem> Search(string q)
{
    var list = new List<MenuItem>();

    using var conn = new SqlConnection(_connectionString);
    conn.Open();

    string sql = @"
        SELECT *
        FROM MenuItems
        WHERE IsActive = 1
        AND (Name LIKE @q OR Description LIKE @q)
        ORDER BY Name ASC";

    using var cmd = new SqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@q", "%" + q + "%");

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        list.Add(new MenuItem
        {
            MenuItemID = (int)reader["MenuItemID"],
            RestaurantID = (int)reader["RestaurantID"],
            Name = reader["Name"].ToString() ?? "",
            Description = reader["Description"].ToString() ?? "",
            Price = Convert.ToDecimal(reader["Price"]),
            ImageUrl = reader["ImageUrl"].ToString() ?? "",
            IsActive = (bool)reader["IsActive"]
        });
    }

    return list;
}

    }
}
