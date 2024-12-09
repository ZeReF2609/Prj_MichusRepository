using Michus.Models;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Michus.Service
{
    public class LoginCliService
    {
        private readonly string _connectionString;
        private readonly ILogger<LoginCliService> _logger;

        public LoginCliService(string connectionString, ILogger<LoginCliService> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<List<TipoDocumento>> ObtenerTiposDocumentoAsync()
        {
            var tiposDocumento = new List<TipoDocumento>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_obtener_tipos_documento", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tiposDocumento.Add(new TipoDocumento
                                {
                                    IdDoc = reader.GetInt32(0),
                                    Descripcion = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener tipos de documento: {ex.Message}");
                throw; // Re-throw the exception to handle it in the controller
            }

            return tiposDocumento;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public async Task<(string message, string? userId, string role)> ValidarUsuarioAsync(string email, string password)
        {
            string message = string.Empty;
            string? userId = null;
            string role = string.Empty;

            try
            {
                // Hash the password for verification
                string hashedPassword = HashPassword(password);

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_validar_usuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_email", email);
                        command.Parameters.AddWithValue("@p_password", hashedPassword);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                userId = reader.GetString(0);
                                role = reader.GetString(3);
                            }
                            else
                            {
                                message = "Usuario o contraseña incorrectos.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al validar usuario: {ex.Message}");
                message = "Error en la validación del usuario.";
            }

            return (message, userId, role);
        }



        public async Task<(string message, string? userId)> RegisterClientAsync(
         string emailre,
         string passwordre,
         string nombres,
         string apellidos,
         string userType,
         string role,
         string avatarUrl,
         string idCliente,
         int idDoc,
         string docIdent,
         DateTime fechaNacimiento)
        {
            string message = string.Empty;
            string? userId = null;

            try
            {
                string hashedPassword = HashPassword(passwordre);

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {


                            // 2. Then, register the client
                            using (var clientCommand = new SqlCommand("sp_register_cliente", connection, transaction))
                            {
                                clientCommand.CommandType = CommandType.StoredProcedure;

                                // Agregar parámetros de entrada
                                clientCommand.Parameters.AddWithValue("@p_nombres", nombres);
                                clientCommand.Parameters.AddWithValue("@p_apellidos", apellidos);
                                clientCommand.Parameters.AddWithValue("@p_docIdent", docIdent);
                                clientCommand.Parameters.AddWithValue("@p_fechaNacimiento", fechaNacimiento);
                                clientCommand.Parameters.AddWithValue("@p_idDoc", idDoc);

                                // Parámetro de salida @p_idCliente
                                var idClienteParam = new SqlParameter("@p_idCliente", SqlDbType.VarChar, 5)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                clientCommand.Parameters.Add(idClienteParam);

                                // Ejecutar el procedimiento
                                await clientCommand.ExecuteNonQueryAsync();

                                // Obtener el valor generado de @p_idCliente
                                idCliente = idClienteParam.Value?.ToString();
                                _logger.LogInformation($"Cliente registrado exitosamente: {idCliente}");
                            }


                            // 1. First, register the user
                            using (var userCommand = new SqlCommand("sp_register_usuario", connection, transaction))
                            {
                                userCommand.CommandType = CommandType.StoredProcedure;

                                userCommand.Parameters.AddWithValue("@p_email", emailre);
                                userCommand.Parameters.AddWithValue("@p_password", hashedPassword);
                                userCommand.Parameters.AddWithValue("@p_userType", userType);
                                userCommand.Parameters.AddWithValue("@p_role", "R05");
                                userCommand.Parameters.AddWithValue("@p_avatarUrl", avatarUrl);

                                var messageParam = new SqlParameter("@p_message", SqlDbType.NVarChar, 100)
                                {
                                    Direction = ParameterDirection.Output
                                };
                                var userIdParam = new SqlParameter("@p_userId", SqlDbType.VarChar, 5)
                                {
                                    Direction = ParameterDirection.Output
                                };

                                userCommand.Parameters.Add(messageParam);
                                userCommand.Parameters.Add(userIdParam);

                                await userCommand.ExecuteNonQueryAsync();

                                message = messageParam.Value?.ToString() ?? string.Empty;
                                userId = userIdParam.Value?.ToString();

                                if (string.IsNullOrEmpty(userId))
                                {
                                    transaction.Rollback();
                                    return ("Error al registrar el usuario", null);
                                }
                            }

                            transaction.Commit();
                            _logger.LogInformation($"Usuario y cliente registrados exitosamente. UserId: {userId}");
                            return ("Registro exitoso", userId);
                        }
                        catch (SqlException sqlEx)
                        {
                            _logger.LogError($"Error SQL en registro: {sqlEx.Message}, Número: {sqlEx.Number}");
                            transaction.Rollback();
                            return (GetFriendlyErrorMessage(sqlEx), null);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error general en registro: {ex.Message}");
                            transaction.Rollback();
                            return ("Error en el registro de usuario y cliente.", null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en conexión: {ex.Message}");
                return ("Error en la conexión con el servidor.", null);
            }
        }



        private string GetFriendlyErrorMessage(SqlException ex)
        {
            switch (ex.Number)
            {
                case 2627: // Unique constraint error
                    return "Ya existe un registro con estos datos.";
                case 547:  // Foreign key violation
                    return "El tipo de documento seleccionado no es válido.";
                case 2601: // Duplicate key error
                    return "Ya existe un registro con estos datos.";
                default:
                    return "Ha ocurrido un error en el registro.";
            }
        }

        ///
        public async Task<Cliente?> ObtenerClientePorIdAsync(string idCliente)
        {
            Cliente? cliente = null;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("sp_obtener_cliente_por_id", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@p_idCliente", idCliente);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                cliente = new Cliente
                                {
                                    IdCliente = reader.GetString(0),
                                    Nombres = reader.GetString(1),
                                    Apellidos = reader.GetString(2),
                                    IdDoc = reader.GetInt32(3),
                                    DocIdent = reader.GetString(4),
                                    FechaNacimiento = DateOnly.FromDateTime(reader.GetDateTime(5)), // Conversión corregida
                                    FechaRegistro = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                                    FechaUltimaCompra = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                                    NivelFidelidad = reader.GetByte(8),
                                    PuntosFidelidad = reader.GetInt32(9)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener cliente por ID: {ex.Message}");
                throw; // Lanza la excepción para que el controlador maneje el error
            }

            return cliente;
        }







    }
}
