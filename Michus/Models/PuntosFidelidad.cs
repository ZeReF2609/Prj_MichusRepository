using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class PuntosFidelidad
{
    public int IdPunto { get; set; }

    public string IdCliente { get; set; } = null!;

    public int Puntos { get; set; }

    public DateOnly Fecha { get; set; }

    public byte TipoAccion { get; set; }

    public string? Descripcion { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;
}
