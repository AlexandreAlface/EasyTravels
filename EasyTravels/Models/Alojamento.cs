using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using EasyTravelsAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

namespace EasyTravelsAPI.Models;

[Table("Alojamento")]
public partial class Alojamento
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("ViagemID")]
    public int? ViagemId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Nome { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Custo { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Endereco { get; set; }

    [ForeignKey("ViagemId")]
    [InverseProperty("Alojamentos")]
    public virtual Viagem? Viagem { get; set; }
}