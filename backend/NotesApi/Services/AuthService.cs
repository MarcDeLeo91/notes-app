using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<User?> Register(string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
                return null;

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<string?> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            // Leer la clave configurada (preferentemente en base64)
            var configuredKey = _config["Jwt:KeyBase64"] ?? _config["Jwt:Key"];
            if (string.IsNullOrEmpty(configuredKey))
            {
                throw new InvalidOperationException("JWT signing key is not configured. Set 'Jwt:KeyBase64' (recommended) or 'Jwt:Key' in configuration.");
            }

            byte[] keyBytes;
            try
            {
                // Intentar decodificar como base64 (recomendado)
                keyBytes = Convert.FromBase64String(configuredKey);
            }
            catch (FormatException)
            {
                // Si no es base64, usar UTF8
                keyBytes = Encoding.UTF8.GetBytes(configuredKey);
            }

            // Validar que la clave sea suficientemente larga para HMAC-SHA256
            if (keyBytes.Length * 8 <= 256)
            {
                throw new InvalidOperationException("Configured JWT key is too short. Provide a key longer than 256 bits (recommended: 512 bits). Use a base64-encoded value in configuration under 'Jwt:KeyBase64'.");
            }

            var signingKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Token válido por 7 días
                NotBefore = DateTime.UtcNow, // Token válido inmediatamente
                IssuedAt = DateTime.UtcNow, // Fecha de emisión
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"] // Validar el Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}