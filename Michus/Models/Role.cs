using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Role
{
    public string IdRol { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public byte TipoUsuario { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<PermisosRol> PermisosRols { get; set; } = new List<PermisosRol>();

    public virtual ICollection<UsuariosSistema> UsuariosSistemas { get; set; } = new List<UsuariosSistema>();
}
