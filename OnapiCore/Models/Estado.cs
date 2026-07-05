using System;
using System.Collections.Generic;

namespace OnapiCore.Models;

public partial class Estado
{
    public int EstadoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Solicitude> Solicitudes { get; set; } = new List<Solicitude>();
}
