using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class RegistroGasto
{
    public int IdGasto { get; set; }

    public DateTime FechaGasto { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal Monto { get; set; }

    public string? IdCategoriaGasto { get; set; }

    public virtual CategoriaGasto? IdCategoriaGastoNavigation { get; set; }
}
