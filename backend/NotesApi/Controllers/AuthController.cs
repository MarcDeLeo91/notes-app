using Microsoft.AspNetCore.Mvc;
using NotesApi.Models;
using NotesApi.Services;
using System.ComponentModel.DataAnnotations;

namespace NotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="req">Datos de registro (email y contraseña).</param>
        /// <returns>Mensaje de éxito o error.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _authService.Register(req.Email, req.Password);
                if (user == null)
                {
                    _logger.LogWarning("Intento de registro fallido: el usuario {Email} ya existe.", req.Email);
                    return BadRequest(new { success = false, message = "El usuario ya existe." });
                }

                _logger.LogInformation("Usuario registrado exitosamente: {Email}", req.Email);
                return Ok(new { success = true, message = "Usuario registrado exitosamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el usuario: {Email}", req.Email);
                return StatusCode(500, new { success = false, message = "Error interno del servidor.", details = ex.Message });
            }
        }

        /// <summary>
        /// Inicia sesión y genera un token JWT.
        /// </summary>
        /// <param name="req">Credenciales de inicio de sesión (email y contraseña).</param>
        /// <returns>Token JWT o error.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = await _authService.Login(req.Email, req.Password);
                if (token == null)
                {
                    _logger.LogWarning("Intento de inicio de sesión fallido para el usuario: {Email}", req.Email);
                    return Unauthorized(new { success = false, message = "Credenciales inválidas." });
                }

                _logger.LogInformation("Inicio de sesión exitoso para el usuario: {Email}", req.Email);
                return Ok(new { success = true, token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión para el usuario: {Email}", req.Email);
                return StatusCode(500, new { success = false, message = "Error interno del servidor.", details = ex.Message });
            }
        }
    }

    /// <summary>
    /// Modelo para la solicitud de registro.
    /// </summary>
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Modelo para la solicitud de inicio de sesión.
    /// </summary>
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}