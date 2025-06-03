using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EasyTravelsAPI.Models;

public partial class Role
{
    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string RoleType { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<Utilizador> Utilizadors { get; set; } = new List<Utilizador>();
}
