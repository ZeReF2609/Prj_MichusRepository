using Michus.Models;
using Michus.Models_Store;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Michus.DAO
{
    public class EmpleadoDAO
    {
        private readonly string _connectionString;

        public EmpleadoDAO(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public List<EmpleadoCompleto> ObtenerTodosEmpleado(string idEmpleado = null)
        {
            List<EmpleadoCompleto> empleadosCompletos = new List<EmpleadoCompleto>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("sp_CRUD_Empleados", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Accion", 2);

                if (!string.IsNullOrEmpty(idEmpleado))
                {
                    command.Parameters.AddWithValue("@ID_EMPLEADO", idEmpleado);
                }
                else
                {
                    command.Parameters.AddWithValue("@ID_EMPLEADO", DBNull.Value);
                }

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        // Creación de empleado
                        Empleado empleado = new Empleado
                        {
                            IdEmpleado = reader["ID_EMPLEADO"].ToString(),
                            Nombres = reader["NOMBRES"].ToString(),
                            Apellidos = reader["APELLIDOS"]?.ToString(),
                            FechaNacimiento = reader["FECHA_NACIMIENTO"] != DBNull.Value
                                ? DateOnly.FromDateTime(Convert.ToDateTime(reader["FECHA_NACIMIENTO"]))
                                : DateOnly.MinValue,
                            DocIdent = reader["DOC_IDENT"]?.ToString(),
                            Salario = reader["SALARIO"] != DBNull.Value ? Convert.ToDecimal(reader["SALARIO"]) : (decimal?)null,
                            FechaIngreso = reader["FECHA_INGRESO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["FECHA_INGRESO"])
                                : DateTime.MinValue,
                            Estado = reader["ESTADO"] != DBNull.Value ? Convert.ToByte(reader["ESTADO"]) : (byte)0
                        };

                        // Creación de contacto
                        Contacto contacto = new Contacto
                        {
                            IdUsuario = reader["ID_EMPLEADO"].ToString(),
                            Telefono = reader["TELEFONO"]?.ToString(),
                            Direccion = reader["DIRECCION"]?.ToString()
                        };

                        // Creación de cuenta
                        Cuenta cuenta = new Cuenta
                        {
                            IdEmpleado = reader["ID_EMPLEADO"].ToString(),
                        };

                        // Creación de usuario del sistema
                        UsuariosSistema usuarioSistema = new UsuariosSistema
                        {
                            IdUsuario = reader["ID_EMPLEADO"].ToString(),
                            Usuario = reader["USUARIO"]?.ToString(),
                            Email = reader["EMAIL"]?.ToString(),
                            Estado = reader["ESTADO"] != DBNull.Value ? Convert.ToByte(reader["ESTADO"]) : (byte)0
                        };

                        // Agregar los datos completos del empleado a la lista
                        empleadosCompletos.Add(new EmpleadoCompleto
                        {
                            Empleado = empleado,
                            Contacto = contacto,
                            Cuenta = cuenta,
                            Sistema = usuarioSistema
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return empleadosCompletos;
        }

        public bool InsertarEmpleado(string usuario,string email,string contrasenia,string nombres,string apellidos,int idDoc,string docIdent,DateTime fechaNacimiento,decimal salario,string telefono,string direccion,string rol, 
        int tipoCuenta,string numeroCuenta) 
        {
            string idEmpleado = GenerarIdEmpleado();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand("sp_CRUD_Empleados", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@ACCION", 1); 
            command.Parameters.AddWithValue("@ID_EMPLEADO", idEmpleado);
            command.Parameters.AddWithValue("@NOMBRES", nombres ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@APELLIDOS", apellidos ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ID_DOC", idDoc);
            command.Parameters.AddWithValue("@DOC_IDENT", docIdent ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FECHA_NACIMIENTO", fechaNacimiento);
            command.Parameters.AddWithValue("@SALARIO", salario);
            command.Parameters.AddWithValue("@TELEFONO", telefono ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DIRECCION", direccion ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@USUARIO", usuario ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EMAIL", email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CONTRASENIA", contrasenia ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ROL", rol ?? (object)DBNull.Value);  
            command.Parameters.AddWithValue("@TIPO_CUENTA", tipoCuenta);  
            command.Parameters.AddWithValue("@NUMERO_CUENTA", numeroCuenta ?? (object)DBNull.Value); 

            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar empleado: {ex.Message}");
                return false;
            }
        }

        public EmpleadoCompleto ObtenerEmpleadoPorId(string idEmpleado)
        {
            EmpleadoCompleto? empleadoCompleto = null;

            // Verificamos si el ID de empleado es válido antes de proceder.
            if (string.IsNullOrEmpty(idEmpleado))
            {
                Console.WriteLine("El ID del empleado no puede ser nulo o vacío.");
                return null; // Si no hay un ID válido, retornamos null.
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("sp_CRUD_Empleados", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Parámetros del procedimiento almacenado
                command.Parameters.AddWithValue("@Accion", 2); // Acción = 2 para listar
                command.Parameters.AddWithValue("@ID_EMPLEADO", idEmpleado); // Usamos el ID proporcionado

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Crear objeto empleado
                            Empleado empleado = new Empleado
                            {
                                IdEmpleado = reader["ID_EMPLEADO"].ToString(),
                                Nombres = reader["NOMBRES"].ToString(),
                                Apellidos = reader["APELLIDOS"]?.ToString(),
                                FechaNacimiento = reader["FECHA_NACIMIENTO"] != DBNull.Value
                                    ? DateOnly.FromDateTime(Convert.ToDateTime(reader["FECHA_NACIMIENTO"]))
                                    : DateOnly.MinValue,
                                DocIdent = reader["DOC_IDENT"]?.ToString(),
                                Salario = reader["SALARIO"] != DBNull.Value ? Convert.ToDecimal(reader["SALARIO"]) : (decimal?)null,
                                FechaIngreso = reader["FECHA_INGRESO"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["FECHA_INGRESO"])
                                    : DateTime.MinValue,
                                IdDoc = reader["ID_DOC"] != DBNull.Value ? Convert.ToInt32(reader["ID_DOC"]) : 0
                            };

                            // Crear objeto contacto
                            Contacto contacto = new Contacto
                            {
                                IdUsuario = reader["ID_EMPLEADO"].ToString(),
                                Telefono = reader["TELEFONO"]?.ToString(),
                                Direccion = reader["DIRECCION"]?.ToString()
                            };

                            // Crear objeto cuenta
                            Cuenta cuenta = new Cuenta
                            {
                                
                            };

                            // Crear objeto usuario sistema
                            UsuariosSistema usuarioSistema = new UsuariosSistema
                            {
                                IdUsuario = reader["ID_EMPLEADO"].ToString(),
                                Usuario = reader["USUARIO"]?.ToString(),
                                Email = reader["EMAIL"]?.ToString(),
                                Estado = reader["ESTADO"] != DBNull.Value ? Convert.ToByte(reader["ESTADO"]) : (byte)0
                            };

                            // Construir el objeto completo del empleado
                            empleadoCompleto = new EmpleadoCompleto
                            {
                                Empleado = empleado,
                                Contacto = contacto,
                                Cuenta = cuenta,
                                Sistema = usuarioSistema
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return empleadoCompleto; // Devuelve el objeto completo del empleado (o null si no se encontró)
        }

        public bool ActualizarEmpleado(string idEmpleado, string nombres, string apellidos, int? idDoc, string docIdent,DateTime? fechaNacimiento, string telefono, string direccion, string usuario,
        string email, DateTime? fechaIngreso)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_CRUD_Empleados", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Accion", 3); 
                    command.Parameters.AddWithValue("@ID_EMPLEADO", idEmpleado ?? (object)DBNull.Value); 
                    command.Parameters.AddWithValue("@NOMBRES", nombres ?? (object)DBNull.Value);  
                    command.Parameters.AddWithValue("@APELLIDOS", apellidos ?? (object)DBNull.Value); 
                    command.Parameters.AddWithValue("@ID_DOC", idDoc ?? (object)DBNull.Value); 
                    command.Parameters.AddWithValue("@DOC_IDENT", docIdent ?? (object)DBNull.Value); 
                    command.Parameters.AddWithValue("@FECHA_NACIMIENTO", fechaNacimiento ?? (object)DBNull.Value); 
                    command.Parameters.AddWithValue("@TELEFONO", telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DIRECCION", direccion ?? (object)DBNull.Value); 
                    command.Parameters.AddWithValue("@USUARIO", usuario ?? (object)DBNull.Value); 
                    command.Parameters.AddWithValue("@EMAIL", email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FECHA_INGRESO", fechaIngreso ?? (object)DBNull.Value); 

                    connection.Open();
                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();  
                        return true; 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al actualizar empleado: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public bool EliminarEmpleado(string idEmpleado)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_CRUD_Empleados", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Accion", 4);  // Acción de eliminar
                        command.Parameters.AddWithValue("@ID_EMPLEADO", idEmpleado);  // ID del cliente

                        connection.Open();
                        command.ExecuteNonQuery();  // Ejecutamos el procedimiento sin verificar filas afectadas

                        return true;  // Si no ocurre excepción, consideramos que la operación fue exitosa
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error: {ex.Message}");
                return false;  // Si ocurre una excepción, la operación falla
            }
        }

        public bool ActivarEmpleado(string idEmpleado)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_CRUD_Empleados", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Accion", 5);  // Acción de activar
                        command.Parameters.AddWithValue("@ID_EMPLEADO", idEmpleado);  // ID del cliente

                        connection.Open();
                        command.ExecuteNonQuery();  // Ejecutamos el procedimiento sin verificar filas afectadas

                        return true;  // Si no ocurre excepción, consideramos que la operación fue exitosa
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error: {ex.Message}");
                return false;  // Si ocurre una excepción, la operación falla
            }
        }

        private string GenerarIdEmpleado()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand("SELECT TOP 1 ID_EMPLEADO FROM EMPLEADO ORDER BY ID_EMPLEADO DESC", connection);
            var ultimoId = command.ExecuteScalar() as string;

            if (string.IsNullOrEmpty(ultimoId))
            {
                return "E001";
            }

            var numeroStr = ultimoId.Substring(1);
            if (int.TryParse(numeroStr, out int numero))
            {
                return $"E{(numero + 1):D3}";
            }

            throw new Exception("Formato inválido en el ID del Empleado.");
        }
    }
}
