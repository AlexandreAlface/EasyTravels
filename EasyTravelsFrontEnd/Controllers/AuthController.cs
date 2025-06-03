using EasyTravelsFrontEnd.Filters;
using EasyTravelsFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EasyTravelsAPI");
        }


        // Página de Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Processo de Login
        [HttpPost]
        public async Task<IActionResult> LoginToApp(string email, string password)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email e senha são obrigatórios.";
                return View("Login");
            }

            var loginData = new
            {
                Email = email,
                Password = password
            };

            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(loginData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("https://localhost:7003/api/Auth/Login", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Credenciais inválidas. Tente novamente.";
                return View("Login");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(responseContent);
            var token = data["token"]?.ToString();
            var userId = data["id"]?.ToString(); // Captura o ID do utilizador
            var roleId = data["roleId"]?.ToString(); // vem null

            if (token == null || userId == null || roleId == null)
            {
                ViewBag.Error = "Erro ao processar o login.";
                return View("Login");
            }

            // Guardar o token na sessão
            HttpContext.Session.SetString("AuthToken", token);
            HttpContext.Session.SetString("RoleId", roleId);

            // Guardar o ID do utilizador no TempData
            TempData["UserId"] = userId;
            TempData.Keep("UserId");

            TempData["RoleId"] = roleId;
            TempData.Keep("RoleId");

            return RedirectToAction("Menu");
        }

        // Página de Registo
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Processo de Registo
        [HttpPost]
        public async Task<IActionResult> Register(string nome, string email, string senha, int roleId)
        {
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                ViewBag.Error = "Todos os campos são obrigatórios.";
                return View();
            }

            var registerData = new UtilizadorDTO
            {
                Nome = nome,
                Email = email,
                Senha = senha,
                RoleId = roleId
            };

            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(registerData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("https://localhost:7003/api/Utilizadors", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Erro ao criar conta. Tente novamente.";
                return View();
            }

            return RedirectToAction("Login");
        }

        // Página do Menu Principal
        [HttpGet]
        public IActionResult Menu()
        {
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var roleId = HttpContext.Session.GetString("RoleId");
            ViewBag.RoleId = roleId;

            return View();
        }

        // Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}