namespace EasyTravelsFrontEnd.Models
{
    public class ReservaViewModel
    {
        public int Id { get; set; }
        public string Tipo { get; set; } // Tipo de reserva: "Individual", "Grupo", etc.
        public string Status { get; set; } // Status da reserva: "Confirmada", "Pendente", etc.

        public int? ViagemId { get; set; } // ID da viagem associada
        public int? ViajanteId { get; set; } // ID do viajante associado

        public ViagemViewModel? Viagem { get; set; } // Detalhes completos da viagem
        public UtilizadorViewModel? Viajante { get; set; } // Detalhes do viajante (opcional)
    }

    public class ViagemViewModel
    {
        public int Id { get; set; }
        public string Destino { get; set; } // Destino da viagem
        public DateTime DataInicio { get; set; } // Data de início da viagem
        public DateTime DataFim { get; set; } // Data de fim da viagem
        public decimal? CustoTotal { get; set; } // Custo total da viagem

        public UtilizadorViewModel Organizador { get; set; } // Detalhes do organizador
        public List<ItinerarioViewModel> Itinerarios { get; set; } = new(); // Lista de itinerários
        public List<AlojamentoViewModel> Alojamentos { get; set; } = new(); // Lista de alojamentos
        public List<TransporteViewModel> Transportes { get; set; } = new(); // Lista de transportes
    }

    public class ItinerarioViewModel
    {
        public int Id { get; set; }
        public string Atividade { get; set; } // Nome da atividade
        public string Descricao { get; set; } // Descrição da atividade
        public DateTime Data { get; set; } // Data da atividade

        public int? ViagemId { get; set; } // ID da viagem associada
    }

    public class UtilizadorViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } // Nome do utilizador
        public string Email { get; set; } // Email do utilizador (opcional)
    }

    public class AlojamentoViewModel
    {
        public int Id { get; set; } // Identificador do alojamento
        public int ViagemId { get; set; } // ID da viagem associada
        public string Nome { get; set; } // Nome do alojamento
        public decimal Custo { get; set; } // Custo do alojamento
        public string Endereco { get; set; } // Endereço do alojamento
    }

    public class TransporteViewModel
    {
        public int Id { get; set; } // Identificador do transporte
        public int ViagemId { get; set; } // ID da viagem associada
        public string Tipo { get; set; } // Tipo de transporte (e.g., Avião, Comboio)
        public decimal Custo { get; set; } // Custo do transporte
        public string Detalhes { get; set; } // Detalhes adicionais sobre o transporte
    }
}
