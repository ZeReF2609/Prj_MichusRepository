using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Promocion
{
    public int IdPromocion { get; set; }

    public string NombrePromocion { get; set; } = null!;

    public byte TipoPromocion { get; set; }

    public decimal? Descuento { get; set; }

    public string? Descripcion { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public byte Estado { get; set; }

    public virtual ICollection<DescuentoCab> DescuentoCabs { get; set; } = new List<DescuentoCab>();

    public virtual ICollection<DetallePromocion> DetallePromocions { get; set; } = new List<DetallePromocion>();
}
