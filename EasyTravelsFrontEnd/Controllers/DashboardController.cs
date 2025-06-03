using EasyTravelsFrontEnd.Filters;
using EasyTravelsFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EasyTravelsFrontEnd.Controllers
{
    [RoleAuthorize(1)]
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EasyTravelsAPI");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Requisição das estatísticas
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7003/api/Estatisticas/EstatisticasComViagem");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao carregar as estatísticas.";
                return View(new List<EstatisticaViewModel>());
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var estatisticas = JsonConvert.DeserializeObject<List<EstatisticaViewModel>>(responseContent);

            // Requisição dos compradores por viagem
            var compradoresRequest = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7003/api/Reservas/CompradoresPorViagem");
            compradoresRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var compradoresResponse = await _httpClient.SendAsync(compradoresRequest);
            if (!compradoresResponse.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao carregar os dados dos compradores.";
                return View(new DashboardViewModel { Estatisticas = estatisticas, CompradoresPorViagem = new List<CompradoresPorViagemViewModel>() });
            }

            var compradoresContent = await compradoresResponse.Content.ReadAsStringAsync();
            var compradoresPorViagem = JsonConvert.DeserializeObject<List<CompradoresPorViagemViewModel>>(compradoresContent);

            // Atualizar o tipo de viagem para "Grupo" caso tenha mais de um comprador
            foreach (var estatistica in estatisticas)
            {
                var compradores = compradoresPorViagem.FirstOrDefault(c => c.Viagem == estatistica.Viagem.Destino)?.Compradores;
                if (compradores != null && compradores.Count > 1)
                {
                    estatistica.Tipo = "Grupo"; // Altera o tipo para "Grupo"
                }
            }

            // Enviar dados para a view
            var viewModel = new DashboardViewModel
            {
                Estatisticas = estatisticas,
                CompradoresPorViagem = compradoresPorViagem
            };

            return View(viewModel);
        }


    }
}
