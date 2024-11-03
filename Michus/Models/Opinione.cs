using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Opinione
{
    public string IdOpinion { get; set; } = null!;

    public string IdClientes { get; set; } = null!;

    public string Comentarios { get; set; } = null!;

    public int? TotalCalificaciones { get; set; }

    public decimal? PromedioCalificaciones { get; set; }

    public DateTime? FechaOpinion { get; set; }

    public byte Estado { get; set; }

    public virtual ICollection<CalificacionesOpinione> CalificacionesOpiniones { get; set; } = new List<CalificacionesOpinione>();

    public virtual Cliente IdClientesNavigation { get; set; } = null!;
}
