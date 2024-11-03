using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

public class LoginService
{
    private readonly string _connectionString;
    private readonly ILogger<LoginService> _logger;

    public LoginService(string connectionString, ILogger<LoginService> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task<(string Message, string? UserId, byte? UserType, string? Role)> ValidateLoginAsync(string email, string password)
    {
        string message;
        string? userId = null;
        byte? userType = null;
        string? role = null;

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_login_usuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@p_email", email);
                    command.Parameters.AddWithValue("@p_contrasenia", password);

                    var paramMessage = new SqlParameter("@p_mensaje", SqlDbType.VarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(paramMessage);

                    var paramUserId = new SqlParameter("@p_usuario_id", SqlDbType.VarChar, 5)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(paramUserId);

                    var paramRole = new SqlParameter("@p_rol", SqlDbType.VarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(paramRole);

                    var paramUserType = new SqlParameter("@p_tipo_usuario", SqlDbType.TinyInt)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(paramUserType);

                    connection.Open();

                    await command.ExecuteNonQueryAsync();

                    message = paramMessage.Value.ToString();
                    userId = paramUserId.Value != DBNull.Value ? paramUserId.Value.ToString() : null;
                    userType = paramUserType.Value != DBNull.Value ? (byte?)Convert.ToByte(paramUserType.Value) : null;
                    role = paramRole.Value != DBNull.Value ? paramRole.Value.ToString() : null;
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo de errores con logger
            _logger.LogError($"Error en ValidateLoginAsync: {ex.Message}");
            message = "Error en el proceso de autenticación";
        }

        return (message, userId, userType, role);
    }
}
