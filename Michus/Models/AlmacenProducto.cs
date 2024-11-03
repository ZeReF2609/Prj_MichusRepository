using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class AlmacenProducto
{
    public string IdProducto { get; set; } = null!;

    public string Presentacion { get; set; } = null!;

    public int Cantidad { get; set; }

    public string Ubicacion { get; set; } = null!;

    public DateTime? FechaIngreso { get; set; }

    public DateTime? FechaVenc { get; set; }

    public string? IdAlmacen { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
