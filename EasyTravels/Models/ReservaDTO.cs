namespace EasyTravelsAPI.Models
{
    internal class ReservaDTO
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Tipo { get; set; }
        public object Viagem { get; set; }
    }
}