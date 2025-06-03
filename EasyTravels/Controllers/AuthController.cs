using EasyTravelsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly string _key;
    private readonly EasyTravelsDbV2Context _context;

    public AuthController(IConfiguration config, EasyTravelsDbV2Context context)
    {
        _key = config["Jwt:Key"]; // Obtenha a chave secreta do arquivo appsettings.json
        _context = context;

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Buscar o usuário no banco de dados
        var user = await _context.Utilizadors
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Senha == request.Password);

        if (user == null)
        {
            return Unauthorized("Usuário ou senha inválidos");
        }

        // Gerar Token JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Nome), // Nome do usuário
                new Claim(ClaimTypes.Email, user.Email), // Email do usuário
                new Claim(ClaimTypes.Role, user.RoleId == 1 ? "1" : "2") // Papel do usuário
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "EasyTravelsAPI",
            Audience = "EasyTravelsClient",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new
        {
            Token = tokenHandler.WriteToken(token),
            Nome = user.Nome,
            RoleId = user.RoleId,
            Id = user.Id
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
