using Microsoft.Data.SqlClient;
using YemekSepeti.Models;

namespace YemekSepeti.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ---------------- LOGIN ----------------
        public User? ValidateUser(string email, string password)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT UserID, FullName, Email, Password, Role
                                 FROM Users
                                 WHERE Email = @Email AND Password = @Password";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = (int)reader["UserID"],
                                Name = reader["FullName"].ToString() ?? "",
                                Email = reader["Email"].ToString() ?? "",
                                Password = reader["Password"].ToString() ?? "",
                                Role = reader["Role"].ToString() ?? "User"
                            };
                        }
                    }
                }
            }

            return null;
        }

        // ---------------- SIGNUP ----------------
        public bool CreateUser(string name, string email, string password)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // check if email exists
                using (var check = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email=@Email", conn))
                {
                    check.Parameters.AddWithValue("@Email", email);
                    int exists = (int)check.ExecuteScalar();

                    if (exists > 0)
                        return false;
                }

                // insert new user
                string query = @"INSERT INTO Users (FullName, Email, Password, Role)
                                 VALUES (@FullName, @Email, @Password, 'User')";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }
    }
}
