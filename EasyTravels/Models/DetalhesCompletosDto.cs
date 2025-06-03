namespace EasyTravelsAPI.Models
{
    public class DetalhesCompletosDto
    {
        public int ItinerarioId { get; set; }
        public string Atividade { get; set; }
        public string Descricao { get; set; }
        public DateTime ItinerarioData { get; set; }

        // Informações da Viagem
        public int ViagemId { get; set; }
        public string ViagemDestino { get; set; }
        public DateTime ViagemDataInicio { get; set; }
        public DateTime ViagemDataFim { get; set; }

        // Informações do Organizador
        public int? OrganizadorId { get; set; }
        public string OrganizadorNome { get; set; }
    }
}
