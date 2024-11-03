using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class DetalleVentum
{
    public string IdVenta { get; set; } = null!;

    public string IdProducto { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal PrecioTotal { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Venta IdVentaNavigation { get; set; } = null!;
}
