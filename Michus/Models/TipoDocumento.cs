using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class TipoDocumento
{
    public int IdDoc { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
