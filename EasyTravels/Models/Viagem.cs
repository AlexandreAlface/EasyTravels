using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EasyTravelsAPI.Models;

[Table("Viagem")]
public partial class Viagem
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Destino { get; set; } = null!;

    public DateOnly DataInicio { get; set; }

    public DateOnly DataFim { get; set; }

    [Column("OrganizadorID")]
    public int? OrganizadorId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? CustoTotal { get; set; }

    [InverseProperty("Viagem")]
    public virtual ICollection<Alojamento> Alojamentos { get; set; } = new List<Alojamento>();

    [InverseProperty("Viagem")]
    public virtual ICollection<Estatistica> Estatisticas { get; set; } = new List<Estatistica>();

    [InverseProperty("Viagem")]
    public virtual ICollection<Itinerario> Itinerarios { get; set; } = new List<Itinerario>();

    [ForeignKey("OrganizadorId")]
    [InverseProperty("Viagems")]
    public virtual Utilizador? Organizador { get; set; }

    [InverseProperty("Viagem")]
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    [InverseProperty("Viagem")]
    public virtual ICollection<Transporte> Transportes { get; set; } = new List<Transporte>();
}
