

namespace EasyTravelsFrontEnd.Models
{
    public class DashboardViewModel
    {
        public List<EstatisticaViewModel> Estatisticas { get; set; }
        public List<CompradoresPorViagemViewModel> CompradoresPorViagem { get; set; }
    }

    public class CompradoresPorViagemViewModel
    {
        public string Viagem { get; set; }
        public List<string> Compradores { get; set; }
    }
}