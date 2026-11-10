
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi2026New.Data;
using WebApi2026New.DTOs;
using WebApi2026New.Models;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PersonasController : ControllerBase
{
    private readonly AppDBContext _context;

    public PersonasController(AppDBContext context)
    {
        _context = context;
    }

    // GET: api/Personas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonaListaDto>>> GetPersonas()
    {
        return await _context.Personas
            .Select(p => new PersonaListaDto
            {
                idPersona = p.idPersona,
                nombre = p.nombre,
                apellido = p.apellido,
                correo = p.correo,
                edad = p.edad,
                totalDirecciones = p.Direcciones.Count
            })
            .ToListAsync();
    }

    // GET: api/Personas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PersonaDTO>> GetPersona(int id)
    {
        var persona = await _context.Personas
            .Where(p => p.idPersona == id)
            .Select(p => new PersonaDTO
            {
                idPersona = p.idPersona,
                nombre = p.nombre,
                apellido = p.apellido,
                correo = p.correo,
                direcciones = p.Direcciones.Select(d => new DireccionDTO
                {
                    idDireccion = d.idDireccion,
                    calle = d.calle,
                    ciudad = d.ciudad,
                    departamento = d.departamento,
                    codigoPostal = d.codigoPostal,
                    pais = d.pais
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (persona == null)
        {
            return NotFound();
        }

        return persona;
    }

    // PUT: api/Personas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPersona(
        int id, PersonaUpdateDTO dto)
    {
        var persona = await _context.Personas.FindAsync(id);

        if (persona == null)
        {
            return NotFound();
        }

        persona.nombre = dto.nombre;
        persona.apellido = dto.apellido;
        persona.edad = dto.edad;

        // No modificamos la contraseña ni el correo

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Personas
    [HttpPost]
    public async Task<ActionResult<PersonaReadDTO>> PostPersona(
        PersonaCreateDTO dto)
    {
        var correo = dto.correo.Trim().ToLowerInvariant();

        if (await _context.Personas.AnyAsync(p => p.correo == correo))
        {
            return Conflict(new
            {
                mensaje = "El correo ya está registrado."
            });
        }

        var persona = new Persona
        {
            nombre = dto.nombre,
            apellido = dto.apellido,
            correo = correo,
            edad = dto.edad,

            // Guardar hash BCrypt, nunca texto plano
            password = BCrypt.Net.BCrypt.HashPassword(dto.password)
        };

        _context.Personas.Add(persona);
        await _context.SaveChangesAsync();

        var resultado = new PersonaReadDTO
        {
            idPersona = persona.idPersona,
            nombre = persona.nombre,
            apellido = persona.apellido,
            correo = persona.correo
        };

        return CreatedAtAction(
            nameof(GetPersona),
            new { id = persona.idPersona },
            resultado
        );
    }

    // DELETE: api/Personas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePersona(int id)
    {
        var persona = await _context.Personas.FindAsync(id);

        if (persona == null)
        {
            return NotFound();
        }

        _context.Personas.Remove(persona);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
