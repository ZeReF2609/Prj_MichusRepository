using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class CalificacionesOpinione
{
    public string IdCalificacion { get; set; } = null!;

    public string IdUsuario { get; set; } = null!;

    public string? IdOpinion { get; set; }

    public int? Calificacion { get; set; }

    public DateTime? FechaCalificacion { get; set; }

    public byte Estado { get; set; }

    public virtual Opinione? IdOpinionNavigation { get; set; }

    public virtual UsuariosSistema IdUsuarioNavigation { get; set; } = null!;
}
