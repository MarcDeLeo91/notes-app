using Microsoft.AspNetCore.Mvc;
using NotesApi.Models;
using NotesApi.Services;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService) => _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var user = await _authService.Register(req.Email, req.Password);
            if (user == null) return BadRequest(new { message = "Usuario ya existe" });
            return Ok(new { message = "Usuario registrado" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var token = await _authService.Login(req.Email, req.Password);
            if (token == null) return Unauthorized(new { message = "Credenciales inválidas" });
            return Ok(new { token });
        }
    }

    public class RegisterRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
    public class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
}
