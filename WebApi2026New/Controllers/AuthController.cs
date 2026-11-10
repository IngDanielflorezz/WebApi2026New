
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi2026New.Data;
using WebApi2026New.DTOs;
using WebApi2026New.Helpers;
using WebApi2026New.Models;

namespace WebApi2026New.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly JwtHelper _jwt;

        public AuthController(AppDBContext context, JwtHelper jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        // POST: api/Auth/registro
        [HttpPost("registro")]
        public async Task<ActionResult<PersonaReadDTO>> Registro(
            RegistroDTO dto)
        {
            var correo = dto.correo.Trim().ToLowerInvariant();

            if (await _context.Personas.AnyAsync(p => p.correo == correo))
                return Conflict(new
                {
                    mensaje = "El correo ya está registrado."
                });

            var persona = new Persona
            {
                nombre = dto.nombre,
                apellido = dto.apellido,
                edad = dto.edad,
                correo = correo,
                password = BCrypt.Net.BCrypt.HashPassword(dto.password)
            };

            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();

            return Ok(new PersonaReadDTO
            {
                idPersona = persona.idPersona,
                nombre = persona.nombre,
                apellido = persona.apellido,
                edad = persona.edad,
                correo = persona.correo
            });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginRespuestaDTO>> Login(
            LoginDTO dto)
        {
            var correo = dto.correo.Trim().ToLowerInvariant();

            var persona = await _context.Personas
                .FirstOrDefaultAsync(p => p.correo == correo);

            if (persona is null ||
                !VerificarPassword(dto.password, persona.password))
            {
                return Unauthorized(new
                {
                    mensaje = "Credenciales inválidas."
                });
            }

            var (token, expira) = _jwt.GenerarToken(persona);

            return Ok(new LoginRespuestaDTO
            {
                token = token,
                expira = expira,
                idPersona = persona.idPersona,
                nombre = $"{persona.nombre} {persona.apellido}"
            });
        }

        // GET: api/Auth/yo
        [HttpGet("yo")]
        [Authorize]
        public IActionResult Yo()
        {
            return Ok(new
            {
                idPersona = User.FindFirst("sub")?.Value,
                correo = User.FindFirst("email")?.Value,
                nombre = User.Identity?.Name
            });
        }

        private static bool VerificarPassword(
            string password, string hashGuardado)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(
                    password, hashGuardado
                );
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return false;
            }
        }
    }
}
