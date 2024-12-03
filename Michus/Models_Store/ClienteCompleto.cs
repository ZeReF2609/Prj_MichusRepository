using Michus.Models;

namespace Michus.Models_Store
{
    public class ClienteCompleto
    {
        public Cliente Cliente { get; set; }
        public Contacto Contacto { get; set; }
        public UsuariosSistema Sistema { get; set; }
    }
}
