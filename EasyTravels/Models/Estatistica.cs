using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EasyTravelsAPI.Models;

public partial class Estatistica
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("ViagemID")]
    public int? ViagemId { get; set; }

    public int Participantes { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Despesas { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? LocaisMaisVisitados { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Tipo { get; set; }

    public int DuracaoDias { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MediaCustoPorParticipante { get; set; }

    [ForeignKey("ViagemId")]
    [InverseProperty("Estatisticas")]
    public virtual Viagem? Viagem { get; set; }
}
