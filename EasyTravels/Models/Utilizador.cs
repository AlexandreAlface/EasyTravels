using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EasyTravelsAPI.Models;

[Table("Utilizador")]
public partial class Utilizador
{
    [Key]
    [Column("ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Nome { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Senha { get; set; } = null!;

    [Column("RoleID")]
    public int? RoleId { get; set; }

    [InverseProperty("Viajante")]
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    [ForeignKey("RoleId")]
    [InverseProperty("Utilizadors")]
    public virtual Role? Role { get; set; }

    [InverseProperty("Organizador")]
    public virtual ICollection<Viagem> Viagems { get; set; } = new List<Viagem>();
}
