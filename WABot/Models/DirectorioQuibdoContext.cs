using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WABot.Models
{
    public partial class directorioquibdoContext : DbContext
    {
 

        public directorioquibdoContext(DbContextOptions<directorioquibdoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categorium> Categoria { get; set; }
        public virtual DbSet<Fuente> Fuentes { get; set; }
        public virtual DbSet<Paciente> Pacientes { get; set; }
        public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categorium>(entity =>
            {
                entity.HasKey(e => e.CatId);

                entity.Property(e => e.CatId).HasColumnName("Cat_Id");

                entity.Property(e => e.CatDescripcion)
                    .HasMaxLength(50)
                    .HasColumnName("Cat_Descripcion");
            });

            modelBuilder.Entity<Fuente>(entity =>
            {
                entity.HasKey(e => e.FueId);

                entity.ToTable("Fuente");

                entity.Property(e => e.FueId).HasColumnName("Fue_Id");

                entity.Property(e => e.FueDescripcion)
                    .HasMaxLength(50)
                    .HasColumnName("Fue_Descripcion");
            });

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.PacId);

                entity.ToTable("Paciente");

                entity.HasIndex(e => e.PacTelefono, "pacienteTelefono_Unico")
                    .IsUnique();

                entity.Property(e => e.PacId).HasColumnName("Pac_Id");

                entity.Property(e => e.PacApellido1)
                    .HasMaxLength(20)
                    .HasColumnName("Pac_Apellido1");

                entity.Property(e => e.PacApellido2)
                    .HasMaxLength(20)
                    .HasColumnName("Pac_Apellido2");

                entity.Property(e => e.PacCodCiudad)
                    .HasMaxLength(3)
                    .HasColumnName("Pac_CodCiudad");

                entity.Property(e => e.PacCodDepto)
                    .HasMaxLength(2)
                    .HasColumnName("Pac_CodDepto");

                entity.Property(e => e.PacCodGenero)
                    .HasMaxLength(1)
                    .HasColumnName("Pac_CodGenero");

                entity.Property(e => e.PacCodNivelEducativo).HasColumnName("Pac_CodNivelEducativo");

                entity.Property(e => e.PacCodProfesion)
                    .HasMaxLength(4)
                    .HasColumnName("Pac_CodProfesion");

                entity.Property(e => e.PacDireccion)
                    .HasMaxLength(50)
                    .HasColumnName("Pac_Direccion");

                entity.Property(e => e.PacDominanciaCodigo).HasColumnName("Pac_Dominancia_Codigo");

                entity.Property(e => e.PacEstadoCivil).HasColumnName("Pac_EstadoCivil");

                entity.Property(e => e.PacFecha)
                    .HasColumnType("date")
                    .HasColumnName("Pac_Fecha");

                entity.Property(e => e.PacFechaNacimiento)
                    .HasColumnType("date")
                    .HasColumnName("Pac_FechaNacimiento");

                entity.Property(e => e.PacFirma)
                    .HasColumnType("image")
                    .HasColumnName("Pac_Firma");

                entity.Property(e => e.PacFoto)
                    .HasColumnType("image")
                    .HasColumnName("Pac_Foto");

                entity.Property(e => e.PacHuella)
                    .HasColumnType("image")
                    .HasColumnName("Pac_Huella");

                entity.Property(e => e.PacIdentificacion)
                    .HasMaxLength(10)
                    .HasColumnName("Pac_Identificacion");

                entity.Property(e => e.PacNombre1)
                    .HasMaxLength(20)
                    .HasColumnName("Pac_Nombre1");

                entity.Property(e => e.PacNombre2)
                    .HasMaxLength(20)
                    .HasColumnName("Pac_Nombre2");

                entity.Property(e => e.PacTelefono)
                    .HasMaxLength(12)
                    .HasColumnName("Pac_Telefono");

                entity.Property(e => e.PacTipoIdentificacion)
                    .HasMaxLength(3)
                    .HasColumnName("Pac_TipoIdentificacion");

                entity.Property(e => e.PacTipoSangre).HasColumnName("Pac_TipoSangre");
            });

            modelBuilder.Entity<TipoUsuario>(entity =>
            {
                entity.HasKey(e => e.TipUsId);

                entity.ToTable("TipoUsuario");

                entity.Property(e => e.TipUsId).HasColumnName("TipUs_Id");

                entity.Property(e => e.TipUsDescripcion)
                    .HasMaxLength(50)
                    .HasColumnName("TipUs_Descripcion");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
