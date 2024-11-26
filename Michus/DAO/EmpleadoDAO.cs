using Michus.Models;
using Michus.Models_Store;
using System.Data;
using System.Data.SqlClient;

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
                command.Parameters.AddWithValue("@Accion", 2);  // Acción 2 es "Listar"

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

                        // Creación de TipoCuenta (con su descripción)
                        TipoCuenta tipoCuenta = new TipoCuenta
                        {
                            DESCRIPCION = reader["TIPO CUENTA"]?.ToString() ?? ""  // Usamos el alias correcto con espacio
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
    }
}
