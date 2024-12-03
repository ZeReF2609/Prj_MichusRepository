using Michus.DAO;
using Michus.Models_Store;
using Michus.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Michus.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly EmpleadoDAO _empleadoDao;
        private readonly MenuService _menuService;
        private readonly TipoDocumentoDAO _tipoDocumentoDAO;
        private readonly CuentaDAO _cuentaDAO;
        private readonly RolesDAO _rolesDAO;

        public EmpleadoController(EmpleadoDAO empleadoDao, MenuService menuService, TipoDocumentoDAO tipoDocumentoDAO, CuentaDAO cuentaDAO, RolesDAO rolesDAO)
        {
            _empleadoDao = empleadoDao;
            _menuService = menuService;
            _tipoDocumentoDAO = tipoDocumentoDAO;
            _cuentaDAO = cuentaDAO;
            _rolesDAO = rolesDAO;
        }

        [HttpGet("Usuarios/lista-empleado")]
        [Authorize]
        public async Task<IActionResult> ListaEmpleados()
        {
            try
            {
                var empleados = _empleadoDao.ObtenerTodosEmpleado();

                await LoadMenuDataAsync();

                if (empleados == null || empleados.Count == 0)
                {
                    return NotFound("No se encontraron empleados.");
                }

                return View(empleados);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los datos de los empleados: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult GuardarEmpleado([FromBody] EmpleadoCompleto empleadoCompleto)
        {
            try
            {
                if (empleadoCompleto == null ||
                    empleadoCompleto.Empleado == null ||
                    empleadoCompleto.Contacto == null ||
                    empleadoCompleto.Sistema == null ||
                    empleadoCompleto.Cuenta == null)
                {
                    return StatusCode(400, new { success = false, mensaje = "Faltan datos requeridos." });
                }

                // Convertir la fecha de nacimiento a DateTime si es necesario
                DateTime fechaNacimiento;
                if (!DateTime.TryParse(empleadoCompleto.Empleado.FechaNacimiento.ToString(), out fechaNacimiento))
                {
                    return StatusCode(400, new { success = false, mensaje = "Fecha de nacimiento inválida." });
                }

                // Asegúrate de que el tipoCuenta es un número
                int tipoCuenta;
                if (!int.TryParse(empleadoCompleto.Cuenta.TipoCuenta.ToString(), out tipoCuenta))
                {
                    return StatusCode(400, new { success = false, mensaje = "Tipo de cuenta no válido." });
                }

                string rol = empleadoCompleto.Sistema.IdRol;

                if (string.IsNullOrEmpty(rol))
                {
                    return StatusCode(400, new { success = false, mensaje = "El rol no es válido." });
                }


                var resultado = _empleadoDao.InsertarEmpleado(
                    empleadoCompleto.Sistema.Usuario ?? string.Empty,
                    empleadoCompleto.Sistema.Email ?? string.Empty,
                    empleadoCompleto.Sistema.Contrasenia ?? string.Empty,
                    empleadoCompleto.Empleado.Nombres ?? string.Empty,
                    empleadoCompleto.Empleado.Apellidos ?? string.Empty,
                    empleadoCompleto.Empleado.IdDoc,
                    empleadoCompleto.Empleado.DocIdent ?? string.Empty,
                    fechaNacimiento,
                    empleadoCompleto.Empleado.Salario ?? 0m, // Asegúrate de que Salario esté correctamente manejado
                    empleadoCompleto.Contacto.Telefono ?? string.Empty,
                    empleadoCompleto.Contacto.Direccion ?? string.Empty,
                    empleadoCompleto.Sistema.IdRol ?? string.Empty,
                    tipoCuenta,  // El tipo de cuenta ya está validado
                    empleadoCompleto.Cuenta.NumeroCuenta ?? string.Empty
                );

                if (resultado)
                {
                    return Ok(new { success = true, mensaje = "Empleado guardado correctamente." });
                }
                else
                {
                    return StatusCode(500, new { success = false, mensaje = "Error al guardar el empleado." });
                }
            }
            catch (Exception ex)
            {
                // Log de error para depuración
                return StatusCode(500, new { success = false, mensaje = "Error al procesar la solicitud", error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ObtenerEmpleado(string idEmpleado)
        {
            try
            {
                var empleadoCompleto = _empleadoDao.ObtenerEmpleadoPorId(idEmpleado);

                if (empleadoCompleto == null)
                {
                    return NotFound(new { message = "Empleado no encontrado" });
                }

                // Aquí aseguramos que estamos enviando todos los datos correctamente.
                return Json(new
                {
                    empleado = new
                    {
                        empleadoCompleto.Empleado.IdEmpleado,
                        empleadoCompleto.Empleado.Nombres,
                        empleadoCompleto.Empleado.Apellidos,
                        empleadoCompleto.Empleado.FechaNacimiento,
                        empleadoCompleto.Empleado.DocIdent,
                        empleadoCompleto.Empleado.Salario,
                        empleadoCompleto.Empleado.FechaIngreso,
                        empleadoCompleto.Empleado.IdDoc // Asegúrate de que este campo esté incluido
                    },
                    contacto = new
                    {
                        empleadoCompleto.Contacto.Telefono,
                        empleadoCompleto.Contacto.Direccion
                    },
                    sistema = new
                    {
                        empleadoCompleto.Sistema.Usuario,
                        empleadoCompleto.Sistema.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, mensaje = "Error al procesar la solicitud", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ActualizarEmpleado([FromBody] EmpleadoCompleto empleadoCompleto)
        {
            if (empleadoCompleto == null)
            {
                return BadRequest("Datos del Empleado no deserializados correctamente.");
            }

            if (string.IsNullOrEmpty(empleadoCompleto.Empleado?.IdEmpleado))
            {
                return BadRequest("El IdEmpleado es obligatorio.");
            }

            DateTime? fechaNacimiento = empleadoCompleto.Empleado?.FechaNacimiento == null
                ? (DateTime?)null
        : empleadoCompleto.Empleado.FechaNacimiento.ToDateTime(new TimeOnly(0, 0));

            DateTime fechaIngreso = (DateTime)(empleadoCompleto.Empleado?.FechaIngreso);

            bool resultado = _empleadoDao.ActualizarEmpleado(
                empleadoCompleto.Empleado.IdEmpleado,  
                empleadoCompleto.Empleado?.Nombres ?? "",    
                empleadoCompleto.Empleado?.Apellidos ?? "",  
                empleadoCompleto.Empleado.IdDoc,  
                empleadoCompleto.Empleado?.DocIdent ?? "",  
                fechaNacimiento,                   
                empleadoCompleto.Contacto?.Telefono ?? "", 
                empleadoCompleto.Contacto?.Direccion ?? "", 
                empleadoCompleto.Sistema?.Usuario ?? "",  
                empleadoCompleto.Sistema?.Email ?? "",   
                fechaIngreso                          
            );

            if (resultado)
            {
                return Ok(new { success = true, mensaje = "Empleado actualizado correctamente." });
            }
            else
            {
                return StatusCode(500, new { success = false, mensaje = "Error al actualizar el empleado." });
            }
        }

        public IActionResult EliminarEmpleado(string idEmpleado)
        {
            if (string.IsNullOrEmpty(idEmpleado))
            {
                return BadRequest(new { success = false, message = "El ID del Empleado no puede estar vacío." });
            }

            bool eliminado = _empleadoDao.EliminarEmpleado(idEmpleado);

            if (eliminado)
            {
                return Ok(new { success = true, message = "Empleado eliminado correctamente." });
            }
            else
            {
                return StatusCode(500, new { success = false, message = "Hubo un problema al eliminar el Empleado." });
            }
        }

        public IActionResult ActivarEmpleado(string idEmpleado)
        {
            if (string.IsNullOrEmpty(idEmpleado))
            {
                return BadRequest(new { success = false, message = "El ID del Empleado no puede estar vacío." });
            }

            bool activado = _empleadoDao.ActivarEmpleado(idEmpleado);

            if (activado)
            {
                return Ok(new { success = true, message = "Empleado activado correctamente." });
            }
            else
            {
                return StatusCode(500, new { success = false, message = "Hubo un problema al activar el Empleado." });
            }
        }

        public IActionResult ObtenerTiposDocumento()
        {
            try
            {
                var tiposDocumento = _tipoDocumentoDAO.ObtenerTiposDocumento();

                if (tiposDocumento == null || !tiposDocumento.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron tipos de documento." });
                }
                return Ok(tiposDocumento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener tipos de documento", error = ex.Message });
            }
        }

        public IActionResult ObtenerTipoCuenta()
        {
            try
            {
                var tiposDocumento = _cuentaDAO.ListarCuenta();

                if (tiposDocumento == null || !tiposDocumento.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron tipos de cuenta" });
                }
                return Ok(tiposDocumento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener tipos de documento", error = ex.Message });
            }
        }

        public IActionResult ObtenerRoles()
        {
            try
            {
                var tiposDocumento = _rolesDAO.ListarRoles();

                if (tiposDocumento == null || !tiposDocumento.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron los roles" });
                }
                return Ok(tiposDocumento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los roles", error = ex.Message });
            }
        }

        //Obtener claim
        private async Task LoadMenuDataAsync()
        {
            string roleId = GetCurrentRoleId();
            var menuJson = await _menuService.GetMenusJsonByRoleAsync(roleId);
            ViewData["MenuItems"] = string.IsNullOrEmpty(menuJson) ? "[]" : menuJson;
        }

        private string GetCurrentRoleId()
        {
            var roleIdClaim = User.FindFirst(ClaimTypes.Role);
            return roleIdClaim?.Value ?? string.Empty;
        }
    }
}
