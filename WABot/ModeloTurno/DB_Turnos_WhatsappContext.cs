using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WABot.ModeloTurno
{
    public partial class DB_Turnos_WhatsappContext : DbContext
    {
       
        public DB_Turnos_WhatsappContext(DbContextOptions<DB_Turnos_WhatsappContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Estado> Estados { get; set; }
        public virtual DbSet<Persona> Personas { get; set; }
        public virtual DbSet<Turno> Turnos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Estado>(entity =>
            {
                entity.HasKey(e => e.EstId);

                entity.ToTable("estado");

                entity.Property(e => e.EstId).HasColumnName("est_id");

                entity.Property(e => e.EstDescripcion)
                    .HasMaxLength(50)
                    .HasColumnName("est_descripcion");
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.PerId);

                entity.ToTable("persona");

                entity.Property(e => e.PerId).HasColumnName("per_id");

                entity.Property(e => e.PerDireccion)
                    .HasMaxLength(50)
                    .HasColumnName("per_direccion");

                entity.Property(e => e.PerIdentificacion)
                    .HasMaxLength(50)
                    .HasColumnName("per_identificacion");

                entity.Property(e => e.PerNombre)
                    .HasMaxLength(50)
                    .HasColumnName("per_nombre");

                entity.Property(e => e.PerTelefono)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("per_telefono");
            });

            modelBuilder.Entity<Turno>(entity =>
            {
                entity.HasKey(e => e.TurnId)
                    .HasName("PK_Table_1");

                entity.ToTable("turno");

                entity.Property(e => e.TurnId).HasColumnName("turn_id");

                entity.Property(e => e.TurnCodigo)
                    .HasMaxLength(60)
                    .HasColumnName("turn_codigo");

                entity.Property(e => e.TurnEstado).HasColumnName("turn_estado");

                entity.Property(e => e.TurnFecha)
                    .HasMaxLength(50)
                    .HasColumnName("turn_fecha");

                entity.Property(e => e.TurnIdPaciente).HasColumnName("turn_idPaciente");

                entity.HasOne(d => d.TurnEstadoNavigation)
                    .WithMany(p => p.Turnos)
                    .HasForeignKey(d => d.TurnEstado)
                    .HasConstraintName("FK_turno_estado");

                entity.HasOne(d => d.TurnIdPacienteNavigation)
                    .WithMany(p => p.Turnos)
                    .HasForeignKey(d => d.TurnIdPaciente)
                    .HasConstraintName("FK_turno_persona");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
