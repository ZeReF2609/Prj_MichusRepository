using System.Data;
using System.Data.SqlClient;
using Michus.Models;
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
        
        public async Task<List<pa_lista_descuento_carta>> GetDescuentosCartilla(int? fechaInicio = null, int? fechaFin = null, byte? tipoDescuento = null)
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
                    command.Parameters.AddWithValue("@TIPO_DESCUENTO", tipoDescuento.HasValue ? (object)tipoDescuento.Value : DBNull.Value);

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

        public async Task RegistrarDescuentoAsync(Descuento descuento)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CREAR_DESCUENTO", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros para el procedimiento almacenado
                    command.Parameters.AddWithValue("@ID_DESCUENTO", descuento.IdDescuento ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ID_PROMOCION", descuento.IdPromocion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ID_EVENTO", descuento.IdEvento ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FECHA_INICIO", descuento.FechaInicio);
                    command.Parameters.AddWithValue("@FECHA_FIN", descuento.FechaFin);
                    command.Parameters.AddWithValue("@PRECIO_DESCUENTO", descuento.PrecioDescuento);
                    command.Parameters.AddWithValue("@TIPO_DESCUENTO", descuento.TipoDescuento);
                    command.Parameters.AddWithValue("@APLICAR_CATEGORIA", descuento.AplicarCategoria);
                    command.Parameters.AddWithValue("@ID_CATEGORIA", descuento.AplicarCategoria ? descuento.IdCategoria : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ID_ARTICULOS", !descuento.AplicarCategoria ? descuento.IdArticulos : (object)DBNull.Value);

                    // Ejecutar el comando
                    await command.ExecuteNonQueryAsync();
                }
            }
        }


    }
}
