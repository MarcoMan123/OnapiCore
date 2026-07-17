namespace OnapiCore.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = null!;
    public string ClaveHash { get; set; } = null!;
    public int RolId { get; set; }
    public DateTime FechaCreacion { get; set; }

    public virtual Rol? Rol { get; set; }
}