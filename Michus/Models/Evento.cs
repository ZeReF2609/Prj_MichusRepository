using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Evento
{
    public string IdEventos { get; set; } = null!;

    public string NombreEvento { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public int Estado { get; set; }

    public virtual ICollection<DescuentoCab> DescuentoCabs { get; set; } = new List<DescuentoCab>();
}
