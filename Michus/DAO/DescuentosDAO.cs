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

        #region PRIMER INTENTO DE DAO
        public async Task<List<pa_lista_descuento>> GetDescuentosAsync()
        {
            var lista = new List<pa_lista_descuento>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_LISTAR_DESCUENTOS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            // Deserializamos el JSON de productos
                            var idArticulosJson = dr.IsDBNull(8) ? "[]" : dr.GetValue(8).ToString(); // Obtener el valor como string sin importar el tipo

                            // Intentar deserializar si el valor es un JSON válido, de lo contrario, asignar una lista vacía
                            List<Producto> idArticulosList = new List<Producto>();
                            try
                            {
                                // Deserializamos el JSON a una lista de objetos Producto
                                idArticulosList = JsonConvert.DeserializeObject<List<Producto>>(idArticulosJson);
                            }
                            catch (JsonSerializationException)
                            {
                                // Si ocurre un error en la deserialización, asignamos una lista vacía
                                idArticulosList = new List<Producto>();
                            }

                            // Extraemos solo los valores de ID_ARTICULOS en una lista de strings
                            List<string> idArticulos = idArticulosList.Select(p => p.IdProducto).ToList();

                            // Agregamos el objeto a la lista
                            lista.Add(
                                new pa_lista_descuento()
                                {
                                    IdDescuento = dr.IsDBNull(0) ? null : dr.GetString(0),
                                    IdPromocion = dr.IsDBNull(1) ? null : dr.GetInt32(1),
                                    IdEvento = dr.IsDBNull(2) ? null : dr.GetString(2),
                                    FechaInicio = dr.IsDBNull(3) ? DateTime.MinValue : dr.GetDateTime(3),
                                    FechaFin = dr.IsDBNull(4) ? DateTime.MinValue : dr.GetDateTime(4),
                                    PrecioDescuento = dr.IsDBNull(5) ? 0 : dr.GetDecimal(5),
                                    TipoDescuento = dr.IsDBNull(6) ? (byte)0 : dr.GetByte(6),
                                    IdCategoria = dr.IsDBNull(7) ? null : dr.GetString(7),
                                    IdArticulos = idArticulos, // Ahora asignamos la lista de ID_ARTICULOS
                                    Estado = dr.IsDBNull(9) ? 0 : dr.GetInt32(9),
                                    TiSitu = dr.IsDBNull(10) ? null : dr.GetString(10),
                                });
                        }
                    }
                }
            }

            return lista;
        }

        #endregion

        public async Task<List<pa_lista_descuento_carta>> GetDescuentosCartilla(DateTime? fechaInicio = null, DateTime? fechaFin = null, byte? tipoDescuento = null)
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


        

    }
}
