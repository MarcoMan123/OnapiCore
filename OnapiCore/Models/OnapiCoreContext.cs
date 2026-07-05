using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnapiCore.Models;

public partial class OnapiCoreContext : DbContext
{
    public OnapiCoreContext()
    {
    }

    public OnapiCoreContext(DbContextOptions<OnapiCoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Solicitante> Solicitantes { get; set; }

    public virtual DbSet<Solicitude> Solicitudes { get; set; }

    public virtual DbSet<TiposDeRegistro> TiposDeRegistros { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        //=> optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OnapiCore;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.EstadoId).HasName("PK__Estados__FEF86B0075E002A5");

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Solicitante>(entity =>
        {
            entity.HasKey(e => e.SolicitanteId).HasName("PK__Solicita__FCD5FE9A29DFB535");

            entity.Property(e => e.Cedula).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<Solicitude>(entity =>
        {
            entity.HasKey(e => e.SolicitudId).HasName("PK__Solicitu__85E95DC7D5321044");

            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreProducto).HasMaxLength(200);

            entity.HasOne(d => d.Estado).WithMany(p => p.Solicitudes)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Solicitud__Estad__534D60F1");

            entity.HasOne(d => d.Solicitante).WithMany(p => p.Solicitudes)
                .HasForeignKey(d => d.SolicitanteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Solicitud__Solic__5165187F");

            entity.HasOne(d => d.Tipo).WithMany(p => p.Solicitudes)
                .HasForeignKey(d => d.TipoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Solicitud__TipoI__52593CB8");
        });

        modelBuilder.Entity<TiposDeRegistro>(entity =>
        {
            entity.HasKey(e => e.TipoId).HasName("PK__TiposDeR__97099EB70F4EF171");

            entity.ToTable("TiposDeRegistro");

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
