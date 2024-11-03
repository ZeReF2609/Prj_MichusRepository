using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class PermisosRol
{
    public string IdRol { get; set; } = null!;

    public int IdSubmenu { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual Role IdRolNavigation { get; set; } = null!;

    public virtual Submenu IdSubmenuNavigation { get; set; } = null!;
}
