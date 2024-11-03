using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Submenu
{
    public int IdSubmenu { get; set; }

    public int IdMenu { get; set; }

    public string NombreSubmenu { get; set; } = null!;

    public string EnlaceSubmenu { get; set; } = null!;

    public int? OrdenSubmenu { get; set; }

    public bool EstadoSubmenu { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual Menu IdMenuNavigation { get; set; } = null!;

    public virtual ICollection<PermisosRol> PermisosRols { get; set; } = new List<PermisosRol>();
}
