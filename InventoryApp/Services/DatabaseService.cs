using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Configuration;
using InventoryApp.Models;


namespace InventoryApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PostgreSQL"].ConnectionString;
        }

        public User Authenticate(string username, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT id, username, role FROM users WHERE username = @username AND password = @password", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Role = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Product> GetProducts()
        {
            var products = new List<Product>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, name, description, quantity, category, price FROM products", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Quantity = reader.GetInt32(3),
                            Category = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Price = reader.GetDecimal(5)
                        });
                    }
                }
            }
            return products;
        }

        public void AddProduct(Product product)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO products (name, description, quantity, category, price) " +
                    "VALUES (@name, @desc, @qty, @cat, @price)", conn))
                {
                    cmd.Parameters.AddWithValue("name", product.Name);
                    cmd.Parameters.AddWithValue("desc", (object)product.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("qty", product.Quantity);
                    cmd.Parameters.AddWithValue("cat", (object)product.Category ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("price", product.Price);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProduct(Product product)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "UPDATE products SET name = @name, description = @desc, quantity = @qty, " +
                    "category = @cat, price = @price WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", product.Id);
                    cmd.Parameters.AddWithValue("name", product.Name);
                    cmd.Parameters.AddWithValue("desc", (object)product.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("qty", product.Quantity);
                    cmd.Parameters.AddWithValue("cat", (object)product.Category ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("price", product.Price);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CreateDeleteRequest(int productId, int workerId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO deleterequests (productid, workerid, status) " +
                    "VALUES (@pid, @wid, 'Pending')", conn))
                {
                    cmd.Parameters.AddWithValue("pid", productId);
                    cmd.Parameters.AddWithValue("wid", workerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<DeleteRequest> GetPendingDeleteRequests()
        {
            var requests = new List<DeleteRequest>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                SELECT dr.id, dr.productid, dr.workerid, p.name, u.username, dr.createdat
                FROM deleterequests dr
                JOIN products p ON dr.productid = p.id
                JOIN users u ON dr.workerid = u.id
                WHERE dr.status = 'Pending'";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        requests.Add(new DeleteRequest
                        {
                            Id = reader.GetInt32(0),
                            ProductId = reader.GetInt32(1),
                            WorkerId = reader.GetInt32(2),
                            Product = new Product { Name = reader.GetString(3) },
                            Worker = new User { Username = reader.GetString(4) },
                            CreatedAt = reader.GetDateTime(5)
                        });
                    }
                }
            }
            return requests;
        }

        public void ProcessDeleteRequest(int requestId, int adminId, bool approve)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string status = approve ? "Approved" : "Rejected";

                using (var cmd = new NpgsqlCommand(
                    "UPDATE deleterequests SET status = @status, adminid = @aid WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("status", status);
                    cmd.Parameters.AddWithValue("aid", adminId);
                    cmd.Parameters.AddWithValue("id", requestId);
                    cmd.ExecuteNonQuery();
                }

                if (approve)
                {
                    int productId;
                    using (var cmd = new NpgsqlCommand(
                        "SELECT productid FROM deleterequests WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("id", requestId);
                        productId = (int)cmd.ExecuteScalar();
                    }

                    using (var cmd = new NpgsqlCommand(
                        "DELETE FROM products WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("id", productId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }

}
