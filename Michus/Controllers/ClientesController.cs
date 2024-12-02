using Microsoft.AspNetCore.Mvc;
using Michus.DAO;
using Michus.Service;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Michus.Models;
using System.Diagnostics;
using Michus.Models_Store;
using Newtonsoft.Json;

namespace Michus.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ClientesDAO _clientesDao;
        private readonly MenuService _menuService;
        private readonly TipoDocumentoDAO _tipoDocumentoDAO;

        // Inyección de dependencias
        public ClientesController(ClientesDAO clientesDao, MenuService menuService, TipoDocumentoDAO tipoDocumentoDAO)
        {
            _clientesDao = clientesDao;
            _menuService = menuService;
            _tipoDocumentoDAO = tipoDocumentoDAO;
        }

        [HttpGet("Usuarios/lista-clientes")]
        [Authorize]
        public async Task<IActionResult> ListaClientes()
        {
            try
            {
                // Obtener todos los clientes
                var clientes = _clientesDao.ObtenerTodosClientes();
                await LoadMenuDataAsync();
                if (clientes == null || clientes.Count == 0)
                {
                    return NotFound("No se encontraron clientes.");
                }

                return View(clientes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los datos de los clientes: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }

        }

        public IActionResult ObtenerTiposDocumento()
        {
            try
            {
                // Llamar al DAO para obtener los tipos de documentos de forma síncrona
                var tiposDocumento = _tipoDocumentoDAO.ObtenerTiposDocumento();

                // Verificar si la lista tiene elementos
                if (tiposDocumento == null || !tiposDocumento.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron tipos de documento." });
                }
                return Ok(tiposDocumento);
            }
            catch (Exception ex)
            {
                // En caso de error, retornar una respuesta con el código de error 500
                return StatusCode(500, new { mensaje = "Error al obtener tipos de documento", error = ex.Message });
            }
        }


        [Authorize]
        public IActionResult GuardarCliente([FromBody] ClienteCompleto clienteCompleto)
        {
            try
            {
                if (clienteCompleto == null || clienteCompleto.Sistema == null || clienteCompleto.Contacto == null)
                {
                    return BadRequest("Los datos del cliente, usuario y contacto son requeridos.");
                }

                // Convertir DateOnly a DateTime (si es necesario)
                var fechaNacimiento = clienteCompleto.Cliente.FechaNacimiento != default
                    ? clienteCompleto.Cliente.FechaNacimiento.ToDateTime(new TimeOnly(0, 0))
                    : DateTime.MinValue;

                // Incrementar el nivel de fidelidad
                var nivelFidelidad = clienteCompleto.Cliente.NivelFidelidad + 1;

                // Generar un nuevo cliente pasando la acción como 1
                var resultado = _clientesDao.InsertarCliente(
                    clienteCompleto.Sistema.Usuario,
                    clienteCompleto.Sistema.Email,
                    clienteCompleto.Sistema.Contrasenia,
                    clienteCompleto.Cliente.Nombres ?? string.Empty,
                    clienteCompleto.Cliente.Apellidos ?? string.Empty,
                    clienteCompleto.Cliente.IdDoc,
                    clienteCompleto.Cliente.DocIdent ?? string.Empty,
                    fechaNacimiento,
                    nivelFidelidad,
                    clienteCompleto.Cliente.PuntosFidelidad,
                    clienteCompleto.Contacto.Telefono ?? string.Empty,
                    clienteCompleto.Contacto.Direccion ?? string.Empty,
                    1 
                );

                // Devolver el resultado adecuado
                if (resultado)
                {
                    return Ok(new { success = true, mensaje = "Cliente guardado correctamente." });
                }
                else
                {
                    return StatusCode(500, new { success = false, mensaje = "Error al guardar el cliente." });
                }
            }
            catch (Exception ex)
            {
                // Manejo de error
                return StatusCode(500, new { success = false, mensaje = "Error al procesar la solicitud", error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ObtenerCliente(string idCliente)
        {
            // Obtenemos el cliente completo
            var clienteCompleto = _clientesDao.ObtenerClientePorId(idCliente);

            if (clienteCompleto == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            // Obtenemos los tipos de documento (esto depende de tu implementación)
            var tiposDocumento = _tipoDocumentoDAO.ObtenerTiposDocumento();

            // Retornamos el cliente completo junto con sistema, contacto y los tipos de documento
            return Json(new
            {
                cliente = clienteCompleto?.Cliente,
                sistema = clienteCompleto?.Sistema,
                contacto = clienteCompleto?.Contacto,
                tiposDocumento
            });
        }

        [HttpPost]
        public IActionResult ActualizarCliente([FromBody] ClienteCompleto clienteCompleto)
        {

            // Si clienteCompleto es null, se indica un error de deserialización
            if (clienteCompleto == null)
            {
                return BadRequest("Datos del cliente no deserializados correctamente.");
            }

            // Verificar que IdCliente no sea nulo ni vacío
            if (string.IsNullOrEmpty(clienteCompleto.Cliente?.IdCliente))
            {
                return BadRequest("El IdCliente es obligatorio.");
            }

            // Manejar la fecha de nacimiento de manera segura
            DateTime? fechaNacimiento = clienteCompleto.Cliente?.FechaNacimiento != default
                ? clienteCompleto.Cliente.FechaNacimiento.ToDateTime(new TimeOnly(0, 0))
                : (DateTime?)null;

            // Realizar la actualización con manejo de posibles nulls y valores por defecto
            bool resultado = _clientesDao.ActualizarCliente(
                3,
                clienteCompleto.Cliente.IdCliente,  // Este es obligatorio
                clienteCompleto.Cliente?.Nombres ?? "",    // Si es null, asignamos una cadena vacía
                clienteCompleto.Cliente?.Apellidos ?? "",  // Si es null, asignamos una cadena vacía
                clienteCompleto.Cliente.IdDoc,  // Este valor está fijo como null
                clienteCompleto.Cliente?.DocIdent ?? "",   // Si es null, asignamos una cadena vacía
                fechaNacimiento,                     // Puede ser null
                clienteCompleto.Cliente?.NivelFidelidad ?? 0,  // Si es null, asignamos un valor por defecto
                clienteCompleto.Cliente?.PuntosFidelidad ?? 0,  // Si es null, asignamos un valor por defecto
                clienteCompleto.Contacto?.Telefono ?? "",  // Si es null, asignamos una cadena vacía
                clienteCompleto.Contacto?.Direccion ?? "", // Si es null, asignamos una cadena vacía
                clienteCompleto.Sistema?.Usuario ?? "",  // Si es null, asignamos una cadena vacía
                clienteCompleto.Sistema?.Email ?? ""      // Si es null, asignamos una cadena vacía
            );

            // Retornar el resultado de la operación
            if (resultado)
            {
                return Ok(new { success = true, mensaje = "Cliente guardado correctamente." });
            }
            else
            {
                return StatusCode(500, new { success = false, mensaje = "Error al guardar el cliente." });
            }
        }




        public IActionResult EliminarCliente(string idCliente)
        {
            if (string.IsNullOrEmpty(idCliente))
            {
                return BadRequest(new { success = false, message = "El ID del cliente no puede estar vacío." });
            }

            bool eliminado = _clientesDao.EliminarCliente(idCliente);

            if (eliminado)
            {
                return Ok(new { success = true, message = "Cliente eliminado correctamente." });
            }
            else
            {
                return StatusCode(500, new { success = false, message = "Hubo un problema al eliminar el cliente." });
            }
        }

        public IActionResult ActivarCliente(string idCliente)
        {
            if (string.IsNullOrEmpty(idCliente))
            {
                return BadRequest(new { success = false, message = "El ID del cliente no puede estar vacío." });
            }

            bool activado = _clientesDao.ActivarCliente(idCliente);

            if (activado)
            {
                return Ok(new { success = true, message = "Cliente activado correctamente." });
            }
            else
            {
                return StatusCode(500, new { success = false, message = "Hubo un problema al activar el cliente." });
            }
        }


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
