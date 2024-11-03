using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;

namespace Michus.Service
{
    public class MenuService
    {
        private readonly string _connectionString;

        public MenuService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> GetMenusJsonByRoleAsync(string roleId)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_get_permissions", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@ID_ROL", roleId);

                var jsonResult = await command.ExecuteScalarAsync() as string;

                // Verifica si jsonResult es nulo o vacío y reemplázalo por "[]"
                if (string.IsNullOrWhiteSpace(jsonResult))
                {
                    jsonResult = "[]"; // JSON vacío para evitar errores
                }

                return jsonResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el JSON de menús: " + ex.Message, ex);
            }
        }
    }
}
