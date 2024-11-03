using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class DetallePromocion
{
    public int IdDetallePromocion { get; set; }

    public int IdPromocion { get; set; }

    public string IdProducto { get; set; } = null!;

    public byte TipoAplicacion { get; set; }

    public string? CantidadAplicable { get; set; }

    public virtual Promocion IdPromocionNavigation { get; set; } = null!;
}
