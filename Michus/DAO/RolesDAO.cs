using Michus.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Michus.DAO
{
    public class RolesDAO
    {
        private readonly string _connectionString;

        public RolesDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Role> ListarRoles()
        {
            List<Role> roles = new List<Role>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_ListarRoles", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Role
                            {
                                IdRol = reader["ID_ROL"].ToString(),
                                Rol = reader["ROL"].ToString()
                            });
                        }
                    }
                }
            }

            return roles;
        }
    }
}