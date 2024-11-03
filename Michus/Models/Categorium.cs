using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Categorium
{
    public string IdCategoria { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
