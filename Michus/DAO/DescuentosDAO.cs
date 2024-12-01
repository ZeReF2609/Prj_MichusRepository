using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using Michus.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Michus.DAO
{
    public class DescuentosDAO
    {
        private readonly string _connectionString;


        public DescuentosDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<pa_lista_descuento_carta>> GetDescuentosCartilla(int? fechaInicio = null, int? fechaFin = null, string ti_situ = "")
        {
            var lista = new List<pa_lista_descuento_carta>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_LISTAR_DESCUENTOS_CARTAS_PORFILTROS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Añadir parámetros con valores predeterminados nulos
                    command.Parameters.AddWithValue("@FECHA_INICIO", fechaInicio.HasValue ? (object)fechaInicio.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@FECHA_FIN", fechaFin.HasValue ? (object)fechaFin.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@TI_SITU", ti_situ);

                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lista.Add(
                                new pa_lista_descuento_carta()
                                {
                                    IdDescuento = dr.IsDBNull(0) ? null : dr.GetString(0),
                                    FechaInicio = dr.IsDBNull(1) ? DateTime.MinValue : dr.GetDateTime(1),
                                    FechaFin = dr.IsDBNull(2) ? DateTime.MinValue : dr.GetDateTime(2),
                                    PrecioDescuento = dr.IsDBNull(3) ? 0 : dr.GetDecimal(3),
                                    TipoDescuento = dr.IsDBNull(4) ? (byte)0 : dr.GetByte(4),
                                    Estado = dr.IsDBNull(5) ? 0 : dr.GetInt32(5),
                                    TiSitu = dr.IsDBNull(6) ? null : dr.GetString(6),
                                });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<List<pa_anio>> GetAnioInicioDesc()
        {
            List<pa_anio> anioinicioDesc = new List<pa_anio>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_LISTAR_ANOS_INICIO_DESCUENTOS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        while (dr.Read())
                        {
                            pa_anio objeto = new pa_anio()
                            {
                                anio = dr.GetInt32(0)
                            };
                            anioinicioDesc.Add(objeto);
                        }
                    }
                }
            }

            return anioinicioDesc;
        }

        public async Task<List<pa_anio>> GetAnioFinDesc()
        {
            List<pa_anio> anioinicioDesc = new List<pa_anio>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_LISTAR_ANOS_FIN_DESCUENTOS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        while (dr.Read())
                        {
                            pa_anio objeto = new pa_anio()
                            {
                                anio = dr.GetInt32(0)
                            };
                            anioinicioDesc.Add(objeto);
                        }
                    }
                }
            }

            return anioinicioDesc;
        }



        // SIN USAR
        public async Task<List<dynamic>> ListarProductosAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var productos = new List<dynamic>();

                // Listar productos usando SP_LISTAR_PRODUCTOS_DESCUENTO
                using (var command = new SqlCommand("SP_LISTAR_PRODUCTOS_DESCUENTO", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var producto = new ExpandoObject() as IDictionary<string, object>;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                producto[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            productos.Add(producto);
                        }
                    }
                }

                return productos;
            }
        }

        public async Task<List<dynamic>> ListarCategoriasAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var categorias = new List<dynamic>();

                // Listar categorías usando SP_LISTAR_CATEGORIA_DESCUENTO
                using (var command = new SqlCommand("SP_LISTAR_CATEGORIA_DESCUENTO", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var categoria = new ExpandoObject() as IDictionary<string, object>;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                categoria[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            categorias.Add(categoria);
                        }
                    }
                }

                return categorias;
            }
        }



    }
}
