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

    public async Task<(string Message, string? UserId, byte? UserType, string? Role, string AvatarUrl)> ValidateLoginAsync(string email, string password)
    {
        string message;
        string? userId = null;
        byte? userType = null;
        string? role = null;
        string avatarUrl = "https://cdn-icons-png.flaticon.com/512/3177/3177440.png"; // URL predeterminada del avatar

        try
        {
            _logger.LogInformation("Iniciando proceso de validación de login para el correo: {Email}", email);

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

                    _logger.LogInformation("Ejecutando el procedimiento almacenado sp_login_usuario.");

                    await command.ExecuteNonQueryAsync();

                    message = paramMessage.Value.ToString();
                    userId = paramUserId.Value != DBNull.Value ? paramUserId.Value.ToString() : null;
                    userType = paramUserType.Value != DBNull.Value ? (byte?)Convert.ToByte(paramUserType.Value) : null;
                    role = paramRole.Value != DBNull.Value ? paramRole.Value.ToString() : null;

                    // Log para los resultados obtenidos de la base de datos
                    _logger.LogInformation("Resultado del procedimiento almacenado: Mensaje: {Message}, Usuario ID: {UserId}, Tipo Usuario: {UserType}, Rol: {Role}", message, userId, userType, role);
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo de errores con logger
            _logger.LogError($"Error en ValidateLoginAsync: {ex.Message}, StackTrace: {ex.StackTrace}");
            message = "Error en el proceso de autenticación";
        }

        return (message, userId, userType, role, avatarUrl);
    }
}

