using System.ComponentModel.DataAnnotations;

namespace EasyTravelsFrontEnd.Models
{
    internal class UtilizadorDTO
    {
        public int Id { get; set; }
 
        public string Nome { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public int? RoleId { get; set; }
    }
}