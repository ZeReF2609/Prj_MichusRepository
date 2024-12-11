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



        public async Task<(string message, string? userId, string role)> ValidarUsuarioAsync(string email, string password)
        {
            string message = string.Empty;
            string? userId = null;
            string role = string.Empty;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("sp_validar_usuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.AddWithValue("@p_email", email);
                        command.Parameters.AddWithValue("@p_password", password);

                        // Parámetros de salida
                        var messageParam = new SqlParameter("@p_mensaje", SqlDbType.VarChar, 100)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(messageParam);

                        var userIdParam = new SqlParameter("@p_userId", SqlDbType.VarChar, 5)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(userIdParam);

                        var roleParam = new SqlParameter("@p_role", SqlDbType.VarChar, 50)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(roleParam);

                        // Ejecutar el comando
                        await command.ExecuteNonQueryAsync();

                        // Obtener los resultados de los parámetros de salida
                        message = messageParam.Value.ToString();
                        userId = userIdParam.Value.ToString();
                        role = roleParam.Value.ToString();
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


        public bool RegisterClientAsync(string usuario, string email, string contrasenia, string nombres, string apellidos, int idDoc, string docIdent, DateTime fechaNacimiento, int accion)
        {
            string idCliente = GenerarIdCliente();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand("sp_CRUD_Clientes", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Parámetros de entrada
            command.Parameters.AddWithValue("@ACCION", 1);
            command.Parameters.AddWithValue("@ID_CLIENTE", idCliente);
            command.Parameters.AddWithValue("@NOMBRES", nombres ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@APELLIDOS", apellidos ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ID_DOC", idDoc);
            command.Parameters.AddWithValue("@DOC_IDENT", docIdent ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FECHA_NACIMIENTO", fechaNacimiento);
            command.Parameters.AddWithValue("@NIVEL_FIDELIDAD", "0");
            command.Parameters.AddWithValue("@PUNTOS_FIDELIDAD", "0");
            command.Parameters.AddWithValue("@TELEFONO", "000");
            command.Parameters.AddWithValue("@DIRECCION","Sin Direccion");
            command.Parameters.AddWithValue("@USUARIO", "Cliente" ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EMAIL", email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CONTRASENIA", (contrasenia) ?? (object)DBNull.Value);

            try
            {
                // Ejecutamos el procedimiento almacenado
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar cliente: {ex.Message}");
                return false;
            }
        }


        private string GenerarIdCliente()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand("SELECT TOP 1 ID_CLIENTE FROM CLIENTES ORDER BY ID_CLIENTE DESC", connection);
            var ultimoId = command.ExecuteScalar() as string;

            if (string.IsNullOrEmpty(ultimoId))
            {
                return "C001";
            }

            var numeroStr = ultimoId.Substring(1);
            if (int.TryParse(numeroStr, out int numero))
            {
                return $"C{(numero + 1):D3}";
            }

            throw new Exception("Formato inválido en el ID del cliente.");
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
