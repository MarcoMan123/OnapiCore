using System;
using System.Collections.Generic;

namespace OnapiCore.Models;

public partial class Solicitante
{
    public int SolicitanteId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string? Email { get; set; }

    public string? Telefono { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<Solicitude> Solicitudes { get; set; } = new List<Solicitude>();
}
