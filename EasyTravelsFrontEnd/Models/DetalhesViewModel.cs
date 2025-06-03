namespace EasyTravelsFrontEnd.Models
{
    public class DetalhesViewModel
    {
        public ItinerarioViewModel Itinerario { get; set; }
        public ViagemViewModel Viagem { get; set; }
        public List<AlojamentoViewModel> Alojamentos { get; set; }
        public List<TransporteViewModel> Transportes { get; set; }
    }
}

