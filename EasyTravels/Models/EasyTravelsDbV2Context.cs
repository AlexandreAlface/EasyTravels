using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EasyTravelsAPI.Models;

public partial class EasyTravelsDbV2Context : DbContext
{
    public EasyTravelsDbV2Context()
    {
    }

    public EasyTravelsDbV2Context(DbContextOptions<EasyTravelsDbV2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Alojamento> Alojamentos { get; set; }

    public virtual DbSet<Estatistica> Estatisticas { get; set; }

    public virtual DbSet<Itinerario> Itinerarios { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transporte> Transportes { get; set; }

    public virtual DbSet<Utilizador> Utilizadors { get; set; }

    public virtual DbSet<Viagem> Viagems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\localDB1;Initial Catalog=EasyTravelsDbV2;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alojamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Alojamen__3214EC27012C8E10");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Viagem).WithMany(p => p.Alojamentos).HasConstraintName("FK__Alojament__Viage__5535A963");
        });

        modelBuilder.Entity<Estatistica>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Estatist__3214EC27FD73905D");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Viagem).WithMany(p => p.Estatisticas).HasConstraintName("FK__Estatisti__Viage__5CD6CB2B");
        });

        modelBuilder.Entity<Itinerario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Itinerar__3214EC2713BB09EC");

            entity.HasOne(d => d.Viagem).WithMany(p => p.Itinerarios).HasConstraintName("FK__Itinerari__Viage__5FB337D6");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasKey(e => e.Id).HasName("PK__Reserva__3214EC27CEF6FD7D");

            entity.HasOne(d => d.Viagem).WithMany(p => p.Reservas).HasConstraintName("FK__Reserva__ViagemI__5812160E");

            entity.HasOne(d => d.Viajante).WithMany(p => p.Reservas).HasConstraintName("FK__Reserva__Viajant__59063A47");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A749D185A");

            entity.Property(e => e.RoleId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Transporte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transpor__3214EC27A1EB72BE");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Viagem).WithMany(p => p.Transportes).HasConstraintName("FK__Transport__Viage__52593CB8");
        });

        modelBuilder.Entity<Utilizador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Utilizad__3214EC27AB68A502");

            entity.Property(e => e.Id).ValueGeneratedOnAdd(); 

            entity.HasOne(d => d.Role).WithMany(p => p.Utilizadors).HasConstraintName("FK__Utilizado__RoleI__4BAC3F29");
        });

        modelBuilder.Entity<Viagem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Viagem__3214EC27C80BF850");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Organizador).WithMany(p => p.Viagems).HasConstraintName("FK__Viagem__Organiza__4E88ABD4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
