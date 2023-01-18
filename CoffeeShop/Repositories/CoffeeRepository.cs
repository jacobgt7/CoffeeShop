using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using CoffeeShop.Models;

namespace CoffeeShop.Repositories
{
    public class CoffeeRepository : ICoffeeRepository
    {
        private readonly string _connectionString;
        public CoffeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection Connection
        {
            get { return new SqlConnection(_connectionString); }
        }

        public List<Coffee> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Title, BeanVarietyId, b.Name, b.Region, b.Notes
                                        FROM Coffee c
                                        LEFT JOIN BeanVariety b ON c.BeanVarietyId = b.Id";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var coffees = new List<Coffee>();
                        while (reader.Read())
                        {
                            var coffee = new Coffee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                BeanVarietyId = reader.GetInt32(reader.GetOrdinal("BeanVarietyId")),
                                BeanVariety = new BeanVariety
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("BeanVarietyId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Region = reader.GetString(reader.GetOrdinal("Region"))
                                }
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
                            {
                                coffee.BeanVariety.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                            }
                            coffees.Add(coffee);
                        }
                        return coffees;
                    }
                }
            }
        }

        public Coffee Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Title, BeanVarietyId, b.Name, b.Region, b.Notes
                                        FROM Coffee c
                                        LEFT JOIN BeanVariety b ON c.BeanVarietyId = b.Id
                                        WHERE c.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var coffee = new Coffee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                BeanVarietyId = reader.GetInt32(reader.GetOrdinal("BeanVarietyId")),
                                BeanVariety = new BeanVariety
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("BeanVarietyId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Region = reader.GetString(reader.GetOrdinal("Region"))
                                }
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
                            {
                                coffee.BeanVariety.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                            }
                            return coffee;
                        }
                        return null;
                    }
                }
            }
        }

        public void Add(Coffee coffee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Coffee(Title, BeanVarietyId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@title, @beanVarietyId)";
                    cmd.Parameters.AddWithValue("@title", coffee.Title);
                    cmd.Parameters.AddWithValue("@beanVarietyId", coffee.BeanVarietyId);

                    coffee.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Coffee coffee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Coffee
                                        SET Title = @title
                                            BeanVarietyId = @beanVarietyId
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@title", coffee.Title);
                    cmd.Parameters.AddWithValue("@beanVarietyId", coffee.BeanVarietyId);
                    cmd.Parameters.AddWithValue("@id", coffee.Id);

                    cmd.ExecuteNonQuery();
                }

            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Coffee
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
