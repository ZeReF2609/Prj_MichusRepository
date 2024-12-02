using Michus.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Michus.DAO
{
    public class CuentaDAO
    {
        private readonly string _connectionString;

        public CuentaDAO(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public List<TipoCuenta> ListarCuenta()
        {
            List<TipoCuenta> cuenta = new List<TipoCuenta>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_LISTAR_TIPO_CUENTA", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cuenta.Add(new TipoCuenta
                            {
                                ID_CUENTA = (byte)reader["ID_CUENTA"],
                                DESCRIPCION = reader["DESCRIPCION"].ToString()
                            });
                        }
                    }
                }
            }

            return cuenta;
        }

    }
}
