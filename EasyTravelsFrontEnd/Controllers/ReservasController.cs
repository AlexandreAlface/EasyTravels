using EasyTravelsFrontEnd.Filters;
using EasyTravelsFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace EasyTravelsFrontEnd.Controllers
{
    [RoleAuthorize(1, 2)]
    public class ReservasController : Controller
    {
        private readonly HttpClient _httpClient;

        public ReservasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GerirReservas()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Você precisa estar autenticado para acessar esta página.";
                return RedirectToAction("Login", "Auth");
            }

            // Get the authenticated user's ID and Role
            var userId = 0;
            var roleId = 0;

            if (TempData["UserId"] != null && TempData["RoleId"] != null)
            {
                TempData.Keep("UserId");
                TempData.Keep("RoleId");
                userId = int.Parse(TempData["UserId"].ToString());
                roleId = int.Parse(TempData["RoleId"].ToString());
            }

            if (userId == 0)
            {
                ViewBag.Error = "Não foi possível obter o ID do utilizador.";
                return RedirectToAction("Login", "Auth");
            }

            HttpResponseMessage response;

            // Check if the user is an organizer
            if (roleId == 1) // Role ID 1: Organizer
            {
                // Fetch all reservations for trips organized by the user
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7003/api/Reservas/ByOrganizador/{userId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await _httpClient.SendAsync(request);
            }
            else if (roleId == 2) // Role ID 2: Traveler
            {
                // Fetch reservations for the logged-in traveler
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7003/api/Reservas/ByViajante/{userId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await _httpClient.SendAsync(request);
            }
            else
            {
                ViewBag.Error = "Acesso negado.";
                return RedirectToAction("Login", "Auth");
            }

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao carregar as reservas. Tente novamente.";
                return View(new List<ReservaViewModel>());
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var reservas = JsonConvert.DeserializeObject<List<ReservaViewModel>>(responseContent);

            return View(reservas);
        }


        [HttpGet]
        public async Task<IActionResult> Alterar(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Você precisa estar autenticado para acessar esta página.";
                return RedirectToAction("Login", "Auth");
            }

            // Obter dados da reserva
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7003/api/Reservas/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao carregar a reserva.";
                return RedirectToAction("GerirReservas");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var reserva = JsonConvert.DeserializeObject<ReservaViewModel>(responseContent);

            return View("Editar", reserva);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(ReservaViewModel reserva)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Você precisa estar autenticado para realizar esta ação.";
                return RedirectToAction("Login", "Auth");
            }

            var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:7003/api/Reservas/{reserva.Id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(JsonConvert.SerializeObject(reserva), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao atualizar a reserva.";
                return View(reserva);
            }

            ViewBag.SuccessMessage = "Reserva atualizada com sucesso!";
            return RedirectToAction("GerirReservas");
        }


        [HttpPost]
        public async Task<IActionResult> Cancelar(int id)
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"https://localhost:7003/api/Reservas/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao cancelar a reserva.";
                return RedirectToAction("GerirReservas");
            }

            TempData["SuccessMessage"] = "Reserva cancelada com sucesso!";
            return RedirectToAction("GerirReservas");
        }
    }
}

