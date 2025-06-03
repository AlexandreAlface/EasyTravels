namespace EasyTravelsAPI.Models
{
    internal class ViagemDTO
    {
        public int Id { get; set; }
        public string Destino { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public object Itinerarios { get; set; }
        public object Alojamentos { get; set; }
        public object Transportes { get; set; }
        public object Organizador { get; set; }
    }
}