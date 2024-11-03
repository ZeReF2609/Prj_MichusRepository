using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Contacto
{
    public string IdUsuario { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public virtual UsuariosSistema IdUsuarioNavigation { get; set; } = null!;
}
