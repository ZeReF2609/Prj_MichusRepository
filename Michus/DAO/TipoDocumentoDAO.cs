using Michus.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Michus.DAO
{
    public class TipoDocumentoDAO
    {

        private readonly string _connectionString;

        public TipoDocumentoDAO(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public List<TipoDocumento> ObtenerTiposDocumento()
        {
            var tipoDocumentos = new List<TipoDocumento>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand("SP_LISTAR_TIPO_DOCUMENTO", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var tipoDocumento = new TipoDocumento
                {
                    IdDoc = reader.GetInt32(reader.GetOrdinal("ID_DOC")),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("DESCRIPCION")) ? string.Empty : reader.GetString(reader.GetOrdinal("DESCRIPCION"))
                };

                tipoDocumentos.Add(tipoDocumento);
            }

            return tipoDocumentos;
        }

    }
}
