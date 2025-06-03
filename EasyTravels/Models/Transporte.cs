using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EasyTravelsAPI.Models;

[Table("Transporte")]
public partial class Transporte
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("ViagemID")]
    public int? ViagemId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Tipo { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Custo { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Detalhes { get; set; }

    [ForeignKey("ViagemId")]
    [InverseProperty("Transportes")]
    public virtual Viagem? Viagem { get; set; }
}
