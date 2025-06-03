using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EasyTravelsAPI.Models;

[Table("Reserva")]
public partial class Reserva
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("ViagemID")]
    public int? ViagemId { get; set; }

    [Column("ViajanteID")]
    public int? ViajanteId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Tipo { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [ForeignKey("ViagemId")]
    [InverseProperty("Reservas")]
    public virtual Viagem? Viagem { get; set; }

    [ForeignKey("ViajanteId")]
    [InverseProperty("Reservas")]
    public virtual Utilizador? Viajante { get; set; }
}
