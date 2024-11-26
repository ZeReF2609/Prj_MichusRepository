using Michus.Models;

namespace Michus.Models_Store
{
    public class EmpleadoCompleto
    {
        public Empleado Empleado { get; set; }
        public Contacto Contacto { get; set; }
        public Cuenta Cuenta { get; set; }
        public TipoCuenta TipoCuenta { get; set; }
        public UsuariosSistema Sistema { get; set; }
    }
}
