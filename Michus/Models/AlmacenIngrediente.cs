using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class AlmacenIngrediente
{
    public int IdAlmacen { get; set; }

    public int IdIngrediente { get; set; }

    public int Cantidad { get; set; }

    public string? Ubicacion { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public DateTime? FechaVenc { get; set; }

    public virtual Ingrediente IdIngredienteNavigation { get; set; } = null!;
}
