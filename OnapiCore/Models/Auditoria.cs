using System.ComponentModel.DataAnnotations.Schema;

namespace OnapiCore.Models;

[Table("Auditoria")]
public partial class Auditoria
{
    public int AuditoriaId { get; set; }
    public string Metodo { get; set; } = null!;
    public string Ruta { get; set; } = null!;
    public int CodigoRespuesta { get; set; }
    public DateTime Fecha { get; set; }
}