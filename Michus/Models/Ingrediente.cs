using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Ingrediente
{
    public int IdIngredientes { get; set; }

    public string NombreIngrediente { get; set; } = null!;

    public decimal Precio { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public byte Estado { get; set; }

    public virtual ICollection<AlmacenIngrediente> AlmacenIngredientes { get; set; } = new List<AlmacenIngrediente>();
}
