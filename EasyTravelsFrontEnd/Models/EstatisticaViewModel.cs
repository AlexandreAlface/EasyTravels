namespace EasyTravelsFrontEnd.Models
{
    public class EstatisticaViewModel
    {
        public string ViagemId { get; set; } // Nome ou Destino da Viagem
        public ViagemViewModel Viagem { get; set; }
        public int Participantes { get; set; }
        public decimal Despesas { get; set; }
        public string LocaisMaisVisitados { get; set; }
        public string Tipo { get; set; } // Individual ou Grupo
        public int DuracaoDias { get; set; }
        public decimal MediaCustoPorParticipante { get; set; }
    }
}
