using EasyTravelsFrontEnd.Filters;
using EasyTravelsFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace EasyTravelsFrontEnd.Controllers
{

    [RoleAuthorize(2)]
    public class ItinerariosController : Controller
    {
        private readonly HttpClient _httpClient;
        private static readonly object _lock = new object();

        public ItinerariosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EasyTravelsAPI");
        }

        [HttpGet]
        public async Task<IActionResult> Pesquisar(string atividade, int? organizador, DateTime? dataInicio, DateTime? dataFim)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Você precisa estar autenticado para acessar esta página.";
                return RedirectToAction("Login", "Auth");
            }

            // Montar a URL com os parâmetros de filtro
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(atividade))
                queryParams.Add($"atividade={Uri.EscapeDataString(atividade)}");
            if (organizador.HasValue)
                queryParams.Add($"organizador={organizador.Value}");
            if (dataInicio.HasValue)
                queryParams.Add($"dataInicio={dataInicio.Value:yyyy-MM-dd}");
            if (dataFim.HasValue)
                queryParams.Add($"dataFim={dataFim.Value:yyyy-MM-dd}");

            var url = "https://localhost:7003/api/Itinerarios/ItinerariosCompletos";
            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

           
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao carregar os itinerários.";
                return View(new List<object>());
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var detalhesCompletos = JsonConvert.DeserializeObject<List<DetalhesCompletosViewModel>>(responseContent);

            // Organizar os dados na ViewBag para exibição
            ViewBag.Itinerarios = detalhesCompletos;
            return View(detalhesCompletos);
        }


        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get Itinerario
            var itinerarioRequest = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7003/api/Itinerarios/{id}");
            itinerarioRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var itinerarioResponse = await _httpClient.SendAsync(itinerarioRequest);
            if (!itinerarioResponse.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao carregar detalhes do itinerário.";
                return RedirectToAction("Pesquisar");
            }
            var itinerarioContent = await itinerarioResponse.Content.ReadAsStringAsync();
            var itinerario = JsonConvert.DeserializeObject<ItinerarioViewModel>(itinerarioContent);

            // Get Viagem associated with Itinerario
            var viagemRequest = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7003/api/Viagems/{itinerario.ViagemId}");
            viagemRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var viagemResponse = await _httpClient.SendAsync(viagemRequest);
            if (!viagemResponse.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao carregar detalhes da viagem.";
                return RedirectToAction("Pesquisar");
            }
            var viagemContent = await viagemResponse.Content.ReadAsStringAsync();
            var viagem = JsonConvert.DeserializeObject<ViagemViewModel>(viagemContent);

            // Get Alojamentos by ViagemId
            var alojamentosRequest = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7003/api/Alojamentoes/GetAlojamentosByViagemId?viagemId={viagem.Id}");
            alojamentosRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var alojamentosResponse = await _httpClient.SendAsync(alojamentosRequest);
            var alojamentos = new List<AlojamentoViewModel>();
            if (alojamentosResponse.IsSuccessStatusCode)
            {
                var alojamentosContent = await alojamentosResponse.Content.ReadAsStringAsync();
                alojamentos = JsonConvert.DeserializeObject<List<AlojamentoViewModel>>(alojamentosContent);
            }

            // Get Transportes by ViagemId
            var transportesRequest = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7003/api/Transportes/GetTransportesByViagemId?viagemId={viagem.Id}");
            transportesRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var transportesResponse = await _httpClient.SendAsync(transportesRequest);
            var transportes = new List<TransporteViewModel>();
            if (transportesResponse.IsSuccessStatusCode)
            {
                var transportesContent = await transportesResponse.Content.ReadAsStringAsync();
                transportes = JsonConvert.DeserializeObject<List<TransporteViewModel>>(transportesContent);
            }

            // Create ViewModel
            var detalhesViewModel = new DetalhesViewModel
            {
                Itinerario = itinerario,
                Viagem = viagem,
                Alojamentos = alojamentos,
                Transportes = transportes
            };

            return View(detalhesViewModel);
        }



        // Página de Compra
        [HttpGet]
        public async Task<IActionResult> ComprarAsync(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Fetch the selected trip from the API
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7003/api/Viagems/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao carregar a viagem selecionada.";
                return RedirectToAction("Pesquisar");
            }

            var viagemContent = await response.Content.ReadAsStringAsync();
            var viagem = JsonConvert.DeserializeObject<ViagemViewModel>(viagemContent);

            if (viagem == null)
            {
                ViewBag.Error = "Viagem não encontrada.";
                return RedirectToAction("Pesquisar");
            }

            return View(viagem);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarCompra(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = GetViajanteId();
            if (userId == 0)
            {
                ViewBag.Error = "Erro ao identificar o utilizador.";
                return RedirectToAction("Login", "Auth");
            }

            var reserva = new ReservaViewModel
            {
                ViagemId = id,
                ViajanteId = userId,
                Tipo = "Individual",  // Tipo pode ser sempre "Individual" ou pode ser alterado conforme necessidade
                Status = "Confirmada"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7003/api/Reservas")
            {
                Content = new StringContent(JsonConvert.SerializeObject(reserva), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao confirmar a compra.";
                return RedirectToAction("Comprar", new { id });
            }

            // Agora, vamos buscar as informações da viagem comprada
            var viagemRequest = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7003/api/Viagems/{id}");
            viagemRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var viagemResponse = await _httpClient.SendAsync(viagemRequest);

            if (!viagemResponse.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao obter informações da viagem.";
                return RedirectToAction("Pesquisar");
            }

            var viagemContent = await viagemResponse.Content.ReadAsStringAsync();
            var viagem = JsonConvert.DeserializeObject<ViagemViewModel>(viagemContent);

            // Agora, coletamos as informações para estatísticas
            var estatisticas = new EstatisticaViewModel
            {
                ViagemId = viagem.Id.ToString(),
                Participantes = 1, 
                Despesas = (decimal) viagem.CustoTotal,  
                LocaisMaisVisitados = viagem.Destino,
                Tipo = "Individual",  
                DuracaoDias = (viagem.DataFim - viagem.DataInicio).Days, 
            };

            // Enviamos os dados para a API que vai salvar as estatísticas
            var estatisticasRequest = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7003/api/Estatisticas")
            {
                Content = new StringContent(JsonConvert.SerializeObject(estatisticas), Encoding.UTF8, "application/json")
            };
            estatisticasRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var estatisticasResponse = await _httpClient.SendAsync(estatisticasRequest);

            if (!estatisticasResponse.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao salvar as estatísticas.";
                return RedirectToAction("Pesquisar");
            }

            ViewBag.SuccessMessage = "Reserva confirmada com sucesso e estatísticas salvas!";
            return RedirectToAction("Pesquisar");
        }



        private int GetViajanteId()
        {
            if (TempData["UserId"] != null)
            {
                TempData.Keep("UserId");
                return int.Parse(TempData["UserId"].ToString());
            }

            return 0;
        }
    }
}
