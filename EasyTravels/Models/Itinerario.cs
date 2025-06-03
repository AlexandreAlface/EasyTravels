using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EasyTravelsAPI.Models;

[Table("Itinerario")]
public partial class Itinerario
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("ViagemID")]
    public int? ViagemId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Atividade { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Descricao { get; set; }

    public DateOnly Data { get; set; }

    [ForeignKey("ViagemId")]
    [InverseProperty("Itinerarios")]
    public virtual Viagem? Viagem { get; set; }
}
