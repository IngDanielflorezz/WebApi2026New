
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebApi2026New.Models;

namespace WebApi2026New.Helpers
{
    // Configuración obtenida de appsettings
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiresMinutes { get; set; } = 60;
    }

    public class JwtHelper
    {
        private readonly JwtSettings _settings;

        public JwtHelper(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public (string token, DateTime expira) GenerarToken(Persona persona)
        {
            var expira = DateTime.UtcNow.AddMinutes(
                _settings.ExpiresMinutes
            );

            // Información que contendrá el token
            var claims = new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    persona.idPersona.ToString()
                ),
                new(
                    JwtRegisteredClaimNames.Email,
                    persona.correo
                ),
                new(
                    JwtRegisteredClaimNames.Name,
                    $"{persona.nombre} {persona.apellido}"
                )
            };

            // Clave para firmar el JWT
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key)
            );

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expira,
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                SigningCredentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                )
            };

            var token = new JsonWebTokenHandler()
                .CreateToken(descriptor);

            return (token, expira);
        }
    }
}
