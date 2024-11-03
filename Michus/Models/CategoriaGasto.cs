using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class CategoriaGasto
{
    public string IdCategoriaGasto { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public byte Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<RegistroGasto> RegistroGastos { get; set; } = new List<RegistroGasto>();
}
