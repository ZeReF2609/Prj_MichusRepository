using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Menu
{
    public int IdMenu { get; set; }

    public string NombreMenu { get; set; } = null!;

    public int OrdenMenu { get; set; }

    public bool EstadoMenu { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public virtual ICollection<Submenu> Submenus { get; set; } = new List<Submenu>();
}
