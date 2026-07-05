using System;
using System.Collections.Generic;

namespace OnapiCore.Models;

public partial class Solicitude
{
    public int SolicitudId { get; set; }

    public int SolicitanteId { get; set; }

    public int TipoId { get; set; }

    public int EstadoId { get; set; }

    public string NombreProducto { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public virtual Estado? Estado { get; set; }
    public virtual Solicitante? Solicitante { get; set; }
    public virtual TiposDeRegistro? Tipo { get; set; }
}
